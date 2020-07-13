using System.IO;
using QIES.Common;
using QIES.Frontend.Transaction;

namespace QIES.Frontend.Session
{
    public class SessionController
    {
        public LoginType ActiveLogin { get; set; }
        public ValidServicesList ServicesList { get; set; }
        public TransactionQueue TransactionQueue { get; set; }
        public FileInfo SummaryFile { get; set; }

        public SessionController(string validServicesFilePath, string summaryFilePath)
        {
            ActiveLogin = LoginType.NONE;
            var validServicesFile = new FileInfo(validServicesFilePath);
            this.ServicesList = new ValidServicesList(validServicesFile);
            this.TransactionQueue = new TransactionQueue();
            this.SummaryFile = new FileInfo(summaryFilePath);
        }

        public (bool, string, LoginType) ProcessLogin(string request)
        {
            if (ActiveLogin == LoginType.NONE)
            {
                (bool, string, LoginType) response = request switch
                {
                    "agent"     => (true, "Logged in as agent.", LoginType.AGENT),
                    "planner"   => (true, "Logged in as planner.", LoginType.PLANNER),
                    _           => (false, "Invalid input.", LoginType.NONE)
                };
                ActiveLogin = response.Item3;
                return response;
            }
            return (false, "Already logged in.", ActiveLogin);
        }

        public (bool, string, LoginType) ProcessLogout(LogoutRequest request)
        {
            if (ActiveLogin != LoginType.NONE)
            {
                var (record, message) = Logout.MakeTransaction(request);
                ActiveLogin = LoginType.NONE;
                TransactionQueue.Push(record);
                PrintTransactionSummary();
                return (true, message, ActiveLogin);
            }
            return (false, "Already logged out.", ActiveLogin);
        }

        public (bool, string) ProcessSellTicket(SellTicketRequest request)
        {
            return (false, "Not implemented");
        }

        public (bool, string) ProcessCancelTicket(CancelTicketRequest request)
        {
            return (false, "Not implemented");
        }

        public (bool, string) ProcessChangeTicket(ChangeTicketRequest request)
        {
            return (false, "Not implemented");
        }

        public (bool, string) ProcessCreateService(CreateServiceRequest request)
        {
            return (false, "Not implemented");
        }

        public (bool, string) ProcessDeleteService(DeleteServiceRequest request)
        {
            return (false, "Not implemented");
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
