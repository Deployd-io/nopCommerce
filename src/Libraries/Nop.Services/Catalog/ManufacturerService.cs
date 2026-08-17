using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog;

/// <summary>
/// Manufacturer service
/// </summary>
public partial class ManufacturerService : IManufacturerService
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerService _customerService;
    protected readonly IRepository<DiscountManufacturerMapping> _discountManufacturerMappingRepository;
    protected readonly IRepository<Manufacturer> _manufacturerRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductManufacturer> _productManufacturerRepository;
    protected readonly IRepository<ProductCategory> _productCategoryRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ManufacturerService(CatalogSettings catalogSettings,
        IAclService aclService,
        ICategoryService categoryService,
        ICustomerService customerService,
        IRepository<DiscountManufacturerMapping> discountManufacturerMappingRepository,
        IRepository<Manufacturer> manufacturerRepository,
        IRepository<Product> productRepository,
        IRepository<ProductManufacturer> productManufacturerRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IWorkContext workContext)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _categoryService = categoryService;
        _customerService = customerService;
        _discountManufacturerMappingRepository = discountManufacturerMappingRepository;
        _manufacturerRepository = manufacturerRepository;
        _productRepository = productRepository;
        _productManufacturerRepository = productManufacturerRepository;
        _productCategoryRepository = productCategoryRepository;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Clean up manufacturer references for a specified discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearDiscountManufacturerMappingAsync(Discount discount)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug(">>> Entering ClearDiscountManufacturerMappingAsync(discount={Discount})", discount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(discount);

        var mappings = _discountManufacturerMappingRepository.Table.Where(dcm => dcm.DiscountId == discount.Id);
		_logger.LogDebug("ClearDiscountManufacturerMappingAsync(discount={Discount}): mappings → {Mappings}", discount, mappings);

        await _discountManufacturerMappingRepository.DeleteAsync(await mappings.ToListAsync());
		_logger.LogDebug("<<< Exiting ClearDiscountManufacturerMappingAsync(discount={Discount})", discount);
		_logger.LogInformation("ClearDiscountManufacturerMappingAsync(discount)={Discount}: external service call took {Arg1} ms", discount, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Deletes a manufacturer
    /// </summary>
    /// <param name="manufacturer">Manufacturer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteManufacturerAsync(Manufacturer manufacturer)
    {
		_logger.LogDebug(">>> Entering DeleteManufacturerAsync(manufacturer={Manufacturer})", manufacturer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _manufacturerRepository.DeleteAsync(manufacturer);
		_logger.LogDebug("<<< Exiting DeleteManufacturerAsync(manufacturer={Manufacturer})", manufacturer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteManufacturerAsync(manufacturer)={Manufacturer}: external service call took {Arg1} ms", manufacturer, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteManufacturerAsync(manufacturer)={Manufacturer}: external service call took {Arg1} ms", manufacturer, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Delete manufacturers
    /// </summary>
    /// <param name="manufacturers">Manufacturers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteManufacturersAsync(IList<Manufacturer> manufacturers)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _manufacturerRepository.DeleteAsync(manufacturers);
    }

    /// <summary>
    /// Gets all manufacturers
    /// </summary>
    /// <param name="manufacturerName">Manufacturer name</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="overridePublished">
    /// null - process "Published" property according to "showHidden" parameter
    /// true - load only "Published" products
    /// false - load only "Unpublished" products
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturers
    /// </returns>
    public virtual async Task<IPagedList<Manufacturer>> GetAllManufacturersAsync(string manufacturerName = "",
        int storeId = 0,
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        bool showHidden = false,
        bool? overridePublished = null)
    {
		_logger.LogDebug(">>> Entering GetAllManufacturersAsync(manufacturerName={ManufacturerName},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden},overridePublished={OverridePublished})", manufacturerName, storeId, pageIndex, pageSize, showHidden, overridePublished);
        return await _manufacturerRepository.GetAllPagedAsync(async query =>
        {
		_logger.LogDebug("<<< Exiting GetAllManufacturersAsync(manufacturerName={ManufacturerName},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden},overridePublished={OverridePublished})", manufacturerName, storeId, pageIndex, pageSize, showHidden, overridePublished);
		_logger.LogDebug("GetAllManufacturersAsync(manufacturerName={ManufacturerName},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden},overridePublished={OverridePublished}): query → {Query}", manufacturerName, storeId, pageIndex, pageSize, showHidden, overridePublished, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
            if (!showHidden)
                query = query.Where(m => m.Published);
				_logger.LogInformation("GetAllManufacturersAsync(manufacturerName,storeId,pageIndex,pageSize,showHidden,overridePublished)={ManufacturerName},{StoreId},{PageIndex},{PageSize},{ShowHidden},{OverridePublished}: find query executed in {Arg6} ms", manufacturerName, storeId, pageIndex, pageSize, showHidden, overridePublished, __logStart.ElapsedMilliseconds);
            else if (overridePublished.HasValue)
                query = query.Where(m => m.Published == overridePublished.Value);

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            if (!showHidden)
            {
                //apply ACL constraints
                var customer = await _workContext.GetCurrentCustomerAsync();
                query = await _aclService.ApplyAcl(query, customer);
            }

            query = query.Where(m => !m.Deleted);

            if (!string.IsNullOrWhiteSpace(manufacturerName))
                query = query.Where(m => m.Name.Contains(manufacturerName));

            return query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Get manufacturer identifiers to which a discount is applied
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer identifiers
    /// </returns>
    public virtual async Task<IList<int>> GetAppliedManufacturerIdsAsync(Discount discount, Customer customer)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(discount);

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopDiscountDefaults.ManufacturerIdsByDiscountCacheKey,
            discount,
            await _customerService.GetCustomerRoleIdsAsync(customer),
            await _storeContext.GetCurrentStoreAsync());

        var query = _discountManufacturerMappingRepository.Table.Where(dmm => dmm.DiscountId == discount.Id)
            .Select(dmm => dmm.EntityId);

        var result = await _staticCacheManager.GetAsync(cacheKey, async () => await query.ToListAsync());

        return result;
    }

    /// <summary>
    /// Gets a manufacturer
    /// </summary>
    /// <param name="manufacturerId">Manufacturer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer
    /// </returns>
    public virtual async Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId)
    {
		_logger.LogDebug(">>> Entering GetManufacturerByIdAsync(manufacturerId={ManufacturerId})", manufacturerId);
		_logger.LogDebug("<<< Exiting GetManufacturerByIdAsync(manufacturerId={ManufacturerId})", manufacturerId);
        return await _manufacturerRepository.GetByIdAsync(manufacturerId, cache => default);
    }

    /// <summary>
    /// Get manufacturers for which a discount is applied
    /// </summary>
    /// <param name="discountId">Discount identifier; pass null to load all records</param>
    /// <param name="showHidden">A value indicating whether to load deleted manufacturers</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of manufacturers
    /// </returns>
    public virtual async Task<IPagedList<Manufacturer>> GetManufacturersWithAppliedDiscountAsync(int? discountId = null,
        bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetManufacturersWithAppliedDiscountAsync(discountId={DiscountId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", discountId, showHidden, pageIndex, pageSize);
        var manufacturers = _manufacturerRepository.Table;

		_logger.LogDebug("GetManufacturersWithAppliedDiscountAsync(discountId={DiscountId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize}): manufacturers → {Manufacturers}", discountId, showHidden, pageIndex, pageSize, manufacturers);
        if (discountId.HasValue)
            manufacturers = from manufacturer in manufacturers
                join dmm in _discountManufacturerMappingRepository.Table on manufacturer.Id equals dmm.EntityId
                where dmm.DiscountId == discountId.Value
                select manufacturer;

        if (!showHidden)
            manufacturers = manufacturers.Where(manufacturer => !manufacturer.Deleted);

        manufacturers = manufacturers.OrderBy(manufacturer => manufacturer.DisplayOrder).ThenBy(manufacturer => manufacturer.Id);

		_logger.LogDebug("<<< Exiting GetManufacturersWithAppliedDiscountAsync(discountId={DiscountId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", discountId, showHidden, pageIndex, pageSize);
        return await manufacturers.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets the manufacturers by category identifier
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturers
    /// </returns>
    public virtual async Task<IList<Manufacturer>> GetManufacturersByCategoryIdAsync(int categoryId)
    {
		_logger.LogDebug(">>> Entering GetManufacturersByCategoryIdAsync(categoryId={CategoryId})", categoryId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (categoryId <= 0)
            return new List<Manufacturer>();

        // get available products in category
        var productsQuery =
            from p in _productRepository.Table
			_logger.LogDebug("GetManufacturersByCategoryIdAsync(categoryId={CategoryId}): productsQuery → {ProductsQuery}", categoryId, productsQuery);
            where !p.Deleted && p.Published &&
                  (p.ParentGroupedProductId == 0 || p.VisibleIndividually) &&
                  (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc <= DateTime.UtcNow) &&
                  (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc >= DateTime.UtcNow)
            select p;

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("GetManufacturersByCategoryIdAsync(categoryId={CategoryId}): store → {Store}", categoryId, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetManufacturersByCategoryIdAsync(categoryId={CategoryId}): customer → {Customer}", categoryId, customer);
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		_logger.LogDebug("GetManufacturersByCategoryIdAsync(categoryId={CategoryId}): customerRoleIds → {CustomerRoleIds}", categoryId, customerRoleIds);

        //apply store mapping constraints
        productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, store.Id);

        //apply ACL constraints
        productsQuery = await _aclService.ApplyAcl(productsQuery, customerRoleIds);

        var subCategoryIds = _catalogSettings.ShowProductsFromSubcategories
            ? await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id)
            : null;

        var productCategoryQuery =
            from pc in _productCategoryRepository.Table
            where (pc.CategoryId == categoryId || (_catalogSettings.ShowProductsFromSubcategories && subCategoryIds.Contains(pc.CategoryId))) &&
                  (_catalogSettings.IncludeFeaturedProductsInNormalLists || !pc.IsFeaturedProduct)
            select pc;

        var manufacturerQuery = from m in _manufacturerRepository.Table
			_logger.LogDebug("GetManufacturersByCategoryIdAsync(categoryId={CategoryId}): manufacturerQuery → {ManufacturerQuery}", categoryId, manufacturerQuery);
            where !m.Deleted && m.Published
            select m;

        //apply store mapping constraints
        manufacturerQuery = await _storeMappingService.ApplyStoreMapping(manufacturerQuery, store.Id);

        //apply ACL constraints
        manufacturerQuery = await _aclService.ApplyAcl(manufacturerQuery, customerRoleIds);

        // get manufacturers of the products
        var manufacturersQuery =
            from m in manufacturerQuery
			using Microsoft.Extensions.Logging;
            join pm in _productManufacturerRepository.Table on m.Id equals pm.ManufacturerId
            join p in productsQuery on pm.ProductId equals p.Id
            join pc in productCategoryQuery on p.Id equals pc.ProductId
            orderby
                m.DisplayOrder, m.Name
            select m;

        var key = _staticCacheManager
            .PrepareKeyForDefaultCache(NopCatalogDefaults.ManufacturersByCategoryCacheKey, categoryId, store, customerRoleIds);

		_logger.LogInformation("GetManufacturersByCategoryIdAsync(categoryId)={CategoryId}: external service call took {Arg1} ms", categoryId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetManufacturersByCategoryIdAsync(categoryId={CategoryId})", categoryId);
        return await _staticCacheManager.GetAsync(key, async () => await manufacturersQuery.Distinct().ToListAsync());
    }

    /// <summary>
    /// Gets manufacturers by identifier
    /// </summary>
    /// <param name="manufacturerIds">manufacturer identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturers
    /// </returns>
    public virtual async Task<IList<Manufacturer>> GetManufacturersByIdsAsync(int[] manufacturerIds)
    {
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
		_logger.LogDebug(">>> Entering GetManufacturersByIdsAsync(manufacturerIds={ManufacturerIds})", manufacturerIds);
		_logger.LogDebug("<<< Exiting GetManufacturersByIdsAsync(manufacturerIds={ManufacturerIds})", manufacturerIds);
        return await _manufacturerRepository.GetByIdsAsync(manufacturerIds, includeDeleted: false);
    }

    /// <summary>
    /// Inserts a manufacturer
    /// </summary>
    /// <param name="manufacturer">Manufacturer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertManufacturerAsync(Manufacturer manufacturer)
    {
		_logger.LogDebug(">>> Entering InsertManufacturerAsync(manufacturer={Manufacturer})", manufacturer);
        await _manufacturerRepository.InsertAsync(manufacturer);
		_logger.LogDebug("<<< Exiting InsertManufacturerAsync(manufacturer={Manufacturer})", manufacturer);
    }

    /// <summary>
    /// Updates the manufacturer
    /// </summary>
    /// <param name="manufacturer">Manufacturer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateManufacturerAsync(Manufacturer manufacturer)
    {
		_logger.LogDebug(">>> Entering UpdateManufacturerAsync(manufacturer={Manufacturer})", manufacturer);
        await _manufacturerRepository.UpdateAsync(manufacturer);
		_logger.LogDebug("<<< Exiting UpdateManufacturerAsync(manufacturer={Manufacturer})", manufacturer);
    }

    /// <summary>
    /// Deletes a product manufacturer mapping
    /// </summary>
    /// <param name="productManufacturer">Product manufacturer mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductManufacturerAsync(ProductManufacturer productManufacturer)
    {
		_logger.LogDebug(">>> Entering DeleteProductManufacturerAsync(productManufacturer={ProductManufacturer})", productManufacturer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productManufacturerRepository.DeleteAsync(productManufacturer);
		_logger.LogDebug("<<< Exiting DeleteProductManufacturerAsync(productManufacturer={ProductManufacturer})", productManufacturer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteProductManufacturerAsync(productManufacturer)={ProductManufacturer}: external service call took {Arg1} ms", productManufacturer, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteProductManufacturerAsync(productManufacturer)={ProductManufacturer}: external service call took {Arg1} ms", productManufacturer, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deletes a list of product manufacturer mapping
    /// </summary>
    /// <param name="productManufacturers">Product manufacturer mappings</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductManufacturersAsync(IList<ProductManufacturer> productManufacturers)
    {
		_logger.LogDebug(">>> Entering DeleteProductManufacturersAsync(productManufacturers={ProductManufacturers})", productManufacturers);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productManufacturerRepository.DeleteAsync(productManufacturers);
		_logger.LogDebug("<<< Exiting DeleteProductManufacturersAsync(productManufacturers={ProductManufacturers})", productManufacturers);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteProductManufacturersAsync(productManufacturers)={ProductManufacturers}: external service call took {Arg1} ms", productManufacturers, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteProductManufacturersAsync(productManufacturers)={ProductManufacturers}: external service call took {Arg1} ms", productManufacturers, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets product manufacturer collection
    /// </summary>
    /// <param name="manufacturerId">Manufacturer identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product manufacturer collection
    /// </returns>
    public virtual async Task<IPagedList<ProductManufacturer>> GetProductManufacturersByManufacturerIdAsync(int manufacturerId,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetProductManufacturersByManufacturerIdAsync(manufacturerId={ManufacturerId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", manufacturerId, pageIndex, pageSize, showHidden);
        if (manufacturerId == 0)
            return new PagedList<ProductManufacturer>(new List<ProductManufacturer>(), pageIndex, pageSize);

        var query = from pm in _productManufacturerRepository.Table
			_logger.LogDebug("GetProductManufacturersByManufacturerIdAsync(manufacturerId={ManufacturerId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): query → {Query}", manufacturerId, pageIndex, pageSize, showHidden, query);
            join p in _productRepository.Table on pm.ProductId equals p.Id
            where pm.ManufacturerId == manufacturerId && !p.Deleted
            orderby pm.DisplayOrder, pm.Id
            select pm;

        if (!showHidden)
        {
            var manufacturersQuery = _manufacturerRepository.Table.Where(m => m.Published);
			_logger.LogDebug("GetProductManufacturersByManufacturerIdAsync(manufacturerId={ManufacturerId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): manufacturersQuery → {ManufacturersQuery}", manufacturerId, pageIndex, pageSize, showHidden, manufacturersQuery);

            //apply store mapping constraints
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("GetProductManufacturersByManufacturerIdAsync(manufacturerId={ManufacturerId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): store → {Store}", manufacturerId, pageIndex, pageSize, showHidden, store);
            manufacturersQuery = await _storeMappingService.ApplyStoreMapping(manufacturersQuery, store.Id);

            //apply ACL constraints
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogDebug("GetProductManufacturersByManufacturerIdAsync(manufacturerId={ManufacturerId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): customer → {Customer}", manufacturerId, pageIndex, pageSize, showHidden, customer);
            manufacturersQuery = await _aclService.ApplyAcl(manufacturersQuery, customer);

            query = query.Where(pm => manufacturersQuery.Any(m => m.Id == pm.ManufacturerId));
        }

		_logger.LogDebug("<<< Exiting GetProductManufacturersByManufacturerIdAsync(manufacturerId={ManufacturerId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", manufacturerId, pageIndex, pageSize, showHidden);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets a product manufacturer mapping collection
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product manufacturer mapping collection
    /// </returns>
    public virtual async Task<IList<ProductManufacturer>> GetProductManufacturersByProductIdAsync(int productId,
        bool showHidden = false)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (productId == 0)
            return new List<ProductManufacturer>();

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("GetProductManufacturersByProductIdAsync(productId={ProductId},showHidden={ShowHidden}): store → {Store}", productId, showHidden, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetProductManufacturersByProductIdAsync(productId={ProductId},showHidden={ShowHidden}): customer → {Customer}", productId, showHidden, customer);
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		_logger.LogDebug("GetProductManufacturersByProductIdAsync(productId={ProductId},showHidden={ShowHidden}): customerRoleIds → {CustomerRoleIds}", productId, showHidden, customerRoleIds);

        var key = _staticCacheManager
            .PrepareKeyForDefaultCache(NopCatalogDefaults.ProductManufacturersByProductCacheKey, productId, showHidden, customerRoleIds, store);

        var query = from pm in _productManufacturerRepository.Table
			_logger.LogDebug("GetProductManufacturersByProductIdAsync(productId={ProductId},showHidden={ShowHidden}): query → {Query}", productId, showHidden, query);
            join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
            where pm.ProductId == productId && !m.Deleted
            orderby pm.DisplayOrder, pm.Id
            select pm;

        if (!showHidden)
        {
            var manufacturersQuery = _manufacturerRepository.Table.Where(m => m.Published);
			_logger.LogDebug("GetProductManufacturersByProductIdAsync(productId={ProductId},showHidden={ShowHidden}): manufacturersQuery → {ManufacturersQuery}", productId, showHidden, manufacturersQuery);

            //apply store mapping constraints
            manufacturersQuery = await _storeMappingService.ApplyStoreMapping(manufacturersQuery, store.Id);

            //apply ACL constraints
            manufacturersQuery = await _aclService.ApplyAcl(manufacturersQuery, customerRoleIds);

            query = query.Where(pm => manufacturersQuery.Any(m => m.Id == pm.ManufacturerId));
        }

		_logger.LogInformation("GetProductManufacturersByProductIdAsync(productId,showHidden)={ProductId},{ShowHidden}: external service call took {Arg2} ms", productId, showHidden, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetProductManufacturersByProductIdAsync(productId={ProductId},showHidden={ShowHidden})", productId, showHidden);
        return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
    }

    /// <summary>
    /// Gets a product manufacturer mapping 
    /// </summary>
    /// <param name="productManufacturerId">Product manufacturer mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product manufacturer mapping
    /// </returns>
    public virtual async Task<ProductManufacturer> GetProductManufacturerByIdAsync(int productManufacturerId)
    {
		_logger.LogDebug(">>> Entering GetProductManufacturerByIdAsync(productManufacturerId={ProductManufacturerId})", productManufacturerId);
		_logger.LogDebug("<<< Exiting GetProductManufacturerByIdAsync(productManufacturerId={ProductManufacturerId})", productManufacturerId);
        return await _productManufacturerRepository.GetByIdAsync(productManufacturerId, cache => default);
    }

    /// <summary>
    /// Inserts a product manufacturer mapping
    /// </summary>
    /// <param name="productManufacturer">Product manufacturer mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductManufacturerAsync(ProductManufacturer productManufacturer)
    {
		_logger.LogDebug(">>> Entering InsertProductManufacturerAsync(productManufacturer={ProductManufacturer})", productManufacturer);
        await _productManufacturerRepository.InsertAsync(productManufacturer);
		_logger.LogDebug("<<< Exiting InsertProductManufacturerAsync(productManufacturer={ProductManufacturer})", productManufacturer);
    }

    /// <summary>
    /// Updates the product manufacturer mapping
    /// </summary>
    /// <param name="productManufacturer">Product manufacturer mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductManufacturerAsync(ProductManufacturer productManufacturer)
    {
		_logger.LogDebug(">>> Entering UpdateProductManufacturerAsync(productManufacturer={ProductManufacturer})", productManufacturer);
        await _productManufacturerRepository.UpdateAsync(productManufacturer);
		_logger.LogDebug("<<< Exiting UpdateProductManufacturerAsync(productManufacturer={ProductManufacturer})", productManufacturer);
    }

    /// <summary>
    /// Get manufacturer IDs for products
    /// </summary>
    /// <param name="productIds">Products IDs</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer IDs for products
    /// </returns>
    public virtual async Task<IDictionary<int, int[]>> GetProductManufacturerIdsAsync(int[] productIds)
    {
        var query = _productManufacturerRepository.Table;

        return (await query.Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.ManufacturerId })
                .ToListAsync())
            .GroupBy(a => a.ProductId)
            .ToDictionary(items => items.Key, items => items.Select(a => a.ManufacturerId).ToArray());
    }

    /// <summary>
    /// Returns a list of names of not existing manufacturers
    /// </summary>
    /// <param name="manufacturerIdsNames">The names and/or IDs of the manufacturers to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of names and/or IDs not existing manufacturers
    /// </returns>
    public virtual async Task<string[]> GetNotExistingManufacturersAsync(string[] manufacturerIdsNames)
    {
		_logger.LogDebug(">>> Entering GetNotExistingManufacturersAsync(manufacturerIdsNames={ManufacturerIdsNames})", manufacturerIdsNames);
        ArgumentNullException.ThrowIfNull(manufacturerIdsNames);

        var query = _manufacturerRepository.Table;//.Where(m => !m.Deleted);
		_logger.LogDebug("GetNotExistingManufacturersAsync(manufacturerIdsNames={ManufacturerIdsNames}): query → {Query}", manufacturerIdsNames, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var queryFilter = manufacturerIdsNames.Distinct().ToArray();
		_logger.LogDebug("GetNotExistingManufacturersAsync(manufacturerIdsNames={ManufacturerIdsNames}): queryFilter → {QueryFilter}", manufacturerIdsNames, queryFilter);
        //filtering by name
        var filter = await query.Select(m => m.Name).Where(m => queryFilter.Contains(m)).ToListAsync();
        queryFilter = queryFilter.Except(filter).ToArray();
		_logger.LogDebug("GetNotExistingManufacturersAsync(manufacturerIdsNames={ManufacturerIdsNames}): filter → {Filter}", manufacturerIdsNames, filter);
		_logger.LogInformation("GetNotExistingManufacturersAsync(manufacturerIdsNames)={ManufacturerIdsNames}: find query executed in {Arg1} ms", manufacturerIdsNames, __logStart.ElapsedMilliseconds);

        //if some names not found
        if (!queryFilter.Any())
            return queryFilter.ToArray();

        //filtering by IDs
        filter = await query.Select(c => c.Id.ToString())
            .Where(c => queryFilter.Contains(c))
            .ToListAsync();

		_logger.LogDebug("<<< Exiting GetNotExistingManufacturersAsync(manufacturerIdsNames={ManufacturerIdsNames})", manufacturerIdsNames);
        return queryFilter.Except(filter).ToArray();
    }

    /// <summary>
    /// Returns a ProductManufacturer that has the specified values
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="productId">Product identifier</param>
    /// <param name="manufacturerId">Manufacturer identifier</param>
    /// <returns>A ProductManufacturer that has the specified values; otherwise null</returns>
    public virtual ProductManufacturer FindProductManufacturer(IList<ProductManufacturer> source, int productId, int manufacturerId)
    {
		_logger.LogDebug(">>> Entering FindProductManufacturer(source={Source},productId={ProductId},manufacturerId={ManufacturerId})", source, productId, manufacturerId);
		_logger.LogDebug("<<< Exiting FindProductManufacturer(source={Source},productId={ProductId},manufacturerId={ManufacturerId})", source, productId, manufacturerId);
        return source.FirstOrDefault(pm => pm.ProductId == productId && pm.ManufacturerId == manufacturerId);
    }

    /// <summary>
    /// Get a discount-manufacturer mapping record
    /// </summary>
    /// <param name="manufacturerId">Manufacturer identifier</param>
    /// <param name="discountId">Discount identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<DiscountManufacturerMapping> GetDiscountAppliedToManufacturerAsync(int manufacturerId, int discountId)
    {
		_logger.LogDebug(">>> Entering GetDiscountAppliedToManufacturerAsync(manufacturerId={ManufacturerId},discountId={DiscountId})", manufacturerId, discountId);
		_logger.LogDebug("<<< Exiting GetDiscountAppliedToManufacturerAsync(manufacturerId={ManufacturerId},discountId={DiscountId})", manufacturerId, discountId);
        return await _discountManufacturerMappingRepository.Table
            .FirstOrDefaultAsync(dcm => dcm.EntityId == manufacturerId && dcm.DiscountId == discountId);
    }

    /// <summary>
    /// Inserts a discount-manufacturer mapping record
    /// </summary>
    /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping)
    {
        await _discountManufacturerMappingRepository.InsertAsync(discountManufacturerMapping);
    }

    /// <summary>
    /// Deletes a discount-manufacturer mapping record
    /// </summary>
    /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _discountManufacturerMappingRepository.DeleteAsync(discountManufacturerMapping);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteDiscountManufacturerMappingAsync(discountManufacturerMapping)={DiscountManufacturerMapping}: external service call took {Arg1} ms", discountManufacturerMapping, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteDiscountManufacturerMappingAsync(discountManufacturerMapping)={DiscountManufacturerMapping}: external service call took {Arg1} ms", discountManufacturerMapping, __logStart.ElapsedMilliseconds);
    #endregion
}