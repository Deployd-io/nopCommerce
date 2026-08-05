using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class TaxController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly ITaxModelFactory _taxModelFactory;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public TaxController(IPermissionService permissionService,
        ISettingService settingService,
        ITaxCategoryService taxCategoryService,
        ITaxModelFactory taxModelFactory,
        ITaxPluginManager taxPluginManager,
        TaxSettings taxSettings)
    {
        _permissionService = permissionService;
        _settingService = settingService;
        _taxCategoryService = taxCategoryService;
        _taxModelFactory = taxModelFactory;
        _taxPluginManager = taxPluginManager;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Methods

    #region Tax Providers

    public virtual IActionResult List()
    {
		_logger.LogDebug(">>> Entering List()");
		_logger.LogDebug("<<< Exiting List()");
        return RedirectToAction("Providers");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> Providers()
    {
		_logger.LogDebug(">>> Entering Providers()");
        //prepare model
        var model = await _taxModelFactory.PrepareTaxProviderSearchModelAsync(new TaxProviderSearchModel());

		_logger.LogDebug("Providers(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting Providers()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> Providers(TaxProviderSearchModel searchModel)
    {
        //prepare model
        var model = await _taxModelFactory.PrepareTaxProviderListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting Providers(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> MarkAsPrimaryProvider(string systemName)
    {
		_logger.LogDebug(">>> Entering MarkAsPrimaryProvider(systemName={SystemName})", systemName);
        if (string.IsNullOrEmpty(systemName))
            return RedirectToAction("Providers");

        var taxProvider = await _taxPluginManager.LoadPluginBySystemNameAsync(systemName);
        if (taxProvider == null)
			_logger.LogDebug("MarkAsPrimaryProvider(systemName={SystemName}): taxProvider → {TaxProvider}", systemName, taxProvider);
            return RedirectToAction("Providers");

        _taxSettings.ActiveTaxProviderSystemName = systemName;
		_logger.LogDebug("MarkAsPrimaryProvider(systemName={SystemName}): _taxSettings.ActiveTaxProviderSystemName → {Arg1}", systemName, _taxSettings.ActiveTaxProviderSystemName);
        await _settingService.SaveSettingAsync(_taxSettings);

		_logger.LogDebug("<<< Exiting MarkAsPrimaryProvider(systemName={SystemName})", systemName);
        return RedirectToAction("Providers");
    }

    #endregion

    #region Tax Categories

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> Categories()
    {
		_logger.LogDebug(">>> Entering Categories()");
        //prepare model
        var model = await _taxModelFactory.PrepareTaxCategorySearchModelAsync(new TaxCategorySearchModel());

		_logger.LogDebug("Categories(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting Categories()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> Categories(TaxCategorySearchModel searchModel)
    {
        //prepare model
        var model = await _taxModelFactory.PrepareTaxCategoryListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting Categories(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> CategoryUpdate(TaxCategoryModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(model.Id);
        taxCategory = model.ToEntity(taxCategory);
        await _taxCategoryService.UpdateTaxCategoryAsync(taxCategory);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> CategoryAdd(TaxCategoryModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var taxCategory = new TaxCategory();
        taxCategory = model.ToEntity(taxCategory);
        await _taxCategoryService.InsertTaxCategoryAsync(taxCategory);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> CategoryDelete(int id)
    {
		_logger.LogDebug(">>> Entering CategoryDelete(id={Id})", id);
        //try to get a tax category with the specified id
        var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(id)
            ?? throw new ArgumentException("No tax category found with the specified id", nameof(id));

        await _taxCategoryService.DeleteTaxCategoryAsync(taxCategory);

		_logger.LogDebug("<<< Exiting CategoryDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #endregion
}