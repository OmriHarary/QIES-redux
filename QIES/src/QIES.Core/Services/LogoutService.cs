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
        private readonly ITransactionQueue transactionQueue;

        public LogoutService(ILogger<LogoutService> logger, IUserManager userManager, ITransactionQueue transactionQueue)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.transactionQueue = transactionQueue;
        }

        public async Task<bool> DoLogout(Guid id)
        {
            var success = userManager.UserLogout(id);
            if (success)
            {
                transactionQueue.Push(new TransactionRecord(Code));
            }
            // TODO: Print transaction summary here
            return success;
        }
    }
}
