using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Trainer;

namespace GymAssistant_API.Repository.Interfaces.User.Trainer
{
    public interface ITrainerRequestService
    {
        // Trainer actions
        Task<Result<TrainerRequestResponse>> SendRequestAsync(
            string trainerId,
            Guid traineeId,
            string? message = null,
            CancellationToken ct = default);

        Task<Result<TrainerRequestListResponse>> GetSentRequestsAsync(
            string trainerId,
            int pageSize,
            int pageNumber,
            CancellationToken ct = default);

        Task<Result<Deleted>> CancelRequestAsync(
            string trainerId,
            Guid requestId,
            CancellationToken ct = default);

        // Trainee actions
        Task<Result<TrainerRequestListResponse>> GetReceivedRequestsAsync(
            string traineeId,
            int pageSize,
            int pageNumber,
            CancellationToken ct = default);

        Task<Result<TrainerRequestResponse>> AcceptRequestAsync(
            string traineeId,
            Guid requestId,
            CancellationToken ct = default);

        Task<Result<TrainerRequestResponse>> RejectRequestAsync(
            string traineeId,
            Guid requestId,
            CancellationToken ct = default);

        // Common
        Task<Result<TrainerRequestResponse>> GetRequestByIdAsync(
            string userId,
            Guid requestId,
            CancellationToken ct = default);
    }
}
