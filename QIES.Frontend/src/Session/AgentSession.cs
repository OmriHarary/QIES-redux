using System.Collections.Generic;
using QIES.Frontend.Transaction;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    public class AgentSession : ActiveSession
    {
        public const string Prompt = " AGENT ";
        public int ChangedTickets { get; set; }
        public int TotalCancelledTickets { get; set; }
        public Dictionary<string, int> CancelledTickets { get; private set; }

        public AgentSession()
        {
            CancelledTickets = new Dictionary<string, int>();
        }

        public override void Process(SessionManager manager, TransactionQueue queue)
        {
            var run = true;
            var goodMessage = "Logged in as Agent. Enter command to begin a transaction.";
            var message = goodMessage;
            TransactionRecord record = null;
            string command;

            while (run)
            {
                command  = manager.Input.TakeInput(message);

                switch (command)
                {
                    case "sellticket":
                        record = SellTicket(manager);
                        message = goodMessage;
                        break;
                    case "changeticket":
                        record = ChangeTicket(manager);
                        message = goodMessage;
                        break;
                    case "cancelticket":
                        record = CancelTicket(manager);
                        message = goodMessage;
                        break;
                    case "logout":
                        record = Logout(manager);
                        run = false;
                        break;
                    default:
                        message = $"Invalid input. {goodMessage}";
                        break;
                }

                if (record != null)
                    queue.Push(record);
            }

            manager.SetSession(new NoSession());
        }

        public TransactionRecord CancelTicket(SessionManager manager)
        {
            CancelTicket cancelTicket = new CancelTicket();
            return cancelTicket.MakeTransaction(manager);
        }

        public TransactionRecord ChangeTicket(SessionManager manager)
        {
            ChangeTicket changeTicket = new ChangeTicket();
            return changeTicket.MakeTransaction(manager);
        }
    }
}
