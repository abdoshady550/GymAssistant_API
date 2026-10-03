namespace GymAssistant_API.Resources;

public static class LocalizationKeys
{
    public static class Common
    {
        public const string ValidationTitle = "Common.ValidationTitle";
        public const string UnauthorizedTitle = "Common.UnauthorizedTitle";
        public const string NotFoundTitle = "Common.NotFoundTitle";
        public const string ConflictTitle = "Common.ConflictTitle";
        public const string ServerErrorTitle = "Common.ServerErrorTitle";
        public const string UnexpectedError = "Common.UnexpectedError";
        public const string InvalidPagination = "Common.InvalidPagination";
        public const string InvalidRequest = "Common.InvalidRequest";
    }

    public static class Auth
    {
        public const string InvalidCredentials = "Auth.InvalidCredentials";
        public const string EmailNotConfirmed = "Auth.EmailNotConfirmed";
        public const string EmailAlreadyExists = "Auth.EmailAlreadyExists";
        public const string ExpiredAccessTokenInvalid = "Auth.ExpiredAccessToken.Invalid";
        public const string UserIdClaimInvalid = "Auth.UserIdClaim.Invalid";
        public const string RefreshTokenExpired = "Auth.RefreshToken.Expired";
        public const string UserNotFound = "Auth.User.NotFound";
        public const string TokenGenerationFailed = "Auth.TokenGeneration.Failed";
        public const string PasswordsDoNotMatch = "Auth.PasswordsDoNotMatch";
        public const string InvalidCurrentPassword = "Auth.InvalidCurrentPassword";
        public const string SamePassword = "Auth.SamePassword";
        public const string ChangePasswordFailed = "Auth.ChangePasswordFailed";
        public const string PasswordResetEmailSent = "Auth.PasswordResetEmailSent";
        public const string ResetFailed = "Auth.ResetFailed";
        public const string TokenExpired = "Auth.TokenExpired";
        public const string TokenUsed = "Auth.TokenUsed";
        public const string InvalidToken = "Auth.InvalidToken";
        public const string ExternalLoginFailed = "Auth.ExternalLoginFailed";
        public const string ExternalSignInFailed = "Auth.ExternalSignInFailed";
        public const string AddExternalLoginFailed = "Auth.AddExternalLoginFailed";
        public const string InvalidProvider = "Auth.InvalidProvider";
        public const string RegistrationFailed = "Auth.RegistrationFailed";
        public const string UserHasNoRole = "Auth.UserHasNoRole";
        public const string InvalidRole = "Auth.InvalidRole";
        public const string RoleRequired = "Auth.RoleRequired";
        public const string InvalidEmail = "Auth.InvalidEmail";
        public const string InvalidPhoneNumber = "Auth.InvalidPhoneNumber";
    }

    public static class RefreshToken
    {
        public const string IdRequired = "RefreshToken.Id.Required";
        public const string TokenRequired = "RefreshToken.Token.Required";
        public const string UserIdRequired = "RefreshToken.UserId.Required";
        public const string ExpiryInvalid = "RefreshToken.Expiry.Invalid";
    }

    public static class User
    {
        public const string NotFound = "User.NotFound";
        public const string IdRequired = "User.Id.Required";
        public const string FirstNameRequired = "User.FirstName.Required";
        public const string LastNameRequired = "User.LastName.Required";
        public const string NameRequired = "User.Name.Required";
        public const string GenderInvalid = "User.Gender.Invalid";
        public const string BirthDayRequired = "User.BirthDay.Required";
        public const string HeightInvalid = "User.Height.Invalid";
        public const string WeightInvalid = "User.Weight.Invalid";
        public const string BodyFatPercentInvalid = "User.BodyFatPercent.Invalid";
        public const string MuscleMassKgInvalid = "User.MuscleMass.Invalid";
        public const string RoleInvalid = "User.Role.Invalid";
        public const string DeleteFailed = "User.Delete.Failed";
    }

    public static class Profile
    {
        public const string NotFound = "Profile.NotFound";
        public const string AlreadyExists = "Profile.AlreadyExists";
        public const string IdRequired = "Profile.Id.Required";
        public const string MeasurementNotFound = "Profile.Measurement.NotFound";
    }

