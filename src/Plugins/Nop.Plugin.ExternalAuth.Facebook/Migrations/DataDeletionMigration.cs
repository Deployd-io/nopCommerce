using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.ExternalAuth.Facebook.Migrations;

[NopMigration("2022-06-23 00:00:00", "ExternalAuth.Facebook 1.77. Data deletion feature", MigrationProcessType.Update)]
public class DataDeletionMigration : MigrationBase
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public DataDeletionMigration(ILanguageService languageService,
        ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering DataDeletionMigration(languageService={LanguageService},localizationService={LocalizationService})", languageService, localizationService);
        _languageService = languageService;
        _localizationService = localizationService;
		_logger.LogDebug("<<< Exiting DataDeletionMigration(languageService={LanguageService},localizationService={LocalizationService})", languageService, localizationService);
		_logger.LogDebug("DataDeletionMigration(languageService={LanguageService},localizationService={LocalizationService}): _localizationService → {_localizationService}", languageService, localizationService, _localizationService);
		_logger.LogDebug("DataDeletionMigration(languageService={LanguageService},localizationService={LocalizationService}): _languageService → {_languageService}", languageService, localizationService, _languageService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var (languageId, _) = this.GetLanguageData(_languageService);

        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.ExternalAuth.Facebook.AuthenticationDataDeletedSuccessfully"] = "Data deletion request completed",
            ["Plugins.ExternalAuth.Facebook.AuthenticationDataExist"] = "Data deletion request is pending, please contact the admin",
            ["Plugins.ExternalAuth.Facebook.Instructions"] = "<p>To configure authentication with Facebook, please follow these steps:<br/><br/><ol><li>Navigate to the <a href=\"https://developers.facebook.com/apps\" target =\"_blank\"> Facebook for Developers</a> page and sign in. If you don't already have a Facebook account, use the <b>Sign up for Facebook</b> link on the login page to create one.</li><li>Tap the <b>+ Add a New App button</b> in the upper right corner to create a new App ID. (If this is your first app with Facebook, the text of the button will be <b>Create a New App</b>.)</li><li>Fill out the form and tap the <b>Create App ID button</b>.</li><li>The <b>Product Setup</b> page is displayed, letting you select the features for your new app. Click <b>Get Started</b> on <b>Facebook Login</b>.</li><li>Click the <b>Settings</b> link in the menu at the left, you are presented with the <b>Client OAuth Settings</b> page with some defaults already set.</li><li>Enter \"{0:s}signin-facebook\" into the <b>Valid OAuth Redirect URIs</b> field.</li><li>From User data deletion dropdown menu select \"Data deletion instructions URL\" </li><li> Enter \"{0:s}facebook/data-deletion-callback/\" into the <b> User data deletion </b> input field.</li><li>Click <b>Save Changes</b>.</li><li>Click the <b>Dashboard</b> link in the left navigation.</li><li>Copy your App ID and App secret below.</li></ol><br/><br/></p>"
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        }, languageId);
		_logger.LogInformation("Up(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting Up()");
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
		_logger.LogDebug(">>> Entering Down()");
        //nothing
		_logger.LogDebug("<<< Exiting Down()");
    }

    #endregion
}