using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class FirebaseLoginDto
    {
        [Required]
        public string IdToken { get; set; }
    }
} 