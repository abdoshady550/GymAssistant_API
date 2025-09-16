using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Req_Res.Reqeust;

namespace GymAssistant_API.Handeler.Identity
{
    public class GenerateTokenQueryHandler(ILogger<GenerateTokenQueryHandler> logger, IIdentityService identityService, ITokenProvider tokenProvider)
    {
        private readonly ILogger<GenerateTokenQueryHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;

        public async Task<Result<TokenResponse>> Handle(LoginRequest query, CancellationToken ct)
        {
            var userResponse = await _identityService.AuthenticateAsync(query.Email, query.Password);

            if (userResponse.IsError)
            {
                return userResponse.Errors;
            }
            var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResponse.Value, ct);

            if (generateTokenResult.IsError)
            {
                _logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);

                return generateTokenResult.Errors;
            }

            return generateTokenResult.Value;
        }
    }
}
