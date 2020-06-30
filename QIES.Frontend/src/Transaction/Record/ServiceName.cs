namespace QIES.Frontend.Transaction.Record
{
    public class ServiceName : RecordElement
    {
        public string Name { get; set; }
        private const string Default = "****";

        public ServiceName(string name)
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
