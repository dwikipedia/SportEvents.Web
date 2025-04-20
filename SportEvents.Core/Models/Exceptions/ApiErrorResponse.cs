using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SportEvents.Core.Models.Exceptions
{
    public class ApiErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("errors")]
        public Dictionary<string, List<string>> Errors { get; set; }

        [JsonPropertyName("status_code")]
        public int StatusCode { get; set; }
    }
}
