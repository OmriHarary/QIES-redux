using System.Collections.Generic;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    public class AgentSession : ActiveSession
    {
        private int changedTickets;
        private int totalCancelledTickets;
        private Dictionary<ServiceNumber, int> cancelledTickets;

        public TransactionRecord CancelTicket(Input input)
        {
            return null;
        }

        public TransactionRecord ChangeTicket(Input input)
        {
            return null;
        }
    }
}
