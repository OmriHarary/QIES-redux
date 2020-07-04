using System;
using QIES.Common.Record;
using QIES.Frontend.Session;

namespace QIES.Frontend.Transaction
{
    public class CreateService : Transaction
    {
        private const TransactionCode Code = TransactionCode.CRE;

        public CreateService() => this.record = new TransactionRecord(Code);

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            var serviceNumberIn = manager.Input.TakeInput("Enter service number of the service you wish to create.");
            ServiceNumber serviceNumber;
            try
            {
                serviceNumber = new ServiceNumber(serviceNumberIn);
                if (manager.ServicesList.IsInList(serviceNumberIn))
                {
                    throw new System.ArgumentException();
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service number.");
                return null;
            }

            var serviceDateIn = manager.Input.TakeInput("Enter service date of the service you wish to create.");
            ServiceDate serviceDate;
            try
            {
                serviceDate = new ServiceDate(serviceDateIn);
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service date.");
                return null;
            }

            var serviceNameIn = manager.Input.TakeInput("Enter service name of the service you wish to create.");
            ServiceName serviceName;
            try
            {
                serviceName = new ServiceName(serviceNameIn);
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service name.");
                return null;
            }

            Console.WriteLine($"Service {serviceNumber} created on {serviceDate} with the name {serviceName}");
            record.SourceNumber = serviceNumber;
            record.ServiceDate = serviceDate;
            record.ServiceName = serviceName;

            return record;
        }
    }
}
