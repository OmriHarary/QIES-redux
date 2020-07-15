using QIES.Common.Record;

namespace QIES.Frontend.Transaction
{
    public class SellTicketRequest
    {
        public string ServiceNumberIn { get; set; }
        public int NumberTicketsIn { get; set; }
        public SellTicketRequest(string serviceNumberIn, int numberTicketsIn) =>
            (ServiceNumberIn, NumberTicketsIn) = (serviceNumberIn, numberTicketsIn);
    }

    public class SellTicket
    {
        private const TransactionCode Code = TransactionCode.SEL;

        public static (TransactionRecord?, string) MakeTransaction(SellTicketRequest request)
        {
            ServiceNumber serviceNumber;
            try
            {
                serviceNumber = new ServiceNumber(request.ServiceNumberIn);
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
            record.SourceNumber = serviceNumber;
            record.NumberTickets = numberTickets;

            return (record, $"{numberTickets} ticket(s) sold for service {serviceNumber}");
        }
    }
}
