using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    class SellTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.SEL;

        public SellTicket()
        {

        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
