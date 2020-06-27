namespace QIES.Frontend.Session
{
    interface ISession
    {
        public void Process(SessionManager manager, TransactionQueue queue);
    }
}
