using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.CloudflareImages;

/// <summary>
/// Represents Cloudflare Images plugin
/// </summary>
public class CloudflareImagesPlugin : BasePlugin, IMiscPlugin
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public CloudflareImagesPlugin(ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering CloudflareImagesPlugin(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper})", localizationService, settingService, webHelper);
        _localizationService = localizationService;
        _settingService = settingService;
		_logger.LogDebug("CloudflareImagesPlugin(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper}): _settingService → {_settingService}", localizationService, settingService, webHelper, _settingService);
		_logger.LogDebug("CloudflareImagesPlugin(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper}): _localizationService → {_localizationService}", localizationService, settingService, webHelper, _localizationService);
        _webHelper = webHelper;
		_logger.LogDebug("<<< Exiting CloudflareImagesPlugin(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper})", localizationService, settingService, webHelper);
		_logger.LogDebug("CloudflareImagesPlugin(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper}): _webHelper → {_webHelper}", localizationService, settingService, webHelper, _webHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		_logger.LogDebug(">>> Entering GetConfigurationPageUrl()");
		_logger.LogDebug("<<< Exiting GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/CloudflareImages/Configure";
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		_logger.LogDebug(">>> Entering InstallAsync()");
        //settings
        await _settingService.SaveSettingAsync(new CloudflareImagesSettings
        {
            Enabled = false,
            AccessToken = string.Empty,
            AccountId = string.Empty,
            DeliveryUrl = string.Empty,
            RequestTimeout = CloudflareImagesDefaults.RequestTimeout
        });

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Nop.Plugin.Misc.CloudflareImages.Enabled"] = "Enabled",
            ["Nop.Plugin.Misc.CloudflareImages.Enabled.Hint"] = "Enable this setting to use Cloudflare Images",
            ["Nop.Plugin.Misc.CloudflareImages.Title"] = "Cloudflare Images configuration",
            ["Nop.Plugin.Misc.CloudflareImages.DeliveryUrl"] = "Delivery URL",
            ["Nop.Plugin.Misc.CloudflareImages.DeliveryUrl.Hint"] = "Specify the Image Delivery URL for Cloudflare Images. Normally it looks like https://imagedelivery.net/[account_hash]/<image_id>/<variant_name>. Please just copy it from Cloudflare and live as is.",
            ["Nop.Plugin.Misc.CloudflareImages.RequestTimeout"] = "Request timeout",
            ["Nop.Plugin.Misc.CloudflareImages.RequestTimeout.Hint"] = "Period (in seconds) before the request times out",
            ["Nop.Plugin.Misc.CloudflareImages.AccountId"] = "Account ID",
            ["Nop.Plugin.Misc.CloudflareImages.AccountId.Hint"] = "Cloudflare Images Account ID.",
            ["Nop.Plugin.Misc.CloudflareImages.AccessToken"] = "Access token",
            ["Nop.Plugin.Misc.CloudflareImages.AccessToken.Hint"] = "Cloudflare Images Access token."
        });

        await base.InstallAsync();
		_logger.LogDebug("<<< Exiting InstallAsync()");
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		_logger.LogDebug(">>> Entering UninstallAsync()");
        //settings
        await _settingService.DeleteSettingAsync<CloudflareImagesSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Nop.Plugin.Misc.CloudflareImages");
		_logger.LogDebug("<<< Exiting UninstallAsync()");
    }

    #endregion
}