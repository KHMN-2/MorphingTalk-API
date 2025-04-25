using System;
using System.ComponentModel.DataAnnotations;
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
        public ICollection<ConversationUser> ConversationUsers { get; set; }
    }
}
