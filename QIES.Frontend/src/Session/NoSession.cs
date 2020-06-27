namespace QIES.Frontend.Session
{
    class NoSession : ISession
    {
        public void Process(SessionManager manager, TransactionQueue queue)
        {
            throw new System.NotImplementedException();
        }

        public bool Login(SessionManager manager)
        {
            return false;
        }
    }
}
