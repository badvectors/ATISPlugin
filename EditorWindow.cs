﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;
using vatsys;

namespace ATISPlugin
{
    public partial class EditorWindow : BaseForm
    {
        private ATISControl Control
        {   
            get
            {
                if (Number == 1) return Plugin.ATIS1;
                else if (Number == 2) return Plugin.ATIS2;
                else if (Number == 3) return Plugin.ATIS3;
                else return Plugin.ATIS4;
            }
        }
        private string ICAO { get; set; }
        private char ID { get; set; }
        private int Number { get; set; } = 1;
        private Dictionary<string, string> Saves { get; set; } = new Dictionary<string, string>();
        private bool Edits => Saves.Any() || 
            TimeCheck != Control.TimeCheck || 
            ID != Control.ID || 
            Control.PromptRate != Rate || 
            Control.InstalledVoice != Voice;
        private bool TimeCheck { get; set; }
        public EventHandler RefreshEvent { get; set; }  
        public InstalledVoice Voice { get; set; }
        public PromptRate Rate { get; set; } 

        public EditorWindow()
        {
            InitializeComponent();

            labelVersion.Text = $"v{Plugin.Version.Major}.{Plugin.Version.Minor}";

            BackColor = Colours.GetColour(Colours.Identities.WindowBackground);
            ForeColor = Colours.GetColour(Colours.Identities.InteractiveText);

            Network.Connected += OnRefeshEvent;
            Network.Disconnected += OnRefeshEvent;

            RefreshEvent += OnRefeshEvent;
        }

        public void Change(int number)
        {
            Number = number;

            ID = Control.ID;

            TimeCheck = Control.TimeCheck;

            Saves.Clear();

            RefreshForm();
        }

        private void OnRefeshEvent(object sender, EventArgs e)
        {
            RefreshForm();
        }

        private void EditorWindow_Load(object sender, EventArgs e)
        {
            comboBoxAirport.Items.Clear();

            foreach (var freq in Plugin.ATISData.Frequencies.OrderBy(x => x.Airport))
            {
                comboBoxAirport.Items.Add(freq.Airport);
            }

            for (char c = 'A'; c <= 'Z'; c++)
            {
                comboBoxLetter.Items.Add(c.ToString());
            }

            foreach (var voice in Control.SpeechSynth.GetInstalledVoices())
            {
                comboBoxVoice.Items.Add(voice.VoiceInfo.Name);
            }

            Change(1);
        }

