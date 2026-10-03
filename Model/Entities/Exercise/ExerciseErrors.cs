using GymAssistant_API.Model.Results;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public static class ExerciseErrors
    {
        public static Error PersonalRecordValueISInvalid =>
            Error.Validation(LocalizationKeys.Exercise.ValueInvalid);

        public static Error PersonalRecordExerciseIsRequired =>
            Error.Validation(LocalizationKeys.Exercise.Required);

        public static Error PersonalRecordExerciseIsConflict =>
            Error.Validation(LocalizationKeys.Exercise.Conflict);

        public static Error GenderInvalid =>
            Error.Validation(LocalizationKeys.User.GenderInvalid);

        public static Error BirthDayRequired =>
            Error.Validation(LocalizationKeys.User.BirthDayRequired);

        public static Error HeightInvalid =>
            Error.Validation(LocalizationKeys.User.HeightInvalid);

        public static Error RoleInvalid =>
            Error.Validation(LocalizationKeys.User.RoleInvalid);

        public static Error SectionIdRequired =>
            Error.Validation(LocalizationKeys.Section.IdRequired);

        public static Error NameRequired =>
            Error.Validation(LocalizationKeys.Exercise.NameRequired);

        public static Error DefaultSetsInvalid =>
            Error.Validation(LocalizationKeys.Exercise.DefaultSetsInvalid);

        public static Error DefaultRepsInvalid =>
            Error.Validation(LocalizationKeys.Exercise.DefaultRepsInvalid);

        public static Error SetNumberInvalid =>
            Error.Validation(LocalizationKeys.Exercise.SetNumberInvalid);

        public static Error RepsInvalid =>
            Error.Validation(LocalizationKeys.Exercise.RepsInvalid);

        public static Error WeightKgInvalid =>
            Error.Validation(LocalizationKeys.Exercise.WeightKgInvalid);

        public static Error ClientProfileIdRequired =>
            Error.Validation(LocalizationKeys.Profile.IdRequired);

        public static Error DateRequired =>
            Error.Validation(LocalizationKeys.Exercise.DateRequired);

        public static Error CreatedByTrainerIdInvalid =>
            Error.Validation(LocalizationKeys.Exercise.CreatedByTrainerIdInvalid);

        public static Error WorkoutExerciseIdRequired =>
            Error.Validation(LocalizationKeys.Exercise.WorkoutExerciseIdRequired);

        public static Error ExerciseIdRequired =>
            Error.Validation(LocalizationKeys.Exercise.IdRequired);

        public static Error RestTimeSecondsInvalid =>
            Error.Validation(LocalizationKeys.Exercise.RestTimeSecondsInvalid);

        public static Error NotFound =>
            Error.NotFound(LocalizationKeys.Exercise.NotFound);

        public static Error CustomExerciseNotFound =>
            Error.NotFound(LocalizationKeys.Exercise.CustomExerciseNotFound);

        public static Error SectionNotFound =>
            Error.NotFound(LocalizationKeys.Section.NotFound);

        public static Error SectionGroupNotFound =>
            Error.NotFound(LocalizationKeys.Section.GroupNotFound);

        public static Error InUse =>
            Error.Validation(LocalizationKeys.Exercise.InUse);

        public static Error AlreadyInGroup =>
            Error.Conflict(LocalizationKeys.Exercise.AlreadyInGroup);

        public static Error SessionNotFound =>
            Error.NotFound(LocalizationKeys.Workout.SessionNotFound);

        public static Error SessionAlreadyCompleted =>
            Error.Validation(LocalizationKeys.Workout.SessionAlreadyCompleted);

        public static Error SessionCompleted =>
            Error.Validation(LocalizationKeys.Workout.SessionCompleted);

        public static Error SessionNotStarted =>
            Error.Validation(LocalizationKeys.Workout.SessionNotStarted);

        public static Error SetNotFound =>
            Error.NotFound(LocalizationKeys.Exercise.SetNotFound);
    }
}
