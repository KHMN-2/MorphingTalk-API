using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.UserDto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string NativeLanguage { get; set; }
        public string AboutStatus { get; set; }
        public string ProfilePicturePath { get; set; }
        public bool? IsFirstLogin { get; set; }

    }
}
