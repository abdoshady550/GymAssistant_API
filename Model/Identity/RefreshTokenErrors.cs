using GymAssistant_API.Model.Results;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Model.Identity;

public static class RefreshTokenErrors
{
    public static readonly Error IdRequired =
        Error.Validation(LocalizationKeys.RefreshToken.IdRequired);

    public static readonly Error TokenRequired =
        Error.Validation(LocalizationKeys.RefreshToken.TokenRequired);

    public static readonly Error UserIdRequired =
        Error.Validation(LocalizationKeys.RefreshToken.UserIdRequired);

    public static readonly Error ExpiryInvalid =
        Error.Validation(LocalizationKeys.RefreshToken.ExpiryInvalid);
}