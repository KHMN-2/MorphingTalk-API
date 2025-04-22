using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities.Chatting;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{

    public class ConversationUserRepository : IConversationUserRepository
    {
        private readonly IdentityDbContext _identityDbContext;
        public ConversationUserRepository(IdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task AddConverstaionUserAsync(ConversationUser conversationUser)
        {
            await _identityDbContext.ConversationUsers.AddAsync(conversationUser);
        }
    }
}
