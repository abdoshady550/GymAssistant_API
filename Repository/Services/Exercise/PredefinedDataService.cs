using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Reqeust.Predefined;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Predefined;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using ExerciseEntity = GymAssistant_API.Model.Entities.Exercise.Exercise;

namespace GymAssistant_API.Repository.Services.Exercise
{
    public class PredefinedDataService(AppDbContext context, IWebHostEnvironment environment) : IPredefinedDataService
    {
        private readonly AppDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;

        private static string LocalizeString(string? en, string? ar)
        {
            var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("ar", StringComparison.OrdinalIgnoreCase);
            if (isArabic && !string.IsNullOrWhiteSpace(ar))
            {
                return ar;
            }
            return en ?? ar ?? string.Empty;
        }

        private async Task<string?> SaveImageAsync(IFormFile? file, string folderName, CancellationToken ct)
        {
            if (file == null || file.Length == 0) return null;

            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "images", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            return $"/images/{folderName}/{uniqueFileName}";
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            try
            {
                var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(webRoot, relativePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // Ignore file deletion errors to prevent failing main operation
            }
        }

        // =========================================================================
        // SECTIONS
        // =========================================================================

        public async Task<Result<AdminSectionRes>> CreateSectionAsync(AdminSectionReq req, CancellationToken ct = default)
        {
            string? imageUrl = await SaveImageAsync(req.ImageFile, "sections", ct);

            var sectionResult = Section.Create(
                Guid.NewGuid(),
                req.NameEn,
                req.NameAr,
                req.DescriptionEn,
                req.DescriptionAr,
                imageUrl
            );

            if (sectionResult.IsError)
            {
                DeleteOldImage(imageUrl);
                return sectionResult.Errors;
            }

            var section = sectionResult.Value;
            _context.Sections.Add(section);
            await _context.SaveChangesAsync(ct);

            return new AdminSectionRes(
                section.Id,
                section.NameEn ?? section.Name,
                section.NameAr,
                section.DescriptionEn ?? section.Description,
                section.DescriptionAr,
                section.ImageUrl,
                0,
                section.CreatedAtUtc
            );
        }

        public async Task<Result<Updated>> UpdateSectionAsync(Guid id, AdminSectionReq req, CancellationToken ct = default)
        {
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == id, ct);
            if (section == null)
            {
                return ExerciseErrors.SectionNotFound;
            }

            string? newImageUrl = section.ImageUrl;
            if (req.ImageFile != null && req.ImageFile.Length > 0)
            {
                DeleteOldImage(section.ImageUrl);
                newImageUrl = await SaveImageAsync(req.ImageFile, "sections", ct);
            }

            var updateResult = section.Update(
                req.NameEn,
                req.NameAr,
                req.DescriptionEn,
                req.DescriptionAr,
                newImageUrl
            );

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            _context.Sections.Update(section);
            await _context.SaveChangesAsync(ct);
            return Result.Updated;
        }

        public async Task<Result<Deleted>> DeleteSectionAsync(Guid id, CancellationToken ct = default)
        {
            var section = await _context.Sections
                .Include(s => s.Exercises)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

            if (section == null)
            {
                return ExerciseErrors.SectionNotFound;
            }

            if (section.Exercises.Any())
            {
                return ExerciseErrors.InUse;
            }

            DeleteOldImage(section.ImageUrl);
            _context.Sections.Remove(section);
            await _context.SaveChangesAsync(ct);

            return Result.Deleted;
        }

        public async Task<Result<List<AdminSectionRes>>> GetAdminSectionsAsync(CancellationToken ct = default)
        {
            var sections = await _context.Sections
                .Include(s => s.Exercises)
                .OrderBy(s => s.NameEn ?? s.Name)
                .ToListAsync(ct);

            var res = sections.Select(s => new AdminSectionRes(
                s.Id,
                s.NameEn ?? s.Name,
                s.NameAr,
                s.DescriptionEn ?? s.Description,
                s.DescriptionAr,
                s.ImageUrl,
                s.Exercises.Count,
                s.CreatedAtUtc
            )).ToList();

            return res;
        }

