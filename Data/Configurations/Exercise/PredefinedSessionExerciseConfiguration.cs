using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.Exercise
{
    public class PredefinedSessionExerciseConfiguration : IEntityTypeConfiguration<PredefinedSessionExercise>
    {
        public void Configure(EntityTypeBuilder<PredefinedSessionExercise> builder)
        {
            builder.ToTable("PredefinedSessionExercises");

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.PredefinedWorkoutSession)
                   .WithMany(s => s.Exercises)
                   .HasForeignKey(e => e.PredefinedWorkoutSessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Exercise)
                   .WithMany()
                   .HasForeignKey(e => e.ExerciseId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
