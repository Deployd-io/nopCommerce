using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.FilterLevels;

/// <summary>
/// Filter level value service
/// </summary>
public partial class FilterLevelValueService : IFilterLevelValueService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IAclService _aclService;
    protected readonly IRepository<FilterLevelValue> _filterLevelValueRepository;
    protected readonly IRepository<FilterLevelValueProductMapping> _filterLevelValueProductMappingRepository;
    protected readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public FilterLevelValueService(FilterLevelSettings filterLevelSettings,
        IAclService aclService,
        IRepository<FilterLevelValue> filterLevelValueRepository,
        IRepository<FilterLevelValueProductMapping> filterLevelValueProductMappingRepository,
        IRepository<LocalizedProperty> localizedPropertyRepository,
        IRepository<Product> productRepository,
        IStoreMappingService storeMappingService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering FilterLevelValueService(filterLevelSettings={FilterLevelSettings},aclService={AclService},filterLevelValueRepository={FilterLevelValueRepository},filterLevelValueProductMappingRepository=<redacted>,localizedPropertyRepository={LocalizedPropertyRepository},productRepository={ProductRepository},storeMappingService=<redacted>,workContext={WorkContext})", filterLevelSettings, aclService, filterLevelValueRepository, localizedPropertyRepository, productRepository, workContext);
        _filterLevelSettings = filterLevelSettings;
        _aclService = aclService;
		_logger.LogDebug("FilterLevelValueService(filterLevelSettings={FilterLevelSettings},aclService={AclService},filterLevelValueRepository={FilterLevelValueRepository},filterLevelValueProductMappingRepository=<redacted>,localizedPropertyRepository={LocalizedPropertyRepository},productRepository={ProductRepository},storeMappingService=<redacted>,workContext={WorkContext}): _aclService → {_aclService}", filterLevelSettings, aclService, filterLevelValueRepository, localizedPropertyRepository, productRepository, workContext, _aclService);
        _filterLevelValueRepository = filterLevelValueRepository;
		_logger.LogDebug("FilterLevelValueService(filterLevelSettings={FilterLevelSettings},aclService={AclService},filterLevelValueRepository={FilterLevelValueRepository},filterLevelValueProductMappingRepository=<redacted>,localizedPropertyRepository={LocalizedPropertyRepository},productRepository={ProductRepository},storeMappingService=<redacted>,workContext={WorkContext}): _filterLevelValueRepository → {_filterLevelValueRepository}", filterLevelSettings, aclService, filterLevelValueRepository, localizedPropertyRepository, productRepository, workContext, _filterLevelValueRepository);
        _filterLevelValueProductMappingRepository = filterLevelValueProductMappingRepository;
		_logger.LogDebug("FilterLevelValueService(filterLevelSettings={FilterLevelSettings},aclService={AclService},filterLevelValueRepository={FilterLevelValueRepository},filterLevelValueProductMappingRepository=<redacted>,localizedPropertyRepository={LocalizedPropertyRepository},productRepository={ProductRepository},storeMappingService=<redacted>,workContext={WorkContext}): _filterLevelValueProductMappingRepository → {_filterLevelValueProductMappingRepository}", filterLevelSettings, aclService, filterLevelValueRepository, localizedPropertyRepository, productRepository, workContext, _filterLevelValueProductMappingRepository);
        _localizedPropertyRepository = localizedPropertyRepository;
		_logger.LogDebug("FilterLevelValueService(filterLevelSettings={FilterLevelSettings},aclService={AclService},filterLevelValueRepository={FilterLevelValueRepository},filterLevelValueProductMappingRepository=<redacted>,localizedPropertyRepository={LocalizedPropertyRepository},productRepository={ProductRepository},storeMappingService=<redacted>,workContext={WorkContext}): _localizedPropertyRepository → {_localizedPropertyRepository}", filterLevelSettings, aclService, filterLevelValueRepository, localizedPropertyRepository, productRepository, workContext, _localizedPropertyRepository);
        _productRepository = productRepository;
		_logger.LogDebug("FilterLevelValueService(filterLevelSettings={FilterLevelSettings},aclService={AclService},filterLevelValueRepository={FilterLevelValueRepository},filterLevelValueProductMappingRepository=<redacted>,localizedPropertyRepository={LocalizedPropertyRepository},productRepository={ProductRepository},storeMappingService=<redacted>,workContext={WorkContext}): _productRepository → {_productRepository}", filterLevelSettings, aclService, filterLevelValueRepository, localizedPropertyRepository, productRepository, workContext, _productRepository);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("FilterLevelValueService(filterLevelSettings={FilterLevelSettings},aclService={AclService},filterLevelValueRepository={FilterLevelValueRepository},filterLevelValueProductMappingRepository=<redacted>,localizedPropertyRepository={LocalizedPropertyRepository},productRepository={ProductRepository},storeMappingService=<redacted>,workContext={WorkContext}): _storeMappingService → {_storeMappingService}", filterLevelSettings, aclService, filterLevelValueRepository, localizedPropertyRepository, productRepository, workContext, _storeMappingService);
        _workContext = workContext;
		_logger.LogDebug("FilterLevelValueService(filterLevelSettings={FilterLevelSettings},aclService={AclService},filterLevelValueRepository={FilterLevelValueRepository},filterLevelValueProductMappingRepository=<redacted>,localizedPropertyRepository={LocalizedPropertyRepository},productRepository={ProductRepository},storeMappingService=<redacted>,workContext={WorkContext}): _workContext → {_workContext}", filterLevelSettings, aclService, filterLevelValueRepository, localizedPropertyRepository, productRepository, workContext, _workContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Check if filter levels are disabled
    /// </summary>
    /// <returns>
    /// Tuple of booleans indicating if filter levels 1, 2, and 3 are disabled
    /// </returns>
    public virtual (bool filterLevel1, bool filterLevel2, bool filterLevel3) IsFilterLevelDisabled()
    {
		_logger.LogError(">>> Entering IsFilterLevelDisabled()");
        var filterLevelEnumDisabled = _filterLevelSettings.FilterLevelEnumDisabled;

		_logger.LogWarning("<<< Exiting IsFilterLevelDisabled()");
        return (filterLevelEnumDisabled.Contains((int)FilterLevelEnum.FilterLevel1),
                filterLevelEnumDisabled.Contains((int)FilterLevelEnum.FilterLevel2),
                filterLevelEnumDisabled.Contains((int)FilterLevelEnum.FilterLevel3));
    }

    /// <summary>
    /// Gets filter level values
    /// </summary>
    /// <param name="filterLevel1Value">Filter level 1 value</param>
    /// <param name="filterLevel2Value">Filter level 2 value</param>
    /// <param name="filterLevel3Value">Filter level 3 value</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value list model
    /// </returns>
    public virtual async Task<IPagedList<FilterLevelValue>> GetAllFilterLevelValuesAsync(string filterLevel1Value = null, string filterLevel2Value = null, string filterLevel3Value = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetAllFilterLevelValuesAsync(filterLevel1Value={FilterLevel1Value},filterLevel2Value={FilterLevel2Value},filterLevel3Value={FilterLevel3Value},pageIndex={PageIndex},pageSize={PageSize})", filterLevel1Value, filterLevel2Value, filterLevel3Value, pageIndex, pageSize);
        var (filterLevel1Disabled, filterLevel2Disabled, filterLevel3Disabled) = IsFilterLevelDisabled();

		_logger.LogError("GetAllFilterLevelValuesAsync(filterLevel1Value,filterLevel2Value,filterLevel3Value,pageIndex,pageSize)={FilterLevel1Value},{FilterLevel2Value},{FilterLevel3Value},{PageIndex},{PageSize}: find query executing", filterLevel1Value, filterLevel2Value, filterLevel3Value, pageIndex, pageSize);
        return await _filterLevelValueRepository.GetAllPagedAsync(query =>
        {
            //filter by filter level 1 value
            if (!string.IsNullOrEmpty(filterLevel1Value) && filterLevel1Value != "0" && !filterLevel1Disabled)
                query = query.Where(pc => pc.FilterLevel1Value == filterLevel1Value);

            //filter by filter level 2 value
            if (!string.IsNullOrEmpty(filterLevel2Value) && filterLevel2Value != "0" && !filterLevel2Disabled)
                query = query.Where(pc => pc.FilterLevel2Value == filterLevel2Value);

            //filter by filter level 3 value
            if (!string.IsNullOrEmpty(filterLevel3Value) && filterLevel3Value != "0" && !filterLevel3Disabled)
                query = query.Where(pc => pc.FilterLevel3Value == filterLevel3Value);

            query = query
                .OrderBy(flv => flv.FilterLevel1Value)
                .ThenBy(flv => flv.FilterLevel2Value)
                .ThenBy(flv => flv.FilterLevel3Value);

		_logger.LogWarning("<<< Exiting GetAllFilterLevelValuesAsync(filterLevel1Value={FilterLevel1Value},filterLevel2Value={FilterLevel2Value},filterLevel3Value={FilterLevel3Value},pageIndex={PageIndex},pageSize={PageSize})", filterLevel1Value, filterLevel2Value, filterLevel3Value, pageIndex, pageSize);
            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Inserts Filter level value
    /// </summary>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertFilterLevelValueAsync(FilterLevelValue filterLevelValue)
    {
		_logger.LogError(">>> Entering InsertFilterLevelValueAsync(filterLevelValue={FilterLevelValue})", filterLevelValue);
        await _filterLevelValueRepository.InsertAsync(filterLevelValue);
		_logger.LogWarning("<<< Exiting InsertFilterLevelValueAsync(filterLevelValue={FilterLevelValue})", filterLevelValue);
    }

    /// <summary>
    /// Gets a filter level value
    /// </summary>
    /// <param name="filterLevelValueId">Filter level value identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value
    /// </returns>
    public virtual async Task<FilterLevelValue> GetFilterLevelValueByIdAsync(int filterLevelValueId)
    {
		_logger.LogError(">>> Entering GetFilterLevelValueByIdAsync(filterLevelValueId={FilterLevelValueId})", filterLevelValueId);
        _logger.LogWarning("<<< Exiting GetFilterLevelValueByIdAsync(filterLevelValueId={FilterLevelValueId})", filterLevelValueId);
        return await _filterLevelValueRepository.GetByIdAsync(filterLevelValueId);
    }

    /// <summary>
    /// Gets filter level values by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level values
    /// </returns>
    public virtual async Task<IList<FilterLevelValue>> GetFilterLevelValuesByProductIdAsync(int productId)
    {
		_logger.LogError(">>> Entering GetFilterLevelValuesByProductIdAsync(productId={ProductId})", productId);
        var query = from flv_map in _filterLevelValueProductMappingRepository.Table
                    join flv in _filterLevelValueRepository.Table on flv_map.FilterLevelValueId equals flv.Id
                    where flv_map.ProductId == productId
                    orderby flv.Id
                    select flv;

		_logger.LogWarning("<<< Exiting GetFilterLevelValuesByProductIdAsync(productId={ProductId})", productId);
        return await  query.ToListAsync();
    }

    /// <summary>
    /// Gets filter level values by identifier
    /// </summary>
    /// <param name="filterLevelValueIds">Filter level value identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level values
    /// </returns>
    public virtual async Task<IList<FilterLevelValue>> GetFilterLevelValuesByIdsAsync(int[] filterLevelValueIds)
    {
		_logger.LogError(">>> Entering GetFilterLevelValuesByIdsAsync(filterLevelValueIds={FilterLevelValueIds})", filterLevelValueIds);
        _logger.LogWarning("<<< Exiting GetFilterLevelValuesByIdsAsync(filterLevelValueIds={FilterLevelValueIds})", filterLevelValueIds);
        return await _filterLevelValueRepository.GetByIdsAsync(filterLevelValueIds, includeDeleted: false);
    }

    /// <summary>
    /// Updates the filter level value
    /// </summary>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateFilterLevelValueAsync(FilterLevelValue filterLevelValue)
    {
		_logger.LogError(">>> Entering UpdateFilterLevelValueAsync(filterLevelValue={FilterLevelValue})", filterLevelValue);
        ArgumentNullException.ThrowIfNull(filterLevelValue);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("UpdateFilterLevelValueAsync(filterLevelValue)={FilterLevelValue}: update query executed in {Arg1} ms", filterLevelValue, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting UpdateFilterLevelValueAsync(filterLevelValue={FilterLevelValue})", filterLevelValue);
        await _filterLevelValueRepository.UpdateAsync(filterLevelValue);
    }

    /// <summary>
    /// Delete filter level value
    /// </summary>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteFilterLevelValueAsync(FilterLevelValue filterLevelValue)
    {
		_logger.LogError(">>> Entering DeleteFilterLevelValueAsync(filterLevelValue={FilterLevelValue})", filterLevelValue);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _filterLevelValueRepository.DeleteAsync(filterLevelValue);
		_logger.LogError("DeleteFilterLevelValueAsync(filterLevelValue)={FilterLevelValue}: external service call took {Arg1} ms", filterLevelValue, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteFilterLevelValueAsync(filterLevelValue={FilterLevelValue})", filterLevelValue);
    }

    /// <summary>
    /// Delete filter level values
    /// </summary>
    /// <param name="filterLevelValues">Filter level values</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteFilterLevelValuesAsync(IList<FilterLevelValue> filterLevelValues)
    {
		_logger.LogError(">>> Entering DeleteFilterLevelValuesAsync(filterLevelValues={FilterLevelValues})", filterLevelValues);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(filterLevelValues);

		_logger.LogError("DeleteFilterLevelValuesAsync(filterLevelValues)={FilterLevelValues}: external service call took {Arg1} ms", filterLevelValues, __logStart.ElapsedMilliseconds);
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
    _logger.LogWarning("<<< Exiting DeleteFilterLevelValuesAsync(filterLevelValues={FilterLevelValues})", filterLevelValues);
        await _filterLevelValueRepository.DeleteAsync(filterLevelValues);
    }

    #region Mapping

    /// <summary>
    /// Gets products collection by filter level value identifier
    /// </summary>
    /// <param name="filterLevelValueId">Filter level value identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="storeId">Store identifier; 0 to load all records</param>
    /// <param name="orderBy">Order by</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products collection
    /// </returns>
    public virtual async Task<IPagedList<Product>> GetProductsByFilterLevelValueIdAsync(int filterLevelValueId,
        int pageIndex = 0, 
        int pageSize = int.MaxValue,
        int storeId = 0,
        ProductSortingEnum orderBy = ProductSortingEnum.Position)
    {
		_logger.LogError(">>> Entering GetProductsByFilterLevelValueIdAsync(filterLevelValueId={FilterLevelValueId},pageIndex={PageIndex},pageSize={PageSize},storeId={StoreId},orderBy={OrderBy})", filterLevelValueId, pageIndex, pageSize, storeId, orderBy);
        if (filterLevelValueId == 0)
            return new PagedList<Product>(new List<Product>(), pageIndex, pageSize);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("GetProductsByFilterLevelValueIdAsync(filterLevelValueId={FilterLevelValueId},pageIndex={PageIndex},pageSize={PageSize},storeId={StoreId},orderBy={OrderBy}): customer → {Customer}", filterLevelValueId, pageIndex, pageSize, storeId, orderBy, customer);

        var query = from pc in _filterLevelValueProductMappingRepository.Table
                    join p in _productRepository.Table on pc.ProductId equals p.Id
                    where pc.FilterLevelValueId == filterLevelValueId && !p.Deleted && p.Published && p.VisibleIndividually
                    orderby pc.Id
                    select p;

        //apply store mapping constraints
        query = await _storeMappingService.ApplyStoreMapping(query, storeId);
		_logger.LogError("GetProductsByFilterLevelValueIdAsync(filterLevelValueId={FilterLevelValueId},pageIndex={PageIndex},pageSize={PageSize},storeId={StoreId},orderBy={OrderBy}): query → {Query}", filterLevelValueId, pageIndex, pageSize, storeId, orderBy, query);

        //apply ACL constraints
        query = await _aclService.ApplyAcl(query, customer);

		_logger.LogWarning("<<< Exiting GetProductsByFilterLevelValueIdAsync(filterLevelValueId={FilterLevelValueId},pageIndex={PageIndex},pageSize={PageSize},storeId={StoreId},orderBy={OrderBy})", filterLevelValueId, pageIndex, pageSize, storeId, orderBy);
        return await query.OrderBy(_localizedPropertyRepository, await _workContext.GetWorkingLanguageAsync(), orderBy).ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets filter level value product mapping collection
    /// </summary>
    /// <param name="filterLevelValueId">Filter level value identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value product mapping collection
    /// </returns>
    public virtual async Task<IPagedList<FilterLevelValueProductMapping>> GetFilterLevelValueProductsByFilterLevelValueIdAsync(int filterLevelValueId,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetFilterLevelValueProductsByFilterLevelValueIdAsync(filterLevelValueId={FilterLevelValueId},pageIndex={PageIndex},pageSize={PageSize})", filterLevelValueId, pageIndex, pageSize);
        if (filterLevelValueId == 0)
            return new PagedList<FilterLevelValueProductMapping>(new List<FilterLevelValueProductMapping>(), pageIndex, pageSize);

        var query = from pc in _filterLevelValueProductMappingRepository.Table
                    join p in _productRepository.Table on pc.ProductId equals p.Id
                    where pc.FilterLevelValueId == filterLevelValueId && !p.Deleted
                    orderby pc.Id
                    select pc;

		_logger.LogWarning("<<< Exiting GetFilterLevelValueProductsByFilterLevelValueIdAsync(filterLevelValueId={FilterLevelValueId},pageIndex={PageIndex},pageSize={PageSize})", filterLevelValueId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets a filter level value product mapping
    /// </summary>
    /// <param name="filterLevelValueProductId">Filter level value product mapping identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value product mapping
    /// </returns>
    public virtual async Task<FilterLevelValueProductMapping> GetFilterLevelValueProductByIdAsync(int filterLevelValueProductId)
    {
		_logger.LogError(">>> Entering GetFilterLevelValueProductByIdAsync(filterLevelValueProductId={FilterLevelValueProductId})", filterLevelValueProductId);
        _logger.LogWarning("<<< Exiting GetFilterLevelValueProductByIdAsync(filterLevelValueProductId={FilterLevelValueProductId})", filterLevelValueProductId);
        return await _filterLevelValueProductMappingRepository.GetByIdAsync(filterLevelValueProductId);
    }

    /// <summary>
    /// Deletes a filter level value product mapping
    /// </summary>
    /// <param name="filterLevelValueProduct">Filter level value product mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteFilterLevelValueProductAsync(FilterLevelValueProductMapping filterLevelValueProduct)
    {
		_logger.LogError(">>> Entering DeleteFilterLevelValueProductAsync(filterLevelValueProduct={FilterLevelValueProduct})", filterLevelValueProduct);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _filterLevelValueProductMappingRepository.DeleteAsync(filterLevelValueProduct);
		_logger.LogError("DeleteFilterLevelValueProductAsync(filterLevelValueProduct)={FilterLevelValueProduct}: external service call took {Arg1} ms", filterLevelValueProduct, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteFilterLevelValueProductAsync(filterLevelValueProduct={FilterLevelValueProduct})", filterLevelValueProduct);
    }

    /// <summary>
    /// Inserts a filter level value product mapping
    /// </summary>
    /// <param name="filterLevelValueProduct">Filter level value product mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductFilterLevelValueAsync(FilterLevelValueProductMapping filterLevelValueProduct)
    {
		_logger.LogError(">>> Entering InsertProductFilterLevelValueAsync(filterLevelValueProduct={FilterLevelValueProduct})", filterLevelValueProduct);
        await _filterLevelValueProductMappingRepository.InsertAsync(filterLevelValueProduct);
		_logger.LogWarning("<<< Exiting InsertProductFilterLevelValueAsync(filterLevelValueProduct={FilterLevelValueProduct})", filterLevelValueProduct);
    }

    #endregion

    #endregion
}
