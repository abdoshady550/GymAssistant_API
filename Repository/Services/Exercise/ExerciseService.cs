using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using ExerciseEntity = GymAssistant_API.Model.Entities.Exercise.Exercise;

using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Req_Res.Response;
using GymAssistant_API.Req_Res.Response.Exercise;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Repository.Services.Exercises
{
    public class ExerciseService(AppDbContext context, IWebHostEnvironment environment) : IExercise
    {
        private readonly AppDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;

        public async Task<Result<CustomExerciseRes>> CreateCustomExerciseAsync(string userId,
                                                                          string name,
                                                                          string? description = null,
                                                                          string? Instructions = null,
                                                                          string? Equipment = null,
                                                                          IFormFile? imageFile = default,
                                                                         CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            // 🖼️ حفظ الصورة في wwwroot (لو موجودة)
            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "custom-exercises");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream, ct);
                }

                // 🔗 هنا حطينا الدومين يدوي
                const string baseUrl = "https://gymassistantapi.runasp.net";
                imageUrl = $"{baseUrl}/images/custom-exercises/{uniqueFileName}";
            }

            var exerciseResult = UserExercise.Create(Guid.NewGuid(),
                                                     userId,
                                                     name,
                                                     description,
                                                     Instructions,
                                                     Equipment,
                                                     imageUrl);

            if (exerciseResult.IsError)
            {
                return exerciseResult.Errors;
            }

            var exercise = exerciseResult.Value;

            exercise.ClientProfileId = profile.Id;

            profile.AddCustomExercise(exercise);

            _context.UserExercises.Add(exercise);

            await _context.SaveChangesAsync(ct);
            var dto = new CustomExerciseRes(
                exercise.Id,
                exercise.UserId,
                exercise.Name,
                exercise.Description,
                exercise.Instructions,
                exercise.Equipment,
                exercise.ImageUrl,
                exercise.CreatedAtUtc
    );
            return dto;
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
                                                                     IFormFile? imageFile = default,
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
            // 🖼️ حفظ الصورة في wwwroot (لو موجودة)
            string? imageUrl = exercise.ImageUrl;
            if (imageFile != null && imageFile.Length > 0)
            {
                // 🗑️ امسح الصورة القديمة من wwwroot لو موجودة
                if (!string.IsNullOrEmpty(exercise.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, exercise.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "custom-exercises");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream, ct);
                }

                const string baseUrl = "https://gymassistantapi.runasp.net";
                imageUrl = $"{baseUrl}/images/custom-exercises/{uniqueFileName}";
            }

            exercise.Update(name, description, Instructions, Equipment, imageUrl);

            _context.UserExercises.Update(exercise);
            await _context.SaveChangesAsync(ct);

            return Result.Updated;
        }

        public async Task<Result<CustomExerciseRes>> GetCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _context.UserExercises
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId, ct);

            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Custom exercise not found.");
            }
            var dto = new CustomExerciseRes(
                exercise.Id,
                exercise.UserId,
                exercise.Name,
                exercise.Description,
                exercise.Instructions,
                exercise.Equipment,
                exercise.ImageUrl,
                exercise.CreatedAtUtc);
            return dto;
        }

        public async Task<Result<List<CustomExerciseRes>>> GetCustomExercisesAsync(string userId, CancellationToken ct = default)
        {
            return await _context.UserExercises
              .Where(e => e.UserId == userId)
              .OrderBy(e => e.Name)
              .Select(e => new CustomExerciseRes(
            e.Id,
            e.UserId,
            e.Name,
            e.Description,
            e.Instructions,
            e.Equipment,
            e.ImageUrl,
            e.CreatedAtUtc
        ))
              .ToListAsync(ct);
        }

        public async Task<Result<ExerciseResponse>> CreateExerciseAsync(Guid sectionId, string name, string? description = null,
                                            string? instructions = null, IFormFile? imageFile = null,
                                            string? equipment = null, DifficultyLevel? difficultyLevel = null,
                                            int? defaultSets = null, int? defaultReps = null, CancellationToken ct = default)
        {
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.Id == sectionId);
            if (section == null)
            {
                return Error.NotFound("Section_NotFound", "Section not found.");
            }
            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "exercises");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream, ct);
                }

                // 🔗 هنا حطينا الدومين يدوي
                const string baseUrl = "https://gymassistantapi.runasp.net";
                imageUrl = $"{baseUrl}/images/custom-exercises/{uniqueFileName}";
            }
            var exerciseResult = ExerciseEntity.Create(Guid.NewGuid(),
                                                 sectionId,
                                                 name,
                                                 description,
                                                 instructions,
                                                 imageUrl,
                                                 equipment,
                                                 difficultyLevel,
                                                 defaultSets,
                                                 defaultReps);
            if (exerciseResult.IsError)
            {
                return exerciseResult.Errors;
            }
            var exercise = exerciseResult.Value;
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync(ct);
            var dto = new ExerciseResponse(
                exercise.Id,
                exercise.SectionId,
                exercise.Name,
                exercise.Description,
                exercise.Instructions,
                exercise.Equipment,
                exercise.ImageUrl,
                exercise.DifficultyLevel,
                exercise.DefaultSets,
                exercise.DefaultReps,
                exercise.CreatedAtUtc
            );
            return dto;

        }
        public async Task<Result<Updated>> UpdateExerciseAsync(Guid id, Guid sectionId, string name, string? description,
                                           string? instructions, IFormFile? imageFile,
                                           string? equipment, DifficultyLevel? difficultyLevel,
                                           int? defaultSets, int? defaultReps, CancellationToken ct = default)
        {
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.Id == sectionId);
            if (section == null)
            {
                return Error.NotFound("Section_NotFound", "Section not found.");
            }
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Exercise not found.");
            }
            string? imageUrl = exercise.ImageUrl;
            if (imageFile != null && imageFile.Length > 0)
            {
                // 🗑️ امسح الصورة القديمة من wwwroot لو موجودة
                if (!string.IsNullOrEmpty(exercise.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, exercise.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "exercises");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream, ct);
                }

                const string baseUrl = "https://gymassistantapi.runasp.net";
                imageUrl = $"{baseUrl}/images/custom-exercises/{uniqueFileName}";
            }
            var exerciseResult = exercise.Update(
                                                 sectionId,
                                                 name,
                                                 description,
                                                 instructions,
                                                 imageUrl,
                                                 equipment,
                                                 difficultyLevel,
                                                 defaultSets,
                                                 defaultReps);
            if (exerciseResult.IsError)
            {
                return exerciseResult.Errors;
            }
            _context.Exercises.Update(exercise);
            await _context.SaveChangesAsync(ct);

            return Result.Updated;

        }
        public async Task<Result<Deleted>> DeleteExerciseAsync(Guid id, CancellationToken ct = default)
        {
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id, ct);
            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Exercise not found.");
            }
            // Check if exercise is used in any workouts
            var isUsedInWorkouts = await _context.WorkoutExercises
                .AnyAsync(we => we.ExerciseId == id, ct);
            if (isUsedInWorkouts)
            {
                return Error.Validation("Exercise_InUse", "Cannot delete exercise that has been used in workouts.");
            }
            // 🗑️ امسح الصورة من wwwroot لو موجودة
            if (!string.IsNullOrEmpty(exercise.ImageUrl))
            {
                var oldImagePath = Path.Combine(_environment.WebRootPath, exercise.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync(ct);
            return Result.Deleted;
        }
        public async Task<Result<ExerciseResponse>> GetExerciseAsync(Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _context.Exercises
                .Include(e => e.Section)
                .FirstOrDefaultAsync(e => e.Id == exerciseId, ct);

            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Exercise not found.");
            }
            var dto = new ExerciseResponse(
                exercise.Id,
                exercise.SectionId,
                exercise.Name,
                exercise.Description,
                exercise.Instructions,
                exercise.Equipment,
                exercise.ImageUrl,
                exercise.DifficultyLevel
            );
            return dto;
        }
        public async Task<Result<List<ExerciseResponse>>> GetExercisesBySectionAsync(Guid sectionId, DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var query = _context.Exercises
                .Where(e => e.SectionId == sectionId);

            if (difficulty.HasValue)
            {
                query = query.Where(e => e.DifficultyLevel == difficulty);
            }

            return await query
                .OrderBy(e => e.Name)
                .Select(e => new ExerciseResponse(
                 e.Id,
                 e.SectionId,
                 e.Name,
                 e.Description,
                 e.Instructions,
                 e.Equipment,
                 e.ImageUrl,
                 e.DifficultyLevel,
                 e.DefaultSets,
                 e.DefaultReps,
                 e.CreatedAtUtc
                ))
                .ToListAsync(ct);
        }

        public async Task<Result<List<SectionResponse>>> GetSectionsAsync(CancellationToken ct = default)
        {
            return await _context.Sections
                .Include(s => s.Exercises)
                .OrderBy(s => s.Name)
                .Select(s => new SectionResponse(
                    s.Id,
                    s.Name,
                    s.Description,
                    s.CreatedAtUtc
                ))
                .ToListAsync(ct);
        }
    }
}
