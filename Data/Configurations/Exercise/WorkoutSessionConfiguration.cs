using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
        public void Configure(EntityTypeBuilder<WorkoutSession> builder)
        {
            builder.ToTable("WorkoutSessions");

            builder.HasKey(ws => ws.Id);

            builder.HasOne(ws => ws.Trainee)
                   .WithMany(c => c.Workouts)
                   .HasForeignKey(ws => ws.ClientProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ws => ws.CreatedByTrainer)
                   .WithMany()
                   .HasForeignKey(ws => ws.CreatedByTrainerId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
