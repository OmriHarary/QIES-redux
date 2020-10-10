using System;
using QIES.Core.Users;

namespace QIES.Api.Responses
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public LoginType Type { get; set; }
    }
}
