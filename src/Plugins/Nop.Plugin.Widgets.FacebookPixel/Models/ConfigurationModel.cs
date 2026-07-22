namespace Nop.Plugin.Widgets.FacebookPixel.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public class ConfigurationModel
{
    #region Ctor

    public ConfigurationModel()
    {
		logger.LogDebug(">>> Entering ConfigurationModel()");
        FacebookPixelSearchModel = new FacebookPixelSearchModel();
		logger.LogDebug("<<< Exiting ConfigurationModel()");
		logger.LogDebug("ConfigurationModel(): FacebookPixelSearchModel → {FacebookPixelSearchModel}", FacebookPixelSearchModel);
    }

    #endregion

    #region Properties

    public bool HideList { get; set; }

    public FacebookPixelSearchModel FacebookPixelSearchModel { get; set; }

    #endregion
}
