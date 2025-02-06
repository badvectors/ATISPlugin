﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using vatsys;
using Timer = System.Timers.Timer;

namespace ATISPlugin
{
    public class ATISControl
    {
        public int Number { get; private set; }
        private int Index => Number - 1;
        private string? Callsign { get; set; }
        public char ID { get; set; } = 'Z';
        public bool IsZulu { get; set; }
        public string? ICAO { get; set; }
        private Coordinate? VisPoint { get; set; }
        public string? FrequencyDisplay { get; set; }
        private uint Frequency { get; set; }
        private uint AliasFrequency { get; set; }
        private int FSDFrequency => AliasFrequency != 199998000U ? VSCSFrequencyToFSDFrequency(AliasFrequency) : VSCSFrequencyToFSDFrequency(Frequency);

        public bool Broadcasting { get; set; }
        public bool Listening { get; set; }
        public bool CanListen => !string.IsNullOrWhiteSpace(FileName);
        public string? FileName { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public bool TimeCheck { get; set; } = true;
        public List<ATISLine> Lines { get; set; } = new List<ATISLine>();
        private double ATISDuration { get; set; }
        private MemoryStream? ATISStream;
        private MemoryStream? TimeCheckStream;
        private double CompleteATISDuration { get; set; }
        private MemoryStream? CompleteStream;
        public List<ATISLine> SuggestedLines { get; set; } = new List<ATISLine>();
        public bool HasUpdates => SuggestedLines.Any();
        public SpeechSynthesizer SpeechSynth { get; set; }
        private PromptBuilder? ATISSpoken { get; set; }
        private SpeechAudioFormatInfo SpeechFormat { get; set; }
        public string? METARRaw { get; set; }
        public string? METARLastRaw { get; set; }
        public string? METARNewRaw { get; set; }
        private WaveFormat WaveForm { get; set; } = new WaveFormat(44100, 1);
        public PromptRate PromptRate { get; set; } = PromptRate.Medium;
        public InstalledVoice InstalledVoice => SpeechSynth.GetInstalledVoices().FirstOrDefault(x => x.VoiceInfo.Name == VoiceName);
        public string? VoiceName { get; set; }
        public SoundPlayer SoundPlayer { get; set; } = new SoundPlayer();
        private CultureInfo CultureInfo => CultureInfo.GetCultureInfo("en");    // TODO: Check if in the ATIS.xml
        public bool Recording { get; set; }

        public event EventHandler? StatusChanged;
        private readonly Timer LoopTimer;

        public ATISControl()
        {
            SetupATISLines();

            LoopTimer = new Timer
            {
                AutoReset = false
            };
            LoopTimer.Elapsed += new ElapsedEventHandler(LoopTimer_Elapsed);

            SpeechSynth = new SpeechSynthesizer()
            {
                Rate = 0
            };
            SpeechFormat = new SpeechAudioFormatInfo(WaveForm.SampleRate, AudioBitsPerSample.Sixteen, AudioChannel.Mono);

            var installedVoice = SpeechSynth.GetInstalledVoices().FirstOrDefault();

            if (installedVoice != null)
            {
                VoiceName = installedVoice.VoiceInfo.Name;
            }
            else
            {
                VoiceName = Plugin.ManualVoiceName;
            }
        }

        private void SetupATISLines()
        {
            Lines.Clear();

            if (Plugin.ATISData?.Editor == null) return;

            int number = 1;

            foreach (var line in Plugin.ATISData.Editor)
            {
                var atisLine = new ATISLine(line.name,
                    number,
                    line.InputType,
                    line.NameIsSpoken,
                    line.NumbersSpokenGrouped,
                    line.value,
                    METARField.None);

                switch (atisLine.Name)
                {
                    case "WIND":
                    case "SFC WIND":
                        atisLine.METARField = METARField.Wind;
                        break;
                    case "VIS":
                        atisLine.METARField = METARField.Visibility;
                        break;
                    case "WX":
                        atisLine.METARField = METARField.Weather;
                        break;
                    case "CLD":
                        atisLine.METARField = METARField.Cloud;
                        break;
                    case "TMP":
                        atisLine.METARField = METARField.Temperature;
                        break;
                    case "DP":
                        atisLine.METARField = METARField.DewPoint;
                        break;
                    case "QNH":
                        atisLine.METARField = METARField.QNH;
                        break;
                }

                Lines.Add(atisLine);

                number++;
            }

            Lines.Add(new ATISLine("ZULU", number));
        }

        public ATISControl(int number) : this()
        {
            Number = number;
        }

        private void LoopTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Broadcasting) return;

