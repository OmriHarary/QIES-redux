namespace QIES.Frontend.Transaction.Record
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

        public override string ToString() => IsSet ? Name : Default;

        public static bool IsValid(string value)
        {
            if ((value.Length >= 3) && (value.Length <= 39))
            {
                if (!(value.StartsWith(' ') || value.EndsWith(' ')))
                {
                    foreach (var c in value)
                    {
                        if (!(char.IsLetterOrDigit(c) || (c == '\'') || (c == ' ')))
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
