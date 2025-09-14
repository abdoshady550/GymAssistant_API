using GymAssistant_API.Model.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class ClientProfileConfiguration : IEntityTypeConfiguration<ClientProfile>
    {
        public void Configure(EntityTypeBuilder<ClientProfile> builder)
        {
            builder.ToTable("ClientProfiles");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);

            builder.HasMany(c => c.Measurements)
                   .WithOne(m => m.User)
                   .HasForeignKey(m => m.ClientProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Workouts)
                   .WithOne(w => w.Trainee)
                   .HasForeignKey(w => w.ClientProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.CustomExercises)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.ClientProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.PersonalRecords)
                   .WithOne(r => r.User)
                   .HasForeignKey(r => r.ClientProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Trainer/Trainee many-to-many (self-relationship)
            builder.HasMany(c => c.Trainees)
                   .WithOne(t => t.Trainer)
                   .HasForeignKey(t => t.TrainerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Trainers)
                   .WithOne(t => t.Trainee)
                   .HasForeignKey(t => t.TraineeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
