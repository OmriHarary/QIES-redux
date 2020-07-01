using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class SellTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.SEL;

        public SellTicket()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            string serviceNumberIn = manager.Input.TakeInput("Enter service number to sell tickets for.");
            ServiceNumber serviceNumber;
            try
            {
                serviceNumber = new ServiceNumber(serviceNumberIn);
                if (!manager.ServicesList.IsInList(serviceNumber))
                {
                    throw new System.ArgumentException();
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service number.");
                return null;
            }

            int numberTicketsIn = int.Parse(manager.Input.TakeInput("Enter number of tickets to sell."));
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

            record.SourceNumber = serviceNumber;
            record.NumberTickets = numberTickets;

            return record;
        }
    }
}
