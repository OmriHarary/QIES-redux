using QIES.Common;
using QIES.Common.Record;
using QIES.Frontend.Transaction;

namespace QIES.Frontend.Session
{
    public abstract class ActiveSession : ISession
    {
        public abstract void Process(SessionManager manager, TransactionQueue queue);

        public TransactionRecord Logout(SessionManager manager)
        {
            Logout logout = new Logout();
            return logout.MakeTransaction(manager);
        }

        public TransactionRecord SellTicket(SessionManager manager)
        {
            SellTicket sellTicket = new SellTicket();
            return sellTicket.MakeTransaction(manager);
        }
    }
}
