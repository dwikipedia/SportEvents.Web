using SportEvents.Core.Models.Auth;
using SportEvents.Core.Models.User;
using SportEvents.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportEvents.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<UserGetByIdRequest> GetUserById(int id);
        Task DeleteUserById(int id);
        Task UpdateUser(UserGetByIdRequest request);
        Task ChangePassword(int id, ChangePasswordRequest request);
    }
}
