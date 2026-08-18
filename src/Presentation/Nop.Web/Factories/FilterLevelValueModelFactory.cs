using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.FilterLevels;
using Nop.Services.Catalog;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Web.Models.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

public partial class FilterLevelValueModelFactory : IFilterLevelValueModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICatalogModelFactory _catalogModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;
    protected readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public FilterLevelValueModelFactory(ICatalogModelFactory catalogModelFactory,
        IFilterLevelValueService filterLevelValueService,
        ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering FilterLevelValueModelFactory(catalogModelFactory={CatalogModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService})", catalogModelFactory, filterLevelValueService, localizationService);
        _catalogModelFactory = catalogModelFactory;
        _filterLevelValueService = filterLevelValueService;
		_logger.LogDebug("FilterLevelValueModelFactory(catalogModelFactory={CatalogModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService}): _filterLevelValueService → {_filterLevelValueService}", catalogModelFactory, filterLevelValueService, localizationService, _filterLevelValueService);
		_logger.LogDebug("FilterLevelValueModelFactory(catalogModelFactory={CatalogModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService}): _catalogModelFactory → {_catalogModelFactory}", catalogModelFactory, filterLevelValueService, localizationService, _catalogModelFactory);
        _localizationService = localizationService;
		_logger.LogDebug("FilterLevelValueModelFactory(catalogModelFactory={CatalogModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService}): _localizationService → {_localizationService}", catalogModelFactory, filterLevelValueService, localizationService, _localizationService);
		_logger.LogDebug("<<< Exiting FilterLevelValueModelFactory(catalogModelFactory={CatalogModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService})", catalogModelFactory, filterLevelValueService, localizationService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare available filter level values
    /// </summary>
    /// <param name="items">Filter level value items</param>
    /// <param name="filterLevelValueEnum">Filter level value enum</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareFilterLevelValuesAsync(IList<SelectListItem> items, FilterLevelEnum filterLevelValueEnum, string filterLevelValue = null)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelValuesAsync(items={Items},filterLevelValueEnum={FilterLevelValueEnum},filterLevelValue={FilterLevelValue})", items, filterLevelValueEnum, filterLevelValue);
        ArgumentNullException.ThrowIfNull(items);

        var param1 = filterLevelValueEnum == FilterLevelEnum.FilterLevel1 ? filterLevelValue : null;
        var param2 = filterLevelValueEnum == FilterLevelEnum.FilterLevel2 ? filterLevelValue : null;
        var param3 = filterLevelValueEnum == FilterLevelEnum.FilterLevel3 ? filterLevelValue : null;

        //prepare available filter level values
        var availableFilterLevelValues = (await _filterLevelValueService.GetAllFilterLevelValuesAsync(param1, param2, param3))
            .Select(filterLevelValue =>
            {
                // filter by filter level value enum
                return filterLevelValueEnum switch
                {
                    FilterLevelEnum.FilterLevel1 => filterLevelValue.FilterLevel1Value,
                    FilterLevelEnum.FilterLevel2 => filterLevelValue.FilterLevel2Value,
                    FilterLevelEnum.FilterLevel3 => filterLevelValue.FilterLevel3Value,
                    _ => string.Empty,
                };
				_logger.LogDebug("PrepareFilterLevelValuesAsync(items={Items},filterLevelValueEnum={FilterLevelValueEnum},filterLevelValue={FilterLevelValue}): FilterLevelEnum.FilterLevel2 → {Arg3}", items, filterLevelValueEnum, filterLevelValue, FilterLevelEnum.FilterLevel2);
				_logger.LogDebug("PrepareFilterLevelValuesAsync(items={Items},filterLevelValueEnum={FilterLevelValueEnum},filterLevelValue={FilterLevelValue}): FilterLevelEnum.FilterLevel1 → {Arg3}", items, filterLevelValueEnum, filterLevelValue, FilterLevelEnum.FilterLevel1);
				_logger.LogDebug("PrepareFilterLevelValuesAsync(items={Items},filterLevelValueEnum={FilterLevelValueEnum},filterLevelValue={FilterLevelValue}): FilterLevelEnum.FilterLevel3 → {Arg3}", items, filterLevelValueEnum, filterLevelValue, FilterLevelEnum.FilterLevel3);
            })
            .Distinct()
            .OrderBy(levelValue => levelValue)
            .ToList();

        foreach (var flv in availableFilterLevelValues)
            items.Add(new SelectListItem { Value = flv, Text = flv });

        //insert special item for the default value
        var defaultItemText = await _localizationService.GetResourceAsync("Admin.Common.Select");
		_logger.LogDebug("PrepareFilterLevelValuesAsync(items={Items},filterLevelValueEnum={FilterLevelValueEnum},filterLevelValue={FilterLevelValue}): defaultItemText → {DefaultItemText}", items, filterLevelValueEnum, filterLevelValue, defaultItemText);
        items.Insert(0, new SelectListItem { Text = defaultItemText, Value = "" });
		_logger.LogDebug("<<< Exiting PrepareFilterLevelValuesAsync(items={Items},filterLevelValueEnum={FilterLevelValueEnum},filterLevelValue={FilterLevelValue})", items, filterLevelValueEnum, filterLevelValue);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the filter level value overview model
    /// </summary>
    /// <param name="filterLevelValues">Filter level values</param>
    /// <returns>
    /// The result contains the filter level value overview model
    /// </returns>
    public virtual FilterLevelValueOverviewModel PrepareFilterLevelValueOverviewModel(IList<FilterLevelValue> filterLevelValues)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelValueOverviewModel(filterLevelValues={FilterLevelValues})", filterLevelValues);
        ArgumentNullException.ThrowIfNull(filterLevelValues);

        var (filterLevel1Disabled, filterLevel2Disabled, filterLevel3Disabled) = _filterLevelValueService.IsFilterLevelDisabled();

        var model = new FilterLevelValueOverviewModel
        {
            TotalFilterLevelValues = filterLevelValues.Count,
            FilterLevel1ValueEnabled = !filterLevel1Disabled,
            FilterLevel2ValueEnabled = !filterLevel2Disabled,
            FilterLevel3ValueEnabled = !filterLevel3Disabled
        };

        foreach (var filterLevelValue in filterLevelValues)
        {
            var itemModel = new FilterLevelValueInfoModel
            {
                FilterLevel1Value = filterLevelValue.FilterLevel1Value,
                FilterLevel2Value = filterLevelValue.FilterLevel2Value,
                FilterLevel3Value = filterLevelValue.FilterLevel3Value,
            };
            model.FilterLevelValues.Add(itemModel);
        }

		_logger.LogDebug("<<< Exiting PrepareFilterLevelValueOverviewModel(filterLevelValues={FilterLevelValues})", filterLevelValues);
        return model;
    }    

    /// <summary>
    /// Prepare filter level value search model
    /// </summary>
    /// <param name="searchModel">Filter level value search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value search model
    /// </returns>
    public virtual async Task<FilterLevelValueSearchModel> PrepareFilterLevelValueSearchModelAsync(FilterLevelValueSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        var (filterLevel1Disabled, filterLevel2Disabled, filterLevel3Disabled) = _filterLevelValueService.IsFilterLevelDisabled();

        //prepare filter (0 - all;)
        searchModel.HideSearchFilterValue1 = filterLevel1Disabled;
		_logger.LogDebug("PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel}): searchModel.HideSearchFilterValue1 → {Arg1}", searchModel, searchModel.HideSearchFilterValue1);
        if (!filterLevel1Disabled)
            await PrepareFilterLevelValuesAsync(searchModel.AvailableFilterLevel1Values, FilterLevelEnum.FilterLevel1);

        searchModel.HideSearchFilterValue2 = filterLevel2Disabled;
		_logger.LogDebug("PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel}): searchModel.HideSearchFilterValue2 → {Arg1}", searchModel, searchModel.HideSearchFilterValue2);
		_logger.LogDebug("PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel}): searchModel.HideSearchFilterValue3 → {Arg1}", searchModel, searchModel.HideSearchFilterValue3);
        searchModel.HideSearchFilterValue3 = filterLevel3Disabled;

		_logger.LogDebug("<<< Exiting PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare search filter level value model
    /// </summary>
    /// <param name="model">Search filter level value model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search filter level value model
    /// </returns>
    public virtual async Task<SearchFilterLevelValueModel> PrepareSearchFilterLevelValueModelAsync(SearchFilterLevelValueModel model, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareSearchFilterLevelValueModelAsync(model={Model},command={Command})", model, command);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(command);

        var (filterLevel1Disabled, filterLevel2Disabled, filterLevel3Disabled) = _filterLevelValueService.IsFilterLevelDisabled();
        model.HideSearchFilterValue1 = filterLevel1Disabled;
		_logger.LogDebug("PrepareSearchFilterLevelValueModelAsync(model={Model},command={Command}): model.HideSearchFilterValue1 → {Arg2}", model, command, model.HideSearchFilterValue1);
		_logger.LogDebug("PrepareSearchFilterLevelValueModelAsync(model={Model},command={Command}): model.HideSearchFilterValue2 → {Arg2}", model, command, model.HideSearchFilterValue2);
		_logger.LogDebug("PrepareSearchFilterLevelValueModelAsync(model={Model},command={Command}): model.HideSearchFilterValue3 → {Arg2}", model, command, model.HideSearchFilterValue3);
        model.HideSearchFilterValue2 = filterLevel2Disabled;
        model.HideSearchFilterValue3 = filterLevel3Disabled;

        if (!filterLevel1Disabled)
        {
            await PrepareFilterLevelValuesAsync(model.AvailableFilterLevel1Values, FilterLevelEnum.FilterLevel1);
            foreach (var item in model.AvailableFilterLevel1Values)
            {
                if (item.Value == model.fl1id)
                    item.Selected = true;
					_logger.LogDebug("PrepareSearchFilterLevelValueModelAsync(model={Model},command={Command}): item.Selected → {Arg2}", model, command, item.Selected);
            }

            if (!string.IsNullOrEmpty(model.fl1id) && !filterLevel2Disabled)
            {
                await PrepareFilterLevelValuesAsync(model.AvailableFilterLevel2Values, FilterLevelEnum.FilterLevel2, model.fl2id);
                foreach (var item in model.AvailableFilterLevel2Values)
                {
                    if (item.Value == model.fl2id)
                        item.Selected = true;
                }

                if (!filterLevel3Disabled)
                {
                    await PrepareFilterLevelValuesAsync(model.AvailableFilterLevel3Values, FilterLevelEnum.FilterLevel3, model.fl3id);
                    foreach (var item in model.AvailableFilterLevel3Values)
                    {
                        if (item.Value == model.fl3id)
                            item.Selected = true;
                    }
                }
            }

            model.CatalogProductsModel = await _catalogModelFactory.PrepareSearchProductsByFilterLevelValuesModelAsync(model, command);
        }

		_logger.LogDebug("<<< Exiting PrepareSearchFilterLevelValueModelAsync(model={Model},command={Command})", model, command);
        return model;
    }

    #endregion
}
