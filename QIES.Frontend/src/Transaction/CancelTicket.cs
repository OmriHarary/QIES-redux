using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class CancelTicket : Transaction
    {
        private const TransactionCode Code = TransactionCode.CAN;

        public CancelTicket()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
