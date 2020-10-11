namespace QIES.Cli.Client.Requests
{
    public record CreateServiceRequest(string ServiceNumber, string ServiceDate, string ServiceName);

    public record DeleteServiceRequest(string ServiceName);

    public record SellTicketsRequest(int NumberTickets);

    public record ChangeTicketsRequest(int NumberTickets, string SourceServiceNumber);

    public record CancelTicketsRequest(int NumberTickets);
}
