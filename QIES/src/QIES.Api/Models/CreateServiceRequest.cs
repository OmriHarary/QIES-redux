using QIES.Common.Records;

namespace QIES.Api.Models
{
    public class CreateServiceRequest
    {
        public ServiceNumber ServiceNumber { get; set; }
        public ServiceDate ServiceDate { get; set; }
        public ServiceName ServiceName { get; set; }
    }
}
