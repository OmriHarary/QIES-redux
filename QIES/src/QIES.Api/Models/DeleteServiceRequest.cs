using QIES.Api.Models.Validation;

namespace QIES.Api.Models
{
    public class DeleteServiceRequest
    {
        [ServiceName]
        public string ServiceName { get; set; }
    }
}
