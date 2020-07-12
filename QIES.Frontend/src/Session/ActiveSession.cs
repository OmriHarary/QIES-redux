using System;
using QIES.Common;
using QIES.Common.Record;
using QIES.Frontend.Transaction;

namespace QIES.Frontend.Session
{
    public abstract class ActiveSession : ISession
    {
        public abstract void Process(SessionManager manager, TransactionQueue queue);

        public TransactionRecord Logout(SessionManager manager)
        {
            var request = new LogoutRequest();
            var (record, message) = Transaction.Logout.MakeTransaction(request);
            Console.WriteLine(message);
            return record;
        }

        public TransactionRecord SellTicket(SessionManager manager)
        {
            var serviceNumberIn = manager.Input.TakeInput("Enter service number to sell tickets for.");
            if (!manager.ServicesList.IsInList(serviceNumberIn))
            {
                Console.WriteLine("Requested service does not exist.");
                return null;
            }

            int numberTicketsIn;
            try
            {
                numberTicketsIn = manager.Input.TakeNumericInput("Enter number of tickets to sell.");
            }
            catch (System.IO.InvalidDataException)
            {
                Console.WriteLine("A number was not entered.");
                return null;
            }

            var request = new SellTicketRequest(serviceNumberIn, numberTicketsIn);
            var (record, message) = Transaction.SellTicket.MakeTransaction(request);
            Console.WriteLine(message);
            return record;
        }
    }
}