            BroadcastStart();
        }

        public async Task Create(string icao, string frequency, Coordinate coordinates)
        {
            if (!Network.IsConnected || !Network.IsValidATC) return;

            if (Network.GetATISConnected(Index)) return;

            try
            {
                var exists = Network.GetOnlineATCs.FirstOrDefault(x => x.Callsign == $"{icao}_ATIS");

                if (exists != null)
                {
                    Errors.Add(new Exception($"ATIS for {icao} already exists."), Plugin.DisplayName);

                    await Delete();

                    return;
                }

                ICAO = icao;
                FrequencyDisplay = Normalize25KhzFrequency(frequency);
                Frequency = Normalize25KhzFrequency(FrequencyToUInt(frequency));
                AliasFrequency = Normalize25KhzFrequency(199998000U);
                Callsign = $"{icao}_ATIS";
                VisPoint = coordinates;
                IsZulu = false;

                Network.ConnectATIS(Index, Callsign, ICAO, FSDFrequency, VisPoint);
            }
            catch (Exception ex)
            {
                Errors.Add(new Exception($"Could not create ATIS: {ex.Message}"), Plugin.DisplayName);

                await Delete();
            }

            try
            {
                MMI.OpenATISWindow(Callsign);
            }
            catch { }

            StatusChanged?.Invoke(this, null);
        }

        public async Task Delete()
        {
            await BroadcastStop();

            if (Network.GetATISConnected(Index))
            {
                Network.DisconnectATIS(Index);
            }

            ICAO = null;
            ID = 'Z'; 
            FrequencyDisplay = null;
            Frequency = 0;
            AliasFrequency = 0;
            Callsign = null;
            VisPoint = null;
            IsZulu = false;

            METARLastRaw = null;
            METARRaw = null;

            foreach (var line in Lines)
            {
                line.Value = null;
                line.Changed = false;
            }

            SetupATISLines();

            SuggestedLines.Clear();

            SoundPlayer.Stop();

            StatusChanged?.Invoke(this, null);
        }

        public async Task Save(char id, Dictionary<string, string> items, bool timeCheck)
        {
            await BroadcastStop();

            DateTimeUtc = DateTime.UtcNow;
            ID = id;
            TimeCheck = timeCheck;

            METARLastRaw = METARRaw;

            METARRaw = METARNewRaw;

            foreach (var line in Lines)
            {
                line.Changed = false;
            }

            foreach (var item in items)
            {
                var line = Lines.FirstOrDefault(x => x.Name == item.Key);

                if (line == null) continue;

                if (line.Value == item.Value) continue;

                line.Value = item.Value.ToUpper();

                line.Changed = true;
            }

            SuggestedLines.Clear();

            if (VoiceName != Plugin.ManualVoiceName)
            {
                GenerateSpoken();

                ATISStream = new MemoryStream();

                ATISDuration = SetContent(ATISSpoken, ref ATISStream);

                GenerateAutoFile();

                Listening = true;
            }
            else
            {
                StartRecording();
            }

            StatusChanged?.Invoke(this, null);
        }

        public void ListenStart()
        {
            if (!CanListen) return;

            Listening = true;

            var directory = Path.Combine(Plugin.DatasetPath, "Temp");

            var file = Path.Combine(directory, FileName);

            SoundPlayer.SoundLocation = file;

            SoundPlayer.Play();
        }

        public void ListenStop()
        {
            Listening = false;

            SoundPlayer.Stop();
        }

