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
            this.Code = code;
            this.SourceNumber = new ServiceNumber();
            this.NumberTickets = new NumberTickets();
            this.DestinationNumber = new ServiceNumber();
            this.ServiceName = new ServiceName();
            this.ServiceDate = new ServiceDate();
        }

        public override string ToString()
        {
            return $"{Code} {SourceNumber} {NumberTickets} {DestinationNumber} {ServiceName} {ServiceDate}";
        }
    }
}
