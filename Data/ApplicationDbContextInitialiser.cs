using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    // Fixed GUIDs to ensure consistency in relationships
    private static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid TrainerUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ClientUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid Client2UserId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    private static readonly Guid AdminProfileId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid TrainerProfileId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid ClientProfileId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid Client2ProfileId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    private static readonly Guid ChestSectionId = Guid.Parse("10101010-1010-1010-1010-101010101010");
    private static readonly Guid BackSectionId = Guid.Parse("20202020-2020-2020-2020-202020202020");
    private static readonly Guid LegsSectionId = Guid.Parse("30303030-3030-3030-3030-303030303030");
    private static readonly Guid ShouldersSectionId = Guid.Parse("40404040-4040-4040-4040-404040404040");
    private static readonly Guid ArmsSectionId = Guid.Parse("50505050-5050-5050-5050-505050505050");

    private static readonly Guid BenchPressId = Guid.Parse("11111111-2222-3333-4444-555555555555");
    private static readonly Guid DeadliftId = Guid.Parse("22222222-3333-4444-5555-666666666666");
    private static readonly Guid SquatId = Guid.Parse("33333333-4444-5555-6666-777777777777");
    private static readonly Guid PullUpId = Guid.Parse("44444444-5555-6666-7777-888888888888");
    private static readonly Guid ShoulderPressId = Guid.Parse("55555555-6666-7777-8888-999999999999");
    private static readonly Guid BicepCurlId = Guid.Parse("66666666-7777-8888-9999-aaaaaaaaaaaa");

    private static readonly Guid WorkoutSession1Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    private static readonly Guid WorkoutSession2Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    private static readonly Guid WorkoutSession3Id = Guid.Parse("12345678-1234-1234-1234-123456789012");

    private static readonly Guid WorkoutExercise1Id = Guid.Parse("12121212-1212-1212-1212-121212121212");
    private static readonly Guid WorkoutExercise2Id = Guid.Parse("13131313-1313-1313-1313-131313131313");
    private static readonly Guid WorkoutExercise3Id = Guid.Parse("14141414-1414-1414-1414-141414141414");
    private static readonly Guid WorkoutExercise4Id = Guid.Parse("15151515-1515-1515-1515-151515151515");
    private static readonly Guid WorkoutExercise5Id = Guid.Parse("16161616-1616-1616-1616-161616161616");

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
        // Create roles
        await SeedRolesAsync();
        // Create users
        await SeedUsersAsync();
        // Create client profiles
        await SeedClientProfilesAsync();
        // Create sections
        await SeedSectionsAsync();
        // Create exercises
        await SeedExercisesAsync();
        // Create body measurements
        await SeedBodyMeasurementsAsync();
        // Create user exercises
        await SeedUserExercisesAsync();
        // Create workout sessions
        await SeedWorkoutSessionsAsync();
        // Create workout exercises
        await SeedWorkoutExercisesAsync();
        // Create exercise sets
        await SeedExerciseSetsAsync();
        // Create personal records
        await SeedPersonalRecordsAsync();
        // Create trainer-trainee relationships
        await SeedTrainerTraineeRelationshipsAsync();

        await _context.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Admin", "Trainer", "User" };
        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        // Admin user
        var adminUser = await _userManager.FindByEmailAsync("admin@gymassistant.com");
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                Id = AdminUserId.ToString(),
                UserName = "admin@gymassistant.com",
                Email = "admin@gymassistant.com",
                EmailConfirmed = true,
                PhoneNumber = "+201234567890",
                PhoneNumberConfirmed = true
            };
            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                _logger.LogInformation("Created admin user: {Email}", adminUser.Email);
            }
            else
            {
                _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Trainer user
        var trainerUser = await _userManager.FindByEmailAsync("trainer@gymassistant.com");
        if (trainerUser == null)
        {
            trainerUser = new AppUser
            {
                Id = TrainerUserId.ToString(),
                UserName = "trainer@gymassistant.com",
                Email = "trainer@gymassistant.com",
                EmailConfirmed = true,
                PhoneNumber = "+201234567891",
                PhoneNumberConfirmed = true
            };
            var result = await _userManager.CreateAsync(trainerUser, "Trainer123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(trainerUser, "Trainer");
                _logger.LogInformation("Created trainer user: {Email}", trainerUser.Email);
            }
            else
            {
                _logger.LogError("Failed to create trainer user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // First client user
        var clientUser = await _userManager.FindByEmailAsync("client@gymassistant.com");
        if (clientUser == null)
        {
            clientUser = new AppUser
            {
                Id = ClientUserId.ToString(),
                UserName = "client@gymassistant.com",
                Email = "client@gymassistant.com",
                EmailConfirmed = true,
                PhoneNumber = "+201234567892",
                PhoneNumberConfirmed = true
            };
            var result = await _userManager.CreateAsync(clientUser, "Client123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(clientUser, "User");
                _logger.LogInformation("Created client user: {Email}", clientUser.Email);
            }
            else
            {
                _logger.LogError("Failed to create client user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Second client user
        var client2User = await _userManager.FindByEmailAsync("client2@gymassistant.com");
        if (client2User == null)
        {
            client2User = new AppUser
            {
                Id = Client2UserId.ToString(),
                UserName = "client2@gymassistant.com",
                Email = "client2@gymassistant.com",
                EmailConfirmed = true,
                PhoneNumber = "+201234567893",
                PhoneNumberConfirmed = true
            };
            var result = await _userManager.CreateAsync(client2User, "Client2123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(client2User, "User");
                _logger.LogInformation("Created client2 user: {Email}", client2User.Email);
            }
            else
            {
                _logger.LogError("Failed to create client2 user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task SeedClientProfilesAsync()
    {
        if (!await _context.ClientProfiles.AnyAsync())
        {
            var profiles = new List<ClientProfile>();

            // Admin profile
            var adminProfile = ClientProfile.CreateProfile(
                AdminProfileId,
                "John",
                "Administrator",
                Gender.Male,
                UserRole.Trainer
            );
            if (adminProfile.IsSuccess)
            {
                var profile = adminProfile.Value;
                profile.AppUserId = AdminUserId.ToString();
                var updateResult = profile.UpdateProfile("John", "Administrator", new DateTime(1985, 5, 15), 180);
                if (updateResult.IsSuccess)
                {
                    profiles.Add(profile);
                }
            }

            // Trainer profile
            var trainerProfile = ClientProfile.CreateProfile(
                TrainerProfileId,
                "Sarah",
                "Johnson",
                Gender.Female,
                UserRole.Trainer
            );
            if (trainerProfile.IsSuccess)
            {
                var profile = trainerProfile.Value;
                profile.AppUserId = TrainerUserId.ToString();
                var updateResult = profile.UpdateProfile("Sarah", "Johnson", new DateTime(1990, 8, 22), 165);
                if (updateResult.IsSuccess)
                {
                    profiles.Add(profile);
                }
            }

            // First client profile
            var clientProfile = ClientProfile.CreateProfile(
                ClientProfileId,
                "Michael",
                "Smith",
                Gender.Male,
                UserRole.User
            );
            if (clientProfile.IsSuccess)
            {
                var profile = clientProfile.Value;
                profile.AppUserId = ClientUserId.ToString();
                var updateResult = profile.UpdateProfile("Michael", "Smith", new DateTime(1995, 3, 10), 175);
                if (updateResult.IsSuccess)
                {
                    profiles.Add(profile);
                }
            }

            // Second client profile
            var client2Profile = ClientProfile.CreateProfile(
                Client2ProfileId,
                "Emma",
                "Davis",
                Gender.Female,
                UserRole.User
            );
            if (client2Profile.IsSuccess)
            {
                var profile = client2Profile.Value;
                profile.AppUserId = Client2UserId.ToString();
                var updateResult = profile.UpdateProfile("Emma", "Davis", new DateTime(1992, 12, 5), 160);
                if (updateResult.IsSuccess)
                {
                    profiles.Add(profile);
                }
            }

            await _context.ClientProfiles.AddRangeAsync(profiles);
            _logger.LogInformation("Seeded {Count} client profiles", profiles.Count);
        }
    }

    private async Task SeedSectionsAsync()
    {
        if (!await _context.Sections.AnyAsync())
        {
            var sections = new List<Section>();

            var chestSection = Section.Create(ChestSectionId, "Chest", "Chest muscle exercises and upper body training");
            if (chestSection.IsSuccess) sections.Add(chestSection.Value);

            var backSection = Section.Create(BackSectionId, "Back", "Back muscle exercises and spine strengthening");
            if (backSection.IsSuccess) sections.Add(backSection.Value);

            var legsSection = Section.Create(LegsSectionId, "Legs", "Leg muscle exercises and lower body training");
            if (legsSection.IsSuccess) sections.Add(legsSection.Value);

            var shouldersSection = Section.Create(ShouldersSectionId, "Shoulders", "Shoulder muscle exercises and deltoid training");
            if (shouldersSection.IsSuccess) sections.Add(shouldersSection.Value);

            var armsSection = Section.Create(ArmsSectionId, "Arms", "Arm muscle exercises - biceps and triceps");
            if (armsSection.IsSuccess) sections.Add(armsSection.Value);

            await _context.Sections.AddRangeAsync(sections);
            _logger.LogInformation("Seeded {Count} sections", sections.Count);
        }
    }

    private async Task SeedExercisesAsync()
    {
        if (!await _context.Exercises.AnyAsync())
        {
            var exercises = new List<Exercise>();

            // Bench Press exercise
            var benchPress = Exercise.Create(
                BenchPressId,
                ChestSectionId,
                "Bench Press",
                "A fundamental exercise for building chest, shoulder, and arm muscles",
                "Lie on the bench and keep your feet firmly on the ground. Grip the bar with a medium-width grip and lower it slowly until it touches your chest, then push it up forcefully",
                "/images/exercises/bench-press.jpg",
                "Barbell, flat bench, weight plates",
                DifficultyLevel.Intermediate,
                3,
                8
            );
            if (benchPress.IsSuccess) exercises.Add(benchPress.Value);

            // Deadlift exercise
            var deadlift = Exercise.Create(
                DeadliftId,
                BackSectionId,
                "Deadlift",
                "A comprehensive full-body exercise focusing on back and leg muscles",
                "Stand with feet directly under the bar. Keep your back straight and bend at the hips and knees to grip the bar, then lift it slowly while maintaining a straight back",
                "/images/exercises/deadlift.jpg",
                "Barbell, weight plates, lifting platform",
                DifficultyLevel.Advanced,
                3,
                5
            );
            if (deadlift.IsSuccess) exercises.Add(deadlift.Value);

            // Squat exercise
            var squat = Exercise.Create(
                SquatId,
                LegsSectionId,
                "Squat",
                "A fundamental exercise for leg, glute, and core muscles",
                "Place the bar on your upper back and stand with feet shoulder-width apart. Lower slowly as if sitting in a chair until thighs are parallel to the floor, then return to starting position",
                "/images/exercises/squat.jpg",
                "Barbell, squat rack, weight plates",
                DifficultyLevel.Intermediate,
                4,
                10
            );
            if (squat.IsSuccess) exercises.Add(squat.Value);

            // Pull-up exercise
            var pullUp = Exercise.Create(
                PullUpId,
                BackSectionId,
                "Pull-up",
                "An excellent exercise for back and arm muscles",
                "Hang from the bar with a grip wider than shoulder-width. Pull your body up until your chin passes the bar, then lower slowly to the starting position",
                "/images/exercises/pull-up.jpg",
                "Pull-up bar or pull-up station",
                DifficultyLevel.Advanced,
                3,
                6
            );
            if (pullUp.IsSuccess) exercises.Add(pullUp.Value);

            // Shoulder Press exercise
            var shoulderPress = Exercise.Create(
                ShoulderPressId,
                ShouldersSectionId,
                "Shoulder Press",
                "An exercise for strengthening shoulder and arm muscles",
                "Hold dumbbells in each hand at shoulder level. Push the weights up until arms are straight, then lower slowly",
                "/images/exercises/shoulder-press.jpg",
                "Dumbbells or barbell",
                DifficultyLevel.Beginner,
                3,
                10
            );
            if (shoulderPress.IsSuccess) exercises.Add(shoulderPress.Value);

            // Bicep Curl exercise
            var bicepCurl = Exercise.Create(
                BicepCurlId,
                ArmsSectionId,
                "Bicep Curl",
                "An isolation exercise for the bicep muscle",
                "Hold dumbbells in each hand with elbows fixed by your sides. Raise the weights toward your shoulders by bending the elbow, then lower slowly",
                "/images/exercises/bicep-curl.jpg",
                "Dumbbells",
                DifficultyLevel.Beginner,
                3,
                12
            );
            if (bicepCurl.IsSuccess) exercises.Add(bicepCurl.Value);

            await _context.Exercises.AddRangeAsync(exercises);
            _logger.LogInformation("Seeded {Count} exercises", exercises.Count);
        }
    }

    private async Task SeedBodyMeasurementsAsync()
    {
        if (!await _context.BodyMeasurements.AnyAsync())
        {
            var measurements = new List<BodyMeasurement>();

            // Multiple measurements for first client (progress over time)
            var measurement1_1 = BodyMeasurement.Create(
                Guid.NewGuid(),
                ClientUserId.ToString(),
                78.5m,
                18.2m,
                62.0m
            );
            if (measurement1_1.IsSuccess)
            {
                var m = measurement1_1.Value;
                SetPrivateProperty(m, "ClientProfileId", ClientProfileId);
                SetPrivateProperty(m, "CreatedAtUtc", DateTimeOffset.UtcNow.AddDays(-30));
                measurements.Add(m);
            }

            var measurement1_2 = BodyMeasurement.Create(
                Guid.NewGuid(),
                ClientUserId.ToString(),
                76.2m,
                16.8m,
                63.5m
            );
            if (measurement1_2.IsSuccess)
            {
                var m = measurement1_2.Value;
                SetPrivateProperty(m, "ClientProfileId", ClientProfileId);
                SetPrivateProperty(m, "CreatedAtUtc", DateTimeOffset.UtcNow.AddDays(-15));
                measurements.Add(m);
            }

            var measurement1_3 = BodyMeasurement.Create(
                Guid.NewGuid(),
                ClientUserId.ToString(),
                75.5m,
                15.2m,
                65.0m
            );
            if (measurement1_3.IsSuccess)
            {
                var m = measurement1_3.Value;
                SetPrivateProperty(m, "ClientProfileId", ClientProfileId);
                measurements.Add(m);
            }

            // Multiple measurements for second client
            var measurement2_1 = BodyMeasurement.Create(
                Guid.NewGuid(),
                Client2UserId.ToString(),
                62.0m,
                22.5m,
                45.0m
            );
            if (measurement2_1.IsSuccess)
            {
                var m = measurement2_1.Value;
                SetPrivateProperty(m, "ClientProfileId", Client2ProfileId);
                SetPrivateProperty(m, "CreatedAtUtc", DateTimeOffset.UtcNow.AddDays(-20));
                measurements.Add(m);
            }

            var measurement2_2 = BodyMeasurement.Create(
                Guid.NewGuid(),
                Client2UserId.ToString(),
                60.0m,
                18.5m,
                48.0m
            );
            if (measurement2_2.IsSuccess)
            {
                var m = measurement2_2.Value;
                SetPrivateProperty(m, "ClientProfileId", Client2ProfileId);
                measurements.Add(m);
            }

            await _context.BodyMeasurements.AddRangeAsync(measurements);
            _logger.LogInformation("Seeded {Count} body measurements", measurements.Count);
        }
    }

    private async Task SeedUserExercisesAsync()
    {
        if (!await _context.UserExercises.AnyAsync())
        {
            var userExercises = new List<UserExercise>();

            var customExercise1 = UserExercise.Create(
                Guid.NewGuid(),
                ClientUserId.ToString(),
                "Custom Ab Workout",
                "A custom abdominal exercise routine with various movements"
            );
            if (customExercise1.IsSuccess)
            {
                var exercise = customExercise1.Value;
                SetPrivateProperty(exercise, "ClientProfileId", ClientProfileId);
                userExercises.Add(exercise);
            }

            var customExercise2 = UserExercise.Create(
                Guid.NewGuid(),
                Client2UserId.ToString(),
                "Home Cardio Workout",
                "A cardio workout that can be performed at home without equipment"
            );
            if (customExercise2.IsSuccess)
            {
                var exercise = customExercise2.Value;
                SetPrivateProperty(exercise, "ClientProfileId", Client2ProfileId);
                userExercises.Add(exercise);
            }

            var customExercise3 = UserExercise.Create(
                Guid.NewGuid(),
                ClientUserId.ToString(),
                "Morning Stretch Routine",
                "A morning stretching routine to improve body flexibility"
            );
            if (customExercise3.IsSuccess)
            {
                var exercise = customExercise3.Value;
                SetPrivateProperty(exercise, "ClientProfileId", ClientProfileId);
                userExercises.Add(exercise);
            }

            await _context.UserExercises.AddRangeAsync(userExercises);
            _logger.LogInformation("Seeded {Count} user exercises", userExercises.Count);
        }
    }

    private async Task SeedWorkoutSessionsAsync()
    {
        if (!await _context.WorkoutSessions.AnyAsync())
        {
            var workoutSessions = new List<WorkoutSession>();

            // Workout session for first client (last week)
            var session1 = WorkoutSession.Create(
                WorkoutSession1Id,
                ClientProfileId,
                DateTime.UtcNow.AddDays(-7).Date,
                "Excellent workout session - strength focus",
                TrainerProfileId
            );
            if (session1.IsSuccess)
            {
                var session = session1.Value;
                session.StartWorkout(DateTime.UtcNow.AddDays(-7).AddHours(9));
                session.CompleteWorkout(DateTime.UtcNow.AddDays(-7).AddHours(10).AddMinutes(30),
                    "All exercises completed successfully. Noticeable improvement in performance.");
                workoutSessions.Add(session);
            }

            // Workout session for second client (5 days ago)
            var session2 = WorkoutSession.Create(
                WorkoutSession2Id,
                Client2ProfileId,
                DateTime.UtcNow.AddDays(-5).Date,
                "Upper body workout session",
                TrainerProfileId
            );
            if (session2.IsSuccess)
            {
                var session = session2.Value;
                session.StartWorkout(DateTime.UtcNow.AddDays(-5).AddHours(16));
                session.CompleteWorkout(DateTime.UtcNow.AddDays(-5).AddHours(17).AddMinutes(45),
                    "Good session with improved technique");
                workoutSessions.Add(session);
            }

            // Another workout session for first client (3 days ago)
            var session3 = WorkoutSession.Create(
                WorkoutSession3Id,
                ClientProfileId,
                DateTime.UtcNow.AddDays(-3).Date,
                "Legs and lower body session",
                TrainerProfileId
            );
            if (session3.IsSuccess)
            {
                var session = session3.Value;
                session.StartWorkout(DateTime.UtcNow.AddDays(-3).AddHours(10));
                session.CompleteWorkout(DateTime.UtcNow.AddDays(-3).AddHours(11).AddMinutes(15),
                    "Challenging leg session with heavier weights than usual");
                workoutSessions.Add(session);
            }

            await _context.WorkoutSessions.AddRangeAsync(workoutSessions);
            _logger.LogInformation("Seeded {Count} workout sessions", workoutSessions.Count);
        }
    }

    private async Task SeedWorkoutExercisesAsync()
    {
        if (!await _context.WorkoutExercises.AnyAsync())
        {
            var workoutExercises = new List<WorkoutExercise>();

            // Exercises for first session (first client)
            var workoutExercise1 = WorkoutExercise.Create(
                WorkoutExercise1Id,
                WorkoutSession1Id,
                BenchPressId
            );
            if (workoutExercise1.IsSuccess)
            {
                var we = workoutExercise1.Value;
                SetPrivateProperty(we, "ClientProfileId", ClientProfileId);
                workoutExercises.Add(we);
            }

            var workoutExercise2 = WorkoutExercise.Create(
                WorkoutExercise2Id,
                WorkoutSession1Id,
                DeadliftId
            );
            if (workoutExercise2.IsSuccess)
            {
                var we = workoutExercise2.Value;
                SetPrivateProperty(we, "ClientProfileId", ClientProfileId);
                workoutExercises.Add(we);
            }

            // Exercises for second session (second client)
            var workoutExercise3 = WorkoutExercise.Create(
                WorkoutExercise3Id,
                WorkoutSession2Id,
                ShoulderPressId
            );
            if (workoutExercise3.IsSuccess)
            {
                var we = workoutExercise3.Value;
                SetPrivateProperty(we, "ClientProfileId", Client2ProfileId);
                workoutExercises.Add(we);
            }

            var workoutExercise4 = WorkoutExercise.Create(
                WorkoutExercise4Id,
                WorkoutSession2Id,
                BicepCurlId
            );
            if (workoutExercise4.IsSuccess)
            {
                var we = workoutExercise4.Value;
                SetPrivateProperty(we, "ClientProfileId", Client2ProfileId);
                workoutExercises.Add(we);
            }

            // Exercises for third session (first client)
            var workoutExercise5 = WorkoutExercise.Create(
                WorkoutExercise5Id,
                WorkoutSession3Id,
                SquatId
            );
            if (workoutExercise5.IsSuccess)
            {
                var we = workoutExercise5.Value;
                SetPrivateProperty(we, "ClientProfileId", ClientProfileId);
                workoutExercises.Add(we);
            }

            await _context.WorkoutExercises.AddRangeAsync(workoutExercises);
            _logger.LogInformation("Seeded {Count} workout exercises", workoutExercises.Count);
        }
    }

    private async Task SeedExerciseSetsAsync()
    {
        if (!await _context.ExerciseSets.AnyAsync())
        {
            var exerciseSets = new List<ExerciseSet>();

            // Bench press sets (3 sets)
            for (int i = 1; i <= 3; i++)
            {
                var exerciseSet = ExerciseSet.Create(
                    Guid.NewGuid(),
                    WorkoutExercise1Id,
                    i,
                    8,
                    60.0m + (i * 2.5m), // Weight increases with each set
                    90, // 90 seconds rest
                    i == 3 ? "Last set was very challenging" : null
                );
                if (exerciseSet.IsSuccess)
                {
                    var set = exerciseSet.Value;
                    if (i == 3) set.MarkAsPersonalRecord(); // Last set is a personal record
                    exerciseSets.Add(set);
                }
            }

            // Deadlift sets (3 sets)
            for (int i = 1; i <= 3; i++)
            {
                var exerciseSet = ExerciseSet.Create(
                    Guid.NewGuid(),
                    WorkoutExercise2Id,
                    i,
                    5,
                    80.0m + (i * 5.0m), // Larger weight increase
                    120, // Longer rest for deadlift
                    i == 1 ? "Warm-up set" : null
                );
                if (exerciseSet.IsSuccess)
                {
                    exerciseSets.Add(exerciseSet.Value);
                }
            }

            // Shoulder press sets (3 sets)
            for (int i = 1; i <= 3; i++)
            {
                var exerciseSet = ExerciseSet.Create(
                    Guid.NewGuid(),
                    WorkoutExercise3Id,
                    i,
                    10,
                    12.5m + (i * 1.25m), // Lighter dumbbell for beginner
                    60,
                    i == 2 ? "Improved stability" : null
                );
                if (exerciseSet.IsSuccess)
                {
                    exerciseSets.Add(exerciseSet.Value);
                }
            }

            // Bicep curl sets (3 sets)
            for (int i = 1; i <= 3; i++)
            {
                var exerciseSet = ExerciseSet.Create(
                    Guid.NewGuid(),
                    WorkoutExercise4Id,
                    i,
                    12,
                    7.5m + (i * 1.25m),
                    45, // Shorter rest for isolation exercises
                    null
                );
                if (exerciseSet.IsSuccess)
                {
                    exerciseSets.Add(exerciseSet.Value);
                }
            }

            // Squat sets (4 sets)
            for (int i = 1; i <= 4; i++)
            {
                var exerciseSet = ExerciseSet.Create(
                    Guid.NewGuid(),
                    WorkoutExercise5Id,
                    i,
                    10,
                    70.0m + (i * 5.0m),
                    90,
                    i == 4 ? "Extra set for challenge" : null
                );
                if (exerciseSet.IsSuccess)
                {
                    var set = exerciseSet.Value;
                    if (i == 4) set.MarkAsPersonalRecord(); // Extra set is a personal record
                    exerciseSets.Add(set);
                }
            }

            await _context.ExerciseSets.AddRangeAsync(exerciseSets);
            _logger.LogInformation("Seeded {Count} exercise sets", exerciseSets.Count);
        }
    }

    private async Task SeedPersonalRecordsAsync()
    {
        if (!await _context.PersonalRecords.AnyAsync())
        {
            var personalRecords = new List<PersonalRecord>();

            // Personal records for first client
            var record1 = PersonalRecord.Create(
                Guid.NewGuid(),
                ClientProfileId,
                WorkoutSession1Id,
                RecordType.MaxWeight,
                65.0m, // Max weight in bench press
                BenchPressId
            );
            if (record1.IsSuccess)
            {
                personalRecords.Add(record1.Value);
            }

            var record2 = PersonalRecord.Create(
                Guid.NewGuid(),
                ClientProfileId,
                WorkoutSession1Id,
                RecordType.MaxWeight,
                95.0m, // Max weight in deadlift
                DeadliftId
            );
            if (record2.IsSuccess)
            {
                personalRecords.Add(record2.Value);
            }

            var record3 = PersonalRecord.Create(
                Guid.NewGuid(),
                ClientProfileId,
                WorkoutSession3Id,
                RecordType.MaxWeight,
                85.0m, // Max weight in squat
                SquatId
            );
            if (record3.IsSuccess)
            {
                personalRecords.Add(record3.Value);
            }

            // Personal record for second client
            var record4 = PersonalRecord.Create(
                Guid.NewGuid(),
                Client2ProfileId,
                WorkoutSession2Id,
                RecordType.MaxReps,
                15m, // Max reps in shoulder press
                ShoulderPressId
            );
            if (record4.IsSuccess)
            {
                personalRecords.Add(record4.Value);
            }

            var record5 = PersonalRecord.Create(
                Guid.NewGuid(),
                ClientProfileId,
                WorkoutSession1Id,
                RecordType.MaxVolume,
                1560.0m, // Total training volume (weight × reps × sets)
                BenchPressId
            );
            if (record5.IsSuccess)
            {
                personalRecords.Add(record5.Value);
            }

            await _context.PersonalRecords.AddRangeAsync(personalRecords);
            _logger.LogInformation("Seeded {Count} personal records", personalRecords.Count);
        }
    }

    private async Task SeedTrainerTraineeRelationshipsAsync()
    {
        if (!await _context.TrainerTrainees.AnyAsync())
        {
            var relationships = new List<TrainerTrainee>();

            // Trainer relationship with first client
            var relationship1 = TrainerTrainee.Create(
                Guid.NewGuid(),
                TrainerProfileId,
                ClientProfileId
            );
            if (relationship1.IsSuccess)
            {
                relationships.Add(relationship1.Value);
            }

            // Trainer relationship with second client
            var relationship2 = TrainerTrainee.Create(
                Guid.NewGuid(),
                TrainerProfileId,
                Client2ProfileId
            );
            if (relationship2.IsSuccess)
            {
                relationships.Add(relationship2.Value);
            }

            // Admin (as trainer) relationship with first client
            var relationship3 = TrainerTrainee.Create(
                Guid.NewGuid(),
                AdminProfileId,
                ClientProfileId
            );
            if (relationship3.IsSuccess)
            {
                relationships.Add(relationship3.Value);
            }

            await _context.TrainerTrainees.AddRangeAsync(relationships);
            _logger.LogInformation("Seeded {Count} trainer-trainee relationships", relationships.Count);
        }
    }

    // Helper method to set protected property values using Reflection
    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            // If the first method doesn't work, try protected fields
            var field = obj.GetType().GetField($"<{propertyName}>k__BackingField",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
    }
}

// Extension Method outside the main class to solve nested class issue
public static class WebApplicationExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("✅ Database initialization and seeding completed successfully");
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "❌ An error occurred while initializing the database");
            throw;
        }
    }
}