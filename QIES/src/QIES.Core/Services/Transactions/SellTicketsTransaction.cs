using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Common.Record;
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

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, SellTicketsCommand command, Guid userId)
        {
            var record = new TransactionRecord(Code);
            record.SourceNumber = new ServiceNumber(serviceNumber);
            record.NumberTickets = new NumberTickets(command.NumberTickets);

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
