using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    abstract class Transaction
    {
        private TransactionRecord record;

        public abstract TransactionRecord MakeTransaction(Input input);
    }
}
