using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Req_Res.Response;
using GymAssistant_API.Req_Res.Response.Exercise;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;
using ExerciseEntity = GymAssistant_API.Model.Entities.Exercise.Exercise;

namespace GymAssistant_API.Repository.Services.Exercises
{
    public class ExerciseService(AppDbContext context, IWebHostEnvironment environment)
        : IExercise
    {
        private readonly AppDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;

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
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var section = await _context.Sections
              .FirstOrDefaultAsync(s => s.Id == sectionId);
            if (section == null)
            {
                return Error.NotFound("Section_NotFound", "Section not found.");
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
                return Error.NotFound("Exercise_NotFound", "Custom exercise not found.");
            }

            // Check if exercise is used in any workouts
            var isUsedInWorkouts = await _context.WorkoutExercises
                .AnyAsync(we => we.UserExerciseId == exerciseId, ct);

            if (isUsedInWorkouts)
            {
                return Error.Validation("Exercise_InUse", "Cannot delete exercise that has been used in workouts.");
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
                imageUrl = $"/images/custom-exercises/{uniqueFileName}";
            }

            exercise.Update(sectionId, name, description, Instructions, Equipment, imageUrl, DifficultyLevel);

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
                exercise.SectionId,
                exercise.Name,
                exercise.Description,
                exercise.Instructions,
                exercise.Equipment,
                exercise.ImageUrl,
                exercise.IsCustomExercise,
                exercise.CreatedAtUtc,
                exercise.DifficultyLevel);
            return dto;
        }

        public async Task<Result<List<CustomExerciseRes>>> GetCustomExercisesAsync(string userId, DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var query = _context.UserExercises
                      .Where(e => e.UserId == userId);


            if (difficulty.HasValue)
            {
                query = query.Where(e => e.DifficultyLevel == difficulty);
            }


            var result = await query
                .OrderBy(e => e.Name)
                .Select(e => new CustomExerciseRes(
                    e.Id,
                    e.UserId,
                    e.SectionId,
                    e.Name,
                    e.Description,
                    e.Instructions,
                    e.Equipment,
                    e.ImageUrl,
                    e.IsCustomExercise,
                    e.CreatedAtUtc,
                    e.DifficultyLevel
                ))
                .ToListAsync(ct);
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
        public async Task<Result<ExercisesResponse>> GetExercisesBySectionAsync(string userId, Guid sectionId, string? searchTerm = null, DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var query = _context.Exercises
        .Where(e => e.SectionId == sectionId);

            var querycustom = _context.UserExercises
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

            var exercises = await query
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
                    e.CreatedAtUtc,
                    e.IsCustomExercise
                ))
                .ToListAsync(ct);

            var customExercises = await querycustom
                .OrderBy(e => e.Name)
                .Select(e => new CustomExerciseRes(
                    e.Id,
                    e.UserId,
                    e.SectionId,
                    e.Name,
                    e.Description,
                    e.Instructions,
                    e.Equipment,
                    e.ImageUrl,
                    e.IsCustomExercise,
                    e.CreatedAtUtc,
                    e.DifficultyLevel
                ))
                .ToListAsync(ct);

            return new ExercisesResponse(exercises, customExercises);
        }

        public async Task<Result<List<SectionResponse>>> GetSectionsAsync(CancellationToken ct = default)
        {
            var sections = await _context.Sections
                     .Include(s => s.Exercises)
                     .Include(s => s.UserExercise)
                     .Include(s => s.SectionGroup)
                        .ThenInclude(s => s.Exercises)
                     .Include(s => s.SectionGroup)
                        .ThenInclude(s => s.UserExercise)
                     .OrderBy(s => s.Name)
                     .ToListAsync(ct);
            return sections
                .Select(SectionResponse.FromEntity).ToList();
        }
        public async Task<Result<SectionResponse>> GetSectionByIdAsync(string userId, Guid sectionId, CancellationToken ct = default)
        {
            var section = await _context.Sections.FindAsync(sectionId, ct);
            if (section == null)
            {
                return Error.NotFound("Section_Not_Found", $"this section with{sectionId} not found");
            }
            var result = SectionResponse.FromEntity(section);

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
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var section = await _context.Sections.FindAsync(sectionId, ct);
            if (section == null)
            {
                return Error.NotFound("Section_Not_Found", $"this section with{sectionId} not found");
            }

            var createGroup = SectionGroup.Create(Guid.NewGuid(), sectionId, name, descripion);

            var group = createGroup.Value;

            group.ClientProfileId = profile.Id;

            profile.AddSectionGroup(group);
            section.AddSectionGroup(group);
            await _context.SectionGroups.AddAsync(group, ct);
            await _context.SaveChangesAsync(ct);
            var dto = SectionGroupResponse.FromEntity(group);
            return dto;
        }
        public async Task<Result<List<SectionGroupResponse>>> AllSectionGroups(string userId, Guid sectionId, CancellationToken ct = default)
        {
            var grops = await _context.SectionGroups
                     .Include(s => s.Exercises)
                     .Include(s => s.UserExercise)
                     .OrderBy(s => s.Name).Where(s => s.SectionId == sectionId).ToListAsync(ct);
            return grops
                     .Select(SectionGroupResponse.FromEntity).ToList();
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
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var group = await _context.SectionGroups.FindAsync(groupId, ct);
            if (group == null)
            {
                return Error.NotFound("Group_NotFound", "User Group not found.");
            }



            ExerciseEntity? exercise = null;

            if (exerciseId.HasValue)
            {
                exercise = await _context.Exercises.FindAsync(exerciseId.Value, ct);
                if (exercise == null)
                {
                    return Error.NotFound("Exercise_NotFound", $"Exercise with ID {exerciseId.Value} not found.");
                }
                group.AddExercise(exercise);
            }

            UserExercise? customExercise = null;
            if (customExerciseId.HasValue)
            {
                customExercise = await _context.UserExercises.FindAsync(customExerciseId.Value, ct);
                if (customExercise == null)
                {
                    return Error.NotFound("Exercise_NotFound", $"Exercise with ID {customExerciseId.Value} not found.");
                }
                group.AddUserExercise(customExercise);

            }

            if (exercise == null && customExercise == null)
            {
                return Error.Validation("No_exercise_Found", "You should add exercise");
            }


            await _context.SaveChangesAsync(ct);

            var dto = SectionGroupResponse.FromEntity(group);

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
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var group = await _context.SectionGroups.FindAsync(groupId, ct);
            if (group == null)
            {
                return Error.NotFound("Group_NotFound", "User Group not found.");
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
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var group = await _context.SectionGroups.FindAsync(groupId, ct);
            if (group == null)
            {
                return Error.NotFound("Group_NotFound", "User Group not found.");
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
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var group = await _context.SectionGroups.FindAsync(groupId, ct);
            if (group == null)
            {
                return Error.NotFound("Group_NotFound", "User Group not found.");
            }
            if (exerciseId == null && customExerciseId == null)
            {
                return Error.Validation("Exercise_Not_Found", "You should add exercise");
            }
            if (exerciseId.HasValue)
            {
                var exercise = await _context.Exercises.FindAsync(exerciseId.Value, ct);
                if (exercise == null)
                {
                    return Error.NotFound("Exercise_Not_Found", $"this Exercise with{exerciseId} not found");
                }
                group.RemoveExercise(exercise);
            }
            if (customExerciseId.HasValue)
            {
                var customExercise = await _context.UserExercises.FindAsync(customExerciseId.Value, ct);
                if (customExercise == null)
                {
                    return Error.NotFound("CustomExercise_Not_Found", $"this Custom Exercise with{customExerciseId} not found");
                }
                group.RemoveUserExercise(customExercise);
            }
            return Result.Deleted;
        }
    }
}
