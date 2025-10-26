namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Res
{

    /// <summary>
    /// Notification response
    /// </summary>
    public record NotificationResponse(
        Guid Id,
        string Title,
        string Body,
        NotificationType? Type,
        bool IsRead,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? ReadAtUtc,
        Dictionary<string, string>? Data = null,
        string? Image = null
    );
}
