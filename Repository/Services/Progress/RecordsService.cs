using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Response.Progress;
using GymAssistant_API.Req_Res.Response.Records;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymAssistant_API.Repository.Services.Progress
{
    public sealed class RecordsService : IRecordsService
    {
        private readonly AppDbContext _context;

        public RecordsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<PersonalRecordResponse>>> GetPersonalRecordsAsync(string userId,
                                                                        RecordType? recordType = null,
                                                                        CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var query = _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id);

            if (recordType.HasValue)
            {
                query = query.Where(pr => pr.RecordType == recordType);
            }
            var records = await query
                .Include(pr => pr.Exercise)
                    .ThenInclude(e => e.Section)
                .Include(pr => pr.UserExercise)
                .Include(pr => pr.WorkoutSession)
                .OrderByDescending(pr => pr.CreatedAtUtc)
                .ToListAsync(ct);
            var response = records.Select(r => r.ToResponse()).ToList();
            return response;
        }

        public async Task<Result<List<PersonalRecordResponse>>> GetExerciseRecordsAsync(string userId,
                                                                                Guid exerciseId,
                                                                                CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var results = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id && pr.ExerciseId == exerciseId)
                .Include(pr => pr.Exercise)
                    .ThenInclude(e => e.Section)
                .Include(pr => pr.WorkoutSession)
                .OrderByDescending(pr => pr.CreatedAtUtc)
                .ToListAsync(ct);
            var response = results.Select(r => r.ToResponse()).ToList();
            return response;
        }

        public async Task<Result<List<PersonalRecordResponse>>> GetCustomExerciseRecordsAsync(string userId,
                                                                                      Guid userExerciseId,
                                                                                      CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var results = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id && pr.UserExerciseId == userExerciseId)
                .Include(pr => pr.UserExercise)
                .Include(pr => pr.WorkoutSession)
                .OrderByDescending(pr => pr.CreatedAtUtc)
                .ToListAsync(ct);
            var response = results.Select(r => r.ToResponse()).ToList();
            return response;
        }

        public async Task<Result<List<PersonalRecordResponse>>> GetRecentRecordsAsync(string userId,
                                                                              int count,
                                                                              CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var results = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id)
                .Include(pr => pr.Exercise)
                    .ThenInclude(e => e.Section)
                .Include(pr => pr.UserExercise)
                .Include(pr => pr.WorkoutSession)
                .OrderByDescending(pr => pr.CreatedAtUtc)
                .Take(count)
                .ToListAsync(ct);
            var response = results.Select(r => r.ToResponse()).ToList();
            return response;
        }

        public async Task<Result<AchievementsData>> GetAchievementsAsync(string userId,
                                                                         DateTime? fromDate = null,
                                                                         DateTime? toDate = null,
                                                                         CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var query = _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id);

            if (fromDate.HasValue)
            {
                query = query.Where(pr => pr.CreatedAtUtc >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(pr => pr.CreatedAtUtc <= toDate.Value);
            }

            var records = await query
                .Include(r => r.WorkoutSession)
                         .ThenInclude(Wo => Wo.WorkoutExercises)
                         .ThenInclude(we => we.Sets)
                .Include(pr => pr.Exercise)
                    .ThenInclude(e => e.Section)
                .Include(pr => pr.UserExercise)
                .Include(pr => pr.WorkoutSession)
                .ToListAsync(ct);

            var achievements = new AchievementsData
            {
                TotalRecords = records.Count,
                WeightRecords = records.Count(r => r.RecordType == RecordType.MaxWeight),
                RepsRecords = records.Count(r => r.RecordType == RecordType.MaxReps),
                VolumeRecords = records.Count(r => r.RecordType == RecordType.MaxVolume),
                RecentRecords = records.OrderByDescending(r => r.CreatedAtUtc).Take(5).Select(r => r.ToResponse()).ToList(),
                RecordsBySection = records
                                   .Where(r => r.Exercise != null)
                                   .GroupBy(r => r.Exercise.Section.Name)
                                   .Select(g => new SectionAchievement
                                   {
                                       SectionName = g.Key,
                                       RecordCount = g.Count(),
                                       LatestRecord = g.Any() ? g.Max(r => r.CreatedAtUtc) : default
                                   })
                                   .OrderByDescending(sa => sa.RecordCount)
                                   .ToList(),
                Milestones = await CalculateMilestones(records)
            };

            return achievements;
        }
        public async Task<Result<StatesRes>> GetStates(string userId, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);
            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }
            var workoutsCount = await _context.WorkoutSessions
                .Where(ws => ws.ClientProfileId == profile.Id)
                .CountAsync(ct);
            var customExercisesCount = await _context.UserExercises.Where(ue => ue.ClientProfileId == profile.Id)
                .CountAsync(ct);
            var recordsCount = await _context.PersonalRecords.Where(pr => pr.ClientProfileId == profile.Id).CountAsync(ct);
            ////////////////////////////
            var lastWorkout = await _context.WorkoutSessions
                .Where(ws => ws.ClientProfileId == profile.Id)
                .OrderByDescending(ws => ws.Date)
                .FirstOrDefaultAsync(ct);
            var lastRecord = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id)
                .OrderByDescending(pr => pr.CreatedAtUtc)
                .FirstOrDefaultAsync(ct);
            var lastWorkoutDate = lastWorkout?.Date;
            var lastRecordDate = lastRecord?.CreatedAtUtc;
            var daysSinceLastWorkout = lastWorkoutDate.HasValue ? (DateTime.UtcNow - lastWorkoutDate.Value).Days : (int?)null;
            var daysSinceLastRecord = lastRecordDate.HasValue ? (DateTime.UtcNow - lastRecordDate.Value).Days : (int?)null;
            ////////////////////////////
            var workoutSessions = await _context.WorkoutSessions
               .Where(ws => ws.ClientProfileId == profile.Id)
               .Select(ws => new { ws.StartTime, ws.EndTime })
               .ToListAsync(ct);

            var averageWorkoutDuration = workoutSessions
                .Where(ws => ws.StartTime.HasValue && ws.EndTime.HasValue)
                .Select(ws => (ws.EndTime!.Value - ws.StartTime!.Value).TotalMinutes)
                .DefaultIfEmpty(0)
                .Average();

            var averageRecordsPerWorkout = workoutsCount > 0 ?
                (decimal)recordsCount / workoutsCount : 0;

            ///////////////////////////
            var personalBestWeight = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id && pr.RecordType == RecordType.MaxWeight)
                .MaxAsync(pr => (decimal?)pr.Value, ct);
            var personalBestReps = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id && pr.RecordType == RecordType.MaxReps)
                .MaxAsync(pr => (int?)pr.Value, ct);
            var personalBestVolume = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id && pr.RecordType == RecordType.MaxVolume)
                .MaxAsync(pr => (decimal?)pr.Value, ct);



            return new StatesRes(workoutsCount,
                                  customExercisesCount,
                                  recordsCount,
                                  daysSinceLastWorkout,
                                  daysSinceLastRecord,
                                  averageWorkoutDuration,
                                  averageRecordsPerWorkout,
                                  personalBestWeight,
                                  personalBestReps,
                                  personalBestVolume);
        }

        private async Task<List<Milestone>> CalculateMilestones(List<PersonalRecord> records)
        {
            var milestones = new List<Milestone>();

            // Weight milestones (every 10kg increment)
            var weightRecords = records.Where(r => r.RecordType == RecordType.MaxWeight).ToList();
            var maxWeight = weightRecords.Any() ? weightRecords.Max(r => r.Value) : 0;

            if (maxWeight >= 100)
            {
                milestones.Add(new Milestone
                {
                    Title = "Century Club",
                    Description = $"Lifted {maxWeight}kg - your first 100kg milestone!",
                    Icon = "💪",
                    Record = records.Where(r => r.RecordType == RecordType.MaxWeight)
                                 .OrderByDescending(r => r.Value)
                                 .Select(r => r.ToResponse())
                                 .FirstOrDefault(),
                    Date = weightRecords
                        .Where(r => r.Value >= 100)
                        .OrderBy(r => r.CreatedAtUtc)
                        .Select(r => r.CreatedAtUtc)
                        .FirstOrDefault()
                });
            }

            // Volume milestones
            var volumeRecords = records.Where(r => r.RecordType == RecordType.MaxVolume).ToList();
            var maxVolume = volumeRecords.Any() ? volumeRecords.Max(r => r.Value) : 0;
            if (maxVolume >= 1000)
            {
                milestones.Add(new Milestone
                {
                    Title = "Volume Beast",
                    Description = $"Achieved {maxVolume}kg total volume in a single exercise!",
                    Icon = "🔥",
                    Record = records.Where(r => r.RecordType == RecordType.MaxVolume)
                                 .OrderByDescending(r => r.Value)
                                 .Select(r => r.ToResponse())
                                 .FirstOrDefault(),
                    Date = volumeRecords
                        .Where(r => r.Value >= 1000)
                        .OrderBy(r => r.CreatedAtUtc)
                        .Select(r => r.CreatedAtUtc)
                        .FirstOrDefault()
                });
            }

            // Consistency milestones
            var repsRecords = records.Where(r => r.RecordType == RecordType.MaxReps).ToList();
            var maxReps = repsRecords.Any() ? repsRecords.Max(r => r.Value) : 0;
            if (maxReps >= 20)
            {
                milestones.Add(new Milestone
                {
                    Title = "Reps Breaker",
                    Description = $"{maxReps} Reps personal records!",
                    Icon = "🏆",
                    Record = records.Where(r => r.RecordType == RecordType.MaxReps)
                                 .OrderByDescending(r => r.Value)
                                 .Select(r => r.ToResponse())
                                 .FirstOrDefault(),
                    Date = repsRecords
                        .Where(r => r.Value >= 20)
                        .OrderBy(r => r.CreatedAtUtc)
                        .Select(r => r.CreatedAtUtc)
                        .FirstOrDefault()
                });
            }

            return milestones.OrderByDescending(m => m.Date).ToList();
        }
    }
}
