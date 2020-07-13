/* using System;
using System.Collections.Generic;
using QIES.Common;
using QIES.Common.Record;
using QIES.Frontend.Transaction;

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

            if (!CancelledTickets.ContainsKey(serviceNumberIn))
                CancelledTickets.Add(serviceNumberIn, 0);

            if (CancelledTickets[serviceNumberIn] + numberTicketsIn > 10)
            {
                Console.WriteLine("Cannot cancel more then 10 tickets for a single service.");
                Console.WriteLine($"User has {10 - CancelledTickets[serviceNumberIn]} tickets left to cancel for this service.");
                return null;
            }
            if (TotalCancelledTickets + numberTicketsIn > 20)
            {
                Console.WriteLine("Cannot cancel as total session canceled tickets would be over 20.");
                Console.WriteLine($"User has {20 - TotalCancelledTickets} tickets left to cancel this session.");
                return null;
            }

            var request = new CancelTicketRequest(serviceNumberIn, numberTicketsIn);
            var (record, message) = Transaction.CancelTicket.MakeTransaction(request);
            Console.WriteLine(message);

            if (record != null)
            {
                TotalCancelledTickets += numberTicketsIn;
                CancelledTickets[serviceNumberIn] += numberTicketsIn;
            }

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

            if (ChangedTickets + numberTicketsIn > 20)
            {
                Console.WriteLine($"Cannot change as total session changed tickets would be over 20.");
                Console.WriteLine($"User has {20 - ChangedTickets} tickets left to change this session.");
                return null;
            }

            var request = new ChangeTicketRequest(sourceNumberIn, numberTicketsIn, destinationNumberIn);
            var (record, message) = Transaction.ChangeTicket.MakeTransaction(request);
            Console.WriteLine(message);

            if (record != null)
                ChangedTickets += numberTicketsIn;

            return record;
        }
    }
}
 */
