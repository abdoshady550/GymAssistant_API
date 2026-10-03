using System.ComponentModel.DataAnnotations;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Req
{
    /// <summary>
    /// Request to send notification to multiple users
    /// </summary>
    public record SendBulkNotificationRequest(
        [Display(Name = LocalizationKeys.Fields.Target)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)] List<string> UserIds,

        [Display(Name = LocalizationKeys.Fields.Name)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)] string Title,

        [Display(Name = LocalizationKeys.Fields.Name)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)] string Body,

        NotificationType Type = NotificationType.General,
        Dictionary<string, string>? Data = null,
        IFormFile? Image = null
    );
}
