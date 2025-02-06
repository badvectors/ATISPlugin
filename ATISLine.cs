namespace ATISPlugin
{
    public class ATISLine
    {
        public int Number { get; set; }
        public string? Name { get; set; }
        public string Type { get; set; } = "Edit";
        public bool NameSpoken { get; set; }
        public bool NumbersGrouped { get; set; }
        public string? Value { get; set ; }
        public bool Visible => !string.IsNullOrWhiteSpace(Value);
        public bool Enabled { get; set; } = true;
        public bool Changed { get; set; }
        public METARField METARField { get; set; }

        public ATISLine(
          string? name,
          int number,
          string? type = "Edit",
          bool nameSpoken = false,
          bool numbersGrouped = false,
          string? value = null,
          METARField field = METARField.None)
        {
            Name = name;
            Number = number;
            Type = type ?? "Edit";
            NameSpoken = nameSpoken;
            NumbersGrouped = numbersGrouped;
            Value = value?.ToUpper();
            METARField = field;
        }
    }
}
