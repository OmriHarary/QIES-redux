using System;

namespace QIES.Cli.Client.Requests
{
    public record LoginRequest(string Login);

    public record LogoutRequest(Guid UserId);
}
