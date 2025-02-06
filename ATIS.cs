namespace ATISPlugin
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public partial class ATIS
    {

        private ATISFrequency[]? frequenciesField;

        private ATISTranslation[]? translationsField;

        private ATISInput[]? editorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem("Frequency", IsNullable = false)]
        public ATISFrequency[]? Frequencies
        {
            get
            {
                return this.frequenciesField;
            }
            set
            {
                this.frequenciesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem("Translation", IsNullable = false)]
        public ATISTranslation[]? Translations
        {
            get
            {
                return this.translationsField;
            }
            set
            {
                this.translationsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem("Input", IsNullable = false)]
        public ATISInput[]? Editor
        {
            get
            {
                return this.editorField;
            }
            set
            {
                this.editorField = value;
            }
        }
    }

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class ATISFrequency
    {

        private string? airportField;

        private decimal frequencyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? Airport
        {
            get
            {
                return this.airportField;
            }
            set
            {
                this.airportField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public decimal Frequency
        {
            get
            {
                return this.frequencyField;
            }
            set
            {
                this.frequencyField = value;
            }
        }
    }

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class ATISTranslation
    {

        private string? stringField;

        private string? spokenField;

        private string? alphabetField;

        private string? fallbackSpokenField;

        private string? regexField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? String
        {
            get
            {
                return this.stringField;
            }
            set
            {
                this.stringField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? Spoken
        {
            get
            {
                return this.spokenField;
            }
            set
            {
                this.spokenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? Alphabet
        {
            get
            {
                return this.alphabetField;
            }
            set
            {
                this.alphabetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? FallbackSpoken
        {
            get
            {
                return this.fallbackSpokenField;
            }
            set
            {
                this.fallbackSpokenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? Regex
        {
            get
            {
                return this.regexField;
            }
            set
            {
                this.regexField = value;
            }
        }
    }

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class ATISInput
    {

        private string? nameField;

        private bool nameIsSpokenField;

        private bool nameIsSpokenFieldSpecified;

        private string? inputTypeField;

        private bool numbersSpokenGroupedField;

        private bool numbersSpokenGroupedFieldSpecified;

        private string? valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public bool NameIsSpoken
        {
            get
            {
                return this.nameIsSpokenField;
            }
            set
            {
                this.nameIsSpokenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnore()]
        public bool NameIsSpokenSpecified
        {
            get
            {
                return this.nameIsSpokenFieldSpecified;
            }
            set
            {
                this.nameIsSpokenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? InputType
        {
            get
            {
                return this.inputTypeField;
            }
            set
            {
                this.inputTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public bool NumbersSpokenGrouped
        {
            get
            {
                return this.numbersSpokenGroupedField;
            }
            set
            {
                this.numbersSpokenGroupedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnore()]
        public bool NumbersSpokenGroupedSpecified
        {
            get
            {
                return this.numbersSpokenGroupedFieldSpecified;
            }
            set
            {
                this.numbersSpokenGroupedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string? value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }


}
