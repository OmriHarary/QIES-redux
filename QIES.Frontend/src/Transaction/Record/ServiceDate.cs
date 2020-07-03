namespace QIES.Frontend.Transaction.Record
{
    public class ServiceDate : RecordElement
    {
        public string Date { get; set; }
        private const string Default = "0";

        public ServiceDate(string date)
        {
            if (!IsValid(date))
            {
                throw new System.ArgumentException();
            }
            this.IsSet = true;
            this.Date = date;
        }

        public ServiceDate()
        {
        }

        public override string ToString() => IsSet ? Date : Default;

        public static bool IsValid(string value)
        {
            int y, m, d;
            var yParse = int.TryParse(value.Substring(0, 4), out y);
            var mParse = int.TryParse(value.Substring(4, 2), out m);
            var dParse = int.TryParse(value.Substring(6), out d);

            return (yParse && mParse && dParse)
                    && (y >= 1980 && y <= 2999)
                    && (m >= 1 && m <= 12)
                    && (d >= 1 && d <= 31);
        }
    }
}
