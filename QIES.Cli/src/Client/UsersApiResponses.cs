using System;

namespace QIES.Cli.Client.Responses
{
    public enum LoginType
    {
        None,
        Agent,
        Planner
    }

    public record LoginResponse(Guid UserId, LoginType Type);
}
