namespace QIES.Frontend.Transaction.Record
{
    public class NumberTickets : RecordElement
    {
        public int Number { get; set; }
        private const int Default = 0;

        public NumberTickets(int number)
        {
            if (!IsValid(number))
            {
                throw new System.ArgumentException();
            }
            this.IsSet = true;
            this.Number = number;
        }

        public NumberTickets()
        {
        }

        public override string ToString() => IsSet ? Number.ToString() : Default.ToString();

        public static bool IsValid(int value) => value >= 1 && value <= 1000;
    }
}
