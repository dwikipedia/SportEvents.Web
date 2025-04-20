using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportEvents.Infrastructure
{
    public static class Constants
    {
        public static class ApiUrl
        {
            public const string BaseUrl = "api/v1";
            public const string Users = $"{BaseUrl}/users";
            public const string UsersLogin = $"{Users}/login";
            public const string Organizers = $"{BaseUrl}/organizers";
        }

        public static class LogMessages
        {
            public const string TokenExpiresMessage = "Token expired.";
        }
    }
}
