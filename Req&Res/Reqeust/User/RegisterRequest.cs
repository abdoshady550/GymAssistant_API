using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record RegisterRequest(
        [Display(Name = LocalizationKeys.Fields.UserName)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        string UserName,

        [Display(Name = LocalizationKeys.Fields.Email)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [EmailAddress(ErrorMessage = LocalizationKeys.Validation.Email)]
        string Email,

        [Display(Name = LocalizationKeys.Fields.Password)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [MinLength(6, ErrorMessage = LocalizationKeys.Validation.MinLength)]
        string Password,

        [Display(Name = LocalizationKeys.Fields.PhoneNumber)]
        [Phone(ErrorMessage = LocalizationKeys.Validation.Phone)]
        string PhoneNumber,

        [Display(Name = LocalizationKeys.Fields.Role)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [SwaggerSchema("Role of the user. Allowed values: 1=User, 2=Trainer, 3=Admin")]
        Role Role
     );
}
