using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Req_Res.Reqeust.User;
using Error = GymAssistant_API.Model.Results.Error;

namespace GymAssistant_API.Handeler.Identity
{
    public sealed class ChangePasswordHandler(ILogger<ChangePasswordHandler> logger,
                                              IIdentityService identityService)
    {
        private readonly ILogger<ChangePasswordHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;

        public async Task<Result<Updated>> Handle(string id, ChangePasswordRequest request)
        {
            _logger.LogInformation("Handling ChangePasswordRequest for UserId: {UserId}", id);
            var result = await _identityService.ChangeUserPasswordAsync(
                id,
                request.CurrentPassword,
                request.NewPassword,
                request.ConfirmPassword);

            if (result.IsError)
            {
                _logger.LogWarning("Change password failed for UserId: {UserId}, Error: {Error}",
                    id, result.Errors.Select(e =>
                  Error.Validation(e.Code, e.Description)).ToList());

                return result.Errors;
            }
            _logger.LogInformation("Password changed successfully for UserId: {UserId}", id);

            return Result.Updated;
        }
        public async Task<Result<Updated>> ChangeSeedingUsersPW(string id, ChangeSeedingPasswordRequest request)
        {
            _logger.LogInformation("Handling ChangePasswordRequest for UserId: {UserId}", id);
            var result = await _identityService.UpdateSeedingUsers(id, request);
            if (result.IsError)
            {
                _logger.LogWarning("Change password failed for UserId: {UserId}, Error: {Error}",
                    id, result.Errors.Select(e =>
                  Error.Validation(e.Code, e.Description)).ToList());
                return result.Errors;
            }

            _logger.LogInformation("Password changed successfully for UserId: {UserId}", id);
            return Result.Updated;
        }
    }
}
