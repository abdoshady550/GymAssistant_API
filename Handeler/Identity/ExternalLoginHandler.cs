using Google.Apis.Auth;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Repository.Services.Identity;
using System.Text.Json;

namespace GymAssistant_API.Handeler.Identity
{
    public class ExternalLoginHandler(
            ILogger<ExternalLoginHandler> logger,
            IIdentityService identityService,
            ITokenProvider tokenProvider,
            IConfiguration configuration)
    {
        private readonly ILogger<ExternalLoginHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IConfiguration _configuration = configuration;
        public async Task<Result<TokenResponse>> Handle(ExternalLoginDto request, CancellationToken ct)
        {
            try
            {
                ExternalAuthInfoDto? externalInfo = null;

                // معالجة حسب نوع الـ Provider
                if (request.Provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
                {
                    externalInfo = await ValidateGoogleTokenAsync(request.IdToken);
                }
                else if (request.Provider.Equals("Facebook", StringComparison.OrdinalIgnoreCase))
                {
                    externalInfo = await ValidateFacebookTokenAsync(request.AccessToken!);
                }
                else
                {
                    return Error.Validation("Invalid_Provider", "Provider must be Google or Facebook");
                }

                if (externalInfo == null)
                {
                    return Error.Unauthorized("Invalid_Token", "Failed to validate external token");
                }

                // تسجيل الدخول أو إنشاء حساب جديد
                var userResult = await _identityService.ExternalLoginAsync(externalInfo);

                if (userResult.IsError)
                {
                    _logger.LogError("External login failed: {Error}", userResult.TopError.Description);
                    return userResult.Errors;
                }

                // إنشاء JWT Token
                var tokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResult.Value, ct);

                if (tokenResult.IsError)
                {
                    _logger.LogError("Token generation failed: {Error}", tokenResult.TopError.Description);
                    return tokenResult.Errors;
                }

                return tokenResult.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "External login error for provider: {Provider}", request.Provider);
                return Error.Failure("External_Login_Failed", "An error occurred during external login");
            }
        }

        private async Task<ExternalAuthInfoDto?> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new ExternalAuthInfoDto
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Picture = payload.Picture,
                    Provider = "Google",
                    ProviderId = payload.Subject
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google token validation failed");
                return null;
            }
        }

        private async Task<ExternalAuthInfoDto?> ValidateFacebookTokenAsync(string accessToken)
        {
            try
            {
                using var httpClient = new HttpClient();

                // التحقق من صحة الـ Token
                var appId = _configuration["Authentication:Facebook:AppId"];
                var appSecret = _configuration["Authentication:Facebook:AppSecret"];

                var verifyUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";
                var verifyResponse = await httpClient.GetAsync(verifyUrl);

                if (!verifyResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                // الحصول على معلومات المستخدم
                var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,first_name,last_name,picture&access_token={accessToken}";
                var userInfoResponse = await httpClient.GetAsync(userInfoUrl);

                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<FacebookUserInfo>(userInfoJson);

                if (userInfo == null || string.IsNullOrEmpty(userInfo.Email))
                {
                    return null;
                }

                return new ExternalAuthInfoDto
                {
                    Email = userInfo.Email,
                    Name = userInfo.Name,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    Picture = userInfo.Picture?.Data?.Url,
                    Provider = "Facebook",
                    ProviderId = userInfo.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Facebook token validation failed");
                return null;
            }
        }

    }
    // Helper classes for Facebook API
    internal class FacebookUserInfo
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public FacebookPicture? Picture { get; set; }
    }

    internal class FacebookPicture
    {
        public FacebookPictureData? Data { get; set; }
    }

    internal class FacebookPictureData
    {
        public string Url { get; set; } = null!;
    }
}
