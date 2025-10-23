using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Res;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Repository.Interfaces.Notifications
{
    /// <summary>
    /// Push Notification Service Interface
    /// </summary>
    public interface IPushNotificationService
    {
        /// <summary>
        /// Register device token for push notifications
        /// </summary>
        Task<Result<DeviceTokenResponse>> RegisterDeviceTokenAsync(
            string userId,
            string token,
            DevicePlatform platform,
            CancellationToken ct = default);

        /// <summary>
        /// Unregister device token
        /// </summary>
        Task<Result<Updated>> UnregisterDeviceTokenAsync(
            string userId,
            string token,
            CancellationToken ct = default);

        /// <summary>
        /// Send push notification to a single user
        /// </summary>
        Task<Result<PushNotificationResult>> SendNotificationAsync(
            string userId,
            string title,
            string body,
            NotificationType type = NotificationType.General,
            Dictionary<string, string>? data = null,
            IFormFile? ImageFile = null,
            CancellationToken ct = default);

        /// <summary>
        /// Send push notification to multiple users
        /// </summary>
        Task<Result<PushNotificationResult>> SendBulkNotificationAsync(
            List<string> userIds,
            string title,
            string body,
            NotificationType type = NotificationType.General,
            Dictionary<string, string>? data = null,
            IFormFile? ImageFile = null,
            CancellationToken ct = default);

        /// <summary>
        /// Send notification to specific device token
        /// </summary>
        Task<Result<PushNotificationResult>> SendToTokenAsync(
            string token,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            IFormFile? ImageFile = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get user notifications history
        /// </summary>
        Task<Result<List<NotificationResponse>>> GetUserNotificationsAsync(
            string userId,
            int pageSize = 20,
            int pageNumber = 1,
            bool unreadOnly = false,
            CancellationToken ct = default);

        /// <summary>
        /// Mark notification as read
        /// </summary>
        Task<Result<Updated>> MarkAsReadAsync(
            string userId,
            Guid notificationId,
            CancellationToken ct = default);

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        Task<Result<Updated>> MarkAllAsReadAsync(
            string userId,
            CancellationToken ct = default);

        /// <summary>
        /// Delete notification
        /// </summary>
        Task<Result<Deleted>> DeleteNotificationAsync(
            string userId,
            Guid notificationId,
            CancellationToken ct = default);

        /// <summary>
        /// Get unread notifications count
        /// </summary>
        Task<Result<int>> GetUnreadCountAsync(
            string userId,
            CancellationToken ct = default);

        /// <summary>
        /// Subscribe a device token to a topic
        /// </summary>
        Task<Result<string>> SubscribeToTopicAsync(
            string deviceToken,
            string topic,
            CancellationToken ct = default);

        /// <summary>
        /// Unsubscribe a device token from a topic
        /// </summary>
        Task<Result<string>> UnsubscribeFromTopicAsync(
            string deviceToken,
            string topic,
            CancellationToken ct = default);

        /// <summary>
        /// Send notification to a specific topic
        /// </summary>
        Task<Result<string>> SendToTopicAsync(
            string topic,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            IFormFile? ImageFile = null,
            CancellationToken ct = default);
    }
}
