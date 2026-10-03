using Microsoft.AspNetCore.SignalR;
using System.Globalization;

namespace GymAssistant_API.Hubs;

public class LocalizationHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var httpContext = invocationContext.Context.GetHttpContext();
        if (httpContext != null)
        {
            var cultureQuery = httpContext.Request.Query["culture"].ToString();
            var acceptLang = httpContext.Request.Headers["Accept-Language"].ToString();

            string cultureName = "en";
            if (!string.IsNullOrEmpty(cultureQuery) && (cultureQuery.StartsWith("ar", StringComparison.OrdinalIgnoreCase) || cultureQuery.StartsWith("en", StringComparison.OrdinalIgnoreCase)))
            {
                cultureName = cultureQuery.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";
            }
            else if (!string.IsNullOrEmpty(acceptLang) && acceptLang.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
            {
                cultureName = "ar";
            }

            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        return await next(invocationContext);
    }
}
