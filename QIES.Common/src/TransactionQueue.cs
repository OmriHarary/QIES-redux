using System.Collections.Generic;
using QIES.Common.Record;

namespace QIES.Common
{
    public class TransactionQueue
    {
        private readonly Queue<TransactionRecord> records;

        public TransactionQueue() => records = new Queue<TransactionRecord>();

        public void Push(TransactionRecord element) => records.Enqueue(element);

        public TransactionRecord Pop() => records.Dequeue();

        public bool IsEmpty() => records.Count == 0;
    }
}
