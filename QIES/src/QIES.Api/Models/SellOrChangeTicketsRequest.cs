using QIES.Api.Models.Validation;

namespace QIES.Api.Models
{
    public class SellOrChangeTicketsRequest
    {
        [NumberTickets]
        public string NumberTickets { get; set; }
        [ServiceNumber(allowNull: true)]
        public string? SourceServiceNumber { get; set; }
    }
}