        public void BroadcastStart()
        {
            Broadcasting = true;

            Network.UpdateATIS(Index, ID, GetInfo());

            if (VoiceName == Plugin.ManualVoiceName)
            {
                var directory = Path.Combine(Plugin.DatasetPath, "Temp");

                var file = Path.Combine(directory, FileName);

                var audio = File.ReadAllBytes(file);

                var atisAudio = new ATISAudio(audio, Index, Callsign, Frequency, VisPoint, TimeSpan.Zero);

                Plugin.ToBroadcast.Add(atisAudio);
            }
            else
            {
                CompleteATISDuration = GenerateCompleteStream();

                var audio = ReadMemoryStream(CompleteStream);

                var duration = TimeCheck ? TimeSpan.FromMilliseconds(CompleteATISDuration + 60000.0) : TimeSpan.Zero;

                var atisAudio = new ATISAudio(audio, Index, Callsign, Frequency, VisPoint, duration);

                Plugin.ToBroadcast.Add(atisAudio);

                if (!TimeCheck) return;

                LoopTimer.Interval = CompleteATISDuration;

                LoopTimer.Start();
            }
        }

        public static byte[] ReadMemoryStream(Stream? input)
        {
            if (input is null)
            {
                return Array.Empty<byte>();
            }

            input.Seek(0, SeekOrigin.Begin);

            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public async Task BroadcastStop()
        {
            Broadcasting = false;

            try
            {
                await AFV.RemoveATISBot(Index);
            }
            catch { }
        }

        public string[] GetInfo()
        {
            var output = new List<string>
            {
                $"ATIS {ICAO} {ID} {DateTimeUtc:ddHHmm}"
            };
            if (IsZulu)
            {
                var zulu = Lines.FirstOrDefault(x => x.Name == "ZULU");
                if (zulu == null || string.IsNullOrWhiteSpace(zulu.Value)) return output.ToArray();
                output.Add(zulu.Value!);
                return output.ToArray();
            }
            foreach (var line in Lines.Where(x => x.Visible).ToList())
            {
                if (line.Name == "OFCW_NOTIFY" || line.Name == "ZULU") continue;
                var change = " ";
                if (line.Changed) change = "+";
                output.Add($"{change}[{line.Name}] {line.Value}");
            }
            return output.ToArray();
        }

        public void SuggestionsCancel()
        {
            METARRaw = METARLastRaw;
            SuggestedLines.Clear();
        }

        public bool UpdateMetar(string metar)
        {
            if (ICAO == null) return false;

            if (metar == METARRaw) return false;

            if (IsZulu && !string.IsNullOrWhiteSpace(METARRaw)) return false;
            
            METARNewRaw = metar;

            var updatedLines = new METAR().Process(metar);

            var suggestedLines = new List<ATISLine>();

            foreach (var type in Enum.GetValues(typeof(METARField)).Cast<METARField>())
            {
                if (type == METARField.None) continue;

                var updatedLine = updatedLines?.GetField(type);

                if (updatedLine == null) continue;

                var currentLine = Lines.FirstOrDefault(x => x.METARField == type);

                if (currentLine == null) continue;

                if (updatedLine == currentLine.Value) continue;

                var suggestLine = new ATISLine(currentLine.Name, 0, currentLine.Type, currentLine.NameSpoken, currentLine.NumbersGrouped, updatedLine, currentLine.METARField);

                suggestedLines.Add(suggestLine);
            }

            var weatherLine = Lines.FirstOrDefault(x => x.METARField == METARField.Weather);

            var visibilityLine = Lines.FirstOrDefault(x => x.METARField == METARField.Visibility);

            var cloudLine = Lines.FirstOrDefault(x => x.METARField == METARField.Cloud);

            var suggestedWeather = suggestedLines.FirstOrDefault(x => x.METARField == METARField.Weather);

            var suggestedVisibility = suggestedLines.FirstOrDefault(x => x.METARField == METARField.Visibility);

            var suggestedCloud = suggestedLines.FirstOrDefault(x => x.METARField == METARField.Cloud);

            // Was CAVOK and now not.
            if (weatherLine != null && weatherLine.Value == "CAVOK")
            {
                if (suggestedCloud != null && !string.IsNullOrWhiteSpace(suggestedCloud.Value))
                {
                    suggestedLines.Add(new ATISLine(weatherLine.Name, 0, weatherLine.Type, weatherLine.NameSpoken, weatherLine.NumbersGrouped, string.Empty, weatherLine.METARField));
                }
                else if (suggestedVisibility != null && !string.IsNullOrWhiteSpace(suggestedVisibility.Value))
                {
                    suggestedLines.Add(new ATISLine(weatherLine.Name, 0, weatherLine.Type, weatherLine.NameSpoken, weatherLine.NumbersGrouped, string.Empty, weatherLine.METARField));
                }
            }

            // Was no CAVOK and now is.
            if (suggestedWeather != null && suggestedWeather.Value == "CAVOK")
            {
                if (visibilityLine != null && !string.IsNullOrWhiteSpace(visibilityLine.Value))
                {
                    suggestedLines.Add(new ATISLine(visibilityLine.Name, 0, visibilityLine.Type, visibilityLine.NameSpoken, visibilityLine.NumbersGrouped, string.Empty, visibilityLine.METARField));
                }
                if (cloudLine != null && !string.IsNullOrWhiteSpace(cloudLine.Value))
                {
                    suggestedLines.Add(new ATISLine(cloudLine.Name, 0, cloudLine.Type, cloudLine.NameSpoken, cloudLine.NumbersGrouped, string.Empty, cloudLine.METARField));
                }
            }

            SuggestedLines = suggestedLines;

            if (!SuggestedLines.Any()) return false;

            return true;
        }

        private PromptBuilder DoReplacements(PromptBuilder promptBuilder, string? text, bool groupNumbers = false)
        {
            if (InstalledVoice == null) return promptBuilder;

            if (text == null) return promptBuilder;

            var stringReplacements = new Dictionary<string, string>();

            var regexReplacements = new Dictionary<string, string>();

            if (Plugin.ATISData == null) return promptBuilder;

            foreach (var item in Plugin.ATISData.Translations.Where(x => !string.IsNullOrWhiteSpace(x.String)).OrderByDescending(x => x.String!.Length))
            {
                stringReplacements.Add(item.String!, item.Spoken!);
            }

            foreach (var item in Plugin.ATISData.Translations.Where(x => !string.IsNullOrWhiteSpace(x.Regex)).OrderByDescending(x => x.Regex!.Length))
            {
                regexReplacements.Add(item.Regex!, item.Spoken!);
            }

            var phonemeReplacements = Plugin.ATISData.Translations.Where(x => !string.IsNullOrWhiteSpace(x.String) && !string.IsNullOrWhiteSpace(x.Alphabet));

            foreach (var keyValuePair in stringReplacements.Where(s => s.Key.Contains(" ")).OrderByDescending(s => s.Key.Length))
                text = text.Replace(keyValuePair.Key, keyValuePair.Value);

            foreach (var keyValuePair in regexReplacements.Where(s => s.Key.Contains(" ") || s.Key.Contains("\\s")).OrderByDescending(s => s.Key.Length))
                text = Regex.Replace(text, keyValuePair.Key, keyValuePair.Value);

            var output = new List<string>();

            foreach (var word in Regex.Split(text, "\\s+"))
            {
                var input = word;

                KeyValuePair<string, string>? keyValuePair = null;

                foreach (var regexReplacement in regexReplacements.OrderByDescending(x => x.Key.Length))
                {
                    if (Regex.IsMatch(input, regexReplacement.Key))
                        keyValuePair = regexReplacement;

                    if (keyValuePair?.Key == null) continue;

                    if (keyValuePair?.Value == null) continue;

                    input = Regex.Replace(input, keyValuePair.Value.Key, keyValuePair.Value.Value);

                    break;
                }

                if (!groupNumbers)
                {
                    foreach (Match match in Regex.Matches(input, "(\\s|^|\\.|\\,)\\d+(\\s|$|\\.|\\,)").Cast<Match>())
                    {
                        string newValue = match.Value.Aggregate(string.Empty, (c, i) => i != ' ' ? c + i.ToString() + " " : c + i.ToString());
                        input = input.Replace(match.Value, newValue);
                    }
                }
                else
                {
                    input = Regex.Replace(Regex.Replace(input, "(\\d{1,2})([0]{3})", "$1 thousand"), "(\\d{1,2})(\\d)([0]{2})", "$1 thousand $2 hundred");
                }

                foreach (var stringReplacement in stringReplacements)
                {
                    var isMatch = Regex.IsMatch(input, stringReplacement.Key);
                    if (!isMatch) continue;
                    input = Regex.Replace(input, "\\b" + Regex.Escape(stringReplacement.Key) + "\\b", stringReplacement.Value);
                    break;
                }

                if (input == word && !phonemeReplacements.Any(x => x.String == word))
                    input = word.ToLowerInvariant();

                output.Add(input);
            }

            foreach (var word in output)
            {
                var phonemeReplacement = phonemeReplacements.FirstOrDefault(x => x.String == word);

                if (phonemeReplacement == null)
                {
                    promptBuilder.AppendText(word + " ");
                    continue;
                }

                if (phonemeReplacement.Alphabet != "Text" && !InstalledVoice.VoiceInfo.Name.Contains("Microsoft"))
                {
                    promptBuilder.AppendText(phonemeReplacement.FallbackSpoken + " ");
                }
                else
                {
                    var temp = $"<phoneme alphabet=";
                    switch (phonemeReplacement.Alphabet)
                    {
                        case "Text":
                            promptBuilder.AppendText(phonemeReplacement.Spoken + " ");
                            continue;
                        case "IPA":
                            promptBuilder.AppendTextWithPronunciation(word, phonemeReplacement.Spoken);
                            continue;
                        case "SAPI":
                            temp += "\"x-microsoft-sapi\"";
                            break;
                        case "UPS":
                            temp += "\"x-microsoft-ups\"";
                            break;
                    }
                    string ssmlMarkup = temp + " ph=\"" + phonemeReplacement.Spoken + "\">" + word + "</phoneme>";
                    promptBuilder.AppendSsmlMarkup(ssmlMarkup);
                }
            }

            return promptBuilder;
        }

        private double SetContent(PromptBuilder? speech, ref MemoryStream stream)
        {
            if (SpeechSynth == null) throw new ArgumentNullException(nameof(speech), "Text to speech not available.");
            SpeechSynth.SetOutputToAudioStream(stream, SpeechFormat);
            SpeechSynth.Speak(speech);
            SpeechSynth.SetOutputToNull();
            stream.Seek(0L, SeekOrigin.Begin);
            return stream.Length / WaveForm.AverageBytesPerSecond * 1000.0;
        }

        private void GenerateSpoken()
        {
            if (InstalledVoice == null) return;

            var speech = new PromptBuilder(CultureInfo);
            speech.StartVoice(InstalledVoice.VoiceInfo.Name);
            speech.StartStyle(new PromptStyle(PromptRate));
            speech.AppendBreak(TimeSpan.FromSeconds(0.5));

            speech.StartSentence();
            speech = DoReplacements(speech, $"{ICAO} ATIS {ID}", false);
            speech.EndSentence();
            speech.AppendBreak(TimeSpan.FromSeconds(1.0));

            if (IsZulu)
            {
                var zulu = Lines.FirstOrDefault(x => x.Name == "ZULU");
                if (zulu != null && zulu.Value != null)
                {
                    speech.StartSentence();
                    speech = DoReplacements(speech, zulu.Value);
                    speech.EndSentence();
                    speech.AppendBreak(TimeSpan.FromSeconds(1.0));
                }
            }
            else
            {
                foreach (var line in Lines)
                {
                    if (string.IsNullOrWhiteSpace(line.Value)) continue;

                    speech.StartSentence();

                    if (line.Name == "OFCW_NOTIFY")
                    {
                        speech = DoReplacements(speech, $"On first contact with {ICAO} {line.Value}, notify receipt of information {ID}", line.NumbersGrouped);
                    }
                    else
                    {
                        speech = DoReplacements(speech, line.NameSpoken ? $"{line.Name} {line.Value}" : line.Value, line.NumbersGrouped);
                    }

                    speech.EndSentence();
                    speech.AppendBreak(TimeSpan.FromSeconds(1.0));
                }
            }

            speech.EndStyle();
            speech.EndVoice();

            ATISSpoken = speech;
        }

        private void GenerateTimeCheck()
        {
            if (InstalledVoice == null) return;

            PromptBuilder speech = new PromptBuilder(CultureInfo);
            speech.StartVoice(InstalledVoice.VoiceInfo.Name);
            speech.StartStyle(new PromptStyle(PromptRate));
            speech.AppendBreak(TimeSpan.FromSeconds(3.0));
            speech.AppendText(CreateTimecheckText(TimeSpan.FromMilliseconds(ATISDuration) + TimeSpan.FromSeconds(7.0)));
            speech.EndStyle();
            speech.EndVoice();
            SetTimeCheckContent(speech);
        }

        private string CreateTimecheckText(TimeSpan offset)
        {
            DateTime nearest = (DateTime.UtcNow + offset).RoundToNearest(TimeSpan.FromSeconds(30.0));
            string timecheckText = "Time check, " + nearest.ToString("HHmm").Aggregate(string.Empty, (c, i) => c + i.ToString() + " ");
            if (nearest.Second == 30)
                timecheckText += " and a half.";
            return timecheckText;
        }

        private double SetTimeCheckContent(PromptBuilder speech)
        {
            TimeCheckStream = new MemoryStream();
            return SetContent(speech, ref TimeCheckStream);
        }

        private double GenerateCompleteStream()
        {
            CompleteStream = new MemoryStream();

            if (ATISStream == null)
            {
                return 0;
            }

            ATISStream.Seek(0, SeekOrigin.Begin);
            ATISStream.CopyTo(CompleteStream);

            CompleteStream.Seek(0, SeekOrigin.End);

            if (TimeCheck && TimeCheckStream != null)
            {
                GenerateTimeCheck();
                TimeCheckStream.Seek(0, SeekOrigin.Begin);
                TimeCheckStream.CopyTo(CompleteStream);
            }

            return CompleteStream.Length / WaveForm.AverageBytesPerSecond * 1000.0;
        }

        private WaveFileWriter? writer; 
        private WaveInEvent? waveIn;

        public void StartRecording()
        {
            FileName = $"{ICAO}_{ID}_{DateTime.UtcNow:HHmmss}.wav";

            var directory = Path.Combine(Plugin.DatasetPath, "Temp");

            var file = Path.Combine(directory, FileName);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            waveIn = new WaveInEvent
            {
                WaveFormat = WaveForm
            };

            writer = new WaveFileWriter(file, waveIn.WaveFormat);

            waveIn.StartRecording();

            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.RecordingStopped += WaveIn_RecordingStopped;

            Recording = true;
        }

        private void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }

            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (writer == null) return;

