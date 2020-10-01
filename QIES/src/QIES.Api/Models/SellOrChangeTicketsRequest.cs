using QIES.Common.Records;

namespace QIES.Api.Models
{
    public class SellOrChangeTicketsRequest
    {
        public NumberTickets NumberTickets { get; set; }
        public ServiceNumber? SourceServiceNumber { get; set; }
    }
}
