using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class ExerciseSetConfiguration : IEntityTypeConfiguration<ExerciseSet>
    {
        public void Configure(EntityTypeBuilder<ExerciseSet> builder)
        {
            builder.ToTable("ExerciseSets");

            builder.HasKey(es => es.Id);

            builder.HasOne(es => es.WorkoutExercise)
                   .WithMany(we => we.Sets)
                   .HasForeignKey(es => es.WorkoutExerciseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
