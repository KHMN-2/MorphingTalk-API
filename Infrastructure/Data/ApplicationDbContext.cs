using System.Reflection.Emit;
using Domain.Entities.AIModels;
using Domain.Entities.Chatting;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
        public DbSet<ImageMessage> ImageMessages { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<UserVoiceModel> UserVoiceModels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>()
                .HasDiscriminator<string>("MessageType")
                .HasValue<TextMessage>("Text")
                .HasValue<VoiceMessage>("Voice")
                .HasValue<ImageMessage>("Image");

            // Configure Message reply relationship
            builder.Entity<Message>()
                .HasOne(m => m.ReplyToMessage)
                .WithMany()
                .HasForeignKey(m => m.ReplyToMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Message StarredBy property as JSON
            builder.Entity<Message>()
                .Property(m => m.StarredBy)
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => v == null ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null)
                )
                .HasColumnType("nvarchar(max)");

            // Configure VoiceMessage specific properties
            builder.Entity<VoiceMessage>()
                .Property(vm => vm.TranslatedVoiceUrls)
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions)null)
                )
                .HasColumnType("nvarchar(max)");

            // Configure TextMessage specific properties
            builder.Entity<TextMessage>()
                .Property(tm => tm.TranslatedContents)
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions)null)
                )
                .HasColumnType("nvarchar(max)");

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

            // Configure BlockedByUserId as optional foreign key
            builder.Entity<Friendship>()
                .Property(f => f.BlockedByUserId)
                .IsRequired(false);

            // Configure User-UserVoiceModel relationship
            builder.Entity<User>()
                .HasOne(u => u.VoiceModel)
                .WithMany()
                .HasForeignKey(u => u.VoiceModelId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
