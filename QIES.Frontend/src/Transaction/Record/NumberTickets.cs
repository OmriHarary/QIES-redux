namespace QIES.Frontend.Transaction.Record
{
    class NumberTickets : RecordElement
    {
        public int Number { get; set; }
        private const int Default = 0;

        public NumberTickets(int number)
        {

        }

        public override string ToString()
        {
            return string.Empty;
        }

        public static bool IsValid(int value)
        {
            return false;
        }
    }
}
