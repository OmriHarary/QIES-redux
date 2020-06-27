using QIES.Frontend.Session;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Transaction
{
    class CreateService : Transaction
    {
        private const TransactionCode Code = TransactionCode.CRE;

        public CreateService()
        {

        }

        public override TransactionRecord MakeTransaction(Input input)
        {
            throw new System.NotImplementedException();
        }
    }
}
