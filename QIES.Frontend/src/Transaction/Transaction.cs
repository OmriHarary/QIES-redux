using QIES.Common.Record;
using QIES.Frontend.Session;

namespace QIES.Frontend.Transaction
{
    public abstract class Transaction
    {
        protected TransactionRecord record;

        public abstract TransactionRecord MakeTransaction(SessionManager manager);
    }
}