        private void RefreshForm()
        {
            labelMETAR.Text = string.Empty;

            comboBoxLetter.SelectedIndex = comboBoxLetter.FindStringExact(ID.ToString());

            comboBoxVoice.SelectedIndex = comboBoxVoice.FindStringExact(Control.InstalledVoice.VoiceInfo.Name);
            Voice = Control.InstalledVoice;

            comboBoxRate.SelectedIndex = comboBoxRate.FindStringExact(Control.PromptRate.ToString());
            Rate = Control.PromptRate;

            comboBoxTimecheck.SelectedIndex = comboBoxTimecheck.FindStringExact(TimeCheck.ToString());

            if (Plugin.ATIS1.ICAO != null)
            {
                buttonATIS1.Text = Plugin.ATIS1.ICAO;
            }
            else
            {
                buttonATIS1.Text = "ATIS #1";
            }

            if (Plugin.ATIS2.ICAO != null)
            {
                buttonATIS2.Text = Plugin.ATIS2.ICAO;
            }
            else
            {
                buttonATIS2.Text = "ATIS #2";
            }

            if (Plugin.ATIS3.ICAO != null)
            {
                buttonATIS3.Text = Plugin.ATIS3.ICAO;
            }
            else
            {
                buttonATIS3.Text = "ATIS #3";
            }

            if (Plugin.ATIS4.ICAO != null)
            {
                buttonATIS4.Text = Plugin.ATIS4.ICAO;
            }
            else
            {
                buttonATIS4.Text = "ATIS #4";
            }

            if (Plugin.ATIS1.SuggestedLines.Any())
            {
                buttonATIS1.BackColor = Color.Yellow;
            }
            else
            {
                buttonATIS1.BackColor = default;
            }

            if (Plugin.ATIS2.SuggestedLines.Any())
            {
                buttonATIS2.BackColor = Color.Yellow;
            }
            else
            {
                buttonATIS2.BackColor = default;
            }

            if (Plugin.ATIS3.SuggestedLines.Any())
            {
                buttonATIS3.BackColor = Color.Yellow;
            }
            else
            {
                buttonATIS3.BackColor = default;
            }

            if (Plugin.ATIS4.SuggestedLines.Any())
            {
                buttonATIS4.BackColor = Color.Yellow;
            }
            else
            {
                buttonATIS4.BackColor = default;
            }

            if (Control.IsNetworkConnected && Control?.ICAO == null)
            {
                comboBoxAirport.Enabled = true;
                buttonCreate.Enabled = true;
                buttonDelete.Visible = false;
                buttonCreate.Visible = true;

                buttonSave.Enabled = false;
                buttonCancel.Enabled = false;
                comboBoxLetter.Enabled = false;
                comboBoxTimecheck.Enabled = false;
                labelCode.Text = string.Empty;
                buttonGetMetar.Enabled = false;
                buttonBroadcast.Enabled = false;
                buttonNext.Enabled = false;
                comboBoxVoice.Enabled = false;
                buttonListen.Enabled = false;
                buttonListen.BackColor = Color.FromName("Control");
                buttonListen.ForeColor = default;
                buttonBroadcast.BackColor = Color.FromName("Control");
                buttonBroadcast.ForeColor = default;

                textBoxAPCH.TextChanged -= TextBox_TextChanged;
                textBoxRWY.TextChanged -= TextBox_TextChanged;
                textBoxSFCCOND.TextChanged -= TextBox_TextChanged;
                textBoxOPRINFO.TextChanged -= TextBox_TextChanged;
                textBoxWIND.TextChanged -= TextBox_TextChanged;
                textBoxVIS.TextChanged -= TextBox_TextChanged;
                textBoxCLD.TextChanged -= TextBox_TextChanged;
                textBoxWX.TextChanged -= TextBox_TextChanged;
                textBoxTMP.TextChanged -= TextBox_TextChanged;
                textBoxQNH.TextChanged -= TextBox_TextChanged;
                textBoxSIGWX.TextChanged -= TextBox_TextChanged;
                textBoxOFCW.TextChanged -= TextBox_TextChanged;

                textBoxAPCH.Text = string.Empty;
                textBoxRWY.Text = string.Empty;
                textBoxSFCCOND.Text = string.Empty;
                textBoxOPRINFO.Text = string.Empty;
                textBoxWIND.Text = string.Empty;
                textBoxVIS.Text = string.Empty;
                textBoxCLD.Text = string.Empty;
                textBoxWX.Text = string.Empty;
                textBoxTMP.Text = string.Empty;
                textBoxQNH.Text = string.Empty;
                textBoxSIGWX.Text = string.Empty;
                textBoxOFCW.Text = string.Empty;

                textBoxAPCH.Enabled = false;
                textBoxRWY.Enabled = false;
                textBoxSFCCOND.Enabled = false;
                textBoxOPRINFO.Enabled = false;
                textBoxWIND.Enabled = false;
                textBoxVIS.Enabled = false;
                textBoxCLD.Enabled = false;
                textBoxWX.Enabled = false;
                textBoxTMP.Enabled = false;
                textBoxQNH.Enabled = false;
                textBoxSIGWX.Enabled = false;
                textBoxOFCW.Enabled = false;

                labelWIND.BackColor = default;
                labelVIS.BackColor = default;
                labelCLD.BackColor = default;
                labelWX.BackColor = default;
                labelQNH.BackColor = default;
                labelTMP.BackColor = default;
            }
            else if (Control.IsNetworkConnected && Control?.ICAO != null)
            {
                comboBoxAirport.Enabled = false;
                comboBoxLetter.Enabled = true;
                comboBoxTimecheck.Enabled = true;
                buttonCreate.Visible = false;
                buttonDelete.Visible = true;
                buttonGetMetar.Enabled = true;
                buttonNext.Enabled = true;
                comboBoxVoice.Enabled = true;
                comboBoxRate.Enabled = true;

                textBoxAPCH.TextChanged -= TextBox_TextChanged;
                textBoxRWY.TextChanged -= TextBox_TextChanged;
                textBoxSFCCOND.TextChanged -= TextBox_TextChanged;
                textBoxOPRINFO.TextChanged -= TextBox_TextChanged;
                textBoxWIND.TextChanged -= TextBox_TextChanged;
                textBoxVIS.TextChanged -= TextBox_TextChanged;
                textBoxCLD.TextChanged -= TextBox_TextChanged;
                textBoxWX.TextChanged -= TextBox_TextChanged;
                textBoxTMP.TextChanged -= TextBox_TextChanged;
                textBoxQNH.TextChanged -= TextBox_TextChanged;
                textBoxSIGWX.TextChanged -= TextBox_TextChanged;
                textBoxOFCW.TextChanged -= TextBox_TextChanged;

                foreach (var line in Control.Lines)
                {
                    switch (line.Name)
                    {
                        case "APCH":
                            textBoxAPCH.Text = line.Value;
                            break;
                        case "RWY":
                            textBoxRWY.Text = line.Value;
                            break;
                        case "SFC COND":
                            textBoxSFCCOND.Text = line.Value;
                            break;
                        case "OPR INFO":
                            textBoxOPRINFO.Text = line.Value;
                            break;
                        case "WIND":
                            textBoxWIND.Text = line.Value;
                            break;
                        case "VIS":
                            textBoxVIS.Text = line.Value;
                            break;
                        case "CLD":
                            textBoxCLD.Text = line.Value;
                            break;
                        case "WX":
                            textBoxWX.Text = line.Value;
                            break;
                        case "QNH":
                            textBoxQNH.Text = line.Value;
                            break;
                        case "TMP":
                            textBoxTMP.Text = line.Value;
                            break;
                        case "SIGWX":
                            textBoxSIGWX.Text = line.Value;
                            break;
                        case "OFCW_NOTIFY":
                            textBoxOFCW.Text = line.Value;
                            break;
                        default:
                            break;
                    }
                }

                textBoxAPCH.TextChanged += TextBox_TextChanged;
                textBoxRWY.TextChanged += TextBox_TextChanged;
                textBoxSFCCOND.TextChanged += TextBox_TextChanged;
                textBoxOPRINFO.TextChanged += TextBox_TextChanged;
                textBoxWIND.TextChanged += TextBox_TextChanged;
                textBoxVIS.TextChanged += TextBox_TextChanged;
                textBoxCLD.TextChanged += TextBox_TextChanged;
                textBoxWX.TextChanged += TextBox_TextChanged;
                textBoxTMP.TextChanged += TextBox_TextChanged;
                textBoxQNH.TextChanged += TextBox_TextChanged;
                textBoxSIGWX.TextChanged += TextBox_TextChanged;
                textBoxOFCW.TextChanged += TextBox_TextChanged;

                textBoxAPCH.Enabled = true;
                textBoxRWY.Enabled = true;
                textBoxSFCCOND.Enabled = true;
                textBoxOPRINFO.Enabled = true;
                textBoxWIND.Enabled = true;
                textBoxVIS.Enabled = true;
                textBoxCLD.Enabled = true;
                textBoxWX.Enabled = true;
                textBoxTMP.Enabled = true;
                textBoxQNH.Enabled = true;
                textBoxSIGWX.Enabled = true;
                textBoxOFCW.Enabled = true;

                labelCode.Text = Control.ICAO;

                labelMETAR.Text = Control.METARRaw;

                foreach (var line in Control.Lines)
                {
                    var suggestedLine = Control.SuggestedLines.FirstOrDefault(x => x.Name == line.Name);

                    var saveLine = Saves.FirstOrDefault(x => x.Key == line.Name);

                    switch (line.Name)
                    {
                        case "APCH":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxAPCH.Text = saveLine.Value;
                            else if (suggestedLine != null) textBoxAPCH.Text = suggestedLine.Value;
                            break;
                        case "RWY":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxRWY.Text = saveLine.Value;
                            else if (suggestedLine != null) textBoxRWY.Text = suggestedLine.Value;
                            break;
                        case "SFC COND":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxSFCCOND.Text = saveLine.Value;
                            else if (suggestedLine != null) textBoxSFCCOND.Text = suggestedLine.Value;
                            break;
                        case "OPR INFO":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxOPRINFO.Text = saveLine.Value;
                            else if (suggestedLine != null) textBoxOPRINFO.Text = suggestedLine.Value;
                            break;
                        case "WIND":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxWIND.Text = saveLine.Value;
                            else if (suggestedLine != null)
                            {
                                textBoxWIND.Text = suggestedLine.Value;
                                labelWIND.BackColor = Color.Yellow;
                            }
                            else
                            {
                                labelWIND.BackColor = default;
                            }
                            break;
                        case "VIS":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxVIS.Text = saveLine.Value;
                            else if (suggestedLine != null)
                            {
                                textBoxVIS.Text = suggestedLine.Value;
                                labelVIS.BackColor = Color.Yellow;
                            }
                            else
                            {
                                labelVIS.BackColor = default;
                            }
                            break;
                        case "CLD":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxCLD.Text = saveLine.Value;
                            else if (suggestedLine != null)
                            {
                                textBoxCLD.Text = suggestedLine.Value;
                                labelCLD.BackColor = Color.Yellow;
                            }
                            else
                            {
                                labelCLD.BackColor = default;
                            }
                            break;
                        case "WX":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxWX.Text = saveLine.Value;
                            else if (suggestedLine != null)
                            {
                                textBoxWX.Text = suggestedLine.Value;
                                labelWX.BackColor = Color.Yellow;
                            }
                            else
                            {
                                labelWX.BackColor = default;
                            }
                            break;
                        case "QNH":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxQNH.Text = saveLine.Value;
                            else if (suggestedLine != null)
                            {
                                textBoxQNH.Text = suggestedLine.Value;
                                labelQNH.BackColor = Color.Yellow;
                            }
                            else
                            {
                                labelQNH.BackColor = default;
                            }
                            break;
                        case "TMP":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxTMP.Text = saveLine.Value;
                            else if (suggestedLine != null)
                            {
                                textBoxTMP.Text = suggestedLine.Value;
                                labelTMP.BackColor = Color.Yellow;
                            }
                            else
                            {
                                labelTMP.BackColor = default;
                            }
                            break;
                        case "SIGWX":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxSIGWX.Text = saveLine.Value;
                            else if (suggestedLine != null) textBoxSIGWX.Text = suggestedLine.Value;
                            break;
                        case "OFCW_NOTIFY":
                            if (!string.IsNullOrWhiteSpace(saveLine.Value)) textBoxOFCW.Text = saveLine.Value;
                            else if (suggestedLine != null) textBoxOFCW.Text = suggestedLine.Value;
                            break;
                        default:
                            break;
                    }
                }

                if (Edits)
                {
                    buttonSave.Enabled = true;
                    buttonCancel.Enabled = true;
                }
                else
                {
                    buttonSave.Enabled = false;
                    buttonCancel.Enabled = false;
                }

                if (Control.CanListen)
                {
                    buttonListen.Enabled = true;
                    buttonBroadcast.Enabled = true;
                }
                else
                {
                    buttonListen.Enabled = false;
                    buttonBroadcast.Enabled = false;
                }

                if (Control.Listening)
                {
                    buttonListen.BackColor = Color.FromName("ControlDarkDark");
                    buttonListen.ForeColor = Color.FromName("ControlLightLight");
                }
                else
                {
                    buttonListen.BackColor = Color.FromName("Control");
                    buttonListen.ForeColor = default;
                }

                if (Control.Broadcasting)
                {
                    buttonBroadcast.BackColor = Color.FromName("ControlDarkDark");
                    buttonBroadcast.ForeColor = Color.FromName("ControlLightLight");
                }
                else
                {
                    buttonBroadcast.BackColor = Color.FromName("Control");
                    buttonBroadcast.ForeColor = default;
                }
            }
            else
            {
                buttonSave.Enabled = false;
                buttonCancel.Enabled = false;

                if (Network.IsConnected && Network.IsValidATC)
                {
                    comboBoxAirport.Enabled = true;
                    buttonCreate.Enabled = true;
                }
                else
                {
                    comboBoxAirport.Enabled = false;
                    buttonCreate.Enabled = false;
                }

                comboBoxLetter.Enabled = false;
                comboBoxTimecheck.Enabled = false;
                labelCode.Text = string.Empty;
                buttonCreate.Visible = true;
                buttonDelete.Visible = false;
                buttonGetMetar.Enabled = false;
                buttonBroadcast.Enabled = false;
                buttonNext.Enabled = false;
                comboBoxVoice.Enabled = false;
                comboBoxRate.Enabled = false;
                buttonListen.Enabled = false;
                buttonListen.BackColor = Color.FromName("Control");
                buttonListen.ForeColor = default;
                buttonBroadcast.BackColor = Color.FromName("Control");
                buttonBroadcast.ForeColor = default;

                textBoxAPCH.TextChanged -= TextBox_TextChanged;
                textBoxRWY.TextChanged -= TextBox_TextChanged;
                textBoxSFCCOND.TextChanged -= TextBox_TextChanged;
                textBoxOPRINFO.TextChanged -= TextBox_TextChanged;
                textBoxWIND.TextChanged -= TextBox_TextChanged;
                textBoxVIS.TextChanged -= TextBox_TextChanged;
                textBoxCLD.TextChanged -= TextBox_TextChanged;
                textBoxWX.TextChanged -= TextBox_TextChanged;
                textBoxTMP.TextChanged -= TextBox_TextChanged;
                textBoxQNH.TextChanged -= TextBox_TextChanged;
                textBoxSIGWX.TextChanged -= TextBox_TextChanged;
                textBoxOFCW.TextChanged -= TextBox_TextChanged;

                textBoxAPCH.Text = string.Empty;
                textBoxRWY.Text = string.Empty;
                textBoxSFCCOND.Text = string.Empty;
                textBoxOPRINFO.Text = string.Empty;
                textBoxWIND.Text = string.Empty;
                textBoxVIS.Text = string.Empty;
                textBoxCLD.Text = string.Empty;
                textBoxWX.Text = string.Empty;
                textBoxTMP.Text = string.Empty;
                textBoxQNH.Text = string.Empty;
                textBoxSIGWX.Text = string.Empty;
                textBoxOFCW.Text = string.Empty;

                textBoxAPCH.Enabled = false;
                textBoxRWY.Enabled = false;
                textBoxSFCCOND.Enabled = false;
                textBoxOPRINFO.Enabled = false;
                textBoxWIND.Enabled = false;
                textBoxVIS.Enabled = false;
                textBoxCLD.Enabled = false;
                textBoxWX.Enabled = false;
                textBoxTMP.Enabled = false;
                textBoxQNH.Enabled = false;
                textBoxSIGWX.Enabled = false;
                textBoxOFCW.Enabled = false;

                labelWIND.BackColor = default;
                labelVIS.BackColor = default;
                labelCLD.BackColor = default;
                labelWX.BackColor = default;
                labelQNH.BackColor = default;
                labelTMP.BackColor = default;
            }
        }

