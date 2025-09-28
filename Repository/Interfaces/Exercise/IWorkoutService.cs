using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Exercise;

namespace GymAssistant_API.Repository.Interfaces.Exercise
{
    public interface IWorkoutService
    {
        Task<Result<WorkoutSessionRes>> CreateWorkoutSessionAsync(string userId, DateTime date, string? notes = null, Guid? traineeId = null, CancellationToken ct = default);
        Task<Result<WorkoutSessionRes>> GetWorkoutSessionAsync(string userId, Guid sessionId, CancellationToken ct = default);
        Task<Result<Updated>> StartWorkoutSessionAsync(string userId, Guid sessionId, DateTime startTime, CancellationToken ct = default);
        Task<Result<Updated>> CompleteWorkoutSessionAsync(string userId, Guid sessionId, DateTime endTime, string? notes = null, CancellationToken ct = default);
        Task<Result<WorkoutExerciseRes>> AddExerciseToWorkoutAsync(string userId, Guid sessionId, Guid? exerciseId = null, Guid? userExerciseId = null, CancellationToken ct = default);
        Task<Result<WorkoutExerciseRes>> GetWorkoutExerciseAsync(string userId, Guid sessionId, Guid exerciseId, CancellationToken ct = default);
        Task<Result<ExerciseSetRes>> AddSetToExerciseAsync(string userId, Guid sessionId, Guid exerciseId, int setNumber, int reps, decimal weightKg, int? restTimeSeconds = null, string? notes = null, CancellationToken ct = default);
        Task<Result<ExerciseSetRes>> GetExerciseSetAsync(string userId, Guid sessionId, Guid exerciseId, Guid setId, CancellationToken ct = default);
        Task<Result<Updated>> UpdateExerciseSetAsync(string userId, Guid sessionId, Guid exerciseId, Guid setId, int reps, decimal weightKg, int? restTimeSeconds = null, string? notes = null, CancellationToken ct = default);
        Task<Result<List<WorkoutSessionRes>>> GetWorkoutHistoryAsync(string userId, int pageSize, int pageNumber, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
    }
}
