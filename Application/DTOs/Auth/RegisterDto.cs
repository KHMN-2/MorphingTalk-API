using System.ComponentModel.DataAnnotations;

namespace MorphingTalk_API.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string gender { get; set; }

        [Required]
        public string nativeLanguage { get; set; }

        public string? aboutStatus { get; set; }

        public string? profilePicturePath { get; set; }



    }
}
