using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Reqeust;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace GymAssistant_API.Repository.Services.User
{
    public class UserCreateService(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager) : IUserCreate
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public async Task<Result<AppUserDto>> AddUserAsync(RegisterRequest request, CancellationToken ct = default)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    return Error.Conflict(Resources.LocalizationKeys.Auth.EmailAlreadyExists);
                }
                // Email validation
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    !Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
                {
                    return Error.Validation(Resources.LocalizationKeys.Auth.InvalidEmail);
                }
                //  Phone number validation 
                if (!string.IsNullOrWhiteSpace(request.PhoneNumber) &&
                    !Regex.IsMatch(request.PhoneNumber, @"^(?:\+20|0)?1[0125][0-9]{8}$"))
                {
                    return Error.Validation(Resources.LocalizationKeys.Auth.InvalidPhoneNumber);
                }


                //  Role validation 
                if (!Enum.IsDefined(typeof(Role), request.Role))
                {
                    return Error.Validation(Resources.LocalizationKeys.Auth.InvalidRole, "User, Trainer");
                }
                user = new AppUser()
                {

                    UserName = request.UserName,
                    Email = request.Email,
                    EmailConfirmed = true,
                    PhoneNumber = request.PhoneNumber,
                    PhoneNumberConfirmed = true

                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = Extensions.IdentityServiceExtensions.ConvertIdentityErrors(result.Errors);
                    return errors;
                }


                var role = request.Role.ToString();

                if (role == null)
                {
                    return Error.Validation(Resources.LocalizationKeys.Auth.RoleRequired);
                }

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);

                return new AppUserDto(user.Id, user.Email!, await _userManager.GetRolesAsync(user));
            }
            catch (Exception ex)
            {
                return Error.Failure(Resources.LocalizationKeys.Auth.RegistrationFailed);
            }
        }
    }
}
