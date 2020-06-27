using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    class ChangeTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.CHG;

        public ChangeTicket()
        {

        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
