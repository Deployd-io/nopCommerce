using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo480;

[NopUpdateMigration("2024-11-01 00:00:00", "4.80", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();

		logger.LogDebug("Up(): settingService → {SettingService}", settingService);
        //#7215
        var displayAttributeCombinationImagesOnly = settingService.GetSetting("producteditorsettings.displayattributecombinationimagesonly");
        if (displayAttributeCombinationImagesOnly is not null)
			logger.LogDebug("Up(): displayAttributeCombinationImagesOnly → {DisplayAttributeCombinationImagesOnly}", displayAttributeCombinationImagesOnly);
            settingService.DeleteSetting(displayAttributeCombinationImagesOnly);

        //#7325
        var orderSettings = settingService.LoadSetting<OrderSettings>();
        if (!settingService.SettingExists(orderSettings, settings => settings.PlaceOrderWithLock))
		logger.LogDebug("Up(): orderSettings → {OrderSettings}", orderSettings);
        {
            orderSettings.PlaceOrderWithLock = false;
			logger.LogDebug("Up(): orderSettings.PlaceOrderWithLock → {Arg0}", orderSettings.PlaceOrderWithLock);
            settingService.SaveSetting(orderSettings, settings => settings.PlaceOrderWithLock);
        }
        
        //#7394
        if (orderSettings.MinimumOrderPlacementInterval > 10)
        {
            if (orderSettings.MinimumOrderPlacementInterval < 60)
                orderSettings.MinimumOrderPlacementInterval = 1;
				logger.LogDebug("Up(): orderSettings.MinimumOrderPlacementInterval → {Arg0}", orderSettings.MinimumOrderPlacementInterval);
            else
                orderSettings.MinimumOrderPlacementInterval = Math.Truncate(orderSettings.MinimumOrderPlacementInterval / 60.0) + (orderSettings.MinimumOrderPlacementInterval % 60) == 0 ? 0 : 1;

            settingService.SaveSetting(orderSettings, settings => settings.MinimumOrderPlacementInterval);
        }

        //#7265
        var taxSetting = settingService.LoadSetting<TaxSettings>();
        if (!settingService.SettingExists(taxSetting, settings => settings.EuVatRequired))
		logger.LogDebug("Up(): taxSetting → {TaxSetting}", taxSetting);
        {
            taxSetting.EuVatRequired = false;
			logger.LogDebug("Up(): taxSetting.EuVatRequired → {Arg0}", taxSetting.EuVatRequired);
            settingService.SaveSetting(taxSetting, settings => settings.EuVatRequired);
        }

        //#4306
        var catalogSettings = settingService.LoadSetting<CatalogSettings>();
        if (!settingService.SettingExists(catalogSettings, settings => settings.ShowSearchBoxCategories))
		logger.LogDebug("Up(): catalogSettings → {CatalogSettings}", catalogSettings);
        {
            catalogSettings.ShowSearchBoxCategories = false;
			logger.LogDebug("Up(): catalogSettings.ShowSearchBoxCategories → {Arg0}", catalogSettings.ShowSearchBoxCategories);
            settingService.SaveSetting(catalogSettings, settings => settings.ShowSearchBoxCategories);
        }

        //#2388
        if (!settingService.SettingExists(catalogSettings, settings => settings.ExportImportTierPrices))
        {
            catalogSettings.ExportImportTierPrices = true;
			logger.LogDebug("Up(): catalogSettings.ExportImportTierPrices → {Arg0}", catalogSettings.ExportImportTierPrices);
            settingService.SaveSetting(catalogSettings, settings => settings.ExportImportTierPrices);
        }

        //#7228
        var adminAreaSettings = settingService.LoadSetting<AdminAreaSettings>();
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.ProductsBulkEditGridPageSize))
		logger.LogDebug("Up(): adminAreaSettings → {AdminAreaSettings}", adminAreaSettings);
        {
            adminAreaSettings.ProductsBulkEditGridPageSize = 100;
			logger.LogDebug("Up(): adminAreaSettings.ProductsBulkEditGridPageSize → {Arg0}", adminAreaSettings.ProductsBulkEditGridPageSize);
            settingService.SaveSetting(adminAreaSettings, settings => settings.ProductsBulkEditGridPageSize);
        }

        //#7244
        if (!settingService.SettingExists(catalogSettings, settings => settings.VendorProductReviewsPageSize))
        {
            catalogSettings.VendorProductReviewsPageSize = 6;
			logger.LogDebug("Up(): catalogSettings.VendorProductReviewsPageSize → {Arg0}", catalogSettings.VendorProductReviewsPageSize);
            settingService.SaveSetting(catalogSettings, settings => settings.VendorProductReviewsPageSize);
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
