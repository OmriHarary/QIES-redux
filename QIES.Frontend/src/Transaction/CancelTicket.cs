using QIES.Common.Record;

namespace QIES.Frontend.Transaction
{
    public class CancelTicketRequest
    {
        public string ServiceNumberIn { get; set; }
        public int NumberTicketsIn { get; set; }
        public CancelTicketRequest(string serviceNumberIn, int numberTicketsIn) =>
            (ServiceNumberIn, NumberTicketsIn) = (serviceNumberIn, numberTicketsIn);
    }

    public class CancelTicket
    {
        private const TransactionCode Code = TransactionCode.CAN;

        public static (TransactionRecord, string) MakeTransaction(CancelTicketRequest request)
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

            return (record, $"{numberTickets} ticket(s) canceled from service {serviceNumber}");
        }
    }
}
