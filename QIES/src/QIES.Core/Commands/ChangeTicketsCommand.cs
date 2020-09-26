namespace QIES.Core.Commands
{
    public record ChangeTicketsCommand(string ServiceNumber, int NumberTickets, string SourceServiceNumber)
        : TransactionCommand(ServiceNumber);
}
