using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SportEvents.Core.Models.Auth;
using SportEvents.Core.Models.Exceptions;
using SportEvents.Domain.Repositories;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using static SportEvents.Infrastructure.Constants;

namespace SportEvents.Infrastructure.Repositories
{

    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiBaseUrl;

        public AuthRepository(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _apiBaseUrl = config["Api:BaseUrl"];
        }

        public async Task RegisterAsync(RegisterRequest request)
        {

            var response = await _httpClient.PostAsJsonAsync(
                $"{_apiBaseUrl}/{ApiUrl.Users}",
                request
            );

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(content);

                // Throw exception with parsed errors
                throw new ApiException(
                    errorResponse.Message,
                    errorResponse.StatusCode,
                    errorResponse.Errors
                );
            }

        }

        public async Task LoginAsync(LoginRequest request)
        {
            var authResult = await AuthenticateWithApi(request);

            StoreSession(authResult);
            ClaimsIdentity claimsIdentity = SetCookies(request, authResult);

            await _httpContextAccessor.HttpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme,
              new ClaimsPrincipal(claimsIdentity),
              new AuthenticationProperties
              {
                  IsPersistent = true // For "Remember Me" functionality
              });
        }

        private void StoreSession(AuthResult result)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("id", result.Id.ToString());
            session.SetString("email", result.Email);
            session.SetString("token", result.Token);
        }

        private void ClearSession()
        {
            _httpContextAccessor.HttpContext.Session.Remove("id");
            _httpContextAccessor.HttpContext.Session.Remove("email");
            _httpContextAccessor.HttpContext.Session.Remove("token");

            _httpContextAccessor.HttpContext.Session.Clear();
        }

        private ClaimsIdentity SetCookies(LoginRequest request, AuthResult authResult)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, request.Email),
                new Claim("Token", authResult.Token)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return claimsIdentity;
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            ClearSession();
        }

        private async Task<AuthResult> AuthenticateWithApi(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_apiBaseUrl}/{ApiUrl.UsersLogin}",
                new
                {
                    email = request.Email,
                    password = request.Password
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
                     "Invalid Username or Password",
                     errorResponse.StatusCode,
                     errorResponse.Errors
                 );
            }

            var authResult = await response.Content.ReadFromJsonAsync<AuthResult>();

            if (string.IsNullOrEmpty(authResult?.Token))
            {
                throw new Exception("Invalid API response: Missing access token");
            }

            return authResult;
        }
    }
}
