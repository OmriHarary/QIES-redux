using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Api.Responses;
using QIES.Core.Users;

namespace QIES.Cli.Client
{
    public class QIESClient
    {
        private readonly ILogger<QIESClient> logger;
        private readonly HttpClient client;
        private Guid userId;

        public QIESClient(ILogger<QIESClient> logger, HttpClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        // public async Task<bool> GetService(string serviceNumber)
        // {
        //     var response = await client.GetAsync($"/api/v1/services/{serviceNumber}");
        //     return response.IsSuccessStatusCode;
        // }

        public async Task<LoginType> LoginAsync(string loginType)
        {
            var request = new LoginRequest();
            request.Login = loginType;

            using var response = await client.PostAsJsonAsync("/api/users/login", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            userId = result.Id;
            logger.LogInformation($"Received user ID: {userId}");
            return result.Type;
        }

        public async Task LogoutAsync()
        {
            var request = new LogoutRequest();
            request.UserId = userId;

            using var response = await client.PostAsJsonAsync("/api/users/logout", request);
            response.EnsureSuccessStatusCode();

            userId = Guid.Empty;
        }
    }
}
