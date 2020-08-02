using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Common.Record;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class LogoutService : ILogoutService
    {
        private const TransactionCode Code = TransactionCode.EOS;

        private readonly ILogger<LogoutService> logger;
        private readonly IUserManager userManager;

        public LogoutService(ILogger<LogoutService> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<bool> DoLogout(Guid id)
        {
            var (success, transactionQueue) = userManager.UserLogout(id);
            if (success)
            {
                transactionQueue.Push(new TransactionRecord(Code));
            }
            // TODO: Print transaction summary here
            return success;
        }
    }
}
