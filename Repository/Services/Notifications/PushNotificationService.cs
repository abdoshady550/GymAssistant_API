using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Model.Entities.Notifications.Dtos.Res;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Notifications;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace GymAssistant_API.Repository.Services.Notifications
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PushNotificationService> _logger;
        private readonly FirebaseMessaging _messaging;
        private readonly IWebHostEnvironment _environment;


        public PushNotificationService(
            AppDbContext context,
            ILogger<PushNotificationService> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment
            )
        {
            _context = context;
            _logger = logger;
            _environment = environment;

            // Initialize Firebase Admin SDK
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    var credentialPath = Path.Combine(AppContext.BaseDirectory,
                        configuration["Firebase:CredentialPath"] ?? string.Empty);

                    if (string.IsNullOrEmpty(credentialPath))
                    {
                        _logger.LogWarning("Firebase credential path not configured");
                    }
                    else
                    {
                        FirebaseApp.Create(new AppOptions
                        {
                            Credential = GoogleCredential.FromFile(credentialPath)
                        });
                    }
                }

                _messaging = FirebaseMessaging.DefaultInstance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase Admin SDK");
            }
        }

        public async Task<Result<DeviceTokenResponse>> RegisterDeviceTokenAsync(
            string userId,
            string token,
            DevicePlatform platform,
            CancellationToken ct = default)
        {
            try
            {
                // Check if token already exists
                var existingToken = await _context.DeviceTokens
                    .FirstOrDefaultAsync(dt => dt.UserId == userId && dt.Token == token, ct);

                if (existingToken != null)
                {
                    existingToken.Activate();
                    existingToken.UpdateLastUsed();
                    await _context.SaveChangesAsync(ct);

                    return new DeviceTokenResponse(
                        existingToken.Id,
                        existingToken.Token,
                        existingToken.Platform,
                        existingToken.IsActive,
                        existingToken.LastUsedUtc
                    );
                }

                // Create new token
                var deviceTokenResult = DeviceToken.Create(Guid.NewGuid(), userId, token, platform);
                if (deviceTokenResult.IsError)
                    return deviceTokenResult.Errors;

                var deviceToken = deviceTokenResult.Value;
                _context.DeviceTokens.Add(deviceToken);
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Device token registered for user {UserId}", userId);

                return new DeviceTokenResponse(
                    deviceToken.Id,
                    deviceToken.Token,
                    deviceToken.Platform,
                    deviceToken.IsActive,
                    deviceToken.LastUsedUtc
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering device token for user {UserId}", userId);
                return Error.Failure("DeviceToken_Registration_Failed", "Failed to register device token");
            }
        }

        public async Task<Result<Updated>> UnregisterDeviceTokenAsync(
            string userId,
            string token,
            CancellationToken ct = default)
        {
            try
            {
                var deviceToken = await _context.DeviceTokens
                    .FirstOrDefaultAsync(dt => dt.UserId == userId && dt.Token == token, ct);

                if (deviceToken == null)
                    return Error.NotFound("DeviceToken_NotFound", "Device token not found");

                deviceToken.Deactivate();
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Device token unregistered for user {UserId}", userId);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering device token for user {UserId}", userId);
                return Error.Failure("DeviceToken_Unregistration_Failed", "Failed to unregister device token");
            }
        }

        public async Task<Result<PushNotificationResult>> SendNotificationAsync(
            string userId,
            string title,
            string body,
            NotificationType? type = NotificationType.General,
            Dictionary<string, string>? data = null,
            IFormFile? ImageFile = null,
            CancellationToken ct = default)
        {
            try
            {
                // 🖼️ حفظ الصورة في wwwroot (لو موجودة)
                string? imageUrl = null;
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "notification");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream, ct);
                    }

                    // 🔗 هنا حطينا الدومين يدوي
                    const string baseUrl = "https://gymassistantapi.runasp.net";
                    imageUrl = $"{baseUrl}/images/notification/{uniqueFileName}";
                }
                // Save notification to database
                var notificationResult = Model.Entities.Notifications.Notification.Create(
                    Guid.NewGuid(),
                    userId,
                    title,
                    body,
                    type,
                    data != null ? JsonSerializer.Serialize(data) : null,
                    imageUrl
                );

                if (notificationResult.IsError)
                    return notificationResult.Errors;

                var notification = notificationResult.Value;
                _context.Notifications.Add(notification);

                // Get user's active device tokens
                var deviceTokens = await _context.DeviceTokens
                    .Where(dt => dt.UserId == userId && dt.IsActive)
                    .ToListAsync(ct);

                if (!deviceTokens.Any())
                {
                    await _context.SaveChangesAsync(ct);
                    return new PushNotificationResult(false, Error: "No active device tokens found");
                }

                // Send to all user's devices
                var successCount = 0;
                var failureCount = 0;
                string? messageId = null;

                foreach (var deviceToken in deviceTokens)
                {
                    var result = await SendToTokenAsync(deviceToken.Token, title, body, data, ImageFile, ct);
                    if (result.IsSuccess && result.Value.Success)
                    {
                        successCount++;
                        messageId = result.Value.MessageId;
                        deviceToken.UpdateLastUsed();
                    }
                    else
                    {
                        failureCount++;
                        // Deactivate invalid tokens
                        if (result.Value.Error?.Contains("invalid") == true)
                        {
                            deviceToken.Deactivate();
                        }
                    }
                }

                notification.MarkAsSent();
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Notification sent to user {UserId}: Success={Success}, Failure={Failure}",
                    userId, successCount, failureCount);

                return new PushNotificationResult(
                    successCount > 0,
                    messageId,
                    failureCount > 0 ? $"{failureCount} devices failed" : null,
                    successCount,
                    failureCount
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
                return Error.Failure("Notification_Send_Failed", "Failed to send notification");
            }
        }

        public async Task<Result<PushNotificationResult>> SendBulkNotificationAsync(
            List<string> userIds,
            string title,
            string body,
            NotificationType? type = NotificationType.General,
            Dictionary<string, string>? data = null,
            IFormFile? ImageFile = null,
            CancellationToken ct = default)
        {
            try
            {
                var totalSuccess = 0;
                var totalFailure = 0;


                foreach (var userId in userIds)
                {
                    var result = await SendNotificationAsync(userId, title, body, type, data, ImageFile, ct);
                    if (result.IsSuccess)
                    {
                        totalSuccess += result.Value.SuccessCount;
                        totalFailure += result.Value.FailureCount;
                    }
                    else
                    {
                        totalFailure++;
                    }
                }

                _logger.LogInformation(
                    "Bulk notification sent: Success={Success}, Failure={Failure}",
                    totalSuccess, totalFailure);

                return new PushNotificationResult(
                    totalSuccess > 0,
                    SuccessCount: totalSuccess,
                    FailureCount: totalFailure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk notification");
                return Error.Failure("BulkNotification_Send_Failed", "Failed to send bulk notification");
            }
        }

        public async Task<Result<PushNotificationResult>> SendToTokenAsync(
            string token,
            string title,
            string body,
            Dictionary<string, string>? data = null,
            IFormFile? ImageFile = null,
            CancellationToken ct = default)
        {
            try
            {
                // 🖼️ حفظ الصورة في wwwroot (لو موجودة)
                string? imageUrl = null;
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "notification");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream, ct);
                    }

                    // 🔗 هنا حطينا الدومين يدوي
                    const string baseUrl = "https://gymassistantapi.runasp.net";
                    imageUrl = $"{baseUrl}/images/notification/{uniqueFileName}";
                }
                if (_messaging == null)
                {
                    return new PushNotificationResult(false, Error: "Firebase not initialized");
                }

                var message = new Message
                {
                    Token = token,
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = body,
                        ImageUrl = imageUrl
                    },
                    Data = data,
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = "default",
                            ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                            ImageUrl = imageUrl,
                            Color = "#FF6B6B"

                        }
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Sound = "default",
                            ContentAvailable = true
                        },
                        FcmOptions = new ApnsFcmOptions
                        {
                            ImageUrl = imageUrl
                        }
                    }
                };

                var response = await _messaging.SendAsync(message, ct);

                _logger.LogInformation("Successfully sent message: {MessageId}", response);

                return new PushNotificationResult(true, response);
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, "Firebase error sending to token: {Token}", token);
                return new PushNotificationResult(false, Error: ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending to token: {Token}", token);
                return new PushNotificationResult(false, Error: ex.Message);
            }
        }

        public async Task<Result<List<NotificationResponse>>> GetUserNotificationsAsync(
            string userId,
            int pageSize = 20,
            int pageNumber = 1,
            bool unreadOnly = false,
            CancellationToken ct = default)
        {
            try
            {
                var query = _context.Notifications
                    .Where(n => n.UserId == userId);

                if (unreadOnly)
                {
                    query = query.Where(n => !n.IsRead);
                }


                var notificationsData = await query
                    .OrderByDescending(n => n.CreatedAtUtc)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct);

                var notifications = notificationsData
                    .Select(n => new NotificationResponse(
                        n.Id,
                        n.Title,
                        n.Body,
                        n.Type,
                        n.IsRead,
                        n.CreatedAtUtc,
                        n.ReadAtUtc,
                        !string.IsNullOrEmpty(n.Data)
                            ? JsonSerializer.Deserialize<Dictionary<string, string>>(n.Data)
                            : null,
                        n.Image
                    ))
                    .ToList();

                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
                return Error.Failure("Notifications_Retrieval_Failed", "Failed to retrieve notifications");
            }
        }

        public async Task<Result<Updated>> MarkAsReadAsync(
            string userId,
            Guid notificationId,
            CancellationToken ct = default)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, ct);

                if (notification == null)
                    return Error.NotFound("Notification_NotFound", "Notification not found");

                notification.MarkAsRead();
                await _context.SaveChangesAsync(ct);

                return Result.Updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read: {NotificationId}", notificationId);
                return Error.Failure("Notification_Update_Failed", "Failed to update notification");
            }
        }

        public async Task<Result<Updated>> MarkAllAsReadAsync(
            string userId,
            CancellationToken ct = default)
        {
            try
            {
                var unreadNotifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync(ct);

                foreach (var notification in unreadNotifications)
                {
                    notification.MarkAsRead();
                }

                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Marked {Count} notifications as read for user {UserId}",
                    unreadNotifications.Count, userId);

                return Result.Updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
                return Error.Failure("Notifications_Update_Failed", "Failed to update notifications");
            }
        }

        public async Task<Result<Deleted>> DeleteNotificationAsync(
            string userId,
            Guid notificationId,
            CancellationToken ct = default)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, ct);

                if (notification == null)
                    return Error.NotFound("Notification_NotFound", "Notification not found");

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync(ct);

                return Result.Deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification: {NotificationId}", notificationId);
                return Error.Failure("Notification_Delete_Failed", "Failed to delete notification");
            }
        }

        public async Task<Result<int>> GetUnreadCountAsync(
            string userId,
            CancellationToken ct = default)
        {
            try
            {
                var count = await _context.Notifications
                    .CountAsync(n => n.UserId == userId && !n.IsRead, ct);

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
                return Error.Failure("UnreadCount_Retrieval_Failed", "Failed to get unread count");
            }
        }

        public async Task<Result<string>> SubscribeToTopicAsync(
             string deviceToken,
             string topic,
             CancellationToken ct = default)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(deviceToken))
                {
                    return Error.Validation("Invalid_Token", "Device token cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(topic))
                {
                    return Error.Validation("Invalid_Topic", "Topic cannot be empty");
                }

                // Subscribe to topic using FCM
                var response = await FirebaseMessaging.DefaultInstance
                    .SubscribeToTopicAsync(new List<string> { deviceToken }, topic);

                if (response.FailureCount > 0)
                {
                    _logger.LogWarning(
                        "Failed to subscribe {FailureCount} tokens to topic {Topic}",
                        response.FailureCount,
                        topic);

                    return Error.Failure(
                        "Subscription_Failed",
                        $"Failed to subscribe to topic: {response.Errors[0].Reason}");
                }

                _logger.LogInformation(
                    "Successfully subscribed to topic {Topic}",
                    topic);

                return $"Successfully subscribed to topic: {topic}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to topic {Topic}", topic);
                return Error.Failure(
                    "Subscription_Error",
                    "An error occurred while subscribing to topic");
            }
        }

        public async Task<Result<string>> UnsubscribeFromTopicAsync(
      string deviceToken,
      string topic,
      CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(deviceToken))
                {
                    return Error.Validation("Invalid_Token", "Device token cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(topic))
                {
                    return Error.Validation("Invalid_Topic", "Topic cannot be empty");
                }

                var response = await FirebaseMessaging.DefaultInstance
                    .UnsubscribeFromTopicAsync(new List<string> { deviceToken }, topic);

                if (response.FailureCount > 0)
                {
                    _logger.LogWarning(
                        "Failed to unsubscribe {FailureCount} tokens from topic {Topic}",
                        response.FailureCount,
                        topic);

                    return Error.Failure(
                        "Unsubscription_Failed",
                        $"Failed to unsubscribe from topic: {response.Errors[0].Reason}");
                }

                _logger.LogInformation(
                    "Successfully unsubscribed from topic {Topic}",
                    topic);

                return $"Successfully unsubscribed from topic: {topic}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing from topic {Topic}", topic);
                return Error.Failure(
                    "Unsubscription_Error",
                    "An error occurred while unsubscribing from topic");
            }
        }

        public async Task<Result<string>> SendToTopicAsync(
              string topic,
              string title,
              string body,
              Dictionary<string, string>? data = null,
              IFormFile? ImageFile = null,
              CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(topic))
                {
                    return Error.Validation("Invalid_Topic", "Topic cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    return Error.Validation("Invalid_Title", "Title cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(body))
                {
                    return Error.Validation("Invalid_Body", "Body cannot be empty");
                }
                // 🖼️ حفظ الصورة في wwwroot (لو موجودة)
                string? imageUrl = null;
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "notification/topic");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream, ct);
                    }

                    // 🔗 هنا حطينا الدومين يدوي
                    const string baseUrl = "https://gymassistantapi.runasp.net";
                    imageUrl = $"{baseUrl}/images/notification/topic/{uniqueFileName}";
                }
                var message = new Message()
                {
                    Topic = topic,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = body,
                        ImageUrl = imageUrl
                    },
                    Data = data,
                    Android = new AndroidConfig()
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification()
                        {
                            Sound = "default",
                            ChannelId = "default"
                        }
                    },
                    Apns = new ApnsConfig()
                    {
                        Aps = new Aps()
                        {
                            Sound = "default"
                        }
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance
                    .SendAsync(message, ct);

                _logger.LogInformation(
                    "Successfully sent notification to topic {Topic}. Message ID: {MessageId}",
                    topic,
                    response);

                return $"Notification sent successfully to topic: {topic}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to topic {Topic}", topic);
                return Error.Failure(
                    "Send_Failed",
                    "An error occurred while sending notification");
            }
        }
    }
}
