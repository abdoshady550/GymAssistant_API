namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Res
{
    /// <summary>
    /// Push notification result
    /// </summary>
    public record PushNotificationResult(
        bool Success,
        string? MessageId = null,
        string? Error = null,
        int SuccessCount = 0,
        int FailureCount = 0
    );
}
