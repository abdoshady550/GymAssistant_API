using GymAssistant_API.Model.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.User
{
    public class TrainerRequestConfiguration : IEntityTypeConfiguration<TrainerRequest>
    {
        public void Configure(EntityTypeBuilder<TrainerRequest> builder)
        {
            builder.HasKey(tr => tr.Id);

            builder.Property(tr => tr.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(tr => tr.Message)
                .HasMaxLength(500);

            builder.Property(tr => tr.CreatedAtUtc)
                .IsRequired();

            // Configure relationships
            builder.HasOne(tr => tr.Trainer)
                .WithMany()
                .HasForeignKey(tr => tr.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tr => tr.Trainee)
                .WithMany()
                .HasForeignKey(tr => tr.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create index for better query performance
            builder.HasIndex(tr => new { tr.TrainerId, tr.TraineeId, tr.Status });
            builder.HasIndex(tr => tr.CreatedAtUtc);
        }
    }
}
