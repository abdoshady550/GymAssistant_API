using GymAssistant_API.Data;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Response.Progress;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Repository.Services.Progress
{
    public class ProgressService : IProgressService
    {
        private readonly AppDbContext _context;

        public ProgressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ExerciseProgressData>> GetExerciseProgressAsync(string userId, Guid exerciseId, int days, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var exercise = await _context.Exercises
                .Include(e => e.Section)
                .FirstOrDefaultAsync(e => e.Id == exerciseId, ct);

            if (exercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Exercise not found.");
            }

            var fromDate = DateTime.UtcNow.AddDays(-days);

            var workoutData = await _context.WorkoutExercises
                .Where(we => we.ExerciseId == exerciseId &&
                            we.ClientProfileId == profile.Id &&
                            we.Sets.Any(s => s.CreatedAtUtc >= fromDate))
                .Include(we => we.Sets)
                .Include(we => we.WorkoutSession)
                .OrderBy(we => we.CreatedAtUtc)
                .ToListAsync(ct);

            var progressData = new ExerciseProgressData
            {
                ExerciseId = exerciseId,
                ExerciseName = exercise.Name,
                SectionName = exercise.Section.Name,
                Sessions = workoutData.Select(we => new ExerciseSessionData
                {
                    Date = we.WorkoutSession.Date,
                    Sets = we.Sets.Select(s => new SetProgressData
                    {
                        SetNumber = s.SetNumber,
                        Reps = s.Reps,
                        WeightKg = s.WeightKg,
                        Volume = s.Reps * s.WeightKg,
                        IsPersonalRecord = s.IsPersonalRecord
                    }).ToList(),
                    MaxWeight = we.Sets.Max(s => s.WeightKg),
                    TotalVolume = we.Sets.Sum(s => s.Reps * s.WeightKg),
                    TotalSets = we.Sets.Count
                }).ToList()
            };

            return progressData;
        }

        public async Task<Result<ExerciseProgressData>> GetCustomExerciseProgressAsync(string userId, Guid userExerciseId, int days, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var userExercise = await _context.UserExercises
                .FirstOrDefaultAsync(e => e.Id == userExerciseId && e.UserId == userId, ct);

            if (userExercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Custom exercise not found.");
            }

            var fromDate = DateTime.UtcNow.AddDays(-days);

            var workoutData = await _context.WorkoutExercises
                .Where(we => we.UserExerciseId == userExerciseId &&
                            we.ClientProfileId == profile.Id &&
                            we.Sets.Any(s => s.CreatedAtUtc >= fromDate))
                .Include(we => we.Sets)
                .Include(we => we.WorkoutSession)
                .OrderBy(we => we.CreatedAtUtc)
                .ToListAsync(ct);

            var progressData = new ExerciseProgressData
            {
                ExerciseId = userExerciseId,
                ExerciseName = userExercise.Name,
                SectionName = "Custom Exercises",
                Sessions = workoutData.Select(we => new ExerciseSessionData
                {
                    Date = we.WorkoutSession.Date,
                    Sets = we.Sets.Select(s => new SetProgressData
                    {
                        SetNumber = s.SetNumber,
                        Reps = s.Reps,
                        WeightKg = s.WeightKg,
                        Volume = s.Reps * s.WeightKg,
                        IsPersonalRecord = s.IsPersonalRecord
                    }).ToList(),
                    MaxWeight = we.Sets.Max(s => s.WeightKg),
                    TotalVolume = we.Sets.Sum(s => s.Reps * s.WeightKg),
                    TotalSets = we.Sets.Count
                }).ToList()
            };

            return progressData;
        }

        public async Task<Result<List<SectionProgressData>>> GetSectionProgressAsync(string userId, Guid sectionId, int days, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var fromDate = DateTime.UtcNow.AddDays(-days);

            var sectionExercises = await _context.Exercises
                .Where(e => e.SectionId == sectionId)
                .Include(e => e.Section)
                .ToListAsync(ct);

            var progressData = new List<SectionProgressData>();

            foreach (var exercise in sectionExercises)
            {
                var workoutData = await _context.WorkoutExercises
                    .Where(we => we.ExerciseId == exercise.Id &&
                                we.ClientProfileId == profile.Id &&
                                we.Sets.Any(s => s.CreatedAtUtc >= fromDate))
                    .Include(we => we.Sets)
                    .ToListAsync(ct);

                if (workoutData.Any())
                {
                    var totalVolume = workoutData.SelectMany(we => we.Sets).Sum(s => s.Reps * s.WeightKg);
                    var totalSets = workoutData.SelectMany(we => we.Sets).Count();
                    var maxWeight = workoutData.SelectMany(we => we.Sets).Max(s => s.WeightKg);
                    var sessionsCount = workoutData.Count;

                    progressData.Add(new SectionProgressData
                    {
                        ExerciseId = exercise.Id,
                        ExerciseName = exercise.Name,
                        TotalVolume = totalVolume,
                        TotalSets = totalSets,
                        MaxWeight = maxWeight,
                        SessionsCount = sessionsCount,
                        LastWorkoutDate = workoutData.Max(we => we.CreatedAtUtc)
                    });
                }
            }

            return progressData.OrderByDescending(p => p.TotalVolume).ToList();
        }

        public async Task<Result<ProgressOverviewData>> GetProgressOverviewAsync(string userId, int days, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var fromDate = DateTime.UtcNow.AddDays(-days);

            var workoutSessions = await _context.WorkoutSessions
                .Where(ws => ws.ClientProfileId == profile.Id &&
                            ws.Date >= fromDate &&
                            ws.IsCompleted)
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .ToListAsync(ct);


            var totalWorkouts = workoutSessions.Count;
            var totalSets = workoutSessions.SelectMany(ws => ws.WorkoutExercises)
                                          .SelectMany(we => we.Sets).Count();
            var totalVolume = workoutSessions.SelectMany(ws => ws.WorkoutExercises)
                                            .SelectMany(we => we.Sets)
                                            .Sum(s => s.Reps * s.WeightKg);

            // هنا الحل - تحقق من وجود بيانات الأول
            var averageDuration = workoutSessions.Where(ws => ws.DurationMinutes.HasValue)
                                                .Select(ws => ws.DurationMinutes ?? 0)
                                                .DefaultIfEmpty(0) // إضافة هذا السطر
                                                .Average();

            var personalRecords = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id &&
                            pr.CreatedAtUtc >= fromDate)
                .CountAsync(ct);

            var mostActiveDay = workoutSessions.GroupBy(ws => ws.Date.DayOfWeek)
                                    .OrderByDescending(g => g.Count())
                                    .FirstOrDefault()?.Key;

            return new ProgressOverviewData
            {
                TotalWorkouts = totalWorkouts,
                TotalSets = totalSets,
                TotalVolume = totalVolume,
                AverageDuration = (int)averageDuration,
                PersonalRecords = personalRecords,
                MostActiveDay = mostActiveDay?.ToString(),
                WorkoutFrequency = totalWorkouts > 0 ? Math.Round((double)totalWorkouts / days, 2) : 0
            };
        }

        public async Task<Result<ExerciseChartData>> GetExerciseChartDataAsync(string userId, Guid exerciseId, int days, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var fromDate = DateTime.UtcNow.AddDays(-days);

            var workoutData = await _context.WorkoutExercises
                .Where(we => we.ExerciseId == exerciseId &&
                            we.ClientProfileId == profile.Id &&
                            we.Sets.Any(s => s.CreatedAtUtc >= fromDate))
                .Include(we => we.Sets)
                .Include(we => we.WorkoutSession)
                .OrderBy(we => we.WorkoutSession.Date)
                .ToListAsync(ct);

            var chartData = new ExerciseChartData
            {
                WeightProgression = workoutData.Select(we => new ChartPoint
                {
                    Date = we.WorkoutSession.Date,
                    Value = we.Sets.Max(s => s.WeightKg)
                }).ToList(),
                VolumeProgression = workoutData.Select(we => new ChartPoint
                {
                    Date = we.WorkoutSession.Date,
                    Value = we.Sets.Sum(s => s.Reps * s.WeightKg)
                }).ToList(),
                RepsProgression = workoutData.Select(we => new ChartPoint
                {
                    Date = we.WorkoutSession.Date,
                    Value = we.Sets.Max(s => s.Reps)
                }).ToList()
            };

            return chartData;
        }

        public async Task<Result<VolumeChartData>> GetVolumeChartDataAsync(string userId, int days, Guid? sectionId = null, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var fromDate = DateTime.UtcNow.AddDays(-days);

            var query = _context.WorkoutSessions
                .Where(ws => ws.ClientProfileId == profile.Id &&
                            ws.Date >= fromDate &&
                            ws.IsCompleted)
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .AsQueryable();

            if (sectionId.HasValue)
            {
                query = query.Where(ws => ws.WorkoutExercises.Any(we =>
                    we.ExerciseId.HasValue &&
                    _context.Exercises.Any(e => e.Id == we.ExerciseId && e.SectionId == sectionId)));
            }

            var workoutSessions = await query.ToListAsync(ct);

            var dailyVolume = workoutSessions.GroupBy(ws => ws.Date.Date)
                .Select(g => new ChartPoint
                {
                    Date = g.Key,
                    Value = g.SelectMany(ws => ws.WorkoutExercises)
                             .SelectMany(we => we.Sets)
                             .Sum(s => s.Reps * s.WeightKg)
                })
                .OrderBy(cp => cp.Date)
                .ToList();

            var weeklyVolume = workoutSessions.GroupBy(ws => GetWeekOfYear(ws.Date))
                .Select(g => new ChartPoint
                {
                    Date = g.First().Date,
                    Value = g.SelectMany(ws => ws.WorkoutExercises)
                             .SelectMany(we => we.Sets)
                             .Sum(s => s.Reps * s.WeightKg)
                })
                .OrderBy(cp => cp.Date)
                .ToList();

            return new VolumeChartData
            {
                DailyVolume = dailyVolume,
                WeeklyVolume = weeklyVolume,
                TotalVolume = dailyVolume.Sum(dv => dv.Value),
                AverageDaily = dailyVolume.Any() ? dailyVolume.Average(dv => dv.Value) : 0
            };
        }

        private static int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }
    }
}
