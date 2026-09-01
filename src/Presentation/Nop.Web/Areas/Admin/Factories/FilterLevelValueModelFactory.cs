using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.FilterLevels;
using Nop.Services.Catalog;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the filter level value model factory implementation
/// </summary>
public partial class FilterLevelValueModelFactory : IFilterLevelValueModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IProductService _productService;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public FilterLevelValueModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        IFilterLevelValueService filterLevelValueService,
        ILocalizationService localizationService,
        IProductService productService,
        IUrlRecordService urlRecordService)
    {
		_logger.LogDebug(">>> Entering FilterLevelValueModelFactory(baseAdminModelFactory={BaseAdminModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService},productService={ProductService},urlRecordService={UrlRecordService})", baseAdminModelFactory, filterLevelValueService, localizationService, productService, urlRecordService);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("FilterLevelValueModelFactory(baseAdminModelFactory={BaseAdminModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService},productService={ProductService},urlRecordService={UrlRecordService}): _baseAdminModelFactory → {_baseAdminModelFactory}", baseAdminModelFactory, filterLevelValueService, localizationService, productService, urlRecordService, _baseAdminModelFactory);
        _filterLevelValueService = filterLevelValueService;
		_logger.LogDebug("FilterLevelValueModelFactory(baseAdminModelFactory={BaseAdminModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService},productService={ProductService},urlRecordService={UrlRecordService}): _filterLevelValueService → {_filterLevelValueService}", baseAdminModelFactory, filterLevelValueService, localizationService, productService, urlRecordService, _filterLevelValueService);
        _localizationService = localizationService;
		_logger.LogDebug("FilterLevelValueModelFactory(baseAdminModelFactory={BaseAdminModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService},productService={ProductService},urlRecordService={UrlRecordService}): _localizationService → {_localizationService}", baseAdminModelFactory, filterLevelValueService, localizationService, productService, urlRecordService, _localizationService);
        _productService = productService;
		_logger.LogDebug("FilterLevelValueModelFactory(baseAdminModelFactory={BaseAdminModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService},productService={ProductService},urlRecordService={UrlRecordService}): _productService → {_productService}", baseAdminModelFactory, filterLevelValueService, localizationService, productService, urlRecordService, _productService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("FilterLevelValueModelFactory(baseAdminModelFactory={BaseAdminModelFactory},filterLevelValueService={FilterLevelValueService},localizationService={LocalizationService},productService={ProductService},urlRecordService={UrlRecordService}): _urlRecordService → {_urlRecordService}", baseAdminModelFactory, filterLevelValueService, localizationService, productService, urlRecordService, _urlRecordService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare filter level value product search model
    /// </summary>
    /// <param name="searchModel">Filter level value product search model</param>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>Filter level value product search model</returns>
    protected virtual FilterLevelValueProductSearchModel PrepareFilterLevelValueProductSearchModel(FilterLevelValueProductSearchModel searchModel, FilterLevelValue filterLevelValue)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelValueProductSearchModel(searchModel={SearchModel},filterLevelValue={FilterLevelValue})", searchModel, filterLevelValue);
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(filterLevelValue);

        searchModel.FilterLevelValueId = filterLevelValue.Id;
		_logger.LogDebug("PrepareFilterLevelValueProductSearchModel(searchModel={SearchModel},filterLevelValue={FilterLevelValue}): searchModel.FilterLevelValueId → {Arg2}", searchModel, filterLevelValue, searchModel.FilterLevelValueId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareFilterLevelValueProductSearchModel(searchModel={SearchModel},filterLevelValue={FilterLevelValue})", searchModel, filterLevelValue);
        return searchModel;
    }    

    #endregion

    #region Methods

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

        // Get all filter level values once
        var allFilterLevelValues = await _filterLevelValueService.GetAllFilterLevelValuesAsync();
		_logger.LogDebug("PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel}): allFilterLevelValues → {AllFilterLevelValues}", searchModel, allFilterLevelValues);

        //prepare filter (0 - all;)
        searchModel.HideSearchFilterValue1 = filterLevel1Disabled;
		_logger.LogDebug("PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel}): searchModel.HideSearchFilterValue1 → {Arg1}", searchModel, searchModel.HideSearchFilterValue1);
		_logger.LogDebug("PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel}): searchModel.HideSearchFilterValue2 → {Arg1}", searchModel, searchModel.HideSearchFilterValue2);
		_logger.LogDebug("PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel}): searchModel.HideSearchFilterValue3 → {Arg1}", searchModel, searchModel.HideSearchFilterValue3);
        searchModel.HideSearchFilterValue2 = filterLevel2Disabled;
        searchModel.HideSearchFilterValue3 = filterLevel3Disabled;

        if (!filterLevel1Disabled)
            await prepareFilterLevelSelectListItemsAsync(searchModel.AvailableFilterLevel1Values, allFilterLevelValues, flv => flv.FilterLevel1Value);

        if (!filterLevel2Disabled)
            await prepareFilterLevelSelectListItemsAsync(searchModel.AvailableFilterLevel2Values, allFilterLevelValues, flv => flv.FilterLevel2Value);

        if (!filterLevel3Disabled)
            await prepareFilterLevelSelectListItemsAsync(searchModel.AvailableFilterLevel3Values, allFilterLevelValues, flv => flv.FilterLevel3Value);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareFilterLevelValueSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;

        async Task prepareFilterLevelSelectListItemsAsync(IList<SelectListItem> items, 
            IList<FilterLevelValue> filterLevelValues, 
            Func<FilterLevelValue, string> valueSelector)
        {
            ArgumentNullException.ThrowIfNull(items);

            // Prepare available filter level values
            var availableValues = filterLevelValues
                .Select(valueSelector)
                .Distinct()
                .OrderBy(value => value)
                .ToList();

            foreach (var value in availableValues)
                items.Add(new SelectListItem { Value = value, Text = value });

            // Insert special item for the default value at first position
            var defaultItemText = await _localizationService.GetResourceAsync("Admin.Common.All");
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = "0" });
			_logger.LogDebug("<<< Exiting prepareFilterLevelSelectListItemsAsync(items={Items},filterLevelValues={FilterLevelValues},valueSelector={ValueSelector})", items, filterLevelValues, valueSelector);
        }
    }

    /// <summary>
    /// Prepare paged filter level value list model
    /// </summary>
    /// <param name="searchModel">Filter level value search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value list model
    /// </returns>
    public virtual async Task<FilterLevelValueListModel> PrepareFilterLevelValueListModelAsync(FilterLevelValueSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelValueListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);
        //get filter level values
        var filterLevelValues = await _filterLevelValueService.GetAllFilterLevelValuesAsync(searchModel.SearchFilterValue1Id, searchModel.SearchFilterValue2Id, searchModel.SearchFilterValue3Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new FilterLevelValueListModel().PrepareToGridAsync(searchModel, filterLevelValues, () =>
        {
            return filterLevelValues.Select(filterLevelValue =>
            {
                //fill in model values from the entity
                var filterLevelValueModel = filterLevelValue.ToModel<FilterLevelValueModel>();
				_logger.LogDebug("PrepareFilterLevelValueListModelAsync(searchModel={SearchModel}): filterLevelValueModel → {FilterLevelValueModel}", searchModel, filterLevelValueModel);

                return filterLevelValueModel;
            }).ToAsyncEnumerable();
        });

		_logger.LogDebug("<<< Exiting PrepareFilterLevelValueListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare filter level value model
    /// </summary>
    /// <param name="model">Filter level value model</param>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>
    /// The result contains the filter level value model
    /// </returns>
    public virtual FilterLevelValueModel PrepareFilterLevelValueModel(FilterLevelValueModel model, FilterLevelValue filterLevelValue)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelValueModel(model={Model},filterLevelValue={FilterLevelValue})", model, filterLevelValue);
        if (filterLevelValue != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = filterLevelValue.ToModel<FilterLevelValueModel>();
				_logger.LogDebug("PrepareFilterLevelValueModel(model={Model},filterLevelValue={FilterLevelValue}): model → {Model1}", model, filterLevelValue, model);
            }

            //prepare nested search model
            PrepareFilterLevelValueProductSearchModel(model.FilterLevelValueProductSearchModel, filterLevelValue);
        }

        var (filterLevel1Disabled, filterLevel2Disabled, filterLevel3Disabled) = _filterLevelValueService.IsFilterLevelDisabled();

        model.FilterLevel1ValueEnabled = !filterLevel1Disabled;
		_logger.LogDebug("PrepareFilterLevelValueModel(model={Model},filterLevelValue={FilterLevelValue}): model.FilterLevel1ValueEnabled → {Arg2}", model, filterLevelValue, model.FilterLevel1ValueEnabled);
		_logger.LogDebug("PrepareFilterLevelValueModel(model={Model},filterLevelValue={FilterLevelValue}): model.FilterLevel2ValueEnabled → {Arg2}", model, filterLevelValue, model.FilterLevel2ValueEnabled);
		_logger.LogDebug("PrepareFilterLevelValueModel(model={Model},filterLevelValue={FilterLevelValue}): model.FilterLevel3ValueEnabled → {Arg2}", model, filterLevelValue, model.FilterLevel3ValueEnabled);
        model.FilterLevel2ValueEnabled = !filterLevel2Disabled;
        model.FilterLevel3ValueEnabled = !filterLevel3Disabled;

		_logger.LogDebug("<<< Exiting PrepareFilterLevelValueModel(model={Model},filterLevelValue={FilterLevelValue})", model, filterLevelValue);
        return model;
    }

    /// <summary>
    /// Prepare paged filter level value product list model
    /// </summary>
    /// <param name="searchModel">Filter level value product search model</param>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value product list model
    /// </returns>
    public virtual async Task<FilterLevelValueProductListModel> PrepareFilterLevelValueProductListModelAsync(FilterLevelValueProductSearchModel searchModel, FilterLevelValue filterLevelValue)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelValueProductListModelAsync(searchModel={SearchModel},filterLevelValue={FilterLevelValue})", searchModel, filterLevelValue);
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(filterLevelValue);

        var filterLevelValueProducts = await _filterLevelValueService.GetFilterLevelValueProductsByFilterLevelValueIdAsync(filterLevelValue.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var productIds = filterLevelValueProducts.Select(p => p.ProductId).Distinct().ToArray();
		_logger.LogDebug("PrepareFilterLevelValueProductListModelAsync(searchModel={SearchModel},filterLevelValue={FilterLevelValue}): productIds → {ProductIds}", searchModel, filterLevelValue, productIds);
        var products = await _productService.GetProductsByIdsAsync(productIds);
		_logger.LogDebug("PrepareFilterLevelValueProductListModelAsync(searchModel={SearchModel},filterLevelValue={FilterLevelValue}): products → {Products}", searchModel, filterLevelValue, products);
        var productNames = products.ToDictionary(p => p.Id, p => p.Name);
		_logger.LogDebug("PrepareFilterLevelValueProductListModelAsync(searchModel={SearchModel},filterLevelValue={FilterLevelValue}): productNames → {ProductNames}", searchModel, filterLevelValue, productNames);

        //prepare grid model
        var model = new FilterLevelValueProductListModel().PrepareToGrid(searchModel, filterLevelValueProducts, () =>
        {
            return filterLevelValueProducts.Select(filterLevelValueProduct =>
            {
                //fill in model values from the entity
                var filterLevelValueProductModel = filterLevelValueProduct.ToModel<FilterLevelValueProductModel>();
				_logger.LogDebug("PrepareFilterLevelValueProductListModelAsync(searchModel={SearchModel},filterLevelValue={FilterLevelValue}): filterLevelValueProductModel → {FilterLevelValueProductModel}", searchModel, filterLevelValue, filterLevelValueProductModel);

                filterLevelValueProductModel.ProductName = productNames.TryGetValue(filterLevelValueProduct.ProductId, out var productName)
                    ? productName
                    : null; 

                return filterLevelValueProductModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareFilterLevelValueProductListModelAsync(searchModel={SearchModel},filterLevelValue={FilterLevelValue})", searchModel, filterLevelValue);
        return model;
    }

    /// <summary>
    /// Prepare product search model to add to the filter level value
    /// </summary>
    /// <param name="searchModel">Product search model to add to the filter level value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the filter level value
    /// </returns>
    public virtual async Task<AddProductToFilterLevelValueSearchModel> PrepareAddProductToFilterLevelValueSearchModelAsync(AddProductToFilterLevelValueSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToFilterLevelValueSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare available product types
        await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareAddProductToFilterLevelValueSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged product list model to add to the filter level value
    /// </summary>
    /// <param name="searchModel">Product search model to add to the filter level value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model to add to the filter level value
    /// </returns>
    public virtual async Task<AddProductToFilterLevelValueListModel> PrepareAddProductToFilterLevelValueListModelAsync(AddProductToFilterLevelValueSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToFilterLevelValueListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get products
        var products = await _productService.SearchProductsAsync(showHidden: true,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            storeId: searchModel.SearchStoreId,
            vendorId: searchModel.SearchVendorId,
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            keywords: searchModel.SearchProductName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AddProductToFilterLevelValueListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();
				_logger.LogDebug("PrepareAddProductToFilterLevelValueListModelAsync(searchModel={SearchModel}): productModel → {ProductModel}", searchModel, productModel);

                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareAddProductToFilterLevelValueListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }    

    #endregion
}
