using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using Microsoft.AspNetCore.Identity;

namespace GymAssistant_API.Repository.Interfaces.Identity;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string? policyName);
    Task<Result<AppUserDto>> AuthenticateAsync(string email, string password);
    Task<Result<AppUserDto>> GetUserByIdAsync(string userId);
    Task<string?> GetUserNameAsync(string userId);

    Task<Result<string>> ForgotPasswordAsync(string email);
    Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto);


    Task<Result<ExternalLoginInfo>> GetExternalLoginInfoAsync();
    Task<Result<AppUserDto>> ExternalLoginAsync(ExternalAuthInfoDto externalInfo);
    Task<Result<SignInResult>> ExternalLoginSignInAsync(string loginProvider, string providerKey);
    Task<Result<IdentityResult>> AddExternalLoginAsync(string userId, ExternalLoginInfo info);
}