using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.UserDto;
using Domain.Entities.Users;

namespace Application.Interfaces.Services.Authentication
{
    public interface IUserService
    {
        public Task<ResponseViewModel<UserDto?>> GetUserByIdAsync(String? id);
        public Task<ResponseViewModel<UserDto?>> GetUserByEmailAsync(string email);
        public Task<ResponseViewModel<List<UserDto>>> GetAllUsersAsync();
        public Task<ResponseViewModel<LoggedInUserDto?>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        public Task<ResponseViewModel<UserDto?>> UpdateUserProfilePictureAsync(string userId, UpdateUserProfileDto updateUserProfileDto);
        public Task<ResponseViewModel<LoggedInUserDto>> GetLoggedUserByIdAsync(string? id);
    }
}
