using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo450;

[NopUpdateMigration("2021-04-23 00:00:00", "4.50", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
        var settingService = EngineContext.Current.Resolve<ISettingService>();
		logger.LogDebug("Up(): settingRepository → {SettingRepository}", settingRepository);
		logger.LogDebug("Up(): settingService → {SettingService}", settingService);

        //miniprofiler settings are moved to appSettings
        settingRepository
            .Delete(setting => setting.Name == "storeinformationsettings.displayminiprofilerforadminonly" ||
                               setting.Name == "storeinformationsettings.displayminiprofilerinpublicstore");

        //#4363
        var commonSettings = settingService.LoadSetting<CommonSettings>();

		logger.LogDebug("Up(): commonSettings → {CommonSettings}", commonSettings);
        if (!settingService.SettingExists(commonSettings, settings => settings.ClearLogOlderThanDays))
        {
            commonSettings.ClearLogOlderThanDays = 0;
			logger.LogDebug("Up(): commonSettings.ClearLogOlderThanDays → {Arg0}", commonSettings.ClearLogOlderThanDays);
            settingService.SaveSetting(commonSettings, settings => settings.ClearLogOlderThanDays);
        }

        //#5551
        var catalogSettings = settingService.LoadSetting<CatalogSettings>();

		logger.LogDebug("Up(): catalogSettings → {CatalogSettings}", catalogSettings);
        if (!settingService.SettingExists(catalogSettings, settings => settings.EnableSpecificationAttributeFiltering))
        {
            catalogSettings.EnableSpecificationAttributeFiltering = true;
			logger.LogDebug("Up(): catalogSettings.EnableSpecificationAttributeFiltering → {Arg0}", catalogSettings.EnableSpecificationAttributeFiltering);
            settingService.SaveSetting(catalogSettings, settings => settings.EnableSpecificationAttributeFiltering);
        }

        //#5204
        var shippingSettings = settingService.LoadSetting<ShippingSettings>();

		logger.LogDebug("Up(): shippingSettings → {ShippingSettings}", shippingSettings);
        if (!settingService.SettingExists(shippingSettings, settings => settings.ShippingSorting))
        {
            shippingSettings.ShippingSorting = ShippingSortingEnum.Position;
			logger.LogDebug("Up(): shippingSettings.ShippingSorting → {Arg0}", shippingSettings.ShippingSorting);
            settingService.SaveSetting(shippingSettings, settings => settings.ShippingSorting);
        }

        //#5698
        var orderSettings = settingService.LoadSetting<OrderSettings>();
        if (!settingService.SettingExists(orderSettings, settings => settings.DisplayOrderSummary))
		logger.LogDebug("Up(): orderSettings → {OrderSettings}", orderSettings);
        {
            orderSettings.DisplayOrderSummary = true;
			logger.LogDebug("Up(): orderSettings.DisplayOrderSummary → {Arg0}", orderSettings.DisplayOrderSummary);
            settingService.SaveSetting(orderSettings, settings => settings.DisplayOrderSummary);
        }
		logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down()
    {
		logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		logger.LogDebug("<<< Exiting Down()");
    }
}
