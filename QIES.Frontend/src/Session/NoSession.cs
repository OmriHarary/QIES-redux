namespace QIES.Frontend.Session
{
    public class NoSession : ISession
    {
        public const string Prompt = " ----- ";

        public void Process(SessionManager manager, TransactionQueue queue)
        {
            Input input = manager.Input;
            bool run = true;
            string message = "You are not logged in, please log in before performing any other actions. (login)";
            string userInput;

            while (run)
            {
                userInput = input.TakeInput(message);
                if (userInput == "login")
                {
                    run = !Login(manager);
                }
                message = "Invalid input. You are not logged in, please log in before performing any other actions. (login)";
            }
        }

        public bool Login(SessionManager manager)
        {
            Input input = manager.Input;
            string message = "Login as agent or planner.";
            string userType = input.TakeInput(message);
            switch (userType)
            {
                case "agent":
                    manager.SetSession(new AgentSession());
                    return true;
                case "planner":
                    manager.SetSession(new PlannerSession());
                    return true;
                default:
                    return false;
            }
        }
    }
}
