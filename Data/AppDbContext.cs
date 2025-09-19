using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<ClientProfile> ClientProfiles => Set<ClientProfile>();
        public DbSet<BodyMeasurement> BodyMeasurements => Set<BodyMeasurement>();
        public DbSet<TrainerTrainee> TrainerTrainees => Set<TrainerTrainee>();
        public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
        public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
        public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<UserExercise> UserExercises => Set<UserExercise>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<PersonalRecord> PersonalRecords => Set<PersonalRecord>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Add the seeding

        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(10, 2);
        }
    }
}