using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExerciseEntity = GymAssistant_API.Model.Entities.Exercise.Exercise;

namespace GymAssistant_API.Data.Configurations.Exercise
{
    public class ExerciseConfiguration : IEntityTypeConfiguration<ExerciseEntity>
    {
        public void Configure(EntityTypeBuilder<ExerciseEntity> builder)
        {
            builder.ToTable("Exercises");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.NameEn).HasMaxLength(200);
            builder.Property(e => e.NameAr).HasMaxLength(200);
            builder.Property(e => e.DescriptionEn).HasMaxLength(2000);
            builder.Property(e => e.DescriptionAr).HasMaxLength(2000);
            builder.Property(e => e.InstructionsEn).HasMaxLength(4000);
            builder.Property(e => e.InstructionsAr).HasMaxLength(4000);
            builder.Property(e => e.EquipmentEn).HasMaxLength(500);
            builder.Property(e => e.EquipmentAr).HasMaxLength(500);
            builder.Property(e => e.ImageUrl).HasMaxLength(500);

            builder.HasOne(e => e.Section)
                   .WithMany(s => s.Exercises)
                   .HasForeignKey(e => e.SectionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.SectionGroup)
                   .WithMany(s => s.Exercises)
                   .HasForeignKey(e => e.SectionGroupId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
