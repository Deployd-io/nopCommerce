using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Migrations.UpgradeTo470;

[NopUpdateMigration("2023-02-01 14:00:03", "4.70", UpdateMigrationType.Settings)]
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

        var customerSettings = settingService.LoadSetting<CustomerSettings>();
		_logger.LogDebug("Up(): customerSettings → {CustomerSettings}", customerSettings);
        if (!settingService.SettingExists(customerSettings, settings => settings.PasswordMaxLength))
        {
            customerSettings.PasswordMaxLength = 64;
			_logger.LogDebug("Up(): customerSettings.PasswordMaxLength → {Arg0}", customerSettings.PasswordMaxLength);
            settingService.SaveSetting(customerSettings, settings => settings.PasswordMaxLength);
        }

        if (!settingService.SettingExists(customerSettings, settings => settings.DefaultCountryId))
        {
            customerSettings.DefaultCountryId = null;
			_logger.LogDebug("Up(): customerSettings.DefaultCountryId → {Arg0}", customerSettings.DefaultCountryId);
            settingService.SaveSetting(customerSettings, settings => settings.DefaultCountryId);
        }

        var securitySettings = settingService.LoadSetting<SecuritySettings>();
		_logger.LogDebug("Up(): securitySettings → {SecuritySettings}", securitySettings);
        if (!settingService.SettingExists(securitySettings, settings => settings.UseAesEncryptionAlgorithm))
        {
            securitySettings.UseAesEncryptionAlgorithm = false;
			_logger.LogDebug("Up(): securitySettings.UseAesEncryptionAlgorithm → {Arg0}", securitySettings.UseAesEncryptionAlgorithm);
            settingService.SaveSetting(securitySettings, settings => settings.UseAesEncryptionAlgorithm);
        }

        if (!settingService.SettingExists(securitySettings, settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword))
        {
            securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword = true;
			_logger.LogDebug("Up(): securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword → {Arg0}", securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword);
            settingService.SaveSetting(securitySettings, settings => settings.AllowStoreOwnerExportImportCustomersWithHashedPassword);
        }

        //#7053
        if (!settingService.SettingExists(securitySettings, settings => settings.LogHoneypotDetection))
        {
            securitySettings.LogHoneypotDetection = true;
			_logger.LogDebug("Up(): securitySettings.LogHoneypotDetection → {Arg0}", securitySettings.LogHoneypotDetection);
            settingService.SaveSetting(securitySettings, settings => settings.LogHoneypotDetection);
        }

        var addressSettings = settingService.LoadSetting<AddressSettings>();
		_logger.LogDebug("Up(): addressSettings → {AddressSettings}", addressSettings);
        if (!settingService.SettingExists(addressSettings, settings => settings.DefaultCountryId))
        {
            addressSettings.DefaultCountryId = null;
			_logger.LogDebug("Up(): addressSettings.DefaultCountryId → {Arg0}", addressSettings.DefaultCountryId);
            settingService.SaveSetting(addressSettings, settings => settings.DefaultCountryId);
        }

        var captchaSettings = settingService.LoadSetting<CaptchaSettings>();
		_logger.LogDebug("Up(): captchaSettings → {CaptchaSettings}", captchaSettings);
        //#6682
        if (!settingService.SettingExists(captchaSettings, settings => settings.ShowOnNewsletterPage))
        {
            captchaSettings.ShowOnNewsletterPage = false;
			_logger.LogDebug("Up(): captchaSettings.ShowOnNewsletterPage → {Arg0}", captchaSettings.ShowOnNewsletterPage);
            settingService.SaveSetting(captchaSettings, settings => settings.ShowOnNewsletterPage);
        }

        var taxSettings = settingService.LoadSetting<TaxSettings>();
		_logger.LogDebug("Up(): taxSettings → {TaxSettings}", taxSettings);
        if (!settingService.SettingExists(taxSettings, settings => settings.AutomaticallyDetectCountry))
        {
            taxSettings.AutomaticallyDetectCountry = true;
			_logger.LogDebug("Up(): taxSettings.AutomaticallyDetectCountry → {Arg0}", taxSettings.AutomaticallyDetectCountry);
            settingService.SaveSetting(taxSettings, settings => settings.AutomaticallyDetectCountry);
        }

        //#6716
        var newDisallowPaths = new[]
        {
            "/cart/estimateshipping", "/cart/selectshippingoption", "/customer/addressdelete",
            "/customer/removeexternalassociation", "/customer/checkusernameavailability",
            "/catalog/searchtermautocomplete", "/catalog/getcatalogroot", "/addproducttocart/catalog/*",
            "/addproducttocart/details/*", "/compareproducts/add/*", "/backinstocksubscribe/*",
            "/subscribenewsletter", "/t-popup/*", "/setproductreviewhelpfulness", "/poll/vote",
            "/country/getstatesbycountryid/", "/eucookielawaccept", "/topic/authenticate",
            "/category/products/", "/product/combinations", "/uploadfileproductattribute/*",
            "/shoppingcart/productdetails_attributechange/*", "/uploadfilereturnrequest",
            "/boards/topicwatch/*", "/boards/forumwatch/*", "/install/restartapplication",
            "/boards/postvote", "/product/estimateshipping/*", "/shoppingcart/checkoutattributechange/*"
        };

        var robotsTxtSettings = settingService.LoadSetting<RobotsTxtSettings>();
		_logger.LogDebug("Up(): robotsTxtSettings → {RobotsTxtSettings}", robotsTxtSettings);

        foreach (var path in newDisallowPaths)
        {
            if (robotsTxtSettings.DisallowPaths.Contains(path))
                continue;

            robotsTxtSettings.DisallowPaths.Add(path);
        }

        settingService.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);

        //#6853
        if (!settingService.SettingExists(customerSettings, settings => settings.NeutralGenderEnabled))
        {
            customerSettings.NeutralGenderEnabled = false;
			_logger.LogDebug("Up(): customerSettings.NeutralGenderEnabled → {Arg0}", customerSettings.NeutralGenderEnabled);
            settingService.SaveSetting(customerSettings, settings => settings.NeutralGenderEnabled);
        }

        //#6891
        if (!settingService.SettingExists(customerSettings, settings => settings.RequiredReLoginAfterPasswordChange))
        {
            customerSettings.RequiredReLoginAfterPasswordChange = false;
			_logger.LogDebug("Up(): customerSettings.RequiredReLoginAfterPasswordChange → {Arg0}", customerSettings.RequiredReLoginAfterPasswordChange);
            settingService.SaveSetting(customerSettings, settings => settings.RequiredReLoginAfterPasswordChange);
        }

        //#7064
        var catalogSettings = settingService.LoadSetting<CatalogSettings>();
		_logger.LogDebug("Up(): catalogSettings → {CatalogSettings}", catalogSettings);
        if (!settingService.SettingExists(catalogSettings, settings => settings.UseStandardSearchWhenSearchProviderThrowsException))
        {
            catalogSettings.UseStandardSearchWhenSearchProviderThrowsException = true;
			_logger.LogDebug("Up(): catalogSettings.UseStandardSearchWhenSearchProviderThrowsException → {Arg0}", catalogSettings.UseStandardSearchWhenSearchProviderThrowsException);
            settingService.SaveSetting(catalogSettings, settings => settings.UseStandardSearchWhenSearchProviderThrowsException);
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