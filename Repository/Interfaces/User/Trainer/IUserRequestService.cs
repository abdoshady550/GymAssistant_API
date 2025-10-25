using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Trainer;

namespace GymAssistant_API.Repository.Interfaces.User.Trainer
{
    public interface IUserRequestService
    {
        Task<Result<TrainerRequestResponse>> SendRequestAsync(
            string trainerId,
            string traineeId,
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

        Task<Result<TrainerRequestListResponse>> GetReceivedRequestsAsync(
            string trainerId,
            int pageSize,
            int pageNumber,
            CancellationToken ct = default);

        Task<Result<TrainerRequestResponse>> AcceptRequestAsync(
            string trainerId,
            Guid requestId,
            CancellationToken ct = default);

        Task<Result<TrainerRequestResponse>> RejectRequestAsync(
            string trainerId,
            Guid requestId,
            CancellationToken ct = default);

        Task<Result<TrainerRequestResponse>> GetRequestByIdAsync(
            string userId,
            Guid requestId,
            CancellationToken ct = default);
        Task<Result<List<UserDto>>> GetAllUserAsync(
          string? searchTerm,
          int pageSize,
          int pageNumber,
          CancellationToken ct = default);
        Task<Result<List<UserDto>>> GetAllTrainerAsync(
          string? searchTerm,
          int pageSize,
          int pageNumber,
          CancellationToken ct = default);
    }
}
