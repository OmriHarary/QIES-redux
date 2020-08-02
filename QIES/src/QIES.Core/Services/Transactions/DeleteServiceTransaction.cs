using System.Threading.Tasks;
using QIES.Api.Models;
using QIES.Common.Record;

namespace QIES.Core.Services
{
    public class DeleteServiceTransaction : ITransaction<DeleteServiceRequest, TransactionRecord>
    {
        private const TransactionCode Code = TransactionCode.DEL;
        private ITransactionQueue transactionQueue;

        public DeleteServiceTransaction(ITransactionQueue transactionQueue)
        {
            this.transactionQueue = transactionQueue;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, DeleteServiceRequest request)
        {
            var record = new TransactionRecord(Code);
            record.SourceNumber = new ServiceNumber(serviceNumber);
            record.ServiceName = new ServiceName(request.ServiceName);

            transactionQueue.Push(record);

            return record;
        }
    }
}
