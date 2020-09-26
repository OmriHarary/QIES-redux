using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Common.Record;
using QIES.Core.Commands;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class DeleteServiceTransaction : ITransaction<DeleteServiceCommand>
    {
        private const TransactionCode Code = TransactionCode.DEL;
        private readonly ILogger<DeleteServiceTransaction> logger;
        private readonly IUserManager userManager;

        public DeleteServiceTransaction(ILogger<DeleteServiceTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(DeleteServiceCommand command, Guid userId)
        {
            var record = new TransactionRecord(Code)
            {
                SourceNumber = new ServiceNumber(command.ServiceNumber),
                ServiceName = new ServiceName(command.ServiceName)
            };

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