    public static class Section
    {
        public const string NotFound = "Section.NotFound";
        public const string IdRequired = "Section.Id.Required";
        public const string NameRequired = "Section.Name.Required";
        public const string GroupNotFound = "Section.Group.NotFound";
    }

    public static class Exercise
    {
        public const string NotFound = "Exercise.NotFound";
        public const string CustomExerciseNotFound = "Exercise.CustomExercise.NotFound";
        public const string WorkoutExerciseNotFound = "Exercise.WorkoutExercise.NotFound";
        public const string Required = "Exercise.Required";
        public const string Conflict = "Exercise.Conflict";
        public const string InUse = "Exercise.InUse";
        public const string NameRequired = "Exercise.Name.Required";
        public const string AlreadyInGroup = "Exercise.AlreadyInGroup";
        public const string AlreadyInWorkout = "Exercise.AlreadyInWorkout";
        public const string DefaultSetsInvalid = "Exercise.DefaultSets.Invalid";
        public const string DefaultRepsInvalid = "Exercise.DefaultReps.Invalid";
        public const string SetNumberInvalid = "Exercise.SetNumber.Invalid";
        public const string RepsInvalid = "Exercise.Reps.Invalid";
        public const string WeightKgInvalid = "Exercise.WeightKg.Invalid";
        public const string RestTimeSecondsInvalid = "Exercise.RestTimeSeconds.Invalid";
        public const string SetNotFound = "Exercise.Set.NotFound";
        public const string IdRequired = "Exercise.Id.Required";
        public const string WorkoutExerciseIdRequired = "Exercise.WorkoutExerciseId.Required";
        public const string ValueInvalid = "Exercise.Value.Invalid";
        public const string CreatedByTrainerIdInvalid = "Exercise.CreatedByTrainerId.Invalid";
        public const string DateRequired = "Exercise.Date.Required";
    }

    public static class Workout
    {
        public const string SessionNotFound = "Workout.Session.NotFound";
        public const string SessionAlreadyCompleted = "Workout.Session.AlreadyCompleted";
        public const string SessionCompleted = "Workout.Session.Completed";
        public const string SessionNotStarted = "Workout.Session.NotStarted";
    }

    public static class Progress
    {
        public const string UnknownExercise = "Progress.UnknownExercise";
        public const string CustomExercises = "Progress.CustomExercises";
    }

    public static class Records
    {
        public static class Milestone
        {
            public const string CenturyClubTitle = "Records.Milestone.CenturyClub.Title";
            public const string CenturyClubDescription = "Records.Milestone.CenturyClub.Description";
            public const string VolumeBeastTitle = "Records.Milestone.VolumeBeast.Title";
            public const string VolumeBeastDescription = "Records.Milestone.VolumeBeast.Description";
            public const string RepsBreakerTitle = "Records.Milestone.RepsBreaker.Title";
            public const string RepsBreakerDescription = "Records.Milestone.RepsBreaker.Description";
        }
    }

    public static class Trainer
    {
        public const string NotFound = "Trainer.NotFound";
        public const string TraineeNotFound = "Trainer.Trainee.NotFound";
        public const string SameTrainerAndTrainee = "Trainer.Request.SameUser";
        public const string RequestNotPending = "Trainer.Request.NotPending";
        public const string RequestAlreadyExists = "Trainer.Request.AlreadyExists";
        public const string RelationshipAlreadyExists = "Trainer.Relationship.AlreadyExists";
        public const string RequestNotFound = "Trainer.Request.NotFound";
        public const string UnauthorizedAccess = "Trainer.Unauthorized";
        public const string NoRelationship = "Trainer.NoRelationship";
        public const string TrainerIdRequired = "Trainer.TrainerId.Required";
        public const string TraineeIdRequired = "Trainer.TraineeId.Required";
    }

