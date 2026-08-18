using FluentMigrator;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Data;

[NopMigration("2022-03-18 12:00:00", "Widgets.FacebookPixel 2.00. Conversions API", MigrationProcessType.Update)]
public class ConversionsApiMigration : MigrationBase
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public ConversionsApiMigration(ILanguageService languageService,
        ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ConversionsApiMigration(languageService={LanguageService},localizationService={LocalizationService})", languageService, localizationService);
        _languageService = languageService;
        _localizationService = localizationService;
		_logger.LogDebug("ConversionsApiMigration(languageService={LanguageService},localizationService={LocalizationService}): _localizationService → {_localizationService}", languageService, localizationService, _localizationService);
		_logger.LogDebug("ConversionsApiMigration(languageService={LanguageService},localizationService={LocalizationService}): _languageService → {_languageService}", languageService, localizationService, _languageService);
		_logger.LogDebug("<<< Exiting ConversionsApiMigration(languageService={LanguageService},localizationService={LocalizationService})", languageService, localizationService);
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

        var facebookPixelConfigurationTableName = NameCompatibilityManager.GetTableName(typeof(FacebookPixelConfiguration));
		_logger.LogDebug("Up(): facebookPixelConfigurationTableName → {FacebookPixelConfigurationTableName}", facebookPixelConfigurationTableName);

        //add ConversionsApiEnabled column if not exists
        if (!Schema.Table(facebookPixelConfigurationTableName).Column(nameof(FacebookPixelConfiguration.ConversionsApiEnabled)).Exists())
        {
            Alter.Table(facebookPixelConfigurationTableName)
                .AddColumn(nameof(FacebookPixelConfiguration.ConversionsApiEnabled))
                .AsBoolean().NotNullable().SetExistingRowsTo(false);
        }

        //add AccessToken column if not exists
        if (!Schema.Table(facebookPixelConfigurationTableName).Column(nameof(FacebookPixelConfiguration.AccessToken)).Exists())
        {
            Alter.Table(facebookPixelConfigurationTableName)
                .AddColumn(nameof(FacebookPixelConfiguration.AccessToken)).AsString().Nullable();
        }

        //rename Enabled column to PixelScriptEnabled
        if (Schema.Table(facebookPixelConfigurationTableName).Column("Enabled").Exists())
        {
            Rename.Column("Enabled")
                .OnTable(facebookPixelConfigurationTableName)
                .To(nameof(FacebookPixelConfiguration.PixelScriptEnabled));
        }

        //locales
        var (languageId, _) = this.GetLanguageData();

        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken"] = "Access token",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Hint"] = "Enter the Facebook Conversions API access token.",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Required"] = "Access token is required",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled"] = "Pixel enabled",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled.Hint"] = "Toggle to enable/disable Facebook Pixel for this configuration.",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled"] = "Conversions API enabled",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled.Hint"] = "Toggle to enable/disable Facebook Conversions API for this configuration."
        }, languageId);

        _localizationService.DeleteLocaleResources(new List<string>
        {
            "Plugins.Widgets.FacebookPixel.Configuration.Fields.Enabled",
            "Plugins.Widgets.FacebookPixel.Configuration.Fields.Enabled.Hint"
        });
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