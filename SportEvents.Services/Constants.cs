using Microsoft.Extensions.Configuration;

namespace SportEvents.Infrastructure
{
    //visit the doc: https://api-sport-events.test.voxteneo.com/api/documentation
    public static class Constants
    {
        public static class AppConfig
        {
            public static IConfiguration Configuration { get; set; } = default!;
        }

        public static class ApiConfig
        {
            public static string ApiBaseUrl =>
                AppConfig.Configuration.GetValue<string>("Api:BaseUrl");
        }

        public static class ApiUrl
        {
            //base api url
            private static readonly string BaseUrl = $"{ApiConfig.ApiBaseUrl}/api/v1";

            public static readonly string Users = $"{BaseUrl}/users";
            public static readonly string UsersLogin = $"{Users}/login";
            public static readonly string Organizers = $"{BaseUrl}/organizers";
        }

        public static class LogMessages
        {
            public const string TokenExpiresMessage = "Token expired.";
            public const string UserNotAuthenticated = "User is not authenticated";
            public const string ErrorOccurred = "An error occurred.";
        }
    }
}
