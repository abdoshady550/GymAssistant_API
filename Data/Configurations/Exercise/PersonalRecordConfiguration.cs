using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class PersonalRecordConfiguration : IEntityTypeConfiguration<PersonalRecord>
    {
        public void Configure(EntityTypeBuilder<PersonalRecord> builder)
        {
            builder.ToTable("PersonalRecords");

            builder.HasKey(pr => pr.Id);

            builder.HasOne(pr => pr.User)
                   .WithMany(c => c.PersonalRecords)
                   .HasForeignKey(pr => pr.ClientProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pr => pr.Exercise)
                   .WithMany()
                   .HasForeignKey(pr => pr.ExerciseId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(pr => pr.UserExercise)
                   .WithMany()
                   .HasForeignKey(pr => pr.UserExerciseId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(pr => pr.WorkoutSession)
                   .WithMany()
                   .HasForeignKey(pr => pr.WorkoutSessionId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
