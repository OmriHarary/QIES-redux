using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class ChangeTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.CHG;

        public ChangeTicket()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
