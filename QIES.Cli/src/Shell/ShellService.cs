using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Cli.Client;
using QIES.Cli.Client.Responses;
using QIES.Common.Records;

namespace QIES.Cli.Shell
{
    public class ShellService : IShellService
    {
        private static readonly Dictionary<LoginType, (string Prompt, string Message)> userPrompts
            = new Dictionary<LoginType, (string Prompt, string Message)>()
            {
                {
                    LoginType.None,
                    (Prompt: " ----- ", Message: "You are not logged in, please log in before performing any other actions. (login)")
                },
                {
                    LoginType.Agent,
                    (Prompt: " AGENT ", Message: "Logged in as Agent. Enter command to begin a transaction.")
                },
                {
                    LoginType.Planner,
                    (Prompt: "PLANNER", Message: "Logged in as Planner. Enter command to begin a transaction.")
                }
            };
        private readonly ILogger<ShellService> logger;
        private readonly QIESClient client;
        private readonly Input input;
        private LoginType activeLogin;

        public ShellService(ILogger<ShellService> logger, QIESClient client)
        {
            this.logger = logger;
            this.client = client;
            activeLogin = LoginType.None;
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
                    "sellticket"    => await SellTickets(),
                    "cancelticket"  => await CancelTickets(),
                    "changeticket"  => await ChangeTickets(),
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

        public async Task<(bool, string)> Login()
        {
            var userType = input.TakeInput("Login as agent or planner.");
            LoginType newLogin;
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
            var message = newLogin != LoginType.None ? $"Successfully logged in as {newLogin}"
                : "Response was successful but did not receive a login type.";

            return (newLogin != LoginType.None, message);
        }

        public async Task<(bool, string)> Logout()
        {
            try
            {
                await client.LogoutAsync();
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, $"Got an unsuccessful response from attempting to logout: [{ex.StatusCode}]");
                return (false, "Logout failed.");
            }

            activeLogin = 0;
            return (true, "Logged out.");
        }

        public async Task<(bool, string)> SellTickets()
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

            TransactionRecord? record;
            try
            {
                record = await client.SellTicketsAsync(serviceNumberIn, numberTicketsIn);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, $"Got an unsuccessful response from attempting to sell tickets: [{ex.StatusCode}]");
                return ex.StatusCode switch
                {
                    HttpStatusCode.BadRequest   => (false, "Invalid request."),
                    HttpStatusCode.Unauthorized => (false, "Must be logged in to sell tickets."),
                    HttpStatusCode.NotFound     => (false, "Requested service does not exist."),
                    _                           => (false, $"An unexpected error occured: {ex.Message}")
                };
            }

            if (record is null)
                return (false, "Was unable to read response.");

            return (true, $"{record.NumberTickets} ticket(s) sold for service {record.SourceNumber}");
        }

        public async Task<(bool, string)> CancelTickets()
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

            TransactionRecord? record;
            try
            {
                record = await client.CancelTicketsAsync(serviceNumberIn, numberTicketsIn);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, $"Got an unsuccessful response from attempting to cancel tickets: [{ex.StatusCode}]");
                return ex.StatusCode switch
                {
                    HttpStatusCode.BadRequest       => (false, "Invalid request."),
                    HttpStatusCode.Unauthorized     => (false, "Must be logged in to cancel tickets."),
                    HttpStatusCode.NotFound         => (false, "Requested service does not exist."),
                    HttpStatusCode.TooManyRequests  => (false, ex.Data["detail"]?.ToString() ?? ex.Message),
                    _                               => (false, $"An unexpected error occured: {ex.Message}")
                };
            }

            if (record is null)
                return (false, "Was unable to read response.");

            return (true, $"{record.NumberTickets} ticket(s) canceled from service {record.SourceNumber}");
        }

        public async Task<(bool, string)> ChangeTickets()
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

            TransactionRecord? record;
            try
            {
                record = await client.ChangeTicketsAsync(destinationNumberIn, numberTicketsIn, sourceNumberIn);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, $"Got an unsuccessful response from attempting to change tickets: [{ex.StatusCode}]");
                return ex.StatusCode switch
                {
                    HttpStatusCode.BadRequest       => (false, "Invalid request."),
                    HttpStatusCode.Unauthorized     => (false, "Must be logged in to cancel tickets."),
                    HttpStatusCode.NotFound         => (false, "Requested service does not exist."),
                    HttpStatusCode.TooManyRequests  => (false, ex.Data["detail"]?.ToString() ?? ex.Message),
                    _                               => (false, $"An unexpected error occured: {ex.Message}")
                };
            }

            if (record is null)
                return (false, "Was unable to read response.");

            return (true, $"{record.NumberTickets} ticket(s) changed from service {record.SourceNumber} to service {record.DestinationNumber}");
        }

        public async Task<(bool, string)> CreateService()
        {
            var serviceNumberIn = input.TakeInput("Enter service number of the service you wish to create.");
            var serviceDateIn = input.TakeInput("Enter service date of the service you wish to create.");
            var serviceNameIn = input.TakeInput("Enter service name of the service you wish to create.");

            TransactionRecord? record;
            try
            {
                record = await client.CreateServiceAsync(serviceNumberIn, serviceDateIn, serviceNameIn);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, $"Got an unsuccessful response from attempting to create a service: [{ex.StatusCode}]");
                return ex.StatusCode switch
                {
                    HttpStatusCode.BadRequest       => (false, "Invalid request."),
                    HttpStatusCode.Unauthorized or
                    HttpStatusCode.Forbidden        => (false, "Must be logged in as Planner to create services."),
                    HttpStatusCode.Conflict         => (false, "Requested service already exists."),
                    _                               => (false, $"An unexpected error occured: {ex.Message}")
                };
            }

            if (record is null)
                return (false, "Was unable to read response.");

            return (true, $"Service {record.SourceNumber} created on {record.ServiceDate} with the name {record.ServiceName}");
        }

        public async Task<(bool, string)> DeleteService()
        {
            var serviceNumberIn = input.TakeInput("Enter service number of the service you wish to delete.");
            var serviceNameIn = input.TakeInput("Enter service name of the service you wish to delete.");

            TransactionRecord? record;
            try
            {
                record = await client.DeleteServiceAsync(serviceNumberIn, serviceNameIn);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, $"Got an unsuccessful response from attempting to delete a service: [{ex.StatusCode}]");
                return ex.StatusCode switch
                {
                    HttpStatusCode.BadRequest       => (false, "Invalid request."),
                    HttpStatusCode.Unauthorized or
                    HttpStatusCode.Forbidden        => (false, "Must be logged in as Planner to delete services."),
                    HttpStatusCode.NotFound         => (false, "Requested service does not exist."),
                    _                               => (false, $"An unexpected error occured: {ex.Message}")
                };
            }

            if (record is null)
                return (false, "Was unable to read response.");

            return (false, $"Service {record.SourceNumber} with service name {record.ServiceName} was deleted");
        }

        public (bool, string) Exit()
        {
            if (activeLogin != LoginType.None)
            {
                return (false, "Please logout before exiting.");
            }
            return (true, "Goodbye.");
        }
    }
}
