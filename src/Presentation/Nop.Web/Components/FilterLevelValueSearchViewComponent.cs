using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.FilterLevels;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class FilterLevelValueSearchViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;

    public FilterLevelValueSearchViewComponent(FilterLevelSettings filterLevelSettings,
        IFilterLevelValueModelFactory filterLevelValueModelFactory)
    {
		_logger.LogError(">>> Entering FilterLevelValueSearchViewComponent(filterLevelSettings={FilterLevelSettings},filterLevelValueModelFactory={FilterLevelValueModelFactory})", filterLevelSettings, filterLevelValueModelFactory);
        _filterLevelSettings = filterLevelSettings;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
		_logger.LogDebug("FilterLevelValueSearchViewComponent(filterLevelSettings={FilterLevelSettings},filterLevelValueModelFactory={FilterLevelValueModelFactory}): _filterLevelValueModelFactory → {_filterLevelValueModelFactory}", filterLevelSettings, filterLevelValueModelFactory, _filterLevelValueModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        if (_filterLevelSettings.FilterLevelEnabled && _filterLevelSettings.DisplayOnHomePage)
        {
            var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueSearchModelAsync(new FilterLevelValueSearchModel());
			_logger.LogError("InvokeAsync(): model → {Model}", model);

            return View(model);
        }

		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return Content(string.Empty);
    }
}
