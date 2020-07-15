using QIES.Common.Record;

namespace QIES.Frontend.Transaction
{
    public class ChangeTicketRequest
    {
        public string SourceNumberIn { get; set; }
        public int NumberTicketsIn { get; set; }
        public string DestinationNumberIn { get; set; }
        public ChangeTicketRequest(string serviceNumberIn, int numberTicketsIn, string destinationNumberIn) =>
            (SourceNumberIn, NumberTicketsIn, DestinationNumberIn) = (serviceNumberIn, numberTicketsIn, destinationNumberIn);
    }

    public class ChangeTicket
    {
        private const TransactionCode Code = TransactionCode.CHG;

        public static (TransactionRecord, string) MakeTransaction(ChangeTicketRequest request)
        {
            ServiceNumber sourceNumber;
            try
            {
                sourceNumber = new ServiceNumber(request.SourceNumberIn);
            }
            catch (System.ArgumentException)
            {
                return (null, "Invalid service number.");
            }

            ServiceNumber destNumber;
            try
            {
                destNumber = new ServiceNumber(request.DestinationNumberIn);

            }
            catch (System.ArgumentException)
            {
                return (null, "Invalid service number.");
            }

            NumberTickets numberTickets;
            try
            {
                numberTickets = new NumberTickets(request.NumberTicketsIn);
            }
            catch (System.ArgumentException)
            {
                return (null, "Invalid number of tickets.");
            }

            var record = new TransactionRecord(Code);
            record.SourceNumber = sourceNumber;
            record.DestinationNumber = destNumber;
            record.NumberTickets = numberTickets;

            return (record, $"{numberTickets} ticket(s) changed from service {sourceNumber} to service {destNumber}");
        }
    }
}
