using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class UserExerciseConfiguration : IEntityTypeConfiguration<UserExercise>
    {
        public void Configure(EntityTypeBuilder<UserExercise> builder)
        {
            builder.ToTable("UserExercises");

            builder.HasKey(ue => ue.Id);

            builder.Property(ue => ue.Name).IsRequired().HasMaxLength(200);

            builder.HasOne(ue => ue.User)
                   .WithMany(c => c.CustomExercises)
                   .HasForeignKey(ue => ue.ClientProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
