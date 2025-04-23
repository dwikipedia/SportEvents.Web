using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SportEvents.Domain.Models.Organizer;
using SportEvents.Domain.Repositories;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static SportEvents.Infrastructure.Constants;

namespace SportEvents.Infrastructure.Repositories
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<UserRepository> _logger;

        public OrganizerRepository(
            IHttpClientFactory httpClientFactory,
            ITokenProvider tokenProvider,
            IConfiguration config,
            ILogger<UserRepository> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        public Task CreateOrganizer(CreateOrganizer organizer)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOrganizer(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrganizerResponse>> GetAllOrganizers(OrganizersRequest request)
        {
            string accessToken = _tokenProvider.GetToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                string message = "User is not authenticated";
                _logger.LogError("Unauthorized", message);

                throw new UnauthorizedAccessException(message);
            }

            // Create request with auth header
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, ApiUrl.Organizers);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Send request
            var response = await _httpClient.SendAsync(httpRequest);

            // Handle unauthorized (401) specifically
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Authentication expired or invalid");
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"Not found.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<OrganizerResponse>>();
        }

        public Task<OrganizerResponse> GetOrganizer()
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrganizer(OrganizerResponse request)
        {
            throw new NotImplementedException();
        }
    }
}
