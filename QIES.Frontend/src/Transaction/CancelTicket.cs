using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class CancelTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.CAN;

        public CancelTicket()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            string serviceNumberIn = manager.Input.TakeInput("Enter service number of ticket you would like to cancel.");
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

            int numberTicketsIn = int.Parse(manager.Input.TakeInput("Enter number of tickets you want to cancel."));
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
