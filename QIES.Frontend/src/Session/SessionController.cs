using System.Collections.Generic;
using System.IO;
using QIES.Common;
using QIES.Frontend.Transaction;

namespace QIES.Frontend.Session
{
    public class SessionController
    {
        private int changedTickets;
        private int totalCancelledTickets;
        private Dictionary<string, int> cancelledTickets;

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
            this.cancelledTickets = new Dictionary<string, int>();
        }

        public SessionController(string summaryFilePath)
        {
            ActiveLogin = LoginType.NONE;
            this.ServicesList = new ValidServicesList();
            this.TransactionQueue = new TransactionQueue();
            this.SummaryFile = new FileInfo(summaryFilePath);
            this.cancelledTickets = new Dictionary<string, int>();
        }

        public (bool, string, LoginType) ProcessLogin(string request)
        {
            if (ActiveLogin == LoginType.NONE)
            {
                (bool, string, LoginType) response = request switch
                {
                    "agent"     => (true, "Successfully logged in as Agent.", LoginType.AGENT),
                    "planner"   => (true, "Successfully logged in as Planner.", LoginType.PLANNER),
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
                changedTickets = 0;
                totalCancelledTickets = 0;
                cancelledTickets.Clear();
                return (true, message, ActiveLogin);
            }
            return (false, "Already logged out.", ActiveLogin);
        }

        public (bool, string) ProcessSellTicket(SellTicketRequest request)
        {
            if (ActiveLogin == LoginType.NONE)
            {
                return (false, "Must be logged in to sell tickets.");
            }
            if (!ServicesList.IsInList(request.ServiceNumberIn))
            {
                return (false, "Requested service does not exist.");
            }
            var (record, message) = SellTicket.MakeTransaction(request);
            if (record != null)
            {
                TransactionQueue.Push(record);
                return (true, message);
            }
            return (false, message);
        }

        public (bool, string) ProcessCancelTicket(CancelTicketRequest request)
        {
            if (ActiveLogin == LoginType.NONE)
            {
                return (false, "Must be logged in to cancel tickets.");
            }
            if (!ServicesList.IsInList(request.ServiceNumberIn))
            {
                return (false, "Requested service does not exist.");
            }
            if (ActiveLogin == LoginType.AGENT)
            {
                if (!cancelledTickets.ContainsKey(request.ServiceNumberIn))
                    cancelledTickets.Add(request.ServiceNumberIn, 0);
                if (cancelledTickets[request.ServiceNumberIn] + request.NumberTicketsIn > 10)
                {
                    return (false, "Cannot cancel more then 10 tickets for a single service.\n" +
                        $"User has {10 - cancelledTickets[request.ServiceNumberIn]} tickets left to cancel for this service.");
                }
                if (totalCancelledTickets + request.NumberTicketsIn > 20)
                {
                    return (false, "Cannot cancel as total session canceled tickets would be over 20.\n" +
                        $"User has {20 - totalCancelledTickets} tickets left to cancel this session.");
                }
            }

            var (record, message) = CancelTicket.MakeTransaction(request);
            if (record != null)
            {
                if (ActiveLogin == LoginType.AGENT)
                {
                    totalCancelledTickets += request.NumberTicketsIn;
                    cancelledTickets[request.ServiceNumberIn] += request.NumberTicketsIn;
                }
                TransactionQueue.Push(record);
                return (true, message);
            }
            return (false, message);
        }

        public (bool, string) ProcessChangeTicket(ChangeTicketRequest request)
        {
            if (ActiveLogin == LoginType.NONE)
            {
                return (false, "Must be logged in to change tickets.");
            }
            if (!ServicesList.IsInList(request.SourceNumberIn))
            {
                return (false, "Source service does not exist.");
            }
            if (!ServicesList.IsInList(request.DestinationNumberIn))
            {
                return (false, "Destination service does not exist.");
            }
            if (ActiveLogin == LoginType.AGENT)
            {
                if (changedTickets + request.NumberTicketsIn > 20)
                {
                    return (false, "Cannot change as total session changed tickets would be over 20.\n" +
                        $"User has {20 - changedTickets} tickets left to cancel this session.");
                }
            }

            var (record, message) = ChangeTicket.MakeTransaction(request);
            if (record != null)
            {
                if (ActiveLogin == LoginType.AGENT)
                    changedTickets += request.NumberTicketsIn;
                TransactionQueue.Push(record);
                return (true, message);
            }
            return (false, message);
        }

        public (bool, string) ProcessCreateService(CreateServiceRequest request)
        {
            if (ActiveLogin != LoginType.PLANNER)
            {
                return (false, "Must be logged in as Planner to create services.");
            }
            if (ServicesList.IsInList(request.ServiceNumberIn))
            {
                return (false, "Requested service already exists.");
            }
            var (record, message) = CreateService.MakeTransaction(request);
            if (record != null)
            {
                TransactionQueue.Push(record);
                return (true, message);
            }
            return (false, message);
        }

        public (bool, string) ProcessDeleteService(DeleteServiceRequest request)
        {
            if (ActiveLogin != LoginType.PLANNER)
            {
                return (false, "Must be logged in as Planner to delete services.");
            }
            if (!ServicesList.IsInList(request.ServiceNumberIn))
            {
                return (false, "Requested service does not exist.");
            }
            var (record, message) = DeleteService.MakeTransaction(request);
            if (record != null)
            {
                TransactionQueue.Push(record);
                ServicesList.DeleteService(request.ServiceNumberIn);
                return (true, message);
            }
            return (false, message);
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
