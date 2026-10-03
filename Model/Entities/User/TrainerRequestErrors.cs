using GymAssistant_API.Model.Results;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Model.Entities.User
{
    public static class TrainerRequestErrors
    {
        public static Error SameTrainerAndTrainee =>
            Error.Validation(LocalizationKeys.Trainer.SameTrainerAndTrainee);

        public static Error RequestNotPending =>
            Error.Validation(LocalizationKeys.Trainer.RequestNotPending);

        public static Error RequestAlreadyExists =>
            Error.Conflict(LocalizationKeys.Trainer.RequestAlreadyExists);

        public static Error RelationshipAlreadyExists =>
            Error.Conflict(LocalizationKeys.Trainer.RelationshipAlreadyExists);

        public static Error RequestNotFound =>
            Error.NotFound(LocalizationKeys.Trainer.RequestNotFound);

        public static Error UnauthorizedAccess =>
            Error.Unauthorized(LocalizationKeys.Trainer.UnauthorizedAccess);

        public static Error TrainerNotFound =>
            Error.NotFound(LocalizationKeys.Trainer.NotFound);

        public static Error TraineeNotFound =>
            Error.NotFound(LocalizationKeys.Trainer.TraineeNotFound);

        public static Error NoRelationship =>
            Error.NotFound(LocalizationKeys.Trainer.NoRelationship);
    }
}
