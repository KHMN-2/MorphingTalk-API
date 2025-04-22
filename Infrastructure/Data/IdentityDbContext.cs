using Domain.Entities.Chatting;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class IdentityDbContext : IdentityDbContext<User>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }
        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<TextMessage> TextMessages { get; set; }
        public DbSet<VoiceMessage> VoiceMessages { get; set; }
        public DbSet<ConversationUser> ConversationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConversationUser>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(cu => cu.UserId);

            modelBuilder.Entity<ConversationUser>()
                .HasOne<Conversation>()
                .WithMany()
                .HasForeignKey(cu => cu.ConversationId);

            modelBuilder.Entity<Message>()
                .HasOne<Conversation>()
                .WithMany()
                .HasForeignKey(m => m.ConversationId);

            modelBuilder.Entity<TextMessage>()
                .HasBaseType<Message>();
            modelBuilder.Entity<VoiceMessage>()
                .HasBaseType<Message>();



            modelBuilder.Entity<ConversationUser>()
                .HasKey(cu => new { cu.UserId, cu.ConversationId });


        }
    }
}
