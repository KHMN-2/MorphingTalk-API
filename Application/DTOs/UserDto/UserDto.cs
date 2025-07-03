using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.DTOs.UserDto
{    public class UserDto
    {
        public string? Id { get; set; }
        
        public string? FullName { get; set; }
        
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? NativeLanguage { get; set; }
        public string? AboutStatus { get; set; }
        public string? ProfilePicPath { get; set; }
        public DateTime LastUpdated { get; set; }


        public static UserDto FromUser(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                NativeLanguage = user.NativeLanguage,
                AboutStatus = user.AboutStatus,
                Gender = user.Gender,
                ProfilePicPath = user.ProfilePicturePath,
                LastUpdated = user.LastUpdatedOn,
            };
        }
    }
    public class LoggedInUserDto : UserDto
    {
        public bool IsTrainedVoice { get; set; }
        public bool UseRobotVoice { get; set; } = true;
        public bool MuteNotifications { get; set; } = false; // Optional, if needed
        public bool TranslateMessages { get; set; } = false; // Optional, if needed
        public bool? IsFirstLogin { get; set; }



        public static LoggedInUserDto FromUser(User user)
        {
            return new LoggedInUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsFirstLogin = user.IsFirstLogin,
                NativeLanguage = user.NativeLanguage,
                AboutStatus = user.AboutStatus,
                Gender = user.Gender,
                ProfilePicPath = user.ProfilePicturePath,
                LastUpdated = user.LastUpdatedOn,
                MuteNotifications = user.MuteNotifications,
            };
        }
    }
}
