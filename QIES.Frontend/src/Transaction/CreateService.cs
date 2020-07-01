using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class CreateService : Transaction
    {
        private const TransactionCode Code = TransactionCode.CRE;

        public CreateService()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            string serviceNumberIn = input.TakeInput("Enter service number of the service you wish to create.");
            ServiceNumber serviceNumber;
            try
            {
                serviceNumber = new ServiceNumber(serviceNumberIn);
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service number.");
                return null;
            }

            string serviceDateIn = input.TakeInput("Enter service date of the service you wish to create.");
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

            string serviceNameIn = input.TakeInput("Enter service name of the service you wish to create.");
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

            record.SourceNumber = serviceNumber;
            record.ServiceDate = serviceDate;
            record.ServiceName = serviceName;

            return record;
        }
    }
}
