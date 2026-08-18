using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Seo;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Migrations.UpgradeTo440;

[NopUpdateMigration("2020-06-10 00:00:00", "4.40", UpdateMigrationType.Settings)]
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
        var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
		_logger.LogDebug("Up(): settingRepository → {SettingRepository}", settingRepository);
        var settingService = EngineContext.Current.Resolve<ISettingService>();
		_logger.LogDebug("Up(): settingService → {SettingService}", settingService);

        //#4904 External authentication errors logging
        var externalAuthenticationSettings = settingService.LoadSetting<ExternalAuthenticationSettings>();
		_logger.LogDebug("Up(): externalAuthenticationSettings → {ExternalAuthenticationSettings}", externalAuthenticationSettings);
        if (!settingService.SettingExists(externalAuthenticationSettings, settings => settings.LogErrors))
        {
            externalAuthenticationSettings.LogErrors = false;
			_logger.LogDebug("Up(): externalAuthenticationSettings.LogErrors → {Arg0}", externalAuthenticationSettings.LogErrors);
            settingService.SaveSetting(externalAuthenticationSettings, settings => settings.LogErrors);
        }

        var multiFactorAuthenticationSettings = settingService.LoadSetting<MultiFactorAuthenticationSettings>();
		_logger.LogDebug("Up(): multiFactorAuthenticationSettings → {MultiFactorAuthenticationSettings}", multiFactorAuthenticationSettings);
        if (!settingService.SettingExists(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication))
        {
            multiFactorAuthenticationSettings.ForceMultifactorAuthentication = false;
			_logger.LogDebug("Up(): multiFactorAuthenticationSettings.ForceMultifactorAuthentication → {Arg0}", multiFactorAuthenticationSettings.ForceMultifactorAuthentication);

            settingService.SaveSetting(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication);
        }

        //#5102 Delete Full-text settings
        settingRepository
            .Delete(setting => setting.Name == "commonsettings.usefulltextsearch" || setting.Name == "commonsettings.fulltextmode");

        //#4196
        settingRepository
            .Delete(setting => setting.Name == "commonsettings.scheduletaskruntimeout" ||
                               setting.Name == "commonsettings.staticfilescachecontrol" ||
                               setting.Name == "commonsettings.supportpreviousnopcommerceversions" ||
                               setting.Name == "securitysettings.pluginstaticfileextensionsBlacklist");

        //#5384
        var seoSettings = settingService.LoadSetting<SeoSettings>();
		_logger.LogDebug("Up(): seoSettings → {SeoSettings}", seoSettings);
        foreach (var slug in NopSeoDefaults.ReservedUrlRecordSlugs)
        {
            if (!seoSettings.ReservedUrlRecordSlugs.Contains(slug))
                seoSettings.ReservedUrlRecordSlugs.Add(slug);
        }
        settingService.SaveSetting(seoSettings, settings => seoSettings.ReservedUrlRecordSlugs);

        //#3015
        var homepageTitleKey = $"{nameof(SeoSettings)}.HomepageTitle".ToLower();
		_logger.LogDebug("Up(): homepageTitleKey → {HomepageTitleKey}", homepageTitleKey);
        if (settingService.GetSettingByKey<string>(homepageTitleKey) == null)
            settingService.SetSetting(homepageTitleKey, settingService.GetSettingByKey<string>($"{nameof(SeoSettings)}.DefaultTitle"));

        var homepageDescriptionKey = $"{nameof(SeoSettings)}.HomepageDescription".ToLower();
		_logger.LogDebug("Up(): homepageDescriptionKey → {HomepageDescriptionKey}", homepageDescriptionKey);
        if (settingService.GetSettingByKey<string>(homepageDescriptionKey) == null)
            settingService.SetSetting(homepageDescriptionKey, "Your home page description");

        //#5210
        var adminAreaSettings = settingService.LoadSetting<AdminAreaSettings>();
		_logger.LogDebug("Up(): adminAreaSettings → {AdminAreaSettings}", adminAreaSettings);
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks))
        {
            adminAreaSettings.ShowDocumentationReferenceLinks = true;
			_logger.LogDebug("Up(): adminAreaSettings.ShowDocumentationReferenceLinks → {Arg0}", adminAreaSettings.ShowDocumentationReferenceLinks);
            settingService.SaveSetting(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks);
        }

        //#4944
        var shippingSettings = settingService.LoadSetting<ShippingSettings>();
		_logger.LogDebug("Up(): shippingSettings → {ShippingSettings}", shippingSettings);
        if (!settingService.SettingExists(shippingSettings, settings => settings.RequestDelay))
        {
            shippingSettings.RequestDelay = 300;
			_logger.LogDebug("Up(): shippingSettings.RequestDelay → {Arg0}", shippingSettings.RequestDelay);
            settingService.SaveSetting(shippingSettings, settings => settings.RequestDelay);
        }

        //#276 AJAX filters
        var catalogSettings = settingService.LoadSetting<CatalogSettings>();
		_logger.LogDebug("Up(): catalogSettings → {CatalogSettings}", catalogSettings);
        if (!settingService.SettingExists(catalogSettings, settings => settings.UseAjaxCatalogProductsLoading))
        {
            catalogSettings.UseAjaxCatalogProductsLoading = true;
			_logger.LogDebug("Up(): catalogSettings.UseAjaxCatalogProductsLoading → {Arg0}", catalogSettings.UseAjaxCatalogProductsLoading);
            settingService.SaveSetting(catalogSettings, settings => settings.UseAjaxCatalogProductsLoading);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.EnableManufacturerFiltering))
        {
            catalogSettings.EnableManufacturerFiltering = true;
			_logger.LogDebug("Up(): catalogSettings.EnableManufacturerFiltering → {Arg0}", catalogSettings.EnableManufacturerFiltering);
            settingService.SaveSetting(catalogSettings, settings => settings.EnableManufacturerFiltering);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.EnablePriceRangeFiltering))
        {
            catalogSettings.EnablePriceRangeFiltering = true;
			_logger.LogDebug("Up(): catalogSettings.EnablePriceRangeFiltering → {Arg0}", catalogSettings.EnablePriceRangeFiltering);
            settingService.SaveSetting(catalogSettings, settings => settings.EnablePriceRangeFiltering);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.SearchPagePriceRangeFiltering))
        {
            catalogSettings.SearchPagePriceRangeFiltering = true;
			_logger.LogDebug("Up(): catalogSettings.SearchPagePriceRangeFiltering → {Arg0}", catalogSettings.SearchPagePriceRangeFiltering);
            settingService.SaveSetting(catalogSettings, settings => settings.SearchPagePriceRangeFiltering);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.SearchPagePriceFrom))
        {
            catalogSettings.SearchPagePriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
			_logger.LogDebug("Up(): catalogSettings.SearchPagePriceFrom → {Arg0}", catalogSettings.SearchPagePriceFrom);
            settingService.SaveSetting(catalogSettings, settings => settings.SearchPagePriceFrom);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.SearchPagePriceTo))
        {
            catalogSettings.SearchPagePriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
			_logger.LogDebug("Up(): catalogSettings.SearchPagePriceTo → {Arg0}", catalogSettings.SearchPagePriceTo);
            settingService.SaveSetting(catalogSettings, settings => settings.SearchPagePriceTo);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.SearchPageManuallyPriceRange))
        {
            catalogSettings.SearchPageManuallyPriceRange = false;
			_logger.LogDebug("Up(): catalogSettings.SearchPageManuallyPriceRange → {Arg0}", catalogSettings.SearchPageManuallyPriceRange);
            settingService.SaveSetting(catalogSettings, settings => settings.SearchPageManuallyPriceRange);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.ProductsByTagPriceRangeFiltering))
        {
            catalogSettings.ProductsByTagPriceRangeFiltering = true;
			_logger.LogDebug("Up(): catalogSettings.ProductsByTagPriceRangeFiltering → {Arg0}", catalogSettings.ProductsByTagPriceRangeFiltering);
            settingService.SaveSetting(catalogSettings, settings => settings.ProductsByTagPriceRangeFiltering);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.ProductsByTagPriceFrom))
        {
            catalogSettings.ProductsByTagPriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
			_logger.LogDebug("Up(): catalogSettings.ProductsByTagPriceFrom → {Arg0}", catalogSettings.ProductsByTagPriceFrom);
            settingService.SaveSetting(catalogSettings, settings => settings.ProductsByTagPriceFrom);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.ProductsByTagPriceTo))
        {
            catalogSettings.ProductsByTagPriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
			_logger.LogDebug("Up(): catalogSettings.ProductsByTagPriceTo → {Arg0}", catalogSettings.ProductsByTagPriceTo);
            settingService.SaveSetting(catalogSettings, settings => settings.ProductsByTagPriceTo);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.ProductsByTagManuallyPriceRange))
        {
            catalogSettings.ProductsByTagManuallyPriceRange = false;
			_logger.LogDebug("Up(): catalogSettings.ProductsByTagManuallyPriceRange → {Arg0}", catalogSettings.ProductsByTagManuallyPriceRange);
            settingService.SaveSetting(catalogSettings, settings => settings.ProductsByTagManuallyPriceRange);
        }

        //#4303
        var orderSettings = settingService.LoadSetting<OrderSettings>();
		_logger.LogDebug("Up(): orderSettings → {OrderSettings}", orderSettings);
        if (!settingService.SettingExists(orderSettings, settings => settings.DisplayCustomerCurrencyOnOrders))
        {
            orderSettings.DisplayCustomerCurrencyOnOrders = false;
			_logger.LogDebug("Up(): orderSettings.DisplayCustomerCurrencyOnOrders → {Arg0}", orderSettings.DisplayCustomerCurrencyOnOrders);
            settingService.SaveSetting(orderSettings, settings => settings.DisplayCustomerCurrencyOnOrders);
        }

        //#16 #2909
        if (!settingService.SettingExists(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType))
        {
            catalogSettings.AttributeValueOutOfStockDisplayType = AttributeValueOutOfStockDisplayType.AlwaysDisplay;
			_logger.LogDebug("Up(): catalogSettings.AttributeValueOutOfStockDisplayType → {Arg0}", catalogSettings.AttributeValueOutOfStockDisplayType);
            settingService.SaveSetting(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType);
        }

        //#5482
        settingService.SetSetting("avalarataxsettings.gettaxratebyaddressonly", true);
        settingService.SetSetting("avalarataxsettings.taxratebyaddresscachetime", 480);

        //#5349
        if (!settingService.SettingExists(shippingSettings, settings => settings.EstimateShippingCityNameEnabled))
        {
            shippingSettings.EstimateShippingCityNameEnabled = false;
			_logger.LogDebug("Up(): shippingSettings.EstimateShippingCityNameEnabled → {Arg0}", shippingSettings.EstimateShippingCityNameEnabled);
            settingService.SaveSetting(shippingSettings, settings => settings.EstimateShippingCityNameEnabled);
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