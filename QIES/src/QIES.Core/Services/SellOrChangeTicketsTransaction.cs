using System.Threading.Tasks;
using QIES.Api.Models;
using QIES.Common.Record;

namespace QIES.Core.Services
{
    public class SellOrChangeTicketsTransaction : ITransaction<SellOrChangeTicketsRequest, TransactionRecord>
    {
        private const TransactionCode CodeSell = TransactionCode.SEL;
        private const TransactionCode CodeChange = TransactionCode.CHG;
        private ITransactionQueue transactionQueue;

        public SellOrChangeTicketsTransaction(ITransactionQueue transactionQueue)
        {
            this.transactionQueue = transactionQueue;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, SellOrChangeTicketsRequest request)
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

            transactionQueue.Push(record);

            return record;
        }
    }
}
