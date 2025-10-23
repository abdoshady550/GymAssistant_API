using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Req
{
    /// <summary>
    /// Request to send push notification
    /// </summary>
    public record SendPushNotificationRequest(
        [Required] string UserId,
        [Required] string Title,
        [Required] string Body,
        NotificationType Type = NotificationType.General,
        Dictionary<string, string>? Data = null,
        IFormFile? Image = null
    );
}
