using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.Exercise
{
    public class PredefinedWorkoutSessionConfiguration : IEntityTypeConfiguration<PredefinedWorkoutSession>
    {
        public void Configure(EntityTypeBuilder<PredefinedWorkoutSession> builder)
        {
            builder.ToTable("PredefinedWorkoutSessions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.NameEn).IsRequired().HasMaxLength(200);
            builder.Property(s => s.NameAr).HasMaxLength(200);
            builder.Property(s => s.DescriptionEn).HasMaxLength(2000);
            builder.Property(s => s.DescriptionAr).HasMaxLength(2000);
            builder.Property(s => s.ImageUrl).HasMaxLength(500);

            builder.HasMany(s => s.Exercises)
                   .WithOne(e => e.PredefinedWorkoutSession)
                   .HasForeignKey(e => e.PredefinedWorkoutSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
