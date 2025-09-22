using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using GymAssistant_API.Model.Identity;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record RegisterRequest(
         [Required(ErrorMessage = "Username is required")]
        string UserName,

         [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        string Email,

         [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        string Password,

         [Phone(ErrorMessage = "Invalid phone number format")]
        string PhoneNumber,

         [Required(ErrorMessage = "Role is required")]
        [SwaggerSchema("Role of the user. Allowed values: 1=User, 2=Trainer, 3=Admin")]
        Role Role
     );
}
