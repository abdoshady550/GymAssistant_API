using GymAssistant_API.Model.Results;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Handeler
{
    public static class ApplicationErrors
    {
        public static readonly Error ExpiredAccessTokenInvalid = Error.Conflict(
             LocalizationKeys.Auth.ExpiredAccessTokenInvalid);

        public static readonly Error UserIdClaimInvalid = Error.Conflict(
             LocalizationKeys.Auth.UserIdClaimInvalid);

        public static readonly Error RefreshTokenExpired = Error.Conflict(
             LocalizationKeys.Auth.RefreshTokenExpired);

        public static readonly Error UserNotFound = Error.NotFound(
             LocalizationKeys.Auth.UserNotFound);

        public static readonly Error TokenGenerationFailed = Error.Failure(
             LocalizationKeys.Auth.TokenGenerationFailed);
    }
}
