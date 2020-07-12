using System;
using QIES.Common.Record;
using QIES.Frontend.Session;

namespace QIES.Frontend.Transaction
{
    public class CreateServiceRequest
    {
        public string ServiceNumberIn { get; set; }
        public string ServiceDateIn { get; set; }
        public string ServiceNameIn { get; set; }
        public CreateServiceRequest(string serviceNumberIn, string serviceDateIn, string serviceNameIn) =>
            (ServiceNumberIn, ServiceDateIn, ServiceNameIn) = (serviceNumberIn, serviceDateIn, serviceNameIn);
    }

    public class CreateService
    {
        private const TransactionCode Code = TransactionCode.CRE;

        public static (TransactionRecord, string) MakeTransaction(CreateServiceRequest request)
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

            ServiceDate serviceDate;
            try
            {
                serviceDate = new ServiceDate(request.ServiceDateIn);
            }
            catch (System.ArgumentException)
            {
                return (null, "Invalid service date.");
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
            record.ServiceDate = serviceDate;
            record.ServiceName = serviceName;

            return (record, $"Service {serviceNumber} created on {serviceDate} with the name {serviceName}");
        }
    }
}
