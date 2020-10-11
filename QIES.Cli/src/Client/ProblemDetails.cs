namespace QIES.Cli.Client.Responses
{
    /// <summary>
    /// A mini-reimplementation of the <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>
    /// type, just for JSON deserialization
    /// </summary>
    public record ProblemDetails(string? Type, string? Title, int? Status, string? Detail, string? Instance);
}
