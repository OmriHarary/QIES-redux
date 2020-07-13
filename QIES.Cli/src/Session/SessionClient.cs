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
                    "login"         => Login(),
                    "logout"        => Logout(),
                    "sellticket"    => SellTicket(),
                    "cancelticket"  => CancelTicket(),
                    "changeticket"  => ChangeTicket(),
                    "createservice" => CreateService(),
                    "deleteservice" => DeleteService(),
                    "exit"          => Exit(),
                    _               => (false, "Invalid input.")
                };
                run = !(command == "exit" && success);
                if (run)
                {
                    message = $"{response}\n{UserPrompts[activeLogin].Message}";
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

        public (bool, string) SellTicket()
        {
            var serviceNumberIn = input.TakeInput("Enter service number to sell tickets for.");

            int numberTicketsIn;
            try
            {
                numberTicketsIn = input.TakeNumericInput("Enter number of tickets to sell.");
            }
            catch (System.IO.InvalidDataException)
            {
                return (false, "A number was not entered.");
            }

            return controller.ProcessSellTicket(new SellTicketRequest(serviceNumberIn, numberTicketsIn));
        }

        public (bool, string) CancelTicket()
        {
            var serviceNumberIn = input.TakeInput("Enter service number of ticket you would like to cancel.");

            int numberTicketsIn;
            try
            {
                numberTicketsIn = input.TakeNumericInput("Enter number of tickets you want to cancel.");
            }
            catch (System.IO.InvalidDataException)
            {
                return (false, "A number was not entered.");
            }

            return controller.ProcessCancelTicket(new CancelTicketRequest(serviceNumberIn, numberTicketsIn));
        }

        public (bool, string) ChangeTicket()
        {
            var sourceNumberIn = input.TakeInput("Enter service number of the service you want to change.");
            var destinationNumberIn = input.TakeInput("Enter service number of the service you want to change to.");
            int numberTicketsIn;
            try
            {
                numberTicketsIn = input.TakeNumericInput("Enter number of tickets you want to change.");
            }
            catch (System.IO.InvalidDataException)
            {
                return (false, "A number was not entered.");
            }

            return controller.ProcessChangeTicket(new ChangeTicketRequest(sourceNumberIn, numberTicketsIn, destinationNumberIn));
        }

        public (bool, string) CreateService()
        {
            var serviceNumberIn = input.TakeInput("Enter service number of the service you wish to create.");
            var serviceDateIn = input.TakeInput("Enter service date of the service you wish to create.");
            var serviceNameIn = input.TakeInput("Enter service name of the service you wish to create.");
            return controller.ProcessCreateService(new CreateServiceRequest(serviceNumberIn, serviceDateIn, serviceNameIn));
        }

        public (bool, string) DeleteService()
        {
            var serviceNumberIn = input.TakeInput("Enter service number of the service you wish to delete.");
            var serviceNameIn = input.TakeInput("Enter service name of the service you wish to delete.");
            return controller.ProcessDeleteService(new DeleteServiceRequest(serviceNumberIn, serviceNameIn));
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