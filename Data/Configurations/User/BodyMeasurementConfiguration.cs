using GymAssistant_API.Model.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class BodyMeasurementConfiguration : IEntityTypeConfiguration<BodyMeasurement>
    {
        public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
        {
            builder.ToTable("BodyMeasurements");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.WeightKg).IsRequired();
        }
    }
}
