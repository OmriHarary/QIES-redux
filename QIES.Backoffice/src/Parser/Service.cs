using QIES.Backoffice.Parser.Record;

namespace QIES.Backoffice.Parser
{
    public class Service
    {
        public ServiceNumber ServiceNumber { get; set; }
        public ServiceName ServiceName { get; set; }
        public NumberTickets TicketsSold { get; set; }
        public int ServiceCapacity { get; set; }

        public Service()
        {
            this.ServiceCapacity = 30;
            this.ServiceNumber = new ServiceNumber();
            this.ServiceName = new ServiceName();
            this.TicketsSold = new NumberTickets();
        }

        public void AddTickets(int toAdd)
        {
            if (TicketsSold.Number + toAdd > ServiceCapacity)
            {
                throw new System.ArgumentException();
            }
            TicketsSold = new NumberTickets(TicketsSold.Number + toAdd);
        }

        public void RemoveTickets(int toRemove)
        {
            if (TicketsSold.Number - toRemove < 0)
            {
                throw new System.ArgumentException();
            }
            TicketsSold = new NumberTickets(TicketsSold.Number - toRemove);
        }

        public override string ToString()
        {
            return $"{ServiceNumber} {ServiceCapacity} {TicketsSold} {ServiceName}";
        }
    }
}