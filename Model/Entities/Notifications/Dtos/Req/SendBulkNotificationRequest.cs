using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Req
{
    /// <summary>
    /// Request to send notification to multiple users
    /// </summary>
    public record SendBulkNotificationRequest(
        [Required] List<string> UserIds,
        [Required] string Title,
        [Required] string Body,
        NotificationType Type = NotificationType.General,
        Dictionary<string, string>? Data = null,
        IFormFile? Image = null
    );
}
