using System.IO;

namespace QIES.Backoffice.Processor
{
    public interface IProcessor
    {
        public void Process(FileInfo transactionFile);
    }
}
