using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities.Users;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContext _identityContext;

        public UserRepository(ApplicationDbContext context, ApplicationDbContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }

        public Task<User> ActivateUserAsync(User user)
        {
            
            throw new NotImplementedException();
        }

        public async Task<User> AddUserAsync(User user)
        {
            user.CreatedOn = DateTime.Now;
            user.LastUpdatedOn = DateTime.Now;
            user.IsDeactivated = false;
            user.IsFirstLogin = true;

            _identityContext.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public Task<User> ChangeEmailAsync(User user, string newEmail)
        {
            throw new NotImplementedException();
        }

        public Task<User> ChangePasswordAsync(User user, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task ConfirmEmail(string email)
        {
            var existingUser = _identityContext.Users.SingleOrDefault(u => u.Email == email);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            existingUser.EmailConfirmed = true;

            _identityContext.Users.Update(existingUser);
            _identityContext.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<User> DeactivateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            var users = _identityContext.Users.ToList();
            return Task.FromResult(users);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = _identityContext.Users.FirstOrDefault<User>(u => u.Email == email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await _identityContext.Users.FindAsync(id);
            if (user == null) { 
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = await _identityContext.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            existingUser.UserName = user.UserName;
            existingUser.FullName = user.FullName;
            existingUser.LastUpdatedOn = DateTime.Now;
            existingUser.IsDeactivated = user.IsDeactivated;
            existingUser.IsFirstLogin = user.IsFirstLogin;

            _identityContext.Users.Update(existingUser);
            await _identityContext.SaveChangesAsync();

            return existingUser;
        }
    }
}
