using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public static class ExerciseErrors
    {
        public static Error PersonalRecordValueISInvalid =>
            Error.Validation("Value.Invalid", "Record value must be a positive number.");

        public static Error PersonalRecordExerciseIsRequired =>
           Error.Validation("Exercise.Required", "Either ExerciseId or UserExerciseId must be provided.");

        public static Error PersonalRecordExerciseIsConflict =>
           Error.Validation("Exercise.Conflict", "Only one of ExerciseId or UserExerciseId should be provided.");
        public static Error GenderInvalid =>
            Error.Validation("GenderInvalid", "Gender must be a Male or Female.");
        public static Error BirthDayRequired =>
           Error.Validation("BirthDay.Required", "BirthDay not Valid.");
        public static Error HeightInvalid =>
            Error.Validation("HeightCm.Invalid", "HeightCm must be a positive integer.");
        public static Error RoleInvalid =>
            Error.Validation("User.Role.Invalid", "Invalid role assigned to User.");

        public static Error SectionIdRequired =>
            Error.Validation("SectionId.Required", "SectionId is required.");
        public static Error NameRequired =>
            Error.Validation("Name.Required", "Name is required.");
        public static Error DefaultSetsInvalid =>
            Error.Validation("DefaultSets.Invalid", "DefaultSets must be a positive integer.");
        public static Error DefaultRepsInvalid =>
            Error.Validation("DefaultReps.Invalid", "DefaultReps must be a positive integer.");

        public static Error SetNumberInvalid =>
            Error.Validation("SetNumber.Invalid", "SetNumber must be a positive integer.");
        public static Error RepsInvalid =>
            Error.Validation("Reps.Invalid", "Reps must be a positive integer.");
        public static Error WeightKgInvalid =>
            Error.Validation("WeightKg.Invalid", "WeightKg must be a non-negative number.");

        public static Error ClientProfileIdRequired =>
            Error.Validation("ClientProfileId.Required", "ClientProfileId is required.");
        public static Error DateRequired =>
            Error.Validation("Date.Required", "Date is required.");
        public static Error CreatedByTrainerIdInvalid =>
            Error.Validation("CreatedByTrainerId.Invalid", "CreatedByTrainerId must be a valid GUID if provided.");

        public static Error WorkoutExerciseIdRequired =>
            Error.Validation("WorkoutExerciseId.Required", "WorkoutExerciseId is required.");
        public static Error ExerciseIdRequired =>
            Error.Validation("ExerciseId.Required", "ExerciseId is required.");
        public static Error RestTimeSecondsInvalid =>
            Error.Validation("RestTimeSeconds.Invalid", "RestTimeSeconds must be a non-negative integer if provided.");
    }
}
