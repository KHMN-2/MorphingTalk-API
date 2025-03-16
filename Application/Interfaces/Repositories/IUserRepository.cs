using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetUserByIdAsync(Guid id);
        public Task<List<User>> GetAllUsersAsync();
    }
}
