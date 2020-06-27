using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    class Logout : Transaction
    {
        private const TransactionCode Code = TransactionCode.EOS;

        public Logout()
        {

        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
