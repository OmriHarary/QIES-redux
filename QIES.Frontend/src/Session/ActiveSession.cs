using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    abstract class ActiveSession : ISession
    {
        public void Process(SessionManager manager, TransactionQueue queue)
        {
            throw new System.NotImplementedException();
        }

        public TransactionRecord Logout(Input input)
        {
            return null;
        }

        public TransactionRecord SellTicket(Input input)
        {
            return null;
        }
    }
}
