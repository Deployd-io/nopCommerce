using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Infrastructure;

/// <summary>
/// Represents base provider
/// </summary>
public partial class BaseRouteProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Get pattern used to detect routes with language code
    /// </summary>
    /// <returns></returns>
    protected string GetLanguageRoutePattern()
    {
		_logger.LogDebug(">>> Entering GetLanguageRoutePattern()");
        if (DataSettingsManager.IsDatabaseInstalled())
        {
            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
			_logger.LogDebug("GetLanguageRoutePattern(): localizationSettings → {LocalizationSettings}", localizationSettings);
            {
                //by default we use empty for the language template, later it'll be specified with the working language code
                var code = string.Empty;
                return $"{{{NopRoutingDefaults.RouteValue.Language}:maxlength(2):{NopRoutingDefaults.LanguageParameterTransformer}={code}}}";
            }
        }

		_logger.LogDebug("<<< Exiting GetLanguageRoutePattern()");
        return string.Empty;
    }
}