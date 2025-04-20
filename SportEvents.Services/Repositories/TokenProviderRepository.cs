using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SportEvents.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SportEvents.Infrastructure.Constants;

namespace SportEvents.Infrastructure.Repositories
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenProvider> _logger;

        public TokenProvider(IHttpContextAccessor httpContextAccessor, ILogger<TokenProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("token");
        }

        public void SetToken(string token)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("token", token);
        }

        public bool IsAuthenticated()
        {
            string token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("No access token found in session");
                return false;
            }

            // Check token expiration
            if (IsTokenExpired(token))
            {
                _logger.LogInformation(LogMessages.TokenExpiresMessage);
                _httpContextAccessor.HttpContext.Session.Remove("token");
                return false;
            }

            return true;
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.ValidTo == DateTime.MinValue)
                    return false; // Token has no expiration

                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token expiration");
                return true; // Treat invalid tokens as expired
            }
        }
    }
}
