using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class DeleteService : Transaction
    {
        private const TransactionCode Code = TransactionCode.DEL;

        public DeleteService()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
