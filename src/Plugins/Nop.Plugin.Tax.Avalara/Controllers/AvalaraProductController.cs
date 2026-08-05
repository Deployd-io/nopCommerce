using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class AvalaraProductController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ITaxPluginManager _taxPluginManager;

    #endregion

    #region Ctor

    public AvalaraProductController(AvalaraTaxManager avalaraTaxManager,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ITaxPluginManager taxPluginManager)
    {
        _avalaraTaxManager = avalaraTaxManager;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _taxPluginManager = taxPluginManager;
    }

    #endregion

    #region Methods

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ExportProducts(string selectedIds)
    {
		_logger.LogDebug(">>> Entering ExportProducts(selectedIds={SelectedIds})", selectedIds);
        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
            return RedirectToAction("List", "Product");
        
        //export items
        var exportedItems = await _avalaraTaxManager.ExportProductsAsync(selectedIds);
        if (exportedItems.HasValue)
		_logger.LogDebug("ExportProducts(selectedIds={SelectedIds}): exportedItems → {ExportedItems}", selectedIds, exportedItems);
        {
            if (exportedItems > 0)
                _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Items.Export.Success"), exportedItems));
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Items.Export.AlreadyExported"));
        }
        else
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Items.Export.Error"));

		_logger.LogDebug("<<< Exiting ExportProducts(selectedIds={SelectedIds})", selectedIds);
        return RedirectToAction("List", "Product");
    }

    #endregion
}