        public async Task<Result<List<PredefinedSectionDto>>> GetClientSectionsAsync(CancellationToken ct = default)
        {
            var sections = await _context.Sections
                .Include(s => s.Exercises)
                .OrderBy(s => s.NameEn ?? s.Name)
                .ToListAsync(ct);

            var res = sections.Select(s => new PredefinedSectionDto(
                s.Id,
                LocalizeString(s.NameEn ?? s.Name, s.NameAr),
                LocalizeString(s.DescriptionEn ?? s.Description, s.DescriptionAr),
                s.ImageUrl,
                s.NameEn ?? s.Name,
                s.NameAr,
                s.Exercises.Count
            )).ToList();

            return res;
        }

        public async Task<Result<PredefinedSectionDto>> GetClientSectionByIdAsync(Guid id, CancellationToken ct = default)
        {
            var section = await _context.Sections
                .Include(s => s.Exercises)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

            if (section == null)
            {
                return ExerciseErrors.SectionNotFound;
            }

            return new PredefinedSectionDto(
                section.Id,
                LocalizeString(section.NameEn ?? section.Name, section.NameAr),
                LocalizeString(section.DescriptionEn ?? section.Description, section.DescriptionAr),
                section.ImageUrl,
                section.NameEn ?? section.Name,
                section.NameAr,
                section.Exercises.Count
            );
        }

        // =========================================================================
        // EXERCISES
        // =========================================================================

        public async Task<Result<AdminExerciseRes>> CreateExerciseAsync(AdminExerciseReq req, CancellationToken ct = default)
        {
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == req.SectionId, ct);
            if (section == null)
            {
                return ExerciseErrors.SectionNotFound;
            }

            string? imageUrl = await SaveImageAsync(req.ImageFile, "exercises", ct);

            var exerciseResult = ExerciseEntity.Create(
                Guid.NewGuid(),
                req.SectionId,
                req.NameEn,
                req.NameAr,
                req.DescriptionEn,
                req.DescriptionAr,
                req.InstructionsEn,
                req.InstructionsAr,
                imageUrl,
                req.EquipmentEn,
                req.EquipmentAr,
                req.DifficultyLevel,
                req.DefaultSets,
                req.DefaultReps
            );

            if (exerciseResult.IsError)
            {
                DeleteOldImage(imageUrl);
                return exerciseResult.Errors;
            }

            var exercise = exerciseResult.Value;
            section.AddExercise(exercise);
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync(ct);

            return new AdminExerciseRes(
                exercise.Id,
                exercise.SectionId,
                section.NameEn ?? section.Name,
                section.NameAr,
                exercise.NameEn ?? exercise.Name,
                exercise.NameAr,
                exercise.DescriptionEn ?? exercise.Description,
                exercise.DescriptionAr,
                exercise.InstructionsEn ?? exercise.Instructions,
                exercise.InstructionsAr,
                exercise.EquipmentEn ?? exercise.Equipment,
                exercise.EquipmentAr,
                exercise.ImageUrl,
                exercise.DifficultyLevel,
                exercise.DefaultSets,
                exercise.DefaultReps,
                exercise.CreatedAtUtc
            );
        }

        public async Task<Result<Updated>> UpdateExerciseAsync(Guid id, AdminExerciseReq req, CancellationToken ct = default)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (exercise == null)
            {
                return ExerciseErrors.NotFound;
            }

            if (req.SectionId != Guid.Empty && req.SectionId != exercise.SectionId)
            {
                var sectionExists = await _context.Sections.AnyAsync(s => s.Id == req.SectionId, ct);
                if (!sectionExists)
                {
                    return ExerciseErrors.SectionNotFound;
                }
            }

            string? newImageUrl = exercise.ImageUrl;
            if (req.ImageFile != null && req.ImageFile.Length > 0)
            {
                DeleteOldImage(exercise.ImageUrl);
                newImageUrl = await SaveImageAsync(req.ImageFile, "exercises", ct);
            }

