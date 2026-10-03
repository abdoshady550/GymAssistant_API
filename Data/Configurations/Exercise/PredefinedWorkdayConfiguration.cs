using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations.Exercise
{
    public class PredefinedWorkdayConfiguration : IEntityTypeConfiguration<PredefinedWorkday>
    {
        public void Configure(EntityTypeBuilder<PredefinedWorkday> builder)
        {
            builder.ToTable("PredefinedWorkdays");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.NameEn).IsRequired().HasMaxLength(200);
            builder.Property(w => w.NameAr).HasMaxLength(200);
            builder.Property(w => w.DescriptionEn).HasMaxLength(1000);
            builder.Property(w => w.DescriptionAr).HasMaxLength(1000);
            builder.Property(w => w.ImageUrl).HasMaxLength(500);

            builder.HasMany(w => w.Sessions)
                   .WithOne(s => s.PredefinedWorkday)
                   .HasForeignKey(s => s.PredefinedWorkdayId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
