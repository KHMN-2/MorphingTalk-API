using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDto
{
    public class UpdateUserDto
    {
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }
        
        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
        public string? Gender { get; set; }
        
        [StringLength(50, ErrorMessage = "Native language cannot exceed 50 characters")]
        public string? NativeLanguage { get; set; }
        
        [StringLength(500, ErrorMessage = "About status cannot exceed 500 characters")]
        public string? AboutStatus { get; set; }
        public string? ProfilePicturePath { get; set; }
        public bool? MuteNotifications { get; set; } = false; // Optional, if needed
    }
}
