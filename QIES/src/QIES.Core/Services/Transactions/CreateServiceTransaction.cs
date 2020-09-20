using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Common;
using QIES.Common.Record;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class CreateServiceTransaction : ITransaction<CreateServiceRequest>
    {
        private const TransactionCode Code = TransactionCode.CRE;
        private readonly ILogger<CreateServiceTransaction> logger;
        private readonly IUserManager userManager;

        public CreateServiceTransaction(ILogger<CreateServiceTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, CreateServiceRequest request, Guid userId)
        {
            var record = new TransactionRecord(Code)
            {
                SourceNumber = new ServiceNumber(request.ServiceNumber),
                ServiceDate = new ServiceDate(request.ServiceDate),
                ServiceName = new ServiceName(request.ServiceName)
            };

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
