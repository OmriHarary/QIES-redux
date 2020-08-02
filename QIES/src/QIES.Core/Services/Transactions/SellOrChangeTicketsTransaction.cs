using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Common.Record;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class SellOrChangeTicketsTransaction : ITransaction<SellOrChangeTicketsRequest, TransactionRecord>
    {
        private const TransactionCode CodeSell = TransactionCode.SEL;
        private const TransactionCode CodeChange = TransactionCode.CHG;
        private readonly ILogger<SellOrChangeTicketsRequest> logger;
        private readonly IUserManager userManager;

        public SellOrChangeTicketsTransaction(ILogger<SellOrChangeTicketsRequest> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, SellOrChangeTicketsRequest request, Guid userId)
        {
            TransactionRecord record;

            if (request.SourceServiceNumber is null)
            {
                record = new TransactionRecord(CodeSell);
                record.SourceNumber = new ServiceNumber(serviceNumber);
            }
            else
            {
                record = new TransactionRecord(CodeChange);
                record.SourceNumber = new ServiceNumber(request.SourceServiceNumber);
                record.DestinationNumber = new ServiceNumber(serviceNumber);
            }

            record.NumberTickets = new NumberTickets(int.Parse(request.NumberTickets));

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
