using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents the main plugin service class
/// </summary>
public class OmnisendService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<NewsLetterSubscription> _newsLetterSubscriptionRepository;
    private readonly IRepository<StateProvince> _stateProvinceRepository;
    private readonly ISettingService _settingService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly IStoreContext _storeContext;
    private readonly ITaxService _taxService;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly OmnisendCustomerService _omnisendCustomerService;
    private readonly OmnisendHelper _omnisendHelper;
    private readonly OmnisendHttpClient _omnisendHttpClient;
    private readonly OmnisendSettings _omnisendSettings;

    #endregion

    #region Ctor

    public OmnisendService(ICategoryService categoryService,
        ICountryService countryService,
        ICustomerService customerService,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IRepository<Country> countryRepository,
        IRepository<Customer> customerRepository,
        IRepository<NewsLetterSubscription> newsLetterSubscriptionRepository,
        IRepository<StateProvince> stateProvinceRepository,
        ISettingService settingService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        ITaxService taxService,
        IWebHelper webHelper,
        IWorkContext workContext,
        OmnisendCustomerService omnisendCustomerService,
        OmnisendHelper omnisendHelper,
        OmnisendHttpClient omnisendHttpClient,
        OmnisendSettings omnisendSettings)
    {
		_logger.LogDebug(">>> Entering OmnisendService()");
        _categoryService = categoryService;
		_logger.LogDebug("OmnisendService(): _categoryService → {_categoryService}", _categoryService);
        _countryService = countryService;
		_logger.LogDebug("OmnisendService(): _countryService → {_countryService}", _countryService);
        _customerService = customerService;
		_logger.LogDebug("OmnisendService(): _customerService → {_customerService}", _customerService);
        _orderService = orderService;
		_logger.LogDebug("OmnisendService(): _orderService → {_orderService}", _orderService);
        _orderTotalCalculationService = orderTotalCalculationService;
		_logger.LogDebug("OmnisendService(): _orderTotalCalculationService → {_orderTotalCalculationService}", _orderTotalCalculationService);
        _productAttributeService = productAttributeService;
		_logger.LogDebug("OmnisendService(): _productAttributeService → {_productAttributeService}", _productAttributeService);
        _productService = productService;
		_logger.LogDebug("OmnisendService(): _productService → {_productService}", _productService);
        _countryRepository = countryRepository;
		_logger.LogDebug("OmnisendService(): _countryRepository → {_countryRepository}", _countryRepository);
        _customerRepository = customerRepository;
		_logger.LogDebug("OmnisendService(): _customerRepository → {_customerRepository}", _customerRepository);
        _newsLetterSubscriptionRepository = newsLetterSubscriptionRepository;
		_logger.LogDebug("OmnisendService(): _newsLetterSubscriptionRepository → {_newsLetterSubscriptionRepository}", _newsLetterSubscriptionRepository);
        _stateProvinceRepository = stateProvinceRepository;
		_logger.LogDebug("OmnisendService(): _stateProvinceRepository → {_stateProvinceRepository}", _stateProvinceRepository);
        _settingService = settingService;
		_logger.LogDebug("OmnisendService(): _settingService → {_settingService}", _settingService);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("OmnisendService(): _shoppingCartService → {_shoppingCartService}", _shoppingCartService);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("OmnisendService(): _stateProvinceService → {_stateProvinceService}", _stateProvinceService);
        _storeContext = storeContext;
		_logger.LogDebug("OmnisendService(): _storeContext → {_storeContext}", _storeContext);
        _taxService = taxService;
		_logger.LogDebug("OmnisendService(): _taxService → {_taxService}", _taxService);
        _webHelper = webHelper;
		_logger.LogDebug("OmnisendService(): _webHelper → {_webHelper}", _webHelper);
        _workContext = workContext;
		_logger.LogDebug("OmnisendService(): _workContext → {_workContext}", _workContext);
        _omnisendCustomerService = omnisendCustomerService;
		_logger.LogDebug("OmnisendService(): _omnisendCustomerService → {_omnisendCustomerService}", _omnisendCustomerService);
        _omnisendHelper = omnisendHelper;
		_logger.LogDebug("OmnisendService(): _omnisendHelper → {_omnisendHelper}", _omnisendHelper);
        _omnisendHttpClient = omnisendHttpClient;
		_logger.LogDebug("OmnisendService(): _omnisendHttpClient → {_omnisendHttpClient}", _omnisendHttpClient);
        _omnisendSettings = omnisendSettings;
		_logger.LogDebug("<<< Exiting OmnisendService()");
		_logger.LogDebug("OmnisendService(): _omnisendSettings → {_omnisendSettings}", _omnisendSettings);
    }

    #endregion

    #region Utilities

    private async Task<string> SendBatchAsync(string data)
    {
		_logger.LogDebug(">>> Entering SendBatchAsync(data={Data})", data);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

		_logger.LogDebug("SendBatchAsync(data={Data}): rez → {Rez}", data, rez);
        if (rez == null)
            return string.Empty;

        var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;
		_logger.LogDebug("SendBatchAsync(data={Data}): bathId → {BathId}", data, bathId);

        if (bathId == null)
            return string.Empty;

        _omnisendSettings.BatchesIds.Add(bathId);
        await _settingService.SaveSettingAsync(_omnisendSettings);

		_logger.LogInformation("SendBatchAsync(data)={Data}: external service call took {Arg1} ms", data, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting SendBatchAsync(data={Data})", data);
        return bathId;
    }

    private async Task<string> SendProductBatchAsync(IEnumerable<Product> products, HttpMethod batchMethod)
    {
		_logger.LogDebug(">>> Entering SendProductBatchAsync(products={Products},batchMethod={BatchMethod})", products, batchMethod);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var data = JsonConvert.SerializeObject(new BatchRequest
        {
            Method = batchMethod.Method.ToUpper(),
            Endpoint = OmnisendDefaults.ProductsEndpoint,
            Items = await products.SelectAwait(async product => await ProductToDtoAsync(product) as IBatchSupport).ToListAsync()
        });
		_logger.LogInformation("SendProductBatchAsync(products,batchMethod)={Products},{BatchMethod}: database query executed in {Arg2} ms", products, batchMethod, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting SendProductBatchAsync(products={Products},batchMethod={BatchMethod})", products, batchMethod);
        return await SendBatchAsync(data);
    }

    /// <summary>
    /// Process the batch response
    /// </summary>
    /// <param name="batchResponse">Batch response to process</param>
    /// <returns>New batch identifier if it placed during process the current one</returns>
    public async Task<string> ProcessBatch(BatchResponse batchResponse)
    {
		_logger.LogDebug(">>> Entering ProcessBatch(batchResponse={BatchResponse})", batchResponse);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var (delete, batchId) = await process();

        if (!delete)
            return batchId;

        _omnisendSettings.BatchesIds.Remove(batchResponse.BatchId);
        await _settingService.SaveSettingAsync(_omnisendSettings);

        return batchId;

        async Task<(bool, string)> process()
        {
            var newBatchId = string.Empty;

            if (!batchResponse.Status.Equals(OmnisendDefaults.BatchFinishedStatus, StringComparison.InvariantCultureIgnoreCase))
            {
                batchResponse.ErrorsCount = 0;
				_logger.LogDebug("ProcessBatch(batchResponse={BatchResponse}): batchResponse.ErrorsCount → {Arg1}", batchResponse, batchResponse.ErrorsCount);

                return (false, newBatchId);
            }

            var endpoint = batchResponse.Endpoint;

            if (!endpoint.Equals(OmnisendDefaults.ProductsEndpoint, StringComparison.InvariantCultureIgnoreCase) &&
                !endpoint.Equals(OmnisendDefaults.CategoriesEndpoint, StringComparison.InvariantCultureIgnoreCase))
                return (true, newBatchId);

            if (batchResponse.ErrorsCount == 0)
                return (true, newBatchId);

            if (batchResponse.Method.Equals("PUT", StringComparison.InvariantCultureIgnoreCase))
                return (false, newBatchId);

            var url = OmnisendDefaults.BatchesApiUrl + $"/{batchResponse.BatchId}/items";

            var result = await _omnisendHttpClient.PerformRequestAsync<BatchItemsResponse>(url, httpMethod: HttpMethod.Get);

            var updateItems = new List<int>();

            foreach (var resultError in result.Errors)
            {
                if (resultError.ResponseCode != 400)
                    continue;

                var needChangeCount = false;

                if (endpoint.Equals(OmnisendDefaults.ProductsEndpoint, StringComparison.InvariantCultureIgnoreCase))
                {
                    var productDto = JsonConvert.DeserializeObject<ProductDto>(resultError.Request?.ToString() ?? string.Empty);

                    updateItems.Add(int.Parse(productDto.ProductId));
                    needChangeCount = true;
                }

                if (endpoint.Equals(OmnisendDefaults.CategoriesEndpoint, StringComparison.InvariantCultureIgnoreCase))
                {
                    var categoryDto = JsonConvert.DeserializeObject<CategoryDto>(resultError.Request?.ToString() ?? string.Empty);

                    updateItems.Add(int.Parse(categoryDto.CategoryId));
                    needChangeCount = true;
                }

                if (!needChangeCount)
                    continue;

                batchResponse.TotalCount--;
                batchResponse.ErrorsCount--;
            }

            if (!updateItems.Any())
                return (false, newBatchId);

            if (endpoint.Equals(OmnisendDefaults.ProductsEndpoint, StringComparison.InvariantCultureIgnoreCase))
                newBatchId = await UpdateProductsAsync(updateItems.ToArray());

            if (endpoint.Equals(OmnisendDefaults.CategoriesEndpoint, StringComparison.InvariantCultureIgnoreCase))
                await UpdateCategoriesAsync(updateItems.ToArray());

			_logger.LogInformation("ProcessBatch(batchResponse)={BatchResponse}: external service call {Endpoint} took {Arg2} ms", batchResponse, endpoint, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting ProcessBatch(batchResponse={BatchResponse})", batchResponse);
            return (batchResponse.ErrorsCount == 0, newBatchId);
        }
		_logger.LogDebug(">>> Entering ProductToDtoAsync(product={Product})", product);
    }

    private async Task<List<BatchResponse>> GetBatchesInfoAsync(IList<string> batchesIds)
    {
		_logger.LogDebug(">>> Entering GetBatchesInfoAsync(batchesIds={BatchesIds})", batchesIds);
        if (!batchesIds.Any())
            return [];

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var batches = await batchesIds.SelectAwait(GetBatchInfoAsync)
            .ToListAsync();

		_logger.LogDebug("GetBatchesInfoAsync(batchesIds={BatchesIds}): batches → {Batches}", batchesIds, batches);
		_logger.LogInformation("GetBatchesInfoAsync(batchesIds)={BatchesIds}: database query executed in {Arg1} ms", batchesIds, __logStart.ElapsedMilliseconds);
        batches = batches.Where(p => p != null).ToList();

        if (batches.Any())
            return batches;

        _omnisendSettings.BatchesIds.Clear();
        await _settingService.SaveSettingAsync(_omnisendSettings);

		_logger.LogDebug("<<< Exiting GetBatchesInfoAsync(batchesIds={BatchesIds})", batchesIds);
        return batches;
    }

    private async Task FillCustomerInfoAsync(BaseContactInfoDto dto, Customer customer)
    {
		_logger.LogDebug(">>> Entering FillCustomerInfoAsync(dto={Dto},customer={Customer})", dto, customer);
        if (customer == null)
            return;

        dto.FirstName = customer.FirstName;
		_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): dto.FirstName → {Arg2}", dto, customer, dto.FirstName);
		_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): dto.LastName → {Arg2}", dto, customer, dto.LastName);
        dto.LastName = customer.LastName;

        var country = await _countryService.GetCountryByIdAsync(customer.CountryId);
		_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): country → {Country}", dto, customer, country);

        if (country != null)
        {
            dto.Country = country.Name;
			_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): dto.Country → {Arg2}", dto, customer, dto.Country);
			_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): dto.CountryCode → {Arg2}", dto, customer, dto.CountryCode);
            dto.CountryCode = country.TwoLetterIsoCode;
        }

        var state = await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId);
		_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): state → {State}", dto, customer, state);

        if (state != null)
            dto.State = state.Name;
			_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): dto.State → {Arg2}", dto, customer, dto.State);

        dto.City = customer.City;
		_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): dto.City → {Arg2}", dto, customer, dto.City);
		_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): dto.Address → {Arg2}", dto, customer, dto.Address);
		_logger.LogDebug("FillCustomerInfoAsync(dto={Dto},customer={Customer}): dto.PostalCode → {Arg2}", dto, customer, dto.PostalCode);
        dto.Address = customer.StreetAddress;
        dto.PostalCode = customer.ZipPostalCode;
        dto.Gender = customer.Gender?.ToLower() ?? "f";
        dto.BirthDate = customer.DateOfBirth?.ToString("yyyy-MM-dd");
		_logger.LogDebug("<<< Exiting FillCustomerInfoAsync(dto={Dto},customer={Customer})", dto, customer);
    }

    private async Task<OrderDto.OrderItemDto> OrderItemToDtoAsync(OrderItem orderItem)
    {
		_logger.LogDebug(">>> Entering OrderItemToDtoAsync(orderItem={OrderItem})", orderItem);
        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

		_logger.LogDebug("OrderItemToDtoAsync(orderItem={OrderItem}): product → {Product}", orderItem, product);
        var (sku, variantId) = await _omnisendHelper.GetSkuAndVariantIdAsync(product, orderItem.AttributesXml);

        var dto = new OrderDto.OrderItemDto
        {
            ProductId = orderItem.ProductId.ToString(),
            Sku = sku,
            VariantId = variantId.ToString(),
            Title = product.Name,
            Quantity = orderItem.Quantity,
            Price = orderItem.PriceInclTax.ToCents()
        };

		_logger.LogDebug("<<< Exiting OrderItemToDtoAsync(orderItem={OrderItem})", orderItem);
        return dto;
    }

    private async Task<ProductDto> ProductToDtoAsync(Product product)
    {
        async Task<List<string>> getProductCategories()
        {
			_logger.LogDebug(">>> Entering getProductCategories()");
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);

			_logger.LogDebug("getProductCategories(): productCategories → {ProductCategories}", productCategories);
			_logger.LogDebug("<<< Exiting getProductCategories()");
            return productCategories.Select(pc => pc.CategoryId.ToString()).ToList();
        }

        async Task<IList<ProductAttributeCombination>> getProductCombinations()
        {
			_logger.LogDebug(">>> Entering getProductCombinations()");
			_logger.LogDebug("<<< Exiting getProductCombinations()");
            return await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
        }

        var combinations = await getProductCombinations();

        async Task<string> getProductStatus(ProductAttributeCombination productAttributeCombination = null)
        {
			_logger.LogDebug(">>> Entering getProductStatus(productAttributeCombination={ProductAttributeCombination})", productAttributeCombination);
            var status = "notAvailable";

			_logger.LogDebug("getProductStatus(productAttributeCombination={ProductAttributeCombination}): status → {Status}", productAttributeCombination, status);
            if (!product.Published || product.Deleted)
                return status;

            int stockQuantity;

            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    stockQuantity = await _productService.GetTotalStockQuantityAsync(product);

					_logger.LogDebug("getProductStatus(productAttributeCombination={ProductAttributeCombination}): stockQuantity → {StockQuantity}", productAttributeCombination, stockQuantity);
                    if (stockQuantity > 0 || product.BackorderMode == BackorderMode.AllowQtyBelow0)
                        status = "inStock";
                    else
                        status = "outOfStock";

                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    if (productAttributeCombination == null)
                        return combinations.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders) ? "inStock" : "outOfStock";

                    stockQuantity = productAttributeCombination.StockQuantity;

                    if (stockQuantity > 0 || productAttributeCombination.AllowOutOfStockOrders)
                        status = "inStock";
                    else
                        status = "outOfStock";

                    break;
                case ManageInventoryMethod.DontManageStock:
                    status = "inStock";
                    break;
            }

			_logger.LogDebug("<<< Exiting getProductStatus(productAttributeCombination={ProductAttributeCombination})", productAttributeCombination);
            return status;
        }

        var dto = new ProductDto
        {
            ProductId = product.Id.ToString(),
            Title = product.Name,
            Status = await getProductStatus(),
            Description = product.ShortDescription,
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            ProductUrl = await _omnisendHelper.GetProductUrlAsync(product),
            Images = new List<ProductDto.Image>
            {
                await _omnisendHelper.GetProductPictureUrlAsync(product),
            },
            CreatedAt = product.CreatedOnUtc.ToDtoString(),
            UpdatedAt = product.UpdatedOnUtc.ToDtoString(),
            CategoryIDs = await getProductCategories(),
            Variants = new List<ProductDto.Variant>
            {
                new()
                {
                    VariantId = product.Id.ToString(),
                    Title = product.Name,
                    Sku = product.Sku,
                    Status = await getProductStatus(),
                    Price = product.Price.ToCents()
                }
            }
        };

        if (combinations.Any())
            dto.Variants.AddRange(await combinations.SelectAwait(async c => new ProductDto.Variant
            {
                VariantId = c.Id.ToString(),
                Title = product.Name,
                Sku = c.Sku,
                Status = await getProductStatus(c),
                Price = (c.OverriddenPrice ?? product.Price).ToCents()
            }).ToListAsync());

        return dto;
    }

    private async Task<OrderDto> OrderToDtoAsync(Order order)
    {
		_logger.LogDebug(">>> Entering OrderToDtoAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        var email = await _omnisendCustomerService.GetEmailAsync(customer, order.BillingAddressId);
		_logger.LogDebug("OrderToDtoAsync(order={Order}): email → {Email}", order, email);
		_logger.LogDebug("OrderToDtoAsync(order={Order}): customer → {Customer}", order, customer);
        if (string.IsNullOrEmpty(email))
            return null;

        var dto = new OrderDto
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        {
            OrderId = order.OrderGuid.ToString(),
            Email = email,
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            OrderSum = order.OrderTotal.ToCents(),
            SubTotalSum = order.OrderSubtotalInclTax.ToCents(),
            DiscountSum = order.OrderDiscount.ToCents(),
            TaxSum = order.OrderTax.ToCents(),
            ShippingSum = order.OrderShippingInclTax.ToCents(),
            CreatedAt = order.CreatedOnUtc.ToDtoString(),
            Products = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async oi => await OrderItemToDtoAsync(oi)).ToListAsync()
        };
		_logger.LogInformation("OrderToDtoAsync(order)={Order}: find query executed in {Arg1} ms", order, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting OrderToDtoAsync(order={Order})", order);
        return dto;
    }

    private async Task<CartItemDto> ShoppingCartItemToDtoAsync(ShoppingCartItem shoppingCartItem, Customer customer)
    {
		_logger.LogDebug(">>> Entering ShoppingCartItemToDtoAsync(shoppingCartItem=<redacted>,customer={Customer})", customer);
        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

		_logger.LogDebug("ShoppingCartItemToDtoAsync(shoppingCartItem=<redacted>,customer={Customer}): product → {Product}", customer, product);
        var (sku, variantId) = await _omnisendHelper.GetSkuAndVariantIdAsync(product, shoppingCartItem.AttributesXml);

        var (scSubTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(shoppingCartItem, true);

        var dto = new CartItemDto
        {
            CartProductId = shoppingCartItem.Id.ToString(),
            ProductId = shoppingCartItem.ProductId.ToString(),
            Sku = sku,
            VariantId = variantId.ToString(),
            Title = product.Name,
            Quantity = shoppingCartItem.Quantity,
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            Price = (await _taxService.GetProductPriceAsync(product, scSubTotal, true, customer)).price.ToCents()
        };

		_logger.LogDebug("<<< Exiting ShoppingCartItemToDtoAsync(shoppingCartItem=<redacted>,customer={Customer})", customer);
        return dto;
    }

    private CategoryDto CategoryToDto(Category category)
    {
		_logger.LogDebug(">>> Entering CategoryToDto(category={Category})", category);
		_logger.LogDebug("<<< Exiting CategoryToDto(category={Category})", category);
        return new CategoryDto
        {
            CategoryId = category.Id.ToString(),
            Title = category.Name,
            CreatedAt = category.CreatedOnUtc.ToDtoString()
        };
    }

    private async Task<CartDto> GetCartDtoAsync(IList<ShoppingCartItem> cart)
    {
		_logger.LogDebug(">>> Entering GetCartDtoAsync(cart={Cart})", cart);
        if (!cart.Any())
            return null;

        var customerId = cart.First().CustomerId;
		_logger.LogDebug("GetCartDtoAsync(cart={Cart}): customerId → {CustomerId}", cart, customerId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
		_logger.LogDebug("GetCartDtoAsync(cart={Cart}): customer → {Customer}", cart, customer);

        var items = await cart
            .SelectAwait(async ci => await ShoppingCartItemToDtoAsync(ci, customer))
            .ToListAsync();
			_logger.LogInformation("GetCartDtoAsync(cart)={Cart}: database query executed in {Arg1} ms", cart, __logStart.ElapsedMilliseconds);

        var cartSum = (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal
            ?.ToCents() ?? 0;

        var email = await _omnisendCustomerService.GetEmailAsync(customer);
		_logger.LogDebug("GetCartDtoAsync(cart={Cart}): email → {Email}", cart, email);
        if (string.IsNullOrEmpty(email))
            return null;

        var cartId = await _omnisendCustomerService.GetCartIdAsync(customer);
		_logger.LogDebug("GetCartDtoAsync(cart={Cart}): cartId → {CartId}", cart, cartId);

		_logger.LogDebug("<<< Exiting GetCartDtoAsync(cart={Cart})", cart);
        return new CartDto
        {
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            Email = email,
            CartId = cartId,
            CartSum = cartSum,
            Products = items,
            CartRecoveryUrl = _omnisendCustomerService.GetAbandonedCheckoutUrl(cartId)
        };
    }

    private async Task CreateOrderAsync(Order order)
    {
		_logger.LogDebug(">>> Entering CreateOrderAsync(order={Order})", order);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var orderDto = await OrderToDtoAsync(order);
        if (orderDto is null)
            return;

        var data = JsonConvert.SerializeObject(orderDto);
		_logger.LogDebug("CreateOrderAsync(order={Order}): data → {Data}", order, data);
		_logger.LogInformation("CreateOrderAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.OrdersApiUrl, data, HttpMethod.Post);
    }

    private async Task CreateCartAsync(IList<ShoppingCartItem> cart)
    {
		_logger.LogDebug(">>> Entering CreateCartAsync(cart={Cart})", cart);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cartDto = await GetCartDtoAsync(cart);
        if (cartDto is null)
            return;

        var data = JsonConvert.SerializeObject(cartDto);
		_logger.LogDebug("CreateCartAsync(cart={Cart}): data → {Data}", cart, data);
		_logger.LogInformation("CreateCartAsync(cart)={Cart}: external service call took {Arg1} ms", cart, __logStart.ElapsedMilliseconds);
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CartsApiUrl, data, HttpMethod.Post);
    }

    private async Task UpdateCartAsync(Customer customer, ShoppingCartItem newItem)
    {
		_logger.LogDebug(">>> Entering UpdateCartAsync(customer={Customer},newItem={NewItem})", customer, newItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var item = await ShoppingCartItemToDtoAsync(newItem, customer);
        if (item is null)
            return;

        var data = JsonConvert.SerializeObject(item);
		_logger.LogDebug("UpdateCartAsync(customer={Customer},newItem={NewItem}): data → {Data}", customer, newItem, data);
		_logger.LogInformation("UpdateCartAsync(customer,newItem)={Customer},{NewItem}: external service call took {Arg2} ms", customer, newItem, __logStart.ElapsedMilliseconds);
        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}", data, HttpMethod.Post);
    }

    private async Task<bool> CanSendRequestAsync(Customer customer)
    {
		_logger.LogDebug(">>> Entering CanSendRequestAsync(customer={Customer})", customer);
		_logger.LogDebug("<<< Exiting CanSendRequestAsync(customer={Customer})", customer);
        return !string.IsNullOrEmpty(
            await _omnisendCustomerService.GetEmailAsync(customer, customer.BillingAddressId));
    }

    /// <summary>
    /// Prepare newsletter subscribers to sync
    /// </summary>
    /// <param name="storeId">Store identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sendWelcomeMessage">Specifies whether to send a welcome message</param>
    /// <param name="subscriber">Newsletter subscription to filter</param>
    /// <param name="inactiveStatus">Inactive status</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of subscriber data
    /// </returns>
    private async Task<List<IBatchSupport>> PrepareNewsletterSubscribersAsync(int storeId,
        int pageIndex, int pageSize,
        bool sendWelcomeMessage = false, NewsLetterSubscription subscriber = null, string inactiveStatus = "nonSubscribed")
    {
		_logger.LogDebug(">>> Entering PrepareNewsletterSubscribersAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},sendWelcomeMessage={SendWelcomeMessage},subscriber={Subscriber},inactiveStatus={InactiveStatus})", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus);
        //get contacts of newsletter subscribers
        var subscriptions = (subscriber == null ? _newsLetterSubscriptionRepository.Table : _newsLetterSubscriptionRepository.Table.Where(nlsr => nlsr.Id.Equals(subscriber.Id)))
            .Where(subscription => subscription.StoreId == storeId)
            .OrderBy(subscription => subscription.Id)
            .Select(subscription => new{ subscription.Email, subscription.Active, subscription.CreatedOnUtc })
            .Distinct()
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

        var contacts = from item in subscriptions
            join c in _customerRepository.Table on item.Email equals c.Email
                into temp
            from c in temp.DefaultIfEmpty()
            where c == null || (c.Active && !c.Deleted)
            select new { subscription = item, customer = c };

        var contactsWithCountry = from item in contacts
            join cr in _countryRepository.Table on item.customer.CountryId equals cr.Id
                into temp
            from cr in temp.DefaultIfEmpty()
            select new { item.customer, item.subscription, country = cr };

        var contactsWithState = from item in contactsWithCountry
            join sp in _stateProvinceRepository.Table on item.customer.StateProvinceId equals sp.Id
                into temp
            from sp in temp.DefaultIfEmpty()
            select new
            {
                item.subscription,
                item.customer.FirstName,
                item.customer.LastName,
                CountryName = item.country.Name,
                CountryTwoLetterIsoCode = item.country.TwoLetterIsoCode,
                StateProvinceName = sp.Name,
                item.customer.City,
                item.customer.StreetAddress,
                item.customer.ZipPostalCode,
                item.customer.Gender,
                item.customer.DateOfBirth
				_logger.LogDebug("PrepareNewsletterSubscribersAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},sendWelcomeMessage={SendWelcomeMessage},subscriber={Subscriber},inactiveStatus={InactiveStatus}): StateProvinceName → {StateProvinceName}", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus, StateProvinceName);
				_logger.LogDebug("PrepareNewsletterSubscribersAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},sendWelcomeMessage={SendWelcomeMessage},subscriber={Subscriber},inactiveStatus={InactiveStatus}): CountryTwoLetterIsoCode → {CountryTwoLetterIsoCode}", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus, CountryTwoLetterIsoCode);
				_logger.LogDebug("PrepareNewsletterSubscribersAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},sendWelcomeMessage={SendWelcomeMessage},subscriber={Subscriber},inactiveStatus={InactiveStatus}): CountryName → {CountryName}", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus, CountryName);
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
            };

        var subscribers = (await contactsWithState.ToListAsync()).Select(item =>
        {
            var dto = new CreateContactRequest(item.subscription.Email, item.subscription.Active, item.subscription.CreatedOnUtc, inactiveStatus, sendWelcomeMessage)
            {
                FirstName = item.FirstName,
                LastName = item.LastName,
                City = item.City,
                Address = item.StreetAddress,
                PostalCode = item.ZipPostalCode,
                Gender = item.Gender?.ToLower(),
                BirthDate = item.DateOfBirth?.ToString("yyyy-MM-dd")
            };
			_logger.LogInformation("PrepareNewsletterSubscribersAsync(storeId,pageIndex,pageSize,sendWelcomeMessage,subscriber,inactiveStatus)={StoreId},{PageIndex},{PageSize},{SendWelcomeMessage},{Subscriber},{InactiveStatus}: find query executed in {Arg6} ms", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus, __logStart.ElapsedMilliseconds);

            if (!string.IsNullOrEmpty(item.CountryName))
                dto.Country = item.CountryName;
				_logger.LogDebug("PrepareNewsletterSubscribersAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},sendWelcomeMessage={SendWelcomeMessage},subscriber={Subscriber},inactiveStatus={InactiveStatus}): dto.Country → {Arg6}", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus, dto.Country);

            if (!string.IsNullOrEmpty(item.CountryTwoLetterIsoCode))
                dto.CountryCode = item.CountryTwoLetterIsoCode;
				_logger.LogDebug("PrepareNewsletterSubscribersAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},sendWelcomeMessage={SendWelcomeMessage},subscriber={Subscriber},inactiveStatus={InactiveStatus}): dto.CountryCode → {Arg6}", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus, dto.CountryCode);

            if (!string.IsNullOrEmpty(item.StateProvinceName))
                dto.State = item.StateProvinceName;
				_logger.LogDebug("PrepareNewsletterSubscribersAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},sendWelcomeMessage={SendWelcomeMessage},subscriber={Subscriber},inactiveStatus={InactiveStatus}): dto.State → {Arg6}", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus, dto.State);

            return (IBatchSupport)dto;
        }).ToList();

		_logger.LogDebug("<<< Exiting PrepareNewsletterSubscribersAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},sendWelcomeMessage={SendWelcomeMessage},subscriber={Subscriber},inactiveStatus={InactiveStatus})", storeId, pageIndex, pageSize, sendWelcomeMessage, subscriber, inactiveStatus);
        return subscribers;
    }

    #endregion

    #region Methods

    #region Sync methods

    /// <summary>
    /// Synchronize contacts
    /// </summary>
    public async Task SyncContactsAsync()
    {
		_logger.LogDebug(">>> Entering SyncContactsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
        var su = await PrepareNewsletterSubscribersAsync(store.Id, 0, _omnisendSettings.PageSize);
		_logger.LogDebug("SyncContactsAsync(): store → {Store}", store);
		_logger.LogDebug("SyncContactsAsync(): su → {Su}", su);

        if (su.Count >= OmnisendDefaults.MinCountToUseBatch)
        {
            var page = 0;

            while (true)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.ContactsEndpoint,
                    Items = su
                });

                var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);
				_logger.LogDebug("SyncContactsAsync(): rez → {Rez}", rez);

                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;
				_logger.LogDebug("SyncContactsAsync(): bathId → {BathId}", bathId);

                if (bathId != null)
                {
                    _omnisendSettings.BatchesIds.Add(bathId);
                    await _settingService.SaveSettingAsync(_omnisendSettings);
                }

                page++;

                su = await PrepareNewsletterSubscribersAsync(store.Id, page, _omnisendSettings.PageSize);

                if (!su.Any())
                    break;
            }
        }
        else
            foreach (var newsLetterSubscription in su)
                await UpdateOrCreateContactAsync(newsLetterSubscription as CreateContactRequest);
				_logger.LogInformation("SyncContactsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
				_logger.LogDebug("<<< Exiting SyncContactsAsync()");
    }

    /// <summary>
    /// Synchronize categories
    /// </summary>
    public async Task SyncCategoriesAsync()
    {
		_logger.LogDebug(">>> Entering SyncCategoriesAsync()");
        var categories = await _categoryService.GetAllCategoriesAsync(null, pageSize: _omnisendSettings.PageSize);

		_logger.LogDebug("SyncCategoriesAsync(): categories → {Categories}", categories);
        if (categories.TotalCount >= OmnisendDefaults.MinCountToUseBatch || categories.TotalCount > _omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < categories.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
					_logger.LogDebug("SyncCategoriesAsync(): data → {Data}", data);
                    Endpoint = OmnisendDefaults.CategoriesEndpoint,
                    Items = categories.Select(category => CategoryToDto(category) as IBatchSupport).ToList()
                });

                await SendBatchAsync(data);

                page++;

                categories = await _categoryService.GetAllCategoriesAsync(null, pageIndex: page, pageSize: _omnisendSettings.PageSize);
            }
        }
        else
            foreach (var category in categories)
            {
                var data = JsonConvert.SerializeObject(CategoryToDto(category));
                await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CategoriesApiUrl, data, HttpMethod.Post);
            }
			_logger.LogDebug(">>> Entering UpdateOrCreateContactAsync(request={Request})", request);
			_logger.LogDebug("<<< Exiting UpdateOrCreateContactAsync(request={Request})", request);
    }

    /// <summary>
    /// Synchronize categories
    /// </summary>
    /// <param name="categoriesId">Categories identifiers list to update</param>
    public async Task UpdateCategoriesAsync(int[] categoriesId)
    {
		_logger.LogDebug(">>> Entering UpdateCategoriesAsync(categoriesId={CategoriesId})", categoriesId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var categories = await _categoryService.GetCategoriesByIdsAsync(categoriesId);

		_logger.LogDebug("UpdateCategoriesAsync(categoriesId={CategoriesId}): categories → {Categories}", categoriesId, categories);
        foreach (var category in categories)
        {
            var data = JsonConvert.SerializeObject(CategoryToDto(category));
			_logger.LogDebug("UpdateCategoriesAsync(categoriesId={CategoriesId}): data → {Data}", categoriesId, data);
            await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CategoriesApiUrl + $"/{category.Id}", data, HttpMethod.Put);
        }
		_logger.LogInformation("UpdateCategoriesAsync(categoriesId)={CategoriesId}: external service call took {Arg1} ms", categoriesId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting UpdateCategoriesAsync(categoriesId={CategoriesId})", categoriesId);
    }

    /// <summary>
    /// Synchronize products
    /// </summary>
    public async Task SyncProductsAsync()
    {
		_logger.LogDebug(">>> Entering SyncProductsAsync()");
        var products = await _productService.SearchProductsAsync(pageSize: _omnisendSettings.PageSize);

		_logger.LogDebug("SyncProductsAsync(): products → {Products}", products);
        if (products.TotalCount >= OmnisendDefaults.MinCountToUseBatch || products.TotalCount > _omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < products.TotalPages)
            {
                await SendProductBatchAsync(products, HttpMethod.Post);

                page++;

                products = await _productService.SearchProductsAsync(pageIndex: page, pageSize: _omnisendSettings.PageSize);
            }
        }
        else
            foreach (var product in products)
                await AddNewProductAsync(product);
				_logger.LogDebug("<<< Exiting SyncProductsAsync()");
    }

    /// <summary>
    /// Synchronize orders
    /// </summary>
    public async Task SyncOrdersAsync()
    {
		_logger.LogDebug(">>> Entering SyncOrdersAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var orders = await _orderService.SearchOrdersAsync(pageSize: _omnisendSettings.PageSize);

		_logger.LogDebug("SyncOrdersAsync(): orders → {Orders}", orders);
        if (orders.TotalCount >= OmnisendDefaults.MinCountToUseBatch || orders.TotalCount > _omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < orders.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.OrdersEndpoint,
                    Items = await orders.SelectAwait(async order => await OrderToDtoAsync(order) as IBatchSupport).ToListAsync()
                });
				_logger.LogInformation("SyncOrdersAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

                var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);
				_logger.LogDebug("SyncOrdersAsync(): rez → {Rez}", rez);

                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;
				_logger.LogDebug("SyncOrdersAsync(): bathId → {BathId}", bathId);

                if (bathId != null)
                {
                    _omnisendSettings.BatchesIds.Add(bathId);
                    await _settingService.SaveSettingAsync(_omnisendSettings);
                }

                page++;

                orders = await _orderService.SearchOrdersAsync(pageIndex: page, pageSize: _omnisendSettings.PageSize);
            }
        }
        else
            foreach (var order in orders)
                await CreateOrderAsync(order);
				_logger.LogInformation("SyncOrdersAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
				_logger.LogDebug(">>> Entering AddNewProductAsync(product={Product})", product);
				_logger.LogDebug("<<< Exiting AddNewProductAsync(product={Product})", product);
    }

    /// <summary>
    /// Synchronize shopping carts
    /// </summary>
    public async Task SyncCartsAsync()
    {
		_logger.LogDebug(">>> Entering SyncCartsAsync()");
        var store = await _storeContext.GetCurrentStoreAsync();
        var customers = await _customerService.GetCustomersWithShoppingCartsAsync(ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogDebug("SyncCartsAsync(): customers → {Customers}", customers);
		_logger.LogDebug("SyncCartsAsync(): store → {Store}", store);
        foreach (var customer in customers)
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
			_logger.LogDebug("SyncCartsAsync(): cart → {Cart}", cart);
            await CreateCartAsync(cart);
        }
		_logger.LogDebug("<<< Exiting SyncCartsAsync()");
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Gets the stored batches
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the stored batches
    /// </returns>
    public async Task<IList<BatchResponse>> GetStoredBatchesAsync()
    {
		_logger.LogDebug(">>> Entering GetStoredBatchesAsync()");
        var batches = await GetBatchesInfoAsync(_omnisendSettings.BatchesIds);

		_logger.LogDebug("GetStoredBatchesAsync(): batches → {Batches}", batches);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var additionalBatches = await batches
            .Where(p => p.Status.Equals(OmnisendDefaults.BatchFinishedStatus,
                StringComparison.InvariantCultureIgnoreCase))
            .SelectAwait(async batchResponse => await ProcessBatch(batchResponse))
            .Where(newBatchId => !string.IsNullOrEmpty(newBatchId)).ToListAsync();
			_logger.LogInformation("GetStoredBatchesAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        
        batches.AddRange(await GetBatchesInfoAsync(additionalBatches));

		_logger.LogDebug("<<< Exiting GetStoredBatchesAsync()");
        return batches.Where(b => b.TotalCount > 0).ToList();
    }

    /// <summary>
    /// Gets the brand identifier 
    /// </summary>
    /// <param name="apiKey">API key to send request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the brand identifier or null
    /// </returns>
    public async Task<string> GetBrandIdAsync(string apiKey)
    {
		_logger.LogDebug(">>> Entering GetBrandIdAsync(apiKey=<redacted>)");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        _omnisendHttpClient.ApiKey = apiKey;

		_logger.LogDebug("GetBrandIdAsync(apiKey=<redacted>): _omnisendHttpClient.ApiKey → {Arg1}", _omnisendHttpClient.ApiKey);
        var result = await _omnisendHttpClient.PerformRequestAsync<AccountsResponse>(OmnisendDefaults.AccountsApiUrl);
		_logger.LogDebug("GetBrandIdAsync(apiKey=<redacted>): result → {Result}", result);

		_logger.LogInformation("GetBrandIdAsync(apiKey)={ApiKey}: external service call took {Arg1} ms", apiKey, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetBrandIdAsync(apiKey=<redacted>)");
        return result?.BrandId;
    }

    /// <summary>
    /// Registers the site on the omnisend service
    /// </summary>
    public async Task SendCustomerDataAsync()
    {
		_logger.LogDebug(">>> Entering SendCustomerDataAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var site = _webHelper.GetStoreLocation();

		_logger.LogDebug("SendCustomerDataAsync(): site → {Site}", site);
        var data = JsonConvert.SerializeObject(new
        {
            website = site,
            platform = OmnisendDefaults.IntegrationOrigin,
            version = OmnisendDefaults.IntegrationVersion
			_logger.LogDebug("SendCustomerDataAsync(): version → {Version}", version);
			_logger.LogDebug("SendCustomerDataAsync(): platform → {Platform}", platform);
			_logger.LogDebug("SendCustomerDataAsync(): website → {Website}", website);
			_logger.LogInformation("SendCustomerDataAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
        });

        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.AccountsApiUrl, data, HttpMethod.Post);
		_logger.LogDebug("<<< Exiting SendCustomerDataAsync()");
    }

    /// <summary>
    /// Gets information about batch
    /// </summary>
    /// <param name="bathId">Batch identifier</param>
    public async ValueTask<BatchResponse> GetBatchInfoAsync(string bathId)
    {
		_logger.LogDebug(">>> Entering GetBatchInfoAsync(bathId={BathId})", bathId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var url = OmnisendDefaults.BatchesApiUrl + $"/{bathId}";

		_logger.LogDebug("GetBatchInfoAsync(bathId={BathId}): url → {Url}", bathId, url);
        var result = await _omnisendHttpClient.PerformRequestAsync<BatchResponse>(url, httpMethod: HttpMethod.Get);
		_logger.LogDebug("GetBatchInfoAsync(bathId={BathId}): result → {Result}", bathId, result);

		_logger.LogInformation("GetBatchInfoAsync(bathId)={BathId}: external service call {Url} took {Arg2} ms", bathId, url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetBatchInfoAsync(bathId={BathId})", bathId);
        return result;
    }

    #endregion

    #region Contacts

    /// <summary>
    /// Gets the contacts information
    /// </summary>
    /// <param name="contactId">Contact identifier</param>
    public async Task<ContactInfoDto> GetContactInfoAsync(string contactId)
    {
		_logger.LogDebug(">>> Entering GetContactInfoAsync(contactId={ContactId})", contactId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var url = $"{OmnisendDefaults.ContactsApiUrl}/{contactId}";

		_logger.LogDebug("GetContactInfoAsync(contactId={ContactId}): url → {Url}", contactId, url);
        var res = await _omnisendHttpClient.PerformRequestAsync<ContactInfoDto>(url, httpMethod: HttpMethod.Get);
		_logger.LogDebug("GetContactInfoAsync(contactId={ContactId}): res → {Res}", contactId, res);

		_logger.LogInformation("GetContactInfoAsync(contactId)={ContactId}: external service call {Url} took {Arg2} ms", contactId, url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetContactInfoAsync(contactId={ContactId})", contactId);
        return res;
    }

    /// <summary>
    /// Update or create contact information
    /// </summary>
    /// <param name="request">Create contact request</param>
    public async Task UpdateOrCreateContactAsync(CreateContactRequest request)
    {
		_logger.LogDebug(">>> Entering UpdateOrCreateContactAsync(request={Request})", request);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var email = request.Identifiers.First().Id;
        var exists = !string.IsNullOrEmpty(await _omnisendCustomerService.GetContactIdAsync(email));
		_logger.LogDebug("UpdateOrCreateContactAsync(request={Request}): email → {Email}", request, email);
		_logger.LogDebug("UpdateOrCreateContactAsync(request={Request}): exists → {Exists}", request, exists);

        if (!exists)
        {
            var data = JsonConvert.SerializeObject(request);
			_logger.LogDebug("UpdateOrCreateContactAsync(request={Request}): data → {Data}", request, data);
            await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.ContactsApiUrl, data, HttpMethod.Post);
        }
        else
        {
            var url = $"{OmnisendDefaults.ContactsApiUrl}?email={email}";
            var data = JsonConvert.SerializeObject(new { identifiers = new[] { request.Identifiers.First() } });
			_logger.LogDebug("UpdateOrCreateContactAsync(request={Request}): url → {Url}", request, url);

            await _omnisendHttpClient.PerformRequestAsync(url, data, HttpMethod.Patch);
        }
		_logger.LogInformation("UpdateOrCreateContactAsync(request)={Request}: external service call {Url} took {Arg2} ms", request, url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting UpdateOrCreateContactAsync(request={Request})", request);
    }

    /// <summary>
    /// Update or create contact information
    /// </summary>
    /// <param name="subscription">Newsletter subscription</param>
    /// <param name="sendWelcomeMessage">Specifies whether to send a welcome message</param>
    public async Task UpdateOrCreateContactAsync(NewsLetterSubscription subscription, bool sendWelcomeMessage = false)
    {
		_logger.LogDebug(">>> Entering UpdateOrCreateContactAsync(subscription={Subscription},sendWelcomeMessage={SendWelcomeMessage})", subscription, sendWelcomeMessage);
        var su = await PrepareNewsletterSubscribersAsync(subscription.StoreId,
            0, _omnisendSettings.PageSize,
            sendWelcomeMessage, subscription, "unsubscribed");

        if (su.FirstOrDefault() is not CreateContactRequest request)
            return;

        await UpdateOrCreateContactAsync(request);
    }

    /// <summary>
    /// Updates contact information by customer data
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task UpdateContactAsync(Customer customer)
    {
		_logger.LogDebug(">>> Entering UpdateContactAsync(customer={Customer})", customer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var contactId = await _omnisendCustomerService.GetContactIdAsync(customer.Email);

		_logger.LogDebug("UpdateContactAsync(customer={Customer}): contactId → {ContactId}", customer, contactId);
        if (string.IsNullOrEmpty(contactId))
            return;

        var url = $"{OmnisendDefaults.ContactsApiUrl}/{contactId}";
		_logger.LogDebug("UpdateContactAsync(customer={Customer}): url → {Url}", customer, url);

        var dto = new BaseContactInfoDto();
		_logger.LogDebug("UpdateContactAsync(customer={Customer}): dto → {Dto}", customer, dto);

        await FillCustomerInfoAsync(dto, customer);

        var data = JsonConvert.SerializeObject(dto);
		_logger.LogDebug("UpdateContactAsync(customer={Customer}): data → {Data}", customer, data);

        await _omnisendHttpClient.PerformRequestAsync(url, data, HttpMethod.Patch);
		_logger.LogInformation("UpdateContactAsync(customer)={Customer}: external service call {Url} took {Arg2} ms", customer, url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting UpdateContactAsync(customer={Customer})", customer);
    }

    #endregion

    #region Products

    /// <summary>
    /// Adds new product
    /// </summary>
    /// <param name="product">Product to add</param>
    public async Task AddNewProductAsync(Product product)
    {
		_logger.LogDebug(">>> Entering AddNewProductAsync(product={Product})", product);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var data = JsonConvert.SerializeObject(await ProductToDtoAsync(product));
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.ProductsApiUrl, data, HttpMethod.Post);
		_logger.LogDebug("AddNewProductAsync(product={Product}): data → {Data}", product, data);
		_logger.LogInformation("AddNewProductAsync(product)={Product}: external service call took {Arg1} ms", product, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting AddNewProductAsync(product={Product})", product);
    }

    /// <summary>
    /// Updates the product
    /// </summary>
    /// <param name="productId">Product identifier to update</param>
    public async Task UpdateProductAsync(int productId)
    {
		_logger.LogDebug(">>> Entering UpdateProductAsync(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);

		_logger.LogDebug("UpdateProductAsync(productId={ProductId}): product → {Product}", productId, product);
        await CreateOrUpdateProductAsync(product);
		_logger.LogDebug("<<< Exiting UpdateProductAsync(productId={ProductId})", productId);
    }

    /// <summary>
    /// Updates products
    /// </summary>
    /// /// <param name="productsId">Products identifiers list to update</param>
    public async Task<string> UpdateProductsAsync(int[] productsId)
    {
		_logger.LogDebug(">>> Entering UpdateProductsAsync(productsId={ProductsId})", productsId);
        var products = await _productService.GetProductsByIdsAsync(productsId);

		_logger.LogDebug("UpdateProductsAsync(productsId={ProductsId}): products → {Products}", productsId, products);
		_logger.LogDebug("<<< Exiting UpdateProductsAsync(productsId={ProductsId})", productsId);
		_logger.LogDebug(">>> Entering CreateOrUpdateProductAsync(product={Product})", product);
		_logger.LogInformation("CreateOrUpdateProductAsync(product)={Product}: external service call took {Arg1} ms", product, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting CreateOrUpdateProductAsync(product={Product})", product);
        return await SendProductBatchAsync(products, HttpMethod.Put);
    }

    /// <summary>
    /// Updates the product
    /// </summary>
    /// <param name="product">Product to update</param>
    public async Task CreateOrUpdateProductAsync(Product product)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var result = await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.ProductsApiUrl}/{product.Id}", httpMethod: HttpMethod.Get);
        if (string.IsNullOrEmpty(result))
            await AddNewProductAsync(product);
        else
        {
            var data = JsonConvert.SerializeObject(await ProductToDtoAsync(product));
			_logger.LogDebug("CreateOrUpdateProductAsync(product={Product}): data → {Data}", product, data);
            await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.ProductsApiUrl}/{product.Id}", data, HttpMethod.Put);
        }
    }

    #endregion

    #region Shopping cart

    /// <summary>
    /// Restore the abandoned shopping cart
    /// </summary>
    /// <param name="cartId">Cart identifier</param>
    public async Task RestoreShoppingCartAsync(string cartId)
    {
		_logger.LogDebug(">>> Entering RestoreShoppingCartAsync(cartId={CartId})", cartId);
        var res = await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{cartId}", httpMethod: HttpMethod.Get);
		_logger.LogDebug("RestoreShoppingCartAsync(cartId={CartId}): res → {Res}", cartId, res);

        if (string.IsNullOrEmpty(res))
            return;

        var restoredCart = JsonConvert.DeserializeObject<CartDto>(res)
            ?? throw new NopException("Cart data can't be loaded");

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("RestoreShoppingCartAsync(cartId={CartId}): customer → {Customer}", cartId, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("RestoreShoppingCartAsync(cartId={CartId}): store → {Store}", cartId, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogDebug("RestoreShoppingCartAsync(cartId={CartId}): cart → {Cart}", cartId, cart);

        foreach (var cartProduct in restoredCart.Products)
        {
            var combination = await _productAttributeService.GetProductAttributeCombinationBySkuAsync(cartProduct.Sku);
			_logger.LogDebug("RestoreShoppingCartAsync(cartId={CartId}): combination → {Combination}", cartId, combination);
            var product = combination == null ? await _productService.GetProductBySkuAsync(cartProduct.Sku) : await _productService.GetProductByIdAsync(combination.ProductId);

			_logger.LogDebug("RestoreShoppingCartAsync(cartId={CartId}): product → {Product}", cartId, product);
            if (product == null)
            {
                if (!int.TryParse(cartProduct.VariantId, out var variantId))
                    continue;

                product = await _productService.GetProductByIdAsync(variantId);

                if (product == null)
                    continue;
            }

            var shoppingCartItem = cart.FirstOrDefault(item => item.ProductId.ToString() == cartProduct.ProductId &&
                item.Quantity == cartProduct.Quantity &&
                item.AttributesXml == combination?.AttributesXml);
            if (shoppingCartItem is not null)
                continue;

            await _shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.ShoppingCart, store.Id,
                combination?.AttributesXml, quantity: cartProduct.Quantity);
        }
		_logger.LogInformation("RestoreShoppingCartAsync(cartId)={CartId}: external service call took {Arg1} ms", cartId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting RestoreShoppingCartAsync(cartId={CartId})", cartId);
    }

    /// <summary>
    /// Adds new item to the shopping cart
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task AddShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
		_logger.LogDebug(">>> Entering AddShoppingCartItemAsync(shoppingCartItem=<redacted>)");
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

		_logger.LogDebug("AddShoppingCartItemAsync(shoppingCartItem=<redacted>): customer → {Customer}", customer);
        if (!await CanSendRequestAsync(customer))
            return;

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, shoppingCartItem.StoreId);
		_logger.LogDebug("AddShoppingCartItemAsync(shoppingCartItem=<redacted>): cart → {Cart}", cart);

        if (cart.Count == 1)
            await CreateCartAsync(cart);
        else
            await UpdateCartAsync(customer, shoppingCartItem);
			_logger.LogDebug("<<< Exiting AddShoppingCartItemAsync(shoppingCartItem=<redacted>)");
    }

    /// <summary>
    /// Updates the shopping cart item
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task EditShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
		_logger.LogDebug(">>> Entering EditShoppingCartItemAsync(shoppingCartItem=<redacted>)");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

		_logger.LogDebug("EditShoppingCartItemAsync(shoppingCartItem=<redacted>): customer → {Customer}", customer);
        if (!await CanSendRequestAsync(customer))
            return;

        var item = await ShoppingCartItemToDtoAsync(shoppingCartItem, customer);
		_logger.LogDebug("EditShoppingCartItemAsync(shoppingCartItem=<redacted>): item → {Item}", item);

        var data = JsonConvert.SerializeObject(item);
		_logger.LogDebug("EditShoppingCartItemAsync(shoppingCartItem=<redacted>): data → {Data}", data);

        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}/{item.CartProductId}", data, HttpMethod.Put);
		_logger.LogInformation("EditShoppingCartItemAsync(shoppingCartItem)={ShoppingCartItem}: external service call took {Arg1} ms", shoppingCartItem, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting EditShoppingCartItemAsync(shoppingCartItem=<redacted>)");
    }

    /// <summary>
    /// Deletes item from shopping cart
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
		_logger.LogDebug(">>> Entering DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>)");
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

		_logger.LogDebug("DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>): customer → {Customer}", customer);
        if (!await CanSendRequestAsync(customer))
            return;

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, shoppingCartItem.StoreId);
		_logger.LogDebug("DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>): cart → {Cart}", cart);

        //var sendRequest = await _omnisendCustomerService.IsNeedToSendDeleteShoppingCartEventAsync(customer);

        //if (sendRequest)
        //    await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}/{shoppingCartItem.Id}", httpMethod: HttpMethod.Delete);

        if (!cart.Any())
        {
            //if (sendRequest)
            //    await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}", httpMethod: HttpMethod.Delete);

            await _omnisendCustomerService.DeleteCurrentCustomerShoppingCartIdAsync(customer);
        }
		_logger.LogDebug("<<< Exiting DeleteShoppingCartItemAsync(shoppingCartItem=<redacted>)");
    }

    #endregion

    #region Orders

    /// <summary>
    /// Sends the new order to the omnisend service
    /// </summary>
    /// <param name="order">Order</param>
    public async Task PlaceOrderAsync(Order order)
    {
		_logger.LogDebug(">>> Entering PlaceOrderAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		_logger.LogDebug("PlaceOrderAsync(order={Order}): customer → {Customer}", order, customer);
        if (!await CanSendRequestAsync(customer))
            return;

        //await CreateOrderAsync(order);

        await _omnisendCustomerService.DeleteStoredCustomerShoppingCartIdAsync(customer);
		_logger.LogDebug("<<< Exiting PlaceOrderAsync(order={Order})", order);
    }

    /// <summary>
    /// Updates the order information
    /// </summary>
    /// <param name="order">Order</param>
    public async Task UpdateOrderAsync(Order order)
    {
		_logger.LogDebug(">>> Entering UpdateOrderAsync(order={Order})", order);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		_logger.LogDebug("UpdateOrderAsync(order={Order}): customer → {Customer}", order, customer);
        if (!await CanSendRequestAsync(customer))
            return;

        var item = await OrderToDtoAsync(order);
		_logger.LogDebug("UpdateOrderAsync(order={Order}): item → {Item}", order, item);
        if (item is null)
            return;

        var data = JsonConvert.SerializeObject(item);
		_logger.LogDebug("UpdateOrderAsync(order={Order}): data → {Data}", order, data);
		_logger.LogInformation("UpdateOrderAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.OrdersApiUrl}/{item.OrderId}", data, HttpMethod.Put);
		_logger.LogDebug("<<< Exiting UpdateOrderAsync(order={Order})", order);
    }

    /// <summary>
    /// Store the CartId during order placing
    /// </summary>
    /// <param name="entity">Order item</param>
    /// <returns></returns>
    public async Task OrderItemAddedAsync(OrderItem entity)
    {
		_logger.LogDebug(">>> Entering OrderItemAddedAsync(entity={Entity})", entity);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("OrderItemAddedAsync(entity={Entity}): store → {Store}", entity, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogDebug("OrderItemAddedAsync(entity={Entity}): cart → {Cart}", entity, cart);
		_logger.LogDebug("OrderItemAddedAsync(entity={Entity}): customer → {Customer}", entity, customer);

        if (cart.Any(sci =>
                sci.ProductId == entity.ProductId &&
                sci.AttributesXml.Equals(entity.AttributesXml, StringComparison.InvariantCultureIgnoreCase) &&
                sci.Quantity == entity.Quantity))
            await _omnisendCustomerService.StoreCartIdAsync(customer);
			_logger.LogDebug("<<< Exiting OrderItemAddedAsync(entity={Entity})", entity);
    }

    #endregion

    #endregion

    #region Properties

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <returns>Result</returns>
    public bool IsConfigured => !string.IsNullOrEmpty(_omnisendSettings.ApiKey);

    #endregion
}