namespace QIES.Core.Commands
{
    public record SellTicketsCommand(string ServiceNumber, int NumberTickets)
        : TransactionCommand(ServiceNumber);
}
