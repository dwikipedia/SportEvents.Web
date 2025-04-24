using Microsoft.Extensions.Logging;
using SportEvents.Domain.Models.Organizer;
using SportEvents.Domain.Repositories;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static SportEvents.Infrastructure.Constants;

namespace SportEvents.Infrastructure
{
    public class ApiRequest
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<ApiRequest> _logger;

        public ApiRequest(HttpClient httpClient,
                          ITokenProvider tokenProvider,
                          ILogger<ApiRequest> logger)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> GetHttpRequestMessage(HttpMethod method, string url)
        {
            var httpRequest = SetBearerForRequest(method, url);

            // Send request
            return await SendRequest(httpRequest);
        }

        public async Task<HttpResponseMessage> GetHttpRequestMessage<T>(HttpMethod method, string url, T? content) where T : class 
        {
            var httpRequest = SetBearerForRequest(method, url);
            httpRequest.Content = JsonContent.Create(content);
            
            // Send request
            return await SendRequest(httpRequest);
        }

        private HttpRequestMessage SetBearerForRequest(HttpMethod method, string url)
        {
            string accessToken = _tokenProvider.GetToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("Unauthorized", LogMessages.UserNotAuthenticated);

                throw new UnauthorizedAccessException(LogMessages.UserNotAuthenticated);
            }

            var httpRequest = new HttpRequestMessage(method, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return httpRequest;
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage httpRequest)
        {
            var response = await _httpClient.SendAsync(httpRequest);

            // Handle unauthorized (401) specifically
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception(LogMessages.TokenExpiresMessage);
            }

            //if (response.StatusCode == HttpStatusCode.NoContent)
            //{
            //    return Enumerable.Empty<OrganizerResponse>();
            //}

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"Not found.");
            }

            return response.EnsureSuccessStatusCode();
        }
    }
}
