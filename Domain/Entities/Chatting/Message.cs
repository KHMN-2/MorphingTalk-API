﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read
    }
    public abstract class Message
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid ConversationUserId { get; set; }
        public DateTime SentAt { get; set; }
        public MessageStatus Status { get; set; }
        
        // Reply functionality
        public Guid? ReplyToMessageId { get; set; }
        public virtual Message? ReplyToMessage { get; set; }
        
        // Star functionality - list of user IDs who starred this message
        public List<string> StarredBy { get; set; } = new List<string>();
        
        // Soft delete functionality
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        
        public virtual Conversation Conversation { get; set; }
        public virtual ConversationUser ConversationUser { get; set; }
    }
}
