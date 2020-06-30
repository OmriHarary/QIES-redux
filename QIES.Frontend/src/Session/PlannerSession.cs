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
            string goodMessage = "Logged in as Agent. Enter command to begin a transaction.";
            string message = goodMessage;
            string command;

            while (run)
            {
                command  = input.TakeInput(message);

                switch (command)
                {
                    case "sellticket":
                        record = SellTicket(input);
                        break;
                    case "changeticket":
                        record = ChangeTicket(input);
                        break;
                    case "cancelticket":
                        record = CancelTicket(input);
                        break;
                    case "createservice":
                        record = CreateService(input);
                        break;
                    case "deleteservice":
                        record = DeleteService(input);
                        break;
                    case "logout":
                        record = Logout(input);
                        run = false;
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
