using QIES.Common.Records;

namespace QIES.Core.Commands
{
    public abstract record TransactionCommand(ServiceNumber ServiceNumber);

    public record CancelTicketsCommand(ServiceNumber ServiceNumber, NumberTickets NumberTickets)
        : TransactionCommand(ServiceNumber);

    public record ChangeTicketsCommand(ServiceNumber ServiceNumber, NumberTickets NumberTickets, ServiceNumber SourceServiceNumber)
        : TransactionCommand(ServiceNumber);

    public record CreateServiceCommand(ServiceNumber ServiceNumber, ServiceDate ServiceDate, ServiceName ServiceName)
        : TransactionCommand(ServiceNumber);

    public record DeleteServiceCommand(ServiceNumber ServiceNumber, ServiceName ServiceName)
        : TransactionCommand(ServiceNumber);

    public record SellTicketsCommand(ServiceNumber ServiceNumber, NumberTickets NumberTickets)
        : TransactionCommand(ServiceNumber);
}
