using System.Security.Claims;

namespace GymAssistant_API.Model.Identity.Dtos
{
    public sealed record AppUserDto(string UserId, string Email, IList<string> Roles, IList<Claim> Claims);
}
