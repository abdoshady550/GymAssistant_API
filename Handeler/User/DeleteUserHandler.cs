using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;

namespace GymAssistant_API.Handeler.User
{
    public sealed class DeleteUserHandler(ILogger<DeleteUserHandler> logger,
                                          IIdentityService identityService)
    {
        private readonly ILogger<DeleteUserHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;

        public async Task<Result<Deleted>> Handle(string userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Handling account deletion for UserId: {UserId}", userId);

            var result = await _identityService.DeleteUserAccountAsync(userId, ct);
            if (result.IsError)
            {
                _logger.LogWarning("Account deletion failed for UserId: {UserId}, Errors: {@Errors}",
                    userId, result.Errors);
                return result.Errors;
            }

            _logger.LogInformation("User account deleted successfully for UserId: {UserId}", userId);
            return Result.Deleted;
        }
    }
}
