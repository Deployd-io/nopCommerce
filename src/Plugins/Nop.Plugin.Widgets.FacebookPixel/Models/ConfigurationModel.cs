using Microsoft.Extensions.Logging;
namespace Nop.Plugin.Widgets.FacebookPixel.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public class ConfigurationModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationModel()
    {
		_logger.LogDebug(">>> Entering ConfigurationModel()");
        FacebookPixelSearchModel = new FacebookPixelSearchModel();
		_logger.LogDebug("ConfigurationModel(): FacebookPixelSearchModel → {FacebookPixelSearchModel}", FacebookPixelSearchModel);
    }

    #endregion

    #region Properties

    public bool HideList { get; set; }

    public FacebookPixelSearchModel FacebookPixelSearchModel { get; set; }

    #endregion
}