﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;
using vatsys;
using vatsys.Plugin;
using Timer = System.Timers.Timer;

namespace ATISPlugin
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin, IDisposable
    {
        public string Name => "ATIS Editor";
        public static string DisplayName => "ATIS Editor";

        public static readonly Version Version = new Version(3, 14);
        private static readonly string VersionUrl = "https://raw.githubusercontent.com/badvectors/ATISPlugin/master/Version.json";
        private static readonly string ZuluUrl = "https://raw.githubusercontent.com/badvectors/ATISPlugin/master/Zulu.json";
        private static readonly string CodesUrl = "https://raw.githubusercontent.com/badvectors/ATISPlugin/master/Codes.json";

        private static readonly HttpClient Client = new HttpClient();

        private static CustomToolStripMenuItem? ATISMenu;

        public static ATISControl ATIS1 = null!;
        public static ATISControl ATIS2 = null!;
        public static ATISControl ATIS3 = null!;
        public static ATISControl ATIS4 = null!;

        private static EditorWindow Editor = null!;

        public static string ProfileName()
        {
            if (Profile.Name.Contains("Australia")) return "Australia";
            else if (Profile.Name.Contains("VATNZ")) return "New Zealand";
            else return string.Empty;
        }
        public static readonly string ManualVoiceName = "Manual Recording";
        public static string DatasetPath => Path.Combine(Helpers.GetFilesFolder(), "Profiles", ProfileName());
        public static ATIS? ATISData { get; set; }
        public static List<ZuluInfo> ZuluInfo { get; set; } = new List<ZuluInfo>();
        public static List<CodeBlock> CodeBlocks { get; set; } = new List<CodeBlock>();

        public static SoundPlayer SoundPlayer { get; set; } = new SoundPlayer();
        private static Timer METARTimer { get; set; } = new Timer();
        private static Timer BroadcastTimer { get; set; } = new Timer();
        public static List<ATISAudio> ToBroadcast { get; set; } = new List<ATISAudio>();

        public static List<VSCSFrequency> Frequencies { get; set; } = new List<VSCSFrequency>();

        public Plugin()
        {
            if (!Profile.Name.Contains("Australia") && !Profile.Name.Contains("VATNZ"))
            {
                return;
            }
            
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

                if (!Directory.Exists(DatasetPath))
                {
                    Errors.Add(new Exception("Could not find profile."), DisplayName);
                    return;
                }

                GetData();

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

                ATIS1 = new ATISControl(1);
                ATIS2 = new ATISControl(2);
                ATIS3 = new ATISControl(3);
                ATIS4 = new ATISControl(4);

                ATIS1.StatusChanged += OnUpdate;
                ATIS2.StatusChanged += OnUpdate;
                ATIS3.StatusChanged += OnUpdate;
                ATIS4.StatusChanged += OnUpdate;

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

            _ = GetCodeBlocks();

            _ = CheckVersion();

            MET.Instance.ProductsChanged += METARChanged;
        }

        private void METARChanged(object sender, MET.ProductsChangedEventArgs e)
        {
            var products = MET.Instance.GetProducts(e.ProductRequest);
            
            if (products == null || products.Count == 0) return;
            
            var metar = products[0];

            if (metar.Type != MET.ProductType.VATSIM_METAR || metar.Text == "No product available.") return;

            var atis = GetATIS(metar.Icao);

            if (atis == null) return;

            var updated = atis.UpdateMetar(metar.Text);

            OnMETARUpdate(atis.Number, updated);
        }

        private ATISControl? GetATIS(string icao)
        {
            if (ATIS1?.ICAO == icao) return ATIS1;
            if (ATIS2?.ICAO == icao) return ATIS2;
            if (ATIS3?.ICAO == icao) return ATIS3;
            if (ATIS4?.ICAO == icao) return ATIS4;
            return null;
        }

        private static async Task CheckVersion()
        {
            try
            {
                var response = await Client.GetStringAsync(VersionUrl);

                var version = JsonConvert.DeserializeObject<Version>(response);

                if (version is null)
                {
                    Errors.Add(new Exception("No version information available"));
                    return;
                }

                if (version.Major == Version.Major && version.Minor == Version.Minor) return;

                Errors.Add(new Exception("A new version of the plugin is available."), DisplayName);
            }
            catch { }
        }

        private static async Task GetCodeBlocks()
        {
            try
            {
                var response = await Client.GetStringAsync(CodesUrl);

                var codeBlocks = JsonConvert.DeserializeObject<CodeBlock[]>(response);

                foreach (var codeBlock in codeBlocks ?? [])
                {
                    CodeBlocks.Add(codeBlock);
                }
            }
            catch (Exception ex)
            {
                Errors.Add(new Exception(ex.Message), DisplayName);
            }
        }

        private static async Task GetZuluInfo()
        {
            try
            {
                var response = await Client.GetStringAsync(ZuluUrl);

                var zuluInfo = JsonConvert.DeserializeObject<ZuluInfo[]>(response);

                foreach (var info in zuluInfo ?? [])
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

                if (!Network.IsOfficialServer) continue;

                if (!Network.GetATISConnected(atb.ATISIndex)) continue;

                try
                {
                    await AFV.AddOrUpdateATISBot(atb.Audio, atb.ATISIndex, atb.Callsign, atb.Frequency, atb.VisPoint, atb.Duration);
                }
                catch (Exception ex)
                {
                    Errors.Add(new Exception($"Could not start voice ATIS: {ex.Message}"), DisplayName);
                }
            }

            BroadcastTimer.Start();
        }

        private void Audio_VSCSFrequenciesChanged(object sender, EventArgs e)
        {
            Frequencies.Clear();

            var frequenciesUpdate = false;

            foreach (var frequency in Audio.VSCSFrequencies)
            {
                if (!frequency.Transmit) continue;

                frequenciesUpdate = true;

                Frequencies.Add(frequency);
            }

            if (!IsEditorOpen()) return;

            if (Editor != null && !Editor.IsDisposed && frequenciesUpdate)
            {
                Editor?.RefreshEvent.Invoke(this, null);
            }
        }

        private async void Network_Disconnected(object? sender, EventArgs? e)
        {
            ToBroadcast.Clear();
            
            await ATIS1.Delete();
            await ATIS2.Delete();
            await ATIS3.Delete();
            await ATIS4.Delete();
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            if (!IsEditorOpen()) return;

            Editor?.RefreshEvent.Invoke(this, null);
        }

        private static void PlayUpdateSound()
        {
            var sound = Path.Combine(Helpers.GetProgramFolder(), "wav", "AIS.wav");

            SoundPlayer.SoundLocation = sound;

            SoundPlayer.Play();
        }

        private void OnMETARUpdate(int number, bool updated)
        {
            if (!updated)
            {
                if (IsEditorOpen() && Editor.Number == number)
                {
                    Editor?.RefreshEvent.Invoke(null, new RefreshEventArgs(number));
                }

                return;
            }

            if (!IsEditorOpen())
            {
                ShowEditorWindow();

                Editor?.RefreshEvent.Invoke(null, new RefreshEventArgs(number));

                PlayUpdateSound();

                return;
            }
            else
            {
                Editor?.RefreshEvent.Invoke(null, null);

                if (Editor?.Number == number) return;

                PlayUpdateSound();
            }
        }

        private void GetData()
        {
            ATISData = (ATIS?)LoadXML(DatasetPath + "\\ATIS.xml", typeof(ATIS)) ?? new ATIS();
        }

        public object? LoadXML(string filePath, Type type)
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

        private static bool IsEditorOpen() => Editor != null && !Editor.IsDisposed && Editor.Visible;

        private static void ShowEditorWindow()
        {
            if (ATISData == null)
            {
                Errors.Add(new Exception("Plugin was not started due to missing data."), DisplayName);
                return;
            }

            if (Editor == null || Editor.IsDisposed)
            {
                Editor = new EditorWindow();
            }

            MMI.InvokeOnGUI(delegate () { Editor.Show(Form.ActiveForm); });
            //{
            //    if (!Editor.Visible)
            //    {
            //        Editor.Show();
            //    }
            //    else
            //    {
            //        Editor.BringToFront();
            //    }
            //});
        }

        public void OnFDRUpdate(FDP2.FDR updated)
        {
            return;
        }

        public void OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
            return;
        }

        public void Dispose()
        {
            Network_Disconnected(this, null);
        }
    }
}
