using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Globalization;

/// <summary>
/// Determines the culture information for a request via the URL
/// </summary>
public partial class NopSeoUrlCultureProvider : RequestCultureProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Implements the provider to determine the culture of the given request
    /// </summary>
    /// <param name="httpContext">HttpContext for the request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains ProviderCultureResult
    /// </returns>
    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
		_logger.LogDebug(">>> Entering DetermineProviderCultureResult(httpContext={HttpContext})", httpContext);
        var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();

		_logger.LogDebug("DetermineProviderCultureResult(httpContext={HttpContext}): localizationSettings → {LocalizationSettings}", httpContext, localizationSettings);
        if (!localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            return await NullProviderCultureResult;

        //localized URLs are enabled, so try to get language from the requested page URL
        var (isLocalized, language) = await httpContext.Request.Path.Value.IsLocalizedUrlAsync(httpContext.Request.PathBase, false);
        if (!isLocalized || language is null)
            return await NullProviderCultureResult;

        return new ProviderCultureResult(language.LanguageCulture);
    }
}