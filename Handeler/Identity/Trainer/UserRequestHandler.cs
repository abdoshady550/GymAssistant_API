using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User.Trainer;
using GymAssistant_API.Req_Res.Reqeust.User.Trainer;
using GymAssistant_API.Req_Res.Response.Trainer;

namespace GymAssistant_API.Handeler.Identity.Trainer
{
    public sealed class UserRequestHandler(ILogger<UserRequestHandler> logger,
                                       ITrainerRequestService requestService)
    {
        private readonly ILogger<UserRequestHandler> _logger = logger;
        private readonly ITrainerRequestService _requestService = requestService;

        public async Task<Result<TrainerRequestResponse>> SendRequest(string userId, SendUserRequestDto request, CancellationToken ct)
        {
            var result = await _requestService.SendRequestAsync(
               userId,
               request.TrainerId,
               request.Message,
               ct);
            if (result.IsError)
            {
                _logger.LogError("Error sending trainer request: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            _logger.LogInformation("Trainer request sent successfully: {RequestId}", result.Value.RequestId);
            return result.Value;
        }
        public async Task<Result<TrainerRequestListResponse>> GetSentRequests(string userId, int pageSize, int pageNumber, CancellationToken ct)
        {
            var result = await _requestService.GetSentRequestsAsync(
               userId,
               pageSize,
               pageNumber,
               ct);
            if (result.IsError)
            {
                _logger.LogError("Error retrieving sent trainer requests: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            _logger.LogInformation("Retrieved {Count} sent trainer requests", result.Value.Requests.Count);
            return result.Value;
        }
        public async Task<Result<Deleted>> CancelRequest(string userId, Guid requestId, CancellationToken ct)
        {
            var result = await _requestService.CancelRequestAsync(
               userId,
               requestId,
               ct);
            if (result.IsError)
            {
                _logger.LogError("Error cancelling trainer request: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            _logger.LogInformation("Trainer request cancelled successfully: {RequestId}", requestId);
            return Result.Deleted;
        }
        public async Task<Result<TrainerRequestListResponse>> GetReceivedRequests(string userId, int pageSize, int pageNumber, CancellationToken ct)
        {
            var result = await _requestService.GetReceivedRequestsAsync(
               userId,
               pageSize,
               pageNumber,
               ct);
            if (result.IsError)
            {
                _logger.LogError("Error retrieving received trainer requests: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            _logger.LogInformation("Retrieved {Count} received trainer requests", result.Value.Requests.Count);
            return result.Value;
        }
        public async Task<Result<TrainerRequestResponse>> AcceptRequest(string userId, Guid requestId, CancellationToken ct)
        {
            var result = await _requestService.AcceptRequestAsync(
               userId,
               requestId,
               ct);
            if (result.IsError)
            {
                _logger.LogError("Error accepting trainer request: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            _logger.LogInformation("Trainer request accepted successfully: {RequestId}", requestId);
            return result.Value;
        }
        public async Task<Result<TrainerRequestResponse>> RejectRequest(string userId, Guid requestId, CancellationToken ct)
        {
            var result = await _requestService.RejectRequestAsync(
               userId,
               requestId,
               ct);
            if (result.IsError)
            {
                _logger.LogError("Error rejecting trainer request: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            _logger.LogInformation("Trainer request rejected successfully: {RequestId}", requestId);
            return result.Value;
        }
        public async Task<Result<TrainerRequestResponse>> GetRequestById(string user, Guid requestId,
            CancellationToken ct)
        {
            var result = await _requestService.GetRequestByIdAsync(
               user,
               requestId,
               ct);
            if (result.IsError)
            {
                _logger.LogError("Error retrieving trainer request by ID: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            _logger.LogInformation("Retrieved trainer request by ID successfully: {RequestId}", requestId);
            return result.Value;
        }
        public async Task<Result<List<UserDto>>> GetAllTrainer(string? searchTerm, int pageSize, int pageNumber, CancellationToken ct)
        {
            var result = await _requestService.GetAllTrainerAsync(
               searchTerm,
               pageSize,
               pageNumber,
               ct);
            if (result.IsError)
            {
                _logger.LogError("Error retrieving all trainers: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            _logger.LogInformation("Retrieved {Count} trainers", result.Value.Count);
            return result.Value;
        }

    }
}
