using FluentMigrator;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Stores;

namespace Nop.Web.Framework.Migrations.UpgradeTo460;

[NopUpdateMigration("2023-07-26 14:00:00", "4.60", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();

		logger.LogDebug("Up(): settingService → {SettingService}", settingService);
        var catalogSettings = settingService.LoadSetting<CatalogSettings>();

		logger.LogDebug("Up(): catalogSettings → {CatalogSettings}", catalogSettings);
        //#3075
        if (!settingService.SettingExists(catalogSettings, settings => settings.AllowCustomersToSearchWithManufacturerName))
        {
            catalogSettings.AllowCustomersToSearchWithManufacturerName = true;
			logger.LogDebug("Up(): catalogSettings.AllowCustomersToSearchWithManufacturerName → {Arg0}", catalogSettings.AllowCustomersToSearchWithManufacturerName);
            settingService.SaveSetting(catalogSettings, settings => settings.AllowCustomersToSearchWithManufacturerName);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.AllowCustomersToSearchWithCategoryName))
        {
            catalogSettings.AllowCustomersToSearchWithCategoryName = true;
			logger.LogDebug("Up(): catalogSettings.AllowCustomersToSearchWithCategoryName → {Arg0}", catalogSettings.AllowCustomersToSearchWithCategoryName);
            settingService.SaveSetting(catalogSettings, settings => settings.AllowCustomersToSearchWithCategoryName);
        }

        //#1933
        if (!settingService.SettingExists(catalogSettings, settings => settings.DisplayAllPicturesOnCatalogPages))
        {
            catalogSettings.DisplayAllPicturesOnCatalogPages = false;
			logger.LogDebug("Up(): catalogSettings.DisplayAllPicturesOnCatalogPages → {Arg0}", catalogSettings.DisplayAllPicturesOnCatalogPages);
            settingService.SaveSetting(catalogSettings, settings => settings.DisplayAllPicturesOnCatalogPages);
        }

        //#3511
        var newProductsNumber = settingService.GetSetting("catalogsettings.newproductsnumber");
        if (newProductsNumber is not null && int.TryParse(newProductsNumber.Value, out var newProductsPageSize))
		logger.LogDebug("Up(): newProductsNumber → {NewProductsNumber}", newProductsNumber);
        {
            catalogSettings.NewProductsPageSize = newProductsPageSize;
			logger.LogDebug("Up(): catalogSettings.NewProductsPageSize → {Arg0}", catalogSettings.NewProductsPageSize);
            settingService.SaveSetting(catalogSettings, settings => settings.NewProductsPageSize);
            settingService.DeleteSetting(newProductsNumber);
        }
        else if (!settingService.SettingExists(catalogSettings, settings => settings.NewProductsPageSize))
        {
            catalogSettings.NewProductsPageSize = 6;
            settingService.SaveSetting(catalogSettings, settings => settings.NewProductsPageSize);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.NewProductsAllowCustomersToSelectPageSize))
        {
            catalogSettings.NewProductsAllowCustomersToSelectPageSize = false;
			logger.LogDebug("Up(): catalogSettings.NewProductsAllowCustomersToSelectPageSize → {Arg0}", catalogSettings.NewProductsAllowCustomersToSelectPageSize);
            settingService.SaveSetting(catalogSettings, settings => settings.NewProductsAllowCustomersToSelectPageSize);
        }

        if (!settingService.SettingExists(catalogSettings, settings => settings.NewProductsPageSizeOptions))
        {
            catalogSettings.NewProductsPageSizeOptions = "6, 3, 9";
			logger.LogDebug("Up(): catalogSettings.NewProductsPageSizeOptions → {Arg0}", catalogSettings.NewProductsPageSizeOptions);
            settingService.SaveSetting(catalogSettings, settings => settings.NewProductsPageSizeOptions);
        }

        //#29
        if (!settingService.SettingExists(catalogSettings, settings => settings.DisplayFromPrices))
        {
            catalogSettings.DisplayFromPrices = false;
			logger.LogDebug("Up(): catalogSettings.DisplayFromPrices → {Arg0}", catalogSettings.DisplayFromPrices);
            settingService.SaveSetting(catalogSettings, settings => settings.DisplayFromPrices);
        }

        //#6115
        if (!settingService.SettingExists(catalogSettings, settings => settings.ShowShortDescriptionOnCatalogPages))
        {
            catalogSettings.ShowShortDescriptionOnCatalogPages = false;
			logger.LogDebug("Up(): catalogSettings.ShowShortDescriptionOnCatalogPages → {Arg0}", catalogSettings.ShowShortDescriptionOnCatalogPages);
            settingService.SaveSetting(catalogSettings, settings => settings.ShowShortDescriptionOnCatalogPages);
        }

        var storeInformationSettings = settingService.LoadSetting<StoreInformationSettings>();

		logger.LogDebug("Up(): storeInformationSettings → {StoreInformationSettings}", storeInformationSettings);
        //#3997
        if (!settingService.SettingExists(storeInformationSettings, settings => settings.InstagramLink))
        {
            storeInformationSettings.InstagramLink = "";
			logger.LogDebug("Up(): storeInformationSettings.InstagramLink → {Arg0}", storeInformationSettings.InstagramLink);
            settingService.SaveSetting(storeInformationSettings, settings => settings.InstagramLink);
        }

        var commonSettings = settingService.LoadSetting<CommonSettings>();

		logger.LogDebug("Up(): commonSettings → {CommonSettings}", commonSettings);
        //#5802
        if (!settingService.SettingExists(commonSettings, settings => settings.HeaderCustomHtml))
        {
            commonSettings.HeaderCustomHtml = "";
			logger.LogDebug("Up(): commonSettings.HeaderCustomHtml → {Arg0}", commonSettings.HeaderCustomHtml);
            settingService.SaveSetting(commonSettings, settings => settings.HeaderCustomHtml);
        }

        if (!settingService.SettingExists(commonSettings, settings => settings.FooterCustomHtml))
        {
            commonSettings.FooterCustomHtml = "";
			logger.LogDebug("Up(): commonSettings.FooterCustomHtml → {Arg0}", commonSettings.FooterCustomHtml);
            settingService.SaveSetting(commonSettings, settings => settings.FooterCustomHtml);
        }

        var orderSettings = settingService.LoadSetting<OrderSettings>();

		logger.LogDebug("Up(): orderSettings → {OrderSettings}", orderSettings);
        //#5604
        if (!settingService.SettingExists(orderSettings, settings => settings.ShowProductThumbnailInOrderDetailsPage))
        {
            orderSettings.ShowProductThumbnailInOrderDetailsPage = true;
			logger.LogDebug("Up(): orderSettings.ShowProductThumbnailInOrderDetailsPage → {Arg0}", orderSettings.ShowProductThumbnailInOrderDetailsPage);
            settingService.SaveSetting(orderSettings, settings => settings.ShowProductThumbnailInOrderDetailsPage);
        }

        var mediaSettings = settingService.LoadSetting<MediaSettings>();

		logger.LogDebug("Up(): mediaSettings → {MediaSettings}", mediaSettings);
        //#5604
        if (!settingService.SettingExists(mediaSettings, settings => settings.OrderThumbPictureSize))
        {
            mediaSettings.OrderThumbPictureSize = 80;
			logger.LogDebug("Up(): mediaSettings.OrderThumbPictureSize → {Arg0}", mediaSettings.OrderThumbPictureSize);
            settingService.SaveSetting(mediaSettings, settings => settings.OrderThumbPictureSize);
        }

        var adminSettings = settingService.LoadSetting<AdminAreaSettings>();
        if (!settingService.SettingExists(adminSettings, settings => settings.CheckLicense))
		logger.LogDebug("Up(): adminSettings → {AdminSettings}", adminSettings);
        {
            adminSettings.CheckLicense = true;
			logger.LogDebug("Up(): adminSettings.CheckLicense → {Arg0}", adminSettings.CheckLicense);
            settingService.SaveSetting(adminSettings, settings => settings.CheckLicense);
        }

        var gdprSettings = settingService.LoadSetting<GdprSettings>();

		logger.LogDebug("Up(): gdprSettings → {GdprSettings}", gdprSettings);
        //#5809
        if (!settingService.SettingExists(gdprSettings, settings => settings.DeleteInactiveCustomersAfterMonths))
        {
            gdprSettings.DeleteInactiveCustomersAfterMonths = 36;
			logger.LogDebug("Up(): gdprSettings.DeleteInactiveCustomersAfterMonths → {Arg0}", gdprSettings.DeleteInactiveCustomersAfterMonths);
            settingService.SaveSetting(gdprSettings, settings => settings.DeleteInactiveCustomersAfterMonths);
        }

        var captchaSettings = settingService.LoadSetting<CaptchaSettings>();

		logger.LogDebug("Up(): captchaSettings → {CaptchaSettings}", captchaSettings);
        //#6182
        if (!settingService.SettingExists(captchaSettings, settings => settings.ShowOnCheckoutPageForGuests))
        {
            captchaSettings.ShowOnCheckoutPageForGuests = false;
			logger.LogDebug("Up(): captchaSettings.ShowOnCheckoutPageForGuests → {Arg0}", captchaSettings.ShowOnCheckoutPageForGuests);
            settingService.SaveSetting(captchaSettings, settings => settings.ShowOnCheckoutPageForGuests);
        }

        //#7
        if (!settingService.SettingExists(mediaSettings, settings => settings.VideoIframeAllow))
        {
            mediaSettings.VideoIframeAllow = "fullscreen";
			logger.LogDebug("Up(): mediaSettings.VideoIframeAllow → {Arg0}", mediaSettings.VideoIframeAllow);
            settingService.SaveSetting(mediaSettings, settings => settings.VideoIframeAllow);
        }

        //#7
        if (!settingService.SettingExists(mediaSettings, settings => settings.VideoIframeWidth))
        {
            mediaSettings.VideoIframeWidth = 300;
			logger.LogDebug("Up(): mediaSettings.VideoIframeWidth → {Arg0}", mediaSettings.VideoIframeWidth);
            settingService.SaveSetting(mediaSettings, settings => settings.VideoIframeWidth);
        }

        //#7
        if (!settingService.SettingExists(mediaSettings, settings => settings.VideoIframeHeight))
        {
            mediaSettings.VideoIframeHeight = 150;
			logger.LogDebug("Up(): mediaSettings.VideoIframeHeight → {Arg0}", mediaSettings.VideoIframeHeight);
            settingService.SaveSetting(mediaSettings, settings => settings.VideoIframeHeight);
        }

        //#385
        if (!settingService.SettingExists(catalogSettings, settings => settings.ProductUrlStructureTypeId))
        {
            catalogSettings.ProductUrlStructureTypeId = (int)ProductUrlStructureType.Product;
            settingService.SaveSetting(catalogSettings, settings => settings.ProductUrlStructureTypeId);
        }

        //#5261
        var robotsTxtSettings = settingService.LoadSetting<RobotsTxtSettings>();

		logger.LogDebug("Up(): robotsTxtSettings → {RobotsTxtSettings}", robotsTxtSettings);
        if (!settingService.SettingExists(robotsTxtSettings, settings => settings.DisallowPaths))
        {
            robotsTxtSettings.DisallowPaths.AddRange(new[]
            {
                "/admin",
                "/bin/",
                "/files/",
                "/files/exportimport/",
                "/country/getstatesbycountryid",
                "/install",
                "/setproductreviewhelpfulness",
                "/*?*returnUrl="
            });

            settingService.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);
        }

        if (!settingService.SettingExists(robotsTxtSettings, settings => settings.LocalizableDisallowPaths))
        {
            robotsTxtSettings.LocalizableDisallowPaths.AddRange(new[]
            {
                "/addproducttocart/catalog/",
                "/addproducttocart/details/",
                "/backinstocksubscriptions/manage",
                "/boards/forumsubscriptions",
                "/boards/forumwatch",
                "/boards/postedit",
                "/boards/postdelete",
                "/boards/postcreate",
                "/boards/topicedit",
                "/boards/topicdelete",
                "/boards/topiccreate",
                "/boards/topicmove",
                "/boards/topicwatch",
                "/cart$",
                "/changecurrency",
                "/changelanguage",
                "/changetaxtype",
                "/checkout",
                "/checkout/billingaddress",
                "/checkout/completed",
                "/checkout/confirm",
                "/checkout/shippingaddress",
                "/checkout/shippingmethod",
                "/checkout/paymentinfo",
                "/checkout/paymentmethod",
                "/clearcomparelist",
                "/compareproducts",
                "/compareproducts/add/*",
                "/customer/avatar",
                "/customer/activation",
                "/customer/addresses",
                "/customer/changepassword",
                "/customer/checkusernameavailability",
                "/customer/downloadableproducts",
                "/customer/info",
                "/customer/productreviews",
                "/deletepm",
                "/emailwishlist",
                "/eucookielawaccept",
                "/inboxupdate",
                "/newsletter/subscriptionactivation",
                "/onepagecheckout",
                "/order/history",
                "/orderdetails",
                "/passwordrecovery/confirm",
                "/poll/vote",
                "/privatemessages",
                "/recentlyviewedproducts",
                "/returnrequest",
                "/returnrequest/history",
                "/rewardpoints/history",
                "/search?",
                "/sendpm",
                "/sentupdate",
                "/shoppingcart/*",
                "/storeclosed",
                "/subscribenewsletter",
                "/topic/authenticate",
                "/viewpm",
                "/uploadfilecheckoutattribute",
                "/uploadfileproductattribute",
                "/uploadfilereturnrequest",
                "/wishlist"
            });

            settingService.SaveSetting(robotsTxtSettings, settings => settings.LocalizableDisallowPaths);
        }

        if (!settingService.SettingExists(robotsTxtSettings, settings => settings.DisallowLanguages))
            settingService.SaveSetting(robotsTxtSettings, settings => settings.DisallowLanguages);

        if (!settingService.SettingExists(robotsTxtSettings, settings => settings.AdditionsRules))
            settingService.SaveSetting(robotsTxtSettings, settings => settings.AdditionsRules);

        if (!settingService.SettingExists(robotsTxtSettings, settings => settings.AllowSitemapXml))
            settingService.SaveSetting(robotsTxtSettings, settings => settings.AllowSitemapXml);

        //#5753
        if (!settingService.SettingExists(mediaSettings, settings => settings.ProductDefaultImageId))
        {
            mediaSettings.ProductDefaultImageId = 0;
			logger.LogDebug("Up(): mediaSettings.ProductDefaultImageId → {Arg0}", mediaSettings.ProductDefaultImageId);
            settingService.SaveSetting(mediaSettings, settings => settings.ProductDefaultImageId);
        }

        //#3651
        if (!settingService.SettingExists(orderSettings, settings => settings.AttachPdfInvoiceToOrderProcessingEmail))
        {
            orderSettings.AttachPdfInvoiceToOrderProcessingEmail = false;
			logger.LogDebug("Up(): orderSettings.AttachPdfInvoiceToOrderProcessingEmail → {Arg0}", orderSettings.AttachPdfInvoiceToOrderProcessingEmail);
            settingService.SaveSetting(orderSettings, settings => settings.AttachPdfInvoiceToOrderProcessingEmail);
        }

        var taxSettings = settingService.LoadSetting<TaxSettings>();

		logger.LogDebug("Up(): taxSettings → {TaxSettings}", taxSettings);
        //#1961
        if (!settingService.SettingExists(taxSettings, settings => settings.EuVatEnabledForGuests))
        {
            taxSettings.EuVatEnabledForGuests = false;
			logger.LogDebug("Up(): taxSettings.EuVatEnabledForGuests → {Arg0}", taxSettings.EuVatEnabledForGuests);
            settingService.SaveSetting(taxSettings, settings => settings.EuVatEnabledForGuests);
        }

        //#5570
        var sitemapXmlSettings = settingService.LoadSetting<SitemapXmlSettings>();

		logger.LogDebug("Up(): sitemapXmlSettings → {SitemapXmlSettings}", sitemapXmlSettings);
        if (!settingService.SettingExists(sitemapXmlSettings, settings => settings.RebuildSitemapXmlAfterHours))
        {
            sitemapXmlSettings.RebuildSitemapXmlAfterHours = 2 * 24;
			logger.LogDebug("Up(): sitemapXmlSettings.RebuildSitemapXmlAfterHours → {Arg0}", sitemapXmlSettings.RebuildSitemapXmlAfterHours);
            settingService.SaveSetting(sitemapXmlSettings, settings => settings.RebuildSitemapXmlAfterHours);
        }

        if (!settingService.SettingExists(sitemapXmlSettings, settings => settings.SitemapBuildOperationDelay))
        {
            sitemapXmlSettings.SitemapBuildOperationDelay = 60;
			logger.LogDebug("Up(): sitemapXmlSettings.SitemapBuildOperationDelay → {Arg0}", sitemapXmlSettings.SitemapBuildOperationDelay);
            settingService.SaveSetting(sitemapXmlSettings, settings => settings.SitemapBuildOperationDelay);
        }

        //#6378
        if (!settingService.SettingExists(mediaSettings, settings => settings.AllowSvgUploads))
        {
            mediaSettings.AllowSvgUploads = false;
			logger.LogDebug("Up(): mediaSettings.AllowSvgUploads → {Arg0}", mediaSettings.AllowSvgUploads);
            settingService.SaveSetting(mediaSettings, settings => settings.AllowSvgUploads);
        }

        //#5599
        var messagesSettings = settingService.LoadSetting<MessagesSettings>();

		logger.LogDebug("Up(): messagesSettings → {MessagesSettings}", messagesSettings);
        if (!settingService.SettingExists(messagesSettings, settings => settings.UseDefaultEmailAccountForSendStoreOwnerEmails))
        {
            messagesSettings.UseDefaultEmailAccountForSendStoreOwnerEmails = false;
			logger.LogDebug("Up(): messagesSettings.UseDefaultEmailAccountForSendStoreOwnerEmails → {Arg0}", messagesSettings.UseDefaultEmailAccountForSendStoreOwnerEmails);
            settingService.SaveSetting(messagesSettings, settings => settings.UseDefaultEmailAccountForSendStoreOwnerEmails);
        }

        //#228
        if (!settingService.SettingExists(catalogSettings, settings => settings.ActiveSearchProviderSystemName))
        {
            catalogSettings.ActiveSearchProviderSystemName = string.Empty;
			logger.LogDebug("Up(): catalogSettings.ActiveSearchProviderSystemName → {Arg0}", catalogSettings.ActiveSearchProviderSystemName);
            settingService.SaveSetting(catalogSettings, settings => settings.ActiveSearchProviderSystemName);
        }

        //#43
        var metaTitleKey = $"{nameof(SeoSettings)}.DefaultTitle".ToLower();
        var metaKeywordsKey = $"{nameof(SeoSettings)}.DefaultMetaKeywords".ToLower();
		logger.LogDebug("Up(): metaTitleKey → {MetaTitleKey}", metaTitleKey);
		logger.LogDebug("Up(): metaKeywordsKey → {MetaKeywordsKey}", metaKeywordsKey);
        var metaDescriptionKey = $"{nameof(SeoSettings)}.DefaultMetaDescription".ToLower();
        var homepageTitleKey = $"{nameof(SeoSettings)}.HomepageTitle".ToLower();
		logger.LogDebug("Up(): metaDescriptionKey → {MetaDescriptionKey}", metaDescriptionKey);
		logger.LogDebug("Up(): homepageTitleKey → {HomepageTitleKey}", homepageTitleKey);
        var homepageDescriptionKey = $"{nameof(SeoSettings)}.HomepageDescription".ToLower();

		logger.LogDebug("Up(): homepageDescriptionKey → {HomepageDescriptionKey}", homepageDescriptionKey);
        var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
        var storeService = EngineContext.Current.Resolve<IStoreService>();
		logger.LogDebug("Up(): settingRepository → {SettingRepository}", settingRepository);
		logger.LogDebug("Up(): storeService → {StoreService}", storeService);

        foreach (var store in storeService.GetAllStores())
        {
            var metaTitle = settingService.GetSettingByKey<string>(metaTitleKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(metaTitleKey);
            var metaKeywords = settingService.GetSettingByKey<string>(metaKeywordsKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(metaKeywordsKey);
			logger.LogDebug("Up(): metaTitle → {MetaTitle}", metaTitle);
			logger.LogDebug("Up(): metaKeywords → {MetaKeywords}", metaKeywords);
            var metaDescription = settingService.GetSettingByKey<string>(metaDescriptionKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(metaDescriptionKey);
            var homepageTitle = settingService.GetSettingByKey<string>(homepageTitleKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(homepageTitleKey);
			logger.LogDebug("Up(): metaDescription → {MetaDescription}", metaDescription);
			logger.LogDebug("Up(): homepageTitle → {HomepageTitle}", homepageTitle);
            var homepageDescription = settingService.GetSettingByKey<string>(homepageDescriptionKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(homepageDescriptionKey);

			logger.LogDebug("Up(): homepageDescription → {HomepageDescription}", homepageDescription);
            if (metaTitle != null)
                store.DefaultTitle = metaTitle;
				logger.LogDebug("Up(): store.DefaultTitle → {Arg0}", store.DefaultTitle);

            if (metaKeywords != null)
                store.DefaultMetaKeywords = metaKeywords;
				logger.LogDebug("Up(): store.DefaultMetaKeywords → {Arg0}", store.DefaultMetaKeywords);

            if (metaDescription != null)
                store.DefaultMetaDescription = metaDescription;
				logger.LogDebug("Up(): store.DefaultMetaDescription → {Arg0}", store.DefaultMetaDescription);

            if (homepageTitle != null)
                store.HomepageTitle = homepageTitle;
				logger.LogDebug("Up(): store.HomepageTitle → {Arg0}", store.HomepageTitle);

            if (homepageDescription != null)
                store.HomepageDescription = homepageDescription;
				logger.LogDebug("Up(): store.HomepageDescription → {Arg0}", store.HomepageDescription);

            storeService.UpdateStore(store);
        }

        settingRepository.Delete(setting => setting.Name == metaTitleKey);
		logger.LogInformation("Up(): delete query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        settingRepository.Delete(setting => setting.Name == metaKeywordsKey);
        settingRepository.Delete(setting => setting.Name == metaDescriptionKey);
        settingRepository.Delete(setting => setting.Name == homepageTitleKey);
        settingRepository.Delete(setting => setting.Name == homepageDescriptionKey);
		logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down()
    {
		logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		logger.LogDebug("<<< Exiting Down()");
    }
}
