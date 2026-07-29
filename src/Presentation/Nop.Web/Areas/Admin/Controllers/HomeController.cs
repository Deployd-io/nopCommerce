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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class HomeController : BaseAdminController
{
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
        _adminAreaSettings = adminAreaSettings;
        _commonModelFactory = commonModelFactory;
        _homeModelFactory = homeModelFactory;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _workContext = workContext;
        _genericAttributeService = genericAttributeService;
        _nopHttpClient = nopHttpClient;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> Index()
    {
		logger.LogDebug(">>> Entering Index()");
        //display a warning to a store owner if there are some error
        var customer = await _workContext.GetCurrentCustomerAsync();
        var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
		logger.LogDebug("Index(): customer → {Customer}", customer);
		logger.LogDebug("Index(): hideCard → {HideCard}", hideCard);
        var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

		logger.LogDebug("Index(): closeCard → {CloseCard}", closeCard);
        if ((hideCard || closeCard) && await _permissionService.AuthorizeAsync(StandardPermission.System.MANAGE_MAINTENANCE))
        {
            var warnings = await _commonModelFactory.PrepareSystemWarningModelsAsync();
            if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail || warning.Level == SystemWarningLevel.Warning))
			logger.LogDebug("Index(): warnings → {Warnings}", warnings);
            {
                var locale = await _localizationService.GetResourceAsync("Admin.System.Warnings.Errors");
                _notificationService.WarningNotification(string.Format(locale, Url.Action("Warnings", "Common")), false); //do not encode URLs
				logger.LogDebug("Index(): locale → {Locale}", locale);
            }
        }

        //progress of localization 
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
        var progress = await _genericAttributeService.GetAttributeAsync<string>(currentLanguage, NopCommonDefaults.LanguagePackProgressAttribute);
		logger.LogDebug("Index(): currentLanguage → {CurrentLanguage}", currentLanguage);
		logger.LogDebug("Index(): progress → {Progress}", progress);
        if (!string.IsNullOrEmpty(progress))
        {
            var locale = await _localizationService.GetResourceAsync("Admin.Configuration.LanguagePackProgressMessage");
            _notificationService.SuccessNotification(string.Format(locale, progress, NopLinksDefaults.OfficialSite.Translations), false);
            await _genericAttributeService.SaveAttributeAsync(currentLanguage, NopCommonDefaults.LanguagePackProgressAttribute, string.Empty);
        }

        //prepare model
        var model = await _homeModelFactory.PrepareDashboardModelAsync(new DashboardModel());

		logger.LogDebug("Index(): model → {Model}", model);
		logger.LogDebug("<<< Exiting Index()");
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> NopCommerceNewsHideAdv()
    {
		logger.LogDebug(">>> Entering NopCommerceNewsHideAdv()");
        _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
        await _settingService.SaveSettingAsync(_adminAreaSettings);
		logger.LogDebug("NopCommerceNewsHideAdv(): _adminAreaSettings.HideAdvertisementsOnAdminArea → {Arg0}", _adminAreaSettings.HideAdvertisementsOnAdminArea);

		logger.LogDebug("<<< Exiting NopCommerceNewsHideAdv()");
        return Content("Setting changed");
    }

    public virtual async Task<IActionResult> GetPopularSearchTerm()
    {
		logger.LogDebug(">>> Entering GetPopularSearchTerm()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PreparePopularSearchTermReportModelAsync(model);
		logger.LogDebug("GetPopularSearchTerm(): model → {Model}", model);
		logger.LogDebug("<<< Exiting GetPopularSearchTerm()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetBestsellersBriefReportByAmount()
    {
		logger.LogDebug(">>> Entering GetBestsellersBriefReportByAmount()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareBestsellersBriefReportByAmountModelAsync(model);
		logger.LogDebug("GetBestsellersBriefReportByAmount(): model → {Model}", model);
		logger.LogDebug("<<< Exiting GetBestsellersBriefReportByAmount()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetBestsellersBriefReportByQuantity()
    {
		logger.LogDebug(">>> Entering GetBestsellersBriefReportByQuantity()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareBestsellersBriefReportByQuantityModelAsync(model);
		logger.LogDebug("GetBestsellersBriefReportByQuantity(): model → {Model}", model);
		logger.LogDebug("<<< Exiting GetBestsellersBriefReportByQuantity()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetLatestOrders()
    {
		logger.LogDebug(">>> Entering GetLatestOrders()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareLatestOrdersModelAsync(model);
		logger.LogDebug("GetLatestOrders(): model → {Model}", model);
		logger.LogDebug("<<< Exiting GetLatestOrders()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetOrderIncomplete()
    {
		logger.LogDebug(">>> Entering GetOrderIncomplete()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareOrderIncompleteModelAsync(model);
		logger.LogDebug("GetOrderIncomplete(): model → {Model}", model);
		logger.LogDebug("<<< Exiting GetOrderIncomplete()");
        return PartialView("Table", model);
    }

    public virtual async Task<IActionResult> GetOrderAverage()
    {
		logger.LogDebug(">>> Entering GetOrderAverage()");
        var model = new DataTablesModel();
        model = await _homeModelFactory.PrepareOrderAverageModelAsync(model);
		logger.LogDebug("GetOrderAverage(): model → {Model}", model);
		logger.LogDebug("<<< Exiting GetOrderAverage()");
        return PartialView("Table", model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AcceptLicenseTerms()
    {
		logger.LogDebug(">>> Entering AcceptLicenseTerms()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!(await _settingService.TryGetLicenseAsync(true)).Exists)
            return Json(new { Result = false });

        try
        {
            await _nopHttpClient.CheckLicenseTermsAsync(true);
        }
        catch { }

		logger.LogError("Exception in AcceptLicenseTerms(): {Arg0}", ex.Message);
		logger.LogInformation("AcceptLicenseTerms(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting AcceptLicenseTerms()");
        return Json(new { Result = true });
    }

    #endregion
}