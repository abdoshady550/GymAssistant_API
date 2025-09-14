using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GymAssistant_API.Data;

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    AppDbContext context,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var adminRole = new IdentityRole("Admin");
        var trainerRole = new IdentityRole(nameof(UserRole.Trainer));
        var userRole = new IdentityRole(nameof(UserRole.User));

        if (_roleManager.Roles.All(r => r.Name != adminRole.Name))
        {
            await _roleManager.CreateAsync(adminRole);
        }

        if (_roleManager.Roles.All(r => r.Name != trainerRole.Name))
        {
            await _roleManager.CreateAsync(trainerRole);
        }

        if (_roleManager.Roles.All(r => r.Name != userRole.Name))
        {
            await _roleManager.CreateAsync(userRole);
        }

        // Default Admin User
        var admin = new AppUser
        {
            Id = "a1234567-1234-1234-1234-123456789012",
            Email = "admin@gymasssistant.com",
            UserName = "admin@gymasssistant.com",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.Email != admin.Email))
        {
            await _userManager.CreateAsync(admin, "Admin@123");
            await _userManager.AddToRolesAsync(admin, [adminRole.Name]);
        }

        // Default Trainers
        var trainer01 = new AppUser
        {
            Id = "t1234567-1234-1234-1234-123456789012",
            Email = "ahmed.trainer@localhost",
            UserName = "ahmed.trainer@localhost",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.Email != trainer01.Email))
        {
            await _userManager.CreateAsync(trainer01, trainer01.Email);
            await _userManager.AddToRolesAsync(trainer01, [trainerRole.Name]);
        }

        var trainer02 = new AppUser
        {
            Id = "t2234567-1234-1234-1234-123456789012",
            Email = "sarah.trainer@localhost",
            UserName = "sarah.trainer@localhost",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.Email != trainer02.Email))
        {
            await _userManager.CreateAsync(trainer02, trainer02.Email);
            await _userManager.AddToRolesAsync(trainer02, [trainerRole.Name]);
        }

        // Default Users
        var user01 = new AppUser
        {
            Id = "u1234567-1234-1234-1234-123456789012",
            Email = "mohamed.client@localhost",
            UserName = "mohamed.client@localhost",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.Email != user01.Email))
        {
            await _userManager.CreateAsync(user01, user01.Email);
            await _userManager.AddToRolesAsync(user01, [userRole.Name]);
        }

        var user02 = new AppUser
        {
            Id = "u2234567-1234-1234-1234-123456789012",
            Email = "fatma.client@localhost",
            UserName = "fatma.client@localhost",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.Email != user02.Email))
        {
            await _userManager.CreateAsync(user02, user02.Email);
            await _userManager.AddToRolesAsync(user02, [userRole.Name]);
        }

        var user03 = new AppUser
        {
            Id = "u3234567-1234-1234-1234-123456789012",
            Email = "ali.client@localhost",
            UserName = "ali.client@localhost",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.Email != user03.Email))
        {
            await _userManager.CreateAsync(user03, user03.Email);
            await _userManager.AddToRolesAsync(user03, [userRole.Name]);
        }

        // Seed Client Profiles if they don't exist
        if (!_context.Set<ClientProfile>().Any())
        {
            var profiles = new List<ClientProfile>
            {
                // Trainers
                ClientProfile.CreateProfile(
                    Guid.Parse("f1234567-1234-1234-1234-123456789012"),
                    "Ahmed", "Hassan", Gender.Male, UserRole.Trainer).Value,

                ClientProfile.CreateProfile(
                    Guid.Parse("f2234567-1234-1234-1234-123456789012"),
                    "Sarah", "Mohamed", Gender.Female, UserRole.Trainer).Value,

                // Users/Clients
                ClientProfile.CreateProfile(
                    Guid.Parse("f3234567-1234-1234-1234-123456789012"),
                    "Mohamed", "Ali", Gender.Male, UserRole.User).Value,

                ClientProfile.CreateProfile(
                    Guid.Parse("f4234567-1234-1234-1234-123456789012"),
                    "Fatma", "Ahmed", Gender.Female, UserRole.User).Value,

                ClientProfile.CreateProfile(
                    Guid.Parse("f5234567-1234-1234-1234-123456789012"),
                    "Ali", "Mahmoud", Gender.Male, UserRole.User).Value
            };

            _context.Set<ClientProfile>().AddRange(profiles);

            // Link profiles to users
            profiles[0].AppUserId = trainer01.Id;
            profiles[1].AppUserId = trainer02.Id;
            profiles[2].AppUserId = user01.Id;
            profiles[3].AppUserId = user02.Id;
            profiles[4].AppUserId = user03.Id;
        }

        // Seed Exercise Sections
        if (!_context.Set<Section>().Any())
        {
            var sections = new List<Section>
            {
                Section.Create(Guid.Parse("s1234567-1234-1234-1234-123456789012"),
                    "Chest", "Chest muscles exercises").Value,
                Section.Create(Guid.Parse("s2234567-1234-1234-1234-123456789012"),
                    "Back", "Back muscles exercises").Value,
                Section.Create(Guid.Parse("s3234567-1234-1234-1234-123456789012"),
                    "Legs", "Leg muscles exercises").Value,
                Section.Create(Guid.Parse("s4234567-1234-1234-1234-123456789012"),
                    "Arms", "Arm muscles exercises").Value,
                Section.Create(Guid.Parse("s5234567-1234-1234-1234-123456789012"),
                    "Shoulders", "Shoulder muscles exercises").Value,
                Section.Create(Guid.Parse("s6234567-1234-1234-1234-123456789012"),
                    "Core", "Core and abdominal exercises").Value,
                Section.Create(Guid.Parse("s7234567-1234-1234-1234-123456789012"),
                    "Cardio", "Cardiovascular exercises").Value
            };

            _context.Set<Section>().AddRange(sections);
        }

        // Seed Exercises
        if (!_context.Set<Exercise>().Any())
        {
            var exercises = new List<Exercise>
            {
                // Chest Exercises
                Exercise.Create(
                    Guid.Parse("e1234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s1234567-1234-1234-1234-123456789012"),
                    "Bench Press",
                    "Classic chest building exercise",
                    "Lie on bench, lower bar to chest, press up",
                    null, "Barbell, Bench", DifficultyLevel.Intermediate, 3, 10).Value,

                Exercise.Create(
                    Guid.Parse("e2234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s1234567-1234-1234-1234-123456789012"),
                    "Push Ups",
                    "Bodyweight chest exercise",
                    "Start in plank, lower chest to ground, push up",
                    null, "Bodyweight", DifficultyLevel.Beginner, 3, 15).Value,

                Exercise.Create(
                    Guid.Parse("e3234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s1234567-1234-1234-1234-123456789012"),
                    "Dumbbell Flyes",
                    "Chest isolation exercise",
                    "Lie on bench, arc dumbbells out and up",
                    null, "Dumbbells, Bench", DifficultyLevel.Intermediate, 3, 12).Value,

                // Back Exercises
                Exercise.Create(
                    Guid.Parse("e4234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s2234567-1234-1234-1234-123456789012"),
                    "Pull Ups",
                    "Upper body compound exercise",
                    "Hang from bar, pull up until chin over bar",
                    null, "Pull-up Bar", DifficultyLevel.Advanced, 3, 8).Value,

                Exercise.Create(
                    Guid.Parse("e5234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s2234567-1234-1234-1234-123456789012"),
                    "Bent Over Rows",
                    "Mid-back building exercise",
                    "Hinge at hips, pull weight to lower ribs",
                    null, "Barbell/Dumbbells", DifficultyLevel.Intermediate, 3, 10).Value,

                Exercise.Create(
                    Guid.Parse("e6234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s2234567-1234-1234-1234-123456789012"),
                    "Lat Pulldowns",
                    "Latissimus dorsi exercise",
                    "Pull bar down to upper chest",
                    null, "Cable Machine", DifficultyLevel.Beginner, 3, 12).Value,

                // Leg Exercises
                Exercise.Create(
                    Guid.Parse("e7234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s3234567-1234-1234-1234-123456789012"),
                    "Squats",
                    "King of all exercises",
                    "Lower hips back and down, drive through heels",
                    null, "Barbell/Bodyweight", DifficultyLevel.Intermediate, 3, 12).Value,

                Exercise.Create(
                    Guid.Parse("e8234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s3234567-1234-1234-1234-123456789012"),
                    "Deadlifts",
                    "Posterior chain exercise",
                    "Hinge at hips, lift weight by extending hips",
                    null, "Barbell", DifficultyLevel.Advanced, 3, 8).Value,

                Exercise.Create(
                    Guid.Parse("e9234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s3234567-1234-1234-1234-123456789012"),
                    "Lunges",
                    "Unilateral leg exercise",
                    "Step forward, lower back knee to ground",
                    null, "Bodyweight/Dumbbells", DifficultyLevel.Beginner, 3, 10).Value,

                // Arm Exercises
                Exercise.Create(
                    Guid.Parse("e10234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s4234567-1234-1234-1234-123456789012"),
                    "Bicep Curls",
                    "Bicep isolation exercise",
                    "Curl weight up, squeeze at top",
                    null, "Dumbbells/Barbell", DifficultyLevel.Beginner, 3, 12).Value,

                Exercise.Create(
                    Guid.Parse("e11234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s4234567-1234-1234-1234-123456789012"),
                    "Tricep Dips",
                    "Tricep compound exercise",
                    "Lower body by bending elbows, push back up",
                    null, "Parallel Bars/Chair", DifficultyLevel.Intermediate, 3, 10).Value,

                // Shoulder Exercises
                Exercise.Create(
                    Guid.Parse("e12234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s5234567-1234-1234-1234-123456789012"),
                    "Overhead Press",
                    "Shoulder compound exercise",
                    "Press weight overhead from shoulder level",
                    null, "Barbell/Dumbbells", DifficultyLevel.Intermediate, 3, 8).Value,

                Exercise.Create(
                    Guid.Parse("e13234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s5234567-1234-1234-1234-123456789012"),
                    "Lateral Raises",
                    "Shoulder isolation exercise",
                    "Raise dumbbells out to sides",
                    null, "Dumbbells", DifficultyLevel.Beginner, 3, 15).Value,

                // Core Exercises
                Exercise.Create(
                    Guid.Parse("e14234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s6234567-1234-1234-1234-123456789012"),
                    "Plank",
                    "Core stability exercise",
                    "Hold straight line position",
                    null, "Bodyweight", DifficultyLevel.Beginner, 3, 30).Value,

                Exercise.Create(
                    Guid.Parse("e15234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s6234567-1234-1234-1234-123456789012"),
                    "Crunches",
                    "Abdominal exercise",
                    "Lift shoulder blades off ground",
                    null, "Bodyweight", DifficultyLevel.Beginner, 3, 20).Value,

                // Cardio Exercises
                Exercise.Create(
                    Guid.Parse("e16234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s7234567-1234-1234-1234-123456789012"),
                    "Treadmill Running",
                    "Cardiovascular exercise",
                    "Run at steady pace",
                    null, "Treadmill", DifficultyLevel.Beginner, 1, 30).Value,

                Exercise.Create(
                    Guid.Parse("e17234567-1234-1234-1234-123456789012"),
                    Guid.Parse("s7234567-1234-1234-1234-123456789012"),
                    "Burpees",
                    "High intensity full body exercise",
                    "Drop down, jump back, push up, jump up",
                    null, "Bodyweight", DifficultyLevel.Advanced, 3, 10).Value
            };

            _context.Set<Exercise>().AddRange(exercises);
        }

        await _context.SaveChangesAsync();

        // Seed Body Measurements
        if (!_context.Set<BodyMeasurement>().Any())
        {
            var clientProfiles = _context.Set<ClientProfile>().Where(p => p.Role == UserRole.User).ToList();
            var measurements = new List<BodyMeasurement>();

            var random = new Random();
            foreach (var profile in clientProfiles)
            {
                // Create initial measurement from 3 months ago
                var initialDate = DateTimeOffset.UtcNow.AddMonths(-3);
                var baseWeight = profile.Gender == Gender.Male
                    ? random.Next(70, 95)
                    : random.Next(55, 75);

                measurements.Add(BodyMeasurement.Create(
                    Guid.NewGuid(),
                    profile.AppUserId,
                    baseWeight,
                    random.Next(15, 25), // Body fat %
                    random.Next(30, 50)  // Muscle mass kg
                ).Value);

                // Create monthly measurements showing progress
                for (int month = 2; month >= 0; month--)
                {
                    var measurementDate = DateTimeOffset.UtcNow.AddMonths(-month);
                    var weightChange = random.Next(-2, 3); // ±2 kg variation

                    measurements.Add(BodyMeasurement.Create(
                        Guid.NewGuid(),
                        profile.AppUserId,
                        baseWeight + weightChange,
                        random.Next(13, 23), // Improving body fat %
                        random.Next(32, 52)  // Increasing muscle mass
                    ).Value);
                }
            }

            _context.Set<BodyMeasurement>().AddRange(measurements);
        }

        // Seed Trainer-Trainee Relationships
        if (!_context.Set<TrainerTrainee>().Any())
        {
            var trainers = _context.Set<ClientProfile>().Where(p => p.Role == UserRole.Trainer).ToList();
            var trainees = _context.Set<ClientProfile>().Where(p => p.Role == UserRole.User).ToList();

            var relationships = new List<TrainerTrainee>
            {
                // Ahmed trainer with Mohamed and Ali
                TrainerTrainee.Create(
                    Guid.NewGuid(),
                    Guid.Parse("f1234567-1234-1234-1234-123456789012"), // Ahmed trainer
                    Guid.Parse("f3234567-1234-1234-1234-123456789012")  // Mohamed client
                ).Value,

                TrainerTrainee.Create(
                    Guid.NewGuid(),
                    Guid.Parse("f1234567-1234-1234-1234-123456789012"), // Ahmed trainer
                    Guid.Parse("f5234567-1234-1234-1234-123456789012")  // Ali client
                ).Value,

                // Sarah trainer with Fatma
                TrainerTrainee.Create(
                    Guid.NewGuid(),
                    Guid.Parse("f2234567-1234-1234-1234-123456789012"), // Sarah trainer
                    Guid.Parse("f4234567-1234-1234-1234-123456789012")  // Fatma client
                ).Value
            };

            _context.Set<TrainerTrainee>().AddRange(relationships);
        }

        // Seed Sample Workout Sessions with realistic data
        if (!_context.Set<WorkoutSession>().Any())
        {
            var clientProfiles = _context.Set<ClientProfile>().Where(p => p.Role == UserRole.User).ToList();
            var exercises = _context.Set<Exercise>().ToList();
            var workoutSessions = new List<WorkoutSession>();
            var workoutExercises = new List<WorkoutExercise>();
            var exerciseSets = new List<ExerciseSet>();
            var random = new Random();

            foreach (var client in clientProfiles)
            {
                // Create workouts for the past 30 days
                for (int day = 30; day >= 0; day -= 2) // Every other day
                {
                    var workoutDate = DateTime.UtcNow.AddDays(-day).Date;
                    var workoutId = Guid.NewGuid();

                    var session = WorkoutSession.Create(
                        workoutId,
                        client.Id,
                        workoutDate,
                        $"Workout session on {workoutDate:yyyy-MM-dd}",
                        day > 0 ? Guid.Parse("f1234567-1234-1234-1234-123456789012") : null // Trainer assignment
                    ).Value;

                    // Add start and end times for completed workouts
                    if (day > 0)
                    {
                        session.StartWorkout(workoutDate.AddHours(random.Next(9, 18)));
                        session.CompleteWorkout(
                            workoutDate.AddHours(random.Next(10, 19)),
                            "Great workout session!"
                        );
                    }

                    workoutSessions.Add(session);

                    // Add 3-5 exercises per workout
                    var selectedExercises = exercises
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(random.Next(3, 6))
                        .ToList();

                    foreach (var exercise in selectedExercises)
                    {
                        var workoutExerciseId = Guid.NewGuid();
                        var workoutExercise = WorkoutExercise.Create(
                            workoutExerciseId,
                            workoutId,
                            exercise.Id
                        ).Value;

                        workoutExercises.Add(workoutExercise);

                        // Add 3-4 sets per exercise
                        var numSets = exercise.DefaultSets ?? random.Next(3, 5);
                        for (int setNum = 1; setNum <= numSets; setNum++)
                        {
                            var reps = exercise.DefaultReps ?? random.Next(8, 15);
                            var weight = exercise.Name.Contains("Bodyweight") || exercise.Name.Contains("Plank")
                                ? 0
                                : random.Next(10, 100);

                            var exerciseSet = ExerciseSet.Create(
                                Guid.NewGuid(),
                                workoutExerciseId,
                                setNum,
                                reps,
                                weight,
                                random.Next(60, 180), // Rest time in seconds
                                setNum == numSets ? "Last set!" : null
                            ).Value;

                            exerciseSets.Add(exerciseSet);
                        }
                    }
                }
            }

            _context.Set<WorkoutSession>().AddRange(workoutSessions);
            await _context.SaveChangesAsync();

            _context.Set<WorkoutExercise>().AddRange(workoutExercises);
            await _context.SaveChangesAsync();

            _context.Set<ExerciseSet>().AddRange(exerciseSets);
        }

        await _context.SaveChangesAsync();
    }
}

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}