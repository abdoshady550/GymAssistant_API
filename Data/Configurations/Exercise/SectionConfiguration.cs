using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.ToTable("Sections");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.NameEn).HasMaxLength(200);
            builder.Property(s => s.NameAr).HasMaxLength(200);
            builder.Property(s => s.DescriptionEn).HasMaxLength(1000);
            builder.Property(s => s.DescriptionAr).HasMaxLength(1000);
            builder.Property(s => s.ImageUrl).HasMaxLength(500);

            builder.HasMany(e => e.SectionGroup)
                    .WithOne(s => s.Section)
                    .HasForeignKey(e => e.SectionId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
