using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Trainer;

namespace GymAssistant_API.Repository.Interfaces.User.Trainer
{
    public interface ITrainerService
    {
        Task<Result<List<TraineeData>>> GetTraineesAsync(string trainerId, CancellationToken ct = default);
        Task<Result<TraineeData>> GetTraineeAsync(string trainerId, Guid traineeId, CancellationToken ct = default);
        Task<Result<Deleted>> RemoveTraineeAsync(string trainerId, Guid traineeId, CancellationToken ct = default);
        Task<Result<WorkoutSessionRes>> CreateSessionForTraineeAsync(string trainerId, Guid traineeId, DateTime date, string? notes = null, CancellationToken ct = default);
        Task<Result<List<WorkoutSessionRes>>> GetTraineeSessionsAsync(string trainerId, Guid traineeId, int pageSize, int pageNumber, CancellationToken ct = default);
        Task<Result<WorkoutSessionRes>> GetTraineeSessionAsync(string trainerId, Guid traineeId, Guid sessionId, CancellationToken ct = default);
        Task<Result<TraineeProgressData>> GetTraineeProgressAsync(string trainerId, Guid traineeId, int days, Guid sectionId, CancellationToken ct = default);
        Task<Result<TrainerDashboardData>> GetTrainerDashboardAsync(string trainerId, CancellationToken ct = default);
    }
}
