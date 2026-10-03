using System.ComponentModel.DataAnnotations;
using GymAssistant_API.Resources;

namespace GymAssistant_API.Model.Identity.Dtos
{
    public class ResetPasswordDto
    {
        [Display(Name = LocalizationKeys.Fields.Email)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [EmailAddress(ErrorMessage = LocalizationKeys.Validation.Email)]
        public string Email { get; set; } = null!;

        [Display(Name = LocalizationKeys.Fields.Token)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        public string Token { get; set; } = null!;

        [Display(Name = LocalizationKeys.Fields.NewPassword)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [MinLength(6, ErrorMessage = LocalizationKeys.Validation.MinLength)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Display(Name = LocalizationKeys.Fields.ConfirmPassword)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [Compare(nameof(NewPassword), ErrorMessage = LocalizationKeys.Validation.Compare)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
