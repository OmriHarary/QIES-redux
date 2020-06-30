using QIES.Frontend.Transaction;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    public abstract class ActiveSession : ISession
    {
        public abstract void Process(SessionManager manager, TransactionQueue queue);

        public TransactionRecord Logout(Input input)
        {
            Logout logout = new Logout();
            return logout.MakeTransaction(input);
        }

        public TransactionRecord SellTicket(Input input)
        {
            SellTicket sellTicket = new SellTicket();
            return sellTicket.MakeTransaction(input);
        }
    }
}
