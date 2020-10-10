using QIES.Common.Records;

namespace QIES.Api.Models
{
    public class CreateServiceRequest
    {
        /// <summary>Service number of service to create.</summary>
        public ServiceNumber ServiceNumber { get; set; }
        /// <summary>Service date of service to create.</summary>
        public ServiceDate ServiceDate { get; set; }
        /// <summary>Service name of service to create.</summary>
        public ServiceName ServiceName { get; set; }
    }
}
