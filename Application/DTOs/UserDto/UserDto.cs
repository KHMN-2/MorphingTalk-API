using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs.UserDto
{
    public class UserDto
    {
        public string Id { get; set; }
        
        [JsonPropertyName("Name")]
        public string FullName { get; set; }
        
        public string Email { get; set; }
        public string Gender { get; set; }
        public string NativeLanguage { get; set; }
        public string AboutStatus { get; set; }
        public string ProfilePicPath { get; set; }
        public ICollection<string> PastProfilePicsPath { get; set; }
        public bool? IsFirstLogin { get; set; }

    }
}
