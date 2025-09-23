using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercise;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Repository.Services.Exercises
{
    public class ExerciseService(AppDbContext context) : IExercise
    {
        private readonly AppDbContext _context = context;

        public async Task<Result<UserExercise>> CreateCustomExerciseAsync(string userId, string name, string? description = null, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var exerciseResult = UserExercise.Create(Guid.NewGuid(), userId, name, description);

            if (exerciseResult.IsError)
            {
                return exerciseResult.Errors;
            }

            var exercise = exerciseResult.Value;

            exercise.ClientProfileId = profile.Id;

            profile.AddCustomExercise(exercise);

            _context.UserExercises.Add(exercise);

            await _context.SaveChangesAsync(ct);
            return exercise;
        }

        public async Task<Result<Deleted>> DeleteCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _context.UserExercises
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId, ct);

            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Custom exercise not found.");
            }

            // Check if exercise is used in any workouts
            var isUsedInWorkouts = await _context.WorkoutExercises
                .AnyAsync(we => we.UserExerciseId == exerciseId, ct);

            if (isUsedInWorkouts)
            {
                return Error.Validation("Exercise_InUse", "Cannot delete exercise that has been used in workouts.");
            }

            _context.UserExercises.Remove(exercise);
            await _context.SaveChangesAsync(ct);

            return Result.Deleted;
        }
        public async Task<Result<Updated>> UpdateCustomExerciseAsync(string userId,
                                                                     Guid exerciseId,
                                                                     string name,
                                                                     string? description = null,
                                                                     string? Instructions = null,
                                                                     string? Equipment = null,
                                                                     string? ImageUrl = null,
                                                                     CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Error.Validation("Exercise_NameRequired", "Name is required.");
            }
            var exercise = await _context.UserExercises
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId, ct);

            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Custom exercise not found.");
            }

            exercise.Update(name, description, Instructions, Equipment, ImageUrl);

            _context.UserExercises.Update(exercise);
            await _context.SaveChangesAsync(ct);

            return Result.Updated;
        }

        public async Task<Result<UserExercise>> GetCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _context.UserExercises
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId, ct);

            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Custom exercise not found.");
            }

            return exercise;
        }

        public async Task<Result<List<UserExercise>>> GetCustomExercisesAsync(string userId, CancellationToken ct = default)
        {
            return await _context.UserExercises
              .Where(e => e.UserId == userId)
              .OrderBy(e => e.Name)
              .ToListAsync(ct);
        }

        public async Task<Result<Exercise>> GetExerciseAsync(Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _context.Exercises
                .Include(e => e.Section)
                .FirstOrDefaultAsync(e => e.Id == exerciseId, ct);

            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Exercise not found.");
            }

            return exercise;
        }

        public async Task<Result<List<Exercise>>> GetExercisesBySectionAsync(Guid sectionId, DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var query = _context.Exercises
                .Where(e => e.SectionId == sectionId);

            if (difficulty.HasValue)
            {
                query = query.Where(e => e.DifficultyLevel == difficulty);
            }

            return await query
                .OrderBy(e => e.Name)
                .ToListAsync(ct);
        }

        public async Task<Result<List<Section>>> GetSectionsAsync(CancellationToken ct = default)
        {
            return await _context.Sections
                .Include(s => s.Exercises)
                .OrderBy(s => s.Name)
                .ToListAsync(ct);
        }


    }
}
