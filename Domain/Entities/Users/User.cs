using System;
using System.ComponentModel.DataAnnotations;
using Domain.Entities.AIModels;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Users
{
    public class User : IdentityUser
    {
        [Required]
        public string? FullName { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        [Required]
        public DateTime LastUpdatedOn { get; set; }
        [Required]
        public bool IsDeactivated { get; set; } = false;
        public bool? IsFirstLogin { get; set; } = null;
        public ICollection<ConversationUser> ConversationUsers { get; set; } = new List<ConversationUser>();
        public string? Gender { get; set; }
        public string? NativeLanguage { get; set; }
        public string? AboutStatus { get; set; }
        public string? ProfilePicturePath { get; set; }
        public ICollection<string>? PastProfilePicturePaths { get; set; }
        public bool IsOnline { get; set; } = false;
        public DateTime? LastSeen { get; set; } = null;
        public bool IsTrainedVoice { get; set; }
        public string? VoiceModelId { get; set; } // Foreign key
        public UserVoiceModel? VoiceModel { get; set; }
        public bool UseRobotVoice { get; set; } = true;
        public bool MuteNotifications { get; set; } = false; // Optional, if needed
        public bool TranslateMessages { get; set; } = false; // Optional, if needed

    }
}
