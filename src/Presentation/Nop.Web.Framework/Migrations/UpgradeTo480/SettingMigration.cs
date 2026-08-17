using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Migrations.UpgradeTo480;

[NopUpdateMigration("2024-11-01 00:00:00", "4.80", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();
		_logger.LogDebug("Up(): settingService → {SettingService}", settingService);

        //#7215
        var displayAttributeCombinationImagesOnly = settingService.GetSetting("producteditorsettings.displayattributecombinationimagesonly");
		_logger.LogDebug("Up(): displayAttributeCombinationImagesOnly → {DisplayAttributeCombinationImagesOnly}", displayAttributeCombinationImagesOnly);
        if (displayAttributeCombinationImagesOnly is not null)
            settingService.DeleteSetting(displayAttributeCombinationImagesOnly);

        //#7325
        var orderSettings = settingService.LoadSetting<OrderSettings>();
		_logger.LogDebug("Up(): orderSettings → {OrderSettings}", orderSettings);
        if (!settingService.SettingExists(orderSettings, settings => settings.PlaceOrderWithLock))
        {
            orderSettings.PlaceOrderWithLock = false;
			_logger.LogDebug("Up(): orderSettings.PlaceOrderWithLock → {Arg0}", orderSettings.PlaceOrderWithLock);
            settingService.SaveSetting(orderSettings, settings => settings.PlaceOrderWithLock);
        }
        
        //#7394
        if (orderSettings.MinimumOrderPlacementInterval > 10)
        {
            if (orderSettings.MinimumOrderPlacementInterval < 60)
                orderSettings.MinimumOrderPlacementInterval = 1;
				_logger.LogDebug("Up(): orderSettings.MinimumOrderPlacementInterval → {Arg0}", orderSettings.MinimumOrderPlacementInterval);
            else
                orderSettings.MinimumOrderPlacementInterval = Math.Truncate(orderSettings.MinimumOrderPlacementInterval / 60.0) + (orderSettings.MinimumOrderPlacementInterval % 60) == 0 ? 0 : 1;

            settingService.SaveSetting(orderSettings, settings => settings.MinimumOrderPlacementInterval);
        }

        //#7265
        var taxSetting = settingService.LoadSetting<TaxSettings>();
		_logger.LogDebug("Up(): taxSetting → {TaxSetting}", taxSetting);
        if (!settingService.SettingExists(taxSetting, settings => settings.EuVatRequired))
        {
            taxSetting.EuVatRequired = false;
			_logger.LogDebug("Up(): taxSetting.EuVatRequired → {Arg0}", taxSetting.EuVatRequired);
            settingService.SaveSetting(taxSetting, settings => settings.EuVatRequired);
        }

        //#4306
        var catalogSettings = settingService.LoadSetting<CatalogSettings>();
		_logger.LogDebug("Up(): catalogSettings → {CatalogSettings}", catalogSettings);
        if (!settingService.SettingExists(catalogSettings, settings => settings.ShowSearchBoxCategories))
        {
            catalogSettings.ShowSearchBoxCategories = false;
			_logger.LogDebug("Up(): catalogSettings.ShowSearchBoxCategories → {Arg0}", catalogSettings.ShowSearchBoxCategories);
            settingService.SaveSetting(catalogSettings, settings => settings.ShowSearchBoxCategories);
        }

        //#2388
        if (!settingService.SettingExists(catalogSettings, settings => settings.ExportImportTierPrices))
        {
            catalogSettings.ExportImportTierPrices = true;
			_logger.LogDebug("Up(): catalogSettings.ExportImportTierPrices → {Arg0}", catalogSettings.ExportImportTierPrices);
            settingService.SaveSetting(catalogSettings, settings => settings.ExportImportTierPrices);
        }

        //#7228
        var adminAreaSettings = settingService.LoadSetting<AdminAreaSettings>();
		_logger.LogDebug("Up(): adminAreaSettings → {AdminAreaSettings}", adminAreaSettings);
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.ProductsBulkEditGridPageSize))
        {
            adminAreaSettings.ProductsBulkEditGridPageSize = 100;
			_logger.LogDebug("Up(): adminAreaSettings.ProductsBulkEditGridPageSize → {Arg0}", adminAreaSettings.ProductsBulkEditGridPageSize);
            settingService.SaveSetting(adminAreaSettings, settings => settings.ProductsBulkEditGridPageSize);
        }

        //#7244
        if (!settingService.SettingExists(catalogSettings, settings => settings.VendorProductReviewsPageSize))
        {
            catalogSettings.VendorProductReviewsPageSize = 6;
			_logger.LogDebug("Up(): catalogSettings.VendorProductReviewsPageSize → {Arg0}", catalogSettings.VendorProductReviewsPageSize);
            settingService.SaveSetting(catalogSettings, settings => settings.VendorProductReviewsPageSize);
        }
		_logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down()
    {
		_logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		_logger.LogDebug("<<< Exiting Down()");
    }
}