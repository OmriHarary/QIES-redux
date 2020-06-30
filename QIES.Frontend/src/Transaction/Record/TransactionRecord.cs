namespace QIES.Frontend.Transaction.Record
{
    public class TransactionRecord
    {
        public TransactionCode Code { get; set; }
        public ServiceNumber SourceNumber { get; set; }
        public NumberTickets NumberTickets { get; set; }
        public ServiceNumber DestinationNumber { get; set; }
        public ServiceName ServiceName { get; set; }
        public ServiceDate ServiceDate { get; set; }

        public TransactionRecord(TransactionCode code)
        {

        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
