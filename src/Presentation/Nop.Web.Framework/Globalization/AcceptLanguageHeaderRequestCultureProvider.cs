using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Globalization;

public partial class NopAcceptLanguageHeaderRequestCultureProvider : RequestCultureProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
		_logger.LogDebug(">>> Entering DetermineProviderCultureResult(httpContext={HttpContext})", httpContext);
        ArgumentNullException.ThrowIfNull(httpContext);

        var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().AcceptLanguage;
		_logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): acceptLanguageHeader → {AcceptLanguageHeader}", httpContext, acceptLanguageHeader);

        if (acceptLanguageHeader == null || acceptLanguageHeader.Count == 0)
            return NullProviderCultureResult;

        var languageHeader = acceptLanguageHeader.FirstOrDefault();
		_logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): languageHeader → {LanguageHeader}", httpContext, languageHeader);

        if (string.IsNullOrEmpty(languageHeader.Value.Value))
            return NullProviderCultureResult;

        try
        {
            var requestedCulture = new CultureInfo(languageHeader.Value.Value);
			_logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): requestedCulture → {RequestedCulture}", httpContext, requestedCulture);

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
			_logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): languageService → {LanguageService}", httpContext, languageService);
            var language = languageService
                .GetAllLanguages()
                .FirstOrDefault(urlLanguage =>
                {
                    var storeCulture = new CultureInfo(urlLanguage.LanguageCulture);
					_logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): storeCulture → {StoreCulture}", httpContext, storeCulture);
                    return storeCulture.Name == requestedCulture.Name || storeCulture.Parent?.Name == requestedCulture.Name;
                });

            if (language == null)
                return new AcceptLanguageHeaderRequestCultureProvider().DetermineProviderCultureResult(httpContext);

            return Task.FromResult(new ProviderCultureResult(language.LanguageCulture));
        }
        catch (CultureNotFoundException)
        {
			_logger.LogError("Exception in DetermineProviderCultureResult(httpContext={HttpContext}): {Arg1}", httpContext, CultureNotFoundException.getMessage());
			_logger.LogDebug("<<< Exiting DetermineProviderCultureResult(httpContext={HttpContext})", httpContext);
            return NullProviderCultureResult;
        }
    }
}
