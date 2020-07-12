namespace QIES.Common.Record
{
    public class ServiceDate
    {
        private const string Default = "0";
        public string Date { get; private set; }

        public ServiceDate(string date)
        {
            if (!IsValid(date))
            {
                throw new System.ArgumentException();
            }
            this.Date = date;
        }

        public ServiceDate()
        {
            this.Date = Default;
        }

        public override string ToString() => Date;

        public static bool IsValid(string value)
        {
            if (value.Length != 8)
                return false;

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
