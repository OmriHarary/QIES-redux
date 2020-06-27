namespace QIES.Frontend.Transaction.Record
{
    class ServiceNumber : RecordElement
    {
        public string Number { get; set; }
        private const string Default = "00000";

        public ServiceNumber(string number)
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
