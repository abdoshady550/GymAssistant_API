using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Repository.Interfaces.User.Trainer;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Progress;
using GymAssistant_API.Req_Res.Response.Records;
using GymAssistant_API.Req_Res.Response.Trainer;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Repository.Services.User.Trainer
{
    public class TrainerService : ITrainerService
    {
        private readonly AppDbContext _context;
        private readonly IWorkoutService _workoutService;
        private readonly IProgressService _ProgressService;
        private readonly IRecordsService _recordsService;



        public TrainerService(AppDbContext context,
                              IWorkoutService workoutService,
                              IProgressService progress,
                              IRecordsService records)
        {
            _context = context;
            _workoutService = workoutService;
            _ProgressService = progress;
            _recordsService = records;
        }

        public async Task<Result<TrainerTraineeResponse>> AddTraineeAsync(string trainerId, Guid traineeId, CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId && p.Role == UserRole.Trainer, ct);

            if (trainerProfile == null)
            {
                return Error.Validation("Trainer_NotFound", "Trainer profile not found or user is not a trainer.");
            }

            var traineeProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.Id == traineeId, ct);

            if (traineeProfile == null)
            {
                return Error.NotFound("Trainee_NotFound", "Trainee not found.");
            }

            // Check if relationship already exists
            var existingRelation = await _context.TrainerTrainees
                .FirstOrDefaultAsync(tt => tt.TrainerId == trainerProfile.Id && tt.TraineeId == traineeId, ct);

            if (existingRelation != null)
            {
                return Error.Validation("Relationship_Exists", "This trainee is already assigned to you.");
            }

            var relationResult = TrainerTrainee.Create(Guid.NewGuid(), trainerProfile.Id, traineeId);
            if (relationResult.IsError)
            {
                return relationResult.Errors;
            }

            var relation = relationResult.Value;
            _context.TrainerTrainees.Add(relation);
            await _context.SaveChangesAsync(ct);
            var response = TrainerTraineeResponse.FromEntity(relation);

            return response;
        }

        public async Task<Result<List<TraineeData>>> GetTraineesAsync(string trainerId, CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId && p.Role == UserRole.Trainer, ct);

            if (trainerProfile == null)
            {
                return Error.Validation("Trainer_NotFound", "Trainer profile not found or user is not a trainer.");
            }

            var trainees = await _context.TrainerTrainees
                .Where(tt => tt.TrainerId == trainerProfile.Id)
                .Include(tt => tt.Trainee)
                    .ThenInclude(t => t.Measurements)
                .Include(tt => tt.Trainee)
                    .ThenInclude(t => t.Workouts)
                .ToListAsync(ct);

            return trainees.Select(tt => new TraineeData
            {
                TraineeId = tt.TraineeId,
                FirstName = tt.Trainee.FirstName,
                LastName = tt.Trainee.LastName,
                FullName = tt.Trainee.FullName,
                Gender = tt.Trainee.Gender,
                Age = tt.Trainee.BirthDate.HasValue ?
                      DateTime.UtcNow.Year - tt.Trainee.BirthDate.Value.Year : null,
                CurrentWeight = tt.Trainee.CurrentWeight,
                TotalWorkouts = tt.Trainee.Workouts.Count,
                LastWorkout = tt.Trainee.Workouts.OrderByDescending(w => w.Date).FirstOrDefault()?.Date,
                AssignedDate = tt.CreatedAtUtc
            }).ToList();
        }

        public async Task<Result<TraineeData>> GetTraineeAsync(string trainerId, Guid traineeId, CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId && p.Role == UserRole.Trainer, ct);

            if (trainerProfile == null)
            {
                return Error.Validation("Trainer_NotFound", "Trainer profile not found or user is not a trainer.");
            }

            var relation = await _context.TrainerTrainees
                .Where(tt => tt.TrainerId == trainerProfile.Id && tt.TraineeId == traineeId)
                .Include(tt => tt.Trainee)
                    .ThenInclude(t => t.Measurements)
                .Include(tt => tt.Trainee)
                    .ThenInclude(t => t.Workouts)
                .Include(tt => tt.Trainee)
                    .ThenInclude(t => t.PersonalRecords)
                .FirstOrDefaultAsync(ct);

            if (relation == null)
            {
                return Error.NotFound("Relationship_NotFound", "This trainee is not assigned to you.");
            }

            var traineeData = new TraineeData
            {
                TraineeId = relation.TraineeId,
                FirstName = relation.Trainee.FirstName,
                LastName = relation.Trainee.LastName,
                FullName = relation.Trainee.FullName,
                Gender = relation.Trainee.Gender,
                Age = relation.Trainee.BirthDate.HasValue ?
                      DateTime.UtcNow.Year - relation.Trainee.BirthDate.Value.Year : null,
                HeightCm = relation.Trainee.HeightCm,
                CurrentWeight = relation.Trainee.CurrentWeight,
                TotalWorkouts = relation.Trainee.Workouts.Count,
                LastWorkout = relation.Trainee.Workouts.OrderByDescending(w => w.Date).FirstOrDefault()?.Date,
                PersonalRecords = relation.Trainee.PersonalRecords.Count,
                AssignedDate = relation.CreatedAtUtc
            };

            return traineeData;
        }

        public async Task<Result<Deleted>> RemoveTraineeAsync(string trainerId, Guid traineeId, CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId && p.Role == UserRole.Trainer, ct);

            if (trainerProfile == null)
            {
                return Error.Validation("Trainer_NotFound", "Trainer profile not found or user is not a trainer.");
            }

            var relation = await _context.TrainerTrainees
                .FirstOrDefaultAsync(tt => tt.TrainerId == trainerProfile.Id && tt.TraineeId == traineeId, ct);

            if (relation == null)
            {
                return Error.NotFound("Relationship_NotFound", "This trainee is not assigned to you.");
            }

            _context.TrainerTrainees.Remove(relation);
            await _context.SaveChangesAsync(ct);

            return Result.Deleted;
        }

        public async Task<Result<WorkoutSessionRes>> CreateSessionForTraineeAsync(string trainerId,
                                                                                  Guid traineeId,
                                                                                  DateTime date,
                                                                                  string? notes = null,
                                                                                  CancellationToken ct = default)
        {
            // Verify trainer-trainee relationship first
            var relationCheck = await GetTraineeAsync(trainerId, traineeId, ct);
            if (relationCheck.IsError)
            {
                return relationCheck.Errors;
            }

            // Use the workout service to create the session
            return await _workoutService.CreateWorkoutSessionAsync(trainerId, date, notes, traineeId, ct);
        }

        public async Task<Result<List<WorkoutSessionRes>>> GetTraineeSessionsAsync(string trainerId,
                                                                                Guid traineeId,
                                                                                int pageSize,
                                                                                int pageNumber, CancellationToken ct = default)
        {
            // Verify trainer-trainee relationship
            var relationCheck = await GetTraineeAsync(trainerId, traineeId, ct);
            if (relationCheck.IsError)
            {
                return relationCheck.Errors;
            }

            var sessions = await _context.WorkoutSessions
                .Where(ws => ws.ClientProfileId == traineeId)
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .OrderByDescending(ws => ws.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
            var sessionResponses = sessions.Select(WorkoutSessionRes.FromEntity).ToList();
            return sessionResponses;
        }

        public async Task<Result<WorkoutSessionRes>> GetTraineeSessionAsync(string trainerId,
                                                                         Guid traineeId,
                                                                         Guid sessionId, CancellationToken ct = default)
        {
            // Verify trainer-trainee relationship
            var relationCheck = await GetTraineeAsync(trainerId, traineeId, ct);
            if (relationCheck.IsError)
            {
                return relationCheck.Errors;
            }

            var session = await _context.WorkoutSessions
                .Where(ws => ws.Id == sessionId && ws.ClientProfileId == traineeId)
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .Include(ws => ws.CreatedByTrainer)
                .FirstOrDefaultAsync(ct);

            if (session == null)
            {
                return Error.NotFound("Session_NotFound", "Workout session not found.");
            }
            var sessionResponse = WorkoutSessionRes.FromEntity(session);
            return sessionResponse;
        }

        public async Task<Result<TraineeProgressData>> GetTraineeProgressAsync(string trainerId,
                                                                               Guid traineeId,
                                                                               int days,
                                                                               Guid sectionId,
                                                                               CancellationToken ct = default)
        {
            // Verify trainer-trainee relationship
            var relationCheck = await GetTraineeAsync(trainerId, traineeId, ct);
            if (relationCheck.IsError)
            {
                return relationCheck.Errors;
            }

            var traineeInfo = relationCheck.Value;
            var fromDate = DateTime.UtcNow.AddDays(-days);

            // Get progress overview
            var progressOverview = await _ProgressService.GetProgressOverviewAsync(trainerId, days, ct);

            if (progressOverview.IsError)
            {
                return progressOverview.Errors;
            }

            // Get recent records
            var recentRecords = await _recordsService.GetRecentRecordsAsync(trainerId, 5, ct);

            if (recentRecords.IsError)
            {
                return recentRecords.Errors;
            }

            // Get recent workouts
            var recentWorkouts = await _workoutService.GetWorkoutHistoryAsync(trainerId,
                                                                              5,
                                                                              1,
                                                                              fromDate,
                                                                              DateTime.UtcNow,
                                                                              ct);
            if (recentWorkouts.IsError)
            {
                return recentWorkouts.Errors;
            }
            // Get section progress

            var sectionProgress = await _ProgressService.GetSectionProgressAsync(trainerId,
                                                                                 sectionId,
                                                                                 days,
                                                                                 ct);

            if (sectionProgress.IsError)
            {
                sectionProgress = new List<SectionProgressData>();
            }
            var traineeProgressData = new TraineeProgressData
            {
                TraineeInfo = traineeInfo,
                ProgressOverview = progressOverview.Value,
                RecentRecords = recentRecords.Value,
                RecentWorkouts = recentWorkouts.Value,
                SectionProgress = sectionProgress.Value
            };

            return traineeProgressData;
        }

        public async Task<Result<TrainerDashboardData>> GetTrainerDashboardAsync(string trainerId, CancellationToken ct = default)
        {
            var trainerProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == trainerId && p.Role == UserRole.Trainer, ct);

            if (trainerProfile == null)
            {
                return Error.Validation("Trainer_NotFound", "Trainer profile not found or user is not a trainer.");
            }

            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            // Get all trainees
            var trainees = await _context.TrainerTrainees
                .Where(tt => tt.TrainerId == trainerProfile.Id)
                .Include(tt => tt.Trainee)
                    .ThenInclude(t => t.Workouts)
                .ToListAsync(ct);

            var totalTrainees = trainees.Count;

            // Active trainees today (had a workout)
            var activeToday = trainees.Count(t =>
                t.Trainee.Workouts.Any(w => w.Date.Date == today));

            // Sessions created by trainer
            var totalSessions = await _context.WorkoutSessions
                .CountAsync(ws => ws.CreatedByTrainerId == trainerProfile.Id, ct);

            var sessionsThisWeek = await _context.WorkoutSessions
                .CountAsync(ws => ws.CreatedByTrainerId == trainerProfile.Id &&
                                 ws.Date >= weekStart, ct);

            // Recently active trainees
            var recentlyActive = trainees
                .Where(t => t.Trainee.Workouts.Any())
                .OrderByDescending(t => t.Trainee.Workouts.Max(w => w.Date))
                .Take(5)
                .Select(t => new TraineeData
                {
                    TraineeId = t.TraineeId,
                    FirstName = t.Trainee.FirstName,
                    LastName = t.Trainee.LastName,
                    FullName = t.Trainee.FullName,
                    Gender = t.Trainee.Gender,
                    CurrentWeight = t.Trainee.CurrentWeight,
                    TotalWorkouts = t.Trainee.Workouts.Count,
                    LastWorkout = t.Trainee.Workouts.OrderByDescending(w => w.Date).FirstOrDefault()?.Date,
                    AssignedDate = t.CreatedAtUtc
                })
                .ToList();

            // Recent sessions
            var recentSessions = await _context.WorkoutSessions
                .Where(ws => ws.CreatedByTrainerId == trainerProfile.Id)
                .Include(ws => ws.Trainee)
                .OrderByDescending(ws => ws.CreatedAtUtc)
                .Take(10)
                .ToListAsync(ct);
            var recentSessionsRes = recentSessions.Select(WorkoutSessionRes.FromEntity).ToList();

            return new TrainerDashboardData
            {
                TotalTrainees = totalTrainees,
                ActiveTraineesToday = activeToday,
                TotalSessionsCreated = totalSessions,
                SessionsThisWeek = sessionsThisWeek,
                RecentlyActiveTrainees = recentlyActive,
                RecentSessions = recentSessionsRes,
                TraineesBySection = new Dictionary<string, int>() // Could be populated based on most used exercises
            };
        }
    }
}
