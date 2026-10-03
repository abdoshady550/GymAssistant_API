using GymAssistant_API.Model.Results;
using GymAssistant_API.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace GymAssistant_API.Extensions;

public static class LocalizationExtensions
{
    public static readonly string[] SupportedCultures = ["en", "ar"];

    public static IServiceCollection AddAppLocalization(this IServiceCollection services)
    {
        services.AddLocalization();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.SetDefaultCulture("en")
                   .AddSupportedCultures(SupportedCultures)
                   .AddSupportedUICultures(SupportedCultures);

            options.ApplyCurrentCultureToResponseHeaders = true;

            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            };
        });

        return services;
    }

    public static string Localize(this IStringLocalizer localizer, Error error)
    {
        if (error.Args != null && error.Args.Length > 0)
        {
            var localized = localizer[error.Code, error.Args];
            return localized.ResourceNotFound ? (error.Description ?? error.Code) : localized.Value;
        }
        else
        {
            var localized = localizer[error.Code];
            return localized.ResourceNotFound ? (error.Description ?? error.Code) : localized.Value;
        }
    }
}
