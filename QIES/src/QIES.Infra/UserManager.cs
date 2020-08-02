using System;
using System.Collections.Concurrent;
using QIES.Core.Users;

namespace QIES.Infra
{
    public class UserManager : IUserManager
    {
        private readonly ConcurrentDictionary<Guid, User> users;

        public UserManager()
        {
            this.users = new ConcurrentDictionary<Guid, User>();
        }

        public bool IsLoggedIn(Guid id) => users.ContainsKey(id);

        public User UserLogin(LoginType login)
        {
            if (login == LoginType.Planner)
            {
                var planner = new Planner();
                users.TryAdd(planner.Id, planner);
                return planner;
            }
            var agent = new Agent();
            users.TryAdd(agent.Id, agent);
            return agent;
        }

        public bool UserLogout(Guid id)
        {
            User? user;
            var success = users.TryRemove(id, out user);
            return success;
        }

        public LoginType UserType(Guid id)
        {
            User? user;
            return users.TryGetValue(id, out user) ? user!.Type : LoginType.None;
        }
    }
}
