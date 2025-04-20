using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SportEvents.Core.Models.Exceptions;
using SportEvents.Core.Models.User;
using SportEvents.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SportEvents.Domain.Models.User;
using static SportEvents.Infrastructure.Constants;

namespace SportEvents.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly string _apiBaseUrl;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            IHttpClientFactory httpClientFactory,
            ITokenProvider tokenProvider,
            IConfiguration config,
            ILogger<UserRepository> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _tokenProvider = tokenProvider;
            _apiBaseUrl = config["Api:BaseUrl"];
            _logger = logger;
        }

        public async Task<UserGetByIdRequest> GetUserById(int id)
        {
            string accessToken = _tokenProvider.GetToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                string message = "User is not authenticated";
                _logger.LogError("Unauthorized", message);

                throw new UnauthorizedAccessException(message);
            }

            // Create request with auth header
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl}/{ApiUrl.Users}/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Send request
            var response = await _httpClient.SendAsync(request);

            // Handle unauthorized (401) specifically
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Authentication expired or invalid");
            }

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"Not found.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserGetByIdRequest>();
        }

        public async Task ChangePassword(int id, ChangePasswordRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"{_apiBaseUrl}/{id}/password",
                new ChangePasswordRequest
                {
                    NewPassword = request.NewPassword,
                    OldPassword = request.OldPassword,
                    RepeatPassword = request.RepeatPassword
                }
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                if (errorResponse.Errors != null)
                {
                    throw new ApiException(
                       errorResponse.Message,
                       errorResponse.StatusCode,
                       errorResponse.Errors
                   );
                }

                throw new ApiException(
                     "Error occurred when changing password.",
                     errorResponse.StatusCode,
                     errorResponse.Errors
                 );
            }
        }


        public async Task UpdateUser(UserGetByIdRequest request)
        {
            string message = string.Empty;
            var getById = await GetUserById(request.Id);
            if (getById != null)
            {
                getById.FirstName = request.FirstName;
                getById.LastName = request.LastName;
                getById.Email = request.Email;

                string accessToken = _tokenProvider.GetToken();
                if (string.IsNullOrEmpty(accessToken))
                {
                    message = "User is not authenticated";
                    _logger.LogError("Unauthorized", message);

                    throw new UnauthorizedAccessException(message);
                }

                // Create request with auth header
                var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{_apiBaseUrl}/{ApiUrl.Users}/{request.Id}")
                {
                    Content = JsonContent.Create(getById)
                };
                
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Send request
                var response = await _httpClient.SendAsync(httpRequest);

                // Handle unauthorized (401) specifically
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    message = "Authentication expired or invalid";
                    _logger.LogError("Unauthorized", message);

                    throw new Exception(message);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    message = "Not found.";
                    _logger.LogError(message, message);

                    throw new Exception(message);
                }

                response.EnsureSuccessStatusCode();

            }
        }

        public async Task DeleteUserById(int id)
        {
            string accessToken = _tokenProvider.GetToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                string message = "User is not authenticated";
                _logger.LogError("Unauthorize", message);

                throw new UnauthorizedAccessException(message);
            }

            // Create request with auth header
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_apiBaseUrl}/{ApiUrl.Users}/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Send request
            var response = await _httpClient.SendAsync(request);

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
        }
    }
}
