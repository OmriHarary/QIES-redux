namespace QIES.Backoffice.Parser.Record
{
    public class ServiceName : RecordElement
    {
        public string Name { get; set; }
        private const string Default = "****";

        public ServiceName(string name)
        {
            if (!IsValid(name))
            {
                throw new System.ArgumentException();
            }
            this.IsSet = true;
            this.Name = name;
        }

        public ServiceName()
        {
        }

        public override string ToString()
        {
            if (IsSet)
            {
                return Name;
            }
            return Default;
        }

        public static bool IsValid(string value)
        {
            if ((value.Length >= 3) && (value.Length <= 39))
            {
                if (!(value.StartsWith(' ') || value.EndsWith(' ')))
                {
                    foreach (char c in value)
                    {
                        if (!((char.IsLetterOrDigit(c)) || (c == '\'') || (c == ' ')))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
