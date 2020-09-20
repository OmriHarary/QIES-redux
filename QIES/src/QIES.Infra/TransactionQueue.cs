using System.Collections.Concurrent;
using QIES.Common.Record;
using QIES.Core;

namespace QIES.Infra
{
    public class TransactionQueue : ITransactionQueue
    {
        private readonly ConcurrentQueue<TransactionRecord> records;

        public TransactionQueue() => records = new ConcurrentQueue<TransactionRecord>();

        public void Push(TransactionRecord element) => records.Enqueue(element);

        public TransactionRecord Pop()
        {
            records.TryDequeue(out TransactionRecord record);
            return record;
        }

        public bool IsEmpty() => records.IsEmpty;
    }
}
