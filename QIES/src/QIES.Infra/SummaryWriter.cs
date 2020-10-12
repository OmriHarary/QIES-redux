using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Core;

namespace QIES.Infra
{
    public class SummaryWriter : ISummaryWriter
    {
        private readonly ILogger<SummaryWriter> logger;

        public SummaryWriter(ILogger<SummaryWriter> logger) => this.logger = logger;

        public async Task WriteTransactionSummaryFile(ITransactionQueue transactionQueue, string path)
        {
            FileInfo summaryFile = new FileInfo(path);

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
                logger.LogError(e.StackTrace);
            }
        }
    }
}
