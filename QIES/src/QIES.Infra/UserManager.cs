using System;
using System.Collections.Concurrent;
using QIES.Core;
using QIES.Core.Users;

namespace QIES.Infra
{
    public class UserManager : IUserManager
    {
        private readonly ConcurrentDictionary<Guid, (User, TransactionQueue)> users;

        public UserManager() => users = new ConcurrentDictionary<Guid, (User, TransactionQueue)>();

        public bool IsLoggedIn(Guid userId) => users.ContainsKey(userId);

        public User UserLogin(LoginType login)
        {
            var transactionQueue = new TransactionQueue();
            if (login == LoginType.Planner)
            {
                var planner = new Planner();
                users.TryAdd(planner.Id, (planner, transactionQueue));
                return planner;
            }
            var agent = new Agent();
            users.TryAdd(agent.Id, (agent, transactionQueue));
            return agent;
        }

        public (bool, ITransactionQueue?) UserLogout(Guid userId)
        {
            var success = users.TryRemove(userId, out (User user, TransactionQueue tq) userTuple);
            return (success, success ? userTuple.tq : null);
        }

        public LoginType UserType(Guid userId) =>
            users.TryGetValue(userId, out (User user, TransactionQueue tq) userTuple)
                ? userTuple.user.Type : LoginType.None;

        public User User(Guid userId)
        {
            var (user, _) = users[userId];
            return user;
        }

        public ITransactionQueue UserTransactionQueue(Guid userId)
        {
            var (_, transactionQueue) = users[userId];
            return transactionQueue;
        }
    }
}
