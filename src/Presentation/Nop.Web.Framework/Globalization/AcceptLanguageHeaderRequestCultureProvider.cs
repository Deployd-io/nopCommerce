using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Globalization;

public partial class NopAcceptLanguageHeaderRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
		logger.LogDebug(">>> Entering DetermineProviderCultureResult(httpContext={HttpContext})", httpContext);
        ArgumentNullException.ThrowIfNull(httpContext);

        var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().AcceptLanguage;

		logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): acceptLanguageHeader → {AcceptLanguageHeader}", httpContext, acceptLanguageHeader);
        if (acceptLanguageHeader == null || acceptLanguageHeader.Count == 0)
            return NullProviderCultureResult;

        var languageHeader = acceptLanguageHeader.FirstOrDefault();

		logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): languageHeader → {LanguageHeader}", httpContext, languageHeader);
        if (string.IsNullOrEmpty(languageHeader.Value.Value))
            return NullProviderCultureResult;

        try
        {
            var requestedCulture = new CultureInfo(languageHeader.Value.Value);

			logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): requestedCulture → {RequestedCulture}", httpContext, requestedCulture);
            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var language = languageService
				logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): languageService → {LanguageService}", httpContext, languageService);
                .GetAllLanguages()
                .FirstOrDefault(urlLanguage =>
                {
                    var storeCulture = new CultureInfo(urlLanguage.LanguageCulture);
                    return storeCulture.Name == requestedCulture.Name || storeCulture.Parent?.Name == requestedCulture.Name;
                });

            if (language == null)
                return new AcceptLanguageHeaderRequestCultureProvider().DetermineProviderCultureResult(httpContext);

            return Task.FromResult(new ProviderCultureResult(language.LanguageCulture));
        }
        catch (CultureNotFoundException)
        {
			logger.LogError("Exception in DetermineProviderCultureResult(httpContext={HttpContext}): {Arg1}", httpContext, ex.Message);
			logger.LogDebug("<<< Exiting DetermineProviderCultureResult(httpContext={HttpContext})", httpContext);
            return NullProviderCultureResult;
        }
    }
}
