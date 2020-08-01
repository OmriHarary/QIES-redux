using System;

namespace QIES.Common.Record
{
    public class TransactionRecord : IEquatable<TransactionRecord>
    {
        public TransactionCode Code { get; private set; }
        public ServiceNumber SourceNumber { get; set; }
        public NumberTickets NumberTickets { get; set; }
        public ServiceNumber DestinationNumber { get; set; }
        public ServiceName ServiceName { get; set; }
        public ServiceDate ServiceDate { get; set; }

        public TransactionRecord(TransactionCode code)
        {
            this.Code = code;
            this.SourceNumber = new ServiceNumber();
            this.NumberTickets = new NumberTickets();
            this.DestinationNumber = new ServiceNumber();
            this.ServiceName = new ServiceName();
            this.ServiceDate = new ServiceDate();
        }

        public bool Equals(TransactionRecord? other) => 
            other is not null &&
            Code.Equals(other.Code) &&
            SourceNumber.Equals(other.SourceNumber) &&
            NumberTickets.Equals(other.NumberTickets) &&
            DestinationNumber.Equals(other.DestinationNumber) &&
            ServiceName.Equals(other.ServiceName) &&
            ServiceDate.Equals(other.ServiceDate);
        public override bool Equals(object? obj) => obj is TransactionRecord otherRecord && Equals(otherRecord);
        public override string ToString() =>
            $"{Code} {SourceNumber} {NumberTickets} {DestinationNumber} {ServiceName} {ServiceDate}";
        public override int GetHashCode() =>
            HashCode.Combine(Code, SourceNumber, NumberTickets, DestinationNumber, ServiceName, ServiceDate);

        public static bool operator ==(TransactionRecord lhs, TransactionRecord rhs) => lhs?.Equals(rhs) ?? rhs is null;
        public static bool operator !=(TransactionRecord lhs, TransactionRecord rhs) => !(lhs == rhs);
    }
}
