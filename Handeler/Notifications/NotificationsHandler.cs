using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Res;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Repository.Interfaces.Notifications;

namespace GymAssistant_API.Handeler.Notifications
{
    public sealed class NotificationsHandler(ILogger<NotificationsHandler> logger,
                                             IPushNotificationService pushNotification)
    {
        private readonly ILogger<NotificationsHandler> _logger = logger;
        private readonly IPushNotificationService _pushNotification = pushNotification;
        public async Task<Result<DeviceTokenResponse>> RegisterDevice(string userId, string token, DevicePlatform platform, CancellationToken ct)
        {
            var result = await _pushNotification.RegisterDeviceTokenAsync(userId, token, platform, ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to register device token for user {UserId}: {TopError}", userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Device token registered successfully for user {UserId}", userId);
            return result.Value;
        }
        public async Task<Result<Updated>> UnregisterDevice(string userId, string token, CancellationToken ct)
        {
            var result = await _pushNotification.UnregisterDeviceTokenAsync(userId, token, ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to unregister device token for user {UserId}: {TopError}", userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Device token unregistered successfully for user {UserId}", userId);
            return Result.Updated;
        }
        public async Task<Result<PushNotificationResult>> SendNotification(string userId,
                                                                                 string title,
                                                                                 string body,
                                                                                 NotificationType type = NotificationType.General,
                                                                                 Dictionary<string, string>? data = null,
                                                                                 IFormFile? image = null,
                                                                                 CancellationToken ct = default)
        {
            var result = await _pushNotification.SendNotificationAsync(userId,
                                                                       title,
                                                                       body,
                                                                       type,
                                                                       data,
                                                                       image,
                                                                       ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to send notification to user {UserId}: {TopError}", userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Notification sent successfully to user {UserId}", userId);
            return result.Value;
        }
        public async Task<Result<PushNotificationResult>> SendBulkNotification(List<string> userIds,
                                                                                     string title,
                                                                                     string body,
                                                                                     NotificationType type = NotificationType.General,
                                                                                     Dictionary<string, string>? data = null,
                                                                                     IFormFile? image = null,
                                                                                     CancellationToken ct = default)
        {
            var result = await _pushNotification.SendBulkNotificationAsync(userIds,
                                                                           title,
                                                                           body,
                                                                           type,
                                                                           data,
                                                                           image,
                                                                           ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to send bulk notification: {TopError}", result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Bulk notification sent successfully to {UserCount} users", userIds.Count);
            return result.Value;
        }
        public async Task<Result<PushNotificationResult>> SendToTokenAsync(string token,
                                                                           string title,
                                                                           string body,
                                                                           Dictionary<string, string>? data = null,
                                                                           CancellationToken ct = default)
        {
            var result = await _pushNotification.SendToTokenAsync(token,
                                                                   title,
                                                                   body,
                                                                   data,
                                                                   ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to send notification to token {Token}: {TopError}", token, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Notification sent successfully to token {Token}", token);
            return result.Value;
        }
        public async Task<Result<List<NotificationResponse>>> GetMyNotifications(
            string userId,
            int pageSize = 20,
            int pageNumber = 1,
            bool unreadOnly = false,
            CancellationToken ct = default)
        {
            var result = await _pushNotification.GetUserNotificationsAsync(userId,
                                                                           pageSize,
                                                                           pageNumber,
                                                                           unreadOnly,
                                                                           ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to retrieve notifications for user {UserId}: {TopError}", userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Retrieved notifications for user {UserId}", userId);
            return result.Value;

        }
        public async Task<Result<int>> GetUnreadCount(
            string userId,
            CancellationToken ct = default)
        {
            var result = await _pushNotification.GetUnreadCountAsync(userId, ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to retrieve unread notification count for user {UserId}: {TopError}", userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Retrieved unread notification count for user {UserId}", userId);
            return result.Value;
        }
        public async Task<Result<Updated>> MarkAsRead(
            string userId,
            Guid notificationId,
            CancellationToken ct = default)
        {
            var result = await _pushNotification.MarkAsReadAsync(userId, notificationId, ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to mark notification {NotificationId} as read for user {UserId}: {TopError}", notificationId, userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Marked notification {NotificationId} as read for user {UserId}", notificationId, userId);
            return Result.Updated;
        }
        public async Task<Result<Updated>> MarkAllAsRead(
            string userId,
            CancellationToken ct = default)
        {
            var result = await _pushNotification.MarkAllAsReadAsync(userId, ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to mark all notifications as read for user {UserId}: {TopError}", userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Marked all notifications as read for user {UserId}", userId);
            return Result.Updated;
        }
        public async Task<Result<Deleted>> DeleteNotification(
            string userId,
            Guid notificationId,
            CancellationToken ct = default)
        {
            var result = await _pushNotification.DeleteNotificationAsync(userId, notificationId, ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to delete notification {NotificationId} for user {UserId}: {TopError}", notificationId, userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Deleted notification {NotificationId} for user {UserId}", notificationId, userId);
            return Result.Deleted;
        }

    }

}
