using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QIES.Core;
using QIES.Core.Config;

namespace QIES.Infra
{
    public class SummaryWriter : ISummaryWriter
    {
        private readonly ILogger<SummaryWriter> logger;
        private readonly TransactionSummaryOptions options;

        public SummaryWriter(ILogger<SummaryWriter> logger, IOptions<TransactionSummaryOptions> options) =>
            (this.logger, this.options) = (logger, options.Value);

        public async Task WriteTransactionSummaryFile(ITransactionQueue transactionQueue, string path)
        {
            FileInfo summaryFile = new FileInfo(Path.Combine(options.Directory, path));

            try
            {
                using StreamWriter summaryWriter = summaryFile.CreateText();
                while (!transactionQueue.IsEmpty())
                {
                    await summaryWriter.WriteLineAsync(transactionQueue.Pop().ToString());
                }
            }
            catch (IOException e)
            {
                // TODO: Actual error handling (the original didn't handle this either)
                logger.LogError(e, "Error encountered while writing transaction summary file.");
            }
        }
    }
}
