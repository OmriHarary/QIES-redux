using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Cli.Client.Requests;
using QIES.Cli.Client.Responses;
using QIES.Common.Records;

namespace QIES.Cli.Client
{
    public class QIESClient
    {
        private const string userIdHeader = "X-User-Id";
        private readonly ILogger<QIESClient> logger;
        private readonly HttpClient client;
        private readonly JsonSerializerOptions serializerOptions;
        public Guid UserId { get; private set; }

        public QIESClient(ILogger<QIESClient> logger, HttpClient client)
        {
            this.logger = logger;
            this.client = client;
            serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            serializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<LoginType> LoginAsync(string loginType)
        {
            var request = new LoginRequest(loginType);

            using var response = await client.PostAsJsonAsync("/api/v1/users/login", request, serializerOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>(serializerOptions);
            if (result is null)
            {
                logger.LogError("Unable to deserialize login response.");
                return LoginType.None;
            }

            UserId = result.UserId;
            logger.LogInformation($"Received user ID: {UserId}");
            client.DefaultRequestHeaders.Add(userIdHeader, UserId.ToString());
            return result.Type;
        }

        public async Task LogoutAsync()
        {
            var request = new LogoutRequest(UserId);

            using var response = await client.PostAsJsonAsync("/api/v1/users/logout", request, serializerOptions);
            response.EnsureSuccessStatusCode();

            UserId = Guid.Empty;
            client.DefaultRequestHeaders.Remove(userIdHeader);
        }

        public async Task<TransactionRecord?> SellTicketsAsync(string serviceNumber, int numberTickets)
        {
            var request = new SellTicketsRequest(numberTickets);

            using var response = await client.PostAsJsonAsync($"/api/v1/services/{serviceNumber}/tickets", request, serializerOptions);
            response.EnsureSuccessStatusCode();

            var record = await response.Content.ReadFromJsonAsync<TransactionRecord>(serializerOptions);

            if (record is null)
                logger.LogError("Unable to deserialize SellTickets response.");

            return record;
        }

        public async Task<TransactionRecord?> ChangeTicketsAsync(string serviceNumber, int numberTickets, string sourceServiceNumber)
        {
            var request = new ChangeTicketsRequest(numberTickets, sourceServiceNumber);

            using var response = await client.PostAsJsonAsync($"/api/v1/services/{serviceNumber}/tickets", request, serializerOptions);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var details = await response.Content.ReadFromJsonAsync<ProblemDetails>(serializerOptions);
                if (details is not null)
                    ex.Data.Add("detail", details.Detail);
                throw;
            }

            var record = await response.Content.ReadFromJsonAsync<TransactionRecord>(serializerOptions);

            if (record is null)
                logger.LogError("Unable to deserialize ChangeTickets response.");

            return record;
        }

        public async Task<TransactionRecord?> CancelTicketsAsync(string serviceNumber, int numberTickets)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{client.BaseAddress}api/v1/services/{serviceNumber}/tickets"),
                Content = JsonContent.Create(new CancelTicketsRequest(numberTickets), options: serializerOptions)
            };

            using var response = await client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var details = await response.Content.ReadFromJsonAsync<ProblemDetails>(serializerOptions);
                if (details is not null)
                    ex.Data.Add("detail", details.Detail);
                throw;
            }

            var record = await response.Content.ReadFromJsonAsync<TransactionRecord>(serializerOptions);

            if (record is null)
                logger.LogError("Unable to deserialize CancelTickets response.");

            return record;
        }

        public async Task<TransactionRecord?> CreateServiceAsync(string serviceNumber, string serviceDate, string serviceName)
        {
            var request = new CreateServiceRequest(serviceNumber, serviceDate, serviceName);

            using var response = await client.PostAsJsonAsync("/api/v1/services", request, serializerOptions);
            response.EnsureSuccessStatusCode();

            var record = await response.Content.ReadFromJsonAsync<TransactionRecord>(serializerOptions);

            if (record is null)
                logger.LogError("Unable to deserialize CreateService response.");

            return record;
        }

        public async Task<TransactionRecord?> DeleteServiceAsync(string serviceNumber, string serviceName)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{client.BaseAddress}api/v1/services/{serviceNumber}"),
                Content = JsonContent.Create(new DeleteServiceRequest(serviceName), options: serializerOptions)
            };

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var record = await response.Content.ReadFromJsonAsync<TransactionRecord>(serializerOptions);

            if (record is null)
                logger.LogError("Unable to deserialize DeleteService response.");

            return record;
        }
    }
}
