using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class ChangeTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.CHG;

        public ChangeTicket() => this.record = new TransactionRecord(Code);

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            var sourceNumberIn = manager.Input.TakeInput("Enter service number of the service you want to change.");
            ServiceNumber sourceNumber;
            try
            {
                sourceNumber = new ServiceNumber(sourceNumberIn);
                if (!manager.ServicesList.IsInList(sourceNumberIn))
                {
                    throw new System.ArgumentException();
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid service number.");
                return null;
            }

            var destNumberIn = manager.Input.TakeInput("Enter service number of the service you want to change to.");
            ServiceNumber destNumber;
            try
            {
                destNumber = new ServiceNumber(destNumberIn);
                if (!manager.ServicesList.IsInList(destNumberIn))
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
                if (!int.TryParse(manager.Input.TakeInput("Enter number of tickets to change."), out numberTicketsIn))
                    throw new System.ArgumentException();
                numberTickets = new NumberTickets(numberTicketsIn);
                if (manager.Session is AgentSession session)
                {
                    if (session.ChangedTickets + numberTicketsIn > 20)
                    {
                        Console.WriteLine($"Cannot change as total session changed tickets would be over 20.");
                        Console.WriteLine($"User has {20 - session.ChangedTickets} tickets left to change this session.");
                        throw new System.ArgumentException();
                    }
                    session.ChangedTickets += numberTicketsIn;
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("Invalid number of tickets.");
                return null;
            }

            Console.WriteLine($"{numberTickets} ticket(s) changed from service {sourceNumber} to service {destNumber}");
            record.SourceNumber = sourceNumber;
            record.DestinationNumber = destNumber;
            record.NumberTickets = numberTickets;

            return record;
        }
    }
}
