using Nop.Core;
using Nop.Plugin.ExternalAuth.Facebook.Components;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.ExternalAuth.Facebook;

/// <summary>
/// Represents method for the authentication with Facebook account
/// </summary>
public class FacebookAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public FacebookAuthenticationMethod(ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering FacebookAuthenticationMethod(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper})", localizationService, settingService, webHelper);
        _localizationService = localizationService;
        _settingService = settingService;
		_logger.LogDebug("FacebookAuthenticationMethod(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper}): _settingService → {_settingService}", localizationService, settingService, webHelper, _settingService);
		_logger.LogDebug("FacebookAuthenticationMethod(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper}): _localizationService → {_localizationService}", localizationService, settingService, webHelper, _localizationService);
        _webHelper = webHelper;
		_logger.LogDebug("<<< Exiting FacebookAuthenticationMethod(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper})", localizationService, settingService, webHelper);
		_logger.LogDebug("FacebookAuthenticationMethod(localizationService={LocalizationService},settingService={SettingService},webHelper={WebHelper}): _webHelper → {_webHelper}", localizationService, settingService, webHelper, _webHelper);
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
        return $"{_webHelper.GetStoreLocation()}Admin/FacebookAuthentication/Configure";
    }

    /// <summary>
    /// Gets a type of a view component for displaying plugin in public store
    /// </summary>
    /// <returns>View component type</returns>
    public Type GetPublicViewComponent()
    {
		_logger.LogDebug(">>> Entering GetPublicViewComponent()");
		_logger.LogDebug("<<< Exiting GetPublicViewComponent()");
        return typeof(FacebookAuthenticationViewComponent);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		_logger.LogDebug(">>> Entering InstallAsync()");
        //settings
        await _settingService.SaveSettingAsync(new FacebookExternalAuthSettings());

        //locales
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.ExternalAuth.Facebook.AuthenticationDataDeletedSuccessfully"] = "Data deletion request completed",
            ["Plugins.ExternalAuth.Facebook.AuthenticationDataExist"] = "Data deletion request is pending, please contact the admin",
            ["Plugins.ExternalAuth.Facebook.ClientKeyIdentifier"] = "App ID/API Key",
            ["Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint"] = "Enter your app ID/API key here. You can find it on your FaceBook application page.",
            ["Plugins.ExternalAuth.Facebook.ClientSecret"] = "App Secret",
            ["Plugins.ExternalAuth.Facebook.ClientSecret.Hint"] = "Enter your app secret here. You can find it on your FaceBook application page.",
            ["Plugins.ExternalAuth.Facebook.Instructions"] = "<p>To configure authentication with Facebook, please follow these steps:<br/><br/><ol><li>Navigate to the <a href=\"https://developers.facebook.com/apps\" target =\"_blank\"> Facebook for Developers</a> page and sign in. If you don't already have a Facebook account, use the <b>Sign up for Facebook</b> link on the login page to create one.</li><li>Tap the <b>+ Add a New App button</b> in the upper right corner to create a new App ID. (If this is your first app with Facebook, the text of the button will be <b>Create a New App</b>.)</li><li>Fill out the form and tap the <b>Create App ID button</b>.</li><li>The <b>Product Setup</b> page is displayed, letting you select the features for your new app. Click <b>Get Started</b> on <b>Facebook Login</b>.</li><li>Click the <b>Settings</b> link in the menu at the left, you are presented with the <b>Client OAuth Settings</b> page with some defaults already set.</li><li>Enter \"{0:s}signin-facebook\" into the <b>Valid OAuth Redirect URIs</b> field.</li><li>From User data deletion dropdown menu select \"Data deletion instructions URL\" </li><li> Enter \"{0:s}facebook/data-deletion-callback/\" into the <b> User data deletion </b> input field.</li><li>Click <b>Save Changes</b>.</li><li>Click the <b>Dashboard</b> link in the left navigation.</li><li>Copy your App ID and App secret below.</li></ol><br/><br/></p>"
        });
		_logger.LogInformation("InstallAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        await base.InstallAsync();
		_logger.LogDebug("<<< Exiting InstallAsync()");
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		_logger.LogDebug(">>> Entering UninstallAsync()");
        //settings
        await _settingService.DeleteSettingAsync<FacebookExternalAuthSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.ExternalAuth.Facebook");

        await base.UninstallAsync();
		_logger.LogDebug("<<< Exiting UninstallAsync()");
    }

    #endregion
}