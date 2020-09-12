using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Common.Record;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class DeleteServiceTransaction : ITransaction<DeleteServiceRequest, TransactionRecord>
    {
        private const TransactionCode Code = TransactionCode.DEL;
        private readonly ILogger<DeleteServiceTransaction> logger;
        private readonly IUserManager userManager;

        public DeleteServiceTransaction(ILogger<DeleteServiceTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, DeleteServiceRequest request, Guid userId)
        {
            var record = new TransactionRecord(Code);
            record.SourceNumber = new ServiceNumber(serviceNumber);
            record.ServiceName = new ServiceName(request.ServiceName);

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
