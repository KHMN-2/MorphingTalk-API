using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities.Users;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<List<User>> GetAllUsersAsync()
        {
            List<User> users = new List<User>();
            users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Test User",
                FullName = "User Test",
                CreatedOn = DateTime.Now,
                LastUpdatedOn = DateTime.Now,
                IsDeactivated = false,
                IsFirstLogin = true
            });
            users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Test User 2",
                FullName = "User Test 2",
                CreatedOn = DateTime.Now,
                LastUpdatedOn = DateTime.Now,
                IsDeactivated = false,
                IsFirstLogin = true
            });
            users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Test User 3",
                FullName = "User Test 3",
                CreatedOn = DateTime.Now,
                LastUpdatedOn = DateTime.Now,
                IsDeactivated = false,
                IsFirstLogin = true
            });

            return Task.FromResult(users);
        }

        public Task<User> GetUserByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
