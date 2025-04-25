using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.Interfaces.Services.Authentication
{
    public interface IUserService
    {
        public Task<User> GetUserByIdAsync(string id);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<List<User>> GetAllUsersAsync();

    }
}
