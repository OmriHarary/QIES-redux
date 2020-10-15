using QIES.Common.Records;

namespace QIES.Api.Models
{
    public class DeleteServiceRequest
    {
        /// <summary>Service number of service to delete.</summary>
        public ServiceName ServiceName { get; set; }
    }
}
