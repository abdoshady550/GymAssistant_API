using GymAssistant_API.Model.Results;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Model.Entities.User
{
    public static class UserErrors
    {
        public static readonly Error IdRequired =
            Error.Validation(LocalizationKeys.User.IdRequired);

        public static Error FirstNameRequired =>
            Error.Validation(LocalizationKeys.User.FirstNameRequired);

        public static Error LastNameRequired =>
            Error.Validation(LocalizationKeys.User.LastNameRequired);

        public static Error GenderInvalid =>
            Error.Validation(LocalizationKeys.User.GenderInvalid);

        public static Error BirthDayRequired =>
            Error.Validation(LocalizationKeys.User.BirthDayRequired);

        public static Error HeightInvalid =>
            Error.Validation(LocalizationKeys.User.HeightInvalid);

        public static Error RoleInvalid =>
            Error.Validation(LocalizationKeys.User.RoleInvalid);

        public static Error WeightKgInvalid =>
            Error.Validation(LocalizationKeys.User.WeightInvalid);

        public static Error BodyFatPercentInvalid =>
            Error.Validation(LocalizationKeys.User.BodyFatPercentInvalid);

        public static Error MuscleMassKgInvalid =>
            Error.Validation(LocalizationKeys.User.MuscleMassKgInvalid);

        public static Error NameRequired =>
            Error.Validation(LocalizationKeys.User.NameRequired);

        public static Error UserNotFound =>
            Error.NotFound(LocalizationKeys.User.NotFound);

        public static Error DeleteUserFailed =>
            Error.Failure(LocalizationKeys.User.DeleteFailed);

        public static Error ProfileNotFound =>
            Error.NotFound(LocalizationKeys.Profile.NotFound);

        public static Error ProfileAlreadyExists =>
            Error.Conflict(LocalizationKeys.Profile.AlreadyExists);

        public static Error MeasurementNotFound =>
            Error.NotFound(LocalizationKeys.Profile.MeasurementNotFound);
    }
}
