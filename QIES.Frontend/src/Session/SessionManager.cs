using System.IO;

namespace QIES.Frontend.Session
{
    class SessionManager
    {
        public ISession Session { get; set; }
        public ValidServicesList servicesList { get; set; }
        public TransactionQueue transactionQueue { get; set; }
        public FileInfo summaryFile { get; set; }
        public Input input { get; set; }

        public SessionManager(string validServicesFilePath, string summaryFilePath)
        {

        }

        public void Operate()
        {

        }

        public void PrintTransactionSummary()
        {

        }
    }
}
