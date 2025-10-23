using Google.Api.Gax;
using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Res;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Repository.Interfaces.Notifications;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Reqeust;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;

namespace GymAssistant_API.Handeler.Identity
{
    public class GenerateTokenQueryHandler(ILogger<GenerateTokenQueryHandler> logger,
        IPushNotificationService pushNotification,
        IIdentityService identityService, ITokenProvider tokenProvider,
        UserManager<AppUser> userManager,
        IProfile profile)
    {
        private readonly UserManager<AppUser> _userManager = userManager;

        private readonly ILogger<GenerateTokenQueryHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IProfile _profile = profile;
        private readonly IPushNotificationService _pushNotification = pushNotification;


        public async Task<Result<object>> Handle(LoginRequest query, CancellationToken ct)
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
            var user = await _userManager.FindByEmailAsync(query.Email);

            var profile = await _profile.GetProfileAsync(user.Id);
            // If profile doesn't exist, return just the token
            if (profile.IsError)
            {
                return new { Token = generateTokenResult.Value };
            }
            if (string.IsNullOrWhiteSpace(query.fcmToken))
            {
                return new
                {
                    Token = generateTokenResult.Value,
                    Profile = profile.Value
                };
            }
            var registerDevice = await _pushNotification.RegisterDeviceTokenAsync(userResponse.Value.UserId, query.fcmToken, DevicePlatform.Android, ct);
            if (registerDevice.IsError)
            {
                _logger.LogError("Failed to register device token for user {UserId}: {TopError}", userResponse.Value.UserId, registerDevice.TopError.Description);
                return registerDevice.Errors;
            }


            return new
            {
                Token = generateTokenResult.Value,
                RegisterDevice = registerDevice.Value,
                Profile = profile.Value
            };
        }
    }
}
