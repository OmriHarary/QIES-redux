using QIES.Api.Models.Validation;

namespace QIES.Api.Models
{
    public class CancelTicketsRequest : AuthenticatedRequest
    {
        [NumberTickets]
        public string NumberTickets { get; set; }
    }
}
