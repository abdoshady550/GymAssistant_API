using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Reqeust.Predefined;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Predefined;

namespace GymAssistant_API.Handeler.Exercise
{
    public sealed class PredefinedDataHandler(ILogger<PredefinedDataHandler> logger, IPredefinedDataService service)
    {
        private readonly ILogger<PredefinedDataHandler> _logger = logger;
        private readonly IPredefinedDataService _service = service;

        public Task<Result<List<PredefinedSectionDto>>> GetSections(CancellationToken ct = default)
            => _service.GetClientSectionsAsync(ct);

        public Task<Result<PredefinedSectionDto>> GetSectionById(Guid id, CancellationToken ct = default)
            => _service.GetClientSectionByIdAsync(id, ct);

        public Task<Result<List<PredefinedExerciseDto>>> GetExercises(Guid? sectionId = null, DifficultyLevel? difficulty = null, string? searchTerm = null, CancellationToken ct = default)
            => _service.GetClientExercisesAsync(sectionId, difficulty, searchTerm, ct);

        public Task<Result<PredefinedExerciseDto>> GetExerciseById(Guid id, CancellationToken ct = default)
            => _service.GetClientExerciseByIdAsync(id, ct);

        public Task<Result<List<PredefinedWorkdayDto>>> GetWorkdays(CancellationToken ct = default)
            => _service.GetClientWorkdaysAsync(ct);

        public Task<Result<PredefinedWorkdayDto>> GetWorkdayById(Guid id, CancellationToken ct = default)
            => _service.GetClientWorkdayByIdAsync(id, ct);

        public Task<Result<List<PredefinedWorkoutSessionDto>>> GetSessions(Guid? workdayId = null, DifficultyLevel? difficulty = null, CancellationToken ct = default)
            => _service.GetClientPredefinedSessionsAsync(workdayId, difficulty, ct);

        public Task<Result<PredefinedWorkoutSessionDto>> GetSessionById(Guid id, CancellationToken ct = default)
            => _service.GetClientPredefinedSessionByIdAsync(id, ct);

        public Task<Result<WorkoutSessionRes>> PickSessionToWorkout(string userId, Guid predefinedSessionId, PickSessionToWorkoutReq req, CancellationToken ct = default)
            => _service.PickSessionToWorkoutAsync(userId, predefinedSessionId, req, ct);
    }
}
