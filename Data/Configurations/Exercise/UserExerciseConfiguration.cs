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

            builder.HasOne(e => e.Section)
             .WithMany(s => s.UserExercise)
             .HasForeignKey(e => e.SectionId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SectionGroup)
           .WithMany(s => s.UserExercise)
           .HasForeignKey(e => e.SectionGroupId)
           .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
