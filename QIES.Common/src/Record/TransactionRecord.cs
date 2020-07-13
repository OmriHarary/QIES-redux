using System;
using System.Collections.Generic;

namespace QIES.Common.Record
{
    public class TransactionRecord
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

        public override string ToString() =>
            $"{Code} {SourceNumber} {NumberTickets} {DestinationNumber} {ServiceName} {ServiceDate}";
        public override bool Equals(object? obj) =>
            obj is TransactionRecord record &&
                    Code == record.Code &&
                    EqualityComparer<ServiceNumber>.Default.Equals(SourceNumber, record.SourceNumber) &&
                    EqualityComparer<NumberTickets>.Default.Equals(NumberTickets, record.NumberTickets) &&
                    EqualityComparer<ServiceNumber>.Default.Equals(DestinationNumber, record.DestinationNumber) &&
                    EqualityComparer<ServiceName>.Default.Equals(ServiceName, record.ServiceName) &&
                    EqualityComparer<ServiceDate>.Default.Equals(ServiceDate, record.ServiceDate);
        public override int GetHashCode() =>
            HashCode.Combine(Code, SourceNumber, NumberTickets, DestinationNumber, ServiceName, ServiceDate);

        public static bool operator ==(TransactionRecord lhs, TransactionRecord rhs) => lhs.Equals(rhs);
        public static bool operator !=(TransactionRecord lhs, TransactionRecord rhs) => !(lhs == rhs);
    }
}
