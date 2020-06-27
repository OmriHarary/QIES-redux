using System.Collections.Generic;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    class TransactionQueue
    {
        private Queue<TransactionRecord> records;

        public TransactionQueue() {

        }

        public void Push(TransactionRecord element)
        {

        }

        public TransactionRecord Pop()
        {
            return null;
        }

        public bool IsEmpty()
        {
            return true;
        }
    }
}
