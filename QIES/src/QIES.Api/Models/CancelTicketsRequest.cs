using QIES.Common.Records;

namespace QIES.Api.Models
{
    public class CancelTicketsRequest
    {
        /// <summary>Number of tickets to cancel.</summary>
        public NumberTickets NumberTickets { get; set; }
    }
}
