using System.Text.Json.Serialization;

namespace SportEvents.Core.Models.Auth
{
    public class AuthResult
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
