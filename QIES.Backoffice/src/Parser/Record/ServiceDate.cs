namespace QIES.Backoffice.Parser.Record
{
    public class ServiceDate : RecordElement
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        private const string Default = "0";

        public ServiceDate(string date)
        {
            if (!IsValid(date))
            {
                throw new System.ArgumentException();
            }
            this.IsSet = true;
            this.Year = date.Substring(0, 4);
            this.Month = date.Substring(4, 2);
            this.Day = date.Substring(6);
        }

        public ServiceDate()
        {
        }

        public override string ToString()
        {
            if (IsSet)
            {
                return $"{Year}{Month}{Day}";
            }
            return Default;
        }

        public static bool IsValid(string value)
        {
            int y = int.Parse(value.Substring(0, 4));
            int m = int.Parse(value.Substring(4, 2));
            int d = int.Parse(value.Substring(6));

            return (y >= 1980 && y <= 2999)
                    && (m >= 1 && m <= 12)
                    && (d >= 1 && d <= 31);
        }
    }
}
