namespace QIES.Core.Commands
{
    public abstract record TransactionCommand(string ServiceNumber);

    public record CancelTicketsCommand(string ServiceNumber, int NumberTickets)
        : TransactionCommand(ServiceNumber);

    public record ChangeTicketsCommand(string ServiceNumber, int NumberTickets, string SourceServiceNumber)
        : TransactionCommand(ServiceNumber);

    public record CreateServiceCommand(string ServiceNumber, string ServiceDate, string ServiceName)
        : TransactionCommand(ServiceNumber);

    public record DeleteServiceCommand(string ServiceNumber, string ServiceName)
        : TransactionCommand(ServiceNumber);

    public record SellTicketsCommand(string ServiceNumber, int NumberTickets)
        : TransactionCommand(ServiceNumber);
}
