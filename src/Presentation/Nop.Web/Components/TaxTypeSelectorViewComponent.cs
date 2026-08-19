using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class TaxTypeSelectorViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly TaxSettings _taxSettings;

    public TaxTypeSelectorViewComponent(ICommonModelFactory commonModelFactory,
        TaxSettings taxSettings)
    {
		_logger.LogDebug(">>> Entering TaxTypeSelectorViewComponent(commonModelFactory={CommonModelFactory},taxSettings={TaxSettings})", commonModelFactory, taxSettings);
        _commonModelFactory = commonModelFactory;
        _taxSettings = taxSettings;
		_logger.LogDebug("TaxTypeSelectorViewComponent(commonModelFactory={CommonModelFactory},taxSettings={TaxSettings}): _taxSettings → {_taxSettings}", commonModelFactory, taxSettings, _taxSettings);
		_logger.LogDebug("TaxTypeSelectorViewComponent(commonModelFactory={CommonModelFactory},taxSettings={TaxSettings}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, taxSettings, _commonModelFactory);
		_logger.LogDebug("<<< Exiting TaxTypeSelectorViewComponent(commonModelFactory={CommonModelFactory},taxSettings={TaxSettings})", commonModelFactory, taxSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
            return Content("");

        var model = await _commonModelFactory.PrepareTaxTypeSelectorModelAsync();
		_logger.LogDebug("InvokeAsync(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}