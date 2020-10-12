using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Common.Records;
using QIES.Core.Commands;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class SellTicketsTransaction : ITransaction<SellTicketsCommand>
    {
        private const TransactionCode Code = TransactionCode.SEL;
        private readonly ILogger<SellTicketsTransaction> logger;
        private readonly IUserManager userManager;

        public SellTicketsTransaction(ILogger<SellTicketsTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(SellTicketsCommand command, Guid userId)
        {
            var record = new TransactionRecord(Code)
            {
                SourceNumber = command.ServiceNumber,
                NumberTickets = command.NumberTickets
            };

            logger.LogDebug("Pushing record to queue: {transaction}", record);
            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
