using GymAssistant_API.Model.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.User
{
    public class UserRequestConfiguration : IEntityTypeConfiguration<UserRequest>
    {
        public void Configure(EntityTypeBuilder<UserRequest> builder)
        {
            builder.ToTable("UserRequests");

            builder.HasKey(ur => ur.Id);

            builder.Property(ur => ur.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(ur => ur.Message)
                .HasMaxLength(500);

            builder.Property(ur => ur.CreatedAtUtc)
                .IsRequired();

            // Configure relationships with Restrict to avoid multiple cascade paths in SQL Server
            builder.HasOne(ur => ur.Trainer)
                .WithMany()
                .HasForeignKey(ur => ur.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ur => ur.Trainee)
                .WithMany()
                .HasForeignKey(ur => ur.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ur => ur.TrainerId);
            builder.HasIndex(ur => ur.TraineeId);
        }
    }
}
