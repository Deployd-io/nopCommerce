using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.FilterLevels;
using Nop.Services.FilterLevels;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class CompatibleWithFilterLevelValuesViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;

    public CompatibleWithFilterLevelValuesViewComponent(FilterLevelSettings filterLevelSettings,
        IFilterLevelValueModelFactory filterLevelValueModelFactory,
        IFilterLevelValueService filterLevelValueService)
    {
		_logger.LogError(">>> Entering CompatibleWithFilterLevelValuesViewComponent(filterLevelSettings={FilterLevelSettings},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService})", filterLevelSettings, filterLevelValueModelFactory, filterLevelValueService);
        _filterLevelSettings = filterLevelSettings;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
		_logger.LogDebug("CompatibleWithFilterLevelValuesViewComponent(filterLevelSettings={FilterLevelSettings},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService}): _filterLevelValueModelFactory → {_filterLevelValueModelFactory}", filterLevelSettings, filterLevelValueModelFactory, filterLevelValueService, _filterLevelValueModelFactory);
        _filterLevelValueService = filterLevelValueService;
		_logger.LogDebug("CompatibleWithFilterLevelValuesViewComponent(filterLevelSettings={FilterLevelSettings},filterLevelValueModelFactory={FilterLevelValueModelFactory},filterLevelValueService={FilterLevelValueService}): _filterLevelValueService → {_filterLevelValueService}", filterLevelSettings, filterLevelValueModelFactory, filterLevelValueService, _filterLevelValueService);
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId)
    {
		_logger.LogError(">>> Entering InvokeAsync(productId={ProductId})", productId);
        if (_filterLevelSettings.FilterLevelEnabled && _filterLevelSettings.DisplayOnProductDetailsPage)
        {
            //get filter level values
            var filterLevelValues = await _filterLevelValueService
                .GetFilterLevelValuesByProductIdAsync(productId: productId);            

            var model = _filterLevelValueModelFactory.PrepareFilterLevelValueOverviewModel(filterLevelValues);
			_logger.LogError("InvokeAsync(productId={ProductId}): model → {Model}", productId, model);

            return View(model);
        }

		_logger.LogWarning("<<< Exiting InvokeAsync(productId={ProductId})", productId);
        return Content(string.Empty);
    }
}
