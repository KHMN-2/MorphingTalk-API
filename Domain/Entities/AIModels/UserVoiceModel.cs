using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.AIModels
{
    public enum UserVoiceModelStatus
    {
        Training,
        Completed,
        Failed
    }
    public class UserVoiceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserVoiceModelStatus Status { get; set; }
    }
}
