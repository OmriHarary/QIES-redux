/* using System;
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
                command = manager.Input.TakeInput(message);

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
            var serviceNumberIn = manager.Input.TakeInput("Enter service number of the service you wish to create.");
            if (manager.ServicesList.IsInList(serviceNumberIn))
            {
                Console.WriteLine("Requested service already exists.");
                return null;
            }

            var serviceDateIn = manager.Input.TakeInput("Enter service date of the service you wish to create.");

            var serviceNameIn = manager.Input.TakeInput("Enter service name of the service you wish to create.");

            var request = new CreateServiceRequest(serviceNumberIn, serviceDateIn, serviceNameIn);
            var (record, message) = Transaction.CreateService.MakeTransaction(request);
            Console.WriteLine(message);
            return record;
        }

        public TransactionRecord DeleteService(SessionManager manager)
        {
            var serviceNumberIn = manager.Input.TakeInput("Enter service number of the service you wish to delete.");
            if (!manager.ServicesList.IsInList(serviceNumberIn))
            {
                Console.WriteLine("Requested service does not exist.");
                return null;
            }

            var serviceNameIn = manager.Input.TakeInput("Enter service name of the service you wish to delete.");

            var request = new DeleteServiceRequest(serviceNumberIn, serviceNameIn);
            var (record, message) = Transaction.DeleteService.MakeTransaction(request);
            Console.WriteLine(message);
            if (record != null)
                manager.ServicesList.DeleteService(serviceNumberIn);
            return record;
        }

        public TransactionRecord CancelTicket(SessionManager manager)
        {
            var serviceNumberIn = manager.Input.TakeInput("Enter service number of ticket you would like to cancel.");
            if (!manager.ServicesList.IsInList(serviceNumberIn))
            {
                Console.WriteLine("Requested service does not exist.");
                return null;
            }

            int numberTicketsIn;
            try
            {
                numberTicketsIn = manager.Input.TakeNumericInput("Enter number of tickets you want to cancel.");
            }
            catch (System.IO.InvalidDataException)
            {
                Console.WriteLine("A number was not entered.");
                return null;
            }

            var request = new CancelTicketRequest(serviceNumberIn, numberTicketsIn);
            var (record, message) = Transaction.CancelTicket.MakeTransaction(request);
            Console.WriteLine(message);
            return record;
        }

        public TransactionRecord ChangeTicket(SessionManager manager)
        {
            var sourceNumberIn = manager.Input.TakeInput("Enter service number of the service you want to change.");
            if (!manager.ServicesList.IsInList(sourceNumberIn))
            {
                Console.WriteLine("Requested service does not exist.");
                return null;
            }

            var destinationNumberIn = manager.Input.TakeInput("Enter service number of the service you want to change to.");
            if (!manager.ServicesList.IsInList(destinationNumberIn))
            {
                throw new System.ArgumentException();
            }

            int numberTicketsIn;
            try
            {
                numberTicketsIn = manager.Input.TakeNumericInput("Enter number of tickets you want to cancel.");
            }
            catch (System.IO.InvalidDataException)
            {
                Console.WriteLine("A number was not entered.");
                return null;
            }

            var request = new ChangeTicketRequest(sourceNumberIn, numberTicketsIn, destinationNumberIn);
            var (record, message) = Transaction.ChangeTicket.MakeTransaction(request);
            Console.WriteLine(message);
            return record;
        }
    }
}
 */
