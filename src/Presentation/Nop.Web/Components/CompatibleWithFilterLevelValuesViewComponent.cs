using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.FilterLevels;
using Nop.Services.FilterLevels;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class CompatibleWithFilterLevelValuesViewComponent : NopViewComponent
{
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;

    public CompatibleWithFilterLevelValuesViewComponent(FilterLevelSettings filterLevelSettings,
        IFilterLevelValueModelFactory filterLevelValueModelFactory,
        IFilterLevelValueService filterLevelValueService)
    {
        _filterLevelSettings = filterLevelSettings;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
        _filterLevelValueService = filterLevelValueService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId)
    {
		logger.LogDebug(">>> Entering InvokeAsync(productId={ProductId})", productId);
        if (_filterLevelSettings.FilterLevelEnabled && _filterLevelSettings.DisplayOnProductDetailsPage)
        {
            //get filter level values
            var filterLevelValues = await _filterLevelValueService
                .GetFilterLevelValuesByProductIdAsync(productId: productId);            

            var model = _filterLevelValueModelFactory.PrepareFilterLevelValueOverviewModel(filterLevelValues);

			logger.LogDebug("InvokeAsync(productId={ProductId}): model → {Model}", productId, model);
            return View(model);
        }

		logger.LogDebug("<<< Exiting InvokeAsync(productId={ProductId})", productId);
        return Content(string.Empty);
    }
}
