using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
    {
        public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
        {
            builder.ToTable("WorkoutExercises");

            builder.HasKey(we => we.Id);

            builder.HasOne(we => we.User)
                   .WithMany()
                   .HasForeignKey(we => we.ClientProfileId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
