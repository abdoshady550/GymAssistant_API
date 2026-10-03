using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust.Predefined;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Predefined;

namespace GymAssistant_API.Repository.Interfaces.Exercise
{
    public interface IPredefinedDataService
    {
        // Sections
        Task<Result<AdminSectionRes>> CreateSectionAsync(AdminSectionReq req, CancellationToken ct = default);
        Task<Result<Updated>> UpdateSectionAsync(Guid id, AdminSectionReq req, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteSectionAsync(Guid id, CancellationToken ct = default);
        Task<Result<List<AdminSectionRes>>> GetAdminSectionsAsync(CancellationToken ct = default);
        Task<Result<List<PredefinedSectionDto>>> GetClientSectionsAsync(CancellationToken ct = default);
        Task<Result<PredefinedSectionDto>> GetClientSectionByIdAsync(Guid id, CancellationToken ct = default);

        // Exercises
        Task<Result<AdminExerciseRes>> CreateExerciseAsync(AdminExerciseReq req, CancellationToken ct = default);
        Task<Result<Updated>> UpdateExerciseAsync(Guid id, AdminExerciseReq req, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteExerciseAsync(Guid id, CancellationToken ct = default);
        Task<Result<List<AdminExerciseRes>>> GetAdminExercisesAsync(Guid? sectionId = null, CancellationToken ct = default);
        Task<Result<List<PredefinedExerciseDto>>> GetClientExercisesAsync(Guid? sectionId = null, DifficultyLevel? difficulty = null, string? searchTerm = null, CancellationToken ct = default);
        Task<Result<PredefinedExerciseDto>> GetClientExerciseByIdAsync(Guid id, CancellationToken ct = default);

        // Workdays
        Task<Result<AdminWorkdayRes>> CreateWorkdayAsync(AdminWorkdayReq req, CancellationToken ct = default);
        Task<Result<Updated>> UpdateWorkdayAsync(Guid id, AdminWorkdayReq req, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteWorkdayAsync(Guid id, CancellationToken ct = default);
        Task<Result<List<AdminWorkdayRes>>> GetAdminWorkdaysAsync(CancellationToken ct = default);
        Task<Result<List<PredefinedWorkdayDto>>> GetClientWorkdaysAsync(CancellationToken ct = default);
        Task<Result<PredefinedWorkdayDto>> GetClientWorkdayByIdAsync(Guid id, CancellationToken ct = default);

        // Predefined Sessions
        Task<Result<AdminPredefinedSessionRes>> CreatePredefinedSessionAsync(AdminPredefinedSessionReq req, CancellationToken ct = default);
        Task<Result<Updated>> UpdatePredefinedSessionAsync(Guid id, AdminPredefinedSessionReq req, CancellationToken ct = default);
        Task<Result<Deleted>> DeletePredefinedSessionAsync(Guid id, CancellationToken ct = default);
        Task<Result<List<AdminPredefinedSessionRes>>> GetAdminPredefinedSessionsAsync(Guid? workdayId = null, CancellationToken ct = default);
        Task<Result<List<PredefinedWorkoutSessionDto>>> GetClientPredefinedSessionsAsync(Guid? workdayId = null, DifficultyLevel? difficulty = null, CancellationToken ct = default);
        Task<Result<PredefinedWorkoutSessionDto>> GetClientPredefinedSessionByIdAsync(Guid id, CancellationToken ct = default);

        // User Picking Flow
        Task<Result<WorkoutSessionRes>> PickSessionToWorkoutAsync(string userId, Guid predefinedSessionId, PickSessionToWorkoutReq req, CancellationToken ct = default);
    }
}
