using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILogger<LoginService> logger;
        private readonly IUserManager userManager;

        public LoginService(ILogger<LoginService> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<User> DoLogin(LoginType login)
        {
            return userManager.UserLogin(login);
        }
    }
}
