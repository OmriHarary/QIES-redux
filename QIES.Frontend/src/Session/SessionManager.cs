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
            this.Session = new NoSession();
            var validServicesFile = new FileInfo(validServicesFilePath);
            this.ServicesList = new ValidServicesList(validServicesFile);
            this.TransactionQueue = new TransactionQueue();
            this.SummaryFile = new FileInfo(summaryFilePath);
            this.Input = new Input(NoSession.Prompt);
        }

        public void Operate() => Session.Process(this, TransactionQueue);

        public void SetSession(ISession value)
        {
            Session = value;
            if (value is AgentSession)
                Input.Prompt = AgentSession.Prompt;
            if (value is PlannerSession)
                Input.Prompt = PlannerSession.Prompt;
            if (value is NoSession)
            {
                Input.Prompt = NoSession.Prompt;
                PrintTransactionSummary();
            }
            Operate();
        }

        public void PrintTransactionSummary()
        {
            try
            {
                using StreamWriter summaryWriter = SummaryFile.CreateText();
                while (!TransactionQueue.IsEmpty())
                {
                    summaryWriter.WriteLine(TransactionQueue.Pop());
                }
            }
            catch (IOException e)
            {
                // TODO: Actual error handling (the original didn't handle this either)
                System.Console.Error.WriteLine(e.StackTrace);
            }
        }
    }
}
