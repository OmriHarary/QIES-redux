using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Common.Records;
using QIES.Core.Commands;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class CreateServiceTransaction : ITransaction<CreateServiceCommand>
    {
        private const TransactionCode Code = TransactionCode.CRE;
        private readonly ILogger<CreateServiceTransaction> logger;
        private readonly IUserManager userManager;

        public CreateServiceTransaction(ILogger<CreateServiceTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(CreateServiceCommand command, Guid userId)
        {
            var record = new TransactionRecord(Code)
            {
                SourceNumber = command.ServiceNumber,
                ServiceDate = command.ServiceDate,
                ServiceName = command.ServiceName
            };

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
