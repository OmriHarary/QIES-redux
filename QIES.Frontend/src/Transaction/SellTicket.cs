using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class SellTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.SEL;

        public SellTicket() => this.record = new TransactionRecord(Code);

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            var serviceNumberIn = manager.Input.TakeInput("Enter service number to sell tickets for.");
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

            int numberTicketsIn;
            NumberTickets numberTickets;
            try
            {
                if (!int.TryParse(manager.Input.TakeInput("Enter number of tickets to sell."), out numberTicketsIn))
                    throw new System.ArgumentException();
                numberTickets = new NumberTickets(numberTicketsIn);
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid number of tickets.");
                return null;
            }

            Console.WriteLine($"{numberTickets} ticket(s) sold for service {serviceNumber}");
            record.SourceNumber = serviceNumber;
            record.NumberTickets = numberTickets;

            return record;
        }
    }
}
