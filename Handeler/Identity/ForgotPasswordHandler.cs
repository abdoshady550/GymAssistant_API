using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using Microsoft.AspNetCore.Mvc;


namespace GymAssistant_API.Handeler.Identity
{
    public class ForgotPasswordHandler(ILogger<ForgotPasswordHandler> logger,
        IIdentityService identityService)
    {
        private readonly IIdentityService _identityService = identityService;
        private readonly ILogger<ForgotPasswordHandler> _logger = logger;



        public async Task<Result<string>> Handle([FromBody] ForgotPasswordDto request, CancellationToken ct = default)
        {


            var result = await _identityService.ForgotPasswordAsync(request.Email);

            if (result.IsError)
            {
                _logger.LogError("User with Email { Email }{ErrorDetails}", request.Email, result.TopError.Description);

                return Error.NotFound("Invalid_email", result.TopError.Description ?? "Email not found");
            }
            return result.Value;
        }

    }
}
