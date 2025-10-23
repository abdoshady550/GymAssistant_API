using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Req
{
    /// <summary>
    /// Request to send push notification
    /// </summary>
    public record SendToTokenPushNotificationRequest(
        [Required] string token,
        [Required] string title,
        [Required] string body,
        Dictionary<string, string>? data = null,
        IFormFile? image = null
    );
}
