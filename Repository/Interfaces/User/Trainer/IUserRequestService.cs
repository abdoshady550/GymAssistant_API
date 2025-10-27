using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Trainer;

namespace GymAssistant_API.Repository.Interfaces.User.Trainer
{
    public interface IUserRequestService
    {
        Task<Result<TrainerRequestResponse>> SendRequestAsync(
            string traineeId,
            string trainerId,
            string? message = null,
            CancellationToken ct = default);

        Task<Result<TrainerRequestListResponse>> GetSentRequestsAsync(
            string traineeId,
            int pageSize,
            int pageNumber,
            CancellationToken ct = default);

        Task<Result<Deleted>> CancelRequestAsync(
            string traineeId,
            Guid requestId,
            CancellationToken ct = default);

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

        Task<Result<TrainerRequestResponse>> GetRequestByIdAsync(
            string userId,
            Guid requestId,
            CancellationToken ct = default);
        Task<Result<List<UserDto>>> GetAllUserAsync(string currentUserId,
          string? searchTerm,
          int pageSize,
          int pageNumber,
          CancellationToken ct = default);
        Task<Result<List<UserDto>>> GetAllTrainerAsync(string currentUserId,
          string? searchTerm,
          int pageSize,
          int pageNumber,
          CancellationToken ct = default);
    }
}
