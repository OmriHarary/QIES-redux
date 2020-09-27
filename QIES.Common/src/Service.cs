using System;
using QIES.Common.Record;

namespace QIES.Common
{
    public class Service : IEquatable<Service>
    {
        public ServiceNumber ServiceNumber { get; set; }
        public ServiceName ServiceName { get; set; }
        public NumberTickets TicketsSold { get; set; }
        public int ServiceCapacity { get; set; }

        public Service()
        {
            ServiceCapacity = 30;
            ServiceNumber = ServiceNumber.Empty;
            ServiceName = ServiceName.Empty;
            TicketsSold = NumberTickets.Empty;
        }

        public void AddTickets(int toAdd)
        {
            if (TicketsSold.Number + toAdd > ServiceCapacity)
            {
                throw new ArgumentException("Addition would exceed capacity.", nameof(toAdd));
            }
            TicketsSold = new NumberTickets(TicketsSold.Number + toAdd);
        }

        public void RemoveTickets(int toRemove)
        {
            if (TicketsSold.Number - toRemove < 0)
            {
                throw new ArgumentException("Removal would reduce below 0.", nameof(toRemove));
            }
            TicketsSold = new NumberTickets(TicketsSold.Number - toRemove);
        }

        public bool Equals(Service? other) =>
            other is not null &&
            ServiceNumber.Equals(other.ServiceNumber) &&
            ServiceName.Equals(other.ServiceName) &&
            TicketsSold.Equals(other.TicketsSold) &&
            ServiceCapacity.Equals(other.ServiceCapacity);
        public override bool Equals(object? obj) => obj is Service otherService && Equals(otherService);
        public override string ToString() => $"{ServiceNumber} {ServiceCapacity} {TicketsSold} {ServiceName}";
        public override int GetHashCode() => HashCode.Combine(ServiceNumber, ServiceName, TicketsSold, ServiceCapacity);

        public static bool operator ==(Service lhs, Service rhs) => lhs?.Equals(rhs) ?? rhs is null;
        public static bool operator !=(Service lhs, Service rhs) => !(lhs == rhs);
    }
}
