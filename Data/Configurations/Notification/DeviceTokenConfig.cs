using GymAssistant_API.Model.Entities;
using GymAssistant_API.Model.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.Notifications
{
    public class DeviceTokenConfig : IEntityTypeConfiguration<DeviceToken>
    {
        public void Configure(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.ToTable("DeviceTokens");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Platform)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.IsActive)
                .IsRequired();

            builder.Property(e => e.LastUsedUtc)
                .IsRequired();

            builder.Property(e => e.CreatedAtUtc)
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.Token);
            builder.HasIndex(e => new { e.UserId, e.Token }).IsUnique();
        }
    }
}
