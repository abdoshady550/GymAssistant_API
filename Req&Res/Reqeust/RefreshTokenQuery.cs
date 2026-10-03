using System.ComponentModel.DataAnnotations;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record RefreshTokenQuery(
        [Display(Name = LocalizationKeys.Fields.RefreshToken)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [MinLength(10, ErrorMessage = LocalizationKeys.Validation.MinLength)]
        string RefreshToken,

        [Display(Name = LocalizationKeys.Fields.Token)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        string ExpiredAccessToken
    );
}
