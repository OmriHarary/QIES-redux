using QIES.Common.Records;

namespace QIES.Core
{
    public interface ITransactionQueue
    {
        public void Push(TransactionRecord element);

        public TransactionRecord Pop();

        public bool IsEmpty();
    }
}
