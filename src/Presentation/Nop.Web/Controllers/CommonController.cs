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
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CommonController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings})", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings);
        _captchaSettings = captchaSettings;
        _commonSettings = commonSettings;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _commonSettings → {_commonSettings}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _commonSettings);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _commonModelFactory → {_commonModelFactory}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _commonModelFactory);
        _currencyService = currencyService;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _currencyService → {_currencyService}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _currencyService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _customerActivityService → {_customerActivityService}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _customerActivityService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _genericAttributeService → {_genericAttributeService}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _genericAttributeService);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _htmlFormatter → {_htmlFormatter}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _htmlFormatter);
        _languageService = languageService;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _languageService → {_languageService}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _localizationService → {_localizationService}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _localizationService);
        _sitemapModelFactory = sitemapModelFactory;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _sitemapModelFactory → {_sitemapModelFactory}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _sitemapModelFactory);
        _storeContext = storeContext;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _storeContext → {_storeContext}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _storeContext);
        _themeContext = themeContext;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _themeContext → {_themeContext}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _themeContext);
        _vendorService = vendorService;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _vendorService → {_vendorService}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _vendorService);
        _workContext = workContext;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _workContext → {_workContext}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _workflowMessageService → {_workflowMessageService}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _workflowMessageService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _localizationSettings → {_localizationSettings}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _localizationSettings);
        _sitemapSettings = sitemapSettings;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _sitemapSettings → {_sitemapSettings}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _sitemapSettings);
        _sitemapXmlSettings = sitemapXmlSettings;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _sitemapXmlSettings → {_sitemapXmlSettings}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _sitemapXmlSettings);
        _storeInformationSettings = storeInformationSettings;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _storeInformationSettings → {_storeInformationSettings}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _storeInformationSettings);
        _vendorSettings = vendorSettings;
		_logger.LogDebug("CommonController(captchaSettings={CaptchaSettings},commonSettings={CommonSettings},commonModelFactory={CommonModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},sitemapModelFactory={SitemapModelFactory},storeContext={StoreContext},themeContext={ThemeContext},vendorService={VendorService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},sitemapSettings={SitemapSettings},sitemapXmlSettings={SitemapXmlSettings},storeInformationSettings={StoreInformationSettings},vendorSettings={VendorSettings}): _vendorSettings → {_vendorSettings}", captchaSettings, commonSettings, commonModelFactory, currencyService, customerActivityService, genericAttributeService, htmlFormatter, languageService, localizationService, sitemapModelFactory, storeContext, themeContext, vendorService, workContext, workflowMessageService, localizationSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings, _vendorSettings);
    }

    #endregion

    #region Methods

    public virtual IActionResult PageNotFound()
    {
		_logger.LogError(">>> Entering PageNotFound()");
        _logger.LogWarning("<<< Exiting PageNotFound()");
        return View();
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetLanguage(int langid, string returnUrl = "")
    {
		_logger.LogError(">>> Entering SetLanguage(langid={Langid},returnUrl={ReturnUrl})", langid, returnUrl);
        var language = await _languageService.GetLanguageByIdAsync(langid);
			_logger.LogError("SetLanguage(langid={Langid},returnUrl={ReturnUrl}): language → {Language}", langid, returnUrl, language);
        if (!language?.Published ?? false) {
            language = await _workContext.GetWorkingLanguageAsync();
        }

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);
			_logger.LogError("SetLanguage(langid={Langid},returnUrl={ReturnUrl}): returnUrl → {ReturnUrl1}", langid, returnUrl, returnUrl);

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

		_logger.LogWarning("<<< Exiting SetLanguage(langid={Langid},returnUrl={ReturnUrl})", langid, returnUrl);
        return Redirect(returnUrl);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetCurrency(int customerCurrency, string returnUrl = "")
    {
		_logger.LogError(">>> Entering SetCurrency(customerCurrency={CustomerCurrency},returnUrl={ReturnUrl})", customerCurrency, returnUrl);
        var currency = await _currencyService.GetCurrencyByIdAsync(customerCurrency);
			_logger.LogError("SetCurrency(customerCurrency={CustomerCurrency},returnUrl={ReturnUrl}): currency → {Currency}", customerCurrency, returnUrl, currency);
        if (currency != null) {
            await _workContext.SetWorkingCurrencyAsync(currency);
        }

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);
			_logger.LogError("SetCurrency(customerCurrency={CustomerCurrency},returnUrl={ReturnUrl}): returnUrl → {ReturnUrl1}", customerCurrency, returnUrl, returnUrl);

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

		_logger.LogWarning("<<< Exiting SetCurrency(customerCurrency={CustomerCurrency},returnUrl={ReturnUrl})", customerCurrency, returnUrl);
        return Redirect(returnUrl);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetTaxType(int customerTaxType, string returnUrl = "")
    {
		_logger.LogError(">>> Entering SetTaxType(customerTaxType={CustomerTaxType},returnUrl={ReturnUrl})", customerTaxType, returnUrl);
        var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
        await _workContext.SetTaxDisplayTypeAsync(taxDisplayType);
		_logger.LogError("SetTaxType(customerTaxType={CustomerTaxType},returnUrl={ReturnUrl}): taxDisplayType → {TaxDisplayType}", customerTaxType, returnUrl, taxDisplayType);

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);
			_logger.LogError("SetTaxType(customerTaxType={CustomerTaxType},returnUrl={ReturnUrl}): returnUrl → {ReturnUrl1}", customerTaxType, returnUrl, returnUrl);

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

		_logger.LogWarning("<<< Exiting SetTaxType(customerTaxType={CustomerTaxType},returnUrl={ReturnUrl})", customerTaxType, returnUrl);
        return Redirect(returnUrl);
    }

    //contact us page
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> ContactUs()
    {
		_logger.LogError(">>> Entering ContactUs()");
        var model = new ContactUsModel();
        model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);
		_logger.LogError("ContactUs(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting ContactUs()");
        return View(model);
    }

    [HttpPost, ActionName("ContactUs")]
    [ValidateCaptcha]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> ContactUsSend(ContactUsModel model, bool captchaValid)
    {
		_logger.LogError(">>> Entering ContactUsSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);
		_logger.LogError("ContactUsSend(model={Model},captchaValid={CaptchaValid}): model → {Model1}", model, captchaValid, model);

        if (ModelState.IsValid)
        {
            var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
            var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);
			_logger.LogError("ContactUsSend(model={Model},captchaValid={CaptchaValid}): body → {Body}", model, captchaValid, body);

            await _workflowMessageService.SendContactUsMessageAsync((await _workContext.GetWorkingLanguageAsync()).Id,
                model.Email, model.FullName, subject, body);

            model.SuccessfullySent = true;
			_logger.LogDebug("ContactUsSend(model={Model},captchaValid={CaptchaValid}): model.SuccessfullySent → {Arg2}", model, captchaValid, model.SuccessfullySent);
            model.Result = await _localizationService.GetResourceAsync("ContactUs.YourEnquiryHasBeenSent");

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ContactUs",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ContactUs"));

            return View(model);
        }

		_logger.LogWarning("<<< Exiting ContactUsSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    //contact vendor page
    public virtual async Task<IActionResult> ContactVendor(int vendorId)
    {
		_logger.LogError(">>> Entering ContactVendor(vendorId={VendorId})", vendorId);
        if (!_vendorSettings.AllowCustomersToContactVendors)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
		_logger.LogError("ContactVendor(vendorId={VendorId}): vendor → {Vendor}", vendorId, vendor);
        if (vendor == null || !vendor.Active || vendor.Deleted)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new ContactVendorModel();
        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, vendor, false);
		_logger.LogError("ContactVendor(vendorId={VendorId}): model → {Model}", vendorId, model);

		_logger.LogWarning("<<< Exiting ContactVendor(vendorId={VendorId})", vendorId);
        return View(model);
    }

    [HttpPost, ActionName("ContactVendor")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ContactVendorSend(ContactVendorModel model, bool captchaValid)
    {
		_logger.LogError(">>> Entering ContactVendorSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        if (!_vendorSettings.AllowCustomersToContactVendors)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var vendor = await _vendorService.GetVendorByIdAsync(model.VendorId);
		_logger.LogError("ContactVendorSend(model={Model},captchaValid={CaptchaValid}): vendor → {Vendor}", model, captchaValid, vendor);
        if (vendor == null || !vendor.Active || vendor.Deleted)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, vendor, true);
		_logger.LogError("ContactVendorSend(model={Model},captchaValid={CaptchaValid}): model → {Model1}", model, captchaValid, model);

        if (ModelState.IsValid)
        {
            var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
            var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);
			_logger.LogError("ContactVendorSend(model={Model},captchaValid={CaptchaValid}): body → {Body}", model, captchaValid, body);

            await _workflowMessageService.SendContactVendorMessageAsync(vendor, (await _workContext.GetWorkingLanguageAsync()).Id,
                model.Email, model.FullName, subject, body);

            model.SuccessfullySent = true;
			_logger.LogDebug("ContactVendorSend(model={Model},captchaValid={CaptchaValid}): model.SuccessfullySent → {Arg2}", model, captchaValid, model.SuccessfullySent);
            model.Result = await _localizationService.GetResourceAsync("ContactVendor.YourEnquiryHasBeenSent");

            return View(model);
        }

		_logger.LogWarning("<<< Exiting ContactVendorSend(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    //sitemap page
    public virtual async Task<IActionResult> Sitemap(SitemapPageModel pageModel)
    {
		_logger.LogError(">>> Entering Sitemap(pageModel={PageModel})", pageModel);
        if (!_sitemapSettings.SitemapEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _sitemapModelFactory.PrepareSitemapModelAsync(pageModel);
		_logger.LogError("Sitemap(pageModel={PageModel}): model → {Model}", pageModel, model);

		_logger.LogWarning("<<< Exiting Sitemap(pageModel={PageModel})", pageModel);
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
		_logger.LogError(">>> Entering SitemapXml(id={Id})", id);
        if (!_sitemapXmlSettings.SitemapXmlEnabled)
            return StatusCode(StatusCodes.Status403Forbidden);

        try
        {
            var sitemapXmlModel = await _sitemapModelFactory.PrepareSitemapXmlModelAsync(id ?? 0);
			_logger.LogError("SitemapXml(id={Id}): sitemapXmlModel → {SitemapXmlModel}", id, sitemapXmlModel);
            return PhysicalFile(sitemapXmlModel.SitemapXmlPath, MimeTypes.ApplicationXml);
        }
        catch
        {
			_logger.LogError("Exception in SitemapXml(id={Id}): {Arg1}", id, ex.Message);
			_logger.LogWarning("<<< Exiting SitemapXml(id={Id})", id);
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public virtual async Task<IActionResult> SetStoreTheme(string themeName, string returnUrl = "")
    {
		_logger.LogError(">>> Entering SetStoreTheme(themeName={ThemeName},returnUrl={ReturnUrl})", themeName, returnUrl);
        await _themeContext.SetWorkingThemeNameAsync(themeName);

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);
			_logger.LogError("SetStoreTheme(themeName={ThemeName},returnUrl={ReturnUrl}): returnUrl → {ReturnUrl1}", themeName, returnUrl, returnUrl);

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

		_logger.LogWarning("<<< Exiting SetStoreTheme(themeName={ThemeName},returnUrl={ReturnUrl})", themeName, returnUrl);
        return Redirect(returnUrl);
    }

    [HttpPost]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> EuCookieLawAccept()
    {
		_logger.LogError(">>> Entering EuCookieLawAccept()");
        if (!_storeInformationSettings.DisplayEuCookieLawWarning)
            //disabled
            return Json(new { stored = false });

        //save setting
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("EuCookieLawAccept(): store → {Store}", store);
        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.EuCookieLawAcceptedAttribute, true, store.Id);
		_logger.LogWarning("<<< Exiting EuCookieLawAccept()");
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
		_logger.LogError(">>> Entering RobotsTextFile()");
        var robotsFileContent = await _commonModelFactory.PrepareRobotsTextFileAsync();

		_logger.LogError("RobotsTextFile(): robotsFileContent → {RobotsFileContent}", robotsFileContent);
		_logger.LogWarning("<<< Exiting RobotsTextFile()");
        return Content(robotsFileContent, MimeTypes.TextPlain);
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual IActionResult GenericUrl()
    {
		_logger.LogError(">>> Entering GenericUrl()");
        //seems that no entity was found
		_logger.LogWarning("<<< Exiting GenericUrl()");
        return InvokeHttp404();
    }

    //store is closed
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual IActionResult StoreClosed()
    {
		_logger.LogError(">>> Entering StoreClosed()");
        _logger.LogWarning("<<< Exiting StoreClosed()");
        return View();
    }

    //helper method to redirect users. Workaround for GenericPathRoute class where we're not allowed to do it
    public virtual IActionResult InternalRedirect(string url, bool permanentRedirect)
    {
		_logger.LogError(">>> Entering InternalRedirect(url={Url},permanentRedirect={PermanentRedirect})", url, permanentRedirect);
        //ensure it's invoked from our GenericPathRoute class
        if (!HttpContext.Items.TryGetValue(NopHttpDefaults.GenericRouteInternalRedirect, out var value) || value is not bool redirect || !redirect)
        {
            url = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);
			_logger.LogError("InternalRedirect(url={Url},permanentRedirect={PermanentRedirect}): url → {Url1}", url, permanentRedirect, url);
            permanentRedirect = false;
			_logger.LogError("InternalRedirect(url={Url},permanentRedirect={PermanentRedirect}): permanentRedirect → {PermanentRedirect1}", url, permanentRedirect, permanentRedirect);
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

		_logger.LogWarning("<<< Exiting InternalRedirect(url={Url},permanentRedirect={PermanentRedirect})", url, permanentRedirect);
        if (permanentRedirect) {
            return RedirectPermanent(url);
        }

        return Redirect(url);
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual IActionResult FallbackRedirect()
    {
		_logger.LogError(">>> Entering FallbackRedirect()");
        //nothing was found
		_logger.LogWarning("<<< Exiting FallbackRedirect()");
        return InvokeHttp404();
    }

    #endregion
}
