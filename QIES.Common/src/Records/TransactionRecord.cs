using System;

namespace QIES.Common.Records
{
    public record TransactionRecord(TransactionCode Code)
    {
        public ServiceNumber SourceNumber { get; init; } = ServiceNumber.Empty;
        public NumberTickets NumberTickets { get; init; } = NumberTickets.Empty;
        public ServiceNumber DestinationNumber { get; init; } = ServiceNumber.Empty;
        public ServiceName ServiceName { get; init; } = ServiceName.Empty;
        public ServiceDate ServiceDate { get; init; } = ServiceDate.Empty;

        public override string ToString() =>
            $"{Code} {SourceNumber} {NumberTickets} {DestinationNumber} {ServiceName} {ServiceDate}";

        public static bool TryParse(string summaryLine, out TransactionRecord? record)
        {
            record = null;
            try
            {
                record = Parse(summaryLine);
            }
            catch(ArgumentException)
            {
                return false;
            }
            return record is not null;
        }

        public static TransactionRecord? Parse(string summaryLine)
        {
            var tokens = summaryLine.Split(' ');
            var code = Enum.Parse<TransactionCode>(tokens[0]);

            var numTickets = int.Parse(tokens[2]);
            var serviceNameStr = string.Join(' ', tokens[4..^1]);
            return new TransactionRecord(code)
            {
                SourceNumber = ServiceNumber.EmptyValue == tokens[1] ?
                    ServiceNumber.Empty : new ServiceNumber(tokens[1]),
                NumberTickets = NumberTickets.EmptyValue == numTickets ?
                    NumberTickets.Empty : new NumberTickets(numTickets),
                DestinationNumber = ServiceNumber.EmptyValue == tokens[3] ?
                    ServiceNumber.Empty : new ServiceNumber(tokens[3]),
                ServiceName = ServiceName.EmptyValue == serviceNameStr ?
                    ServiceName.Empty : new ServiceName(serviceNameStr),
                ServiceDate = ServiceDate.EmptyValue == tokens[^1] ?
                    ServiceDate.Empty : new ServiceDate(tokens[^1])
            };
        }
    }
}
