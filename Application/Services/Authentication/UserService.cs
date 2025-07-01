using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.UserDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Authentication;
using Application.Interfaces.Services.FileService;
using Domain.Entities.Users;

namespace Application.Services.Authentication
{    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private IFileStorageService _fileStorageService;
        private IConversationRepository _conversationRepository;
        private IConversationUserRepository _conversationUserRepository;

        public UserService(IUserRepository userRepository, IFileStorageService fileStorageService,
            IConversationRepository conversationRepository, IConversationUserRepository conversationUserRepository)
        {
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
            _conversationRepository = conversationRepository;
            _conversationUserRepository = conversationUserRepository;
        }

        public async Task<ResponseViewModel<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = users.Select(user => UserDto.FromUser(user)).ToList();
            return new ResponseViewModel<List<UserDto>>(userDtos, "Users retrieved successfully", true, 200);
        }
        public async Task<ResponseViewModel<LoggedInUserDto>> GetLoggedUserByIdAsync(string? id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return new ResponseViewModel<LoggedInUserDto>(null, "User not found", false, 404);
            }
            var userDto = LoggedInUserDto.FromUser(user);
            return new ResponseViewModel<LoggedInUserDto>(userDto, "User found", true, 200);

        }


        public async Task<ResponseViewModel<UserDto>> GetUserByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ResponseViewModel<UserDto?>(null, "User ID is required", false, 400);
            }

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return new ResponseViewModel<UserDto?>(null, "User not found", false, 404);
            }

            var userDto = UserDto.FromUser(user);
            return new ResponseViewModel<UserDto>(userDto, "User found", true, 200);
        }
        public async Task<ResponseViewModel<UserDto?>> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return new ResponseViewModel<UserDto?>(null, "User not found", false, 404);
            }

            var userDto = UserDto.FromUser(user);
            return new ResponseViewModel<UserDto>(userDto, "User found", true, 200);
        }

        public async Task<ResponseViewModel<LoggedInUserDto?>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseViewModel<LoggedInUserDto?>(null, "User not found", false, 404);
                }

                // Update only non-null fields
                if (!string.IsNullOrWhiteSpace(updateUserDto.FullName))
                    user.FullName = updateUserDto.FullName;
                
                if (!string.IsNullOrWhiteSpace(updateUserDto.Gender))
                    user.Gender = updateUserDto.Gender;
                
                if (!string.IsNullOrWhiteSpace(updateUserDto.NativeLanguage))
                    user.NativeLanguage = updateUserDto.NativeLanguage;
                
                if (string.IsNullOrEmpty(updateUserDto.ProfilePicturePath))
                    user.ProfilePicturePath = updateUserDto.ProfilePicturePath;

                if (!string.IsNullOrWhiteSpace(updateUserDto.AboutStatus))
                    user.AboutStatus = updateUserDto.AboutStatus;

                if(updateUserDto.MuteNotifications != null)
                    user.MuteNotifications = updateUserDto.MuteNotifications.Value;



                user.LastUpdatedOn = DateTime.UtcNow;

                var updatedUser = await _userRepository.UpdateUserAsync(user);

                var userDto = new LoggedInUserDto
                {
                    Id = updatedUser.Id,
                    Email = updatedUser.Email,
                    FullName = updatedUser.FullName,
                    NativeLanguage = updatedUser.NativeLanguage,
                    AboutStatus = updatedUser.AboutStatus,
                    Gender = updatedUser.Gender,
                    ProfilePicPath = updatedUser.ProfilePicturePath,
                    MuteNotifications = updatedUser.MuteNotifications,
                    IsTrainedVoice = updatedUser.IsTrainedVoice,
                    IsFirstLogin = updatedUser.IsFirstLogin,
                };

                return new ResponseViewModel<LoggedInUserDto?>(userDto, "User updated successfully", true, 200);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<LoggedInUserDto?>(null, $"Error updating user: {ex.Message}", false, 500);
            }
        }    
        public async Task<ResponseViewModel<UserDto?>> UpdateUserProfilePictureAsync(string userId, UpdateUserProfileDto updateUserProfileDto)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseViewModel<UserDto?>(null, "User not found", false, 404);
                }

                // Upload the new profile picture
                var uploadedFilePath = await _fileStorageService.UploadProfilePictureAsync(updateUserProfileDto.ProfilePicture, userId);

                // Keep previous picture if requested
                if (updateUserProfileDto.KeepPreviousPicture && !string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    if (user.PastProfilePicturePaths == null)
                        user.PastProfilePicturePaths = new List<string>();
                    
                    user.PastProfilePicturePaths.Add(user.ProfilePicturePath);
                }
                else if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    // Delete the old profile picture if not keeping it
                    try
                    {
                        _fileStorageService.DeleteFile(user.ProfilePicturePath);
                    }
                    catch
                    {
                        // Log the error but don't fail the update
                    }
                }

                user.ProfilePicturePath = uploadedFilePath;
                user.LastUpdatedOn = DateTime.UtcNow;

                var updatedUser = await _userRepository.UpdateUserAsync(user);

                var userDto = new UserDto
                {
                    Id = updatedUser.Id,
                    Email = updatedUser.Email,
                    FullName = updatedUser.FullName,
                    NativeLanguage = updatedUser.NativeLanguage,
                    AboutStatus = updatedUser.AboutStatus,
                    Gender = updatedUser.Gender,
                    ProfilePicPath = updatedUser.ProfilePicturePath,
                };

                return new ResponseViewModel<UserDto?>(userDto, "Profile picture updated successfully", true, 200);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<UserDto?>(null, $"Error updating profile picture: {ex.Message}", false, 500);
            }
        }

        public async Task<ResponseViewModel<string>> DeleteUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new ResponseViewModel<string>(null, "User ID is required", false, 400);
                }

                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseViewModel<string>(null, "User not found", false, 404);
                }

                // Delete user profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    try
                    {
                        _fileStorageService.DeleteFile(user.ProfilePicturePath);
                    }
                    catch
                    {
                        // Log the error but don't fail the deletion
                    }
                }

                // Delete past profile pictures if they exist
                if (user.PastProfilePicturePaths?.Any() == true)
                {
                    foreach (var pastPicPath in user.PastProfilePicturePaths)
                    {
                        try
                        {
                            _fileStorageService.DeleteFile(pastPicPath);
                        }
                        catch
                        {
                            // Log the error but don't fail the deletion
                        }
                    }
                }

                await _userRepository.DeleteUserAsync(user);

                return new ResponseViewModel<string>(null, "User deleted successfully", true, 200);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<string>(null, $"Error deleting user: {ex.Message}", false, 500);
            }
        }
    }
}
