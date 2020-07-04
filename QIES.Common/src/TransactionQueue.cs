using System.Collections.Generic;

namespace QIES.Frontend.Session
{
    public class TransactionQueue
    {
        private Queue<TransactionRecord> records;

        public TransactionQueue() => records = new Queue<TransactionRecord>();

        public void Push(TransactionRecord element) => records.Enqueue(element);

        public TransactionRecord Pop() => records.Dequeue();

        public bool IsEmpty() => records.Count == 0;
    }
}
