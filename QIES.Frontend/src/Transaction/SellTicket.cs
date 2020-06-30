using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class SellTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.SEL;

        public SellTicket()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