        private async Task GetMetar()
        {
            labelMETAR.Text = "LOADING";

            await Control.UpdateMetar();

            RefreshForm();
        }

        private void IncreaseID()
        {
            if (ID != 'Z')
            {
                ID = (Char)(Convert.ToUInt16(ID) + 1);

                if (Convert.ToUInt16(ID) >= 90) ID = 'A';
            }

            RefreshForm();
        }

        private async void ButtonCreate_Click(object sender, EventArgs e)
        {
            if (!Network.IsConnected || !Network.IsValidATC) return;

            if (Number == 4 && vatsys.ATIS.IsBroadcasting)
            {
                Errors.Add(new Exception("Cannot start ATIS as you already have 4 running."), Plugin.DisplayName);

                return;
            }

            if (ICAO == null) return;

            var frequency = Plugin.ATISData.Frequencies.FirstOrDefault(x => x.Airport == ICAO);

            if (frequency == null) return;

            var airport = Plugin.Airspace.Airports.FirstOrDefault(x => x.ICAO == ICAO.ToUpper());

            if (airport == null) return;

            Control.Create(ICAO, frequency.Frequency.ToString(), airport.Position);

            await GetMetar();

            ID = 'A';

            comboBoxLetter.SelectedIndex = comboBoxLetter.FindStringExact(ID.ToString());
        }

