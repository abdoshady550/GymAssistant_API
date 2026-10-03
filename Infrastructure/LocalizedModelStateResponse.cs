using Microsoft.AspNetCore.Mvc;

namespace GymAssistant_API.Infrastructure;

public static class LocalizedModelStateResponse
{
    public static IActionResult Create(ActionContext context)
    {
        var statusCode = StatusCodes.Status400BadRequest;
        var isArabic = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("ar", StringComparison.OrdinalIgnoreCase);

        var title = isArabic
            ? "حدث خطأ واحد أو أكثر في التحقق من صحة البيانات."
            : "One or more validation errors occurred.";

        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = title,
            Status = statusCode
        };

        return new BadRequestObjectResult(problemDetails);
    }
}
