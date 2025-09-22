using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record LoginRequest(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,
    [Required(ErrorMessage = "Password is required")]
    [MinLength(1, ErrorMessage = "Password cannot be empty")]
    string Password);
}