    public static class Chat
    {
        public const string ConversationNotFound = "Chat.Conversation.NotFound";
        public const string ConversationIdRequired = "Chat.ConversationId.Required";
        public const string FailedToGetConversations = "Chat.FailedToGetConversations";
        public const string FailedToGetOrCreateConversation = "Chat.FailedToGetOrCreateConversation";
        public const string MessageNotFound = "Chat.Message.NotFound";
        public const string ContentRequired = "Chat.Content.Required";
        public const string SenderIdRequired = "Chat.SenderId.Required";
        public const string AttachmentRequired = "Chat.Attachment.Required";
        public const string CannotEdit = "Chat.CannotEdit";
        public const string SendMessageFailed = "Chat.SendMessageFailed";
        public const string MessagesError = "Chat.MessagesError";
        public const string UnreadCountError = "Chat.UnreadCountError";
        public const string UserNotAuthenticated = "Chat.Hub.UserNotAuthenticated";
        public const string UserProfileNotFound = "Chat.Hub.UserProfileNotFound";
        public const string NotPartOfConversation = "Chat.Hub.NotPartOfConversation";
        public const string MarkReadFailed = "Chat.Hub.MarkReadFailed";
        public const string ConversationMarkReadFailed = "Chat.Hub.ConversationMarkReadFailed";
        public const string EditMessageFailed = "Chat.Hub.EditMessageFailed";
        public const string NotSenderOrMessageNotFound = "Chat.Hub.NotSenderOrNotFound";
        public const string DeleteMessageFailed = "Chat.Hub.DeleteMessageFailed";
    }

    public static class Notification
    {
        public const string NotFound = "Notification.NotFound";
        public const string DeleteFailed = "Notification.DeleteFailed";
        public const string SendFailed = "Notification.SendFailed";
        public const string UpdateFailed = "Notification.UpdateFailed";
        public const string RetrievalFailed = "Notification.RetrievalFailed";
        public const string UnreadCountFailed = "Notification.UnreadCountFailed";
        public const string DeviceTokenNotFound = "Notification.DeviceToken.NotFound";
        public const string DeviceTokenRegistrationFailed = "Notification.DeviceToken.RegistrationFailed";
        public const string DeviceTokenUnregistrationFailed = "Notification.DeviceToken.UnregistrationFailed";
        public const string DeviceTokenRequired = "Notification.DeviceToken.Required";
        public const string SubscriptionFailed = "Notification.SubscriptionFailed";
        public const string UnsubscriptionFailed = "Notification.UnsubscriptionFailed";
        public const string BulkSendFailed = "Notification.BulkSendFailed";
        public const string TitleRequired = "Notification.Title.Required";
        public const string BodyRequired = "Notification.Body.Required";
        public const string InvalidTitle = "Notification.InvalidTitle";
        public const string InvalidBody = "Notification.InvalidBody";
        public const string InvalidTopic = "Notification.InvalidTopic";

        public static class Push
        {
            public const string TrainingRequestTitle = "Notification.Push.TrainingRequest.Title";
            public const string TrainingRequestBody = "Notification.Push.TrainingRequest.Body";
            public const string RequestAcceptedTitle = "Notification.Push.RequestAccepted.Title";
            public const string RequestAcceptedBody = "Notification.Push.RequestAccepted.Body";
            public const string RequestRejectedTitle = "Notification.Push.RequestRejected.Title";
            public const string RequestRejectedBody = "Notification.Push.RequestRejected.Body";
            public const string WorkoutCompletedTitle = "Notification.Push.WorkoutCompleted.Title";
            public const string WorkoutCompletedBody = "Notification.Push.WorkoutCompleted.Body";
            public const string PersonalRecordTitle = "Notification.Push.PersonalRecord.Title";
            public const string PersonalRecordBody = "Notification.Push.PersonalRecord.Body";
            public const string TrainerMessageTitle = "Notification.Push.TrainerMessage.Title";
            public const string NewWorkoutAssignedTitle = "Notification.Push.NewWorkoutAssigned.Title";
            public const string NewWorkoutAssignedBody = "Notification.Push.NewWorkoutAssigned.Body";
            public const string AchievementTitle = "Notification.Push.Achievement.Title";
            public const string AchievementBody = "Notification.Push.Achievement.Body";
            public const string WorkoutReminderTitle = "Notification.Push.WorkoutReminder.Title";
            public const string WorkoutReminderBody = "Notification.Push.WorkoutReminder.Body";
            public const string MeasurementReminderTitle = "Notification.Push.MeasurementReminder.Title";
            public const string MeasurementReminderBody = "Notification.Push.MeasurementReminder.Body";
            public const string InactivityReminderTitle = "Notification.Push.InactivityReminder.Title";
            public const string InactivityReminderBody = "Notification.Push.InactivityReminder.Body";
        }
    }

