using GymAssistant_API.Model.Entities.User;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GymAssistant_API.Infrastructure;

public class UserPreferredLanguageMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, UserManager<AppUser> userManager)
    {
        await _next(context);

        // Update preferred language after request if authenticated
        if (context.User?.Identity?.IsAuthenticated == true && context.Response.StatusCode < 400)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var currentCulture = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
                if (currentCulture is "ar" or "en")
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null && user.PreferredLanguage != currentCulture)
                    {
                        user.PreferredLanguage = currentCulture;
                        await userManager.UpdateAsync(user);
                    }
                }
            }
        }
    }
}
