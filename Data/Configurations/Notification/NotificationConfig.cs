using GymAssistant_API.Model.Entities;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.Notifications
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Body)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(e => e.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.Data)
                .HasMaxLength(2000);

            builder.Property(e => e.IsRead)
                .IsRequired();

            builder.Property(e => e.IsSent)
                .IsRequired();

            builder.Property(e => e.CreatedAtUtc)
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => new { e.UserId, e.IsRead });
            builder.HasIndex(e => e.CreatedAtUtc);
        }
    }
}
