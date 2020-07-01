using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public abstract class Transaction
    {
        protected TransactionRecord record;

        public abstract TransactionRecord MakeTransaction(SessionManager manager);
    }
}
