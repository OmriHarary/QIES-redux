using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    public class CreateService : Transaction
    {
        private const TransactionCode Code = TransactionCode.CRE;

        public CreateService()
        {
            this.record = new TransactionRecord(Code);
        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
