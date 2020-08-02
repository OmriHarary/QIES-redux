using System;

namespace QIES.Core.Users
{
    public interface IUserManager
    {
        public bool IsLoggedIn(Guid userId);
        public User UserLogin(LoginType login);
        public (bool, ITransactionQueue) UserLogout(Guid userId);
        public LoginType UserType(Guid userId);
        public ITransactionQueue UserTransactionQueue(Guid userId);
    }
}
