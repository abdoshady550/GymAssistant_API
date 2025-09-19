using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace GymAssistant_API.Repository.Services.Identity;

public class IdentityService(AppDbContext context,
                             UserManager<AppUser> userManager,
                             IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
                             IAuthorizationService authorizationService,
                             IEmailService emailService,
                             IConfiguration configuration,
                             ILogger<IdentityService> logger) : IIdentityService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IEmailService _emailService = emailService;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<IdentityService> _logger = logger;


    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string? policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName!);

        return result.Succeeded;
    }

    public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Error.NotFound("User_Not_Found", $"User with email {UtilityService.MaskEmail(email)} not found");
        }

        if (!user.EmailConfirmed)
        {
            return Error.Conflict("Email_Not_Confirmed", $"email '{UtilityService.MaskEmail(email)}' not confirmed");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.Conflict("Invalid_Login_Attempt", "Email / Password are incorrect");
        }

        return new AppUserDto(user.Id, user.Email!, await _userManager.GetRolesAsync(user), await _userManager.GetClaimsAsync(user));
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException(nameof(userId));

        var roles = await _userManager.GetRolesAsync(user);

        var claims = await _userManager.GetClaimsAsync(user);

        return new AppUserDto(user.Id, user.Email!, roles, claims);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<Result<string>> ForgotPasswordAsync(string email)
    {
        try
        {

            var user = await _userManager.FindByEmailAsync(email);


            if (user == null)
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                throw new ValidationException("Email not found ");
            }
            // Invalidate any existing tokens for this email
            var existingTokens = await _context.PasswordResetTokens
                .Where(t => t.Email == email && !t.IsUsed && t.ExpiryDate > DateTime.UtcNow)
                .ToListAsync();

            foreach (var tokeny in existingTokens)
            {
                tokeny.IsUsed = true;
            }
            // Generate new reset token
            var resetToken = GenerateSecureToken();
            var resetTokenEntity = new PasswordResetToken
            {
                Email = email,
                Token = resetToken,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                IsUsed = false
            };

            _context.PasswordResetTokens.Add(resetTokenEntity);
            await _context.SaveChangesAsync();

            // Send reset email
            await _emailService.SendPasswordResetEmailAsync(email, resetToken);

            _logger.LogInformation("Password reset token generated for email:{email}", email);

            var m = "If your email is registered, you can get the link from here.";
            return m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception during password reset for email: {Email}. Exception: {Exception}",
                email ?? "NULL", ex.ToString());
            return Error.NotFound("Not_Exist", "Password reset requested for non-existent email");
        }
    }
    private string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        try
        {
            _logger.LogInformation("=== Password Reset Debug ===");
            _logger.LogInformation("Email: {Email}, Token: {Token}", dto?.Email ?? "NULL", dto?.Token ?? "NULL");

            // Validate input first
            if (dto == null)
            {
                _logger.LogError("ResetPasswordDto is null");
                return Error.Validation("Invalid_Request", "Request data is missing");
            }

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Token))
            {
                _logger.LogError("Email or Token is null/empty. Email: {Email}, Token: {Token}", dto.Email, dto.Token);
                return Error.Validation("Invalid_Request", "Email and token are required");
            }

            _logger.LogInformation("Searching for token in database...");

            // Find the token with explicit null checks
            var resetToken = await _context.PasswordResetTokens
                .Where(t => t.Token == dto.Token && t.Email == dto.Email && !t.IsUsed)
                .FirstOrDefaultAsync();

            _logger.LogInformation("Token search result: {Found}", resetToken != null);

            // Check if token exists
            if (resetToken == null)
            {
                // Check if token exists but is already used
                var usedToken = await _context.PasswordResetTokens
                    .Where(t => t.Token == dto.Token && t.Email == dto.Email)
                    .FirstOrDefaultAsync();

                if (usedToken != null && usedToken.IsUsed)
                {
                    _logger.LogWarning("Token already used for email: {Email}", dto.Email);
                    return Error.Conflict("Token_Used", "This reset link has already been used");
                }

                _logger.LogWarning("Token not found for email: {Email}", dto.Email);
                return Error.NotFound("Invalid_Token", "Invalid token or email");
            }

            // Check if token is expired (with explicit null checks)
            if (resetToken.ExpiryDate < DateTime.UtcNow)
            {
                _logger.LogWarning("Expired token used for email: {Email}, Expiry: {ExpiryDate}",
                    dto.Email, resetToken.ExpiryDate);
                resetToken.IsUsed = true;
                await _context.SaveChangesAsync();
                return Error.Conflict("Token_Expired", "Token has expired");
            }

            _logger.LogInformation("Looking for user with email: {Email}", dto.Email);

            // Find the user
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", dto.Email);
                return Error.NotFound("User_Not_Found", "User not found");
            }

            _logger.LogInformation("User found: {UserId}", user.Id);

            // Check password confirmation
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return Error.Validation("Invalid_Passwords", "New password and confirmation are required");
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return Error.Conflict("Passwords_Not_Match", "Passwords do not match");
            }

            _logger.LogInformation("Starting password reset process for user: {UserId}", user.Id);

            // Remove the old password and set the new one
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                var removeErrors = removeResult.Errors.Select(e => e.Description);
                _logger.LogError("Failed to remove password for {Email}: {Errors}", dto.Email, string.Join(", ", removeErrors));
                return Error.Failure("Remove_Password_Failed", string.Join(", ", removeErrors));
            }

            _logger.LogInformation("Old password removed, adding new password...");

            var addResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!addResult.Succeeded)
            {
                var addErrors = addResult.Errors.Select(e => e.Description);
                _logger.LogError("Failed to add new password for {Email}: {Errors}", dto.Email, string.Join(", ", addErrors));
                return Error.Failure("Add_Password_Failed", string.Join(", ", addErrors));
            }

            _logger.LogInformation("New password added, updating security stamp...");

            // Update security stamp to invalidate existing tokens/sessions
            await _userManager.UpdateSecurityStampAsync(user);

            // Mark token as used
            resetToken.IsUsed = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password reset completed successfully for email: {Email}", dto.Email);

            // Remove token
            _context.PasswordResetTokens.Remove(resetToken);
            await _context.SaveChangesAsync();

            return "Password has been reset successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception during password reset for email: {Email}. Exception: {Exception}",
                dto?.Email ?? "NULL", ex.ToString());
            return Error.Failure("Reset_Failed", "An unexpected error occurred while resetting the password");
        }


    }
}