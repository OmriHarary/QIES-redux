using System;

namespace QIES.Api.Models
{
    public abstract class AuthenticatedRequest
    {
        public Guid? UserId { get; set; }
    }
}
