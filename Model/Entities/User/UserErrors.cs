using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.User
{
    public static class UserErrors
    {
        public static readonly Error IdRequired =
            Error.Validation("User.Id.Required", "Employee Id is required.");

        public static Error FirstNameRequired =>
            Error.Validation("User.FirstName.Required", "First name is required.");

        public static Error LastNameRequired =>
            Error.Validation("User.LastName.Required", "Last name is required.");
        public static Error GenderInvalid =>
            Error.Validation("GenderInvalid", "Gender must be a Male or Female.");
        public static Error BirthDayRequired =>
            Error.Validation("BirthDay.Required", "BirthDay not Valid.");
        public static Error HeightInvalid =>
            Error.Validation("HeightCm.Invalid", "HeightCm must be a positive integer.");
        public static Error RoleInvalid =>
            Error.Validation("User.Role.Invalid", "Invalid role assigned to User.");
        public static Error WeightKgInvalid =>
            Error.Validation("WeightKg.Invalid", "WeightKg must be a non-negative number.");

        public static Error NameRequired =>
            Error.Validation("User.Name.Required", " name is required.");
    }
}
