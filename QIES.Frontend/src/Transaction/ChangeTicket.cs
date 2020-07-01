using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class ChangeTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.CHG;

        public ChangeTicket()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            string sourceNumberIn = manager.Input.TakeInput("Enter service number of the service you want to change.");
            ServiceNumber sourceNumber;
            try
            {
                sourceNumber = new ServiceNumber(sourceNumberIn);
                if (!manager.ServicesList.IsInList(sourceNumber))
                {
                    throw new System.ArgumentException();
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service number.");
                return null;
            }

            string destNumberIn = manager.Input.TakeInput("Enter service number of the service you want to change to.");
            ServiceNumber destNumber;
            try
            {
                destNumber = new ServiceNumber(destNumberIn);
                if (!manager.ServicesList.IsInList(destNumber))
                {
                    throw new System.ArgumentException();
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service number.");
                return null;
            }

            int numberTicketsIn = int.Parse(manager.Input.TakeInput("Enter number of tickets to change."));
            NumberTickets numberTickets;
            try
            {
                numberTickets = new NumberTickets(numberTicketsIn);
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid number of tickets.");
                return null;
            }

            record.SourceNumber = sourceNumber;
            record.DestinationNumber = destNumber;
            record.NumberTickets = numberTickets;

            return record;
        }
    }
}
