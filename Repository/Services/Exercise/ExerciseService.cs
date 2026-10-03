using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Req_Res.Response;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Resources;
using Microsoft.EntityFrameworkCore;
using ExerciseEntity = GymAssistant_API.Model.Entities.Exercise.Exercise;

namespace GymAssistant_API.Repository.Services.Exercises
{
    public class ExerciseService(AppDbContext context, IWebHostEnvironment environment, ISeedDataLocalizer seedLocalizer)
        : IExercise
    {
        private readonly AppDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly ISeedDataLocalizer _seedLocalizer = seedLocalizer;

        public async Task<Result<CustomExerciseRes>> CreateCustomExerciseAsync(string userId,
                                                                               Guid sectionId,
                                                                               string name,
                                                                               string? description = null,
                                                                               string? Instructions = null,
                                                                               string? Equipment = null,
                                                                               IFormFile? ImageFile = default,
                                                                               DifficultyLevel? DifficultyLevel = null,
                                                                               CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return UserErrors.ProfileNotFound;
            }
            var section = await _context.Sections
              .FirstOrDefaultAsync(s => s.Id == sectionId);
            if (section == null)
            {
                return ExerciseErrors.SectionNotFound;
            }

            // 🖼️ حفظ الصورة في wwwroot (لو موجودة)
            string? imageUrl = null;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "custom-exercises");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream, ct);
                }

                // 🔗 هنا حطينا الدومين يدوي
                const string baseUrl = "https://gymassistantapi.runasp.net";
                imageUrl = $"/images/custom-exercises/{uniqueFileName}";
            }

            var exerciseResult = UserExercise.Create(Guid.NewGuid(),
                                                     userId,
                                                     sectionId,
                                                     name,
                                                     description,
                                                     Instructions,
                                                     Equipment,
                                                     imageUrl, DifficultyLevel);

            if (exerciseResult.IsError)
            {
                return exerciseResult.Errors;
            }

            var exercise = exerciseResult.Value;

            exercise.ClientProfileId = profile.Id;

            profile.AddCustomExercise(exercise);

            section.AddUserExercise(exercise);

            _context.UserExercises.Add(exercise);

            await _context.SaveChangesAsync(ct);
            var dto = new CustomExerciseRes(
        exercise.Id,
        exercise.UserId,
        exercise.SectionId,
        section.Name,
        exercise.Name,
        exercise.Description,
        exercise.Instructions,
        exercise.Equipment,
        exercise.ImageUrl,
        exercise.IsCustomExercise,
        exercise.CreatedAtUtc,
        exercise.DifficultyLevel
    );
            return dto;
        }

        public async Task<Result<Deleted>> DeleteCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _context.UserExercises
                 .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId, ct);

            if (exercise == null)
            {
                return ExerciseErrors.CustomExerciseNotFound;
            }

            // Check if exercise is used in any workouts
            var isUsedInWorkouts = await _context.WorkoutExercises
                 .AnyAsync(we => we.UserExerciseId == exerciseId, ct);

            if (isUsedInWorkouts)
            {
                return ExerciseErrors.InUse;
            }
            // 🗑️ احذف الصورة من wwwroot لو موجودة
            if (!string.IsNullOrEmpty(exercise.ImageUrl))
            {
                var oldImagePath = Path.Combine(_environment.WebRootPath, exercise.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            _context.UserExercises.Remove(exercise);
            await _context.SaveChangesAsync(ct);

            return Result.Deleted;
        }
        public async Task<Result<Updated>> UpdateCustomExerciseAsync(string userId,
                                                                     Guid exerciseId,
                                                                     Guid sectionId,
                                                                     string name,
                                                                     string? description = null,
                                                                     string? Instructions = null,
                                                                     string? Equipment = null,
                                                                     IFormFile? imageFile = default,
                                                                     DifficultyLevel? DifficultyLevel = null,
                                                                     CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ExerciseErrors.NameRequired;
            }
            var exercise = await _context.UserExercises
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId, ct);

            if (exercise == null)
            {
                return ExerciseErrors.CustomExerciseNotFound;
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
                imageUrl = $"/images/custom-exercises/{uniqueFileName}";
            }

            exercise.Update(sectionId, name, description, Instructions, Equipment, imageUrl, DifficultyLevel);

            _context.UserExercises.Update(exercise);
            await _context.SaveChangesAsync(ct);

            return Result.Updated;
        }

        public async Task<Result<CustomExerciseRes>> GetCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _context.UserExercises.Include(e => e.Section)
                .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId, ct);

            if (exercise == null)
            {
                return ExerciseErrors.CustomExerciseNotFound;
            }
            var dto = CustomExerciseRes.FromEntity(exercise);
            return dto;
        }

        public async Task<Result<List<CustomExerciseRes>>> GetCustomExercisesAsync(string userId, DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var query = _context.UserExercises.Include(u => u.Section)
                      .Where(e => e.UserId == userId);


            if (difficulty.HasValue)
            {
                query = query.Where(e => e.DifficultyLevel == difficulty);
            }


            var result = query
                .OrderBy(e => e.Name)
                .Select(CustomExerciseRes.FromEntity).ToList();

            return result;
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
                return ExerciseErrors.SectionNotFound;
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
                imageUrl = $"/images/exercises/{uniqueFileName}";
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
            section.AddExercise(exercise);
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync(ct);

            var dto = new ExerciseResponse(
                exercise.Id,
                exercise.SectionId,
                section.Name,
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
                return ExerciseErrors.SectionNotFound;
            }
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exercise == null)
            {
                return ExerciseErrors.NotFound;
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
                imageUrl = $"/images/exercises/{uniqueFileName}";
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
                return ExerciseErrors.NotFound;
            }
            // Check if exercise is used in any workouts
            var isUsedInWorkouts = await _context.WorkoutExercises
                .AnyAsync(we => we.ExerciseId == id, ct);
            if (isUsedInWorkouts)
            {
                return ExerciseErrors.InUse;
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
                return ExerciseErrors.NotFound;
            }
            var dto = ExerciseResponse.FromEntity(exercise, _seedLocalizer);
            return dto;
        }
        public async Task<Result<ExercisesResponse>> GetExercisesBySectionAsync(string userId,
                                                                                Guid sectionId,
                                                                                string? searchTerm = null,
                                                                                DifficultyLevel? difficulty = null,
                                                                                CancellationToken ct = default)
        {
            var query = _context.Exercises.Include(e => e.Section)
        .Where(e => e.SectionId == sectionId);

            var querycustom = _context.UserExercises.Include(e => e.Section)
                .Where(e => e.SectionId == sectionId && e.UserId == userId);



            if (difficulty.HasValue)
            {
                query = query.Where(e => e.DifficultyLevel == difficulty);
                querycustom = querycustom.Where(e => e.DifficultyLevel == difficulty);
            }
            // تطبيق البحث
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(e =>
                    e.Name.ToLower().Contains(search) ||
                    e.Description.ToLower().Contains(search));

                querycustom = querycustom.Where(e =>
                    e.Name.ToLower().Contains(search) ||
                    e.Description.ToLower().Contains(search));
            }

            var exercises = query
                .OrderBy(e => e.Name)
                .Select(e => ExerciseResponse.FromEntity(e, _seedLocalizer))
                .ToList();

            var customExercises = querycustom
                .OrderBy(e => e.Name)
                .Select(CustomExerciseRes.FromEntity).ToList();

            return new ExercisesResponse(exercises, customExercises);
        }

        public async Task<Result<List<SectionResponse>>> GetSectionsAsync(string userId, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);
            if (profile == null)
            {
                return UserErrors.ProfileNotFound;
            }
            var sections = await _context.Sections
                 .Include(s => s.Exercises)
                 .Include(s => s.UserExercise)
                 .Include(s => s.SectionGroup)
                    .ThenInclude(s => s.Exercises)
                .Include(s => s.SectionGroup)
                     .ThenInclude(sg => sg.UserExercise)
                 .OrderBy(s => s.Name)
                 .ToListAsync(ct);
            return sections
                .Select(s => SectionResponse.FromEntity(userId, s, _seedLocalizer)).ToList();
        }
        public async Task<Result<SectionResponse>> GetSectionByIdAsync(string userId, Guid sectionId, CancellationToken ct = default)
        {
            var section = await _context.Sections.FindAsync(sectionId, ct);
            if (section == null)
            {
                return ExerciseErrors.SectionNotFound;
            }
            var result = SectionResponse.FromEntity(userId, section, _seedLocalizer);

            return result;
        }
        public async Task<Result<SectionGroupResponse>> CreateSectionGroup(string userId,
                                                                        Guid sectionId,
                                                                        string name,
                                                                        string descripion,
                                                                        CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                  .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);
            if (profile == null)
            {
                return UserErrors.ProfileNotFound;
            }
            var section = await _context.Sections.FindAsync(sectionId, ct);
            if (section == null)
            {
                return ExerciseErrors.SectionNotFound;
            }

            var createGroup = SectionGroup.Create(Guid.NewGuid(), sectionId, name, descripion);

            var group = createGroup.Value;

            group.ClientProfileId = profile.Id;

            profile.AddSectionGroup(group);
            section.AddSectionGroup(group);
            await _context.SectionGroups.AddAsync(group, ct);
            await _context.SaveChangesAsync(ct);
            var dto = SectionGroupResponse.FromEntity(group, _seedLocalizer);
            return dto;
        }
        public async Task<Result<List<SectionGroupResponse>>> AllSectionGroups(string userId, Guid sectionId, CancellationToken ct = default)
        {
            var grops = await _context.SectionGroups
                     .Include(s => s.Exercises)
                     .Include(s => s.UserExercise)
                     .OrderBy(s => s.Name).Where(s => s.SectionId == sectionId).ToListAsync(ct);
            return grops
                     .Select(sg => SectionGroupResponse.FromEntity(sg, _seedLocalizer)).ToList();
        }
        public async Task<Result<SectionGroupResponse>> AddExerciseToGroup(string userId,
                                                                           Guid groupId,
                                                                           Guid? exerciseId,
                                                                           Guid? customExerciseId,
                                                                           CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
              .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);
            if (profile == null)
            {
                return UserErrors.ProfileNotFound;
            }
            var group = await _context.SectionGroups
                .Include(s => s.Exercises)
                .Include(s => s.UserExercise)
                .FirstOrDefaultAsync(s => s.Id == groupId, ct);
            if (group == null)
            {
                return ExerciseErrors.SectionGroupNotFound;
            }

            ExerciseEntity? exercise = null;

            if (exerciseId.HasValue)
            {
                exercise = await _context.Exercises.FindAsync(exerciseId.Value, ct);
                if (exercise == null)
                {
                    return ExerciseErrors.NotFound;
                }
                if (group.Exercises.Any(e => e.Id == exercise.Id))
                {
                    return ExerciseErrors.AlreadyInGroup;
                }
                group.AddExercise(exercise);
            }

            UserExercise? customExercise = null;
            if (customExerciseId.HasValue)
            {
                customExercise = await _context.UserExercises.FindAsync(customExerciseId.Value, ct);
                if (customExercise == null)
                {
                    return ExerciseErrors.CustomExerciseNotFound;
                }
                if (group.UserExercise.Any(e => e.Id == customExercise.Id))
                {
                    return ExerciseErrors.AlreadyInGroup;
                }
                group.AddUserExercise(customExercise);

            }

            if (exercise == null && customExercise == null)
            {
                return ExerciseErrors.ExerciseIdRequired;
            }


            await _context.SaveChangesAsync(ct);

            var dto = SectionGroupResponse.FromEntity(group, _seedLocalizer);

            return dto;
        }
        public async Task<Result<Updated>> UpdateGroup(string userId,
                                                                        Guid groupId,
                                                                        string name,
                                                                        string descripion,
                                                                        CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
              .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);
            if (profile == null)
            {
                return UserErrors.ProfileNotFound;
            }
            var group = await _context.SectionGroups.FindAsync(groupId, ct);
            if (group == null)
            {
                return ExerciseErrors.SectionGroupNotFound;
            }

            group.Update(name, descripion);
            await _context.SaveChangesAsync(ct);

            return Result.Updated;
        }
        public async Task<Result<Deleted>> DeleteGroup(string userId, Guid groupId, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
              .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);
            if (profile == null)
            {
                return UserErrors.ProfileNotFound;
            }
            var group = await _context.SectionGroups.FindAsync(groupId, ct);
            if (group == null)
            {
                return ExerciseErrors.SectionGroupNotFound;
            }
            _context.SectionGroups.Remove(group);
            await _context.SaveChangesAsync(ct);
            return Result.Deleted;
        }
        public async Task<Result<Deleted>> DeleteExerciseFromGroup(string userId, Guid groupId, Guid? exerciseId, Guid? customExerciseId, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
            .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);
            if (profile == null)
            {
                return UserErrors.ProfileNotFound;
            }
            var group = await _context.SectionGroups.FindAsync(groupId, ct);
            if (group == null)
            {
                return ExerciseErrors.SectionGroupNotFound;
            }
            if (exerciseId == null && customExerciseId == null)
            {
                return ExerciseErrors.ExerciseIdRequired;
            }
            if (exerciseId.HasValue)
            {
                var exercise = await _context.Exercises.FindAsync(exerciseId.Value, ct);
                if (exercise == null)
                {
                    return ExerciseErrors.NotFound;
                }
                group.RemoveExercise(exercise);
            }
            if (customExerciseId.HasValue)
            {
                var customExercise = await _context.UserExercises.FindAsync(customExerciseId.Value, ct);
                if (customExercise == null)
                {
                    return ExerciseErrors.CustomExerciseNotFound;
                }
                group.RemoveUserExercise(customExercise);
            }
            return Result.Deleted;
        }
    }
}
