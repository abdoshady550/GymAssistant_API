using System.ComponentModel.DataAnnotations;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Model.Entities.Notifications.Dtos.Req
{
    /// <summary>
    /// Request to send push notification
    /// </summary>
    public record SendToTokenPushNotificationRequest(
        [Display(Name = LocalizationKeys.Fields.DeviceToken)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)] string token,

        [Display(Name = LocalizationKeys.Fields.Name)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)] string title,

        [Display(Name = LocalizationKeys.Fields.Name)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)] string body,

        Dictionary<string, string>? data = null,
        IFormFile? image = null
    );
}
