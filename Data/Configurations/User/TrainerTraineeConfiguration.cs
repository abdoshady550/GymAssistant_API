using GymAssistant_API.Model.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class TrainerTraineeConfiguration : IEntityTypeConfiguration<TrainerTrainee>
    {
        public void Configure(EntityTypeBuilder<TrainerTrainee> builder)
        {
            builder.ToTable("TrainerTrainees");

            builder.HasKey(tt => tt.Id);

            builder.HasOne(tt => tt.Trainer)
                   .WithMany(c => c.Trainees)
                   .HasForeignKey(tt => tt.TrainerId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(tt => tt.Trainee)
                   .WithMany(c => c.Trainers)
                   .HasForeignKey(tt => tt.TraineeId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
