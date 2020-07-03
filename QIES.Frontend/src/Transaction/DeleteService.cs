using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class DeleteService : Transaction
    {
        private const TransactionCode Code = TransactionCode.DEL;

        public DeleteService() => this.record = new TransactionRecord(Code);

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            var serviceNumberIn = manager.Input.TakeInput("Enter service number of the service you wish to delete.");
            ServiceNumber serviceNumber;
            try
            {
                serviceNumber = new ServiceNumber(serviceNumberIn);
                if (!manager.ServicesList.IsInList(serviceNumberIn))
                {
                    throw new System.ArgumentException();
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service number.");
                return null;
            }

            var serviceNameIn = manager.Input.TakeInput("Enter service name of the service you wish to delete.");
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

            Console.WriteLine($"Service {serviceNumber} with service name {serviceName} was deleted");
            manager.ServicesList.DeleteService(serviceNumberIn);
            record.SourceNumber = serviceNumber;
            record.ServiceName = serviceName;

            return record;
        }
    }
}
