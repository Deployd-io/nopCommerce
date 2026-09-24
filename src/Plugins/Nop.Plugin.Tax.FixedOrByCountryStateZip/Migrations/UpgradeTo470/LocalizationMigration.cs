using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Migrations.UpgradeTo470;

[NopMigration("2024-04-19 12:00:00", "Tax.FixedOrByCountryStateZip 4.70.5. Update localizations", MigrationProcessType.Update)]
public class LocalizationMigration : MigrationBase
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override void Down()
    {
		_logger.LogError(">>> Entering Down()");
        //add the downgrade logic if necessary 
		_logger.LogWarning("<<< Exiting Down()");
    }

    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_logger.LogError("Up(): localizationService → {LocalizationService}", localizationService);

        var (languageId, _) = this.GetLanguageData();

        //use localizationService to add, update and delete localization resources
        localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Tax.FixedOrByCountryStateZip.SwitchRate"] = @"
                    <p>
                        You are going to change the way the tax rate is calculated. This will cause the tax rate to be calculated based on the settings specified on the configuration page.
                    </p>
                    <p>
                        Any current tax rate settings will be saved, but will not be active until you return to this tax calculation method.
                    </p>",

        }, languageId);
		_logger.LogWarning("<<< Exiting Up()");
    }
}
