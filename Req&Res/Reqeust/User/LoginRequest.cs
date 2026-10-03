using System.ComponentModel.DataAnnotations;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record LoginRequest(
        [Display(Name = LocalizationKeys.Fields.Email)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [EmailAddress(ErrorMessage = LocalizationKeys.Validation.Email)]
        string Email,

        [Display(Name = LocalizationKeys.Fields.Password)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [MinLength(1, ErrorMessage = LocalizationKeys.Validation.MinLength)]
        string Password,

        string? fcmToken = null);
}
