using System;

namespace QIES.Core.Users
{
    public interface IUserManager
    {
        public bool IsLoggedIn(Guid id);
        public User UserLogin(LoginType login);
        public bool UserLogout(Guid id);
        public LoginType UserType(Guid id);
    }
}
