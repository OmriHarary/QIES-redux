using System.Collections.Generic;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction;

namespace QIES.Cli.Session
{
    public class SessionClient
    {
        private static readonly Dictionary<LoginType, (string Prompt, string Message)> UserPrompts
            = new Dictionary<LoginType, (string Prompt, string Message)>()
            {
                {
                    LoginType.NONE,
                    (Prompt: " ----- ", Message: "You are not logged in, please log in before performing any other actions. (login)")
                },
                {
                    LoginType.AGENT,
                    (Prompt: " AGENT ", Message: "Logged in as Agent. Enter command to begin a transaction.")
                },
                {
                    LoginType.PLANNER,
                    (Prompt: "PLANNER", Message: "Logged in as Planner. Enter command to begin a transaction.")
                }
            };

        private LoginType activeLogin;
        private Input input;
        private SessionController controller;

        public SessionClient(SessionController controller)
        {
            this.activeLogin = LoginType.NONE;
            this.input = new Input(UserPrompts[activeLogin].Prompt);
            this.controller = controller;
        }

        public int Operate()
        {
            var run = true;
            var message = UserPrompts[activeLogin].Message;
            string command;

            while (run)
            {
                command = input.TakeInput(message);

                var (success, response) = command switch
                {
                    "login"     => Login(),
                    "logout"    => Logout(),
                    "exit"      => Exit(),
                    _           => (false, "Invalid input.")
                };
                run = !(command == "exit" && success);
                if (run)
                {
                    message = success ? UserPrompts[activeLogin].Message : $"{response} {UserPrompts[activeLogin].Message}";
                }
            }

            return 0;
        }

        public (bool, string) Login()
        {
            var userType = input.TakeInput("Login as agent or planner.");
            var (success, response, activeLogin) = controller.ProcessLogin(userType);
            this.activeLogin = activeLogin;
            this.input.Prompt = UserPrompts[activeLogin].Prompt;
            return (success, response);
        }

        public (bool, string) Logout()
        {
            var (success, response, activeLogin) = controller.ProcessLogout(new LogoutRequest());
            this.activeLogin = activeLogin;
            this.input.Prompt = UserPrompts[activeLogin].Prompt;
            return (success, response);
        }

        public (bool, string) Exit()
        {
            if (activeLogin != LoginType.NONE)
            {
                return (false, "Please logout before exiting.");
            }
            return (true, "Goodbye.");
        }
    }
}
