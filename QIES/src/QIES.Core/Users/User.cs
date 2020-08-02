using System;

namespace QIES.Core.Users
{
    public abstract class User
    {
        public Guid Id { get; }
        public LoginType Type { get; }

        public User(LoginType login)
        {
            this.Id = Guid.NewGuid();
            this.Type = login;
        }
    }
}
