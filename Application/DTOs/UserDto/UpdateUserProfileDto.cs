using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDto
{
    public class UpdateUserProfileDto
    {
        [Required]
        public IFormFile ProfilePicture { get; set; }
        
        public bool KeepPreviousPicture { get; set; } = true;
    }
}
