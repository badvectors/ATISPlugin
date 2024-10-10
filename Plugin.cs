using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using vatsys;
using vatsys.Plugin;
using Timer = System.Timers.Timer;

namespace ATISPlugin
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        public string Name => "ATIS Editor";
        public static string DisplayName => "ATIS Editor";

        private static readonly string MetarUri = "https://metar.vatsim.net/metar.php?id=";

        public static readonly Version Version = new Version(2, 3);
        private static readonly string VersionUrl = "https://raw.githubusercontent.com/badvectors/ATISPlugin/master/Version.json";
        private static readonly string ZuluUrl = "https://raw.githubusercontent.com/badvectors/ATISPlugin/master/Zulu.json";

        private static readonly HttpClient Client = new HttpClient();

        private static CustomToolStripMenuItem ATISMenu;

        public static ATISControl ATIS1;
        public static ATISControl ATIS2;
        public static ATISControl ATIS3;
        public static ATISControl ATIS4;

        private static EditorWindow Editor;

        public static string DatasetPath { get; set; }
        public static ATIS ATISData { get; set; }
        public static Sectors Sectors { get; set; }
        public static Airspace Airspace { get; set; }
        public static List<ZuluInfo> ZuluInfo { get; set; } = new List<ZuluInfo>();

        public static SoundPlayer SoundPlayer { get; set; } = new SoundPlayer();
        private static Timer METARTimer { get; set; } = new Timer();
        private static Timer BroadcastTimer { get; set; } = new Timer();
        public static List<ATISAudio> ToBroadcast { get; set; } = new List<ATISAudio>();

        public static List<VSCSFrequency> Frequencies { get; set; } = new List<VSCSFrequency>();

        public Plugin()
        {
            vatsys.ATIS.Disable();

            try
            {
                Network.Connected += OnUpdate;
                Network.Disconnected += Network_Disconnected;

                Audio.VSCSFrequenciesChanged += Audio_VSCSFrequenciesChanged;
                Audio.FrequencyErrorStateChanged += Audio_VSCSFrequenciesChanged;
                Network.PrimaryFrequencyChanged += Audio_VSCSFrequenciesChanged;

                ATISMenu = new CustomToolStripMenuItem(CustomToolStripMenuItemWindowType.Main, CustomToolStripMenuItemCategory.Windows, new ToolStripMenuItem(DisplayName));
                ATISMenu.Item.Click += ATISMenu_Click;
                MMI.AddCustomMenuItem(ATISMenu);

                GetSettings();

                if (DatasetPath == null)
                {
                    Errors.Add(new Exception("Could not load vatSys settings."), DisplayName);
                    return;
                }

                GetData();

                if (Airspace == null)
                {
                    Errors.Add(new Exception("Could not load Airspace data."), DisplayName);
                    return;
                }

                if (Sectors == null)
                {
                    Errors.Add(new Exception("Could not load Sectors data."), DisplayName);
                    return;
                }

                if (ATISData == null)
                {
                    Errors.Add(new Exception("Could not load ATIS data."), DisplayName);
                    return;
                }

                var directory = Path.Combine(DatasetPath, "Temp");

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var toDelete = Directory
                    .EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => Path.GetExtension(s).TrimStart('.').ToLowerInvariant() == "wav");

                foreach (var file in toDelete)
                {
                    if (file.Contains("AIS.wav")) continue;
                    File.Delete(file);
                }

                ATIS1 = new ATISControl(0);
                ATIS2 = new ATISControl(1);
                ATIS3 = new ATISControl(2);
                ATIS4 = new ATISControl(3);

                ATIS1.StatusChanged += OnUpdate;
                ATIS2.StatusChanged += OnUpdate;
                ATIS3.StatusChanged += OnUpdate;
                ATIS4.StatusChanged += OnUpdate;

                METARTimer.Elapsed += new ElapsedEventHandler(METARTimer_Elapsed);
                METARTimer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
                METARTimer.AutoReset = false;
                METARTimer.Start();

                BroadcastTimer.Elapsed += new ElapsedEventHandler(BroadcastTimer_Elasped);
                BroadcastTimer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
                BroadcastTimer.AutoReset = false;
                BroadcastTimer.Start();
            }
            catch (Exception ex)
            {
                Errors.Add(new Exception(ex.Message), DisplayName);
                if (ex.InnerException != null) Errors.Add(new Exception(ex.InnerException.Message), DisplayName);
            }

            _ = GetZuluInfo();

            _ = CheckVersion();
        }

        private static async Task CheckVersion()
        {
            try
            {
                var response = await Client.GetStringAsync(VersionUrl);

                var version = JsonConvert.DeserializeObject<Version>(response);

                if (version.Major == Version.Major && version.Minor == Version.Minor) return;

                Errors.Add(new Exception("A new version of the plugin is available."), DisplayName);
            }
            catch { }
        }

        private static async Task GetZuluInfo()
        {
            try
            {
                var response = await Client.GetStringAsync(ZuluUrl);

                var zuluInfo = JsonConvert.DeserializeObject<ZuluInfo[]>(response);

                foreach (var info in zuluInfo)
                {
                    ZuluInfo.Add(info);
                }
            }
            catch (Exception ex) 
            {
                Errors.Add(new Exception(ex.Message), DisplayName);
            }
        }

        private async void BroadcastTimer_Elasped(object sender, ElapsedEventArgs e)
        {
            var toBroadcast = ToBroadcast.ToList();

            foreach (var atb in toBroadcast)
            {
                var atis = ToBroadcast.FirstOrDefault(x => x.Id == atb.Id);

                if (atis != null) ToBroadcast.Remove(atis);

                if (!Network.GetATISConnected(atb.ATISIndex)) continue;

                try
                {
                    await AFV.AddOrUpdateATISBot(atb.Audio, atb.ATISIndex, atb.Callsign, atb.Frequency, atb.VisPoint, atb.Duration);
                }
                catch (Exception ex)
                {
                    Errors.Add(new Exception($"Could not start voice ATIS: {ex.Message}"), Plugin.DisplayName);
                }
            }

            BroadcastTimer.Start();
        }

        private async void METARTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var changes = false;

            try
            {
                var atis1Updated = await ATIS1.UpdateMetar();

                if (atis1Updated)
                {
                    changes = true;
                    OnMETARUpdate(1);
                }
            }
            catch { }

            try
            {
                var atis2Updated = await ATIS2.UpdateMetar();

                if (atis2Updated)
                {
                    changes = true;
                    OnMETARUpdate(2);
                }
            }
            catch { }

            try
            {
                var atis3Updated = await ATIS3.UpdateMetar();

                if (atis3Updated)
                {
                    changes = true;
                    OnMETARUpdate(3);
                }
            }
            catch { }

            try
            {
                var atis4Updated = await ATIS4.UpdateMetar();

                if (atis4Updated)
                {
                    changes = true;
                    OnMETARUpdate(4);
                }
            }
            catch { }

            if (changes) PlayUpdateSound();

            METARTimer.Start();
        }

        private void Audio_VSCSFrequenciesChanged(object sender, EventArgs e)
        {
            Frequencies.Clear();

            foreach (var frequency in Audio.VSCSFrequencies)
            {
                if (!frequency.Transmit) continue;

                Frequencies.Add(frequency);
            }
        }

        private async void Network_Disconnected(object sender, EventArgs e)
        {
            if (Editor != null && !Editor.IsDisposed)
            {
                Editor.Dispose();
            }

            ToBroadcast.Clear();
            
            await ATIS1?.Delete(false);
            await ATIS2?.Delete(false);
            await ATIS3?.Delete(false);
            await ATIS4?.Delete(false);
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            Editor?.RefreshEvent.Invoke(this, null);
        }

        private void PlayUpdateSound()
        {
            var sound = Path.Combine(Helpers.GetProgramFolder(), "wav", "AIS.wav");

            SoundPlayer.SoundLocation = sound;

            SoundPlayer.Play();
        }

        private void OnMETARUpdate(int number)
        {
            ShowEditorWindow();

            Editor.Change(number);

            Editor?.RefreshEvent.Invoke(this, null);
        }

        private void GetSettings()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

            if (!configuration.HasFile) return;

            if (!File.Exists(configuration.FilePath)) return;

            var config = File.ReadAllText(configuration.FilePath);

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(config);

            XmlElement root = doc.DocumentElement;

            var userSettings = root.SelectSingleNode("userSettings");

            var settings = userSettings.SelectSingleNode("vatsys.Properties.Settings");

            foreach (XmlNode node in settings.ChildNodes)
            {
                if (node.Attributes.GetNamedItem("name").Value == "DatasetPath")
                {
                    DatasetPath = node.InnerText;
                    break;
                }
            }
        }

        private void GetData()
        {
            ATISData = (ATIS)LoadXML(DatasetPath + "\\ATIS.xml", typeof(ATIS));
            Sectors = (Sectors)LoadXML(DatasetPath + "\\Sectors.xml", typeof(Sectors));
            Airspace = (Airspace)LoadXML(DatasetPath + "\\Airspace.xml", typeof(Airspace));
        }

        public static async Task<string> GetMetar(string icao)
        {
            var response = await Client.GetAsync($"{MetarUri}{icao}");

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public object LoadXML(string filePath, Type type)
        {
            if (!File.Exists(filePath)) return null;

            var fileContents = File.ReadAllText(filePath);

            var serializer = new XmlSerializer(type);

            using (StringReader reader = new StringReader(fileContents))
            {
                return serializer.Deserialize(reader);
            }
        }

        private void ATISMenu_Click(object sender, EventArgs e)
        {
            ShowEditorWindow();
        }

        private static void ShowEditorWindow()
        {
            if (ATISData == null || Sectors == null || Airspace == null)
            {
                Errors.Add(new Exception("Plugin was not started due to missing data."), DisplayName);

                return;
            }

            MMI.InvokeOnGUI((MethodInvoker)delegate ()
            {
                if (Editor == null || Editor.IsDisposed)
                {
                    Editor = new EditorWindow();
                }
                else if (Editor.Visible) return;

                Editor.Show();
            });
        }

        public void OnFDRUpdate(FDP2.FDR updated)
        {
            return;
        }

        public void OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
            return;
        }
    }
}
