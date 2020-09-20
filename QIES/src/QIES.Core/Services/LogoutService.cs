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
        private readonly ISummaryWriter summaryWriter;

        public LogoutService(ILogger<LogoutService> logger, IUserManager userManager, ISummaryWriter summaryWriter)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.summaryWriter = summaryWriter;
        }

        public async Task<bool> DoLogout(Guid id)
        {
            var (success, transactionQueue) = userManager.UserLogout(id);
            if (success && transactionQueue is not null)
            {
                transactionQueue.Push(new TransactionRecord(Code));
                await summaryWriter.WriteTransactionSummaryFile(transactionQueue, $"{id}.txt");
            }
            return success;
        }
    }
}
