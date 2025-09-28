using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Repository.Interfaces.Exercises;
using GymAssistant_API.Req_Res.Response.Exercise;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Repository.Services.Exercise
{
    public class WorkoutService(AppDbContext context,
                                IPersonalRecordService personalRecordService) : IWorkoutService
    {
        private readonly AppDbContext _context = context;
        private readonly IPersonalRecordService _personalRecordService = personalRecordService;

        public async Task<Result<WorkoutExerciseRes>> AddExerciseToWorkoutAsync(string userId, Guid sessionId, Guid? exerciseId = null, Guid? userExerciseId = null, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var session = await _context.WorkoutSessions
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(ws => ws.Id == sessionId &&
                    (ws.ClientProfileId == profile.Id || ws.CreatedByTrainerId == profile.Id), ct);

            if (session == null)
            {
                return Error.NotFound("Session_NotFound", "Workout session not found.");
            }

            if (session.IsCompleted)
            {
                return Error.Validation("Session_Completed", "Cannot add exercises to completed session.");
            }
            var exercis = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == exerciseId, ct);
            if (exerciseId.HasValue)
            {
                var exerciseExists = await _context.Exercises
                    .AnyAsync(e => e.Id == exerciseId.Value, ct);
                if (!exerciseExists)
                {
                    return Error.NotFound("Exercise_NotFound", "Exercise not found.");
                }
            }
            else if (!userExerciseId.HasValue)
            {
                return Error.Validation("ExerciseId_Or_UserExerciseId_Required", "Either ExerciseId or UserExerciseId must be provided.");
            }

            var workoutExerciseResult = WorkoutExercise.Create(Guid.NewGuid(), sessionId, exerciseId, userExerciseId);

            if (workoutExerciseResult.IsError)
            {
                return workoutExerciseResult.Errors;
            }

            var workoutExercise = workoutExerciseResult.Value;
            workoutExercise.ClientProfileId = session.ClientProfileId;

            session.AddWorkoutExercise(workoutExercise);
            _context.WorkoutExercises.Add(workoutExercise);

            await _context.SaveChangesAsync(ct);
            var dto = WorkoutExerciseRes.FromEntity(workoutExercise);
            return dto;
        }

        public async Task<Result<ExerciseSetRes>> AddSetToExerciseAsync(string userId, Guid sessionId, Guid exerciseId, int setNumber, int reps, decimal weightKg, int? restTimeSeconds = null, string? notes = null, CancellationToken ct = default)
        {
            var sessionResult = await GetWorkoutSessionAsync(userId, sessionId, ct);
            if (sessionResult.IsError)
            {
                return sessionResult.Errors;
            }

            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Sets)
                .FirstOrDefaultAsync(we => we.Id == exerciseId && we.WorkoutSessionId == sessionId, ct);

            if (workoutExercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Workout exercise not found.");
            }



            var setResult = ExerciseSet.Create(Guid.NewGuid(), exerciseId, setNumber, reps, weightKg, restTimeSeconds, notes);
            if (setResult.IsError)
            {
                return setResult.Errors;
            }

            var exerciseSet = setResult.Value;
            workoutExercise.AddSet(exerciseSet);
            _context.ExerciseSets.Add(exerciseSet);

            await _context.SaveChangesAsync(ct);
            var dto = ExerciseSetRes.FromEntity(exerciseSet);
            return dto;
        }

        public async Task<Result<Updated>> CompleteWorkoutSessionAsync(string userId, Guid sessionId, DateTime endTime, string? notes = null, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                  .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var session = await _context.WorkoutSessions
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(ws => ws.Id == sessionId &&
                    (ws.ClientProfileId == profile.Id || ws.CreatedByTrainerId == profile.Id), ct);

            if (session == null)
            {
                return Error.NotFound("Session_NotFound", "Workout session not found.");
            }

            if (session.IsCompleted)
            {
                return Error.Validation("Session_AlreadyCompleted", "Session is already completed.");
            }

            if (!session.StartTime.HasValue)
            {
                return Error.Validation("Session_NotStarted", "Session must be started before it can be completed.");
            }

            session.CompleteWorkout(endTime, notes);

            // Check for personal records
            await _personalRecordService.CheckAndCreatePersonalRecordsAsync(session, ct);

            await _context.SaveChangesAsync(ct);
            return Result.Updated;
        }

        public async Task<Result<WorkoutSessionRes>> CreateWorkoutSessionAsync(string userId,
                                                                            DateTime date,
                                                                            string? notes = null,
                                                                            Guid? traineeId = null, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            Guid clientProfileId = profile.Id;
            Guid? createdByTrainerId = null;

            // If traineeId is provided, this is a trainer creating a session for a trainee
            if (traineeId.HasValue)
            {
                // Verify trainer-trainee relationship
                var relationship = await _context.TrainerTrainees
                    .FirstOrDefaultAsync(tt => tt.TrainerId == profile.Id && tt.TraineeId == traineeId.Value, ct);

                if (relationship == null)
                {
                    return Error.Validation("Trainer_Unauthorized", "You are not authorized to create sessions for this trainee.");
                }

                clientProfileId = traineeId.Value;
                createdByTrainerId = profile.Id;
            }

            var sessionResult = WorkoutSession.Create(Guid.NewGuid(), clientProfileId, date, notes, createdByTrainerId);
            if (sessionResult.IsError)
            {
                return sessionResult.Errors;
            }

            var session = sessionResult.Value;
            _context.WorkoutSessions.Add(session);

            await _context.SaveChangesAsync(ct);
            var dto = WorkoutSessionRes.FromEntity(session);

            return dto;
        }

        public async Task<Result<ExerciseSetRes>> GetExerciseSetAsync(string userId, Guid sessionId, Guid exerciseId, Guid setId, CancellationToken ct = default)
        {
            var workoutExerciseResult = await GetWorkoutExerciseAsync(userId, sessionId, exerciseId, ct);
            if (workoutExerciseResult.IsError)
            {
                return workoutExerciseResult.Errors;
            }

            var exerciseSet = await _context.ExerciseSets
                .FirstOrDefaultAsync(es => es.Id == setId && es.WorkoutExerciseId == exerciseId, ct);

            if (exerciseSet == null)
            {
                return Error.NotFound("Set_NotFound", "Exercise set not found.");
            }
            var dto = ExerciseSetRes.FromEntity(exerciseSet);
            return dto;
        }

        public async Task<Result<WorkoutExerciseRes>> GetWorkoutExerciseAsync(string userId, Guid sessionId, Guid exerciseId, CancellationToken ct = default)
        {
            var sessionResult = await GetWorkoutSessionAsync(userId, sessionId, ct);
            if (sessionResult.IsError)
            {
                return sessionResult.Errors;
            }

            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Sets)
                .FirstOrDefaultAsync(we => we.Id == exerciseId && we.WorkoutSessionId == sessionId, ct);

            if (workoutExercise == null)
            {
                return Error.NotFound("Exercise_NotFound", "Workout exercise not found.");
            }
            var dto = WorkoutExerciseRes.FromEntity(workoutExercise);
            return dto;
        }

        public async Task<Result<List<WorkoutSessionRes>>> GetWorkoutHistoryAsync(string userId, int pageSize, int pageNumber, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", " Profile not found");
            }

            var query = _context.WorkoutSessions
                .Where(ws => ws.ClientProfileId == profile.Id || ws.CreatedByTrainerId == profile.Id);

            if (fromDate.HasValue)
            {
                query = query.Where(ws => ws.Date >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(ws => ws.Date <= toDate.Value);
            }
            var sessions = await query
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .OrderByDescending(ws => ws.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
            var dtoList = sessions.Select(WorkoutSessionRes.FromEntity).ToList();

            return dtoList;
        }

        public async Task<Result<WorkoutSessionRes>> GetWorkoutSessionAsync(string userId, Guid sessionId, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var session = await _context.WorkoutSessions
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(ws => ws.Id == sessionId &&
                    (ws.ClientProfileId == profile.Id || ws.CreatedByTrainerId == profile.Id), ct);

            if (session == null)
            {
                return Error.NotFound("Session_NotFound", "Workout session not found.");
            }
            var dto = WorkoutSessionRes.FromEntity(session);
            return dto;
        }

        public async Task<Result<Updated>> StartWorkoutSessionAsync(string userId, Guid sessionId, DateTime startTime, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return Error.NotFound("Profile_NotFound", "User profile not found.");
            }

            var session = await _context.WorkoutSessions
                .Include(ws => ws.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(ws => ws.Id == sessionId &&
                    (ws.ClientProfileId == profile.Id || ws.CreatedByTrainerId == profile.Id), ct);

            if (session == null)
            {
                return Error.NotFound("Session_NotFound", "Workout session not found.");
            }
            if (session.IsCompleted)
            {
                return Error.Validation("Session_AlreadyCompleted", "Cannot start a completed session.");
            }

            session.StartWorkout(startTime);
            await _context.SaveChangesAsync(ct);

            return Result.Updated;
        }

        public async Task<Result<Updated>> UpdateExerciseSetAsync(string userId,
                                                                  Guid sessionId,
                                                                  Guid exerciseId,
                                                                  Guid setId,
                                                                  int reps,
                                                                  decimal weightKg,
                                                                  int? restTimeSeconds = null,
                                                                  string? notes = null, CancellationToken ct = default)
        {
            var setResult = await GetExerciseSetAsync(userId, sessionId, exerciseId, setId, ct);
            if (setResult.IsError)
            {
                return setResult.Errors;
            }

            var exerciseSet = setResult.Value;

            // Check if session is completed
            var session = await _context.WorkoutSessions
                .FirstOrDefaultAsync(ws => ws.Id == sessionId, ct);

            if (session?.IsCompleted == true)
            {
                return Error.Validation("Session_Completed", "Cannot modify sets in completed session.");
            }

            // Since ExerciseSet has private setters, we need update methods
            // For now, creating a new set with updated values
            var updatedSetResult = ExerciseSet.Create(setId, exerciseId, exerciseSet.SetNumber, reps, weightKg, restTimeSeconds, notes);
            if (updatedSetResult.IsError)
            {
                return updatedSetResult.Errors;
            }

            await _context.SaveChangesAsync(ct);
            return Result.Updated;
        }
    }
}
