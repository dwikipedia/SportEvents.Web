using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportEvents.Core.Models.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public Dictionary<string, List<string>> Errors { get; }

        public ApiException(string message, int statusCode, Dictionary<string, List<string>> errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors ?? new Dictionary<string, List<string>>();
        }
    }

}
