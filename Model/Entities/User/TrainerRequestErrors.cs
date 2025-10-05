using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.User
{
    public static class TrainerRequestErrors
    {
        public static Error SameTrainerAndTrainee =>
            Error.Validation("Request_SameUser", "Trainer and trainee cannot be the same person.");

        public static Error RequestNotPending =>
            Error.Validation("Request_NotPending", "Request is not in pending status.");

        public static Error RequestAlreadyExists =>
            Error.Conflict("Request_AlreadyExists", "A pending request already exists between these users.");

        public static Error RelationshipAlreadyExists =>
            Error.Conflict("Relationship_AlreadyExists", "Trainer-trainee relationship already exists.");

        public static Error RequestNotFound =>
            Error.NotFound("Request_NotFound", "Trainer request not found.");

        public static Error UnauthorizedAccess =>
            Error.Unauthorized("Request_Unauthorized", "You are not authorized to perform this action.");
    }
}
