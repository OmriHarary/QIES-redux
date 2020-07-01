using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class Logout : Transaction
    {
        private const TransactionCode Code = TransactionCode.EOS;

        public Logout()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            return record;
        }
    }
}
