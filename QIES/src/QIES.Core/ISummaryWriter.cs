using System.Threading.Tasks;

namespace QIES.Core
{
    public interface ISummaryWriter
    {
        public Task WriteTransactionSummaryFile(ITransactionQueue transactionQueue, string path);
    }
}
