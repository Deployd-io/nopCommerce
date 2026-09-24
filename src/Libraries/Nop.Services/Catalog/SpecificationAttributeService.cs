using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Security;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Specification attribute service
/// </summary>
public partial class SpecificationAttributeService : ISpecificationAttributeService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly ICategoryService _categoryService;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductCategory> _productCategoryRepository;
    protected readonly IRepository<ProductManufacturer> _productManufacturerRepository;
    protected readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
    protected readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
    protected readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
    protected readonly IRepository<SpecificationAttributeGroup> _specificationAttributeGroupRepository;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public SpecificationAttributeService(
        CatalogSettings catalogSettings,
        IAclService aclService,
        ICategoryService categoryService,
        IRepository<Product> productRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<ProductManufacturer> productManufacturerRepository,
        IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
        IRepository<SpecificationAttribute> specificationAttributeRepository,
        IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
        IRepository<SpecificationAttributeGroup> specificationAttributeGroupRepository,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStaticCacheManager staticCacheManager,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext})", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext);
        _catalogSettings = catalogSettings;
        _aclService = aclService;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _aclService → {_aclService}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _aclService);
        _categoryService = categoryService;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _categoryService → {_categoryService}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _categoryService);
        _productRepository = productRepository;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _productRepository → {_productRepository}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _productRepository);
        _productCategoryRepository = productCategoryRepository;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _productCategoryRepository → {_productCategoryRepository}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _productCategoryRepository);
        _productManufacturerRepository = productManufacturerRepository;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _productManufacturerRepository → {_productManufacturerRepository}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _productManufacturerRepository);
        _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _productSpecificationAttributeRepository → {_productSpecificationAttributeRepository}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _productSpecificationAttributeRepository);
        _specificationAttributeRepository = specificationAttributeRepository;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _specificationAttributeRepository → {_specificationAttributeRepository}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _specificationAttributeRepository);
        _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _specificationAttributeOptionRepository → {_specificationAttributeOptionRepository}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _specificationAttributeOptionRepository);
        _specificationAttributeGroupRepository = specificationAttributeGroupRepository;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _specificationAttributeGroupRepository → {_specificationAttributeGroupRepository}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _specificationAttributeGroupRepository);
        _storeContext = storeContext;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _storeContext → {_storeContext}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _storeContext);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _storeMappingService → {_storeMappingService}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _storeMappingService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _staticCacheManager);
        _workContext = workContext;
		_logger.LogDebug("SpecificationAttributeService(catalogSettings={CatalogSettings},aclService={AclService},categoryService={CategoryService},productRepository={ProductRepository},productCategoryRepository={ProductCategoryRepository},productManufacturerRepository={ProductManufacturerRepository},productSpecificationAttributeRepository={ProductSpecificationAttributeRepository},specificationAttributeRepository={SpecificationAttributeRepository},specificationAttributeOptionRepository={SpecificationAttributeOptionRepository},specificationAttributeGroupRepository={SpecificationAttributeGroupRepository},storeContext={StoreContext},storeMappingService=<redacted>,staticCacheManager={StaticCacheManager},workContext={WorkContext}): _workContext → {_workContext}", catalogSettings, aclService, categoryService, productRepository, productCategoryRepository, productManufacturerRepository, productSpecificationAttributeRepository, specificationAttributeRepository, specificationAttributeOptionRepository, specificationAttributeGroupRepository, storeContext, staticCacheManager, workContext, _workContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the available products query
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the available products query
    /// </returns>
    protected virtual async Task<IQueryable<Product>> GetAvailableProductsQueryAsync()
    {
		_logger.LogError(">>> Entering GetAvailableProductsQueryAsync()");
        var productsQuery =
            from p in _productRepository.Table
            where !p.Deleted && p.Published &&
                  (p.ParentGroupedProductId == 0 || p.VisibleIndividually) &&
                  (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc <= DateTime.UtcNow) &&
                  (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc >= DateTime.UtcNow)
            select p;

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("GetAvailableProductsQueryAsync(): store → {Store}", store);
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("GetAvailableProductsQueryAsync(): currentCustomer → {CurrentCustomer}", currentCustomer);

        //apply store mapping constraints
        productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, store.Id);
		_logger.LogError("GetAvailableProductsQueryAsync(): productsQuery → {ProductsQuery}", productsQuery);

        //apply ACL constraints
        productsQuery = await _aclService.ApplyAcl(productsQuery, currentCustomer);

		_logger.LogWarning("<<< Exiting GetAvailableProductsQueryAsync()");
        return productsQuery;
    }

    #endregion

    #region Methods

    #region Specification attribute group

    /// <summary>
    /// Gets a specification attribute group
    /// </summary>
    /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute group
    /// </returns>
    public virtual async Task<SpecificationAttributeGroup> GetSpecificationAttributeGroupByIdAsync(int specificationAttributeGroupId)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributeGroupByIdAsync(specificationAttributeGroupId={SpecificationAttributeGroupId})", specificationAttributeGroupId);
		_logger.LogError("GetSpecificationAttributeGroupByIdAsync(specificationAttributeGroupId)={SpecificationAttributeGroupId}: find query executing", specificationAttributeGroupId);
    _logger.LogWarning("<<< Exiting GetSpecificationAttributeGroupByIdAsync(specificationAttributeGroupId={SpecificationAttributeGroupId})", specificationAttributeGroupId);
        return await _specificationAttributeGroupRepository.GetByIdAsync(specificationAttributeGroupId, cache => default);
    }

    /// <summary>
    /// Gets specification attribute groups
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute groups
    /// </returns>
    public virtual async Task<IPagedList<SpecificationAttributeGroup>> GetSpecificationAttributeGroupsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributeGroupsAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
        var query = from sag in _specificationAttributeGroupRepository.Table
            orderby sag.DisplayOrder, sag.Id
            select sag;

		_logger.LogWarning("<<< Exiting GetSpecificationAttributeGroupsAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets product specification attribute groups
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute groups
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeGroup>> GetProductSpecificationAttributeGroupsAsync(int productId)
    {
		_logger.LogError(">>> Entering GetProductSpecificationAttributeGroupsAsync(productId={ProductId})", productId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var productAttributesForGroupQuery =
            from sa in _specificationAttributeRepository.Table
            join sao in _specificationAttributeOptionRepository.Table
                on sa.Id equals sao.SpecificationAttributeId
            join psa in _productSpecificationAttributeRepository.Table
                on sao.Id equals psa.SpecificationAttributeOptionId
            where psa.ProductId == productId && psa.ShowOnProductPage
            select sa.SpecificationAttributeGroupId;

        var availableGroupsQuery =
            from sag in _specificationAttributeGroupRepository.Table
            where productAttributesForGroupQuery.Any(groupId => groupId == sag.Id)
            select sag;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributeGroupByProductCacheKey, productId);
		_logger.LogError("GetProductSpecificationAttributeGroupsAsync(productId={ProductId}): key → {Key}", productId, key);
		_logger.LogError("GetProductSpecificationAttributeGroupsAsync(productId)={ProductId}: find query executing", productId);
		_logger.LogError("GetProductSpecificationAttributeGroupsAsync(productId)={ProductId}: external service call took {Arg1} ms", productId, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetProductSpecificationAttributeGroupsAsync(productId={ProductId})", productId);
        return await _staticCacheManager.GetAsync(key, async () => await availableGroupsQuery.ToListAsync());
    }

    /// <summary>
    /// Deletes a specification attribute group
    /// </summary>
    /// <param name="specificationAttributeGroup">The specification attribute group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
    {
		_logger.LogError(">>> Entering DeleteSpecificationAttributeGroupAsync(specificationAttributeGroup={SpecificationAttributeGroup})", specificationAttributeGroup);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _specificationAttributeGroupRepository.DeleteAsync(specificationAttributeGroup);
		_logger.LogError("DeleteSpecificationAttributeGroupAsync(specificationAttributeGroup)={SpecificationAttributeGroup}: external service call took {Arg1} ms", specificationAttributeGroup, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteSpecificationAttributeGroupAsync(specificationAttributeGroup={SpecificationAttributeGroup})", specificationAttributeGroup);
    }

    /// <summary>
    /// Inserts a specification attribute group
    /// </summary>
    /// <param name="specificationAttributeGroup">The specification attribute group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
    {
		_logger.LogError(">>> Entering InsertSpecificationAttributeGroupAsync(specificationAttributeGroup={SpecificationAttributeGroup})", specificationAttributeGroup);
        await _specificationAttributeGroupRepository.InsertAsync(specificationAttributeGroup);
		_logger.LogWarning("<<< Exiting InsertSpecificationAttributeGroupAsync(specificationAttributeGroup={SpecificationAttributeGroup})", specificationAttributeGroup);
    }

    /// <summary>
    /// Updates the specification attribute group
    /// </summary>
    /// <param name="specificationAttributeGroup">The specification attribute group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
    {
		_logger.LogError(">>> Entering UpdateSpecificationAttributeGroupAsync(specificationAttributeGroup={SpecificationAttributeGroup})", specificationAttributeGroup);
        await _specificationAttributeGroupRepository.UpdateAsync(specificationAttributeGroup);
		_logger.LogWarning("<<< Exiting UpdateSpecificationAttributeGroupAsync(specificationAttributeGroup={SpecificationAttributeGroup})", specificationAttributeGroup);
    }

    #endregion

    #region Specification attribute

    /// <summary>
    /// Gets all specification attributes
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes
    /// </returns>
    public virtual async Task<IPagedList<SpecificationAttribute>> GetAllSpecificationAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetAllSpecificationAttributesAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
        return await _specificationAttributeRepository.GetAllPagedAsync(query =>
        {
            return from sa in query
                   orderby sa.Name
                   select sa;
				_logger.LogError("GetAllSpecificationAttributesAsync(pageIndex,pageSize)={PageIndex},{PageSize}: find query executing", pageIndex, pageSize);
        }, pageIndex, pageSize);
		_logger.LogWarning("<<< Exiting GetAllSpecificationAttributesAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
    }

    /// <summary>
    /// Gets a specification attribute
    /// </summary>
    /// <param name="specificationAttributeId">The specification attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute
    /// </returns>
    public virtual async Task<SpecificationAttribute> GetSpecificationAttributeByIdAsync(int specificationAttributeId)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributeByIdAsync(specificationAttributeId={SpecificationAttributeId})", specificationAttributeId);
		_logger.LogError("GetSpecificationAttributeByIdAsync(specificationAttributeId)={SpecificationAttributeId}: find query executing", specificationAttributeId);
    _logger.LogWarning("<<< Exiting GetSpecificationAttributeByIdAsync(specificationAttributeId={SpecificationAttributeId})", specificationAttributeId);
        return await _specificationAttributeRepository.GetByIdAsync(specificationAttributeId, cache => default);
    }

    /// <summary>
    /// Gets specification attributes
    /// </summary>
    /// <param name="specificationAttributeIds">The specification attribute identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes
    /// </returns>
    public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributeByIdsAsync(int[] specificationAttributeIds)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributeByIdsAsync(specificationAttributeIds={SpecificationAttributeIds})", specificationAttributeIds);
        _logger.LogWarning("<<< Exiting GetSpecificationAttributeByIdsAsync(specificationAttributeIds={SpecificationAttributeIds})", specificationAttributeIds);
        return await _specificationAttributeRepository.GetByIdsAsync(specificationAttributeIds);
    }

    /// <summary>
    /// Gets specification attributes by group identifier
    /// </summary>
    /// <param name="groupId">The specification attribute group identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes
    /// </returns>
    public virtual async Task<IPagedList<SpecificationAttribute>> GetSpecificationAttributesByGroupIdAsync(
        int? groupId,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributesByGroupIdAsync(groupId={GroupId},pageIndex={PageIndex},pageSize={PageSize})", groupId, pageIndex, pageSize);
        _logger.LogWarning("<<< Exiting GetSpecificationAttributesByGroupIdAsync(groupId={GroupId},pageIndex={PageIndex},pageSize={PageSize})", groupId, pageIndex, pageSize);
        return await _specificationAttributeRepository.Table
            .Where(sa => sa.SpecificationAttributeGroupId == groupId)
            .OrderBy(sa => sa.DisplayOrder).ThenBy(sa => sa.Id)
            .ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets specification attributes by name
    /// </summary>
    /// <param name="name">The specification attribute name</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes
    /// </returns>
    public virtual async Task<IPagedList<SpecificationAttribute>> GetSpecificationAttributesByNameAsync(
        string name,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributesByNameAsync(name={Name},pageIndex={PageIndex},pageSize={PageSize})", name, pageIndex, pageSize);
        ArgumentNullException.ThrowIfNullOrEmpty(name);

		_logger.LogWarning("<<< Exiting GetSpecificationAttributesByNameAsync(name={Name},pageIndex={PageIndex},pageSize={PageSize})", name, pageIndex, pageSize);
        return await _specificationAttributeRepository.Table
            .Where(sa => sa.Name.Contains(name))
            .OrderBy(sa => sa.DisplayOrder)
            .ThenBy(sa => sa.Id)
            .ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets specification attributes that have options
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes that have available options
    /// </returns>
    public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributesWithOptionsAsync()
    {
		_logger.LogError(">>> Entering GetSpecificationAttributesWithOptionsAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = from sa in _specificationAttributeRepository.Table
            where _specificationAttributeOptionRepository.Table.Any(o => o.SpecificationAttributeId == sa.Id)
            orderby sa.DisplayOrder, sa.Id
            select sa;

		_logger.LogError("GetSpecificationAttributesWithOptionsAsync(): find query executing");
		_logger.LogError("GetSpecificationAttributesWithOptionsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetSpecificationAttributesWithOptionsAsync()");
        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributesWithOptionsCacheKey), async () => await query.ToListAsync());
    }

    /// <summary>
    /// Deletes a specification attribute
    /// </summary>
    /// <param name="specificationAttribute">The specification attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
    {
		_logger.LogError(">>> Entering DeleteSpecificationAttributeAsync(specificationAttribute={SpecificationAttribute})", specificationAttribute);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _specificationAttributeRepository.DeleteAsync(specificationAttribute);
		_logger.LogError("DeleteSpecificationAttributeAsync(specificationAttribute)={SpecificationAttribute}: external service call took {Arg1} ms", specificationAttribute, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteSpecificationAttributeAsync(specificationAttribute={SpecificationAttribute})", specificationAttribute);
    }

    /// <summary>
    /// Deletes specifications attributes
    /// </summary>
    /// <param name="specificationAttributes">Specification attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSpecificationAttributesAsync(IList<SpecificationAttribute> specificationAttributes)
    {
		_logger.LogError(">>> Entering DeleteSpecificationAttributesAsync(specificationAttributes={SpecificationAttributes})", specificationAttributes);
        ArgumentNullException.ThrowIfNull(specificationAttributes);

        foreach (var specificationAttribute in specificationAttributes)
            await DeleteSpecificationAttributeAsync(specificationAttribute);
			_logger.LogWarning("<<< Exiting DeleteSpecificationAttributesAsync(specificationAttributes={SpecificationAttributes})", specificationAttributes);
    }

    /// <summary>
    /// Inserts a specification attribute
    /// </summary>
    /// <param name="specificationAttribute">The specification attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
    {
		_logger.LogError(">>> Entering InsertSpecificationAttributeAsync(specificationAttribute={SpecificationAttribute})", specificationAttribute);
        await _specificationAttributeRepository.InsertAsync(specificationAttribute);
		_logger.LogWarning("<<< Exiting InsertSpecificationAttributeAsync(specificationAttribute={SpecificationAttribute})", specificationAttribute);
    }

    /// <summary>
    /// Updates the specification attribute
    /// </summary>
    /// <param name="specificationAttribute">The specification attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
    {
		_logger.LogError(">>> Entering UpdateSpecificationAttributeAsync(specificationAttribute={SpecificationAttribute})", specificationAttribute);
        await _specificationAttributeRepository.UpdateAsync(specificationAttribute);
		_logger.LogWarning("<<< Exiting UpdateSpecificationAttributeAsync(specificationAttribute={SpecificationAttribute})", specificationAttribute);
    }

    #endregion

    #region Specification attribute option

    /// <summary>
    /// Gets a specification attribute option
    /// </summary>
    /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute option
    /// </returns>
    public virtual async Task<SpecificationAttributeOption> GetSpecificationAttributeOptionByIdAsync(int specificationAttributeOptionId)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributeOptionByIdAsync(specificationAttributeOptionId={SpecificationAttributeOptionId})", specificationAttributeOptionId);
		_logger.LogError("GetSpecificationAttributeOptionByIdAsync(specificationAttributeOptionId)={SpecificationAttributeOptionId}: find query executing", specificationAttributeOptionId);
    _logger.LogWarning("<<< Exiting GetSpecificationAttributeOptionByIdAsync(specificationAttributeOptionId={SpecificationAttributeOptionId})", specificationAttributeOptionId);
        return await _specificationAttributeOptionRepository.GetByIdAsync(specificationAttributeOptionId, cache => default);
    }

    /// <summary>
    /// Get specification attribute options by identifiers
    /// </summary>
    /// <param name="specificationAttributeOptionIds">Identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute options
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIdsAsync(int[] specificationAttributeOptionIds)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributeOptionsByIdsAsync(specificationAttributeOptionIds={SpecificationAttributeOptionIds})", specificationAttributeOptionIds);
        _logger.LogWarning("<<< Exiting GetSpecificationAttributeOptionsByIdsAsync(specificationAttributeOptionIds={SpecificationAttributeOptionIds})", specificationAttributeOptionIds);
        return await _specificationAttributeOptionRepository.GetByIdsAsync(specificationAttributeOptionIds);
    }

    /// <summary>
    /// Gets a specification attribute option by specification attribute id
    /// </summary>
    /// <param name="specificationAttributeId">The specification attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute option
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsBySpecificationAttributeAsync(int specificationAttributeId)
    {
		_logger.LogError(">>> Entering GetSpecificationAttributeOptionsBySpecificationAttributeAsync(specificationAttributeId={SpecificationAttributeId})", specificationAttributeId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = from sao in _specificationAttributeOptionRepository.Table
            orderby sao.DisplayOrder, sao.Id
            where sao.SpecificationAttributeId == specificationAttributeId
            select sao;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("GetSpecificationAttributeOptionsBySpecificationAttributeAsync(specificationAttributeId)={SpecificationAttributeId}: external service call took {Arg1} ms", specificationAttributeId, __logStart.ElapsedMilliseconds);
		_logger.LogError("GetSpecificationAttributeOptionsBySpecificationAttributeAsync(specificationAttributeId)={SpecificationAttributeId}: find query executed in {Arg1} ms", specificationAttributeId, __logStartDb.ElapsedMilliseconds);
        var specificationAttributeOptions = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributeOptionsCacheKey, specificationAttributeId), async () => await query.ToListAsync());
		_logger.LogError("GetSpecificationAttributeOptionsBySpecificationAttributeAsync(specificationAttributeId={SpecificationAttributeId}): specificationAttributeOptions → {SpecificationAttributeOptions}", specificationAttributeId, specificationAttributeOptions);

		_logger.LogWarning("<<< Exiting GetSpecificationAttributeOptionsBySpecificationAttributeAsync(specificationAttributeId={SpecificationAttributeId})", specificationAttributeId);
        return specificationAttributeOptions;
    }

    /// <summary>
    /// Deletes a specification attribute option
    /// </summary>
    /// <param name="specificationAttributeOption">The specification attribute option</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
    {
		_logger.LogError(">>> Entering DeleteSpecificationAttributeOptionAsync(specificationAttributeOption={SpecificationAttributeOption})", specificationAttributeOption);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _specificationAttributeOptionRepository.DeleteAsync(specificationAttributeOption);
		_logger.LogError("DeleteSpecificationAttributeOptionAsync(specificationAttributeOption)={SpecificationAttributeOption}: external service call took {Arg1} ms", specificationAttributeOption, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteSpecificationAttributeOptionAsync(specificationAttributeOption={SpecificationAttributeOption})", specificationAttributeOption);
    }

    /// <summary>
    /// Inserts a specification attribute option
    /// </summary>
    /// <param name="specificationAttributeOption">The specification attribute option</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
    {
		_logger.LogError(">>> Entering InsertSpecificationAttributeOptionAsync(specificationAttributeOption={SpecificationAttributeOption})", specificationAttributeOption);
        await _specificationAttributeOptionRepository.InsertAsync(specificationAttributeOption);
		_logger.LogWarning("<<< Exiting InsertSpecificationAttributeOptionAsync(specificationAttributeOption={SpecificationAttributeOption})", specificationAttributeOption);
    }

    /// <summary>
    /// Updates the specification attribute
    /// </summary>
    /// <param name="specificationAttributeOption">The specification attribute option</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
    {
		_logger.LogError(">>> Entering UpdateSpecificationAttributeOptionAsync(specificationAttributeOption={SpecificationAttributeOption})", specificationAttributeOption);
        await _specificationAttributeOptionRepository.UpdateAsync(specificationAttributeOption);
		_logger.LogWarning("<<< Exiting UpdateSpecificationAttributeOptionAsync(specificationAttributeOption={SpecificationAttributeOption})", specificationAttributeOption);
    }

    /// <summary>
    /// Returns a list of IDs of not existing specification attribute options
    /// </summary>
    /// <param name="attributeOptionIds">The IDs of the attribute options to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of IDs not existing specification attribute options
    /// </returns>
    public virtual async Task<int[]> GetNotExistingSpecificationAttributeOptionsAsync(int[] attributeOptionIds)
    {
		_logger.LogError(">>> Entering GetNotExistingSpecificationAttributeOptionsAsync(attributeOptionIds={AttributeOptionIds})", attributeOptionIds);
        ArgumentNullException.ThrowIfNull(attributeOptionIds);

        var query = _specificationAttributeOptionRepository.Table;
		_logger.LogError("GetNotExistingSpecificationAttributeOptionsAsync(attributeOptionIds={AttributeOptionIds}): query → {Query}", attributeOptionIds, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var queryFilter = attributeOptionIds.Distinct().ToArray();
        var filter = await query.Select(a => a.Id)
            .Where(m => queryFilter.Contains(m))
            .ToListAsync();
			_logger.LogError("GetNotExistingSpecificationAttributeOptionsAsync(attributeOptionIds={AttributeOptionIds}): queryFilter → {QueryFilter}", attributeOptionIds, queryFilter);
			_logger.LogError("GetNotExistingSpecificationAttributeOptionsAsync(attributeOptionIds)={AttributeOptionIds}: database query executed in {Arg1} ms", attributeOptionIds, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetNotExistingSpecificationAttributeOptionsAsync(attributeOptionIds={AttributeOptionIds})", attributeOptionIds);
        return queryFilter.Except(filter).ToArray();
    }

    /// <summary>
    /// Gets the filtrable specification attribute options by category id
    /// </summary>
    /// <param name="categoryId">The category id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute options
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeOption>> GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(int categoryId)
    {
		_logger.LogError(">>> Entering GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(categoryId={CategoryId})", categoryId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (categoryId <= 0)
            return new List<SpecificationAttributeOption>();

        var productsQuery = await GetAvailableProductsQueryAsync();
		_logger.LogError("GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(categoryId={CategoryId}): productsQuery → {ProductsQuery}", categoryId, productsQuery);

        IList<int> subCategoryIds = null;

        if (_catalogSettings.ShowProductsFromSubcategories)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(categoryId={CategoryId}): store → {Store}", categoryId, store);
            subCategoryIds = await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id);
			_logger.LogError("GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(categoryId={CategoryId}): subCategoryIds → {SubCategoryIds}", categoryId, subCategoryIds);
        }

        var productCategoryQuery =
            from pc in _productCategoryRepository.Table
            where (pc.CategoryId == categoryId || (_catalogSettings.ShowProductsFromSubcategories && subCategoryIds.Contains(pc.CategoryId))) &&
                  (_catalogSettings.IncludeFeaturedProductsInNormalLists || !pc.IsFeaturedProduct)
            select pc;

        var result =
            from sao in _specificationAttributeOptionRepository.Table
            join psa in _productSpecificationAttributeRepository.Table on sao.Id equals psa.SpecificationAttributeOptionId
            join p in productsQuery on psa.ProductId equals p.Id
            join pc in productCategoryQuery on p.Id equals pc.ProductId
            join sa in _specificationAttributeRepository.Table on sao.SpecificationAttributeId equals sa.Id
            where psa.AllowFiltering
            orderby
                sa.DisplayOrder, sa.Name,
                sao.DisplayOrder, sao.Name
            //linq2db don't specify 'sa' in 'SELECT' statement
            //see also https://github.com/nopSolutions/nopCommerce/issues/5425
            select new { sa, sao };

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            NopCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, categoryId.ToString());

		_logger.LogError("GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(categoryId)={CategoryId}: find query executing", categoryId);
		_logger.LogError("GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(categoryId)={CategoryId}: external service call took {Arg1} ms", categoryId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(categoryId={CategoryId})", categoryId);
        return await _staticCacheManager.GetAsync(cacheKey, async () => (await result.Distinct().ToListAsync()).Select(query => query.sao).ToList());
    }

    /// <summary>
    /// Gets the filtrable specification attribute options by manufacturer id
    /// </summary>
    /// <param name="manufacturerId">The manufacturer id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute options
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeOption>> GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(int manufacturerId)
    {
		_logger.LogError(">>> Entering GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(manufacturerId={ManufacturerId})", manufacturerId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (manufacturerId <= 0)
            return new List<SpecificationAttributeOption>();

        var productsQuery = await GetAvailableProductsQueryAsync();
		_logger.LogError("GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(manufacturerId={ManufacturerId}): productsQuery → {ProductsQuery}", manufacturerId, productsQuery);

        var productManufacturerQuery =
            from pm in _productManufacturerRepository.Table
            where pm.ManufacturerId == manufacturerId &&
                  (_catalogSettings.IncludeFeaturedProductsInNormalLists || !pm.IsFeaturedProduct)
            select pm;

        var result =
            from sao in _specificationAttributeOptionRepository.Table
            join psa in _productSpecificationAttributeRepository.Table on sao.Id equals psa.SpecificationAttributeOptionId
            join p in productsQuery on psa.ProductId equals p.Id
            join pm in productManufacturerQuery on p.Id equals pm.ProductId
            join sa in _specificationAttributeRepository.Table on sao.SpecificationAttributeId equals sa.Id
            where psa.AllowFiltering
            orderby
                sa.DisplayOrder, sa.Name,
                sao.DisplayOrder, sao.Name
            //linq2db don't specify 'sa' in 'SELECT' statement
            //see also https://github.com/nopSolutions/nopCommerce/issues/5425
            select new { sa, sao };

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            NopCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, manufacturerId.ToString());

		_logger.LogError("GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(manufacturerId)={ManufacturerId}: find query executing", manufacturerId);
		_logger.LogError("GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(manufacturerId)={ManufacturerId}: external service call took {Arg1} ms", manufacturerId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(manufacturerId={ManufacturerId})", manufacturerId);
        return await _staticCacheManager.GetAsync(cacheKey, async () => (await result.Distinct().ToListAsync()).Select(query => query.sao).ToList());
    }

    #endregion

    #region Product specification attribute

    /// <summary>
    /// Deletes a product specification attribute mapping
    /// </summary>
    /// <param name="productSpecificationAttribute">Product specification attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductSpecificationAttributeAsync(ProductSpecificationAttribute productSpecificationAttribute)
    {
		_logger.LogError(">>> Entering DeleteProductSpecificationAttributeAsync(productSpecificationAttribute={ProductSpecificationAttribute})", productSpecificationAttribute);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productSpecificationAttributeRepository.DeleteAsync(productSpecificationAttribute);
		_logger.LogError("DeleteProductSpecificationAttributeAsync(productSpecificationAttribute)={ProductSpecificationAttribute}: external service call took {Arg1} ms", productSpecificationAttribute, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteProductSpecificationAttributeAsync(productSpecificationAttribute={ProductSpecificationAttribute})", productSpecificationAttribute);
    }

    /// <summary>
    /// Gets a product specification attribute mapping collection
    /// </summary>
    /// <param name="productId">Product identifier; 0 to load all records</param>
    /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
    /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
    /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 1 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
    /// <param name="specificationAttributeGroupId">Specification attribute group identifier; 0 to load all records; null to load attributes without group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product specification attribute mapping collection
    /// </returns>
    public virtual async Task<IList<ProductSpecificationAttribute>> GetProductSpecificationAttributesAsync(int productId = 0,
        int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnProductPage = null, int? specificationAttributeGroupId = 0)
    {
		_logger.LogError(">>> Entering GetProductSpecificationAttributesAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId},allowFiltering={AllowFiltering},showOnProductPage={ShowOnProductPage},specificationAttributeGroupId={SpecificationAttributeGroupId})", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
        var showOnProductPageCacheStr = showOnProductPage.HasValue ? showOnProductPage.ToString() : "null";
		_logger.LogError("GetProductSpecificationAttributesAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId},allowFiltering={AllowFiltering},showOnProductPage={ShowOnProductPage},specificationAttributeGroupId={SpecificationAttributeGroupId}): showOnProductPageCacheStr → {ShowOnProductPageCacheStr}", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId, showOnProductPageCacheStr);
        var specificationAttributeGroupIdCacheStr = specificationAttributeGroupId.HasValue ? specificationAttributeGroupId.ToString() : "null";
		_logger.LogError("GetProductSpecificationAttributesAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId},allowFiltering={AllowFiltering},showOnProductPage={ShowOnProductPage},specificationAttributeGroupId={SpecificationAttributeGroupId}): specificationAttributeGroupIdCacheStr → {SpecificationAttributeGroupIdCacheStr}", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId, specificationAttributeGroupIdCacheStr);
		_logger.LogError("GetProductSpecificationAttributesAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId},allowFiltering={AllowFiltering},showOnProductPage={ShowOnProductPage},specificationAttributeGroupId={SpecificationAttributeGroupId}): allowFilteringCacheStr → {AllowFilteringCacheStr}", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId, allowFilteringCacheStr);

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductSpecificationAttributeByProductCacheKey,
            productId, specificationAttributeOptionId, allowFilteringCacheStr, showOnProductPageCacheStr, specificationAttributeGroupIdCacheStr);

        var query = _productSpecificationAttributeRepository.Table;
		_logger.LogError("GetProductSpecificationAttributesAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId},allowFiltering={AllowFiltering},showOnProductPage={ShowOnProductPage},specificationAttributeGroupId={SpecificationAttributeGroupId}): query → {Query}", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId, query);
        if (productId > 0)
            query = query.Where(psa => psa.ProductId == productId);
        if (specificationAttributeOptionId > 0)
            query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
        if (allowFiltering.HasValue)
            query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
        if (!specificationAttributeGroupId.HasValue || specificationAttributeGroupId > 0)
        {
            query = from psa in query
                join sao in _specificationAttributeOptionRepository.Table
                    on psa.SpecificationAttributeOptionId equals sao.Id
                join sa in _specificationAttributeRepository.Table
                    on sao.SpecificationAttributeId equals sa.Id
                where sa.SpecificationAttributeGroupId == specificationAttributeGroupId
                select psa;
        }
        if (showOnProductPage.HasValue)
            query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
        query = query.OrderBy(psa => psa.DisplayOrder).ThenBy(psa => psa.Id);

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("GetProductSpecificationAttributesAsync(productId,specificationAttributeOptionId,allowFiltering,showOnProductPage,specificationAttributeGroupId)={ProductId},{SpecificationAttributeOptionId},{AllowFiltering},{ShowOnProductPage},{SpecificationAttributeGroupId}: find query executed in {Arg5} ms", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId, __logStartDb.ElapsedMilliseconds);
        var productSpecificationAttributes = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
		_logger.LogError("GetProductSpecificationAttributesAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId},allowFiltering={AllowFiltering},showOnProductPage={ShowOnProductPage},specificationAttributeGroupId={SpecificationAttributeGroupId}): productSpecificationAttributes → {ProductSpecificationAttributes}", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId, productSpecificationAttributes);
		_logger.LogError("GetProductSpecificationAttributesAsync(productId,specificationAttributeOptionId,allowFiltering,showOnProductPage,specificationAttributeGroupId)={ProductId},{SpecificationAttributeOptionId},{AllowFiltering},{ShowOnProductPage},{SpecificationAttributeGroupId}: external service call took {Arg5} ms", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetProductSpecificationAttributesAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId},allowFiltering={AllowFiltering},showOnProductPage={ShowOnProductPage},specificationAttributeGroupId={SpecificationAttributeGroupId})", productId, specificationAttributeOptionId, allowFiltering, showOnProductPage, specificationAttributeGroupId);
        return productSpecificationAttributes;
    }

    /// <summary>
    /// Gets a product specification attribute mapping 
    /// </summary>
    /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product specification attribute mapping
    /// </returns>
    public virtual async Task<ProductSpecificationAttribute> GetProductSpecificationAttributeByIdAsync(int productSpecificationAttributeId)
    {
		_logger.LogError(">>> Entering GetProductSpecificationAttributeByIdAsync(productSpecificationAttributeId={ProductSpecificationAttributeId})", productSpecificationAttributeId);
        _logger.LogWarning("<<< Exiting GetProductSpecificationAttributeByIdAsync(productSpecificationAttributeId={ProductSpecificationAttributeId})", productSpecificationAttributeId);
        return await _productSpecificationAttributeRepository.GetByIdAsync(productSpecificationAttributeId);
    }

    /// <summary>
    /// Inserts a product specification attribute mapping
    /// </summary>
    /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductSpecificationAttributeAsync(ProductSpecificationAttribute productSpecificationAttribute)
    {
		_logger.LogError(">>> Entering InsertProductSpecificationAttributeAsync(productSpecificationAttribute={ProductSpecificationAttribute})", productSpecificationAttribute);
        await _productSpecificationAttributeRepository.InsertAsync(productSpecificationAttribute);
		_logger.LogWarning("<<< Exiting InsertProductSpecificationAttributeAsync(productSpecificationAttribute={ProductSpecificationAttribute})", productSpecificationAttribute);
    }

    /// <summary>
    /// Updates the product specification attribute mapping
    /// </summary>
    /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductSpecificationAttributeAsync(ProductSpecificationAttribute productSpecificationAttribute)
    {
		_logger.LogError(">>> Entering UpdateProductSpecificationAttributeAsync(productSpecificationAttribute={ProductSpecificationAttribute})", productSpecificationAttribute);
        await _productSpecificationAttributeRepository.UpdateAsync(productSpecificationAttribute);
		_logger.LogWarning("<<< Exiting UpdateProductSpecificationAttributeAsync(productSpecificationAttribute={ProductSpecificationAttribute})", productSpecificationAttribute);
    }

    /// <summary>
    /// Gets a count of product specification attribute mapping records
    /// </summary>
    /// <param name="productId">Product identifier; 0 to load all records</param>
    /// <param name="specificationAttributeOptionId">The specification attribute option identifier; 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the count
    /// </returns>
    public virtual async Task<int> GetProductSpecificationAttributeCountAsync(int productId = 0, int specificationAttributeOptionId = 0)
    {
		_logger.LogError(">>> Entering GetProductSpecificationAttributeCountAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId})", productId, specificationAttributeOptionId);
        var query = _productSpecificationAttributeRepository.Table;
        if (productId > 0)
            query = query.Where(psa => psa.ProductId == productId);
        if (specificationAttributeOptionId > 0)
            query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

		_logger.LogWarning("<<< Exiting GetProductSpecificationAttributeCountAsync(productId={ProductId},specificationAttributeOptionId={SpecificationAttributeOptionId})", productId, specificationAttributeOptionId);
        return await query.CountAsync();
    }

    /// <summary>
    /// Get mapped products for specification attribute
    /// </summary>
    /// <param name="specificationAttributeId">The specification attribute identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IPagedList<Product>> GetProductsBySpecificationAttributeIdAsync(int specificationAttributeId, int pageIndex, int pageSize)
    {
		_logger.LogError(">>> Entering GetProductsBySpecificationAttributeIdAsync(specificationAttributeId={SpecificationAttributeId},pageIndex={PageIndex},pageSize={PageSize})", specificationAttributeId, pageIndex, pageSize);
        var query = from product in _productRepository.Table
            join psa in _productSpecificationAttributeRepository.Table on product.Id equals psa.ProductId
            join spao in _specificationAttributeOptionRepository.Table on psa.SpecificationAttributeOptionId equals spao.Id
            where spao.SpecificationAttributeId == specificationAttributeId
            orderby product.Name
            select product;

		_logger.LogWarning("<<< Exiting GetProductsBySpecificationAttributeIdAsync(specificationAttributeId={SpecificationAttributeId},pageIndex={PageIndex},pageSize={PageSize})", specificationAttributeId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion

    #endregion
}