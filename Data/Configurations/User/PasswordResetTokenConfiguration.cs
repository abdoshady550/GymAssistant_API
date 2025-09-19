using GymAssistant_API.Model.Entities;
using GymAssistant_API.Model.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.User
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Token).IsRequired().HasMaxLength(255);
            builder.HasIndex(e => e.Token).IsUnique();
            builder.HasIndex(e => new { e.Email, e.Token });
        }
    }
}
