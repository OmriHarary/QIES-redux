using System.Threading.Tasks;
using QIES.Api.Models;
using QIES.Common.Record;

namespace QIES.Core.Services
{
    public class CancelTicketsTransaction : ITransaction<CancelTicketsRequest, TransactionRecord>
    {
        private const TransactionCode Code = TransactionCode.CAN;
        private ITransactionQueue transactionQueue;

        public CancelTicketsTransaction(ITransactionQueue transactionQueue)
        {
            this.transactionQueue = transactionQueue;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, CancelTicketsRequest request)
        {
            var record = new TransactionRecord(Code);
            record.SourceNumber = new ServiceNumber(serviceNumber);
            record.NumberTickets = new NumberTickets(int.Parse(request.NumberTickets));

            transactionQueue.Push(record);

            return record;
        }
    }
}