        private void ComboBoxAirport_SelectedIndexChanged(object sender, EventArgs e)
        {
            ICAO = comboBoxAirport.GetItemText(comboBoxAirport.SelectedItem);
        }

        private async void ButtonDelete_Click(object sender, EventArgs e)
        {
            await Control.Delete();

            Change(Number);
        }

        private async void ButtonGetMetar_Click(object sender, EventArgs e)
        {
            await GetMetar();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            ID = Control.ID;

            TimeCheck = Control.TimeCheck;

            Saves.Clear();

            Control.SuggestedLines.Clear();

            labelWIND.BackColor = default;
            labelVIS.BackColor = default;
            labelCLD.BackColor = default;
            labelWX.BackColor = default;
            labelQNH.BackColor = default;
            labelTMP.BackColor = default;

            RefreshForm();
        }

        private void ButtonNext_Click(object sender, EventArgs e)
        {
            IncreaseID();
        }

        private async void ButtonSave_Click(object sender, EventArgs e)
        {
            await SaveATIS();
        }

        private async Task SaveATIS()
        {
            Control.InstalledVoice = Voice;
            Control.PromptRate = Rate;

            await Control.Save(ID, Saves, TimeCheck);

            Saves.Clear();

            labelWIND.BackColor = default;
            labelVIS.BackColor = default;
            labelCLD.BackColor = default;
            labelWX.BackColor = default;
            labelQNH.BackColor = default;
            labelTMP.BackColor = default;

            RefreshForm();
        }

