using QIES.Common.Record;
using QIES.Frontend.Session;

namespace QIES.Frontend.Transaction
{
    public class Logout : Transaction
    {
        private const TransactionCode Code = TransactionCode.EOS;

        public Logout() => this.record = new TransactionRecord(Code);

        public override TransactionRecord MakeTransaction(SessionManager manager)
        {
            return record;
        }
    }
}
