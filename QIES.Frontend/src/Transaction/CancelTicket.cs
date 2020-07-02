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

            int numberTicketsIn = int.Parse(manager.Input.TakeInput("Enter number of tickets you want to cancel."));
            NumberTickets numberTickets;
            try
            {
                numberTickets = new NumberTickets(numberTicketsIn);
                if (manager.Session is AgentSession session)
                {
                    if (!session.CancelledTickets.ContainsKey(serviceNumberIn))
                    {
                        session.CancelledTickets.Add(serviceNumberIn, 0);
                    }
                    if (numberTicketsIn > 10)
                    {
                        Console.WriteLine("Cannot cancel more then 10 tickets at once.");
                        throw new System.ArgumentException();
                    }
                    if (session.CancelledTickets[serviceNumberIn] + numberTicketsIn > 10)
                    {
                        Console.WriteLine("Cannot cancel more then 10 tickets for a single service.");
                        Console.WriteLine($"User has {10 - session.CancelledTickets[serviceNumberIn]} tickets left to cancel for this service.");
                        throw new System.ArgumentException();
                    }
                    if (session.TotalCancelledTickets + numberTicketsIn > 20)
                    {
                        Console.WriteLine("Cannot cancel as total session canceled tickets would be over 20.");
                        Console.WriteLine($"User has {20 - session.TotalCancelledTickets} tickets left to cancel this session.");
                        throw new System.ArgumentException();
                    }
                    session.TotalCancelledTickets += numberTicketsIn;
                    session.CancelledTickets[serviceNumberIn] += numberTicketsIn;
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid number of tickets.");
                return null;
            }

            Console.WriteLine($"{numberTickets} ticket(s) canceled from service {serviceNumber}");
            record.SourceNumber = serviceNumber;
            record.NumberTickets = numberTickets;

            return record;
        }
    }
}