            var updateResult = exercise.Update(
                req.SectionId,
                req.NameEn,
                req.NameAr,
                req.DescriptionEn,
                req.DescriptionAr,
                req.InstructionsEn,
                req.InstructionsAr,
                newImageUrl,
                req.EquipmentEn,
                req.EquipmentAr,
                req.DifficultyLevel,
                req.DefaultSets,
                req.DefaultReps
            );

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            _context.Exercises.Update(exercise);
            await _context.SaveChangesAsync(ct);
            return Result.Updated;
        }

        public async Task<Result<Deleted>> DeleteExerciseAsync(Guid id, CancellationToken ct = default)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (exercise == null)
            {
                return ExerciseErrors.NotFound;
            }

            var isUsedInWorkouts = await _context.WorkoutExercises.AnyAsync(we => we.ExerciseId == id, ct);
            var isUsedInTemplates = await _context.PredefinedSessionExercises.AnyAsync(pe => pe.ExerciseId == id, ct);

            if (isUsedInWorkouts || isUsedInTemplates)
            {
                return ExerciseErrors.InUse;
            }

            DeleteOldImage(exercise.ImageUrl);
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync(ct);

            return Result.Deleted;
        }

        public async Task<Result<List<AdminExerciseRes>>> GetAdminExercisesAsync(Guid? sectionId = null, CancellationToken ct = default)
        {
            var query = _context.Exercises.Include(e => e.Section).AsQueryable();
            if (sectionId.HasValue && sectionId.Value != Guid.Empty)
            {
                query = query.Where(e => e.SectionId == sectionId.Value);
            }

            var exercises = await query.OrderBy(e => e.NameEn ?? e.Name).ToListAsync(ct);

            var res = exercises.Select(e => new AdminExerciseRes(
                e.Id,
                e.SectionId,
                e.Section?.NameEn ?? e.Section?.Name,
                e.Section?.NameAr,
                e.NameEn ?? e.Name,
                e.NameAr,
                e.DescriptionEn ?? e.Description,
                e.DescriptionAr,
                e.InstructionsEn ?? e.Instructions,
                e.InstructionsAr,
                e.EquipmentEn ?? e.Equipment,
                e.EquipmentAr,
                e.ImageUrl,
                e.DifficultyLevel,
                e.DefaultSets,
                e.DefaultReps,
                e.CreatedAtUtc
            )).ToList();

            return res;
        }

        public async Task<Result<List<PredefinedExerciseDto>>> GetClientExercisesAsync(Guid? sectionId = null, DifficultyLevel? difficulty = null, string? searchTerm = null, CancellationToken ct = default)
        {
            var query = _context.Exercises.Include(e => e.Section).Where(e => !e.IsCustomExercise).AsQueryable();

            if (sectionId.HasValue && sectionId.Value != Guid.Empty)
            {
                query = query.Where(e => e.SectionId == sectionId.Value);
            }

            if (difficulty.HasValue)
            {
                query = query.Where(e => e.DifficultyLevel == difficulty.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(e =>
                    (e.NameEn != null && e.NameEn.ToLower().Contains(term)) ||
                    (e.NameAr != null && e.NameAr.ToLower().Contains(term)) ||
                    e.Name.ToLower().Contains(term) ||
                    (e.DescriptionEn != null && e.DescriptionEn.ToLower().Contains(term)) ||
                    (e.DescriptionAr != null && e.DescriptionAr.ToLower().Contains(term)));
            }

            var exercises = await query.OrderBy(e => e.NameEn ?? e.Name).ToListAsync(ct);

            var res = exercises.Select(e => new PredefinedExerciseDto(
                e.Id,
                e.SectionId,
                LocalizeString(e.Section?.NameEn ?? e.Section?.Name, e.Section?.NameAr),
                LocalizeString(e.NameEn ?? e.Name, e.NameAr),
                LocalizeString(e.DescriptionEn ?? e.Description, e.DescriptionAr),
                LocalizeString(e.InstructionsEn ?? e.Instructions, e.InstructionsAr),
                LocalizeString(e.EquipmentEn ?? e.Equipment, e.EquipmentAr),
                e.ImageUrl,
                e.DifficultyLevel,
                e.DefaultSets,
                e.DefaultReps,
                e.NameEn ?? e.Name,
                e.NameAr
            )).ToList();

            return res;
        }

        public async Task<Result<PredefinedExerciseDto>> GetClientExerciseByIdAsync(Guid id, CancellationToken ct = default)
        {
            var e = await _context.Exercises.Include(ex => ex.Section).FirstOrDefaultAsync(ex => ex.Id == id, ct);
            if (e == null)
            {
                return ExerciseErrors.NotFound;
            }

            return new PredefinedExerciseDto(
                e.Id,
                e.SectionId,
                LocalizeString(e.Section?.NameEn ?? e.Section?.Name, e.Section?.NameAr),
                LocalizeString(e.NameEn ?? e.Name, e.NameAr),
                LocalizeString(e.DescriptionEn ?? e.Description, e.DescriptionAr),
                LocalizeString(e.InstructionsEn ?? e.Instructions, e.InstructionsAr),
                LocalizeString(e.EquipmentEn ?? e.Equipment, e.EquipmentAr),
                e.ImageUrl,
                e.DifficultyLevel,
                e.DefaultSets,
                e.DefaultReps,
                e.NameEn ?? e.Name,
                e.NameAr
            );
        }

        // =========================================================================
        // WORKDAYS
        // =========================================================================

        public async Task<Result<AdminWorkdayRes>> CreateWorkdayAsync(AdminWorkdayReq req, CancellationToken ct = default)
        {
            string? imageUrl = await SaveImageAsync(req.ImageFile, "workdays", ct);

            var workdayResult = PredefinedWorkday.Create(
                Guid.NewGuid(),
                req.NameEn,
                req.NameAr,
                req.DescriptionEn,
                req.DescriptionAr,
                imageUrl,
                req.DayNumber,
                req.IsActive ?? true
            );

            if (workdayResult.IsError)
            {
                DeleteOldImage(imageUrl);
                return workdayResult.Errors;
            }

            var workday = workdayResult.Value;
            _context.PredefinedWorkdays.Add(workday);
            await _context.SaveChangesAsync(ct);

            return new AdminWorkdayRes(
                workday.Id,
                workday.NameEn,
                workday.NameAr,
                workday.DescriptionEn,
                workday.DescriptionAr,
                workday.ImageUrl,
                workday.DayNumber,
                workday.IsActive,
                0,
                workday.CreatedAtUtc
            );
        }

        public async Task<Result<Updated>> UpdateWorkdayAsync(Guid id, AdminWorkdayReq req, CancellationToken ct = default)
        {
            var workday = await _context.PredefinedWorkdays.FirstOrDefaultAsync(w => w.Id == id, ct);
            if (workday == null)
            {
                return ExerciseErrors.WorkdayNotFound;
            }

            string? newImageUrl = workday.ImageUrl;
            if (req.ImageFile != null && req.ImageFile.Length > 0)
            {
                DeleteOldImage(workday.ImageUrl);
                newImageUrl = await SaveImageAsync(req.ImageFile, "workdays", ct);
            }

            var updateResult = workday.Update(
                req.NameEn,
                req.NameAr,
                req.DescriptionEn,
                req.DescriptionAr,
                newImageUrl,
                req.DayNumber,
                req.IsActive
            );

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            _context.PredefinedWorkdays.Update(workday);
            await _context.SaveChangesAsync(ct);
            return Result.Updated;
        }

        public async Task<Result<Deleted>> DeleteWorkdayAsync(Guid id, CancellationToken ct = default)
        {
            var workday = await _context.PredefinedWorkdays
                .Include(w => w.Sessions)
                .FirstOrDefaultAsync(w => w.Id == id, ct);

            if (workday == null)
            {
                return ExerciseErrors.WorkdayNotFound;
            }

            DeleteOldImage(workday.ImageUrl);
            _context.PredefinedWorkdays.Remove(workday);
            await _context.SaveChangesAsync(ct);

            return Result.Deleted;
        }

        public async Task<Result<List<AdminWorkdayRes>>> GetAdminWorkdaysAsync(CancellationToken ct = default)
        {
            var workdays = await _context.PredefinedWorkdays
                .Include(w => w.Sessions)
                .OrderBy(w => w.DayNumber ?? 999)
                .ThenBy(w => w.NameEn)
                .ToListAsync(ct);

            var res = workdays.Select(w => new AdminWorkdayRes(
                w.Id,
                w.NameEn,
                w.NameAr,
                w.DescriptionEn,
                w.DescriptionAr,
                w.ImageUrl,
                w.DayNumber,
                w.IsActive,
                w.Sessions.Count,
                w.CreatedAtUtc
            )).ToList();

            return res;
        }

        public async Task<Result<List<PredefinedWorkdayDto>>> GetClientWorkdaysAsync(CancellationToken ct = default)
        {
            var workdays = await _context.PredefinedWorkdays
                .Where(w => w.IsActive)
                .Include(w => w.Sessions.Where(s => s.IsActive))
                    .ThenInclude(s => s.Exercises)
                        .ThenInclude(e => e.Exercise)
                .OrderBy(w => w.DayNumber ?? 999)
                .ThenBy(w => w.NameEn)
                .ToListAsync(ct);

            var res = workdays.Select(w => new PredefinedWorkdayDto(
                w.Id,
                LocalizeString(w.NameEn, w.NameAr),
                LocalizeString(w.DescriptionEn, w.DescriptionAr),
                w.ImageUrl,
                w.DayNumber,
                w.NameEn,
                w.NameAr,
                w.Sessions.Select(s => MapToSessionDto(s)).ToList()
            )).ToList();

            return res;
        }

        public async Task<Result<PredefinedWorkdayDto>> GetClientWorkdayByIdAsync(Guid id, CancellationToken ct = default)
        {
            var w = await _context.PredefinedWorkdays
                .Include(w => w.Sessions.Where(s => s.IsActive))
                    .ThenInclude(s => s.Exercises)
                        .ThenInclude(e => e.Exercise)
                .FirstOrDefaultAsync(w => w.Id == id, ct);

            if (w == null)
            {
                return ExerciseErrors.WorkdayNotFound;
            }

            return new PredefinedWorkdayDto(
                w.Id,
                LocalizeString(w.NameEn, w.NameAr),
                LocalizeString(w.DescriptionEn, w.DescriptionAr),
                w.ImageUrl,
                w.DayNumber,
                w.NameEn,
                w.NameAr,
                w.Sessions.Select(s => MapToSessionDto(s)).ToList()
            );
        }

        // =========================================================================
        // PREDEFINED SESSIONS
        // =========================================================================

        public async Task<Result<AdminPredefinedSessionRes>> CreatePredefinedSessionAsync(AdminPredefinedSessionReq req, CancellationToken ct = default)
        {
            if (req.WorkdayId.HasValue && req.WorkdayId.Value != Guid.Empty)
            {
                var workdayExists = await _context.PredefinedWorkdays.AnyAsync(w => w.Id == req.WorkdayId.Value, ct);
                if (!workdayExists)
                {
                    return ExerciseErrors.WorkdayNotFound;
                }
            }

            string? imageUrl = await SaveImageAsync(req.ImageFile, "sessions", ct);

            var sessionResult = PredefinedWorkoutSession.Create(
                Guid.NewGuid(),
                req.WorkdayId,
                req.NameEn,
                req.NameAr,
                req.DescriptionEn,
                req.DescriptionAr,
                imageUrl,
                req.DifficultyLevel,
                req.EstimatedDurationMinutes,
                req.IsActive ?? true
            );

            if (sessionResult.IsError)
            {
                DeleteOldImage(imageUrl);
                return sessionResult.Errors;
            }

            var session = sessionResult.Value;

            // Parse exercises if provided
            List<SessionExerciseItemReq> items = [];
            if (!string.IsNullOrWhiteSpace(req.ExercisesJson))
            {
                try
                {
                    items = JsonSerializer.Deserialize<List<SessionExerciseItemReq>>(
                        req.ExercisesJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ) ?? [];
                }
                catch
                {
                    // If JSON fails, proceed with empty or log
                }
            }

            foreach (var item in items)
            {
                var exerciseExists = await _context.Exercises.AnyAsync(e => e.Id == item.ExerciseId, ct);
                if (exerciseExists)
                {
                    var exerciseEntity = PredefinedSessionExercise.Create(
                        Guid.NewGuid(),
                        session.Id,
                        item.ExerciseId,
                        item.Order,
                        item.DefaultSets,
                        item.DefaultReps,
                        item.DefaultRestTimeSeconds
                    );
                    if (exerciseEntity.IsSuccess)
                    {
                        session.AddExercise(exerciseEntity.Value);
                    }
                }
            }

            _context.PredefinedWorkoutSessions.Add(session);
            await _context.SaveChangesAsync(ct);

            return await GetAdminSessionResByIdAsync(session.Id, ct);
        }

        public async Task<Result<Updated>> UpdatePredefinedSessionAsync(Guid id, AdminPredefinedSessionReq req, CancellationToken ct = default)
        {
            var session = await _context.PredefinedWorkoutSessions
                .Include(s => s.Exercises)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

            if (session == null)
            {
                return ExerciseErrors.PredefinedSessionNotFound;
            }

            if (req.WorkdayId.HasValue && req.WorkdayId.Value != Guid.Empty)
            {
                var workdayExists = await _context.PredefinedWorkdays.AnyAsync(w => w.Id == req.WorkdayId.Value, ct);
                if (!workdayExists)
                {
                    return ExerciseErrors.WorkdayNotFound;
                }
            }

            string? newImageUrl = session.ImageUrl;
            if (req.ImageFile != null && req.ImageFile.Length > 0)
            {
                DeleteOldImage(session.ImageUrl);
                newImageUrl = await SaveImageAsync(req.ImageFile, "sessions", ct);
            }

            var updateResult = session.Update(
                req.WorkdayId,
                req.NameEn,
                req.NameAr,
                req.DescriptionEn,
                req.DescriptionAr,
                newImageUrl,
                req.DifficultyLevel,
                req.EstimatedDurationMinutes,
                req.IsActive
            );

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            // If exercises are provided, update them
            if (!string.IsNullOrWhiteSpace(req.ExercisesJson))
            {
                try
                {
                    var items = JsonSerializer.Deserialize<List<SessionExerciseItemReq>>(
                        req.ExercisesJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (items != null)
                    {
                        // Remove existing exercises
                        _context.PredefinedSessionExercises.RemoveRange(session.Exercises);
                        session.ClearExercises();

                        foreach (var item in items)
                        {
                            var exerciseExists = await _context.Exercises.AnyAsync(e => e.Id == item.ExerciseId, ct);
                            if (exerciseExists)
                            {
                                var exerciseEntity = PredefinedSessionExercise.Create(
                                    Guid.NewGuid(),
                                    session.Id,
                                    item.ExerciseId,
                                    item.Order,
                                    item.DefaultSets,
                                    item.DefaultReps,
                                    item.DefaultRestTimeSeconds
                                );
                                if (exerciseEntity.IsSuccess)
                                {
                                    session.AddExercise(exerciseEntity.Value);
                                    _context.PredefinedSessionExercises.Add(exerciseEntity.Value);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore parse error or keep existing exercises
                }
            }

            _context.PredefinedWorkoutSessions.Update(session);
            await _context.SaveChangesAsync(ct);
            return Result.Updated;
        }

        public async Task<Result<Deleted>> DeletePredefinedSessionAsync(Guid id, CancellationToken ct = default)
        {
            var session = await _context.PredefinedWorkoutSessions
                .Include(s => s.Exercises)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

            if (session == null)
            {
                return ExerciseErrors.PredefinedSessionNotFound;
            }

            DeleteOldImage(session.ImageUrl);
            _context.PredefinedWorkoutSessions.Remove(session);
            await _context.SaveChangesAsync(ct);

            return Result.Deleted;
        }

        public async Task<Result<List<AdminPredefinedSessionRes>>> GetAdminPredefinedSessionsAsync(Guid? workdayId = null, CancellationToken ct = default)
        {
            var query = _context.PredefinedWorkoutSessions
                .Include(s => s.PredefinedWorkday)
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Exercise)
                .AsQueryable();

            if (workdayId.HasValue && workdayId.Value != Guid.Empty)
            {
                query = query.Where(s => s.PredefinedWorkdayId == workdayId.Value);
            }

            var sessions = await query.OrderBy(s => s.NameEn).ToListAsync(ct);

            var res = sessions.Select(s => MapToAdminSessionRes(s)).ToList();
            return res;
        }

        public async Task<Result<List<PredefinedWorkoutSessionDto>>> GetClientPredefinedSessionsAsync(Guid? workdayId = null, DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var query = _context.PredefinedWorkoutSessions
                .Where(s => s.IsActive)
                .Include(s => s.PredefinedWorkday)
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Exercise)
                .AsQueryable();

            if (workdayId.HasValue && workdayId.Value != Guid.Empty)
            {
                query = query.Where(s => s.PredefinedWorkdayId == workdayId.Value);
            }

            if (difficulty.HasValue)
            {
                query = query.Where(s => s.DifficultyLevel == difficulty.Value);
            }

            var sessions = await query.OrderBy(s => s.NameEn).ToListAsync(ct);

            var res = sessions.Select(s => MapToSessionDto(s)).ToList();
            return res;
        }

        public async Task<Result<PredefinedWorkoutSessionDto>> GetClientPredefinedSessionByIdAsync(Guid id, CancellationToken ct = default)
        {
            var session = await _context.PredefinedWorkoutSessions
                .Include(s => s.PredefinedWorkday)
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Exercise)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

            if (session == null)
            {
                return ExerciseErrors.PredefinedSessionNotFound;
            }

            return MapToSessionDto(session);
        }

        // =========================================================================
        // USER PICKING FLOW
        // =========================================================================

        public async Task<Result<WorkoutSessionRes>> PickSessionToWorkoutAsync(string userId, Guid predefinedSessionId, PickSessionToWorkoutReq req, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId, ct);
            if (profile == null)
            {
                return UserErrors.ProfileNotFound;
            }

            Guid clientProfileId = profile.Id;
            Guid? createdByTrainerId = null;

            if (req.TraineeId.HasValue)
            {
                var relationship = await _context.TrainerTrainees
                    .FirstOrDefaultAsync(tt => tt.TrainerId == profile.Id && tt.TraineeId == req.TraineeId.Value, ct);

                if (relationship == null)
                {
                    return TrainerRequestErrors.UnauthorizedAccess;
                }

                clientProfileId = req.TraineeId.Value;
                createdByTrainerId = profile.Id;
            }

            var predefinedSession = await _context.PredefinedWorkoutSessions
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Exercise)
                        .ThenInclude(ex => ex.Section)
                .FirstOrDefaultAsync(s => s.Id == predefinedSessionId, ct);

            if (predefinedSession == null)
            {
                return ExerciseErrors.PredefinedSessionNotFound;
            }

            var sessionNotes = !string.IsNullOrWhiteSpace(req.Notes)
                ? req.Notes
                : $"{LocalizeString(predefinedSession.NameEn, predefinedSession.NameAr)}";

            var sessionResult = WorkoutSession.Create(
                Guid.NewGuid(),
                clientProfileId,
                req.Date,
                sessionNotes,
                createdByTrainerId
            );

            if (sessionResult.IsError)
            {
                return sessionResult.Errors;
            }

            var session = sessionResult.Value;
            _context.WorkoutSessions.Add(session);

            // Instantiate each exercise and its sets
            foreach (var pe in predefinedSession.Exercises.OrderBy(e => e.Order))
            {
                var workoutExerciseResult = WorkoutExercise.Create(
                    Guid.NewGuid(),
                    session.Id,
                    pe.ExerciseId,
                    null
                );

                if (workoutExerciseResult.IsSuccess)
                {
                    var workoutExercise = workoutExerciseResult.Value;
                    workoutExercise.ClientProfileId = clientProfileId;
                    session.AddWorkoutExercise(workoutExercise);
                    _context.WorkoutExercises.Add(workoutExercise);

                    // Add planned sets
                    var setsCount = pe.DefaultSets > 0 ? pe.DefaultSets : 3;
                    var repsCount = pe.DefaultReps > 0 ? pe.DefaultReps : 10;
                    for (int i = 1; i <= setsCount; i++)
                    {
                        var setResult = ExerciseSet.Create(
                            Guid.NewGuid(),
                            workoutExercise.Id,
                            setNumber: i,
                            reps: repsCount,
                            weightKg: 0m,
                            restTimeSeconds: pe.DefaultRestTimeSeconds ?? 60,
                            notes: null
                        );
                        if (setResult.IsSuccess)
                        {
                            workoutExercise.AddSet(setResult.Value);
                            _context.ExerciseSets.Add(setResult.Value);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync(ct);

            // Re-fetch with all navigations for accurate DTO
            var createdSession = await _context.WorkoutSessions
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                        .ThenInclude(e => e.Section)
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(ws => ws.Id == session.Id, ct);

            return WorkoutSessionRes.FromEntity(createdSession ?? session);
        }

        // =========================================================================
        // PRIVATE HELPERS
        // =========================================================================

        private async Task<AdminPredefinedSessionRes> GetAdminSessionResByIdAsync(Guid id, CancellationToken ct)
        {
            var s = await _context.PredefinedWorkoutSessions
                .Include(s => s.PredefinedWorkday)
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Exercise)
                .FirstAsync(s => s.Id == id, ct);

            return MapToAdminSessionRes(s);
        }

        private static AdminPredefinedSessionRes MapToAdminSessionRes(PredefinedWorkoutSession s)
        {
            return new AdminPredefinedSessionRes(
                s.Id,
                s.PredefinedWorkdayId,
                s.PredefinedWorkday?.NameEn,
                s.PredefinedWorkday?.NameAr,
                s.NameEn,
                s.NameAr,
                s.DescriptionEn,
                s.DescriptionAr,
                s.ImageUrl,
                s.DifficultyLevel,
                s.EstimatedDurationMinutes,
                s.IsActive,
                s.Exercises.OrderBy(e => e.Order).Select(e => new AdminSessionExerciseRes(
                    e.Id,
                    e.ExerciseId,
                    e.Exercise?.NameEn ?? e.Exercise?.Name ?? string.Empty,
                    e.Exercise?.NameAr,
                    e.Exercise?.ImageUrl,
                    e.Order,
                    e.DefaultSets,
                    e.DefaultReps,
                    e.DefaultRestTimeSeconds
                )).ToList(),
                s.CreatedAtUtc
            );
        }

        private static PredefinedWorkoutSessionDto MapToSessionDto(PredefinedWorkoutSession s)
        {
            return new PredefinedWorkoutSessionDto(
                s.Id,
                s.PredefinedWorkdayId,
                s.PredefinedWorkday != null ? LocalizeString(s.PredefinedWorkday.NameEn, s.PredefinedWorkday.NameAr) : null,
                LocalizeString(s.NameEn, s.NameAr),
                LocalizeString(s.DescriptionEn, s.DescriptionAr),
                s.ImageUrl,
                s.DifficultyLevel,
                s.EstimatedDurationMinutes,
                s.NameEn,
                s.NameAr,
                s.Exercises.OrderBy(e => e.Order).Select(e => new PredefinedSessionExerciseDto(
                    e.Id,
                    e.ExerciseId,
                    LocalizeString(e.Exercise?.NameEn ?? e.Exercise?.Name, e.Exercise?.NameAr),
                    LocalizeString(e.Exercise?.DescriptionEn ?? e.Exercise?.Description, e.Exercise?.DescriptionAr),
                    e.Exercise?.ImageUrl,
                    LocalizeString(e.Exercise?.EquipmentEn ?? e.Exercise?.Equipment, e.Exercise?.EquipmentAr),
                    e.Order,
                    e.DefaultSets,
                    e.DefaultReps,
                    e.DefaultRestTimeSeconds
                )).ToList()
            );
        }
    }
}
