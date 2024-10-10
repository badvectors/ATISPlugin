namespace ATISPlugin
{
    partial class EditorWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelICAO = new System.Windows.Forms.Label();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.comboBoxAirport = new System.Windows.Forms.ComboBox();
            this.textBoxAPCH = new System.Windows.Forms.TextBox();
            this.textBoxRWY = new System.Windows.Forms.TextBox();
            this.textBoxSFCCOND = new System.Windows.Forms.TextBox();
            this.labelAPCH = new System.Windows.Forms.Label();
            this.labelRWY = new System.Windows.Forms.Label();
            this.labelSFCCOND = new System.Windows.Forms.Label();
            this.textBoxWIND = new System.Windows.Forms.TextBox();
            this.textBoxVIS = new System.Windows.Forms.TextBox();
            this.textBoxCLD = new System.Windows.Forms.TextBox();
            this.textBoxWX = new System.Windows.Forms.TextBox();
            this.textBoxTMP = new System.Windows.Forms.TextBox();
            this.textBoxQNH = new System.Windows.Forms.TextBox();
            this.textBoxSIGWX = new System.Windows.Forms.TextBox();
            this.textBoxOFCW = new System.Windows.Forms.TextBox();
            this.comboBoxTimecheck = new System.Windows.Forms.ComboBox();
            this.labelOPRINFO = new System.Windows.Forms.Label();
            this.textBoxOPRINFO = new System.Windows.Forms.TextBox();
            this.labelWIND = new System.Windows.Forms.Label();
            this.labelVIS = new System.Windows.Forms.Label();
            this.labelCLD = new System.Windows.Forms.Label();
            this.labelWX = new System.Windows.Forms.Label();
            this.labelQNH = new System.Windows.Forms.Label();
            this.labelTMP = new System.Windows.Forms.Label();
            this.labelSIGWX = new System.Windows.Forms.Label();
            this.labelOFCW = new System.Windows.Forms.Label();
            this.labelTimeCheck = new System.Windows.Forms.Label();
            this.comboBoxLetter = new System.Windows.Forms.ComboBox();
            this.buttonNext = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.labelMETAR = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonListen = new System.Windows.Forms.Button();
            this.buttonGetMetar = new System.Windows.Forms.Button();
            this.buttonBroadcast = new System.Windows.Forms.Button();
            this.comboBoxVoice = new System.Windows.Forms.ComboBox();
            this.comboBoxRate = new System.Windows.Forms.ComboBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.buttonATIS1 = new System.Windows.Forms.Button();
            this.buttonATIS2 = new System.Windows.Forms.Button();
            this.buttonATIS3 = new System.Windows.Forms.Button();
            this.buttonATIS4 = new System.Windows.Forms.Button();
            this.textBoxZulu = new System.Windows.Forms.TextBox();
            this.buttonZulu = new System.Windows.Forms.Button();
            this.comboBoxZuluFrequency = new System.Windows.Forms.ComboBox();
            this.labelFrequency = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelICAO
            // 
            this.labelICAO.AutoSize = true;
            this.labelICAO.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelICAO.Location = new System.Drawing.Point(44, 66);
            this.labelICAO.Name = "labelICAO";
            this.labelICAO.Size = new System.Drawing.Size(64, 17);
            this.labelICAO.TabIndex = 0;
            this.labelICAO.Text = "Airport";
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(384, 60);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(100, 28);
            this.buttonCreate.TabIndex = 4;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.ButtonCreate_Click);
            // 
            // comboBoxAirport
            // 
            this.comboBoxAirport.BackColor = System.Drawing.SystemColors.ControlDark;
            this.comboBoxAirport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAirport.FormattingEnabled = true;
            this.comboBoxAirport.Location = new System.Drawing.Point(114, 62);
            this.comboBoxAirport.Name = "comboBoxAirport";
            this.comboBoxAirport.Size = new System.Drawing.Size(264, 25);
            this.comboBoxAirport.TabIndex = 1;
            this.comboBoxAirport.SelectedIndexChanged += new System.EventHandler(this.ComboBoxAirport_SelectedIndexChanged);
            // 
            // textBoxAPCH
            // 
            this.textBoxAPCH.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxAPCH.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxAPCH.Location = new System.Drawing.Point(114, 219);
            this.textBoxAPCH.Name = "textBoxAPCH";
            this.textBoxAPCH.Size = new System.Drawing.Size(370, 25);
            this.textBoxAPCH.TabIndex = 5;
            // 
            // textBoxRWY
            // 
            this.textBoxRWY.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxRWY.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxRWY.Location = new System.Drawing.Point(114, 250);
            this.textBoxRWY.Name = "textBoxRWY";
            this.textBoxRWY.Size = new System.Drawing.Size(370, 25);
            this.textBoxRWY.TabIndex = 6;
            // 
            // textBoxSFCCOND
            // 
            this.textBoxSFCCOND.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxSFCCOND.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxSFCCOND.Location = new System.Drawing.Point(114, 281);
            this.textBoxSFCCOND.Name = "textBoxSFCCOND";
            this.textBoxSFCCOND.Size = new System.Drawing.Size(370, 25);
            this.textBoxSFCCOND.TabIndex = 7;
            // 
            // labelAPCH
            // 
            this.labelAPCH.AutoSize = true;
            this.labelAPCH.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelAPCH.Location = new System.Drawing.Point(68, 222);
            this.labelAPCH.Name = "labelAPCH";
            this.labelAPCH.Size = new System.Drawing.Size(40, 17);
            this.labelAPCH.TabIndex = 10;
            this.labelAPCH.Text = "APCH";
            this.labelAPCH.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelRWY
            // 
            this.labelRWY.AutoSize = true;
            this.labelRWY.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelRWY.Location = new System.Drawing.Point(76, 253);
            this.labelRWY.Name = "labelRWY";
            this.labelRWY.Size = new System.Drawing.Size(32, 17);
            this.labelRWY.TabIndex = 11;
            this.labelRWY.Text = "RWY";
            this.labelRWY.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelSFCCOND
            // 
            this.labelSFCCOND.AutoSize = true;
            this.labelSFCCOND.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelSFCCOND.Location = new System.Drawing.Point(36, 284);
            this.labelSFCCOND.Name = "labelSFCCOND";
            this.labelSFCCOND.Size = new System.Drawing.Size(72, 17);
            this.labelSFCCOND.TabIndex = 12;
            this.labelSFCCOND.Text = "SFC COND";
            this.labelSFCCOND.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxWIND
            // 
            this.textBoxWIND.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxWIND.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxWIND.Location = new System.Drawing.Point(114, 343);
            this.textBoxWIND.Name = "textBoxWIND";
            this.textBoxWIND.Size = new System.Drawing.Size(370, 25);
            this.textBoxWIND.TabIndex = 9;
            // 
            // textBoxVIS
            // 
            this.textBoxVIS.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxVIS.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxVIS.Location = new System.Drawing.Point(114, 374);
            this.textBoxVIS.Name = "textBoxVIS";
            this.textBoxVIS.Size = new System.Drawing.Size(370, 25);
            this.textBoxVIS.TabIndex = 10;
            // 
            // textBoxCLD
            // 
            this.textBoxCLD.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxCLD.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxCLD.Location = new System.Drawing.Point(114, 405);
            this.textBoxCLD.Name = "textBoxCLD";
            this.textBoxCLD.Size = new System.Drawing.Size(370, 25);
            this.textBoxCLD.TabIndex = 11;
            // 
            // textBoxWX
            // 
            this.textBoxWX.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxWX.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxWX.Location = new System.Drawing.Point(114, 436);
            this.textBoxWX.Name = "textBoxWX";
            this.textBoxWX.Size = new System.Drawing.Size(370, 25);
            this.textBoxWX.TabIndex = 12;
            // 
            // textBoxTMP
            // 
            this.textBoxTMP.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxTMP.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxTMP.Location = new System.Drawing.Point(114, 467);
            this.textBoxTMP.Name = "textBoxTMP";
            this.textBoxTMP.Size = new System.Drawing.Size(370, 25);
            this.textBoxTMP.TabIndex = 13;
            // 
            // textBoxQNH
            // 
            this.textBoxQNH.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxQNH.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxQNH.Location = new System.Drawing.Point(114, 498);
            this.textBoxQNH.Name = "textBoxQNH";
            this.textBoxQNH.Size = new System.Drawing.Size(370, 25);
            this.textBoxQNH.TabIndex = 14;
            // 
            // textBoxSIGWX
            // 
            this.textBoxSIGWX.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxSIGWX.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxSIGWX.Location = new System.Drawing.Point(114, 529);
            this.textBoxSIGWX.Name = "textBoxSIGWX";
            this.textBoxSIGWX.Size = new System.Drawing.Size(370, 25);
            this.textBoxSIGWX.TabIndex = 15;
            // 
            // textBoxOFCW
            // 
            this.textBoxOFCW.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxOFCW.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxOFCW.Location = new System.Drawing.Point(114, 559);
            this.textBoxOFCW.Name = "textBoxOFCW";
            this.textBoxOFCW.Size = new System.Drawing.Size(370, 25);
            this.textBoxOFCW.TabIndex = 16;
            // 
            // comboBoxTimecheck
            // 
            this.comboBoxTimecheck.BackColor = System.Drawing.SystemColors.ControlDark;
            this.comboBoxTimecheck.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTimecheck.FormattingEnabled = true;
            this.comboBoxTimecheck.Items.AddRange(new object[] {
            "True",
            "False"});
            this.comboBoxTimecheck.Location = new System.Drawing.Point(114, 591);
            this.comboBoxTimecheck.Name = "comboBoxTimecheck";
            this.comboBoxTimecheck.Size = new System.Drawing.Size(121, 25);
            this.comboBoxTimecheck.TabIndex = 17;
            this.comboBoxTimecheck.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTimecheck_SelectedIndexChanged);
            // 
            // labelOPRINFO
            // 
            this.labelOPRINFO.AutoSize = true;
            this.labelOPRINFO.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelOPRINFO.Location = new System.Drawing.Point(36, 315);
            this.labelOPRINFO.Name = "labelOPRINFO";
            this.labelOPRINFO.Size = new System.Drawing.Size(72, 17);
            this.labelOPRINFO.TabIndex = 22;
            this.labelOPRINFO.Text = "OPR INFO";
            this.labelOPRINFO.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxOPRINFO
            // 
            this.textBoxOPRINFO.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxOPRINFO.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxOPRINFO.Location = new System.Drawing.Point(114, 312);
            this.textBoxOPRINFO.Name = "textBoxOPRINFO";
            this.textBoxOPRINFO.Size = new System.Drawing.Size(370, 25);
            this.textBoxOPRINFO.TabIndex = 8;
            // 
            // labelWIND
            // 
            this.labelWIND.AutoSize = true;
            this.labelWIND.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelWIND.Location = new System.Drawing.Point(68, 346);
            this.labelWIND.Name = "labelWIND";
            this.labelWIND.Size = new System.Drawing.Size(40, 17);
            this.labelWIND.TabIndex = 24;
            this.labelWIND.Text = "WIND";
            this.labelWIND.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelVIS
            // 
            this.labelVIS.AutoSize = true;
            this.labelVIS.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelVIS.Location = new System.Drawing.Point(76, 377);
            this.labelVIS.Name = "labelVIS";
            this.labelVIS.Size = new System.Drawing.Size(32, 17);
            this.labelVIS.TabIndex = 25;
            this.labelVIS.Text = "VIS";
            this.labelVIS.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCLD
            // 
            this.labelCLD.AutoSize = true;
            this.labelCLD.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelCLD.Location = new System.Drawing.Point(76, 408);
            this.labelCLD.Name = "labelCLD";
            this.labelCLD.Size = new System.Drawing.Size(32, 17);
            this.labelCLD.TabIndex = 26;
            this.labelCLD.Text = "CLD";
            this.labelCLD.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelWX
            // 
            this.labelWX.AutoSize = true;
            this.labelWX.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelWX.Location = new System.Drawing.Point(84, 439);
            this.labelWX.Name = "labelWX";
            this.labelWX.Size = new System.Drawing.Size(24, 17);
            this.labelWX.TabIndex = 27;
            this.labelWX.Text = "WX";
            this.labelWX.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelQNH
            // 
            this.labelQNH.AutoSize = true;
            this.labelQNH.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelQNH.Location = new System.Drawing.Point(76, 501);
            this.labelQNH.Name = "labelQNH";
            this.labelQNH.Size = new System.Drawing.Size(32, 17);
            this.labelQNH.TabIndex = 28;
            this.labelQNH.Text = "QNH";
            this.labelQNH.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTMP
            // 
            this.labelTMP.AutoSize = true;
            this.labelTMP.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTMP.Location = new System.Drawing.Point(76, 470);
            this.labelTMP.Name = "labelTMP";
            this.labelTMP.Size = new System.Drawing.Size(32, 17);
            this.labelTMP.TabIndex = 29;
            this.labelTMP.Text = "TMP";
            this.labelTMP.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelSIGWX
            // 
            this.labelSIGWX.AutoSize = true;
            this.labelSIGWX.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelSIGWX.Location = new System.Drawing.Point(60, 532);
            this.labelSIGWX.Name = "labelSIGWX";
            this.labelSIGWX.Size = new System.Drawing.Size(48, 17);
            this.labelSIGWX.TabIndex = 30;
            this.labelSIGWX.Text = "SIGWX";
            this.labelSIGWX.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelOFCW
            // 
            this.labelOFCW.AutoSize = true;
            this.labelOFCW.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelOFCW.Location = new System.Drawing.Point(12, 562);
            this.labelOFCW.Name = "labelOFCW";
            this.labelOFCW.Size = new System.Drawing.Size(96, 17);
            this.labelOFCW.TabIndex = 31;
            this.labelOFCW.Text = "OFCW_NOTIFY";
            this.labelOFCW.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTimeCheck
            // 
            this.labelTimeCheck.AutoSize = true;
            this.labelTimeCheck.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTimeCheck.Location = new System.Drawing.Point(20, 594);
            this.labelTimeCheck.Name = "labelTimeCheck";
            this.labelTimeCheck.Size = new System.Drawing.Size(88, 17);
            this.labelTimeCheck.TabIndex = 32;
            this.labelTimeCheck.Text = "TIME_CHECK";
            this.labelTimeCheck.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // comboBoxLetter
            // 
            this.comboBoxLetter.BackColor = System.Drawing.SystemColors.ControlDark;
            this.comboBoxLetter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLetter.FormattingEnabled = true;
            this.comboBoxLetter.Location = new System.Drawing.Point(114, 188);
            this.comboBoxLetter.Name = "comboBoxLetter";
            this.comboBoxLetter.Size = new System.Drawing.Size(121, 25);
            this.comboBoxLetter.TabIndex = 4;
            this.comboBoxLetter.SelectedIndexChanged += new System.EventHandler(this.ComboBoxLetter_SelectedIndexChanged);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(241, 187);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(100, 28);
            this.buttonNext.TabIndex = 34;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.ButtonNext_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label11.Location = new System.Drawing.Point(68, 193);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(40, 17);
            this.label11.TabIndex = 35;
            this.label11.Text = "ATIS";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelCode.Location = new System.Drawing.Point(30, 193);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(40, 17);
            this.labelCode.TabIndex = 36;
            this.labelCode.Text = "XXXX";
            this.labelCode.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelMETAR
            // 
            this.labelMETAR.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelMETAR.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelMETAR.Location = new System.Drawing.Point(10, 161);
            this.labelMETAR.Name = "labelMETAR";
            this.labelMETAR.Size = new System.Drawing.Size(474, 19);
            this.labelMETAR.TabIndex = 37;
            this.labelMETAR.Text = "LOADING";
            this.labelMETAR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(114, 627);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(182, 28);
            this.buttonSave.TabIndex = 18;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(302, 627);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(182, 28);
            this.buttonCancel.TabIndex = 19;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonListen
            // 
            this.buttonListen.BackColor = System.Drawing.SystemColors.Control;
            this.buttonListen.Location = new System.Drawing.Point(278, 125);
            this.buttonListen.Name = "buttonListen";
            this.buttonListen.Size = new System.Drawing.Size(100, 28);
            this.buttonListen.TabIndex = 40;
            this.buttonListen.Text = "Listen";
            this.buttonListen.UseVisualStyleBackColor = false;
            this.buttonListen.Click += new System.EventHandler(this.ButtonListen_Click);
            // 
            // buttonGetMetar
            // 
            this.buttonGetMetar.BackColor = System.Drawing.SystemColors.Control;
            this.buttonGetMetar.Location = new System.Drawing.Point(172, 125);
            this.buttonGetMetar.Name = "buttonGetMetar";
            this.buttonGetMetar.Size = new System.Drawing.Size(100, 28);
            this.buttonGetMetar.TabIndex = 41;
            this.buttonGetMetar.Text = "Load METAR";
            this.buttonGetMetar.UseVisualStyleBackColor = false;
            this.buttonGetMetar.Click += new System.EventHandler(this.ButtonGetMetar_Click);
            // 
            // buttonBroadcast
            // 
            this.buttonBroadcast.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonBroadcast.Location = new System.Drawing.Point(384, 125);
            this.buttonBroadcast.Name = "buttonBroadcast";
            this.buttonBroadcast.Size = new System.Drawing.Size(100, 28);
            this.buttonBroadcast.TabIndex = 42;
            this.buttonBroadcast.Text = "Broadcast";
            this.buttonBroadcast.UseVisualStyleBackColor = true;
            this.buttonBroadcast.Click += new System.EventHandler(this.ButtonBroadcast_Click);
            // 
            // comboBoxVoice
            // 
            this.comboBoxVoice.BackColor = System.Drawing.SystemColors.ControlDark;
            this.comboBoxVoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVoice.Enabled = false;
            this.comboBoxVoice.FormattingEnabled = true;
            this.comboBoxVoice.Location = new System.Drawing.Point(114, 94);
            this.comboBoxVoice.Name = "comboBoxVoice";
            this.comboBoxVoice.Size = new System.Drawing.Size(264, 25);
            this.comboBoxVoice.TabIndex = 2;
            this.comboBoxVoice.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVoice_SelectedIndexChanged);
            // 
            // comboBoxRate
            // 
            this.comboBoxRate.BackColor = System.Drawing.SystemColors.ControlDark;
            this.comboBoxRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRate.Enabled = false;
            this.comboBoxRate.FormattingEnabled = true;
            this.comboBoxRate.Items.AddRange(new object[] {
            "Extra Fast",
            "Fast",
            "Medium",
            "Slow",
            "Extra Slow"});
            this.comboBoxRate.Location = new System.Drawing.Point(384, 94);
            this.comboBoxRate.Name = "comboBoxRate";
            this.comboBoxRate.Size = new System.Drawing.Size(100, 25);
            this.comboBoxRate.TabIndex = 3;
            this.comboBoxRate.SelectedIndexChanged += new System.EventHandler(this.ComboBoxRate_SelectedIndexChanged);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(384, 60);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(100, 28);
            this.buttonDelete.TabIndex = 45;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label12.Location = new System.Drawing.Point(60, 97);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 17);
            this.label12.TabIndex = 46;
            this.label12.Text = "Voice";
            // 
            // buttonATIS1
            // 
            this.buttonATIS1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonATIS1.Location = new System.Drawing.Point(10, 8);
            this.buttonATIS1.Name = "buttonATIS1";
            this.buttonATIS1.Size = new System.Drawing.Size(114, 44);
            this.buttonATIS1.TabIndex = 47;
            this.buttonATIS1.Text = "ATIS #1";
            this.buttonATIS1.UseVisualStyleBackColor = true;
            this.buttonATIS1.Click += new System.EventHandler(this.ButtonATIS1_Click);
            // 
            // buttonATIS2
            // 
            this.buttonATIS2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonATIS2.Location = new System.Drawing.Point(130, 8);
            this.buttonATIS2.Name = "buttonATIS2";
            this.buttonATIS2.Size = new System.Drawing.Size(114, 44);
            this.buttonATIS2.TabIndex = 48;
            this.buttonATIS2.Text = "ATIS #2";
            this.buttonATIS2.UseVisualStyleBackColor = true;
            this.buttonATIS2.Click += new System.EventHandler(this.ButtonATIS2_Click);
            // 
            // buttonATIS3
            // 
            this.buttonATIS3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonATIS3.Location = new System.Drawing.Point(250, 8);
            this.buttonATIS3.Name = "buttonATIS3";
            this.buttonATIS3.Size = new System.Drawing.Size(114, 44);
            this.buttonATIS3.TabIndex = 49;
            this.buttonATIS3.Text = "ATIS #3";
            this.buttonATIS3.UseVisualStyleBackColor = true;
            this.buttonATIS3.Click += new System.EventHandler(this.ButtonATIS3_Click);
            // 
            // buttonATIS4
            // 
            this.buttonATIS4.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonATIS4.Location = new System.Drawing.Point(370, 8);
            this.buttonATIS4.Name = "buttonATIS4";
            this.buttonATIS4.Size = new System.Drawing.Size(114, 44);
            this.buttonATIS4.TabIndex = 50;
            this.buttonATIS4.Text = "ATIS #4";
            this.buttonATIS4.UseVisualStyleBackColor = true;
            this.buttonATIS4.Click += new System.EventHandler(this.ButtonATIS4_Click);
            // 
            // textBoxZulu
            // 
            this.textBoxZulu.BackColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxZulu.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxZulu.Location = new System.Drawing.Point(114, 188);
            this.textBoxZulu.Multiline = true;
            this.textBoxZulu.Name = "textBoxZulu";
            this.textBoxZulu.Size = new System.Drawing.Size(370, 397);
            this.textBoxZulu.TabIndex = 52;
            this.textBoxZulu.Visible = false;
            this.textBoxZulu.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // buttonZulu
            // 
            this.buttonZulu.Location = new System.Drawing.Point(114, 125);
            this.buttonZulu.Name = "buttonZulu";
            this.buttonZulu.Size = new System.Drawing.Size(52, 28);
            this.buttonZulu.TabIndex = 53;
            this.buttonZulu.Text = "Z";
            this.buttonZulu.UseVisualStyleBackColor = true;
            this.buttonZulu.Click += new System.EventHandler(this.ButtonZulu_Click);
            // 
            // comboBoxZuluFrequency
            // 
            this.comboBoxZuluFrequency.BackColor = System.Drawing.SystemColors.ControlDark;
            this.comboBoxZuluFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxZuluFrequency.FormattingEnabled = true;
            this.comboBoxZuluFrequency.Location = new System.Drawing.Point(114, 591);
            this.comboBoxZuluFrequency.Name = "comboBoxZuluFrequency";
            this.comboBoxZuluFrequency.Size = new System.Drawing.Size(370, 25);
            this.comboBoxZuluFrequency.TabIndex = 54;
            this.comboBoxZuluFrequency.SelectedIndexChanged += new System.EventHandler(this.ComboBoxZuluFrequency_SelectedIndexChanged);
            // 
            // labelFrequency
            // 
            this.labelFrequency.AutoSize = true;
            this.labelFrequency.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelFrequency.Location = new System.Drawing.Point(28, 594);
            this.labelFrequency.Name = "labelFrequency";
            this.labelFrequency.Size = new System.Drawing.Size(80, 17);
            this.labelFrequency.TabIndex = 55;
            this.labelFrequency.Text = "FREQUENCY";
            this.labelFrequency.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // EditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ClientSize = new System.Drawing.Size(496, 672);
            this.Controls.Add(this.labelFrequency);
            this.Controls.Add(this.comboBoxZuluFrequency);
            this.Controls.Add(this.buttonZulu);
            this.Controls.Add(this.textBoxZulu);
            this.Controls.Add(this.buttonATIS4);
            this.Controls.Add(this.buttonATIS3);
            this.Controls.Add(this.buttonATIS2);
            this.Controls.Add(this.buttonATIS1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.comboBoxRate);
            this.Controls.Add(this.comboBoxVoice);
            this.Controls.Add(this.buttonBroadcast);
            this.Controls.Add(this.buttonGetMetar);
            this.Controls.Add(this.buttonListen);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelMETAR);
            this.Controls.Add(this.labelCode);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.comboBoxLetter);
            this.Controls.Add(this.labelTimeCheck);
            this.Controls.Add(this.labelOFCW);
            this.Controls.Add(this.labelSIGWX);
            this.Controls.Add(this.labelTMP);
            this.Controls.Add(this.labelQNH);
            this.Controls.Add(this.labelWX);
            this.Controls.Add(this.labelCLD);
            this.Controls.Add(this.labelVIS);
            this.Controls.Add(this.labelWIND);
            this.Controls.Add(this.textBoxOPRINFO);
            this.Controls.Add(this.labelOPRINFO);
            this.Controls.Add(this.comboBoxTimecheck);
            this.Controls.Add(this.textBoxOFCW);
            this.Controls.Add(this.textBoxSIGWX);
            this.Controls.Add(this.textBoxQNH);
            this.Controls.Add(this.textBoxTMP);
            this.Controls.Add(this.textBoxWX);
            this.Controls.Add(this.textBoxCLD);
            this.Controls.Add(this.textBoxVIS);
            this.Controls.Add(this.textBoxWIND);
            this.Controls.Add(this.labelSFCCOND);
            this.Controls.Add(this.labelRWY);
            this.Controls.Add(this.labelAPCH);
            this.Controls.Add(this.textBoxSFCCOND);
            this.Controls.Add(this.textBoxRWY);
            this.Controls.Add(this.textBoxAPCH);
            this.Controls.Add(this.comboBoxAirport);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.labelICAO);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HideOnClose = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(500, 700);
            this.MinimumSize = new System.Drawing.Size(500, 700);
            this.Name = "EditorWindow";
            this.Resizeable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ATIS Editor";
            this.TitleTextColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EditorWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelICAO;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.ComboBox comboBoxAirport;
        private System.Windows.Forms.TextBox textBoxAPCH;
        private System.Windows.Forms.TextBox textBoxRWY;
        private System.Windows.Forms.TextBox textBoxSFCCOND;
        private System.Windows.Forms.Label labelAPCH;
        private System.Windows.Forms.Label labelRWY;
        private System.Windows.Forms.Label labelSFCCOND;
        private System.Windows.Forms.TextBox textBoxWIND;
        private System.Windows.Forms.TextBox textBoxVIS;
        private System.Windows.Forms.TextBox textBoxCLD;
        private System.Windows.Forms.TextBox textBoxWX;
        private System.Windows.Forms.TextBox textBoxTMP;
        private System.Windows.Forms.TextBox textBoxQNH;
        private System.Windows.Forms.TextBox textBoxSIGWX;
        private System.Windows.Forms.TextBox textBoxOFCW;
        private System.Windows.Forms.ComboBox comboBoxTimecheck;
        private System.Windows.Forms.Label labelOPRINFO;
        private System.Windows.Forms.TextBox textBoxOPRINFO;
        private System.Windows.Forms.Label labelWIND;
        private System.Windows.Forms.Label labelVIS;
        private System.Windows.Forms.Label labelCLD;
        private System.Windows.Forms.Label labelWX;
        private System.Windows.Forms.Label labelQNH;
        private System.Windows.Forms.Label labelTMP;
        private System.Windows.Forms.Label labelSIGWX;
        private System.Windows.Forms.Label labelOFCW;
        private System.Windows.Forms.Label labelTimeCheck;
        private System.Windows.Forms.ComboBox comboBoxLetter;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.Label labelMETAR;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonListen;
        private System.Windows.Forms.Button buttonGetMetar;
        private System.Windows.Forms.Button buttonBroadcast;
        private System.Windows.Forms.ComboBox comboBoxVoice;
        private System.Windows.Forms.ComboBox comboBoxRate;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button buttonATIS1;
        private System.Windows.Forms.Button buttonATIS2;
        private System.Windows.Forms.Button buttonATIS3;
        private System.Windows.Forms.Button buttonATIS4;
        private System.Windows.Forms.TextBox textBoxZulu;
        private System.Windows.Forms.Button buttonZulu;
        private System.Windows.Forms.ComboBox comboBoxZuluFrequency;
        private System.Windows.Forms.Label labelFrequency;
    }
}