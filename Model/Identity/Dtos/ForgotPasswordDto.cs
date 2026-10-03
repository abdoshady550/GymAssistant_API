using System.ComponentModel.DataAnnotations;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Model.Identity.Dtos
{
    public class ForgotPasswordDto
    {
        [Display(Name = LocalizationKeys.Fields.Email)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [EmailAddress(ErrorMessage = LocalizationKeys.Validation.Email)]
        public string Email { get; set; } = null!;
    }

}
