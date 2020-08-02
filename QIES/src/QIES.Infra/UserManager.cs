using System;
using System.Collections.Concurrent;
using QIES.Core;
using QIES.Core.Users;

namespace QIES.Infra
{
    public class UserManager : IUserManager
    {
        private readonly ConcurrentDictionary<Guid, (User, TransactionQueue)> users;

        public UserManager()
        {
            this.users = new ConcurrentDictionary<Guid, (User, TransactionQueue)>();
        }

        public bool IsLoggedIn(Guid? userId) => userId is Guid id ? users.ContainsKey(id) : false;

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

        public (bool, ITransactionQueue) UserLogout(Guid userId)
        {
            (User user, TransactionQueue tq) userTuple;
            var success = users.TryRemove(userId, out userTuple);
            return (success, userTuple.tq);
        }

        public LoginType UserType(Guid? userId)
        {
            if (userId is Guid id)
            {
                (User user, TransactionQueue tq) userTuple;
                return users.TryGetValue(id, out userTuple) ? userTuple.user.Type : LoginType.None;
            }
            return LoginType.None;
        }
    }
}
