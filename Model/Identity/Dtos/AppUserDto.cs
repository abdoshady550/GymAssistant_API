using GymAssistant_API.Model.Entities.User;
using System.Security.Claims;

namespace GymAssistant_API.Model.Identity.Dtos
{
    public sealed record AppUserDto(string UserId,
                                    string Email,
                                    IList<string> Roles);
    public sealed record UserDto(string UserId,
                                 bool IsInRelation,
                                 string UserName,
                                 string? Email,
                                 Gender? Gender,
                                 string? PhoneNumber,
                                 string? Image);
}