    public static class Identity
    {
        public const string Prefix = "Identity.";
    }

    public static class Validation
    {
        public const string Required = "Validation.Required";
        public const string Email = "Validation.Email";
        public const string MinLength = "Validation.MinLength";
        public const string Range = "Validation.Range";
        public const string Phone = "Validation.Phone";
        public const string Compare = "Validation.Compare";
        public const string InvalidValue = "Validation.InvalidValue";
    }

    public static class Fields
    {
        public const string UserName = "Fields.UserName";
        public const string Email = "Fields.Email";
        public const string Password = "Fields.Password";
        public const string ConfirmPassword = "Fields.ConfirmPassword";
        public const string CurrentPassword = "Fields.CurrentPassword";
        public const string NewPassword = "Fields.NewPassword";
        public const string PhoneNumber = "Fields.PhoneNumber";
        public const string Role = "Fields.Role";
        public const string FirstName = "Fields.FirstName";
        public const string LastName = "Fields.LastName";
        public const string BirthDate = "Fields.BirthDate";
        public const string Gender = "Fields.Gender";
        public const string Height = "Fields.Height";
        public const string Weight = "Fields.Weight";
        public const string BodyFat = "Fields.BodyFat";
        public const string MuscleMass = "Fields.MuscleMass";
        public const string Name = "Fields.Name";
        public const string Token = "Fields.Token";
        public const string RefreshToken = "Fields.RefreshToken";
        public const string DeviceToken = "Fields.DeviceToken";
        public const string Topic = "Fields.Topic";
        public const string Target = "Fields.Target";
    }

    public static class Data
    {
        public static class Section
        {
            public const string ChestName = "Data.Section.Chest.Name";
            public const string ChestDesc = "Data.Section.Chest.Desc";
            public const string BackName = "Data.Section.Back.Name";
            public const string BackDesc = "Data.Section.Back.Desc";
            public const string LegsName = "Data.Section.Legs.Name";
            public const string LegsDesc = "Data.Section.Legs.Desc";
            public const string ShouldersName = "Data.Section.Shoulders.Name";
            public const string ShouldersDesc = "Data.Section.Shoulders.Desc";
            public const string ArmsName = "Data.Section.Arms.Name";
            public const string ArmsDesc = "Data.Section.Arms.Desc";
        }

        public static class Exercise
        {
            public const string BenchPressName = "Data.Exercise.BenchPress.Name";
            public const string BenchPressDesc = "Data.Exercise.BenchPress.Desc";
            public const string BenchPressInst = "Data.Exercise.BenchPress.Inst";
            public const string BenchPressEquip = "Data.Exercise.BenchPress.Equip";

            public const string DeadliftName = "Data.Exercise.Deadlift.Name";
            public const string DeadliftDesc = "Data.Exercise.Deadlift.Desc";
            public const string DeadliftInst = "Data.Exercise.Deadlift.Inst";
            public const string DeadliftEquip = "Data.Exercise.Deadlift.Equip";

            public const string SquatName = "Data.Exercise.Squat.Name";
            public const string SquatDesc = "Data.Exercise.Squat.Desc";
            public const string SquatInst = "Data.Exercise.Squat.Inst";
            public const string SquatEquip = "Data.Exercise.Squat.Equip";

            public const string PullUpName = "Data.Exercise.PullUp.Name";
            public const string PullUpDesc = "Data.Exercise.PullUp.Desc";
            public const string PullUpInst = "Data.Exercise.PullUp.Inst";
            public const string PullUpEquip = "Data.Exercise.PullUp.Equip";

            public const string ShoulderPressName = "Data.Exercise.ShoulderPress.Name";
            public const string ShoulderPressDesc = "Data.Exercise.ShoulderPress.Desc";
            public const string ShoulderPressInst = "Data.Exercise.ShoulderPress.Inst";
            public const string ShoulderPressEquip = "Data.Exercise.ShoulderPress.Equip";

            public const string BicepCurlName = "Data.Exercise.BicepCurl.Name";
            public const string BicepCurlDesc = "Data.Exercise.BicepCurl.Desc";
            public const string BicepCurlInst = "Data.Exercise.BicepCurl.Inst";
            public const string BicepCurlEquip = "Data.Exercise.BicepCurl.Equip";
        }
    }
}
