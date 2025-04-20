using SportEvents.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportEvents.Domain.Repositories
{
    public interface IAuthRepository
    {
        Task RegisterAsync(RegisterRequest request);
        Task LoginAsync(LoginRequest request);
        Task LogoutAsync();
    }
}
