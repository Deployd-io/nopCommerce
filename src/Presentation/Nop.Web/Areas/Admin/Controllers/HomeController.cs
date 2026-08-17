using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Framework.Models.DataTables;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class HomeController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AdminAreaSettings _adminAreaSettings;
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly IHomeModelFactory _homeModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IWorkContext _workContext;
    protected readonly NopHttpClient _nopHttpClient;

    #endregion

    #region Ctor

    public HomeController(AdminAreaSettings adminAreaSettings,
        ICommonModelFactory commonModelFactory,
        IHomeModelFactory homeModelFactory,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IGenericAttributeService genericAttributeService,
        IWorkContext workContext,
        NopHttpClient nopHttpClient)
    {
		_logger.LogDebug(">>> Entering HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient})", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient);
        _adminAreaSettings = adminAreaSettings;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _adminAreaSettings → {_adminAreaSettings}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _adminAreaSettings);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _commonModelFactory → {_commonModelFactory}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _commonModelFactory);
        _homeModelFactory = homeModelFactory;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _homeModelFactory → {_homeModelFactory}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _homeModelFactory);
        _localizationService = localizationService;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _localizationService → {_localizationService}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _notificationService → {_notificationService}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _permissionService → {_permissionService}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _settingService → {_settingService}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _settingService);
        _workContext = workContext;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _workContext → {_workContext}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _workContext);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _genericAttributeService → {_genericAttributeService}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _genericAttributeService);
        _nopHttpClient = nopHttpClient;
		_logger.LogDebug("<<< Exiting HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient})", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient);
		_logger.LogDebug("HomeController(adminAreaSettings={AdminAreaSettings},commonModelFactory={CommonModelFactory},homeModelFactory={HomeModelFactory},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},genericAttributeService={GenericAttributeService},workContext={WorkContext},nopHttpClient={NopHttpClient}): _nopHttpClient → {_nopHttpClient}", adminAreaSettings, commonModelFactory, homeModelFactory, localizationService, notificationService, permissionService, settingService, genericAttributeService, workContext, nopHttpClient, _nopHttpClient);
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> Index()
    {
		_logger.LogDebug(">>> Entering Index()");
        //display a warning to a store owner if there are some error
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("Index(): customer → {Customer}", customer);
        var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
		_logger.LogDebug("Index(): hideCard → {HideCard}", hideCard);
        var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);
		_logger.LogDebug("Index(): closeCard → {CloseCard}", closeCard);

        if ((hideCard || closeCard) && await _permissionService.AuthorizeAsync(StandardPermission.System.MANAGE_MAINTENANCE))
        {
            var warnings = await _commonModelFactory.PrepareSystemWarningModelsAsync();
			_logger.LogDebug("Index(): warnings → {Warnings}", warnings);
            if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail || warning.Level == SystemWarningLevel.Warning))
            {
                var locale = await _localizationService.GetResourceAsync("Admin.System.Warnings.Errors");
				_logger.LogDebug("Index(): locale → {Locale}", locale);
                _notificationService.WarningNotification(string.Format(locale, Url.Action("Warnings", "Common")), false); //do not encode URLs
            }
        }

        //progress of localization 
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
		_logger.LogDebug("Index(): currentLanguage → {CurrentLanguage}", currentLanguage);
        var progress = await _genericAttributeService.GetAttributeAsync<string>(currentLanguage, NopCommonDefaults.LanguagePackProgressAttribute);
		_logger.LogDebug("Index(): progress → {Progress}", progress);
        if (!string.IsNullOrEmpty(progress))
        {
            var locale = await _localizationService.GetResourceAsync("Admin.Configuration.LanguagePackProgressMessage");
            _notificationService.SuccessNotification(string.Format(locale, progress, NopLinksDefaults.OfficialSite.Translations), false);
            await _genericAttributeService.SaveAttributeAsync(currentLanguage, NopCommonDefaults.LanguagePackProgressAttribute, string.Empty);
        }

        //prepare model
        var model = await _homeModelFactory.PrepareDashboardModelAsync(new DashboardModel());
		_logger.LogDebug("Index(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Index()");
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> NopCommerceNewsHideAdv()
    {
		_logger.LogDebug(">>> Entering NopCommerceNewsHideAdv()");
        _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
        await _settingService.SaveSettingAsync(_adminAreaSettings);
		_logger.LogDebug("NopCommerceNewsHideAdv(): _adminAreaSettings.HideAdvertisementsOnAdminArea → {Arg0}", _adminAreaSettings.HideAdvertisementsOnAdminArea);

		_logger.LogDebug("<<< Exiting NopCommerceNewsHideAdv()");
        return Content("Setting changed");
    }

    public virtual async Task<IActionResult> GetPopularSearchTerm()
    {
		_logger.LogDebug(">>> Entering GetPopularSearchTerm()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PreparePopularSearchTermReportModelAsync(model);
		_logger.LogDebug("GetPopularSearchTerm(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting GetPopularSearchTerm()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetBestsellersBriefReportByAmount()
    {
		_logger.LogDebug(">>> Entering GetBestsellersBriefReportByAmount()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareBestsellersBriefReportByAmountModelAsync(model);
		_logger.LogDebug("GetBestsellersBriefReportByAmount(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting GetBestsellersBriefReportByAmount()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetBestsellersBriefReportByQuantity()
    {
		_logger.LogDebug(">>> Entering GetBestsellersBriefReportByQuantity()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareBestsellersBriefReportByQuantityModelAsync(model);
		_logger.LogDebug("GetBestsellersBriefReportByQuantity(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting GetBestsellersBriefReportByQuantity()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetLatestOrders()
    {
		_logger.LogDebug(">>> Entering GetLatestOrders()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareLatestOrdersModelAsync(model);
		_logger.LogDebug("GetLatestOrders(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting GetLatestOrders()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetOrderIncomplete()
    {
		_logger.LogDebug(">>> Entering GetOrderIncomplete()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareOrderIncompleteModelAsync(model);
		_logger.LogDebug("GetOrderIncomplete(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting GetOrderIncomplete()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetOrderAverage()
    {
		_logger.LogDebug(">>> Entering GetOrderAverage()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareOrderAverageModelAsync(model);
		_logger.LogDebug("GetOrderAverage(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting GetOrderAverage()");
        return PartialView("Table", model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AcceptLicenseTerms()
    {
		_logger.LogDebug(">>> Entering AcceptLicenseTerms()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!(await _settingService.TryGetLicenseAsync(true)).Exists)
            return Json(new { Result = false });

        try
        {
            await _nopHttpClient.CheckLicenseTermsAsync(true);
        }
        catch { }
		_logger.LogError("Exception in AcceptLicenseTerms(): {Arg0}", ex.Message);

		_logger.LogInformation("AcceptLicenseTerms(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting AcceptLicenseTerms()");
        return Json(new { Result = true });
    }

    #endregion
}