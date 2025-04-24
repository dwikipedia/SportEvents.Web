using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SportEvents.Core.Models.Exceptions;
using SportEvents.Core.Models.User;
using SportEvents.Domain.Models.User;
using SportEvents.Domain.Repositories;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static SportEvents.Infrastructure.Constants;

namespace SportEvents.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ApiRequest _apiRequest;

        public UserRepository(
            IHttpClientFactory httpClientFactory,
            ApiRequest apiRequest)
        {
            _apiRequest = apiRequest;
        }

        public async Task<UserGetByIdRequest> GetUserById(int id)
        {
            var response = await _apiRequest.GetHttpRequestMessage(HttpMethod.Get, $"{ApiUrl.Users}/{id}");
            return await response.Content.ReadFromJsonAsync<UserGetByIdRequest>();
        }

        public async Task ChangePassword(int id, ChangePasswordRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"{ApiUrl.Users}/{id}/password",
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
                     LogMessages.ErrorOccurred,
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

                await _apiRequest.GetHttpRequestMessage(HttpMethod.Put, $"{ApiUrl.Users}/{request.Id}", getById);
            }
        }

        public async Task DeleteUserById(int id)
        {
            await _apiRequest.GetHttpRequestMessage(HttpMethod.Delete, $"{ApiUrl.Users}/{id}");
        }
    }
}
