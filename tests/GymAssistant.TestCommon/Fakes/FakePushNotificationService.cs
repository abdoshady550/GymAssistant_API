using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Res;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Notifications;
using Microsoft.AspNetCore.Http;

namespace GymAssistant.TestCommon.Fakes;

public class FakePushNotificationService : IPushNotificationService
{
    public List<Notification> Notifications { get; } = new();

    public Task<Result<DeviceTokenResponse>> RegisterDeviceTokenAsync(
        string userId,
        string token,
        DevicePlatform platform,
        CancellationToken ct = default)
    {
        var response = new DeviceTokenResponse(
            Guid.NewGuid(),
            token,
            platform,
            true,
            DateTimeOffset.UtcNow
        );
        return Task.FromResult<Result<DeviceTokenResponse>>(response);
    }

    public Task<Result<Updated>> UnregisterDeviceTokenAsync(
        string userId,
        string token,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<Updated>>(Result.Updated);
    }

    public Task<Result<PushNotificationResult>> SendNotificationAsync(
        string userId,
        string title,
        string body,
        NotificationType? type = NotificationType.General,
        Dictionary<string, string>? data = null,
        IFormFile? ImageFile = null,
        CancellationToken ct = default)
    {
        var result = new PushNotificationResult(true, "msg-1", null, 1, 0);
        return Task.FromResult<Result<PushNotificationResult>>(result);
    }

    public Task<Result<PushNotificationResult>> SendBulkNotificationAsync(
        List<string> userIds,
        string title,
        string body,
        NotificationType? type = NotificationType.General,
        Dictionary<string, string>? data = null,
        IFormFile? ImageFile = null,
        CancellationToken ct = default)
    {
        var result = new PushNotificationResult(true, "bulk-1", null, userIds.Count, 0);
        return Task.FromResult<Result<PushNotificationResult>>(result);
    }

    public Task<Result<PushNotificationResult>> SendToTokenAsync(
        string token,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        IFormFile? ImageFile = null,
        CancellationToken ct = default)
    {
        var result = new PushNotificationResult(true, "token-1", null, 1, 0);
        return Task.FromResult<Result<PushNotificationResult>>(result);
    }

    public Task<Result<List<NotificationResponse>>> GetUserNotificationsAsync(
        string userId,
        int pageSize = 20,
        int pageNumber = 1,
        bool unreadOnly = false,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<List<NotificationResponse>>>(new List<NotificationResponse>());
    }

    public Task<Result<Updated>> MarkAsReadAsync(
        string userId,
        Guid notificationId,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<Updated>>(Result.Updated);
    }

    public Task<Result<Updated>> MarkAllAsReadAsync(
        string userId,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<Updated>>(Result.Updated);
    }

    public Task<Result<Deleted>> DeleteNotificationAsync(
        string userId,
        Guid notificationId,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<Deleted>>(Result.Deleted);
    }

    public Task<Result<int>> GetUnreadCountAsync(
        string userId,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<int>>(0);
    }

    public Task<Result<string>> SubscribeToTopicAsync(
        string deviceToken,
        string topic,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<string>>("Subscribed");
    }

    public Task<Result<string>> UnsubscribeFromTopicAsync(
        string deviceToken,
        string topic,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<string>>("Unsubscribed");
    }

    public Task<Result<string>> SendToTopicAsync(
        string topic,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        IFormFile? ImageFile = null,
        CancellationToken ct = default)
    {
        return Task.FromResult<Result<string>>("Sent");
    }
}
