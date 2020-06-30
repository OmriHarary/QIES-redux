using System.Collections.Generic;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    public class TransactionQueue
    {
        private Queue<TransactionRecord> records;

        public TransactionQueue() {
            records = new Queue<TransactionRecord>();
        }

        public void Push(TransactionRecord element)
        {
            records.Enqueue(element);
        }

        public TransactionRecord Pop()
        {
            return records.Dequeue();
        }

        public bool IsEmpty()
        {
            return records.Count == 0;
        }
    }
}
