using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.ExchangeRate.EcbExchange.Data;

[NopMigration("2021-09-16 00:00:00", "ExchangeRate.EcbExchange 1.30. Add setting for url for ECB", MigrationProcessType.Update)]
public class ExchangeEcbMigration : MigrationBase
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly EcbExchangeRateSettings _ecbExchangeRateSettings;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public ExchangeEcbMigration(EcbExchangeRateSettings ecbExchangeRateSettings,
        ISettingService settingService)
    {
        _ecbExchangeRateSettings = ecbExchangeRateSettings;
        _settingService = settingService;
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

        //settings
        if (!_settingService.SettingExists(_ecbExchangeRateSettings, settings => settings.EcbLink))
            _ecbExchangeRateSettings.EcbLink = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
			_logger.LogDebug("Up(): _ecbExchangeRateSettings.EcbLink → {Arg0}", _ecbExchangeRateSettings.EcbLink);

        _settingService.SaveSetting(_ecbExchangeRateSettings);
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