using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.UserDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Authentication;
using Domain.Entities.Users;

namespace Application.Services.Authentication
{

    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseViewModel<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsFirstLogin = user.IsFirstLogin,
                NativeLanguage = user.NativeLanguage,
                AboutStatus = user.AboutStatus,
                Gender = user.Gender,
                ProfilePicPath = user.ProfilePicturePath,
                PastProfilePicsPath = user.PastProfilePicturePaths
            }).ToList();
            return new ResponseViewModel<List<UserDto>>(userDtos, "Users retrieved successfully", true, 200);
        }

        public async Task<ResponseViewModel<UserDto>> GetUserByIdAsync(string? id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return new ResponseViewModel<UserDto>(null, "User not found", false, 404);
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsFirstLogin = user.IsFirstLogin,
                NativeLanguage = user.NativeLanguage,
                AboutStatus = user.AboutStatus,
                Gender = user.Gender,
                ProfilePicPath = user.ProfilePicturePath,
                PastProfilePicsPath = user.PastProfilePicturePaths
            };
            return new ResponseViewModel<UserDto>(userDto, "User found", true, 200);
        }
        public async Task<ResponseViewModel<UserDto>> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return new ResponseViewModel<UserDto>(null, "User not found", false, 404);
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsFirstLogin = user.IsFirstLogin,
                NativeLanguage = user.NativeLanguage,
                AboutStatus = user.AboutStatus,
                Gender = user.Gender,
                ProfilePicPath = user.ProfilePicturePath,
                PastProfilePicsPath = user.PastProfilePicturePaths
            };
            return new ResponseViewModel<UserDto>(userDto, "User found", true, 200);
        }

    }
}
