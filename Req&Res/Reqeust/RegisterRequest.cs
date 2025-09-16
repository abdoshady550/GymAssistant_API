using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using GymAssistant_API.Model.Identity;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record RegisterRequest(
        string UserName,
        string Email,
        string Password,
        string PhoneNumber,

        [SwaggerSchema("Role of the user. Allowed values: 1=User, 2=Trainer, 3=Admin")]
        Role Role);
}
