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
		_logger.LogDebug(">>> Entering FilterLevelValueSearchViewComponent(filterLevelSettings={FilterLevelSettings},filterLevelValueModelFactory={FilterLevelValueModelFactory})", filterLevelSettings, filterLevelValueModelFactory);
        _filterLevelSettings = filterLevelSettings;
		_logger.LogDebug("FilterLevelValueSearchViewComponent(filterLevelSettings={FilterLevelSettings},filterLevelValueModelFactory={FilterLevelValueModelFactory}): _filterLevelSettings → {_filterLevelSettings}", filterLevelSettings, filterLevelValueModelFactory, _filterLevelSettings);
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        if (_filterLevelSettings.FilterLevelEnabled && _filterLevelSettings.DisplayOnHomePage)
        {
            var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueSearchModelAsync(new FilterLevelValueSearchModel());
			_logger.LogDebug("InvokeAsync(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting InvokeAsync()");
            return View(model);
        }

        return Content(string.Empty);
    }
}
