using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Cli.Client;

namespace QIES.Cli.Shell
{
    public class ShellService : IShellService
    {
        private static readonly (string Prompt, string Message)[] userPrompts = new (string Prompt, string Message)[]
        {
            (Prompt: " ----- ", Message: "You are not logged in, please log in before performing any other actions. (login)"),
            (Prompt: " AGENT ", Message: "Logged in as Agent. Enter command to begin a transaction."),
            (Prompt: "PLANNER", Message: "Logged in as Planner. Enter command to begin a transaction.")
        };
        private readonly ILogger<ShellService> logger;
        private readonly QIESClient client;
        private readonly Input input;
        private int activeLogin;

        public ShellService(ILogger<ShellService> logger, QIESClient client)
        {
            this.logger = logger;
            this.client = client;
            activeLogin = 0;
            input = new Input(userPrompts[activeLogin].Prompt);
        }

        public async Task<int> RunAsync()
        {
            var run = true;
            var message = userPrompts[activeLogin].Message;
            string command;

            while (run)
            {
                input.Prompt = userPrompts[activeLogin].Prompt;
                command = input.TakeInput(message);

                var (success, response) = command switch
                {
                    "login"         => await Login(),
                    "logout"        => await Logout(),
                    "sellticket"    => await SellTicket(),
                    "cancelticket"  => await CancelTicket(),
                    "changeticket"  => await ChangeTicket(),
                    "createservice" => await CreateService(),
                    "deleteservice" => await DeleteService(),
                    "exit"          => Exit(),
                    _               => (false, "Invalid input.")
                };
                run = !(command == "exit" && success);
                if (run)
                {
                    message = $"{response}\n{userPrompts[activeLogin].Message}";
                }
            }

            return 0;
        }

        private async Task<(bool, string)> Login()
        {
            var userType = input.TakeInput("Login as agent or planner.");
            int newLogin;
            try
            {
                newLogin = await client.LoginAsync(userType);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, $"Got an unsuccessful response from attempting to login: [{ex.StatusCode}]");
                return (false, "Invalid input.");
            }

            activeLogin = newLogin;
            var message = newLogin == 1 ? "Successfully logged in as Agent." : "Successfully logged in as Planner.";

            return (true, message);
        }

        private async Task<(bool, string)> Logout()
        {
            try
            {
                await client.LogoutAsync();
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, $"Got an unsuccessful response from attempting to logout: [{ex.StatusCode}]");
                return (false, ""); // TODO: Failure case message once Logout is fixed on server side
            }

            activeLogin = 0;
            return (true, "Logged out.");
        }

        private async Task<(bool, string)> SellTicket()
        {
            return (false, "Not implemented");
        }

        private async Task<(bool, string)> CancelTicket()
        {
            return (false, "Not implemented");
        }

        private async Task<(bool, string)> ChangeTicket()
        {
            return (false, "Not implemented");
        }

        private async Task<(bool, string)> CreateService()
        {
            return (false, "Not implemented");
        }

        private async Task<(bool, string)> DeleteService()
        {
            return (false, "Not implemented");
        }

        private (bool, string) Exit()
        {
            if (activeLogin != 0)
            {
                return (false, "Please logout before exiting.");
            }
            return (true, "Goodbye.");
        }
    }
}
