using Azure.Core;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using Microsoft.AspNetCore.Mvc;


namespace GymAssistant_API.Handeler.Identity
{
    public class ResetPasswordHandler(ILogger<ResetPasswordHandler> logger,
        IIdentityService identityService)
    {
        private readonly IIdentityService _identityService = identityService;
        private readonly ILogger<ResetPasswordHandler> _logger = logger;



        public async Task<Result<string>> Handle([FromBody] ResetPasswordDto request, CancellationToken ct = default)
        {


            var result = await _identityService.ResetPasswordAsync(request);

            if (result.IsError)
            {
                _logger.LogError("User with Email { Email }{ErrorDetails}", request.Email, result.TopError.Description);

                return Error.NotFound("Some thing wrong", result.TopError.Description);

            }
            return result.Value;
        }

    }
}
