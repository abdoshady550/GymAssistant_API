using GymAssistant_API.Model.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.Chat
{
    public class ChatConfiguration : IEntityTypeConfiguration<ChatConversation>, IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatConversation> builder)
        {
            builder.ToTable("ChatConversations");

            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Trainer)
                   .WithMany()
                   .HasForeignKey(c => c.TrainerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Trainee)
                   .WithMany()
                   .HasForeignKey(c => c.TraineeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Conversation)
                   .HasForeignKey(m => m.ConversationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages");

            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Conversation)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.ConversationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Sender)
                   .WithMany()
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
