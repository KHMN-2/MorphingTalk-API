using System.Reflection.Emit;
using Domain.Entities.Chatting;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationUser> ConversationUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<TextMessage> TextMessages { get; set; }
        public DbSet<VoiceMessage> VoiceMessages { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>()
                .HasDiscriminator<string>("MessageType")
                .HasValue<TextMessage>("Text")
                .HasValue<VoiceMessage>("Voice");

            builder.Entity<ConversationUser>()
                .HasIndex(cu => new { cu.ConversationId, cu.UserId })
                .IsUnique();

            builder.Entity<ConversationUser>()
               .HasOne(cu => cu.Conversation)
               .WithMany(c => c.ConversationUsers)
               .HasForeignKey(cu => cu.ConversationId)
               .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<ConversationUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.ConversationUsers)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(m => m.ConversationUser)
                .WithMany(cu => cu.Messages)
                .HasForeignKey(m => m.ConversationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friendship>()
                .HasKey(f => f.Id); // or HasKey(f => new { f.UserId1, f.UserId2 });

            builder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany()
                .HasForeignKey(f => f.UserId1)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.UserId2)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friendship>()
                .HasIndex(f => new { f.UserId1, f.UserId2 }).IsUnique();
        }
    }
}