        private async void ButtonBroadcast_Click(object sender, EventArgs e)
        {
            if (!Control.Broadcasting)
            {
                buttonBroadcast.BackColor = Color.FromName("ControlDarkDark");
                buttonBroadcast.ForeColor = Color.FromName("ControlLightLight");

                if (Saves.Any()) await SaveATIS();

                await Control.BroadcastStart();
            }
            else
            {
                buttonBroadcast.BackColor = default;
                buttonBroadcast.ForeColor = default;

                await Control.BroadcastStop();
            }

            RefreshForm();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            var lineName = string.Empty;

            switch (textBox.Name)
            {
                case "textBoxAPCH":
                    lineName = "APCH";
                    break;
                case "textBoxRWY":
                    lineName = "RWY";
                    break;
                case "textBoxSFCCOND":
                    lineName = "SFC COND";
                    break;
                case "textBoxOPRINFO":
                    lineName = "OPR INFO";
                    break;
                case "textBoxWIND":
                    lineName = "WIND";
                    break;
                case "textBoxVIS":
                    lineName = "VIS";
                    break;
                case "textBoxCLD":
                    lineName = "CLD";
                    break;
                case "textBoxWX":
                    lineName = "WX";
                    break;
                case "textBoxQNH":
                    lineName = "QNH";
                    break;
                case "textBoxTMP":
                    lineName = "TMP";
                    break;
                case "textBoxSIGWX":
                    lineName = "SIGWX";
                    break;
                case "textBoxOFCW":
                    lineName = "OFCW_NOTIFY";
                    break;
                default:
                    break;
            }

            if (lineName == string.Empty) return;

            var existing = Control.Lines.FirstOrDefault(x => x.Name == lineName);

            if (existing == null) return;

            if (existing.Value == textBox.Text) return;

            var save = Saves.FirstOrDefault(x => x.Key == lineName);

            if (save.Key != null)
            {
                Saves.Remove(save.Key);
            }

            Saves.Add(lineName, textBox.Text);

            if (Edits && ID == Control.ID && ID != 'Z')
            {
                IncreaseID();
            }
        }

