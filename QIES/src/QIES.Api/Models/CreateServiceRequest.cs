using QIES.Api.Models.Validation;

namespace QIES.Api.Models
{
    public class CreateServiceRequest : AuthenticatedRequest
    {
        [ServiceNumber]
        public string ServiceNumber { get; set; }
        [ServiceDate]
        public string ServiceDate { get; set; }
        [ServiceName]
        public string ServiceName { get; set; }
    }
}
