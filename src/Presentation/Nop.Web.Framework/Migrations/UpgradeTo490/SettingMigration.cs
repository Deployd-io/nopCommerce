using FluentMigrator;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Translation;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopUpdateMigration("2025-02-26 00:00:00", "4.90", UpdateMigrationType.Settings)]
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

        //#6590
        var adminAreaSettings = settingService.LoadSetting<AdminAreaSettings>();
		_logger.LogDebug("Up(): adminAreaSettings → {AdminAreaSettings}", adminAreaSettings);
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.UseStickyHeaderLayout))
        {
            adminAreaSettings.UseStickyHeaderLayout = false;
			_logger.LogDebug("Up(): adminAreaSettings.UseStickyHeaderLayout → {Arg0}", adminAreaSettings.UseStickyHeaderLayout);
            settingService.SaveSetting(adminAreaSettings, settings => settings.UseStickyHeaderLayout);
        }

        //#7387
        var productEditorSettings = settingService.LoadSetting<ProductEditorSettings>();
		_logger.LogDebug("Up(): productEditorSettings → {ProductEditorSettings}", productEditorSettings);
        if (!settingService.SettingExists(productEditorSettings, settings => settings.AgeVerification))
        {
            productEditorSettings.AgeVerification = false;
			_logger.LogDebug("Up(): productEditorSettings.AgeVerification → {Arg0}", productEditorSettings.AgeVerification);
            settingService.SaveSetting(productEditorSettings, settings => settings.AgeVerification);
        }

        //#2184
        var vendorSettings = settingService.LoadSetting<VendorSettings>();
		_logger.LogDebug("Up(): vendorSettings → {VendorSettings}", vendorSettings);
        if (!settingService.SettingExists(vendorSettings, settings => settings.MaximumProductPicturesNumber))
        {
            vendorSettings.MaximumProductPicturesNumber = 5;
			_logger.LogDebug("Up(): vendorSettings.MaximumProductPicturesNumber → {Arg0}", vendorSettings.MaximumProductPicturesNumber);
            settingService.SaveSetting(vendorSettings, settings => settings.MaximumProductPicturesNumber);
        }

        //#7571
        var captchaSettings = settingService.LoadSetting<CaptchaSettings>();
		_logger.LogDebug("Up(): captchaSettings → {CaptchaSettings}", captchaSettings);
        if (!settingService.SettingExists(captchaSettings, settings => settings.ShowOnCheckGiftCardBalance))
        {
            captchaSettings.ShowOnCheckGiftCardBalance = true;
			_logger.LogDebug("Up(): captchaSettings.ShowOnCheckGiftCardBalance → {Arg0}", captchaSettings.ShowOnCheckGiftCardBalance);
            settingService.SaveSetting(captchaSettings, settings => settings.ShowOnCheckGiftCardBalance);
        }

        //#5818
        var mediaSettings = settingService.LoadSetting<MediaSettings>();
		_logger.LogDebug("Up(): mediaSettings → {MediaSettings}", mediaSettings);
        if (!settingService.SettingExists(mediaSettings, settings => settings.AutoOrientImage))
        {
            mediaSettings.AutoOrientImage = false;
			_logger.LogDebug("Up(): mediaSettings.AutoOrientImage → {Arg0}", mediaSettings.AutoOrientImage);
            settingService.SaveSetting(mediaSettings, settings => settings.AutoOrientImage);
        }

        //#1892
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.MinimumDropdownItemsForSearch))
        {
            adminAreaSettings.MinimumDropdownItemsForSearch = 50;
			_logger.LogDebug("Up(): adminAreaSettings.MinimumDropdownItemsForSearch → {Arg0}", adminAreaSettings.MinimumDropdownItemsForSearch);
            settingService.SaveSetting(adminAreaSettings, settings => settings.MinimumDropdownItemsForSearch);
        }

        //#7405
        var catalogSettings = settingService.LoadSetting<CatalogSettings>();
		_logger.LogDebug("Up(): catalogSettings → {CatalogSettings}", catalogSettings);
        if (!settingService.SettingExists(catalogSettings, settings => settings.ExportImportCategoryUseLimitedToStores))
        {
            catalogSettings.ExportImportCategoryUseLimitedToStores = false;
			_logger.LogDebug("Up(): catalogSettings.ExportImportCategoryUseLimitedToStores → {Arg0}", catalogSettings.ExportImportCategoryUseLimitedToStores);
            settingService.SaveSetting(catalogSettings, settings => settings.ExportImportCategoryUseLimitedToStores);
        }

        //#7477
        var pdfSettings = settingService.LoadSetting<PdfSettings>();
		_logger.LogDebug("Up(): pdfSettings → {PdfSettings}", pdfSettings);
        var pdfSettingsFontFamily = settingService.GetSetting("pdfsettings.fontfamily");
		_logger.LogDebug("Up(): pdfSettingsFontFamily → {PdfSettingsFontFamily}", pdfSettingsFontFamily);
        if (pdfSettingsFontFamily is not null)
            settingService.DeleteSetting(pdfSettingsFontFamily);

        if (!settingService.SettingExists(pdfSettings, settings => settings.RtlFontName))
        {
            pdfSettings.RtlFontName = NopCommonDefaults.PdfRtlFontName;
			_logger.LogDebug("Up(): pdfSettings.RtlFontName → {Arg0}", pdfSettings.RtlFontName);
            settingService.SaveSetting(pdfSettings, settings => pdfSettings.RtlFontName);
        }

        if (!settingService.SettingExists(pdfSettings, settings => settings.LtrFontName))
        {
            pdfSettings.LtrFontName = NopCommonDefaults.PdfLtrFontName;
			_logger.LogDebug("Up(): pdfSettings.LtrFontName → {Arg0}", pdfSettings.LtrFontName);
            settingService.SaveSetting(pdfSettings, settings => pdfSettings.LtrFontName);
        }

        if (!settingService.SettingExists(pdfSettings, settings => settings.BaseFontSize))
        {
            pdfSettings.BaseFontSize = 10f;
			_logger.LogDebug("Up(): pdfSettings.BaseFontSize → {Arg0}", pdfSettings.BaseFontSize);
            settingService.SaveSetting(pdfSettings, settings => pdfSettings.BaseFontSize);
        }

        if (!settingService.SettingExists(pdfSettings, settings => settings.ImageTargetSize))
        {
            pdfSettings.ImageTargetSize = 200;
			_logger.LogDebug("Up(): pdfSettings.ImageTargetSize → {Arg0}", pdfSettings.ImageTargetSize);
            settingService.SaveSetting(pdfSettings, settings => pdfSettings.ImageTargetSize);
        }

        //#7397
        var richEditorAllowStyleTag = settingService.GetSetting("adminareasettings.richeditorallowstyletag");
		_logger.LogDebug("Up(): richEditorAllowStyleTag → {RichEditorAllowStyleTag}", richEditorAllowStyleTag);
        if (richEditorAllowStyleTag is not null)
            settingService.DeleteSetting(richEditorAllowStyleTag);

        if (settingService.SettingExists(adminAreaSettings, settings => settings.RichEditorAdditionalSettings))
        {
            adminAreaSettings.RichEditorAdditionalSettings = string.Empty;
			_logger.LogDebug("Up(): adminAreaSettings.RichEditorAdditionalSettings → {Arg0}", adminAreaSettings.RichEditorAdditionalSettings);
            settingService.SaveSetting(adminAreaSettings, settings => settings.RichEditorAdditionalSettings);
        }

        //#6874
        var newsletterTickedByDefault = settingService.GetSetting("customersettings.newslettertickedbydefault");
		_logger.LogDebug("Up(): newsletterTickedByDefault → {NewsletterTickedByDefault}", newsletterTickedByDefault);
        if (newsletterTickedByDefault is not null)
            settingService.DeleteSetting(newsletterTickedByDefault);

        //#820
        var currencySettings = settingService.LoadSetting<CurrencySettings>();
		_logger.LogDebug("Up(): currencySettings → {CurrencySettings}", currencySettings);
        if (!settingService.SettingExists(currencySettings, settings => settings.DisplayCurrencySymbolInCurrencySelector))
        {
            currencySettings.DisplayCurrencySymbolInCurrencySelector = false;
			_logger.LogDebug("Up(): currencySettings.DisplayCurrencySymbolInCurrencySelector → {Arg0}", currencySettings.DisplayCurrencySymbolInCurrencySelector);
            settingService.SaveSetting(currencySettings, settings => settings.DisplayCurrencySymbolInCurrencySelector);
        }

        //#1779
        var customerSettings = settingService.LoadSetting<CustomerSettings>();
		_logger.LogDebug("Up(): customerSettings → {CustomerSettings}", customerSettings);
        if (!settingService.SettingExists(customerSettings, settings => settings.NotifyFailedLoginAttempt))
        {
            customerSettings.NotifyFailedLoginAttempt = false;
			_logger.LogDebug("Up(): customerSettings.NotifyFailedLoginAttempt → {Arg0}", customerSettings.NotifyFailedLoginAttempt);
            settingService.SaveSetting(customerSettings, settings => settings.NotifyFailedLoginAttempt);
        }

        //#7630
        var taxSettings = settingService.LoadSetting<TaxSettings>();
		_logger.LogDebug("Up(): taxSettings → {TaxSettings}", taxSettings);

        if (!settingService.SettingExists(taxSettings, settings => settings.HmrcApiUrl))
        {
            taxSettings.HmrcApiUrl = "https://api.service.hmrc.gov.uk";
			_logger.LogDebug("Up(): taxSettings.HmrcApiUrl → {Arg0}", taxSettings.HmrcApiUrl);
            settingService.SaveSetting(taxSettings, settings => taxSettings.HmrcApiUrl);
        }

        if (!settingService.SettingExists(taxSettings, settings => settings.HmrcClientId))
        {
            taxSettings.HmrcClientId = string.Empty;
			_logger.LogDebug("Up(): taxSettings.HmrcClientId → {Arg0}", taxSettings.HmrcClientId);
            settingService.SaveSetting(taxSettings, settings => taxSettings.HmrcClientId);
        }

        if (!settingService.SettingExists(taxSettings, settings => settings.HmrcClientSecret))
        {
            taxSettings.HmrcClientSecret = string.Empty;
			_logger.LogDebug("Up(): taxSettings.HmrcClientSecret → {Arg0}", taxSettings.HmrcClientSecret);
            settingService.SaveSetting(taxSettings, settings => taxSettings.HmrcClientSecret);
        }

        //#1266
        var orderSettings = settingService.LoadSetting<OrderSettings>();
		_logger.LogDebug("Up(): orderSettings → {OrderSettings}", orderSettings);
        if (!settingService.SettingExists(orderSettings, settings => settings.CustomerOrdersPageSize))
        {
            orderSettings.CustomerOrdersPageSize = 10;
			_logger.LogDebug("Up(): orderSettings.CustomerOrdersPageSize → {Arg0}", orderSettings.CustomerOrdersPageSize);
            settingService.SaveSetting(orderSettings, settings => settings.CustomerOrdersPageSize);
        }

        //#7625
        var addressSetting = settingService.LoadSetting<AddressSettings>();
		_logger.LogDebug("Up(): addressSetting → {AddressSetting}", addressSetting);
        if (!settingService.SettingExists(addressSetting, settings => settings.PrePopulateCountryByCustomer))
        {
            addressSetting.PrePopulateCountryByCustomer = true;
			_logger.LogDebug("Up(): addressSetting.PrePopulateCountryByCustomer → {Arg0}", addressSetting.PrePopulateCountryByCustomer);
            settingService.SaveSetting(addressSetting, settings => settings.PrePopulateCountryByCustomer);
        }

        //#7747
        var forumSettings = settingService.LoadSetting<ForumSettings>();
		_logger.LogDebug("Up(): forumSettings → {ForumSettings}", forumSettings);
        if (!settingService.SettingExists(forumSettings, settings => settings.TopicMetaDescriptionLength))
        {
            forumSettings.TopicMetaDescriptionLength = 160;
			_logger.LogDebug("Up(): forumSettings.TopicMetaDescriptionLength → {Arg0}", forumSettings.TopicMetaDescriptionLength);
            settingService.SaveSetting(forumSettings, settings => settings.TopicMetaDescriptionLength);
        }

        //#7388
        var translationSettings = settingService.LoadSetting<TranslationSettings>();
		_logger.LogDebug("Up(): translationSettings → {TranslationSettings}", translationSettings);
        if (!settingService.SettingExists(translationSettings, settings => settings.AllowPreTranslate))
        {
            translationSettings.AllowPreTranslate = false;
			_logger.LogDebug("Up(): translationSettings.AllowPreTranslate → {Arg0}", translationSettings.AllowPreTranslate);
            settingService.SaveSetting(translationSettings, settings => settings.AllowPreTranslate);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.TranslateFromLanguageId))
        {
            var languageRepository = EngineContext.Current.Resolve<IRepository<Language>>();
			_logger.LogDebug("Up(): languageRepository → {LanguageRepository}", languageRepository);

            translationSettings.TranslateFromLanguageId = languageRepository.Table.First().Id;
            settingService.SaveSetting(translationSettings, settings => settings.TranslateFromLanguageId);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.GoogleApiKey))
        {
            translationSettings.GoogleApiKey = string.Empty;
			_logger.LogDebug("Up(): translationSettings.GoogleApiKey → {Arg0}", translationSettings.GoogleApiKey);
            settingService.SaveSetting(translationSettings, settings => settings.GoogleApiKey);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.DeepLAuthKey))
        {
            translationSettings.DeepLAuthKey = string.Empty;
			_logger.LogDebug("Up(): translationSettings.DeepLAuthKey → {Arg0}", translationSettings.DeepLAuthKey);
            settingService.SaveSetting(translationSettings, settings => settings.DeepLAuthKey);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.NotTranslateLanguages))
        {
            translationSettings.NotTranslateLanguages = new List<int>();
            settingService.SaveSetting(translationSettings, settings => settings.NotTranslateLanguages);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.TranslationServiceId))
        {
            translationSettings.TranslationServiceId = 0;
			_logger.LogDebug("Up(): translationSettings.TranslationServiceId → {Arg0}", translationSettings.TranslationServiceId);
            settingService.SaveSetting(translationSettings, settings => settings.TranslationServiceId);
        }

        //#7779
        var robotsTxtSettings = settingService.LoadSetting<RobotsTxtSettings>();
		_logger.LogDebug("Up(): robotsTxtSettings → {RobotsTxtSettings}", robotsTxtSettings);
        var newDisallowPaths = new List<string> { "/*?*returnurl=", "/*?*ReturnUrl=" };
		_logger.LogDebug("Up(): newDisallowPaths → {NewDisallowPaths}", newDisallowPaths);

        foreach (var newDisallowPath in newDisallowPaths.Where(newDisallowPath => !robotsTxtSettings.DisallowPaths.Contains(newDisallowPath)))
            robotsTxtSettings.DisallowPaths.Add(newDisallowPath);

        robotsTxtSettings.DisallowPaths.Sort();
        settingService.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);

        //#1921
        var shoppingCartSettings = settingService.LoadSetting<ShoppingCartSettings>();
		_logger.LogDebug("Up(): shoppingCartSettings → {ShoppingCartSettings}", shoppingCartSettings);
        if (!settingService.SettingExists(shoppingCartSettings, settings => settings.AllowMultipleWishlist))
        {
            shoppingCartSettings.AllowMultipleWishlist = true;
			_logger.LogDebug("Up(): shoppingCartSettings.AllowMultipleWishlist → {Arg0}", shoppingCartSettings.AllowMultipleWishlist);
            settingService.SaveSetting(shoppingCartSettings, settings => settings.AllowMultipleWishlist);
        }
        if (!settingService.SettingExists(shoppingCartSettings, settings => settings.MaximumNumberOfCustomWishlist))
        {
            shoppingCartSettings.MaximumNumberOfCustomWishlist = 10;
			_logger.LogDebug("Up(): shoppingCartSettings.MaximumNumberOfCustomWishlist → {Arg0}", shoppingCartSettings.MaximumNumberOfCustomWishlist);
            settingService.SaveSetting(shoppingCartSettings, settings => settings.MaximumNumberOfCustomWishlist);
        }

        //#7730
        var aiSettings = settingService.LoadSetting<ArtificialIntelligenceSettings>();
		_logger.LogDebug("Up(): aiSettings → {AiSettings}", aiSettings);

        if (!settingService.SettingExists(aiSettings, settings => settings.Enabled))
        {
            aiSettings.Enabled = false;
			_logger.LogDebug("Up(): aiSettings.Enabled → {Arg0}", aiSettings.Enabled);
            settingService.SaveSetting(aiSettings, settings => settings.Enabled);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.ChatGptApiKey))
        {
            aiSettings.ChatGptApiKey = string.Empty;
			_logger.LogDebug("Up(): aiSettings.ChatGptApiKey → {Arg0}", aiSettings.ChatGptApiKey);
            settingService.SaveSetting(aiSettings, settings => settings.ChatGptApiKey);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.DeepSeekApiKey))
        {
            aiSettings.DeepSeekApiKey = string.Empty;
			_logger.LogDebug("Up(): aiSettings.DeepSeekApiKey → {Arg0}", aiSettings.DeepSeekApiKey);
            settingService.SaveSetting(aiSettings, settings => settings.DeepSeekApiKey);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.GeminiApiKey))
        {
            aiSettings.GeminiApiKey = string.Empty;
			_logger.LogDebug("Up(): aiSettings.GeminiApiKey → {Arg0}", aiSettings.GeminiApiKey);
            settingService.SaveSetting(aiSettings, settings => settings.GeminiApiKey);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.ProviderType))
        {
            aiSettings.ProviderType = ArtificialIntelligenceProviderType.Gemini;
			_logger.LogDebug("Up(): aiSettings.ProviderType → {Arg0}", aiSettings.ProviderType);
            settingService.SaveSetting(aiSettings, settings => settings.ProviderType);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.RequestTimeout))
        {
            aiSettings.RequestTimeout = ArtificialIntelligenceDefaults.RequestTimeout;
			_logger.LogDebug("Up(): aiSettings.RequestTimeout → {Arg0}", aiSettings.RequestTimeout);
            settingService.SaveSetting(aiSettings, settings => settings.RequestTimeout);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.ProductDescriptionQuery))
        {
            aiSettings.ProductDescriptionQuery = ArtificialIntelligenceDefaults.ProductDescriptionQuery;
			_logger.LogDebug("Up(): aiSettings.ProductDescriptionQuery → {Arg0}", aiSettings.ProductDescriptionQuery);
            settingService.SaveSetting(aiSettings, settings => settings.ProductDescriptionQuery);
        }

        //#5986
        if (!settingService.SettingExists(mediaSettings, settings => settings.PicturePath))
        {
            mediaSettings.PicturePath = NopMediaDefaults.DefaultImagesPath;
			_logger.LogDebug("Up(): mediaSettings.PicturePath → {Arg0}", mediaSettings.PicturePath);
            settingService.SaveSetting(mediaSettings, settings => settings.PicturePath);
        }

        //#7390
        var menuSettings = settingService.LoadSetting<MenuSettings>();
		_logger.LogDebug("Up(): menuSettings → {MenuSettings}", menuSettings);
        if (!settingService.SettingExists(menuSettings, settings => settings.MaximumNumberEntities))
        {
            menuSettings.MaximumNumberEntities = 8;
			_logger.LogDebug("Up(): menuSettings.MaximumNumberEntities → {Arg0}", menuSettings.MaximumNumberEntities);
            settingService.SaveSetting(menuSettings, settings => settings.MaximumNumberEntities);
        }

        if (!settingService.SettingExists(menuSettings, settings => settings.NumberOfItemsPerGridRow))
        {
            menuSettings.NumberOfItemsPerGridRow = 4;
			_logger.LogDebug("Up(): menuSettings.NumberOfItemsPerGridRow → {Arg0}", menuSettings.NumberOfItemsPerGridRow);
            settingService.SaveSetting(menuSettings, settings => settings.NumberOfItemsPerGridRow);
        }

        if (!settingService.SettingExists(menuSettings, settings => settings.NumberOfSubItemsPerGridElement))
        {
            menuSettings.NumberOfSubItemsPerGridElement = 3;
			_logger.LogDebug("Up(): menuSettings.NumberOfSubItemsPerGridElement → {Arg0}", menuSettings.NumberOfSubItemsPerGridElement);
            settingService.SaveSetting(menuSettings, settings => settings.NumberOfSubItemsPerGridElement);
        }

        if (!settingService.SettingExists(menuSettings, settings => settings.MaximumMainMenuLevels))
        {
            menuSettings.MaximumMainMenuLevels = 2;
			_logger.LogDebug("Up(): menuSettings.MaximumMainMenuLevels → {Arg0}", menuSettings.MaximumMainMenuLevels);
            settingService.SaveSetting(menuSettings, settings => settings.MaximumMainMenuLevels);
        }

        if (!settingService.SettingExists(menuSettings, settings => settings.GridThumbPictureSize))
        {
            menuSettings.GridThumbPictureSize = 340;
			_logger.LogDebug("Up(): menuSettings.GridThumbPictureSize → {Arg0}", menuSettings.GridThumbPictureSize);
            settingService.SaveSetting(menuSettings, settings => settings.GridThumbPictureSize);
        }

        var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        settingRepository.Delete(setting => setting.Name.StartsWith("displaydefaultmenuitemsettings"));

		_logger.LogDebug("Up(): settingRepository → {SettingRepository}", settingRepository);
		_logger.LogInformation("Up(): delete query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        var useajaxloadmenu = settingService.GetSetting("catalogsettings.useajaxloadmenu");
		_logger.LogDebug("Up(): useajaxloadmenu → {Useajaxloadmenu}", useajaxloadmenu);
        if (useajaxloadmenu is not null)
            settingService.DeleteSetting(useajaxloadmenu);

        //#7732
        if (!settingService.SettingExists(aiSettings, settings => settings.AllowProductDescriptionGeneration))
        {
            aiSettings.AllowProductDescriptionGeneration = true;
			_logger.LogDebug("Up(): aiSettings.AllowProductDescriptionGeneration → {Arg0}", aiSettings.AllowProductDescriptionGeneration);
            settingService.SaveSetting(aiSettings, settings => settings.AllowProductDescriptionGeneration);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.AllowMetaTitleGeneration))
        {
            aiSettings.AllowMetaTitleGeneration = true;
			_logger.LogDebug("Up(): aiSettings.AllowMetaTitleGeneration → {Arg0}", aiSettings.AllowMetaTitleGeneration);
            settingService.SaveSetting(aiSettings, settings => settings.AllowMetaTitleGeneration);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.MetaTitleQuery))
        {
            aiSettings.MetaTitleQuery = ArtificialIntelligenceDefaults.MetaTitleQuery;
			_logger.LogDebug("Up(): aiSettings.MetaTitleQuery → {Arg0}", aiSettings.MetaTitleQuery);
            settingService.SaveSetting(aiSettings, settings => settings.MetaTitleQuery);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.AllowMetaKeywordsGeneration))
        {
            aiSettings.AllowMetaKeywordsGeneration = true;
			_logger.LogDebug("Up(): aiSettings.AllowMetaKeywordsGeneration → {Arg0}", aiSettings.AllowMetaKeywordsGeneration);
            settingService.SaveSetting(aiSettings, settings => settings.AllowMetaKeywordsGeneration);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.MetaKeywordsQuery))
        {
            aiSettings.MetaKeywordsQuery = ArtificialIntelligenceDefaults.MetaKeywordsQuery;
			_logger.LogDebug("Up(): aiSettings.MetaKeywordsQuery → {Arg0}", aiSettings.MetaKeywordsQuery);
            settingService.SaveSetting(aiSettings, settings => settings.MetaKeywordsQuery);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.AllowMetaDescriptionGeneration))
        {
            aiSettings.AllowMetaDescriptionGeneration = true;
			_logger.LogDebug("Up(): aiSettings.AllowMetaDescriptionGeneration → {Arg0}", aiSettings.AllowMetaDescriptionGeneration);
            settingService.SaveSetting(aiSettings, settings => settings.AllowMetaDescriptionGeneration);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.MetaDescriptionQuery))
        {
            aiSettings.MetaDescriptionQuery = ArtificialIntelligenceDefaults.MetaDescriptionQuery;
			_logger.LogDebug("Up(): aiSettings.MetaDescriptionQuery → {Arg0}", aiSettings.MetaDescriptionQuery);
            settingService.SaveSetting(aiSettings, settings => settings.MetaDescriptionQuery);
        }

        //#7411
        var filterLevelSettings = settingService.LoadSetting<FilterLevelSettings>();
		_logger.LogDebug("Up(): filterLevelSettings → {FilterLevelSettings}", filterLevelSettings);
        if (!settingService.SettingExists(filterLevelSettings, settings => settings.DisplayOnHomePage))
        {
            filterLevelSettings.DisplayOnHomePage = true;
			_logger.LogDebug("Up(): filterLevelSettings.DisplayOnHomePage → {Arg0}", filterLevelSettings.DisplayOnHomePage);
            settingService.SaveSetting(filterLevelSettings, settings => settings.DisplayOnHomePage);
        }
        if (!settingService.SettingExists(filterLevelSettings, settings => settings.DisplayOnProductDetailsPage))
        {
            filterLevelSettings.DisplayOnProductDetailsPage = true;
			_logger.LogDebug("Up(): filterLevelSettings.DisplayOnProductDetailsPage → {Arg0}", filterLevelSettings.DisplayOnProductDetailsPage);
            settingService.SaveSetting(filterLevelSettings, settings => settings.DisplayOnProductDetailsPage);
        }
        if (!settingService.SettingExists(productEditorSettings, settings => settings.FilterLevelValuesProducts))
        {
            productEditorSettings.FilterLevelValuesProducts = true;
			_logger.LogDebug("Up(): productEditorSettings.FilterLevelValuesProducts → {Arg0}", productEditorSettings.FilterLevelValuesProducts);
            settingService.SaveSetting(productEditorSettings, settings => settings.FilterLevelValuesProducts);
        }

        //#7384
        if (!settingService.SettingExists(orderSettings, settings => settings.AllowCustomersCancelOrders))
        {
            orderSettings.AllowCustomersCancelOrders = true;
			_logger.LogDebug("Up(): orderSettings.AllowCustomersCancelOrders → {Arg0}", orderSettings.AllowCustomersCancelOrders);
            settingService.SaveSetting(orderSettings, settings => settings.AllowCustomersCancelOrders);
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
