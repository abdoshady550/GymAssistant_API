using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Reqeust.Exercises;
using GymAssistant_API.Req_Res.Response.Exercise;

namespace GymAssistant_API.Handeler.Exercise.Workout
{
    public sealed class WorkoutHandler(ILogger<WorkoutHandler> logger,
                                IWorkoutService workoutService)
    {
        private readonly ILogger<WorkoutHandler> _logger = logger;
        private readonly IWorkoutService _workoutService = workoutService;

        public async Task<Result<WorkoutSessionRes>> CreateWorkoutSession(string userId, CreateWorkoutSessionRequest request, CancellationToken ct = default)
        {

            var result = await _workoutService.CreateWorkoutSessionAsync(userId,
                                                                         request.Date,
                                                                         request.Notes,
                                                                         request.TraineeId, ct);
            if (result.IsError)
            {
                _logger.LogError("Error creating workout session for user ID: {UserId}. Error: {Error}", userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully created workout session with ID: {WorkoutSessionId} for user ID: {UserId}", result.Value.Id, userId);
            return result;

        }
        public async Task<Result<WorkoutSessionRes>> GetWorkoutSession(string userId,
                                                                    Guid id,
                                                                    CancellationToken ct = default)
        {
            var result = await _workoutService.GetWorkoutSessionAsync(userId, id, ct);
            if (result.IsError)
            {
                _logger.LogError("Error retrieving workout session with ID: {WorkoutSessionId} for user ID: {UserId}. Error: {Error}", id, userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved workout session with ID: {WorkoutSessionId} for user ID: {UserId}", result.Value.Id, userId);

            return result;
        }
        public async Task<Result<Updated>> StartWorkoutSession(string userId, Guid id, DateTime StartTime, CancellationToken ct = default)
        {
            var result = await _workoutService.StartWorkoutSessionAsync(userId, id, StartTime, ct);
            if (result.IsError)
            {
                _logger.LogError("Error starting workout session with ID: {WorkoutSessionId} for user ID: {UserId}. Error: {Error}", id, userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully started workout session with ID: {WorkoutSessionId} for user ID: {UserId}", id, userId);
            return Result.Updated;

        }
        public async Task<Result<Updated>> CompleteWorkoutSession(string userId, Guid id, DateTime endTime, string? notes = null, CancellationToken ct = default)
        {
            var result = await _workoutService.CompleteWorkoutSessionAsync(userId,
                                                                           id,
                                                                           endTime,
                                                                           notes, ct);
            if (result.IsError)
            {
                _logger.LogError("Error completing workout session with ID: {WorkoutSessionId} for user ID: {UserId}. Error: {Error}", id, userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully completed workout session with ID: {WorkoutSessionId} for user ID: {UserId}", id, userId);
            return Result.Updated;
        }
        public async Task<Result<WorkoutExerciseRes>> AddExerciseToWorkout(string userId, Guid sessionId, Guid? exerciseId = null, Guid? userExerciseId = null, CancellationToken ct = default)
        {
            var result = await _workoutService.AddExerciseToWorkoutAsync(userId,
                                                                         sessionId,
                                                                         exerciseId,
                                                                         userExerciseId, ct);
            if (result.IsError)
            {
                _logger.LogError("Error adding exercise to workout session with ID: {WorkoutSessionId} for user ID: {UserId}. Error: {Error}", sessionId, userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully added exercise to workout session with ID: {WorkoutSessionId} for user ID: {UserId}", sessionId, userId);
            return result;
        }
        public async Task<Result<WorkoutExerciseRes>> GetWorkoutExercise(string userId, Guid exerciseId, CancellationToken ct = default)
        {
            var result = await _workoutService.GetWorkoutExerciseAsync(userId, exerciseId, ct);
            if (result.IsError)
            {
                _logger.LogError("Error retrieving workout exercise with ID: {ExerciseId} for user ID: {UserId}. Error: {Error}", exerciseId, userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved workout exercise with ID: {ExerciseId} for user ID: {UserId}", exerciseId, userId);
            return result;
        }
        public async Task<Result<ExerciseSetRes>> AddSetToExercise(string userId, Guid sessionId, Guid exerciseId, AddExerciseSetRequest request, CancellationToken ct = default)
        {
            var result = await _workoutService.AddSetToExerciseAsync(userId,
                                                                     sessionId,
                                                                     exerciseId,
                                                                     request.SetNumber,
                                                                     request.Reps,
                                                                     request.WeightKg,
                                                                     request.RestTimeSeconds,
                                                                     request.Notes, ct);
            if (result.IsError)
            {
                _logger.LogError("Error adding set to exercise with ID: {ExerciseId} in session ID: {WorkoutSessionId} for user ID: {UserId}. Error: {Error}", exerciseId, sessionId, userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully added set to exercise with ID: {ExerciseId} in session ID: {WorkoutSessionId} for user ID: {UserId}", exerciseId, sessionId, userId);
            return result;
        }
        public async Task<Result<ExerciseSetRes>> GetExerciseSet(string userId, Guid setId, CancellationToken ct = default)
        {
            var result = await _workoutService.GetExerciseSetAsync(userId, setId, ct);
            if (result.IsError)
            {
                _logger.LogError("Error retrieving exercise set with ID: {SetId}for user ID: {UserId}. Error: {Error}", setId, userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved exercise set with ID: {SetId} for user ID: {UserId}", setId, userId);
            return result;

        }
        public async Task<Result<ExerciseSetRes>> UpdateExerciseSet(string userId, Guid setId, UpdateExerciseSetRequest request, CancellationToken ct = default)
        {
            var result = await _workoutService.UpdateExerciseSetAsync(userId,
                                                                      setId,
                                                                      request.Reps,
                                                                      request.WeightKg,
                                                                      request.RestTimeSeconds,
                                                                      request.Notes, ct);
            if (result.IsError)
            {
                _logger.LogError("Error updating exercise set with ID: {SetId}  for user ID: {UserId}. Error: {Error}", setId, userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully updated exercise set with ID: {SetId} for for user ID: {UserId}", setId, userId);
            return result.Value;
        }
        public async Task<Result<List<WorkoutSessionRes>>> GetWorkoutHistory(string userId,
                                                                          int pageSize = 20,
                                                                          int pageNumber = 1,
                                                                          DateTime? fromDate = null,
                                                                          DateTime? toDate = null,
                                                                          CancellationToken ct = default)
        {
            var result = await _workoutService.GetWorkoutHistoryAsync(userId,
                                                                      pageSize,
                                                                      pageNumber,
                                                                      fromDate,
                                                                      toDate, ct);
            if (result.IsError)
            {
                _logger.LogError("Error retrieving workout history for user ID: {UserId}. Error: {Error}", userId, result.Errors);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved workout history for user ID: {UserId}", userId);
            return result;

        }

    }
}
