using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Repository.Interfaces.Notifications;
using static System.Net.Mime.MediaTypeNames;

namespace GymAssistant_API.Extensions
{
    public static class NotificationHelpers
    {
        private static IFormFile ConvertToIFormFile(string imagePath)
        {
            var fileName = Path.GetFileName(imagePath);
            var bytes = File.ReadAllBytes(imagePath);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
        /// <summary>
        /// إشعار عند إرسال المستخدم طلب تدريب لمدرب
        /// </summary>
        public static async Task SendTrainingRequestNotification(
            this IPushNotificationService service,
            string userId,
            string Name,
            string img,
            CancellationToken ct = default)
        {
            IFormFile imgage = null;
            if (!string.IsNullOrEmpty(img))
            {
                imgage = ConvertToIFormFile(img);
            }
            await service.SendNotificationAsync(
                userId,
                "New Training Request 💪",
                $"{Name} sent you a training request!",
                NotificationType.TrainerRequest,
                new Dictionary<string, string>
                {
                    { "trainee_name", Name },
                    { "action", "open_requests" }
                },
                imgage,
                ct
            );
        }
        /// <summary>
        /// إشعار عند قبول المدرب لطلب المستخدم
        /// </summary>
        public static async Task SendAcceptedRequestNotification(
            this IPushNotificationService service,
            string userId,
            string Name,
            string img,
            CancellationToken ct = default)
        {
            IFormFile imgage = null;
            if (!string.IsNullOrEmpty(img))
            {
                imgage = ConvertToIFormFile(img);
            }
            await service.SendNotificationAsync(
                userId,
                "Request Accepted ✅",
                $"{Name} accepted your training request! Welcome to the team!",
                NotificationType.TrainerRequest,
                new Dictionary<string, string>
                {
                    { "trainer_name", Name },
                    { "action", "view_profile" }
                },
                imgage,
                ct
            );
        }
        /// <summary>
        /// إشعار عند رفض المدرب لطلب المستخدم
        /// </summary>
        public static async Task SendRejectedRequestNotification(
            this IPushNotificationService service,
            string userId,
            string Name,
            string img,
            CancellationToken ct = default)
        {
            IFormFile imgage = null;
            if (!string.IsNullOrEmpty(img))
            {
                imgage = ConvertToIFormFile(img);
            }
            await service.SendNotificationAsync(
                userId,
                "Request Rejected ❌",
                $"{Name} has declined your training request.",
                NotificationType.TrainerRequest,
                new Dictionary<string, string>
                {
                    { "trainer_name", Name },
                    { "action", "open_requests" }
                },
                imgage,
                ct
            );
        }

        /// <summary>
        /// Send workout completed notification
        /// </summary>
        public static async Task SendWorkoutCompletedNotification(
            this IPushNotificationService service,
            string userId,
            string workoutName,
            int exercisesCompleted,
            CancellationToken ct = default)
        {
            await service.SendNotificationAsync(
                userId,
                "Workout Completed! 🎉",
                $"Great job! You completed {workoutName} with {exercisesCompleted} exercises.",
                NotificationType.WorkoutCompleted,
                new Dictionary<string, string>
                {
                    { "workout_name", workoutName },
                    { "exercises_count", exercisesCompleted.ToString() }
                }, null,
                ct
            );
        }

        /// <summary>
        /// Send personal record notification
        /// </summary>
        public static async Task SendPersonalRecordNotification(
            this IPushNotificationService service,
            string userId,
            string exerciseName,
            string achievement,
            CancellationToken ct = default)
        {
            await service.SendNotificationAsync(
                userId,
                "New Personal Record! 🏆",
                $"Congratulations! New PR in {exerciseName}: {achievement}",
                NotificationType.PersonalRecord,
                new Dictionary<string, string>
                {
                    { "exercise", exerciseName },
                    { "achievement", achievement }
                }, null,
                ct
            );
        }

        /// <summary>
        /// Send trainer message notification
        /// </summary>
        public static async Task SendTrainerMessageNotification(
            this IPushNotificationService service,
            string userId,
            string trainerName,
            string messagePreview,
            CancellationToken ct = default)
        {
            await service.SendNotificationAsync(
                userId,
                $"Message from {trainerName}",
                messagePreview,
                NotificationType.TrainerMessage,
                new Dictionary<string, string>
                {
                    { "trainer_name", trainerName },
                    { "action", "open_chat" }
                },
                null, ct
            );
        }

        /// <summary>
        /// Send new workout assigned notification
        /// </summary>
        public static async Task SendNewWorkoutAssignedNotification(
            this IPushNotificationService service,
            string userId,
            string workoutName,
            string trainerName,
            CancellationToken ct = default)
        {
            await service.SendNotificationAsync(
                userId,
                "New Workout Assigned! 💪",
                $"{trainerName} assigned you a new workout: {workoutName}",
                NotificationType.NewWorkoutAssigned,
                new Dictionary<string, string>
                {
                    { "workout_name", workoutName },
                    { "trainer_name", trainerName },
                    { "action", "open_workout" }
                },
                null, ct
            );
        }

        /// <summary>
        /// Send trainer request notification
        /// </summary>
        public static async Task SendTrainerRequestNotification(
            this IPushNotificationService service,
            string trainerId,
            string userName,
            CancellationToken ct = default)
        {
            await service.SendNotificationAsync(
                trainerId,
                "New Training Request",
                $"{userName} wants you to be their trainer",
                NotificationType.TrainerRequest,
                new Dictionary<string, string>
                {
                    { "user_name", userName },
                    { "action", "open_requests" }
                }, null,
                ct
            );
        }

        /// <summary>
        /// Send achievement notification
        /// </summary>
        public static async Task SendAchievementNotification(
            this IPushNotificationService service,
            string userId,
            string achievementTitle,
            string achievementDescription,
            CancellationToken ct = default)
        {
            await service.SendNotificationAsync(
                userId,
                $"Achievement Unlocked! 🏅",
                $"{achievementTitle}: {achievementDescription}",
                NotificationType.Achievement,
                new Dictionary<string, string>
                {
                    { "achievement_title", achievementTitle },
                    { "achievement_description", achievementDescription }
                },
                null, ct
            );
        }

        /// <summary>
        /// Get notification icon based on type
        /// </summary>
        public static string GetNotificationIcon(NotificationType type)
        {
            return type switch
            {
                NotificationType.WorkoutReminder => "🔔",
                NotificationType.WorkoutCompleted => "✅",
                NotificationType.PersonalRecord => "🏆",
                NotificationType.TrainerMessage => "💬",
                NotificationType.TrainerRequest => "👤",
                NotificationType.NewWorkoutAssigned => "💪",
                NotificationType.MeasurementReminder => "📏",
                NotificationType.Achievement => "🏅",
                NotificationType.General => "📢",
                _ => "🔔"
            };
        }

        /// <summary>
        /// Get notification color based on type (for mobile app)
        /// </summary>
        public static string GetNotificationColor(NotificationType type)
        {
            return type switch
            {
                NotificationType.WorkoutReminder => "#FF6B6B",
                NotificationType.WorkoutCompleted => "#51CF66",
                NotificationType.PersonalRecord => "#FFD43B",
                NotificationType.TrainerMessage => "#339AF0",
                NotificationType.TrainerRequest => "#845EF7",
                NotificationType.NewWorkoutAssigned => "#FF8787",
                NotificationType.MeasurementReminder => "#74C0FC",
                NotificationType.Achievement => "#FFD43B",
                NotificationType.General => "#ADB5BD",
                _ => "#ADB5BD"
            };
        }
    }
}