            writer.Write(e.Buffer, 0, e.BytesRecorded);

            writer.Flush();
        }

        public void StopRecording()
        {
            if (writer == null) return;
            if (waveIn == null) return;

            waveIn.StopRecording();

            Recording = false;

            ListenStart();
        }

        private void GenerateAutoFile()
        {
            FileName = $"{ICAO}_{ID}_{DateTime.UtcNow:HHmmss}.wav";

            var directory = Path.Combine(Plugin.DatasetPath, "Temp");

            var file = Path.Combine(directory, FileName);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            SpeechSynth.SetOutputToWaveFile(file, SpeechFormat);
            SpeechSynth.Speak(ATISSpoken);
            SpeechSynth.SetOutputToNull();

            ListenStart();
        }

        private static int VSCSFrequencyToFSDFrequency(uint freq) => (int)(freq / 1000U) - 100000;

        private static uint Normalize25KhzFrequency(uint freq) => freq < 100000000U || freq % 100000U != 20000U && freq % 100000U != 70000U ? freq : freq + 5000U;

        public static string Normalize25KhzFrequency(string freq)
        {
            if (freq.IndexOf('.') < 3)
                return freq;
            string str = Conversions.Normalize25KhzFrequency(Convert.ToUInt32(freq.Replace(".", "")) * (uint)Math.Pow(10.0, 9 - (freq.Length - 1))).ToString();
            freq = str.Substring(0, 3) + "." + str.Substring(3);
            return freq;
        }

        public static uint FrequencyToUInt(string freq)
        {
            int freq1 = (int)(uint)Math.Round(double.Parse(Normalize25KhzFrequency(freq.Trim())) * 1000000.0);
            CheckFrequencyValid((uint)freq1);
            return (uint)freq1;
        }

        public static void CheckFrequencyValid(uint freq)
        {
            switch (freq % 100000U)
            {
                case 0:
                    break;
                case 25000:
                    break;
                case 50000:
                    break;
                case 75000:
                    break;
                default:
                    throw new ArgumentException("8.33kHz frequencies not currently supported.");
            }
        }
    }
}
