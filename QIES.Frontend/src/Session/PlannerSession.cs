using QIES.Common;
using QIES.Common.Record;
using QIES.Frontend.Transaction;

namespace QIES.Frontend.Session
{
    public class PlannerSession : ActiveSession
    {
        public const string Prompt = "PLANNER";

        public override void Process(SessionManager manager, TransactionQueue queue)
        {
            var run = true;
            var goodMessage = "Logged in as Planner. Enter command to begin a transaction.";
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
                    case "createservice":
                        record = CreateService(manager);
                        message = goodMessage;
                        break;
                    case "deleteservice":
                        record = DeleteService(manager);
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

        public TransactionRecord CreateService(SessionManager manager)
        {
            CreateService createService = new CreateService();
            return createService.MakeTransaction(manager);
        }

        public TransactionRecord DeleteService(SessionManager manager)
        {
            DeleteService deleteService = new DeleteService();
            return deleteService.MakeTransaction(manager);
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
