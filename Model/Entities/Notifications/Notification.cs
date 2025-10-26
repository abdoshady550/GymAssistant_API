using GymAssistant_API.Model.Results;
using static System.Net.Mime.MediaTypeNames;

namespace GymAssistant_API.Model.Entities.Notifications
{
    /// <summary>
    /// Notification Entity - stores notification history
    /// </summary>
    public sealed class Notification : Entity
    {
        public string UserId { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public NotificationType? Type { get; private set; } = NotificationType.General;
        public string? Data { get; private set; } // JSON data
        public string? Image { get; private set; }
        public bool IsRead { get; private set; } = false;
        public DateTimeOffset? ReadAtUtc { get; private set; }
        public bool IsSent { get; private set; } = false;
        public DateTimeOffset? SentAtUtc { get; private set; }

        private Notification() { }

        private Notification(Guid id, string userId, string title, string body,
                            NotificationType? type, string? data = null, string? image = null) : base(id)
        {
            UserId = userId;
            Title = title;
            Body = body;
            Type = type;
            Data = data;
            Image = image;
            CreatedAtUtc = DateTimeOffset.UtcNow;

        }

        public static Result<Notification> Create(Guid id, string userId, string title,
                                                 string body, NotificationType? type, string? data = null, string? image = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Error.Validation("UserId_Required", "User ID is required");

            if (string.IsNullOrWhiteSpace(title))
                return Error.Validation("Title_Required", "Title is required");

            if (string.IsNullOrWhiteSpace(body))
                return Error.Validation("Body_Required", "Body is required");

            return new Notification(id, userId, title, body, type, data, image);
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAtUtc = DateTimeOffset.UtcNow;
        }

        public void MarkAsSent()
        {
            IsSent = true;
            SentAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public enum NotificationType
    {
        WorkoutReminder = 1,
        WorkoutCompleted = 2,
        PersonalRecord = 3,
        TrainerMessage = 4,
        TrainerRequest = 5,
        NewWorkoutAssigned = 6,
        MeasurementReminder = 7,
        Achievement = 8,
        General = 9
    }
}
