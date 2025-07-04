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
        public Task<User> GetUserByIdAsync(String id);
        public Task<List<User>> GetAllUsersAsync();

        public Task<User> GetUserByEmailAsync(string email);

        public Task<User> GetUserByVoiceModelIdAsync(string voiceModelId);

        public Task<User> AddUserAsync(User user);

        public Task<User> UpdateUserAsync(User user);

        public Task<User> DeleteUserAsync(User user);

        public Task<User> DeactivateUserAsync(User user);

        public Task<User> ActivateUserAsync(User user);

        public Task<User> ChangePasswordAsync(User user, string newPassword);

        public Task<User> ChangeEmailAsync(User user, string newEmail);

        public Task ConfirmEmail(string email);

    }
}


