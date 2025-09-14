using GymAssistant_API.Model.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymAssistant_API.Data.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AppUsers");

            builder.HasOne(u => u.Profile)
                   .WithOne(p => p.AppUser)
                   .HasForeignKey<ClientProfile>(p => p.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
