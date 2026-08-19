using System.Data.SqlTypes;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Vendors;

namespace Nop.Services.Catalog;

/// <summary>
/// Product service
/// </summary>
public partial class ProductService : IProductService
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IRepository<Category> _categoryRepository;
    protected readonly IRepository<CrossSellProduct> _crossSellProductRepository;
    protected readonly IRepository<DiscountProductMapping> _discountProductMappingRepository;
    protected readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
    protected readonly IRepository<Manufacturer> _manufacturerRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
    protected readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
    protected readonly IRepository<ProductCategory> _productCategoryRepository;
    protected readonly IRepository<ProductManufacturer> _productManufacturerRepository;
    protected readonly IRepository<ProductPicture> _productPictureRepository;
    protected readonly IRepository<ProductProductTagMapping> _productTagMappingRepository;
    protected readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
    protected readonly IRepository<ProductTag> _productTagRepository;
    protected readonly IRepository<ProductVideo> _productVideoRepository;
    protected readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
    protected readonly IRepository<RelatedProduct> _relatedProductRepository;
    protected readonly IRepository<Shipment> _shipmentRepository;
    protected readonly IRepository<StockQuantityHistory> _stockQuantityHistoryRepository;
    protected readonly IRepository<TierPrice> _tierPriceRepository;
    protected readonly ISearchPluginManager _searchPluginManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ProductService(CatalogSettings catalogSettings,
        IAclService aclService,
        ICustomerService customerService,
        IDateRangeService dateRangeService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IRepository<Category> categoryRepository,
        IRepository<CrossSellProduct> crossSellProductRepository,
        IRepository<DiscountProductMapping> discountProductMappingRepository,
        IRepository<LocalizedProperty> localizedPropertyRepository,
        IRepository<Manufacturer> manufacturerRepository,
        IRepository<Product> productRepository,
        IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
        IRepository<ProductAttributeMapping> productAttributeMappingRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<ProductManufacturer> productManufacturerRepository,
        IRepository<ProductPicture> productPictureRepository,
        IRepository<ProductProductTagMapping> productTagMappingRepository,
        IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
        IRepository<ProductTag> productTagRepository,
        IRepository<ProductVideo> productVideoRepository,
        IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
        IRepository<RelatedProduct> relatedProductRepository,
        IRepository<Shipment> shipmentRepository,
        IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
        IRepository<TierPrice> tierPriceRepository,
        ISearchPluginManager searchPluginManager,
        IStaticCacheManager staticCacheManager,
        IVendorService vendorService,
        IStoreMappingService storeMappingService,
        IWorkContext workContext,
        LocalizationSettings localizationSettings)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _customerService = customerService;
        _dateRangeService = dateRangeService;
        _languageService = languageService;
        _localizationService = localizationService;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _categoryRepository = categoryRepository;
        _crossSellProductRepository = crossSellProductRepository;
        _discountProductMappingRepository = discountProductMappingRepository;
        _localizedPropertyRepository = localizedPropertyRepository;
        _manufacturerRepository = manufacturerRepository;
        _productRepository = productRepository;
        _productAttributeCombinationRepository = productAttributeCombinationRepository;
        _productAttributeMappingRepository = productAttributeMappingRepository;
        _productCategoryRepository = productCategoryRepository;
        _productManufacturerRepository = productManufacturerRepository;
        _productPictureRepository = productPictureRepository;
        _productTagMappingRepository = productTagMappingRepository;
        _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
        _productTagRepository = productTagRepository;
        _productVideoRepository = productVideoRepository;
        _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
        _relatedProductRepository = relatedProductRepository;
        _shipmentRepository = shipmentRepository;
        _stockQuantityHistoryRepository = stockQuantityHistoryRepository;
        _tierPriceRepository = tierPriceRepository;
        _searchPluginManager = searchPluginManager;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
        _vendorService = vendorService;
        _workContext = workContext;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Applies the low stock activity to specified product by the total stock quantity
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="totalStock">Total stock</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ApplyLowStockActivityAsync(Product product, int totalStock)
    {
		_logger.LogDebug(">>> Entering ApplyLowStockActivityAsync(product={Product},totalStock={TotalStock})", product, totalStock);
        var isMinimumStockReached = totalStock <= product.MinStockQuantity;

        if (!isMinimumStockReached && !_catalogSettings.PublishBackProductWhenCancellingOrders)
            return;

        switch (product.LowStockActivity)
        {
            case LowStockActivity.DisableBuyButton:
                product.DisableBuyButton = isMinimumStockReached;
				_logger.LogDebug("ApplyLowStockActivityAsync(product={Product},totalStock={TotalStock}): product.DisableBuyButton → {Arg2}", product, totalStock, product.DisableBuyButton);
				_logger.LogDebug("ApplyLowStockActivityAsync(product={Product},totalStock={TotalStock}): product.DisableWishlistButton → {Arg2}", product, totalStock, product.DisableWishlistButton);
                product.DisableWishlistButton = isMinimumStockReached;
                await UpdateProductAsync(product);
                break;

            case LowStockActivity.Unpublish:
                product.Published = !isMinimumStockReached;
				_logger.LogDebug("ApplyLowStockActivityAsync(product={Product},totalStock={TotalStock}): product.Published → {Arg2}", product, totalStock, product.Published);
                await UpdateProductAsync(product);
				_logger.LogDebug(">>> Entering UpdateProductAsync(product={Product})", product);
				_logger.LogDebug("<<< Exiting UpdateProductAsync(product={Product})", product);
                break;

            default:
                break;
        }
		_logger.LogDebug("<<< Exiting ApplyLowStockActivityAsync(product={Product},totalStock={TotalStock})", product, totalStock);
    }

    /// <summary>
    /// Gets SKU, Manufacturer part number and GTIN
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sKU, Manufacturer part number, GTIN
    /// </returns>
    protected virtual async Task<(string sku, string manufacturerPartNumber, string gtin)> GetSkuMpnGtinAsync(Product product, string attributesXml)
    {
        ArgumentNullException.ThrowIfNull(product);

        string sku = null;
        string manufacturerPartNumber = null;
        string gtin = null;

        if (!string.IsNullOrEmpty(attributesXml) &&
            product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
        {
            //manage stock by attribute combinations
            //let's find appropriate record
            var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
            if (combination != null)
            {
                sku = combination.Sku;
                manufacturerPartNumber = combination.ManufacturerPartNumber;
                gtin = combination.Gtin;
            }
        }

        if (string.IsNullOrEmpty(sku))
            sku = product.Sku;
        if (string.IsNullOrEmpty(manufacturerPartNumber))
            manufacturerPartNumber = product.ManufacturerPartNumber;
        if (string.IsNullOrEmpty(gtin))
            gtin = product.Gtin;

        return (sku, manufacturerPartNumber, gtin);
    }

    /// <summary>
    /// Get stock message for a product with attributes
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message
    /// </returns>
    protected virtual async Task<string> GetStockMessageForAttributesAsync(Product product, string attributesXml)
    {
		_logger.LogDebug(">>> Entering GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        if (!product.DisplayStockAvailability)
            return string.Empty;

        string stockMessage;

        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
		_logger.LogDebug("GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml}): combination → {Combination}", product, attributesXml, combination);
        if (combination != null)
        {
            //combination exists
            var stockQuantity = combination.StockQuantity;
            if (stockQuantity > 0)
            {
                if (product.MinStockQuantity >= stockQuantity && product.LowStockActivity == LowStockActivity.Nothing)
                {
                    stockMessage = product.DisplayStockQuantity
                        ?
						_logger.LogDebug("GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml}): stockMessage → {StockMessage}", product, attributesXml, stockMessage);
                        //display "low stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("Products.Availability.LowStockWithQuantity"), stockQuantity)
                        :
                        //display "low stock" without stock quantity
                        await _localizationService.GetResourceAsync("Products.Availability.LowStock");
                }
                else
                {
                    stockMessage = product.DisplayStockQuantity
                        ?
                        //display "in stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("Products.Availability.InStockWithQuantity"), stockQuantity)
                        :
                        //display "in stock" without stock quantity
                        await _localizationService.GetResourceAsync("Products.Availability.InStock");
                }
            }
            else
            {
                if (combination.AllowOutOfStockOrders)
                {
                    stockMessage = await _localizationService.GetResourceAsync("Products.Availability.InStock");
                }
                else
                {
                    var productAvailabilityRange = await
                        _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                    stockMessage = productAvailabilityRange == null
                        ? await _localizationService.GetResourceAsync("Products.Availability.OutOfStock")
                        : string.Format(await _localizationService.GetResourceAsync("Products.Availability.AvailabilityRange"),
                            await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                }
            }
        }
        else
        {
            //no combination configured
            if (product.AllowAddingOnlyExistingAttributeCombinations)
            {
                var allIds = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id)).Where(pa => pa.IsRequired).Select(pa => pa.Id).ToList();
				_logger.LogDebug("GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml}): allIds → {AllIds}", product, attributesXml, allIds);
                var exIds = (await _productAttributeParser.ParseProductAttributeMappingsAsync(attributesXml)).Select(pa => pa.Id).ToList();
				_logger.LogDebug("GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml}): exIds → {ExIds}", product, attributesXml, exIds);

                var selectedIds = allIds.Intersect(exIds).ToList();
				_logger.LogDebug("GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml}): selectedIds → {SelectedIds}", product, attributesXml, selectedIds);

                if (selectedIds.Count != allIds.Count)
                    if (_catalogSettings.AttributeValueOutOfStockDisplayType == AttributeValueOutOfStockDisplayType.AlwaysDisplay)
                        return await _localizationService.GetResourceAsync("Products.Availability.SelectRequiredAttributes");
                    else
                    {
                        var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
						_logger.LogDebug("GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml}): combinations → {Combinations}", product, attributesXml, combinations);

                        combinations = combinations.Where(p => p.StockQuantity >= 0 || p.AllowOutOfStockOrders).ToList();

						var __logStart = System.Diagnostics.Stopwatch.StartNew();
                        var attributes = await combinations.SelectAwait(async c => await _productAttributeParser.ParseProductAttributeMappingsAsync(c.AttributesXml)).ToListAsync();

						_logger.LogDebug("GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml}): attributes → {Attributes}", product, attributesXml, attributes);
						_logger.LogInformation("GetStockMessageForAttributesAsync(product,attributesXml)={Product},{AttributesXml}: database query executed in {Arg2} ms", product, attributesXml, __logStart.ElapsedMilliseconds);
                        var flag = attributes.SelectMany(a => a).Any(a => selectedIds.Contains(a.Id));
						_logger.LogDebug("GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml}): flag → {Flag}", product, attributesXml, flag);

                        if (flag)
                            return await _localizationService.GetResourceAsync("Products.Availability.SelectRequiredAttributes");
                    }

                var productAvailabilityRange = await
                    _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                stockMessage = productAvailabilityRange == null
                    ? await _localizationService.GetResourceAsync("Products.Availability.OutOfStock")
                    : string.Format(await _localizationService.GetResourceAsync("Products.Availability.AvailabilityRange"),
                        await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
            }
            else
            {
                stockMessage = await _localizationService.GetResourceAsync("Products.Availability.InStock");
            }
        }

		_logger.LogDebug("<<< Exiting GetStockMessageForAttributesAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        return stockMessage;
    }

    /// <summary>
    /// Get stock message
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message
    /// </returns>
    protected virtual async Task<string> GetStockMessageAsync(Product product)
    {
		_logger.LogDebug(">>> Entering GetStockMessageAsync(product={Product})", product);
        if (!product.DisplayStockAvailability)
            return string.Empty;

        var stockMessage = string.Empty;
		_logger.LogDebug("GetStockMessageAsync(product={Product}): stockMessage → {StockMessage}", product, stockMessage);
        var stockQuantity = await GetTotalStockQuantityAsync(product);
		_logger.LogDebug("GetStockMessageAsync(product={Product}): stockQuantity → {StockQuantity}", product, stockQuantity);

        if (stockQuantity > 0)
        {
            if (product.MinStockQuantity >= stockQuantity && product.LowStockActivity == LowStockActivity.Nothing)
            {
                stockMessage = product.DisplayStockQuantity
                    ?
                    //display "low stock" with stock quantity
                    string.Format(await _localizationService.GetResourceAsync("Products.Availability.LowStockWithQuantity"), stockQuantity)
                    :
                    //display "low stock" without stock quantity
                    await _localizationService.GetResourceAsync("Products.Availability.LowStock");
            }
            else
            {
                stockMessage = product.DisplayStockQuantity
                    ?
                    //display "in stock" with stock quantity
                    string.Format(await _localizationService.GetResourceAsync("Products.Availability.InStockWithQuantity"), stockQuantity)
                    :
                    //display "in stock" without stock quantity
                    await _localizationService.GetResourceAsync("Products.Availability.InStock");
            }
			_logger.LogDebug(">>> Entering GetTotalStockQuantityAsync(product={Product},useReservedQuantity={UseReservedQuantity},warehouseId={WarehouseId})", product, useReservedQuantity, warehouseId);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("GetTotalStockQuantityAsync(product,useReservedQuantity,warehouseId)={Product},{UseReservedQuantity},{WarehouseId}: database query executed in {Arg3} ms", product, useReservedQuantity, warehouseId, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting GetTotalStockQuantityAsync(product={Product},useReservedQuantity={UseReservedQuantity},warehouseId={WarehouseId})", product, useReservedQuantity, warehouseId);
        }
        else
        {
            //out of stock
            var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
			_logger.LogDebug("GetStockMessageAsync(product={Product}): productAvailabilityRange → {ProductAvailabilityRange}", product, productAvailabilityRange);
            switch (product.BackorderMode)
            {
                case BackorderMode.NoBackorders:
                    stockMessage = productAvailabilityRange == null
                        ? await _localizationService.GetResourceAsync("Products.Availability.OutOfStock")
                        : string.Format(await _localizationService.GetResourceAsync("Products.Availability.AvailabilityRange"),
                            await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                    break;
                case BackorderMode.AllowQtyBelow0:
                    stockMessage = await _localizationService.GetResourceAsync("Products.Availability.InStock");
                    break;
                case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
                    stockMessage = productAvailabilityRange == null
                        ? await _localizationService.GetResourceAsync("Products.Availability.Backordering")
                        : string.Format(await _localizationService.GetResourceAsync("Products.Availability.BackorderingWithDate"),
                            await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                    break;
            }
        }

		_logger.LogDebug("<<< Exiting GetStockMessageAsync(product={Product})", product);
        return stockMessage;
    }

    /// <summary>
    /// Reserve the given quantity in the warehouses.
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="quantity">Quantity, must be negative</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ReserveInventoryAsync(Product product, int quantity)
    {
		_logger.LogDebug(">>> Entering ReserveInventoryAsync(product={Product},quantity={Quantity})", product, quantity);
        ArgumentNullException.ThrowIfNull(product);

        if (quantity >= 0)
			_logger.LogWarning("Throwing ArgumentException from ReserveInventoryAsync(product={Product},quantity={Quantity})", product, quantity);
            throw new ArgumentException("Value must be negative.", nameof(quantity));

        var qty = -quantity;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var productInventory = await _productWarehouseInventoryRepository.Table.Where(pwi => pwi.ProductId == product.Id)
            .OrderByDescending(pwi => pwi.StockQuantity - pwi.ReservedQuantity)
            .ToListAsync();
			_logger.LogInformation("ReserveInventoryAsync(product,quantity)={Product},{Quantity}: database query executed in {Arg2} ms", product, quantity, __logStart.ElapsedMilliseconds);

        if (productInventory.Count <= 0)
            return;

        // 1st pass: Applying reserved
        foreach (var item in productInventory)
        {
            var selectQty = Math.Min(Math.Max(0, item.StockQuantity - item.ReservedQuantity), qty);
			_logger.LogDebug("ReserveInventoryAsync(product={Product},quantity={Quantity}): selectQty → {SelectQty}", product, quantity, selectQty);
            item.ReservedQuantity += selectQty;
            qty -= selectQty;

            if (qty <= 0)
                break;
        }

        if (qty > 0)
        {
            // 2rd pass: Booking negative stock!
            var pwi = productInventory[0];
            pwi.ReservedQuantity += qty;
        }

        await UpdateProductWarehouseInventoryAsync(productInventory);
		_logger.LogDebug("<<< Exiting ReserveInventoryAsync(product={Product},quantity={Quantity})", product, quantity);
    }

    /// <summary>
    /// Unblocks the given quantity reserved items in the warehouses
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="quantity">Quantity, must be positive</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UnblockReservedInventoryAsync(Product product, int quantity)
    {
		_logger.LogDebug(">>> Entering UnblockReservedInventoryAsync(product={Product},quantity={Quantity})", product, quantity);
        ArgumentNullException.ThrowIfNull(product);

        if (quantity < 0)
			_logger.LogWarning("Throwing ArgumentException from UnblockReservedInventoryAsync(product={Product},quantity={Quantity})", product, quantity);
            throw new ArgumentException("Value must be positive.", nameof(quantity));

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var productInventory = await _productWarehouseInventoryRepository.Table.Where(pwi => pwi.ProductId == product.Id)
            .OrderByDescending(pwi => pwi.ReservedQuantity)
            .ThenByDescending(pwi => pwi.StockQuantity)
            .ToListAsync();
			_logger.LogInformation("UnblockReservedInventoryAsync(product,quantity)={Product},{Quantity}: database query executed in {Arg2} ms", product, quantity, __logStart.ElapsedMilliseconds);

        if (!productInventory.Any())
            return;

        var qty = quantity;

        foreach (var item in productInventory)
        {
            var selectQty = Math.Min(item.ReservedQuantity, qty);
			_logger.LogDebug("UnblockReservedInventoryAsync(product={Product},quantity={Quantity}): selectQty → {SelectQty}", product, quantity, selectQty);
            item.ReservedQuantity -= selectQty;
            qty -= selectQty;

            if (qty <= 0)
                break;
        }

        if (qty > 0)
        {
            var pwi = productInventory[0];
            pwi.StockQuantity += qty;
        }

        await UpdateProductWarehouseInventoryAsync(productInventory);
		_logger.LogDebug(">>> Entering UpdateProductWarehouseInventoryAsync(pwi={Pwi})", pwi);
		_logger.LogDebug("<<< Exiting UnblockReservedInventoryAsync(product={Product},quantity={Quantity})", product, quantity);
		_logger.LogDebug("<<< Exiting UpdateProductWarehouseInventoryAsync(pwi={Pwi})", pwi);
    }

    /// <summary>
    /// Gets cross-sell products by product identifier
    /// </summary>
    /// <param name="productIds">The first product identifiers</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cross-sell products
    /// </returns>
    protected virtual async Task<IList<CrossSellProduct>> GetCrossSellProductsByProductIdsAsync(int[] productIds, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetCrossSellProductsByProductIdsAsync(productIds={ProductIds},showHidden={ShowHidden})", productIds, showHidden);
        if (productIds == null || productIds.Length == 0)
            return new List<CrossSellProduct>();

        var query = from csp in _crossSellProductRepository.Table
            join p in _productRepository.Table on csp.ProductId2 equals p.Id
            where productIds.Contains(csp.ProductId1) &&
                  !p.Deleted &&
                  (showHidden || p.Published)
            orderby csp.Id
            select csp;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var crossSellProducts = await query.ToListAsync();

		_logger.LogDebug("GetCrossSellProductsByProductIdsAsync(productIds={ProductIds},showHidden={ShowHidden}): crossSellProducts → {CrossSellProducts}", productIds, showHidden, crossSellProducts);
		_logger.LogInformation("GetCrossSellProductsByProductIdsAsync(productIds,showHidden)={ProductIds},{ShowHidden}: database query executed in {Arg2} ms", productIds, showHidden, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetCrossSellProductsByProductIdsAsync(productIds={ProductIds},showHidden={ShowHidden})", productIds, showHidden);
        return crossSellProducts;
    }

    #endregion

    #region Methods

    #region Products

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAsync(Product product)
    {
		_logger.LogDebug(">>> Entering DeleteProductAsync(product={Product})", product);
		_logger.LogInformation("DeleteProductAsync(product)={Product}: external service call took {Arg1} ms", product, __logStart.ElapsedMilliseconds);
        await _productRepository.DeleteAsync(product);
		_logger.LogDebug("<<< Exiting DeleteProductAsync(product={Product})", product);
    }

    /// <summary>
    /// Delete products
    /// </summary>
    /// <param name="products">Products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductsAsync(IList<Product> products)
    {
		_logger.LogDebug(">>> Entering DeleteProductsAsync(products={Products})", products);
		_logger.LogInformation("DeleteProductsAsync(products)={Products}: external service call took {Arg1} ms", products, __logStart.ElapsedMilliseconds);
        await _productRepository.DeleteAsync(products);
		_logger.LogDebug("<<< Exiting DeleteProductsAsync(products={Products})", products);
    }

    /// <summary>
    /// Gets all products displayed on the home page
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IList<Product>> GetAllProductsDisplayedOnHomepageAsync()
    {
		_logger.LogDebug(">>> Entering GetAllProductsDisplayedOnHomepageAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var products = await _productRepository.GetAllAsync(query =>
        {
            return from p in query
                orderby p.DisplayOrder, p.Id
                where p.Published &&
                      !p.Deleted &&
                      p.ShowOnHomepage
                select p;
				_logger.LogInformation("GetAllProductsDisplayedOnHomepageAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductsHomepageCacheKey));

		_logger.LogDebug("<<< Exiting GetAllProductsDisplayedOnHomepageAsync()");
        return products;
    }

    /// <summary>
    /// Gets a product
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product
    /// </returns>
    public virtual async Task<Product> GetProductByIdAsync(int productId)
    {
		_logger.LogDebug(">>> Entering GetProductByIdAsync(productId={ProductId})", productId);
		_logger.LogDebug("<<< Exiting GetProductByIdAsync(productId={ProductId})", productId);
        return await _productRepository.GetByIdAsync(productId, cache => default);
    }

    /// <summary>
    /// Get products by identifiers
    /// </summary>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IList<Product>> GetProductsByIdsAsync(int[] productIds)
    {
		_logger.LogDebug(">>> Entering GetProductsByIdsAsync(productIds={ProductIds})", productIds);
		_logger.LogDebug("<<< Exiting GetProductsByIdsAsync(productIds={ProductIds})", productIds);
        return await _productRepository.GetByIdsAsync(productIds, cache => default, false);
    }

    /// <summary>
    /// Inserts a product
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAsync(Product product)
    {
		_logger.LogDebug(">>> Entering InsertProductAsync(product={Product})", product);
        await _productRepository.InsertAsync(product);
		_logger.LogDebug("<<< Exiting InsertProductAsync(product={Product})", product);
    }

    /// <summary>
    /// Inserts products
    /// </summary>
    /// <param name="products">Products to insert</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductsAsync(IList<Product> products)
    {
		_logger.LogDebug(">>> Entering InsertProductsAsync(products={Products})", products);
        await _productRepository.InsertAsync(products);
		_logger.LogDebug("<<< Exiting InsertProductsAsync(products={Products})", products);
    }

    /// <summary>
    /// Updates the product
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAsync(Product product)
    {
		_logger.LogDebug(">>> Entering UpdateProductAsync(product={Product})", product);
        await _productRepository.UpdateAsync(product);
		_logger.LogDebug("<<< Exiting UpdateProductAsync(product={Product})", product);
    }

    /// <summary>
    /// Update products
    /// </summary>
    /// <param name="products">Products to update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductsAsync(IList<Product> products)
    {
		_logger.LogDebug(">>> Entering UpdateProductsAsync(products={Products})", products);
        await _productRepository.UpdateAsync(products);
		_logger.LogDebug("<<< Exiting UpdateProductsAsync(products={Products})", products);
    }

    /// <summary>
    /// Gets featured products by a category identifier
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of featured products
    /// </returns>
    public virtual async Task<IList<Product>> GetCategoryFeaturedProductsAsync(int categoryId, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering GetCategoryFeaturedProductsAsync(categoryId={CategoryId},storeId={StoreId})", categoryId, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        IList<Product> featuredProducts = new List<Product>();

		_logger.LogDebug("GetCategoryFeaturedProductsAsync(categoryId={CategoryId},storeId={StoreId}): featuredProducts → {FeaturedProducts}", categoryId, storeId, featuredProducts);
        if (categoryId == 0)
            return featuredProducts;

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetCategoryFeaturedProductsAsync(categoryId={CategoryId},storeId={StoreId}): customer → {Customer}", categoryId, storeId, customer);
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		_logger.LogDebug("GetCategoryFeaturedProductsAsync(categoryId={CategoryId},storeId={StoreId}): customerRoleIds → {CustomerRoleIds}", categoryId, storeId, customerRoleIds);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryFeaturedProductsIdsKey, categoryId, customerRoleIds, storeId);
		_logger.LogDebug("GetCategoryFeaturedProductsAsync(categoryId={CategoryId},storeId={StoreId}): cacheKey → {CacheKey}", categoryId, storeId, cacheKey);

        var featuredProductIds = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var query = from p in _productRepository.Table
				_logger.LogDebug("GetCategoryFeaturedProductsAsync(categoryId={CategoryId},storeId={StoreId}): query → {Query}", categoryId, storeId, query);
                join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                where p.Published && !p.Deleted && p.VisibleIndividually &&
                      (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                      (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                      pc.IsFeaturedProduct && categoryId == pc.CategoryId
                select p;

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            query = await _aclService.ApplyAcl(query, customerRoleIds);

            featuredProducts = await query.ToListAsync();

			_logger.LogInformation("GetCategoryFeaturedProductsAsync(categoryId,storeId)={CategoryId},{StoreId}: database query executed in {Arg2} ms", categoryId, storeId, __logStart.ElapsedMilliseconds);
            return featuredProducts.Select(p => p.Id).ToList();
        });

        if (!featuredProducts.Any() && featuredProductIds.Any())
            featuredProducts = await _productRepository.GetByIdsAsync(featuredProductIds, cache => default, false);

		_logger.LogInformation("GetCategoryFeaturedProductsAsync(categoryId,storeId)={CategoryId},{StoreId}: external service call took {Arg2} ms", categoryId, storeId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetCategoryFeaturedProductsAsync(categoryId={CategoryId},storeId={StoreId})", categoryId, storeId);
        return featuredProducts;
    }

    /// <summary>
    /// Gets featured products by manufacturer identifier
    /// </summary>
    /// <param name="manufacturerId">Manufacturer identifier</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of featured products
    /// </returns>
    public virtual async Task<IList<Product>> GetManufacturerFeaturedProductsAsync(int manufacturerId, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering GetManufacturerFeaturedProductsAsync(manufacturerId={ManufacturerId},storeId={StoreId})", manufacturerId, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        IList<Product> featuredProducts = new List<Product>();

		_logger.LogDebug("GetManufacturerFeaturedProductsAsync(manufacturerId={ManufacturerId},storeId={StoreId}): featuredProducts → {FeaturedProducts}", manufacturerId, storeId, featuredProducts);
        if (manufacturerId == 0)
            return featuredProducts;

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetManufacturerFeaturedProductsAsync(manufacturerId={ManufacturerId},storeId={StoreId}): customer → {Customer}", manufacturerId, storeId, customer);
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		_logger.LogDebug("GetManufacturerFeaturedProductsAsync(manufacturerId={ManufacturerId},storeId={StoreId}): customerRoleIds → {CustomerRoleIds}", manufacturerId, storeId, customerRoleIds);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ManufacturerFeaturedProductIdsKey, manufacturerId, customerRoleIds, storeId);
		_logger.LogDebug("GetManufacturerFeaturedProductsAsync(manufacturerId={ManufacturerId},storeId={StoreId}): cacheKey → {CacheKey}", manufacturerId, storeId, cacheKey);

        var featuredProductIds = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var query = from p in _productRepository.Table
				_logger.LogDebug("GetManufacturerFeaturedProductsAsync(manufacturerId={ManufacturerId},storeId={StoreId}): query → {Query}", manufacturerId, storeId, query);
                join pm in _productManufacturerRepository.Table on p.Id equals pm.ProductId
                where p.Published && !p.Deleted && p.VisibleIndividually &&
                      (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                      (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                      pm.IsFeaturedProduct && manufacturerId == pm.ManufacturerId
                select p;

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            query = await _aclService.ApplyAcl(query, customerRoleIds);

			_logger.LogInformation("GetManufacturerFeaturedProductsAsync(manufacturerId,storeId)={ManufacturerId},{StoreId}: find query executed in {Arg2} ms", manufacturerId, storeId, __logStart.ElapsedMilliseconds);
            return await query.Select(p => p.Id).ToListAsync();
        });

        if (!featuredProducts.Any() && featuredProductIds.Any())
            featuredProducts = await _productRepository.GetByIdsAsync(featuredProductIds, cache => default, false);

		_logger.LogInformation("GetManufacturerFeaturedProductsAsync(manufacturerId,storeId)={ManufacturerId},{StoreId}: external service call took {Arg2} ms", manufacturerId, storeId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetManufacturerFeaturedProductsAsync(manufacturerId={ManufacturerId},storeId={StoreId})", manufacturerId, storeId);
        return featuredProducts;
    }

    /// <summary>
    /// Gets products which marked as new
    /// </summary>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of new products
    /// </returns>
    public virtual async Task<IPagedList<Product>> GetProductsMarkedAsNewAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetProductsMarkedAsNewAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", storeId, pageIndex, pageSize);
        var query = from p in _productRepository.Table
            where p.Published && p.VisibleIndividually && p.MarkAsNew && !p.Deleted &&
                  DateTime.UtcNow >= (p.MarkAsNewStartDateTimeUtc ?? SqlDateTime.MinValue.Value) &&
                  DateTime.UtcNow <= (p.MarkAsNewEndDateTimeUtc ?? SqlDateTime.MaxValue.Value)
            select p;
				_logger.LogDebug("GetProductsMarkedAsNewAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", storeId, pageIndex, pageSize, query);

        //apply store mapping constraints
        query = await _storeMappingService.ApplyStoreMapping(query, storeId);

        //apply ACL constraints
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetProductsMarkedAsNewAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize}): customer → {Customer}", storeId, pageIndex, pageSize, customer);
        query = await _aclService.ApplyAcl(query, customer);

        query = query.OrderByDescending(p => p.CreatedOnUtc);

		_logger.LogDebug("<<< Exiting GetProductsMarkedAsNewAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", storeId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Get number of product (published and visible) in certain category
    /// </summary>
    /// <param name="categoryIds">Category identifiers</param>
    /// <param name="storeId">Store identifier; 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of products
    /// </returns>
    public virtual async Task<int> GetNumberOfProductsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering GetNumberOfProductsInCategoryAsync(categoryIds={CategoryIds},storeId={StoreId})", categoryIds, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //validate "categoryIds" parameter
        if (categoryIds != null && categoryIds.Contains(0))
            categoryIds.Remove(0);

        var query = _productRepository.Table.Where(p => p.Published && !p.Deleted && p.VisibleIndividually);
		_logger.LogDebug("GetNumberOfProductsInCategoryAsync(categoryIds={CategoryIds},storeId={StoreId}): query → {Query}", categoryIds, storeId, query);

        //apply store mapping constraints
        query = await _storeMappingService.ApplyStoreMapping(query, storeId);

        //apply ACL constraints
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetNumberOfProductsInCategoryAsync(categoryIds={CategoryIds},storeId={StoreId}): customer → {Customer}", categoryIds, storeId, customer);
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		_logger.LogDebug("GetNumberOfProductsInCategoryAsync(categoryIds={CategoryIds},storeId={StoreId}): customerRoleIds → {CustomerRoleIds}", categoryIds, storeId, customerRoleIds);
        query = await _aclService.ApplyAcl(query, customerRoleIds);

        //category filtering
        if (categoryIds != null && categoryIds.Any())
        {
            query = from p in query
                join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                where categoryIds.Contains(pc.CategoryId)
                select p;
        }

        var cacheKey = _staticCacheManager
            .PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryProductsNumberCacheKey, customerRoleIds, storeId, categoryIds);

        //only distinct products
		_logger.LogInformation("GetNumberOfProductsInCategoryAsync(categoryIds,storeId)={CategoryIds},{StoreId}: external service call took {Arg2} ms", categoryIds, storeId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetNumberOfProductsInCategoryAsync(categoryIds={CategoryIds},storeId={StoreId})", categoryIds, storeId);
        return await _staticCacheManager.GetAsync(cacheKey, () => query.Select(p => p.Id).CountAsync());
    }

    /// <summary>
    /// Search products
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="categoryIds">Category identifiers</param>
    /// <param name="manufacturerIds">Manufacturer identifiers</param>
    /// <param name="storeId">Store identifier; 0 to load all records</param>
    /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
    /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
    /// <param name="productType">Product type; 0 to load all records</param>
    /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
    /// <param name="excludeFeaturedProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers); "false" (by default) to load all records; "true" to exclude featured products from results</param>
    /// <param name="priceMin">Minimum price; null to load all records</param>
    /// <param name="priceMax">Maximum price; null to load all records</param>
    /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
    /// <param name="keywords">Keywords</param>
    /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
    /// <param name="searchManufacturerPartNumber">A value indicating whether to search by a specified "keyword" in manufacturer part number</param>
    /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
    /// <param name="searchProductTags">A value indicating whether to search by a specified "keyword" in product tags</param>
    /// <param name="languageId">Language identifier (search for text searching)</param>
    /// <param name="filteredSpecOptions">Specification options list to filter products; null to load all records</param>
    /// <param name="orderBy">Order by</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="overridePublished">
    /// null - process "Published" property according to "showHidden" parameter
    /// true - load only "Published" products
    /// false - load only "Unpublished" products
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IPagedList<Product>> SearchProductsAsync(
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        IList<int> categoryIds = null,
        IList<int> manufacturerIds = null,
        int storeId = 0,
        int vendorId = 0,
        int warehouseId = 0,
        ProductType? productType = null,
        bool visibleIndividuallyOnly = false,
        bool excludeFeaturedProducts = false,
        decimal? priceMin = null,
        decimal? priceMax = null,
        int productTagId = 0,
        string keywords = null,
        bool searchDescriptions = false,
        bool searchManufacturerPartNumber = true,
        bool searchSku = true,
        bool searchProductTags = false,
        int languageId = 0,
        IList<SpecificationAttributeOption> filteredSpecOptions = null,
        ProductSortingEnum orderBy = ProductSortingEnum.Position,
        bool showHidden = false,
        bool? overridePublished = null)
    {
        //some databases don't support int.MaxValue
		_logger.LogDebug(">>> Entering SearchProductsAsync()");
        if (pageSize == int.MaxValue)
            pageSize = int.MaxValue - 1;

        var productsQuery = _productRepository.Table;
		_logger.LogDebug("SearchProductsAsync(): productsQuery → {ProductsQuery}", productsQuery);

        if (!showHidden)
            productsQuery = productsQuery.Where(p => p.Published);
        else if (overridePublished.HasValue)
            productsQuery = productsQuery.Where(p => p.Published == overridePublished.Value);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("SearchProductsAsync(): customer → {Customer}", customer);

        if (!showHidden || storeId > 0)
        {
            //apply store mapping constraints
            productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, storeId);
        }

        if (!showHidden)
        {
            //apply ACL constraints
            productsQuery = await _aclService.ApplyAcl(productsQuery, customer);
        }

        productsQuery =
            from p in productsQuery
            where !p.Deleted &&
                  (!visibleIndividuallyOnly || p.VisibleIndividually) &&
                  (vendorId == 0 || p.VendorId == vendorId) &&
                  (
                      warehouseId == 0 ||
                      (
                          !p.UseMultipleWarehouses ? p.WarehouseId == warehouseId :
                              _productWarehouseInventoryRepository.Table.Any(pwi => pwi.WarehouseId == warehouseId && pwi.ProductId == p.Id)
                      )
                  ) &&
                  (productType == null || p.ProductTypeId == (int)productType) &&
                  (showHidden ||
                   DateTime.UtcNow >= (p.AvailableStartDateTimeUtc ?? SqlDateTime.MinValue.Value) &&
                   DateTime.UtcNow <= (p.AvailableEndDateTimeUtc ?? SqlDateTime.MaxValue.Value)
                  ) &&
                  (priceMin == null || p.Price >= priceMin) &&
                  (priceMax == null || p.Price <= priceMax)
            select p;

        var activeSearchProvider = await _searchPluginManager.LoadPrimaryPluginAsync(customer, storeId);
		_logger.LogDebug("SearchProductsAsync(): activeSearchProvider → {ActiveSearchProvider}", activeSearchProvider);
        var providerResults = new List<int>();
		_logger.LogDebug("SearchProductsAsync(): providerResults → {ProviderResults}", providerResults);

        if (!string.IsNullOrEmpty(keywords))
        {
            var langs = await _languageService.GetAllLanguagesAsync(showHidden: true);
			_logger.LogDebug("SearchProductsAsync(): langs → {Langs}", langs);

            //Set a flag which will to points need to search in localized properties. If showHidden doesn't set to true should be at least two published languages.
            var searchLocalizedValue = languageId > 0 && langs.Count >= 2 && (showHidden || langs.Count(l => l.Published) >= 2);
			_logger.LogDebug("SearchProductsAsync(): searchLocalizedValue → {SearchLocalizedValue}", searchLocalizedValue);
            var productsByKeywords = new List<int>().AsQueryable();
			_logger.LogDebug("SearchProductsAsync(): productsByKeywords → {ProductsByKeywords}", productsByKeywords);
            var runStandardSearch = activeSearchProvider is null || showHidden;
			_logger.LogDebug("SearchProductsAsync(): runStandardSearch → {RunStandardSearch}", runStandardSearch);

            try
            {
                if (!runStandardSearch)
                {
                    providerResults = await activeSearchProvider.SearchProductsAsync(keywords, searchLocalizedValue);
                    productsByKeywords = providerResults.AsQueryable();
                }
            }
            catch
            {
				_logger.LogError("Exception in SearchProductsAsync(): {Arg0}", ex.Message);
                runStandardSearch = _catalogSettings.UseStandardSearchWhenSearchProviderThrowsException;
            }

            if (runStandardSearch)
            {
                productsByKeywords =
                    from p in _productRepository.Table
                    where p.Name.Contains(keywords) ||
                          (searchDescriptions &&
                           (p.ShortDescription.Contains(keywords) || p.FullDescription.Contains(keywords))) ||
                          (searchManufacturerPartNumber && p.ManufacturerPartNumber == keywords) ||
                          (searchSku && p.Sku == keywords)
                    select p.Id;

                if (searchLocalizedValue)
                {
                    productsByKeywords = productsByKeywords.Union(
                        from lp in _localizedPropertyRepository.Table
                        let checkName = lp.LocaleKey == nameof(Product.Name) &&
                                        lp.LocaleValue.Contains(keywords)
                        let checkShortDesc = searchDescriptions &&
                                             lp.LocaleKey == nameof(Product.ShortDescription) &&
                                             lp.LocaleValue.Contains(keywords)
                        where
                            lp.LocaleKeyGroup == nameof(Product) && lp.LanguageId == languageId && (checkName || checkShortDesc)

                        select lp.EntityId);
                }
            }

            //search by SKU for ProductAttributeCombination
            if (searchSku)
            {
                productsByKeywords = productsByKeywords.Union(
                    from pac in _productAttributeCombinationRepository.Table
                    where pac.Sku == keywords
                    select pac.ProductId);
            }

            //search by category name if admin allows
            if (_catalogSettings.AllowCustomersToSearchWithCategoryName)
            {
                var categoryQuery = _categoryRepository.Table;
				_logger.LogDebug("SearchProductsAsync(): categoryQuery → {CategoryQuery}", categoryQuery);

                if (!showHidden)
                    categoryQuery = categoryQuery.Where(p => p.Published);
                else if (overridePublished.HasValue)
                    categoryQuery = categoryQuery.Where(p => p.Published == overridePublished.Value);

                if (!showHidden || storeId > 0)
                    categoryQuery = await _storeMappingService.ApplyStoreMapping(categoryQuery, storeId);

                if (!showHidden)
                    categoryQuery = await _aclService.ApplyAcl(categoryQuery, customer);

                productsByKeywords = productsByKeywords.Union(
                    from pc in _productCategoryRepository.Table
                    join c in categoryQuery on pc.CategoryId equals c.Id
                    where c.Name.Contains(keywords) && !c.Deleted
                    select pc.ProductId
                );

                if (searchLocalizedValue)
                {
                    productsByKeywords = productsByKeywords.Union(
                        from pc in _productCategoryRepository.Table
                        join lp in _localizedPropertyRepository.Table on pc.CategoryId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(Category) &&
                              lp.LocaleKey == nameof(Category.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pc.ProductId);
                }
            }

            //search by manufacturer name if admin allows
            if (_catalogSettings.AllowCustomersToSearchWithManufacturerName)
            {
                var manufacturerQuery = _manufacturerRepository.Table;
				_logger.LogDebug("SearchProductsAsync(): manufacturerQuery → {ManufacturerQuery}", manufacturerQuery);

                if (!showHidden)
                    manufacturerQuery = manufacturerQuery.Where(p => p.Published);
                else if (overridePublished.HasValue)
                    manufacturerQuery = manufacturerQuery.Where(p => p.Published == overridePublished.Value);

                if (!showHidden || storeId > 0)
                    manufacturerQuery = await _storeMappingService.ApplyStoreMapping(manufacturerQuery, storeId);

                if (!showHidden)
                    manufacturerQuery = await _aclService.ApplyAcl(manufacturerQuery, customer);

                productsByKeywords = productsByKeywords.Union(
                    from pm in _productManufacturerRepository.Table
                    join m in manufacturerQuery on pm.ManufacturerId equals m.Id
                    where m.Name.Contains(keywords) && !m.Deleted
                    select pm.ProductId
                );

                if (searchLocalizedValue)
                {
                    productsByKeywords = productsByKeywords.Union(
                        from pm in _productManufacturerRepository.Table
                        join lp in _localizedPropertyRepository.Table on pm.ManufacturerId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(Manufacturer) &&
                              lp.LocaleKey == nameof(Manufacturer.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pm.ProductId);
                }
            }

            if (searchProductTags)
            {
                productsByKeywords = productsByKeywords.Union(
                    from pptm in _productTagMappingRepository.Table
                    join pt in _productTagRepository.Table on pptm.ProductTagId equals pt.Id
                    where pt.Name.Contains(keywords)
                    select pptm.ProductId
                );

                if (searchLocalizedValue)
                {
                    productsByKeywords = productsByKeywords.Union(
                        from pptm in _productTagMappingRepository.Table
                        join lp in _localizedPropertyRepository.Table on pptm.ProductTagId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(ProductTag) &&
                              lp.LocaleKey == nameof(ProductTag.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pptm.ProductId);
                }
            }

            productsQuery =
                from p in productsQuery
                join pbk in productsByKeywords on p.Id equals pbk
                select p;
        }

        if (categoryIds is not null)
        {
            categoryIds.Remove(0);

            if (categoryIds.Any())
            {
                var productCategoryQuery =
                    from pc in _productCategoryRepository.Table
                    join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                    where (!excludeFeaturedProducts || !pc.IsFeaturedProduct) &&
                          categoryIds.Contains(pc.CategoryId)
                    orderby c.DisplayOrder
                    group pc by pc.ProductId into pc
                    select new
                    {
                        ProductId = pc.Key,
                        DisplayOrder = pc.First().DisplayOrder
						_logger.LogDebug("SearchProductsAsync(): ProductId → {ProductId}", ProductId);
						_logger.LogDebug("SearchProductsAsync(): DisplayOrder → {DisplayOrder}", DisplayOrder);
                    };

                productsQuery =
                    from p in productsQuery
                    join pc in productCategoryQuery on p.Id equals pc.ProductId
                    orderby pc.DisplayOrder, p.Name
                    select p;
            }
        }

        if (manufacturerIds is not null)
        {
            manufacturerIds.Remove(0);

            if (manufacturerIds.Any())
            {
                var productManufacturerQuery =
                    from pm in _productManufacturerRepository.Table
                    where (!excludeFeaturedProducts || !pm.IsFeaturedProduct) &&
                          manufacturerIds.Contains(pm.ManufacturerId)
                    group pm by pm.ProductId into pm
                    select new
                    {
                        ProductId = pm.Key,
                        DisplayOrder = pm.First().DisplayOrder
                    };

                productsQuery =
                    from p in productsQuery
                    join pm in productManufacturerQuery on p.Id equals pm.ProductId
                    orderby pm.DisplayOrder, p.Name
                    select p;
            }
        }

        if (productTagId > 0)
        {
            productsQuery =
                from p in productsQuery
                join ptm in _productTagMappingRepository.Table on p.Id equals ptm.ProductId
                where ptm.ProductTagId == productTagId
                select p;
        }

        if (filteredSpecOptions?.Count > 0)
        {
            var specificationAttributeIds = filteredSpecOptions
                .Select(sao => sao.SpecificationAttributeId)
                .Distinct();

            foreach (var specificationAttributeId in specificationAttributeIds)
            {
                var optionIdsBySpecificationAttribute = filteredSpecOptions
                    .Where(o => o.SpecificationAttributeId == specificationAttributeId)
                    .Select(o => o.Id);

                var productSpecificationQuery =
                    from psa in _productSpecificationAttributeRepository.Table
                    where psa.AllowFiltering && optionIdsBySpecificationAttribute.Contains(psa.SpecificationAttributeOptionId)
                    select psa;

                productsQuery =
                    from p in productsQuery
                    where productSpecificationQuery.Any(pc => pc.ProductId == p.Id)
                    select p;
            }
        }

        if (providerResults.Any() && orderBy == ProductSortingEnum.Position && !showHidden)
        {
            var sortedProducts = from p in productsQuery
                                 join pr in providerResults.Select((id, ind) => new { ind, id }) on p.Id equals pr.id into orderSeq
                                 from os in orderSeq.DefaultIfEmpty()
								using Microsoft.Extensions.Logging;
                                 orderby os == null ? int.MaxValue : os.ind
                                 select p;
                                 

            return await sortedProducts.ToPagedListAsync(pageIndex, pageSize);
        }

		_logger.LogDebug("<<< Exiting SearchProductsAsync()");
        return await productsQuery.OrderBy(_localizedPropertyRepository, await _workContext.GetWorkingLanguageAsync(), orderBy).ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets products by product attribute
    /// </summary>
    /// <param name="productAttributeId">Product attribute identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IPagedList<Product>> GetProductsByProductAttributeIdAsync(int productAttributeId,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        var query = from p in _productRepository.Table
			_logger.LogDebug(">>> Entering GetProductsByProductAttributeIdAsync(productAttributeId={ProductAttributeId},pageIndex={PageIndex},pageSize={PageSize})", productAttributeId, pageIndex, pageSize);
            join pam in _productAttributeMappingRepository.Table on p.Id equals pam.ProductId
            where
                pam.ProductAttributeId == productAttributeId &&
                !p.Deleted
            orderby p.Name
			_logger.LogDebug("<<< Exiting GetProductsByProductAttributeIdAsync(productAttributeId={ProductAttributeId},pageIndex={PageIndex},pageSize={PageSize})", productAttributeId, pageIndex, pageSize);
            select p;

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets associated products
    /// </summary>
    /// <param name="parentGroupedProductId">Parent product identifier (used with grouped products)</param>
    /// <param name="storeId">Store identifier; 0 to load all records</param>
    /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IList<Product>> GetAssociatedProductsAsync(int parentGroupedProductId,
        int storeId = 0, int vendorId = 0, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetAssociatedProductsAsync(parentGroupedProductId={ParentGroupedProductId},storeId={StoreId},vendorId={VendorId},showHidden={ShowHidden})", parentGroupedProductId, storeId, vendorId, showHidden);
        var query = _productRepository.Table;
        query = query.Where(x => x.ParentGroupedProductId == parentGroupedProductId);
		_logger.LogDebug("GetAssociatedProductsAsync(parentGroupedProductId={ParentGroupedProductId},storeId={StoreId},vendorId={VendorId},showHidden={ShowHidden}): query → {Query}", parentGroupedProductId, storeId, vendorId, showHidden, query);
        if (!showHidden)
        {
            query = query.Where(x => x.Published);

            //available dates
            query = query.Where(p =>
                (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow));
        }
        //vendor filtering
        if (vendorId > 0)
        {
            query = query.Where(p => p.VendorId == vendorId);
        }

        //apply store mapping constraints
        if (!showHidden && storeId > 0)
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

        if (!showHidden)
        {
            //apply ACL constraints
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogDebug("GetAssociatedProductsAsync(parentGroupedProductId={ParentGroupedProductId},storeId={StoreId},vendorId={VendorId},showHidden={ShowHidden}): customer → {Customer}", parentGroupedProductId, storeId, vendorId, showHidden, customer);
            query = await _aclService.ApplyAcl(query, customer);
        }

        query = query.Where(x => !x.Deleted);
        query = query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id);

		_logger.LogDebug("<<< Exiting GetAssociatedProductsAsync(parentGroupedProductId={ParentGroupedProductId},storeId={StoreId},vendorId={VendorId},showHidden={ShowHidden})", parentGroupedProductId, storeId, vendorId, showHidden);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Get low stock products
    /// </summary>
    /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
    /// <param name="loadPublishedOnly">Whether to load published products only; pass null to load all products, pass true to load only published products, pass false to load only unpublished products</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IPagedList<Product>> GetLowStockProductsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
		_logger.LogDebug(">>> Entering GetLowStockProductsAsync(vendorId={VendorId},loadPublishedOnly={LoadPublishedOnly},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", vendorId, loadPublishedOnly, pageIndex, pageSize, getOnlyTotalCount);
        var query = _productRepository.Table;

		_logger.LogDebug("GetLowStockProductsAsync(vendorId={VendorId},loadPublishedOnly={LoadPublishedOnly},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount}): query → {Query}", vendorId, loadPublishedOnly, pageIndex, pageSize, getOnlyTotalCount, query);
        //filter by products with tracking inventory
        query = query.Where(product => product.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStock);

        //filter by products with stock quantity less than the minimum
        query = query.Where(product =>
            (product.UseMultipleWarehouses ? _productWarehouseInventoryRepository.Table.Where(pwi => pwi.ProductId == product.Id).Sum(pwi => pwi.StockQuantity - pwi.ReservedQuantity)
                : product.StockQuantity) <= product.MinStockQuantity);

        //ignore deleted products
        query = query.Where(product => !product.Deleted);

        //ignore grouped products
        query = query.Where(product => product.ProductTypeId != (int)ProductType.GroupedProduct);

        //filter by vendor
        if (vendorId.HasValue && vendorId.Value > 0)
            query = query.Where(product => product.VendorId == vendorId.Value);

        //whether to load published products only
        if (loadPublishedOnly.HasValue)
            query = query.Where(product => product.Published == loadPublishedOnly.Value);

        query = query.OrderBy(product => product.MinStockQuantity).ThenBy(product => product.DisplayOrder).ThenBy(product => product.Id);

		_logger.LogDebug("<<< Exiting GetLowStockProductsAsync(vendorId={VendorId},loadPublishedOnly={LoadPublishedOnly},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", vendorId, loadPublishedOnly, pageIndex, pageSize, getOnlyTotalCount);
        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Get low stock product combinations
    /// </summary>
    /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
    /// <param name="loadPublishedOnly">Whether to load combinations of published products only; pass null to load all products, pass true to load only published products, pass false to load only unpublished products</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product combinations
    /// </returns>
    public virtual async Task<IPagedList<ProductAttributeCombination>> GetLowStockProductCombinationsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
		_logger.LogDebug(">>> Entering GetLowStockProductCombinationsAsync(vendorId={VendorId},loadPublishedOnly={LoadPublishedOnly},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", vendorId, loadPublishedOnly, pageIndex, pageSize, getOnlyTotalCount);
        var combinations = from pac in _productAttributeCombinationRepository.Table
            join p in _productRepository.Table on pac.ProductId equals p.Id
            where
                //filter by combinations with stock quantity less than the minimum
                pac.StockQuantity <= pac.MinStockQuantity &&
                //filter by products with tracking inventory by attributes
                p.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStockByAttributes &&
                //ignore deleted products
                !p.Deleted &&
                //ignore grouped products
                p.ProductTypeId != (int)ProductType.GroupedProduct &&
                //filter by vendor
                ((vendorId ?? 0) == 0 || p.VendorId == vendorId) &&
                //whether to load published products only
                (loadPublishedOnly == null || p.Published == loadPublishedOnly)
            orderby pac.ProductId, pac.Id
            select pac;

		_logger.LogDebug("<<< Exiting GetLowStockProductCombinationsAsync(vendorId={VendorId},loadPublishedOnly={LoadPublishedOnly},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", vendorId, loadPublishedOnly, pageIndex, pageSize, getOnlyTotalCount);
        return await combinations.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Gets a product by SKU
    /// </summary>
    /// <param name="sku">SKU</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product
    /// </returns>
    public virtual async Task<Product> GetProductBySkuAsync(string sku)
    {
		_logger.LogDebug(">>> Entering GetProductBySkuAsync(sku={Sku})", sku);
        if (string.IsNullOrEmpty(sku))
            return null;

        sku = sku.Trim();
		_logger.LogDebug("GetProductBySkuAsync(sku={Sku}): sku → {Sku1}", sku, sku);

        var query = from p in _productRepository.Table
            orderby p.Id
            where !p.Deleted &&
                  p.Sku == sku
            select p;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var product = await query.FirstOrDefaultAsync();

		_logger.LogDebug("GetProductBySkuAsync(sku={Sku}): product → {Product}", sku, product);
		_logger.LogInformation("GetProductBySkuAsync(sku)={Sku}: database query executed in {Arg1} ms", sku, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetProductBySkuAsync(sku={Sku})", sku);
        return product;
    }

    /// <summary>
    /// Gets a products by SKU array
    /// </summary>
    /// <param name="skuArray">SKU array</param>
    /// <param name="vendorId">Vendor ID; 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IList<Product>> GetProductsBySkuAsync(string[] skuArray, int vendorId = 0)
    {
		_logger.LogDebug(">>> Entering GetProductsBySkuAsync(skuArray={SkuArray},vendorId={VendorId})", skuArray, vendorId);
        ArgumentNullException.ThrowIfNull(skuArray);

        var query = _productRepository.Table;
		_logger.LogDebug("GetProductsBySkuAsync(skuArray={SkuArray},vendorId={VendorId}): query → {Query}", skuArray, vendorId, query);
        query = query.Where(p => !p.Deleted && skuArray.Contains(p.Sku));

        if (vendorId != 0)
            query = query.Where(p => p.VendorId == vendorId);

		_logger.LogDebug("<<< Exiting GetProductsBySkuAsync(skuArray={SkuArray},vendorId={VendorId})", skuArray, vendorId);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Gets number of products by vendor identifier
    /// </summary>
    /// <param name="vendorId">Vendor identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of products
    /// </returns>
    public virtual async Task<int> GetNumberOfProductsByVendorIdAsync(int vendorId)
    {
		_logger.LogDebug(">>> Entering GetNumberOfProductsByVendorIdAsync(vendorId={VendorId})", vendorId);
        if (vendorId == 0)
            return 0;

        return await _productRepository.Table.CountAsync(p => p.VendorId == vendorId && !p.Deleted);
    }

    /// <summary>
    /// Parse "required product Ids" property
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A list of required product IDs</returns>
    public virtual int[] ParseRequiredProductIds(Product product)
    {
		_logger.LogDebug(">>> Entering ParseRequiredProductIds(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        if (string.IsNullOrEmpty(product.RequiredProductIds))
            return Array.Empty<int>();

        var ids = new List<int>();
		_logger.LogDebug("ParseRequiredProductIds(product={Product}): ids → {Ids}", product, ids);

        foreach (var idStr in product.RequiredProductIds
                     .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                     .Select(x => x.Trim()))
            if (int.TryParse(idStr, out var id))
                ids.Add(id);

		_logger.LogDebug("<<< Exiting ParseRequiredProductIds(product={Product})", product);
        return ids.ToArray();
    }

    /// <summary>
    /// Get a value indicating whether a product is available now (availability dates)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="dateTime">Datetime to check; pass null to use current date</param>
    /// <returns>Result</returns>
    public virtual bool ProductIsAvailable(Product product, DateTime? dateTime = null)
    {
		_logger.LogDebug(">>> Entering ProductIsAvailable(product={Product},dateTime={DateTime})", product, dateTime);
        ArgumentNullException.ThrowIfNull(product);

        dateTime ??= DateTime.UtcNow;

        if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value > dateTime)
            return false;

        if (product.AvailableEndDateTimeUtc.HasValue && product.AvailableEndDateTimeUtc.Value < dateTime)
            return false;

        return true;
    }

    /// <summary>
    /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>Result</returns>
    public virtual int[] ParseAllowedQuantities(Product product)
    {
		_logger.LogDebug(">>> Entering ParseAllowedQuantities(product={Product})", product);
        ArgumentNullException.ThrowIfNull(product);

        var result = new List<int>();
		_logger.LogDebug("ParseAllowedQuantities(product={Product}): result → {Result}", product, result);
        if (!string.IsNullOrWhiteSpace(product.AllowedQuantities))
        {
            var quantities = product.AllowedQuantities
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            foreach (var qtyStr in quantities)
            {
                if (int.TryParse(qtyStr.Trim(), out var qty))
                    result.Add(qty);
            }
        }

		_logger.LogDebug("<<< Exiting ParseAllowedQuantities(product={Product})", product);
        return result.ToArray();
    }

    /// <summary>
    /// Get total quantity
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="useReservedQuantity">
    /// A value indicating whether we should consider "Reserved Quantity" property 
    /// when "multiple warehouses" are used
    /// </param>
    /// <param name="warehouseId">
    /// Warehouse identifier. Used to limit result to certain warehouse.
    /// Used only with "multiple warehouses" enabled.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<int> GetTotalStockQuantityAsync(Product product, bool useReservedQuantity = true, int warehouseId = 0)
    {
		_logger.LogDebug(">>> Entering GetTotalStockQuantityAsync(product={Product},useReservedQuantity={UseReservedQuantity},warehouseId={WarehouseId})", product, useReservedQuantity, warehouseId);
        ArgumentNullException.ThrowIfNull(product);

        if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
            //We can calculate total stock quantity when 'Manage inventory' property is set to 'Track inventory'
            return 0;

        if (!product.UseMultipleWarehouses)
            return product.StockQuantity;

        var pwi = _productWarehouseInventoryRepository.Table.Where(wi => wi.ProductId == product.Id);

        if (warehouseId > 0)
            pwi = pwi.Where(x => x.WarehouseId == warehouseId);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("GetTotalStockQuantityAsync(product,useReservedQuantity,warehouseId)={Product},{UseReservedQuantity},{WarehouseId}: database query executed in {Arg3} ms", product, useReservedQuantity, warehouseId, __logStart.ElapsedMilliseconds);
        var result = await pwi.SumAsync(x => x.StockQuantity);
        if (useReservedQuantity)
            result -= await pwi.SumAsync(x => x.ReservedQuantity);

        return result;
    }

    /// <summary>
    /// Get number of rental periods (price ratio)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Number of rental periods</returns>
    public virtual int GetRentalPeriods(Product product, DateTime startDate, DateTime endDate)
    {
		_logger.LogDebug(">>> Entering GetRentalPeriods(product={Product},startDate={StartDate},endDate={EndDate})", product, startDate, endDate);
        ArgumentNullException.ThrowIfNull(product);

        if (!product.IsRental)
            return 1;

        if (startDate.CompareTo(endDate) >= 0)
            return 1;

        int totalPeriods;
        switch (product.RentalPricePeriod)
        {
            case RentalPricePeriod.Days:
            {
                var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                var configuredPeriodDays = product.RentalPriceLength;
                totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
            }
			_logger.LogDebug("GetRentalPeriods(product={Product},startDate={StartDate},endDate={EndDate}): totalDaysToRent → {TotalDaysToRent}", product, startDate, endDate, totalDaysToRent);
			_logger.LogDebug("GetRentalPeriods(product={Product},startDate={StartDate},endDate={EndDate}): totalPeriods → {TotalPeriods}", product, startDate, endDate, totalPeriods);

                break;
            case RentalPricePeriod.Weeks:
            {
                var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                var configuredPeriodDays = 7 * product.RentalPriceLength;
                totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
            }

                break;
            case RentalPricePeriod.Months:
            {
                //Source: http://stackoverflow.com/questions/4638993/difference-in-months-between-two-dates
                var totalMonthsToRent = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
                if (startDate.AddMonths(totalMonthsToRent) < endDate)
                    //several days added (not full month)
                    totalMonthsToRent++;

                var configuredPeriodMonths = product.RentalPriceLength;
                totalPeriods = Convert.ToInt32(Math.Ceiling((double)totalMonthsToRent / configuredPeriodMonths));
            }
			_logger.LogDebug("GetRentalPeriods(product={Product},startDate={StartDate},endDate={EndDate}): totalMonthsToRent → {TotalMonthsToRent}", product, startDate, endDate, totalMonthsToRent);

                break;
            case RentalPricePeriod.Years:
            {
                var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                var configuredPeriodDays = 365 * product.RentalPriceLength;
                totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
            }

                break;
            default:
				_logger.LogWarning("Throwing Exception from GetRentalPeriods(product={Product},startDate={StartDate},endDate={EndDate})", product, startDate, endDate);
		_logger.LogDebug("<<< Exiting GetRentalPeriods(product={Product},startDate={StartDate},endDate={EndDate})", product, startDate, endDate);
                throw new Exception("Not supported rental period");
        }

        return totalPeriods;
    }

    /// <summary>
    /// Formats the stock availability/quantity message
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Selected product attributes in XML format (if specified)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the stock message
    /// </returns>
    public virtual async Task<string> FormatStockMessageAsync(Product product, string attributesXml)
    {
		_logger.LogDebug(">>> Entering FormatStockMessageAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        ArgumentNullException.ThrowIfNull(product);

        var stockMessage = string.Empty;
		_logger.LogDebug("FormatStockMessageAsync(product={Product},attributesXml={AttributesXml}): stockMessage → {StockMessage}", product, attributesXml, stockMessage);

        switch (product.ManageInventoryMethod)
        {
            case ManageInventoryMethod.ManageStock:
                stockMessage = await GetStockMessageAsync(product);
                break;
            case ManageInventoryMethod.ManageStockByAttributes:
                stockMessage = await GetStockMessageForAttributesAsync(product, attributesXml);
                break;
        }

		_logger.LogDebug("<<< Exiting FormatStockMessageAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        return stockMessage;
    }

    /// <summary>
    /// Formats SKU
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sKU
    /// </returns>
    public virtual async Task<string> FormatSkuAsync(Product product, string attributesXml = null)
    {
		_logger.LogDebug(">>> Entering FormatSkuAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        ArgumentNullException.ThrowIfNull(product);

        var (sku, _, _) = await GetSkuMpnGtinAsync(product, attributesXml);

		_logger.LogDebug("<<< Exiting FormatSkuAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        return sku;
    }

    /// <summary>
    /// Formats manufacturer part number
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer part number
    /// </returns>
    public virtual async Task<string> FormatMpnAsync(Product product, string attributesXml = null)
    {
		_logger.LogDebug(">>> Entering FormatMpnAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        ArgumentNullException.ThrowIfNull(product);

        var (_, manufacturerPartNumber, _) = await GetSkuMpnGtinAsync(product, attributesXml);

		_logger.LogDebug("<<< Exiting FormatMpnAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        return manufacturerPartNumber;
    }

    /// <summary>
    /// Formats GTIN
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gTIN
    /// </returns>
    public virtual async Task<string> FormatGtinAsync(Product product, string attributesXml = null)
    {
		_logger.LogDebug(">>> Entering FormatGtinAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        ArgumentNullException.ThrowIfNull(product);

        var (_, _, gtin) = await GetSkuMpnGtinAsync(product, attributesXml);

		_logger.LogDebug("<<< Exiting FormatGtinAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        return gtin;
    }

    /// <summary>
    /// Formats start/end date for rental product
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="date">Date</param>
    /// <returns>Formatted date</returns>
    public virtual string FormatRentalDate(Product product, DateTime date)
    {
		_logger.LogDebug(">>> Entering FormatRentalDate(product={Product},date={Date})", product, date);
        ArgumentNullException.ThrowIfNull(product);

        if (!product.IsRental)
            return null;

        return date.ToShortDateString();
    }

    /// <summary>
    /// Gets the value whether the sequence contains downloadable products
    /// </summary>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> HasAnyDownloadableProductAsync(int[] productIds)
    {
		_logger.LogDebug(">>> Entering HasAnyDownloadableProductAsync(productIds={ProductIds})", productIds);
		_logger.LogDebug("<<< Exiting HasAnyDownloadableProductAsync(productIds={ProductIds})", productIds);
        return await _productRepository.Table
            .AnyAsync(p => productIds.Contains(p.Id) && p.IsDownload);
    }

    /// <summary>
    /// Gets the value whether the sequence contains gift card products
    /// </summary>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> HasAnyGiftCardProductAsync(int[] productIds)
    {
		_logger.LogDebug(">>> Entering HasAnyGiftCardProductAsync(productIds={ProductIds})", productIds);
		_logger.LogDebug("<<< Exiting HasAnyGiftCardProductAsync(productIds={ProductIds})", productIds);
        return await _productRepository.Table
            .AnyAsync(p => productIds.Contains(p.Id) && p.IsGiftCard);
    }

    /// <summary>
    /// Gets the value whether the sequence contains recurring products
    /// </summary>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> HasAnyRecurringProductAsync(int[] productIds)
    {
		_logger.LogDebug(">>> Entering HasAnyRecurringProductAsync(productIds={ProductIds})", productIds);
		_logger.LogDebug("<<< Exiting HasAnyRecurringProductAsync(productIds={ProductIds})", productIds);
        return await _productRepository.Table
            .AnyAsync(p => productIds.Contains(p.Id) && p.IsRecurring);
    }

    /// <summary>
    /// Returns a list of sku of not existing products
    /// </summary>
    /// <param name="productSku">The sku of the products to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of sku not existing products
    /// </returns>
    public virtual async Task<string[]> GetNotExistingProductsAsync(string[] productSku)
    {
		_logger.LogDebug(">>> Entering GetNotExistingProductsAsync(productSku={ProductSku})", productSku);
        ArgumentNullException.ThrowIfNull(productSku);

        var query = _productRepository.Table;
		_logger.LogDebug("GetNotExistingProductsAsync(productSku={ProductSku}): query → {Query}", productSku, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var queryFilter = productSku.Distinct().ToArray();
		_logger.LogDebug("GetNotExistingProductsAsync(productSku={ProductSku}): queryFilter → {QueryFilter}", productSku, queryFilter);
        //filtering by SKU
        var filter = await query.Select(p => p.Sku)
            .Where(p => queryFilter.Contains(p))
            .ToListAsync();
			_logger.LogInformation("GetNotExistingProductsAsync(productSku)={ProductSku}: database query executed in {Arg1} ms", productSku, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting GetNotExistingProductsAsync(productSku={ProductSku})", productSku);
        return queryFilter.Except(filter).ToArray();
    }

    #endregion

    #region Inventory management methods

    /// <summary>
    /// Adjust inventory
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="quantityToChange">Quantity to increase or decrease</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="message">Message for the stock quantity history</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task AdjustInventoryAsync(Product product, int quantityToChange, string attributesXml = "", string message = "")
    {
		_logger.LogDebug(">>> Entering AdjustInventoryAsync(product={Product},quantityToChange={QuantityToChange},attributesXml={AttributesXml},message={Message})", product, quantityToChange, attributesXml, message);
        ArgumentNullException.ThrowIfNull(product);

        if (quantityToChange == 0)
            return;

        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
        {
            //update stock quantity
            if (product.UseMultipleWarehouses)
            {
                //use multiple warehouses
                if (quantityToChange < 0)
                    await ReserveInventoryAsync(product, quantityToChange);
                else
                    await UnblockReservedInventoryAsync(product, quantityToChange);
            }
            else
            {
                //do not use multiple warehouses
                //simple inventory management
                product.StockQuantity += quantityToChange;
                await UpdateProductAsync(product);

                //quantity change history
                await AddStockQuantityHistoryEntryAsync(product, quantityToChange, product.StockQuantity, product.WarehouseId, message);
            }

            var totalStock = await GetTotalStockQuantityAsync(product);
			_logger.LogDebug("AdjustInventoryAsync(product={Product},quantityToChange={QuantityToChange},attributesXml={AttributesXml},message={Message}): totalStock → {TotalStock}", product, quantityToChange, attributesXml, message, totalStock);

            await ApplyLowStockActivityAsync(product, totalStock);

            //send email notification
            if (quantityToChange < 0 && totalStock < product.NotifyAdminForQuantityBelow)
            {
                //do not inject IWorkflowMessageService via constructor because it'll cause circular references
                var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
				_logger.LogDebug("AdjustInventoryAsync(product={Product},quantityToChange={QuantityToChange},attributesXml={AttributesXml},message={Message}): workflowMessageService → {WorkflowMessageService}", product, quantityToChange, attributesXml, message, workflowMessageService);
                await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(product, _localizationSettings.DefaultAdminLanguageId);

                if (product.VendorId != 0)
                {
                    var vendor = await _vendorService.GetVendorByIdAsync(product.VendorId);
					_logger.LogDebug("AdjustInventoryAsync(product={Product},quantityToChange={QuantityToChange},attributesXml={AttributesXml},message={Message}): vendor → {Vendor}", product, quantityToChange, attributesXml, message, vendor);
                    await workflowMessageService.SendQuantityBelowVendorNotificationAsync(product, vendor, _localizationSettings.DefaultAdminLanguageId);
                }
            }
        }

        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
        {
            var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
			_logger.LogDebug("AdjustInventoryAsync(product={Product},quantityToChange={QuantityToChange},attributesXml={AttributesXml},message={Message}): combination → {Combination}", product, quantityToChange, attributesXml, message, combination);
            if (combination != null)
            {
                combination.StockQuantity += quantityToChange;
                await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);

                //quantity change history
                await AddStockQuantityHistoryEntryAsync(product, quantityToChange, combination.StockQuantity, message: message, combinationId: combination.Id);

                if (product.AllowAddingOnlyExistingAttributeCombinations)
                {
					var __logStart = System.Diagnostics.Stopwatch.StartNew();
                    var totalStockByAllCombinations = await (await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id))
                        .ToAsyncEnumerable()
                        .SumAsync(c => c.StockQuantity);
						_logger.LogInformation("AdjustInventoryAsync(product,quantityToChange,attributesXml,message)={Product},{QuantityToChange},{AttributesXml},{Message}: database query executed in {Arg4} ms", product, quantityToChange, attributesXml, message, __logStart.ElapsedMilliseconds);

                    await ApplyLowStockActivityAsync(product, totalStockByAllCombinations);
					_logger.LogDebug(">>> Entering AddStockQuantityHistoryEntryAsync(product={Product},quantityAdjustment={QuantityAdjustment},stockQuantity={StockQuantity},warehouseId={WarehouseId},message={Message},combinationId={CombinationId})", product, quantityAdjustment, stockQuantity, warehouseId, message, combinationId);
					_logger.LogInformation("AddStockQuantityHistoryEntryAsync(product,quantityAdjustment,stockQuantity,warehouseId,message,combinationId)={Product},{QuantityAdjustment},{StockQuantity},{WarehouseId},{Message},{CombinationId}: save query executed in {Arg6} ms", product, quantityAdjustment, stockQuantity, warehouseId, message, combinationId, __logStart.ElapsedMilliseconds);
					_logger.LogDebug("<<< Exiting AddStockQuantityHistoryEntryAsync(product={Product},quantityAdjustment={QuantityAdjustment},stockQuantity={StockQuantity},warehouseId={WarehouseId},message={Message},combinationId={CombinationId})", product, quantityAdjustment, stockQuantity, warehouseId, message, combinationId);
                }

                //send email notification
                if (quantityToChange < 0 && combination.StockQuantity < combination.NotifyAdminForQuantityBelow)
                {
                    //do not inject IWorkflowMessageService via constructor because it'll cause circular references
                    var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                    await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(combination, _localizationSettings.DefaultAdminLanguageId);

                    if (product.VendorId != 0)
                    {
                        var vendor = await _vendorService.GetVendorByIdAsync(product.VendorId);
                        await workflowMessageService.SendQuantityBelowVendorNotificationAsync(combination, vendor, _localizationSettings.DefaultAdminLanguageId);
                    }
                }
            }
        }

        //bundled products
        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
		_logger.LogDebug("AdjustInventoryAsync(product={Product},quantityToChange={QuantityToChange},attributesXml={AttributesXml},message={Message}): attributeValues → {AttributeValues}", product, quantityToChange, attributesXml, message, attributeValues);
        foreach (var attributeValue in attributeValues)
        {
            if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToProduct)
                continue;

            //associated product (bundle)
            var associatedProduct = await GetProductByIdAsync(attributeValue.AssociatedProductId);
			_logger.LogDebug("AdjustInventoryAsync(product={Product},quantityToChange={QuantityToChange},attributesXml={AttributesXml},message={Message}): associatedProduct → {AssociatedProduct}", product, quantityToChange, attributesXml, message, associatedProduct);
            if (associatedProduct != null)
            {
                await AdjustInventoryAsync(associatedProduct, quantityToChange * attributeValue.Quantity, message);
            }
        }
		_logger.LogDebug("<<< Exiting AdjustInventoryAsync(product={Product},quantityToChange={QuantityToChange},attributesXml={AttributesXml},message={Message})", product, quantityToChange, attributesXml, message);
    }

    /// <summary>
    /// Book the reserved quantity
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="warehouseId">Warehouse identifier</param>
    /// <param name="quantity">Quantity, must be negative</param>
    /// <param name="message">Message for the stock quantity history</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task BookReservedInventoryAsync(Product product, int warehouseId, int quantity, string message = "")
    {
		_logger.LogDebug(">>> Entering BookReservedInventoryAsync(product={Product},warehouseId={WarehouseId},quantity={Quantity},message={Message})", product, warehouseId, quantity, message);
        ArgumentNullException.ThrowIfNull(product);

        if (quantity >= 0)
			_logger.LogWarning("Throwing ArgumentException from BookReservedInventoryAsync(product={Product},warehouseId={WarehouseId},quantity={Quantity},message={Message})", product, warehouseId, quantity, message);
            throw new ArgumentException("Value must be negative.", nameof(quantity));

        //only products with "use multiple warehouses" are handled this way
        if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !product.UseMultipleWarehouses)
            return;

        var pwi = await _productWarehouseInventoryRepository.Table
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

            .FirstOrDefaultAsync(wi => wi.ProductId == product.Id && wi.WarehouseId == warehouseId);
			_logger.LogInformation("BookReservedInventoryAsync(product,warehouseId,quantity,message)={Product},{WarehouseId},{Quantity},{Message}: database query executed in {Arg4} ms", product, warehouseId, quantity, message, __logStart.ElapsedMilliseconds);
        if (pwi == null)
            return;

        pwi.ReservedQuantity = Math.Max(pwi.ReservedQuantity + quantity, 0);
        pwi.StockQuantity += quantity;

        await UpdateProductWarehouseInventoryAsync(pwi);

        //quantity change history
        await AddStockQuantityHistoryEntryAsync(product, quantity, pwi.StockQuantity, warehouseId, message);
		_logger.LogDebug("<<< Exiting BookReservedInventoryAsync(product={Product},warehouseId={WarehouseId},quantity={Quantity},message={Message})", product, warehouseId, quantity, message);
    }

    /// <summary>
    /// Reverse booked inventory (if acceptable)
    /// </summary>
    /// <param name="product">product</param>
    /// <param name="shipmentItem">Shipment item</param>
    /// <param name="message">Message for the stock quantity history</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quantity reversed
    /// </returns>
    public virtual async Task<int> ReverseBookedInventoryAsync(Product product, ShipmentItem shipmentItem, string message = "")
    {
		_logger.LogDebug(">>> Entering ReverseBookedInventoryAsync(product={Product},shipmentItem={ShipmentItem},message={Message})", product, shipmentItem, message);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(shipmentItem);

        //only products with "use multiple warehouses" are handled this way
        if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !product.UseMultipleWarehouses)
            return 0;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var pwi = await _productWarehouseInventoryRepository.Table
            .FirstOrDefaultAsync(wi => wi.ProductId == product.Id && wi.WarehouseId == shipmentItem.WarehouseId);
			_logger.LogInformation("ReverseBookedInventoryAsync(product,shipmentItem,message)={Product},{ShipmentItem},{Message}: database query executed in {Arg3} ms", product, shipmentItem, message, __logStart.ElapsedMilliseconds);
        if (pwi == null)
            return 0;

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentItem.ShipmentId, cache => default);

		_logger.LogDebug("ReverseBookedInventoryAsync(product={Product},shipmentItem={ShipmentItem},message={Message}): shipment → {Shipment}", product, shipmentItem, message, shipment);
		_logger.LogInformation("ReverseBookedInventoryAsync(product,shipmentItem,message)={Product},{ShipmentItem},{Message}: find query executed in {Arg3} ms", product, shipmentItem, message, __logStart.ElapsedMilliseconds);
        //not shipped yet? hence "BookReservedInventory" method was not invoked
        if (!shipment.ShippedDateUtc.HasValue)
            return 0;

        var qty = shipmentItem.Quantity;

        pwi.StockQuantity += qty;
        pwi.ReservedQuantity += qty;

        await UpdateProductWarehouseInventoryAsync(pwi);

        //quantity change history
        await AddStockQuantityHistoryEntryAsync(product, qty, pwi.StockQuantity, shipmentItem.WarehouseId, message);

		_logger.LogDebug("<<< Exiting ReverseBookedInventoryAsync(product={Product},shipmentItem={ShipmentItem},message={Message})", product, shipmentItem, message);
        return qty;
    }

    #endregion

    #region Related products

    /// <summary>
    /// Deletes a related product
    /// </summary>
    /// <param name="relatedProduct">Related product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteRelatedProductAsync(RelatedProduct relatedProduct)
    {
		_logger.LogDebug(">>> Entering DeleteRelatedProductAsync(relatedProduct={RelatedProduct})", relatedProduct);
		_logger.LogInformation("DeleteRelatedProductAsync(relatedProduct)={RelatedProduct}: external service call took {Arg1} ms", relatedProduct, __logStart.ElapsedMilliseconds);
        await _relatedProductRepository.DeleteAsync(relatedProduct);
		_logger.LogDebug("<<< Exiting DeleteRelatedProductAsync(relatedProduct={RelatedProduct})", relatedProduct);
    }

    /// <summary>
    /// Gets related products by product identifier
    /// </summary>
    /// <param name="productId">The first product identifier</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the related products
    /// </returns>
    public virtual async Task<IList<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetRelatedProductsByProductId1Async(productId={ProductId},showHidden={ShowHidden})", productId, showHidden);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = from rp in _relatedProductRepository.Table
            join p in _productRepository.Table on rp.ProductId2 equals p.Id
            where rp.ProductId1 == productId &&
                  !p.Deleted &&
                  (showHidden || p.Published)
            orderby rp.DisplayOrder, rp.Id
            select rp;

        var relatedProducts = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.RelatedProductsCacheKey, productId, showHidden), async () => await query.ToListAsync());

		_logger.LogDebug("GetRelatedProductsByProductId1Async(productId={ProductId},showHidden={ShowHidden}): relatedProducts → {RelatedProducts}", productId, showHidden, relatedProducts);
		_logger.LogInformation("GetRelatedProductsByProductId1Async(productId,showHidden)={ProductId},{ShowHidden}: external service call took {Arg2} ms", productId, showHidden, __logStart.ElapsedMilliseconds);
		_logger.LogInformation("GetRelatedProductsByProductId1Async(productId,showHidden)={ProductId},{ShowHidden}: find query executed in {Arg2} ms", productId, showHidden, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetRelatedProductsByProductId1Async(productId={ProductId},showHidden={ShowHidden})", productId, showHidden);
        return relatedProducts;
    }

    /// <summary>
    /// Gets a related product
    /// </summary>
    /// <param name="relatedProductId">Related product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the related product
    /// </returns>
    public virtual async Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId)
    {
		_logger.LogDebug(">>> Entering GetRelatedProductByIdAsync(relatedProductId={RelatedProductId})", relatedProductId);
		_logger.LogDebug("<<< Exiting GetRelatedProductByIdAsync(relatedProductId={RelatedProductId})", relatedProductId);
        return await _relatedProductRepository.GetByIdAsync(relatedProductId, cache => default);
    }

    /// <summary>
    /// Inserts a related product
    /// </summary>
    /// <param name="relatedProduct">Related product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertRelatedProductAsync(RelatedProduct relatedProduct)
    {
		_logger.LogDebug(">>> Entering InsertRelatedProductAsync(relatedProduct={RelatedProduct})", relatedProduct);
        await _relatedProductRepository.InsertAsync(relatedProduct);
		_logger.LogDebug("<<< Exiting InsertRelatedProductAsync(relatedProduct={RelatedProduct})", relatedProduct);
    }

    /// <summary>
    /// Updates a related product
    /// </summary>
    /// <param name="relatedProduct">Related product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateRelatedProductAsync(RelatedProduct relatedProduct)
    {
		_logger.LogDebug(">>> Entering UpdateRelatedProductAsync(relatedProduct={RelatedProduct})", relatedProduct);
        await _relatedProductRepository.UpdateAsync(relatedProduct);
		_logger.LogDebug("<<< Exiting UpdateRelatedProductAsync(relatedProduct={RelatedProduct})", relatedProduct);
    }

    /// <summary>
    /// Finds a related product item by specified identifiers
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="productId1">The first product identifier</param>
    /// <param name="productId2">The second product identifier</param>
    /// <returns>Related product</returns>
    public virtual RelatedProduct FindRelatedProduct(IList<RelatedProduct> source, int productId1, int productId2)
    {
		_logger.LogDebug(">>> Entering FindRelatedProduct(source={Source},productId1={ProductId1},productId2={ProductId2})", source, productId1, productId2);
		_logger.LogDebug("<<< Exiting FindRelatedProduct(source={Source},productId1={ProductId1},productId2={ProductId2})", source, productId1, productId2);
        return source.FirstOrDefault(rp => rp.ProductId1 == productId1 && rp.ProductId2 == productId2);
    }

    #endregion

    #region Cross-sell products

    /// <summary>
    /// Deletes a cross-sell product
    /// </summary>
    /// <param name="crossSellProduct">Cross-sell identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCrossSellProductAsync(CrossSellProduct crossSellProduct)
    {
		_logger.LogDebug(">>> Entering DeleteCrossSellProductAsync(crossSellProduct={CrossSellProduct})", crossSellProduct);
		_logger.LogInformation("DeleteCrossSellProductAsync(crossSellProduct)={CrossSellProduct}: external service call took {Arg1} ms", crossSellProduct, __logStart.ElapsedMilliseconds);
        await _crossSellProductRepository.DeleteAsync(crossSellProduct);
		_logger.LogDebug("<<< Exiting DeleteCrossSellProductAsync(crossSellProduct={CrossSellProduct})", crossSellProduct);
    }

    /// <summary>
    /// Gets cross-sell products by product identifier
    /// </summary>
    /// <param name="productId1">The first product identifier</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cross-sell products
    /// </returns>
    public virtual async Task<IList<CrossSellProduct>> GetCrossSellProductsByProductId1Async(int productId1, bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetCrossSellProductsByProductId1Async(productId1={ProductId1},showHidden={ShowHidden})", productId1, showHidden);
		_logger.LogDebug("<<< Exiting GetCrossSellProductsByProductId1Async(productId1={ProductId1},showHidden={ShowHidden})", productId1, showHidden);
        return await GetCrossSellProductsByProductIdsAsync([productId1], showHidden);
    }

    /// <summary>
    /// Gets a cross-sell product
    /// </summary>
    /// <param name="crossSellProductId">Cross-sell product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cross-sell product
    /// </returns>
    public virtual async Task<CrossSellProduct> GetCrossSellProductByIdAsync(int crossSellProductId)
    {
		_logger.LogDebug(">>> Entering GetCrossSellProductByIdAsync(crossSellProductId={CrossSellProductId})", crossSellProductId);
		_logger.LogDebug("<<< Exiting GetCrossSellProductByIdAsync(crossSellProductId={CrossSellProductId})", crossSellProductId);
        return await _crossSellProductRepository.GetByIdAsync(crossSellProductId, cache => default);
    }

    /// <summary>
    /// Inserts a cross-sell product
    /// </summary>
    /// <param name="crossSellProduct">Cross-sell product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCrossSellProductAsync(CrossSellProduct crossSellProduct)
    {
		_logger.LogDebug(">>> Entering InsertCrossSellProductAsync(crossSellProduct={CrossSellProduct})", crossSellProduct);
        await _crossSellProductRepository.InsertAsync(crossSellProduct);
		_logger.LogDebug("<<< Exiting InsertCrossSellProductAsync(crossSellProduct={CrossSellProduct})", crossSellProduct);
    }

    /// <summary>
    /// Gets a cross-sells
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="numberOfProducts">Number of products to return</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cross-sells
    /// </returns>
    public virtual async Task<IList<Product>> GetCrossSellProductsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfProducts)
    {
		_logger.LogDebug(">>> Entering GetCrossSellProductsByShoppingCartAsync(cart={Cart},numberOfProducts={NumberOfProducts})", cart, numberOfProducts);
        var result = new List<Product>();

		_logger.LogDebug("GetCrossSellProductsByShoppingCartAsync(cart={Cart},numberOfProducts={NumberOfProducts}): result → {Result}", cart, numberOfProducts, result);
        if (numberOfProducts == 0 || cart?.Any() != true)
            return result;

        var cartProductIds = cart.Select(sci => sci.ProductId).ToHashSet();
		_logger.LogDebug("GetCrossSellProductsByShoppingCartAsync(cart={Cart},numberOfProducts={NumberOfProducts}): cartProductIds → {CartProductIds}", cart, numberOfProducts, cartProductIds);
		_logger.LogDebug("<<< Exiting GetCrossSellProductsByShoppingCartAsync(cart={Cart},numberOfProducts={NumberOfProducts})", cart, numberOfProducts);
        return await (await GetCrossSellProductsByProductIdsAsync(cartProductIds.ToArray()))
            .Select(cs => cs.ProductId2)
            .Except(cartProductIds)
            .SelectAwait(async cs => await GetProductByIdAsync(cs))
            .Where(p => p != null && !p.Deleted && p.Published)
            .Take(numberOfProducts)
            .ToListAsync();
    }

    /// <summary>
    /// Finds a cross-sell product item by specified identifiers
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="productId1">The first product identifier</param>
    /// <param name="productId2">The second product identifier</param>
    /// <returns>Cross-sell product</returns>
    public virtual CrossSellProduct FindCrossSellProduct(IList<CrossSellProduct> source, int productId1, int productId2)
    {
		_logger.LogDebug(">>> Entering FindCrossSellProduct(source={Source},productId1={ProductId1},productId2={ProductId2})", source, productId1, productId2);
		_logger.LogDebug("<<< Exiting FindCrossSellProduct(source={Source},productId1={ProductId1},productId2={ProductId2})", source, productId1, productId2);
        return source.FirstOrDefault(csp => csp.ProductId1 == productId1 && csp.ProductId2 == productId2);
    }

    #endregion

    #region Tier prices

    /// <summary>
    /// Gets a product tier prices for customer
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <param name="store">Store</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IList<TierPrice>> GetTierPricesAsync(Product product, Customer customer, Store store)
    {
		_logger.LogDebug(">>> Entering GetTierPricesAsync(product={Product},customer={Customer},store={Store})", product, customer, store);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(customer);

        //get actual tier prices
		_logger.LogDebug("<<< Exiting GetTierPricesAsync(product={Product},customer={Customer},store={Store})", product, customer, store);
        return (await GetTierPricesByProductAsync(product.Id))
            .OrderBy(price => price.Quantity)
            .FilterByStore(store)
            .FilterByCustomerRole(await _customerService.GetCustomerRoleIdsAsync(customer))
            .FilterByDate()
            .RemoveDuplicatedQuantities()
            .ToList();
    }

    /// <summary>
    /// Gets a tier prices by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IList<TierPrice>> GetTierPricesByProductAsync(int productId)
    {
		_logger.LogDebug(">>> Entering GetTierPricesByProductAsync(productId={ProductId})", productId);
		_logger.LogInformation("GetTierPricesByProductAsync(productId)={ProductId}: external service call took {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await _staticCacheManager.GetAsync(
            _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.TierPricesByProductCacheKey, productId),
            async () => await _tierPriceRepository.Table.Where(tp => tp.ProductId == productId).ToListAsync());
    }

    /// <summary>
    /// Deletes a tier price
    /// </summary>
    /// <param name="tierPrice">Tier price</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteTierPriceAsync(TierPrice tierPrice)
    {
		_logger.LogDebug(">>> Entering DeleteTierPriceAsync(tierPrice={TierPrice})", tierPrice);
		_logger.LogInformation("DeleteTierPriceAsync(tierPrice)={TierPrice}: external service call took {Arg1} ms", tierPrice, __logStart.ElapsedMilliseconds);
        await _tierPriceRepository.DeleteAsync(tierPrice);
		_logger.LogDebug("<<< Exiting DeleteTierPriceAsync(tierPrice={TierPrice})", tierPrice);
    }

    /// <summary>
    /// Gets a tier price
    /// </summary>
    /// <param name="tierPriceId">Tier price identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ier price
    /// </returns>
    public virtual async Task<TierPrice> GetTierPriceByIdAsync(int tierPriceId)
    {
		_logger.LogDebug(">>> Entering GetTierPriceByIdAsync(tierPriceId={TierPriceId})", tierPriceId);
		_logger.LogDebug("<<< Exiting GetTierPriceByIdAsync(tierPriceId={TierPriceId})", tierPriceId);
        return await _tierPriceRepository.GetByIdAsync(tierPriceId, cache => default);
    }

    /// <summary>
    /// Inserts a tier price
    /// </summary>
    /// <param name="tierPrice">Tier price</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTierPriceAsync(TierPrice tierPrice)
    {
		_logger.LogDebug(">>> Entering InsertTierPriceAsync(tierPrice={TierPrice})", tierPrice);
        await _tierPriceRepository.InsertAsync(tierPrice);
		_logger.LogDebug("<<< Exiting InsertTierPriceAsync(tierPrice={TierPrice})", tierPrice);
    }

    /// <summary>
    /// Updates the tier price
    /// </summary>
    /// <param name="tierPrice">Tier price</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTierPriceAsync(TierPrice tierPrice)
    {
		_logger.LogDebug(">>> Entering UpdateTierPriceAsync(tierPrice={TierPrice})", tierPrice);
        await _tierPriceRepository.UpdateAsync(tierPrice);
		_logger.LogDebug("<<< Exiting UpdateTierPriceAsync(tierPrice={TierPrice})", tierPrice);
    }

    /// <summary>
    /// Gets a preferred tier price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <param name="store">Store</param>
    /// <param name="quantity">Quantity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the tier price
    /// </returns>
    public virtual async Task<TierPrice> GetPreferredTierPriceAsync(Product product, Customer customer, Store store, int quantity)
    {
		_logger.LogDebug(">>> Entering GetPreferredTierPriceAsync(product={Product},customer={Customer},store={Store},quantity={Quantity})", product, customer, store, quantity);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(customer);

        //get the most suitable tier price based on the passed quantity
		_logger.LogDebug("<<< Exiting GetPreferredTierPriceAsync(product={Product},customer={Customer},store={Store},quantity={Quantity})", product, customer, store, quantity);
        return (await GetTierPricesAsync(product, customer, store))?.LastOrDefault(price => quantity >= price.Quantity);
    }

    #endregion

    #region Product pictures

    /// <summary>
    /// Deletes a product picture
    /// </summary>
    /// <param name="productPicture">Product picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductPictureAsync(ProductPicture productPicture)
    {
		_logger.LogDebug(">>> Entering DeleteProductPictureAsync(productPicture={ProductPicture})", productPicture);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productPictureRepository.DeleteAsync(productPicture);
		_logger.LogInformation("DeleteProductPictureAsync(productPicture)={ProductPicture}: external service call took {Arg1} ms", productPicture, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteProductPictureAsync(productPicture={ProductPicture})", productPicture);
    }

    /// <summary>
    /// Gets a product pictures by product identifier
    /// </summary>
    /// <param name="productId">The product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product pictures
    /// </returns>
    public virtual async Task<IList<ProductPicture>> GetProductPicturesByProductIdAsync(int productId)
    {
		_logger.LogDebug(">>> Entering GetProductPicturesByProductIdAsync(productId={ProductId})", productId);
        var query = from pp in _productPictureRepository.Table
            where pp.ProductId == productId
            orderby pp.DisplayOrder, pp.Id
            select pp;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var productPictures = await query.ToListAsync();

		_logger.LogDebug("GetProductPicturesByProductIdAsync(productId={ProductId}): productPictures → {ProductPictures}", productId, productPictures);
		_logger.LogInformation("GetProductPicturesByProductIdAsync(productId)={ProductId}: database query executed in {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetProductPicturesByProductIdAsync(productId={ProductId})", productId);
        return productPictures;
    }

    /// <summary>
    /// Gets a product picture
    /// </summary>
    /// <param name="productPictureId">Product picture identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product picture
    /// </returns>
    public virtual async Task<ProductPicture> GetProductPictureByIdAsync(int productPictureId)
    {
		_logger.LogDebug(">>> Entering GetProductPictureByIdAsync(productPictureId={ProductPictureId})", productPictureId);
		_logger.LogDebug("<<< Exiting GetProductPictureByIdAsync(productPictureId={ProductPictureId})", productPictureId);
        return await _productPictureRepository.GetByIdAsync(productPictureId, cache => default);
    }

    /// <summary>
    /// Inserts a product picture
    /// </summary>
    /// <param name="productPicture">Product picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductPictureAsync(ProductPicture productPicture)
    {
		_logger.LogDebug(">>> Entering InsertProductPictureAsync(productPicture={ProductPicture})", productPicture);
        await _productPictureRepository.InsertAsync(productPicture);
		_logger.LogDebug("<<< Exiting InsertProductPictureAsync(productPicture={ProductPicture})", productPicture);
    }

    /// <summary>
    /// Updates a product picture
    /// </summary>
    /// <param name="productPicture">Product picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductPictureAsync(ProductPicture productPicture)
    {
		_logger.LogDebug(">>> Entering UpdateProductPictureAsync(productPicture={ProductPicture})", productPicture);
        await _productPictureRepository.UpdateAsync(productPicture);
		_logger.LogDebug("<<< Exiting UpdateProductPictureAsync(productPicture={ProductPicture})", productPicture);
    }

    /// <summary>
    /// Get the IDs of all product images 
    /// </summary>
    /// <param name="productsIds">Products IDs</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the all picture identifiers grouped by product ID
    /// </returns>
    public virtual async Task<IDictionary<int, int[]>> GetProductsImagesIdsAsync(int[] productsIds)
    {
        var productPictures = await _productPictureRepository.Table
            .Where(p => productsIds.Contains(p.ProductId))
            .ToListAsync();

        return productPictures.GroupBy(p => p.ProductId).ToDictionary(p => p.Key, p => p.Select(p1 => p1.PictureId).ToArray());
    }

    /// <summary>
    /// Get products for which a discount is applied
    /// </summary>
    /// <param name="discountId">Discount identifier; pass null to load all records</param>
    /// <param name="showHidden">A value indicating whether to load deleted products</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of products
    /// </returns>
    public virtual async Task<IPagedList<Product>> GetProductsWithAppliedDiscountAsync(int? discountId = null,
        bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetProductsWithAppliedDiscountAsync(discountId={DiscountId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", discountId, showHidden, pageIndex, pageSize);
        var products = _productRepository.Table;

		_logger.LogDebug("GetProductsWithAppliedDiscountAsync(discountId={DiscountId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize}): products → {Products}", discountId, showHidden, pageIndex, pageSize, products);
        if (discountId.HasValue)
            products = from product in products
                join dpm in _discountProductMappingRepository.Table on product.Id equals dpm.EntityId
                where dpm.DiscountId == discountId.Value
                select product;

        if (!showHidden)
            products = products.Where(product => !product.Deleted);

        products = products.OrderBy(product => product.DisplayOrder).ThenBy(product => product.Id);

		_logger.LogDebug("<<< Exiting GetProductsWithAppliedDiscountAsync(discountId={DiscountId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", discountId, showHidden, pageIndex, pageSize);
        return await products.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion

    #region Product videos

    /// <summary>
    /// Deletes a product video
    /// </summary>
    /// <param name="productVideo">Product video</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductVideoAsync(ProductVideo productVideo)
    {
		_logger.LogDebug(">>> Entering DeleteProductVideoAsync(productVideo={ProductVideo})", productVideo);
		_logger.LogInformation("DeleteProductVideoAsync(productVideo)={ProductVideo}: external service call took {Arg1} ms", productVideo, __logStart.ElapsedMilliseconds);
        await _productVideoRepository.DeleteAsync(productVideo);
		_logger.LogDebug("<<< Exiting DeleteProductVideoAsync(productVideo={ProductVideo})", productVideo);
    }

    /// <summary>
    /// Gets a product videos by product identifier
    /// </summary>
    /// <param name="productId">The product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product videos
    /// </returns>
    public virtual async Task<IList<ProductVideo>> GetProductVideosByProductIdAsync(int productId)
    {
		_logger.LogDebug(">>> Entering GetProductVideosByProductIdAsync(productId={ProductId})", productId);
        var query = from pvm in _productVideoRepository.Table
            where pvm.ProductId == productId
            orderby pvm.DisplayOrder, pvm.Id
            select pvm;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var productVideos = await query.ToListAsync();

		_logger.LogDebug("GetProductVideosByProductIdAsync(productId={ProductId}): productVideos → {ProductVideos}", productId, productVideos);
		_logger.LogInformation("GetProductVideosByProductIdAsync(productId)={ProductId}: database query executed in {Arg1} ms", productId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetProductVideosByProductIdAsync(productId={ProductId})", productId);
        return productVideos;
    }

    /// <summary>
    /// Gets a product video
    /// </summary>
    /// <param name="productPictureId">Product video identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product video
    /// </returns>
    public virtual async Task<ProductVideo> GetProductVideoByIdAsync(int productVideoId)
    {
		_logger.LogDebug(">>> Entering GetProductVideoByIdAsync(productVideoId={ProductVideoId})", productVideoId);
		_logger.LogDebug("<<< Exiting GetProductVideoByIdAsync(productVideoId={ProductVideoId})", productVideoId);
        return await _productVideoRepository.GetByIdAsync(productVideoId, cache => default);
    }

    /// <summary>
    /// Inserts a product video
    /// </summary>
    /// <param name="productVideo">Product picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductVideoAsync(ProductVideo productVideo)
    {
		_logger.LogDebug(">>> Entering InsertProductVideoAsync(productVideo={ProductVideo})", productVideo);
        await _productVideoRepository.InsertAsync(productVideo);
		_logger.LogDebug("<<< Exiting InsertProductVideoAsync(productVideo={ProductVideo})", productVideo);
    }

    /// <summary>
    /// Updates a product video
    /// </summary>
    /// <param name="productVideo">Product video</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductVideoAsync(ProductVideo productVideo)
    {
		_logger.LogDebug(">>> Entering UpdateProductVideoAsync(productVideo={ProductVideo})", productVideo);
        await _productVideoRepository.UpdateAsync(productVideo);
		_logger.LogDebug("<<< Exiting UpdateProductVideoAsync(productVideo={ProductVideo})", productVideo);
    }

    #endregion

    #region Product warehouses

    /// <summary>
    /// Get a product warehouse-inventory records by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IList<ProductWarehouseInventory>> GetAllProductWarehouseInventoryRecordsAsync(int productId)
    {
		_logger.LogDebug(">>> Entering GetAllProductWarehouseInventoryRecordsAsync(productId={ProductId})", productId);
		_logger.LogDebug("<<< Exiting GetAllProductWarehouseInventoryRecordsAsync(productId={ProductId})", productId);
        return await _productWarehouseInventoryRepository.GetAllAsync(query => query.Where(pwi => pwi.ProductId == productId));
    }

    /// <summary>
    /// Deletes a record to manage product inventory per warehouse
    /// </summary>
    /// <param name="pwi">Record to manage product inventory per warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductWarehouseInventoryAsync(ProductWarehouseInventory pwi)
    {
		_logger.LogDebug(">>> Entering DeleteProductWarehouseInventoryAsync(pwi={Pwi})", pwi);
		_logger.LogInformation("DeleteProductWarehouseInventoryAsync(pwi)={Pwi}: external service call took {Arg1} ms", pwi, __logStart.ElapsedMilliseconds);
        await _productWarehouseInventoryRepository.DeleteAsync(pwi);
		_logger.LogDebug("<<< Exiting DeleteProductWarehouseInventoryAsync(pwi={Pwi})", pwi);
    }

    /// <summary>
    /// Inserts a record to manage product inventory per warehouse
    /// </summary>
    /// <param name="pwi">Record to manage product inventory per warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductWarehouseInventoryAsync(ProductWarehouseInventory pwi)
    {
		_logger.LogDebug(">>> Entering InsertProductWarehouseInventoryAsync(pwi={Pwi})", pwi);
        await _productWarehouseInventoryRepository.InsertAsync(pwi);
		_logger.LogDebug("<<< Exiting InsertProductWarehouseInventoryAsync(pwi={Pwi})", pwi);
    }

    /// <summary>
    /// Updates a record to manage product inventory per warehouse
    /// </summary>
    /// <param name="pwi">Record to manage product inventory per warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductWarehouseInventoryAsync(ProductWarehouseInventory pwi)
    {
        await _productWarehouseInventoryRepository.UpdateAsync(pwi);
    }

    /// <summary>
    /// Updates a records to manage product inventory per warehouse
    /// </summary>
    /// <param name="pwis">Records to manage product inventory per warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductWarehouseInventoryAsync(IList<ProductWarehouseInventory> pwis)
    {
		_logger.LogDebug(">>> Entering UpdateProductWarehouseInventoryAsync(pwis={Pwis})", pwis);
        await _productWarehouseInventoryRepository.UpdateAsync(pwis);
		_logger.LogDebug("<<< Exiting UpdateProductWarehouseInventoryAsync(pwis={Pwis})", pwis);
    }

    #endregion

    #region Stock quantity history

    /// <summary>
    /// Add stock quantity change entry
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="quantityAdjustment">Quantity adjustment</param>
    /// <param name="stockQuantity">Current stock quantity</param>
    /// <param name="warehouseId">Warehouse identifier</param>
    /// <param name="message">Message</param>
    /// <param name="combinationId">Product attribute combination identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task AddStockQuantityHistoryEntryAsync(Product product, int quantityAdjustment, int stockQuantity,
        int warehouseId = 0, string message = "", int? combinationId = null)
    {
		_logger.LogDebug(">>> Entering AddStockQuantityHistoryEntryAsync(product={Product},quantityAdjustment={QuantityAdjustment},stockQuantity={StockQuantity},warehouseId={WarehouseId},message={Message},combinationId={CombinationId})", product, quantityAdjustment, stockQuantity, warehouseId, message, combinationId);
        ArgumentNullException.ThrowIfNull(product);

        if (quantityAdjustment == 0)
            return;

        var historyEntry = new StockQuantityHistory
        {
            ProductId = product.Id,
            CombinationId = combinationId,
            WarehouseId = warehouseId > 0 ? (int?)warehouseId : null,
            QuantityAdjustment = quantityAdjustment,
            StockQuantity = stockQuantity,
            Message = message,
            CreatedOnUtc = DateTime.UtcNow
        };

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("AddStockQuantityHistoryEntryAsync(product,quantityAdjustment,stockQuantity,warehouseId,message,combinationId)={Product},{QuantityAdjustment},{StockQuantity},{WarehouseId},{Message},{CombinationId}: save query executed in {Arg6} ms", product, quantityAdjustment, stockQuantity, warehouseId, message, combinationId, __logStart.ElapsedMilliseconds);
        await _stockQuantityHistoryRepository.InsertAsync(historyEntry);
    }

    /// <summary>
    /// Get the history of the product stock quantity changes
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="warehouseId">Warehouse identifier; pass 0 to load all entries</param>
    /// <param name="combinationId">Product attribute combination identifier; pass 0 to load all entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of stock quantity change entries
    /// </returns>
    public virtual async Task<IPagedList<StockQuantityHistory>> GetStockQuantityHistoryAsync(Product product, int warehouseId = 0, int combinationId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetStockQuantityHistoryAsync(product={Product},warehouseId={WarehouseId},combinationId={CombinationId},pageIndex={PageIndex},pageSize={PageSize})", product, warehouseId, combinationId, pageIndex, pageSize);
        ArgumentNullException.ThrowIfNull(product);

        var query = _stockQuantityHistoryRepository.Table.Where(historyEntry => historyEntry.ProductId == product.Id);
		_logger.LogDebug("GetStockQuantityHistoryAsync(product={Product},warehouseId={WarehouseId},combinationId={CombinationId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", product, warehouseId, combinationId, pageIndex, pageSize, query);

        if (warehouseId > 0)
            query = query.Where(historyEntry => historyEntry.WarehouseId == warehouseId);

        if (combinationId > 0)
            query = query.Where(historyEntry => historyEntry.CombinationId == combinationId);

        query = query.OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id);

		_logger.LogDebug("<<< Exiting GetStockQuantityHistoryAsync(product={Product},warehouseId={WarehouseId},combinationId={CombinationId},pageIndex={PageIndex},pageSize={PageSize})", product, warehouseId, combinationId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion

    #region Product discounts

    /// <summary>
    /// Clean up product references for a specified discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearDiscountProductMappingAsync(Discount discount)
    {
		_logger.LogDebug(">>> Entering ClearDiscountProductMappingAsync(discount={Discount})", discount);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(discount);

        var mappingsWithProducts = await _discountProductMappingRepository.Table
            .Where(dpm => dpm.DiscountId == discount.Id)
            .ToListAsync();
			_logger.LogInformation("ClearDiscountProductMappingAsync(discount)={Discount}: database query executed in {Arg1} ms", discount, __logStart.ElapsedMilliseconds);

        await _discountProductMappingRepository.DeleteAsync(mappingsWithProducts);
		_logger.LogInformation("ClearDiscountProductMappingAsync(discount)={Discount}: external service call took {Arg1} ms", discount, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting ClearDiscountProductMappingAsync(discount={Discount})", discount);
    }

    /// <summary>
    /// Get a discount-product mapping records by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IList<DiscountProductMapping>> GetAllDiscountsAppliedToProductAsync(int productId)
    {
		_logger.LogDebug(">>> Entering GetAllDiscountsAppliedToProductAsync(productId={ProductId})", productId);
		_logger.LogDebug("<<< Exiting GetAllDiscountsAppliedToProductAsync(productId={ProductId})", productId);
        return await _discountProductMappingRepository.GetAllAsync(query => query.Where(dcm => dcm.EntityId == productId));
    }

    /// <summary>
    /// Get a discount-product mapping record
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="discountId">Discount identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<DiscountProductMapping> GetDiscountAppliedToProductAsync(int productId, int discountId)
    {
		_logger.LogDebug(">>> Entering GetDiscountAppliedToProductAsync(productId={ProductId},discountId={DiscountId})", productId, discountId);
		_logger.LogDebug("<<< Exiting GetDiscountAppliedToProductAsync(productId={ProductId},discountId={DiscountId})", productId, discountId);
        return await _discountProductMappingRepository.Table
            .FirstOrDefaultAsync(dcm => dcm.EntityId == productId && dcm.DiscountId == discountId);
    }

    /// <summary>
    /// Inserts a discount-product mapping record
    /// </summary>
    /// <param name="discountProductMapping">Discount-product mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertDiscountProductMappingAsync(DiscountProductMapping discountProductMapping)
    {
		_logger.LogDebug(">>> Entering InsertDiscountProductMappingAsync(discountProductMapping=<redacted>)");
        await _discountProductMappingRepository.InsertAsync(discountProductMapping);
		_logger.LogDebug("<<< Exiting InsertDiscountProductMappingAsync(discountProductMapping=<redacted>)");
    }

    /// <summary>
    /// Deletes a discount-product mapping record
    /// </summary>
    /// <param name="discountProductMapping">Discount-product mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteDiscountProductMappingAsync(DiscountProductMapping discountProductMapping)
    {
		_logger.LogDebug(">>> Entering DeleteDiscountProductMappingAsync(discountProductMapping=<redacted>)");
		_logger.LogInformation("DeleteDiscountProductMappingAsync(discountProductMapping)={DiscountProductMapping}: external service call took {Arg1} ms", discountProductMapping, __logStart.ElapsedMilliseconds);
        await _discountProductMappingRepository.DeleteAsync(discountProductMapping);
		_logger.LogDebug("<<< Exiting DeleteDiscountProductMappingAsync(discountProductMapping=<redacted>)");
    }

    #endregion

    #endregion
}