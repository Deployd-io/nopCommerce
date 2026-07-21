using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Common;
using Nop.Web.Models.Sitemap;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CommonController : BasePublicController
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ISitemapModelFactory _sitemapModelFactory;
    protected readonly IStoreContext _storeContext;
    protected readonly IThemeContext _themeContext;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly SitemapSettings _sitemapSettings;
    protected readonly SitemapXmlSettings _sitemapXmlSettings;
    protected readonly StoreInformationSettings _storeInformationSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public CommonController(CaptchaSettings captchaSettings,
        CommonSettings commonSettings,
        ICommonModelFactory commonModelFactory,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        IGenericAttributeService genericAttributeService,
        IHtmlFormatter htmlFormatter,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ISitemapModelFactory sitemapModelFactory,
        IStoreContext storeContext,
        IThemeContext themeContext,
        IVendorService vendorService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        SitemapSettings sitemapSettings,
        SitemapXmlSettings sitemapXmlSettings,
        StoreInformationSettings storeInformationSettings,
        VendorSettings vendorSettings)
    {
        _captchaSettings = captchaSettings;
        _commonSettings = commonSettings;
        _commonModelFactory = commonModelFactory;
        _currencyService = currencyService;
        _customerActivityService = customerActivityService;
        _genericAttributeService = genericAttributeService;
        _htmlFormatter = htmlFormatter;
        _languageService = languageService;
        _localizationService = localizationService;
        _sitemapModelFactory = sitemapModelFactory;
        _storeContext = storeContext;
        _themeContext = themeContext;
        _vendorService = vendorService;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
        _sitemapSettings = sitemapSettings;
        _sitemapXmlSettings = sitemapXmlSettings;
        _storeInformationSettings = storeInformationSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Methods

    public virtual IActionResult PageNotFound()
    {
		logger.LogDebug(">>> Entering PageNotFound()");
		logger.LogDebug("<<< Exiting PageNotFound()");
        return View();
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetLanguage(int langid, string returnUrl = "")
    {
		logger.LogDebug(">>> Entering SetLanguage(langid={Langid},returnUrl={ReturnUrl})", langid, returnUrl);
        var language = await _languageService.GetLanguageByIdAsync(langid);
        if (!language?.Published ?? false)
			logger.LogDebug("SetLanguage(langid={Langid},returnUrl={ReturnUrl}): language → {Language}", langid, returnUrl, language);
			logger.LogDebug("SetLanguage(langid={Langid},returnUrl={ReturnUrl}): returnUrl → {ReturnUrl1}", langid, returnUrl, returnUrl);
            language = await _workContext.GetWorkingLanguageAsync();

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

        //language part in URL
        if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
        {
            //remove current language code if it's already localized URL
            if ((await returnUrl.IsLocalizedUrlAsync(Request.PathBase, true)).IsLocalized)
                returnUrl = returnUrl.RemoveLanguageSeoCodeFromUrl(Request.PathBase, true);

            //and add code of passed language
            returnUrl = returnUrl.AddLanguageSeoCodeToUrl(Request.PathBase, true, language);
        }

        await _workContext.SetWorkingLanguageAsync(language);

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

		logger.LogDebug("<<< Exiting SetLanguage(langid={Langid},returnUrl={ReturnUrl})", langid, returnUrl);
        return Redirect(returnUrl);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetCurrency(int customerCurrency, string returnUrl = "")
    {
		logger.LogDebug(">>> Entering SetCurrency(customerCurrency={CustomerCurrency},returnUrl={ReturnUrl})", customerCurrency, returnUrl);
        var currency = await _currencyService.GetCurrencyByIdAsync(customerCurrency);
        if (currency != null)
			logger.LogDebug("SetCurrency(customerCurrency={CustomerCurrency},returnUrl={ReturnUrl}): currency → {Currency}", customerCurrency, returnUrl, currency);
			logger.LogDebug("SetCurrency(customerCurrency={CustomerCurrency},returnUrl={ReturnUrl}): returnUrl → {ReturnUrl1}", customerCurrency, returnUrl, returnUrl);
            await _workContext.SetWorkingCurrencyAsync(currency);

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

		logger.LogDebug("<<< Exiting SetCurrency(customerCurrency={CustomerCurrency},returnUrl={ReturnUrl})", customerCurrency, returnUrl);
        return Redirect(returnUrl);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetTaxType(int customerTaxType, string returnUrl = "")
    {
		logger.LogDebug(">>> Entering SetTaxType(customerTaxType={CustomerTaxType},returnUrl={ReturnUrl})", customerTaxType, returnUrl);
        var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
        await _workContext.SetTaxDisplayTypeAsync(taxDisplayType);
		logger.LogDebug("SetTaxType(customerTaxType={CustomerTaxType},returnUrl={ReturnUrl}): taxDisplayType → {TaxDisplayType}", customerTaxType, returnUrl, taxDisplayType);
		logger.LogDebug("SetTaxType(customerTaxType={CustomerTaxType},returnUrl={ReturnUrl}): returnUrl → {ReturnUrl1}", customerTaxType, returnUrl, returnUrl);

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

		logger.LogDebug("<<< Exiting SetTaxType(customerTaxType={CustomerTaxType},returnUrl={ReturnUrl})", customerTaxType, returnUrl);
        return Redirect(returnUrl);
    }

    //contact us page
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> ContactUs()
    {
		logger.LogDebug(">>> Entering ContactUs()");
        var model = new ContactUsModel();
        model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);
		logger.LogDebug("ContactUs(): model → {Model}", model);

		logger.LogDebug("<<< Exiting ContactUs()");
        return View(model);
    }

    [HttpPost, ActionName("ContactUs")]
    [ValidateCaptcha]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> ContactUsSend(ContactUsModel model, bool captchaValid)
    {
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);

		logger.LogDebug("ContactUsSend(model={Model},captchaValid={CaptchaValid}): model → {Model1}", model, captchaValid, model);
        if (ModelState.IsValid)
        {
            var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
            var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);

			logger.LogDebug("ContactUsSend(model={Model},captchaValid={CaptchaValid}): body → {Body}", model, captchaValid, body);
            await _workflowMessageService.SendContactUsMessageAsync((await _workContext.GetWorkingLanguageAsync()).Id,
                model.Email, model.FullName, subject, body);

            model.SuccessfullySent = true;
			logger.LogDebug("ContactUsSend(model={Model},captchaValid={CaptchaValid}): model.SuccessfullySent → {Arg2}", model, captchaValid, model.SuccessfullySent);
            model.Result = await _localizationService.GetResourceAsync("ContactUs.YourEnquiryHasBeenSent");

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ContactUs",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ContactUs"));

            return View(model);
        }

		logger.LogDebug("<<< Exiting ContactUsSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    //contact vendor page
    public virtual async Task<IActionResult> ContactVendor(int vendorId)
    {
		logger.LogDebug(">>> Entering ContactVendor(vendorId={VendorId})", vendorId);
        if (!_vendorSettings.AllowCustomersToContactVendors)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
        if (vendor == null || !vendor.Active || vendor.Deleted)
			logger.LogDebug("ContactVendor(vendorId={VendorId}): vendor → {Vendor}", vendorId, vendor);
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new ContactVendorModel();
        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, vendor, false);
		logger.LogDebug("ContactVendor(vendorId={VendorId}): model → {Model}", vendorId, model);

		logger.LogDebug("<<< Exiting ContactVendor(vendorId={VendorId})", vendorId);
        return View(model);
    }

    [HttpPost, ActionName("ContactVendor")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ContactVendorSend(ContactVendorModel model, bool captchaValid)
    {
        if (!_vendorSettings.AllowCustomersToContactVendors)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var vendor = await _vendorService.GetVendorByIdAsync(model.VendorId);
        if (vendor == null || !vendor.Active || vendor.Deleted)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, vendor, true);

		logger.LogDebug("ContactVendorSend(model={Model},captchaValid={CaptchaValid}): model → {Model1}", model, captchaValid, model);
        if (ModelState.IsValid)
        {
            var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
            var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);

			logger.LogDebug("ContactVendorSend(model={Model},captchaValid={CaptchaValid}): body → {Body}", model, captchaValid, body);
            await _workflowMessageService.SendContactVendorMessageAsync(vendor, (await _workContext.GetWorkingLanguageAsync()).Id,
                model.Email, model.FullName, subject, body);

            model.SuccessfullySent = true;
			logger.LogDebug("ContactVendorSend(model={Model},captchaValid={CaptchaValid}): model.SuccessfullySent → {Arg2}", model, captchaValid, model.SuccessfullySent);
            model.Result = await _localizationService.GetResourceAsync("ContactVendor.YourEnquiryHasBeenSent");

            return View(model);
        }

		logger.LogDebug("<<< Exiting ContactVendorSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    //sitemap page
    public virtual async Task<IActionResult> Sitemap(SitemapPageModel pageModel)
    {
		logger.LogDebug(">>> Entering Sitemap(pageModel={PageModel})", pageModel);
        if (!_sitemapSettings.SitemapEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _sitemapModelFactory.PrepareSitemapModelAsync(pageModel);

		logger.LogDebug("Sitemap(pageModel={PageModel}): model → {Model}", pageModel, model);
		logger.LogDebug("<<< Exiting Sitemap(pageModel={PageModel})", pageModel);
        return View(model);
    }

    //SEO sitemap page
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> SitemapXml(int? id)
    {
		logger.LogDebug(">>> Entering SitemapXml(id={Id})", id);
        if (!_sitemapXmlSettings.SitemapXmlEnabled)
            return StatusCode(StatusCodes.Status403Forbidden);

        try
        {
            var sitemapXmlModel = await _sitemapModelFactory.PrepareSitemapXmlModelAsync(id ?? 0);
            return PhysicalFile(sitemapXmlModel.SitemapXmlPath, MimeTypes.ApplicationXml);
        }
        catch
        {
			logger.LogError("Exception in SitemapXml(id={Id}): {Arg1}", id, ex.Message);
			logger.LogDebug("<<< Exiting SitemapXml(id={Id})", id);
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public virtual async Task<IActionResult> SetStoreTheme(string themeName, string returnUrl = "")
    {
		logger.LogDebug(">>> Entering SetStoreTheme(themeName={ThemeName},returnUrl={ReturnUrl})", themeName, returnUrl);
        await _themeContext.SetWorkingThemeNameAsync(themeName);

		logger.LogDebug("SetStoreTheme(themeName={ThemeName},returnUrl={ReturnUrl}): returnUrl → {ReturnUrl1}", themeName, returnUrl, returnUrl);
        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

		logger.LogDebug("<<< Exiting SetStoreTheme(themeName={ThemeName},returnUrl={ReturnUrl})", themeName, returnUrl);
        return Redirect(returnUrl);
    }

    [HttpPost]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> EuCookieLawAccept()
    {
		logger.LogDebug(">>> Entering EuCookieLawAccept()");
        if (!_storeInformationSettings.DisplayEuCookieLawWarning)
            //disabled
            return Json(new { stored = false });

        //save setting
        var store = await _storeContext.GetCurrentStoreAsync();
        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.EuCookieLawAcceptedAttribute, true, store.Id);
		logger.LogDebug("EuCookieLawAccept(): store → {Store}", store);
		logger.LogDebug("<<< Exiting EuCookieLawAccept()");
        return Json(new { stored = true });
    }

    //robots.txt file
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> RobotsTextFile()
    {
		logger.LogDebug(">>> Entering RobotsTextFile()");
        var robotsFileContent = await _commonModelFactory.PrepareRobotsTextFileAsync();

		logger.LogDebug("RobotsTextFile(): robotsFileContent → {RobotsFileContent}", robotsFileContent);
		logger.LogDebug("<<< Exiting RobotsTextFile()");
        return Content(robotsFileContent, MimeTypes.TextPlain);
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual IActionResult GenericUrl()
    {
		logger.LogDebug(">>> Entering GenericUrl()");
        //seems that no entity was found
		logger.LogDebug("<<< Exiting GenericUrl()");
        return InvokeHttp404();
    }

    //store is closed
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual IActionResult StoreClosed()
    {
		logger.LogDebug(">>> Entering StoreClosed()");
		logger.LogDebug("<<< Exiting StoreClosed()");
        return View();
    }

    //helper method to redirect users. Workaround for GenericPathRoute class where we're not allowed to do it
    public virtual IActionResult InternalRedirect(string url, bool permanentRedirect)
    {
		logger.LogDebug(">>> Entering InternalRedirect(url={Url},permanentRedirect={PermanentRedirect})", url, permanentRedirect);
        //ensure it's invoked from our GenericPathRoute class
        if (!HttpContext.Items.TryGetValue(NopHttpDefaults.GenericRouteInternalRedirect, out var value) || value is not bool redirect || !redirect)
        {
            url = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);
            permanentRedirect = false;
			logger.LogDebug("InternalRedirect(url={Url},permanentRedirect={PermanentRedirect}): url → {Url1}", url, permanentRedirect, url);
			logger.LogDebug("InternalRedirect(url={Url},permanentRedirect={PermanentRedirect}): permanentRedirect → {PermanentRedirect1}", url, permanentRedirect, permanentRedirect);
        }

        //home page
        if (string.IsNullOrEmpty(url))
        {
            url = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);
            permanentRedirect = false;
        }

        //prevent open redirection attack
        if (!Url.IsLocalUrl(url))
        {
            url = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);
            permanentRedirect = false;
        }

        if (permanentRedirect)
		logger.LogDebug("<<< Exiting InternalRedirect(url={Url},permanentRedirect={PermanentRedirect})", url, permanentRedirect);
            return RedirectPermanent(url);

        return Redirect(url);
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual IActionResult FallbackRedirect()
    {
		logger.LogDebug(">>> Entering FallbackRedirect()");
        //nothing was found
		logger.LogDebug("<<< Exiting FallbackRedirect()");
        return InvokeHttp404();
    }

    #endregion
}
