using QIES.Frontend.Transaction;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    public class PlannerSession : ActiveSession
    {
        public const string Prompt = "PLANNER";

        public override void Process(SessionManager manager, TransactionQueue queue)
        {
            Input input = manager.Input;
            bool run = true;
            TransactionRecord record = null;
            string goodMessage = "Logged in as Planner. Enter command to begin a transaction.";
            string message = goodMessage;
            string command;

            while (run)
            {
                command  = input.TakeInput(message);

                switch (command)
                {
                    case "sellticket":
                        record = SellTicket(input);
                        message = goodMessage;
                        break;
                    case "changeticket":
                        record = ChangeTicket(input);
                        message = goodMessage;
                        break;
                    case "cancelticket":
                        record = CancelTicket(input);
                        message = goodMessage;
                        break;
                    case "createservice":
                        record = CreateService(input);
                        message = goodMessage;
                        break;
                    case "deleteservice":
                        record = DeleteService(input);
                        message = goodMessage;
                        break;
                    case "logout":
                        record = Logout(input);
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

        public TransactionRecord CreateService(Input input)
        {
            CreateService createService = new CreateService();
            return createService.MakeTransaction(input);
        }

        public TransactionRecord DeleteService(Input input)
        {
            DeleteService deleteService = new DeleteService();
            return deleteService.MakeTransaction(input);
        }

        public TransactionRecord CancelTicket(Input input)
        {
            CancelTicket cancelTicket = new CancelTicket();
            return cancelTicket.MakeTransaction(input);
        }

        public TransactionRecord ChangeTicket(Input input)
        {
            ChangeTicket changeTicket = new ChangeTicket();
            return changeTicket.MakeTransaction(input);
        }
    }
}
