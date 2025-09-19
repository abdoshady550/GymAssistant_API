using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Model.Identity.Dtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = null!;
    }

}