        private void ComboBoxLetter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            string selectedLetter = (string)comboBox.SelectedItem;

            var ok = Char.TryParse(selectedLetter, out Char selectedChar);

            if (!ok) return;

            if (ID == selectedChar) return;

            ID = selectedChar;

            RefreshForm();
        }

        private void ComboBoxVoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            string selectedVoice = (string)comboBox.SelectedItem;

            var voice = Control.SpeechSynth.GetInstalledVoices().FirstOrDefault(x => x.VoiceInfo.Name == selectedVoice);   

            if (voice == null) return;  

            Voice =  voice;

            RefreshForm();
        }

        private void ComboBoxRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            string rate = (string)comboBox.SelectedItem;

            switch (rate)
            {
                case "Extra Fast":
                    Rate = PromptRate.ExtraFast;
                    break;
                case "Fast":
                    Rate = PromptRate.Fast;
                    break;
                case "Medium":
                    Rate = PromptRate.Medium;
                    break;
                case "Slow":
                    Rate = PromptRate.Slow;
                    break;
                case "Extra Slow":
                    Rate = PromptRate.ExtraSlow;
                    break;
                default:
                    break;
            }

            RefreshForm();
        }

        private void ButtonListen_Click(object sender, EventArgs e)
        {
            if (Control.Listening)
            {
                Control.ListenStop();
            }
            else
            {
                if (!Control.CanListen) return;
                Control.ListenStart();
            }

            RefreshForm();
        }

        private void ButtonATIS1_Click(object sender, EventArgs e)
        {
            Control.SoundPlayer.Stop();
            Change(1);
        }

        private void ButtonATIS2_Click(object sender, EventArgs e)
        {
            Control.SoundPlayer.Stop();
            Change(2);
        }

        private void ButtonATIS3_Click(object sender, EventArgs e)
        {
            Control.SoundPlayer.Stop();
            Change(3);
        }

        private void ButtonATIS4_Click(object sender, EventArgs e)
        {
            Control.SoundPlayer.Stop();
            Change(4);
        }

        private void ComboBoxTimecheck_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            var timecheckOK = bool.TryParse((string)comboBox.SelectedItem, out bool timecheck);

            if (!timecheckOK) return;

            TimeCheck = timecheck;

            RefreshForm();
        }
    }
}
