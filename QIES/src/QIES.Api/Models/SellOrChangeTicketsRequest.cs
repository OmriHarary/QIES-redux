using QIES.Common.Records;

namespace QIES.Api.Models
{
    public class SellOrChangeTicketsRequest
    {
        /// <summary>Number of tickets to sell or change.</summary>
        public NumberTickets NumberTickets { get; set; }
        /// <summary>Service number of service to transfer tickets from.</summary>
        public ServiceNumber? SourceServiceNumber { get; set; }
    }
}
