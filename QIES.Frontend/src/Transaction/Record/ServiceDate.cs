namespace QIES.Frontend.Transaction.Record
{
    class ServiceDate : RecordElement
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        private const string Default = "0";

        public ServiceDate(string date)
        {

        }

        public override string ToString()
        {
            return string.Empty;
        }

        public static bool IsValid(string value)
        {
            return false;
        }
    }
}
