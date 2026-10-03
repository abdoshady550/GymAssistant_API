using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Req_Res.Reqeust.User;
using GymAssistant_API.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace GymAssistant_API.Repository.Services.Identity;

public class IdentityService(AppDbContext context,
                             UserManager<AppUser> userManager,
                             SignInManager<AppUser> signInManager,
                             IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
                             IAuthorizationService authorizationService,
                             IEmailService emailService,
                             IConfiguration configuration,
                             IWebHostEnvironment environment,
                             ILogger<IdentityService> logger) : IIdentityService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IEmailService _emailService = emailService;
    private readonly IConfiguration _configuration = configuration;
    private readonly IWebHostEnvironment _environment = environment;
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
            return Error.NotFound(LocalizationKeys.Auth.UserNotFound, $"User with email {UtilityService.MaskEmail(email)} not found");
        }

        if (!user.EmailConfirmed)
        {
            return Error.Conflict(LocalizationKeys.Auth.EmailNotConfirmed, $"email '{UtilityService.MaskEmail(email)}' not confirmed");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.Conflict(LocalizationKeys.Auth.InvalidCredentials, "Email / Password are incorrect");
        }

        return new AppUserDto(user.Id, user.Email!, await _userManager.GetRolesAsync(user));
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException(nameof(userId));

        var roles = await _userManager.GetRolesAsync(user);


        return new AppUserDto(user.Id, user.Email!, roles);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }
    public async Task<Result<Updated>> ChangeUserPasswordAsync(string userId, string password, string newPassword, string confirmPassword)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return UserErrors.UserNotFound;
            }
            var IsAuthenticate = await AuthenticateAsync(user.Email!, password);
            if (IsAuthenticate.IsError)
            {
                return Error.Conflict(LocalizationKeys.Auth.InvalidCurrentPassword, "The provided current password is incorrect");
            }
            // Check password confirmation
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return Error.Validation(LocalizationKeys.Validation.Required, "New password and confirmation are required");
            }


            if (newPassword != confirmPassword)
            {
                return Error.Conflict(LocalizationKeys.Auth.PasswordsDoNotMatch, "Passwords do not match");
            }
            if (password == newPassword)
            {
                return Error.Conflict(LocalizationKeys.Auth.SamePassword, "The new password must be different from the current password");
            }

            _logger.LogInformation("Starting password reset process for user: {UserId}", user.Id);


            // استخدم ChangePasswordAsync بدل Remove/Add
            var result = await _userManager.ChangePasswordAsync(user, password, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => Error.Validation($"{LocalizationKeys.Identity.Prefix}{e.Code}", e.Description))
                    .ToList();

                _logger.LogError("Failed to change password for user: {ID}, {Errors}", user.Id, errors);
                return errors;
            }

            return Result.Updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception during password change for userId: {UserId}. Exception: {Exception}",
                userId ?? "NULL", ex.ToString());
            return Error.Failure(LocalizationKeys.Auth.ChangePasswordFailed, "An unexpected error occurred while changing the password");

        }
    }
    public async Task<Result<Updated>> UpdateSeedingUsers(string id, ChangeSeedingPasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return UserErrors.UserNotFound;
        }
        var removeResult = await _userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            var errors = removeResult.Errors.Select(e =>
              Error.Validation($"{LocalizationKeys.Identity.Prefix}{e.Code}", e.Description)).ToList();

            _logger.LogError("Failed to remove password for user: {ID}, {Errors}", user.Id, errors);
            return errors;
        }
        _logger.LogInformation("Old password removed, adding new password...");

        var addResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!addResult.Succeeded)
        {
            var errors = addResult.Errors.Select(e =>
              Error.Validation($"{LocalizationKeys.Identity.Prefix}{e.Code}", e.Description)).ToList();

            _logger.LogError("Failed to add new password for user: {ID}, {Errors}", user.Id, errors);
            return errors;
        }
        return Result.Updated;
    }

    public async Task<Result<string>> ForgotPasswordAsync(string email)
    {
        try
        {

            var user = await _userManager.FindByEmailAsync(email);


            if (user == null)
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                return Error.NotFound(LocalizationKeys.Auth.InvalidEmail, "Email not exist ");
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

            var m = "If your email is registered, you can get the link from there.";
            return m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception during password reset for email: {Email}. Exception: {Exception}",
                email ?? "NULL", ex.ToString());
            return Error.NotFound(LocalizationKeys.Auth.InvalidEmail, "Password reset requested for non-existent email");
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
                return Error.Validation(LocalizationKeys.Common.InvalidRequest, "Request data is missing");
            }

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Token))
            {
                _logger.LogError("Email or Token is null/empty. Email: {Email}, Token: {Token}", dto.Email, dto.Token);
                return Error.Validation(LocalizationKeys.Common.InvalidRequest, "Email and token are required");
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
                    return Error.Conflict(LocalizationKeys.Auth.TokenUsed, "This reset link has already been used");
                }

                _logger.LogWarning("Token not found for email: {Email}", dto.Email);
                return Error.NotFound(LocalizationKeys.Auth.InvalidToken, "Invalid token or email");
            }

            // Check if token is expired (with explicit null checks)
            if (resetToken.ExpiryDate < DateTime.UtcNow)
            {
                _logger.LogWarning("Expired token used for email: {Email}, Expiry: {ExpiryDate}",
                    dto.Email, resetToken.ExpiryDate);
                resetToken.IsUsed = true;
                await _context.SaveChangesAsync();
                return Error.Conflict(LocalizationKeys.Auth.TokenExpired, "Token has expired");
            }

            _logger.LogInformation("Looking for user with email: {Email}", dto.Email);

            // Find the user
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", dto.Email);
                return UserErrors.UserNotFound;
            }

            _logger.LogInformation("User found: {UserId}", user.Id);

            // Check password confirmation
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return Error.Validation(LocalizationKeys.Validation.Required, "New password and confirmation are required");
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return Error.Conflict(LocalizationKeys.Auth.PasswordsDoNotMatch, "Passwords do not match");
            }

            _logger.LogInformation("Starting password reset process for user: {UserId}", user.Id);

            // Remove the old password and set the new one
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                var errors = removeResult.Errors.Select(e =>
                  Error.Validation($"{LocalizationKeys.Identity.Prefix}{e.Code}", e.Description)).ToList();


                _logger.LogError("Failed to remove password for {Email}: {Errors}", dto.Email, errors);
                return errors;
            }

            _logger.LogInformation("Old password removed, adding new password...");

            var addResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!addResult.Succeeded)
            {
                var errors = addResult.Errors.Select(e =>
                  Error.Validation($"{LocalizationKeys.Identity.Prefix}{e.Code}", e.Description)).ToList();

                _logger.LogError("Failed to add new password for {Email}: {Errors}", dto.Email, errors);
                return errors;
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
            return Error.Failure(LocalizationKeys.Auth.ResetFailed, "An unexpected error occurred while resetting the password");
        }


    }

    #region External Login Methods 
    public async Task<Result<ExternalLoginInfo>> GetExternalLoginInfoAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            _logger.LogWarning("Failed to get external login info");
            return Error.NotFound(LocalizationKeys.Auth.ExternalLoginFailed, "Failed to get external login info");
        }

        return info;
    }
    public async Task<Result<AppUserDto>> ExternalLoginAsync(ExternalAuthInfoDto externalInfo)
    {
        try
        {
            _logger.LogInformation("Starting external login for email: {Email}, provider: {Provider}",
                externalInfo.Email, externalInfo.Provider);

            // البحث عن المستخدم بالإيميل
            var user = await _userManager.FindByEmailAsync(externalInfo.Email);

            if (user == null)
            {
                _logger.LogInformation("User not found, creating new user for email: {Email}", externalInfo.Email);

                // إنشاء مستخدم جديد
                user = new AppUser
                {
                    UserName = externalInfo.Email,
                    Email = externalInfo.Email,
                    EmailConfirmed = true // تأكيد الإيميل تلقائياً
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create user: {Errors}",
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));

                    var errors = createResult.Errors.Select(e =>
                        Error.Validation($"{LocalizationKeys.Identity.Prefix}{e.Code}", e.Description)).ToList();
                    return errors;
                }

                // إضافة Role افتراضي
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Failed to assign default role to user {UserId}", user.Id);
                }

                // إنشاء ClientProfile للمستخدم الجديد
                var firstName = externalInfo.FirstName ?? "User";
                var lastName = externalInfo.LastName ?? "Name";

                var profileResult = ClientProfile.CreateProfile(
                    Guid.NewGuid(),
                    user.Id,
                    firstName,
                    lastName,
                    Gender.Male, // افتراضي - يمكن للمستخدم تعديله لاحقاً
                    UserRole.User
                );

                if (!profileResult.IsError)
                {
                    _context.ClientProfiles.Add(profileResult.Value);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Created profile for user {UserId}", user.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to create profile for user {UserId}: {Error}",
                        user.Id, profileResult.TopError.Description);
                }
            }
            else
            {
                _logger.LogInformation("Existing user found with email: {Email}", externalInfo.Email);
            }

            // ربط External Login بالمستخدم
            var loginInfo = new UserLoginInfo(
                externalInfo.Provider,
                externalInfo.ProviderId,
                externalInfo.Provider
            );

            // التحقق من وجود الـ External Login
            var logins = await _userManager.GetLoginsAsync(user);
            var loginExists = logins.Any(l =>
                l.LoginProvider == externalInfo.Provider &&
                l.ProviderKey == externalInfo.ProviderId
            );

            if (!loginExists)
            {
                _logger.LogInformation("Adding external login for user {UserId}, provider: {Provider}",
                    user.Id, externalInfo.Provider);

                var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);

                if (!addLoginResult.Succeeded)
                {
                    _logger.LogWarning("Failed to add external login for user {Email}: {Errors}",
                        user.Email,
                        string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                }
                else
                {
                    _logger.LogInformation("Successfully added external login for user {UserId}", user.Id);
                }
            }

            // إرجاع معلومات المستخدم
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            _logger.LogInformation("External login successful for user {UserId}", user.Id);

            return new AppUserDto(user.Id, user.Email!, roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during external login for email: {Email}, provider: {Provider}",
                externalInfo.Email, externalInfo.Provider);
            return Error.Failure(LocalizationKeys.Auth.ExternalLoginFailed, "An error occurred during external login");
        }
    }
    public async Task<Result<SignInResult>> ExternalLoginSignInAsync(string loginProvider, string providerKey)
    {
        try
        {
            _logger.LogInformation("Attempting external sign-in with provider: {Provider}", loginProvider);

            var result = await _signInManager.ExternalLoginSignInAsync(
                loginProvider,
                providerKey,
                isPersistent: false,
                bypassTwoFactor: true
            );

            if (result.Succeeded)
            {
                _logger.LogInformation("External sign-in successful for provider: {Provider}", loginProvider);
            }
            else
            {
                _logger.LogWarning("External sign-in failed for provider: {Provider}. IsLockedOut: {IsLockedOut}, RequiresTwoFactor: {RequiresTwoFactor}",
                    loginProvider, result.IsLockedOut, result.RequiresTwoFactor);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during external sign-in for provider: {Provider}", loginProvider);
            return Error.Failure(LocalizationKeys.Auth.ExternalSignInFailed, "An error occurred during external sign-in");
        }
    }
    public async Task<Result<IdentityResult>> AddExternalLoginAsync(string userId, ExternalLoginInfo info)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return UserErrors.UserNotFound;
            }

            _logger.LogInformation("Adding external login for user {UserId}, provider: {Provider}",
                userId, info.LoginProvider);

            var result = await _userManager.AddLoginAsync(user, info);

            if (result.Succeeded)
            {
                _logger.LogInformation("Successfully added external login for user {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Failed to add external login for user {UserId}: {Errors}",
                    userId,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding external login for user: {UserId}", userId);
            return Error.Failure(LocalizationKeys.Auth.AddExternalLoginFailed, "An error occurred while adding external login");
        }
    }



    #endregion

    public async Task<Result<Deleted>> DeleteUserAccountAsync(string userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Account deletion attempted for non-existent user: {UserId}", userId);
                return UserErrors.UserNotFound;
            }

            var profile = await _context.ClientProfiles
                .Include(p => p.CustomExercises)
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            var filesToDelete = new List<string>();

            using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                if (profile != null)
                {
                    var profileId = profile.Id;

                    // 1. Collect images to delete from disk
                    if (!string.IsNullOrEmpty(profile.Image))
                    {
                        var profileImgPath = Path.Combine(_environment.WebRootPath, profile.Image.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        filesToDelete.Add(profileImgPath);
                    }

                    foreach (var customEx in profile.CustomExercises)
                    {
                        if (!string.IsNullOrEmpty(customEx.ImageUrl))
                        {
                            var exImgPath = Path.Combine(_environment.WebRootPath, customEx.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                            filesToDelete.Add(exImgPath);
                        }
                    }

                    // 2. Chat Conversations and Messages
                    var chatConversations = await _context.ChatConversations
                        .Where(c => c.TrainerId == profileId || c.TraineeId == profileId)
                        .Include(c => c.Messages)
                        .ToListAsync(ct);
                    _context.ChatConversations.RemoveRange(chatConversations);

                    // 3. Trainer Requests (both sent and received)
                    var trainerRequests = await _context.TrainerRequests
                        .Where(tr => tr.TrainerId == profileId || tr.TraineeId == profileId)
                        .ToListAsync(ct);
                    _context.TrainerRequests.RemoveRange(trainerRequests);

                    // 4. User Requests (both sent and received)
                    var userRequests = await _context.UserRequests
                        .Where(ur => ur.TrainerId == profileId || ur.TraineeId == profileId)
                        .ToListAsync(ct);
                    _context.UserRequests.RemoveRange(userRequests);

                    // 5. Trainer-Trainee relationships
                    var trainerTrainees = await _context.TrainerTrainees
                        .Where(tt => tt.TrainerId == profileId || tt.TraineeId == profileId)
                        .ToListAsync(ct);
                    _context.TrainerTrainees.RemoveRange(trainerTrainees);

                    // 6. Workout sessions created by this user as trainer for other trainees
                    var workoutsCreatedByTrainer = await _context.WorkoutSessions
                        .Where(ws => ws.CreatedByTrainerId == profileId)
                        .ToListAsync(ct);
                    foreach (var ws in workoutsCreatedByTrainer)
                    {
                        _context.Entry(ws).Property(x => x.CreatedByTrainerId).CurrentValue = null;
                    }

                    // 7. Workout sessions of this user (including exercises and sets)
                    var userWorkouts = await _context.WorkoutSessions
                        .Where(ws => ws.ClientProfileId == profileId)
                        .Include(ws => ws.WorkoutExercises)
                            .ThenInclude(we => we.Sets)
                        .ToListAsync(ct);
                    _context.WorkoutSessions.RemoveRange(userWorkouts);

                    // 8. Personal records of this user
                    var personalRecords = await _context.PersonalRecords
                        .Where(pr => pr.ClientProfileId == profileId)
                        .ToListAsync(ct);
                    _context.PersonalRecords.RemoveRange(personalRecords);

                    // 9. SectionGroups created by this user
                    var sectionGroups = await _context.SectionGroups
                        .Where(sg => sg.ClientProfileId == profileId)
                        .ToListAsync(ct);
                    _context.SectionGroups.RemoveRange(sectionGroups);

                    // 10. Custom exercises of this user
                    var customExercises = await _context.UserExercises
                        .Where(ue => ue.ClientProfileId == profileId)
                        .ToListAsync(ct);
                    _context.UserExercises.RemoveRange(customExercises);

                    // 11. Body measurements of this user
                    var measurements = await _context.BodyMeasurements
                        .Where(bm => bm.ClientProfileId == profileId)
                        .ToListAsync(ct);
                    _context.BodyMeasurements.RemoveRange(measurements);

                    // 12. Remove ClientProfile
                    _context.ClientProfiles.Remove(profile);
                }

                // 13. Device tokens
                var deviceTokens = await _context.DeviceTokens
                    .Where(dt => dt.UserId == userId)
                    .ToListAsync(ct);
                _context.DeviceTokens.RemoveRange(deviceTokens);

                // 14. Notifications
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .ToListAsync(ct);
                _context.Notifications.RemoveRange(notifications);

                // 15. Refresh tokens
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId)
                    .ToListAsync(ct);
                _context.RefreshTokens.RemoveRange(refreshTokens);

                // 16. Password reset tokens
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var resetTokens = await _context.PasswordResetTokens
                        .Where(prt => prt.Email == user.Email)
                        .ToListAsync(ct);
                    _context.PasswordResetTokens.RemoveRange(resetTokens);
                }

                await _context.SaveChangesAsync(ct);

                // 17. Delete Identity AppUser
                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    await transaction.RollbackAsync(ct);
                    var errors = deleteResult.Errors.Select(e => Error.Validation($"{LocalizationKeys.Identity.Prefix}{e.Code}", e.Description)).ToList();
                    _logger.LogError("Failed to delete AppUser {UserId}: {Errors}", userId, errors);
                    return errors;
                }

                await transaction.CommitAsync(ct);

                // 18. Safely delete physical files after successful commit
                foreach (var filePath in filesToDelete)
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete physical file {FilePath} for user {UserId}", filePath, userId);
                    }
                }

                _logger.LogInformation("Successfully deleted user account {UserId}", userId);
                return Result.Deleted;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Transaction rolled back while deleting user account {UserId}", userId);
                return UserErrors.DeleteUserFailed;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting user account {UserId}", userId);
            return UserErrors.DeleteUserFailed;
        }
    }
}