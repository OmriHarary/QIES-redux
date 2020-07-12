using System;
using QIES.Common.Record;
using QIES.Frontend.Session;

namespace QIES.Frontend.Transaction
{
    public class DeleteServiceRequest
    {
        public string ServiceNumberIn { get; set; }
        public string ServiceNameIn { get; set; }
        public DeleteServiceRequest(string serviceNumberIn, string serviceNameIn) =>
            (ServiceNumberIn, ServiceNameIn) = (serviceNumberIn, serviceNameIn);
    }

    public class DeleteService
    {
        private const TransactionCode Code = TransactionCode.DEL;

        public static (TransactionRecord, string) MakeTransaction(DeleteServiceRequest request)
        {
            ServiceNumber serviceNumber;
            try
            {
                serviceNumber = new ServiceNumber(request.ServiceNumberIn);
            }
            catch (System.ArgumentException)
            {
                return (null, "Invalid service number.");
            }

            ServiceName serviceName;
            try
            {
                serviceName = new ServiceName(request.ServiceNameIn);
            }
            catch (System.ArgumentException)
            {
                return (null, "Invalid service name.");
            }

            var record = new TransactionRecord(Code);
            record.SourceNumber = serviceNumber;
            record.ServiceName = serviceName;

            return (record, $"Service {serviceNumber} with service name {serviceName} was deleted");
        }
    }
}
