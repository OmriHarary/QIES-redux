namespace QIES.Frontend.Transaction.Record
{
    public class ServiceNumber : RecordElement
    {
        public string Number { get; set; }
        private const string Default = "00000";

        public ServiceNumber(string number)
        {
            if (!IsValid(number))
            {
                throw new System.ArgumentException();
            }
            this.IsSet = true;
            this.Number = number;
        }

        public ServiceNumber()
        {
        }

        public override string ToString()
        {
            if (IsSet)
            {
                return Number;
            }
            return Default;
        }

        public static bool IsValid(string value)
        {
            if (value.Length == 5 && !value.StartsWith('0'))
            {
                foreach (char c in value)
                {
                    if (!char.IsDigit(c))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
