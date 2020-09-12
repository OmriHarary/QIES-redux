using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Common.Record;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class CancelTicketsTransaction : ITransaction<CancelTicketsRequest, TransactionRecord>
    {
        private const TransactionCode Code = TransactionCode.CAN;
        private readonly ILogger<CancelTicketsTransaction> logger;
        private readonly IUserManager userManager;

        public CancelTicketsTransaction(ILogger<CancelTicketsTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, CancelTicketsRequest request, Guid userId)
        {
            var record = new TransactionRecord(Code);
            record.SourceNumber = new ServiceNumber(serviceNumber);
            record.NumberTickets = new NumberTickets(int.Parse(request.NumberTickets));

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
