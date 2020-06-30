using System.IO;

namespace QIES.Frontend.Session
{
    public class SessionManager
    {
        public ISession Session { get; set; }
        public ValidServicesList ServicesList { get; set; }
        public TransactionQueue TransactionQueue { get; set; }
        public FileInfo SummaryFile { get; set; }
        public Input Input { get; set; }

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
