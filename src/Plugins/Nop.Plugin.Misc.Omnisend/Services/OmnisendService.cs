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

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents the main plugin service class
/// </summary>
public class OmnisendService
{
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
        _categoryService = categoryService;
        _countryService = countryService;
        _customerService = customerService;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _countryRepository = countryRepository;
        _customerRepository = customerRepository;
        _newsLetterSubscriptionRepository = newsLetterSubscriptionRepository;
        _stateProvinceRepository = stateProvinceRepository;
        _settingService = settingService;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _taxService = taxService;
        _webHelper = webHelper;
        _workContext = workContext;
        _omnisendCustomerService = omnisendCustomerService;
        _omnisendHelper = omnisendHelper;
        _omnisendHttpClient = omnisendHttpClient;
        _omnisendSettings = omnisendSettings;
    }

    #endregion

    #region Utilities

    private async Task<string> SendBatchAsync(string data)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

        if (rez == null)
            return string.Empty;

        var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

        if (bathId == null)
            return string.Empty;

        _omnisendSettings.BatchesIds.Add(bathId);
        await _settingService.SaveSettingAsync(_omnisendSettings);

        return bathId;
    }

    private async Task<string> SendProductBatchAsync(IEnumerable<Product> products, HttpMethod batchMethod)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var data = JsonConvert.SerializeObject(new BatchRequest
        {
            Method = batchMethod.Method.ToUpper(),
            Endpoint = OmnisendDefaults.ProductsEndpoint,
            Items = await products.SelectAwait(async product => await ProductToDtoAsync(product) as IBatchSupport).ToListAsync()
        });

        return await SendBatchAsync(data);
    }

    /// <summary>
    /// Process the batch response
    /// </summary>
    /// <param name="batchResponse">Batch response to process</param>
    /// <returns>New batch identifier if it placed during process the current one</returns>
    public async Task<string> ProcessBatch(BatchResponse batchResponse)
    {
		logger.LogDebug(">>> Entering ProcessBatch(batchResponse={BatchResponse})", batchResponse);
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
				logger.LogDebug("ProcessBatch(batchResponse={BatchResponse}): batchResponse.ErrorsCount → {Arg1}", batchResponse, batchResponse.ErrorsCount);

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

			logger.LogInformation("ProcessBatch(batchResponse)={BatchResponse}: external service call {Endpoint} took {Arg2} ms", batchResponse, endpoint, __logStart.ElapsedMilliseconds);
			logger.LogDebug("<<< Exiting ProcessBatch(batchResponse={BatchResponse})", batchResponse);
            return (batchResponse.ErrorsCount == 0, newBatchId);
        }
		logger.LogDebug(">>> Entering ProductToDtoAsync(product={Product})", product);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("ProductToDtoAsync(product)={Product}: database query executed in {Arg1} ms", product, __logStart.ElapsedMilliseconds);
    }

    private async Task<List<BatchResponse>> GetBatchesInfoAsync(IList<string> batchesIds)
    {
		logger.LogDebug(">>> Entering GetBatchesInfoAsync(batchesIds={BatchesIds})", batchesIds);
        if (!batchesIds.Any())
            return [];
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var batches = await batchesIds.SelectAwait(GetBatchInfoAsync)
            .ToListAsync();

		logger.LogDebug("GetBatchesInfoAsync(batchesIds={BatchesIds}): batches → {Batches}", batchesIds, batches);
		logger.LogInformation("GetBatchesInfoAsync(batchesIds)={BatchesIds}: database query executed in {Arg1} ms", batchesIds, __logStart.ElapsedMilliseconds);
        batches = batches.Where(p => p != null).ToList();

        if (batches.Any())
            return batches;

        _omnisendSettings.BatchesIds.Clear();
        await _settingService.SaveSettingAsync(_omnisendSettings);

		logger.LogDebug("<<< Exiting GetBatchesInfoAsync(batchesIds={BatchesIds})", batchesIds);
        return batches;
    }

    private async Task FillCustomerInfoAsync(BaseContactInfoDto dto, Customer customer)
    {
        if (customer == null)
            return;

        dto.FirstName = customer.FirstName;
        dto.LastName = customer.LastName;

        var country = await _countryService.GetCountryByIdAsync(customer.CountryId);

        if (country != null)
        {
            dto.Country = country.Name;
            dto.CountryCode = country.TwoLetterIsoCode;
        }

        var state = await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId);

        if (state != null)
            dto.State = state.Name;

        dto.City = customer.City;
        dto.Address = customer.StreetAddress;
        dto.PostalCode = customer.ZipPostalCode;
        dto.Gender = customer.Gender?.ToLower() ?? "f";
        dto.BirthDate = customer.DateOfBirth?.ToString("yyyy-MM-dd");
		logger.LogDebug("<<< Exiting FillCustomerInfoAsync(dto={Dto},customer={Customer})", dto, customer);
    }

    private async Task<OrderDto.OrderItemDto> OrderItemToDtoAsync(OrderItem orderItem)
    {
		logger.LogDebug(">>> Entering OrderItemToDtoAsync(orderItem={OrderItem})", orderItem);
        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

		logger.LogDebug("OrderItemToDtoAsync(orderItem={OrderItem}): product → {Product}", orderItem, product);
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

		logger.LogDebug("<<< Exiting OrderItemToDtoAsync(orderItem={OrderItem})", orderItem);
        return dto;
    }

    private async Task<ProductDto> ProductToDtoAsync(Product product)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        async Task<List<string>> getProductCategories()
        {
			logger.LogDebug(">>> Entering getProductCategories()");
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);

			logger.LogDebug("getProductCategories(): productCategories → {ProductCategories}", productCategories);
			logger.LogDebug("<<< Exiting getProductCategories()");
            return productCategories.Select(pc => pc.CategoryId.ToString()).ToList();
        }

        async Task<IList<ProductAttributeCombination>> getProductCombinations()
        {
			logger.LogDebug(">>> Entering getProductCombinations()");
			logger.LogDebug("<<< Exiting getProductCombinations()");
            return await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
        }

        var combinations = await getProductCombinations();

        async Task<string> getProductStatus(ProductAttributeCombination productAttributeCombination = null)
        {
			logger.LogDebug(">>> Entering getProductStatus(productAttributeCombination={ProductAttributeCombination})", productAttributeCombination);
            var status = "notAvailable";

			logger.LogDebug("getProductStatus(productAttributeCombination={ProductAttributeCombination}): status → {Status}", productAttributeCombination, status);
            if (!product.Published || product.Deleted)
                return status;

            int stockQuantity;

            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    stockQuantity = await _productService.GetTotalStockQuantityAsync(product);
					logger.LogDebug("getProductStatus(productAttributeCombination={ProductAttributeCombination}): stockQuantity → {StockQuantity}", productAttributeCombination, stockQuantity);

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
		logger.LogDebug(">>> Entering OrderToDtoAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        var email = await _omnisendCustomerService.GetEmailAsync(customer, order.BillingAddressId);
		logger.LogDebug("OrderToDtoAsync(order={Order}): customer → {Customer}", order, customer);
		logger.LogDebug("OrderToDtoAsync(order={Order}): email → {Email}", order, email);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(email))
            return null;

        var dto = new OrderDto
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
		logger.LogInformation("OrderToDtoAsync(order)={Order}: find query executed in {Arg1} ms", order, __logStart.ElapsedMilliseconds);

		logger.LogDebug("<<< Exiting OrderToDtoAsync(order={Order})", order);
        return dto;
    }

    private async Task<CartItemDto> ShoppingCartItemToDtoAsync(ShoppingCartItem shoppingCartItem, Customer customer)
    {
        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

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

        return dto;
    }

    private CategoryDto CategoryToDto(Category category)
    {
		logger.LogDebug(">>> Entering CategoryToDto(category={Category})", category);
		logger.LogDebug("<<< Exiting CategoryToDto(category={Category})", category);
        return new CategoryDto
        {
            CategoryId = category.Id.ToString(),
            Title = category.Name,
            CreatedAt = category.CreatedOnUtc.ToDtoString()
        };
    }

    private async Task<CartDto> GetCartDtoAsync(IList<ShoppingCartItem> cart)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (!cart.Any())
            return null;

        var customerId = cart.First().CustomerId;
        var customer = await _customerService.GetCustomerByIdAsync(customerId);

        var items = await cart
            .SelectAwait(async ci => await ShoppingCartItemToDtoAsync(ci, customer))
            .ToListAsync();

        var cartSum = (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal
            ?.ToCents() ?? 0;

        var email = await _omnisendCustomerService.GetEmailAsync(customer);
        if (string.IsNullOrEmpty(email))
            return null;

        var cartId = await _omnisendCustomerService.GetCartIdAsync(customer);

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
		logger.LogDebug(">>> Entering CreateOrderAsync(order={Order})", order);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var orderDto = await OrderToDtoAsync(order);
        if (orderDto is null)
			logger.LogDebug("CreateOrderAsync(order={Order}): orderDto → {OrderDto}", order, orderDto);
            return;

        var data = JsonConvert.SerializeObject(orderDto);
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.OrdersApiUrl, data, HttpMethod.Post);
		logger.LogInformation("CreateOrderAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
		logger.LogDebug("CreateOrderAsync(order={Order}): data → {Data}", order, data);
		logger.LogDebug("<<< Exiting CreateOrderAsync(order={Order})", order);
    }

    private async Task CreateCartAsync(IList<ShoppingCartItem> cart)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cartDto = await GetCartDtoAsync(cart);
        if (cartDto is null)
            return;

        var data = JsonConvert.SerializeObject(cartDto);
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CartsApiUrl, data, HttpMethod.Post);
		logger.LogDebug("<<< Exiting CreateCartAsync(cart={Cart})", cart);
    }

    private async Task UpdateCartAsync(Customer customer, ShoppingCartItem newItem)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var item = await ShoppingCartItemToDtoAsync(newItem, customer);
        if (item is null)
            return;

        var data = JsonConvert.SerializeObject(item);
        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}", data, HttpMethod.Post);
		logger.LogDebug("<<< Exiting UpdateCartAsync(customer={Customer},newItem={NewItem})", customer, newItem);
    }

    private async Task<bool> CanSendRequestAsync(Customer customer)
    {
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

            if (!string.IsNullOrEmpty(item.CountryName))
                dto.Country = item.CountryName;

            if (!string.IsNullOrEmpty(item.CountryTwoLetterIsoCode))
                dto.CountryCode = item.CountryTwoLetterIsoCode;

            if (!string.IsNullOrEmpty(item.StateProvinceName))
                dto.State = item.StateProvinceName;

            return (IBatchSupport)dto;
        }).ToList();

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
		logger.LogDebug(">>> Entering SyncContactsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
        var su = await PrepareNewsletterSubscribersAsync(store.Id, 0, _omnisendSettings.PageSize);
		logger.LogDebug("SyncContactsAsync(): store → {Store}", store);
		logger.LogDebug("SyncContactsAsync(): su → {Su}", su);

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

				logger.LogDebug("SyncContactsAsync(): rez → {Rez}", rez);
                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

				logger.LogDebug("SyncContactsAsync(): bathId → {BathId}", bathId);
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
				logger.LogDebug("<<< Exiting SyncContactsAsync()");
				logger.LogInformation("SyncContactsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Synchronize categories
    /// </summary>
    public async Task SyncCategoriesAsync()
    {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug(">>> Entering SyncCategoriesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var categories = await _categoryService.GetAllCategoriesAsync(null, pageSize: _omnisendSettings.PageSize);

		logger.LogDebug("SyncCategoriesAsync(): categories → {Categories}", categories);
        if (categories.TotalCount >= OmnisendDefaults.MinCountToUseBatch || categories.TotalCount > _omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < categories.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
					logger.LogDebug("SyncCategoriesAsync(): data → {Data}", data);
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
				logger.LogInformation("SyncCategoriesAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
            }
			logger.LogDebug(">>> Entering UpdateOrCreateContactAsync(request={Request})", request);
    }

    /// <summary>
    /// Synchronize categories
    /// </summary>
    /// <param name="categoriesId">Categories identifiers list to update</param>
    public async Task UpdateCategoriesAsync(int[] categoriesId)
    {
		logger.LogDebug(">>> Entering UpdateCategoriesAsync(categoriesId={CategoriesId})", categoriesId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var categories = await _categoryService.GetCategoriesByIdsAsync(categoriesId);

		logger.LogDebug("UpdateCategoriesAsync(categoriesId={CategoriesId}): categories → {Categories}", categoriesId, categories);
        foreach (var category in categories)
        {
            var data = JsonConvert.SerializeObject(CategoryToDto(category));
            await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CategoriesApiUrl + $"/{category.Id}", data, HttpMethod.Put);
			logger.LogDebug("UpdateCategoriesAsync(categoriesId={CategoriesId}): data → {Data}", categoriesId, data);
			logger.LogInformation("UpdateCategoriesAsync(categoriesId)={CategoriesId}: external service call took {Arg1} ms", categoriesId, __logStart.ElapsedMilliseconds);
        }
		logger.LogDebug("<<< Exiting UpdateCategoriesAsync(categoriesId={CategoriesId})", categoriesId);
		logger.LogInformation("UpdateCategoriesAsync(categoriesId)={CategoriesId}: external service call took {Arg1} ms", categoriesId, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Synchronize products
    /// </summary>
    public async Task SyncProductsAsync()
    {
		logger.LogDebug(">>> Entering SyncProductsAsync()");
        var products = await _productService.SearchProductsAsync(pageSize: _omnisendSettings.PageSize);

		logger.LogDebug("SyncProductsAsync(): products → {Products}", products);
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
				logger.LogDebug("<<< Exiting SyncProductsAsync()");
    }

    /// <summary>
    /// Synchronize orders
    /// </summary>
    public async Task SyncOrdersAsync()
    {
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug(">>> Entering SyncOrdersAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var orders = await _orderService.SearchOrdersAsync(pageSize: _omnisendSettings.PageSize);

        if (orders.TotalCount >= OmnisendDefaults.MinCountToUseBatch || orders.TotalCount > _omnisendSettings.PageSize)
		logger.LogDebug("SyncOrdersAsync(): orders → {Orders}", orders);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        {
            var page = 0;

            while (page < orders.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.OrdersEndpoint,
                    Items = await orders.SelectAwait(async order => await OrderToDtoAsync(order) as IBatchSupport).ToListAsync()
                });
				logger.LogInformation("SyncOrdersAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

                var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

				logger.LogDebug("SyncOrdersAsync(): rez → {Rez}", rez);
                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

				logger.LogDebug("SyncOrdersAsync(): bathId → {BathId}", bathId);
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
				logger.LogDebug("<<< Exiting SyncOrdersAsync()");
				logger.LogInformation("SyncOrdersAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Synchronize shopping carts
    /// </summary>
    public async Task SyncCartsAsync()
    {
		logger.LogDebug(">>> Entering SyncCartsAsync()");
        var store = await _storeContext.GetCurrentStoreAsync();
        var customers = await _customerService.GetCustomersWithShoppingCartsAsync(ShoppingCartType.ShoppingCart, store.Id);
		logger.LogDebug("SyncCartsAsync(): store → {Store}", store);
		logger.LogDebug("SyncCartsAsync(): customers → {Customers}", customers);
        foreach (var customer in customers)
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            await CreateCartAsync(cart);
			logger.LogDebug("SyncCartsAsync(): cart → {Cart}", cart);
        }
		logger.LogDebug("<<< Exiting SyncCartsAsync()");
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
		logger.LogDebug(">>> Entering GetStoredBatchesAsync()");
        var batches = await GetBatchesInfoAsync(_omnisendSettings.BatchesIds);

		logger.LogDebug("GetStoredBatchesAsync(): batches → {Batches}", batches);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var additionalBatches = await batches
            .Where(p => p.Status.Equals(OmnisendDefaults.BatchFinishedStatus,
                StringComparison.InvariantCultureIgnoreCase))
            .SelectAwait(async batchResponse => await ProcessBatch(batchResponse))
            .Where(newBatchId => !string.IsNullOrEmpty(newBatchId)).ToListAsync();
			logger.LogInformation("GetStoredBatchesAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        
        batches.AddRange(await GetBatchesInfoAsync(additionalBatches));

		logger.LogDebug("<<< Exiting GetStoredBatchesAsync()");
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
		logger.LogDebug(">>> Entering GetBrandIdAsync(apiKey={ApiKey})", apiKey);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        _omnisendHttpClient.ApiKey = apiKey;

		logger.LogDebug("GetBrandIdAsync(apiKey={ApiKey}): _omnisendHttpClient.ApiKey → {Arg1}", apiKey, _omnisendHttpClient.ApiKey);
        var result = await _omnisendHttpClient.PerformRequestAsync<AccountsResponse>(OmnisendDefaults.AccountsApiUrl);

		logger.LogDebug("GetBrandIdAsync(apiKey={ApiKey}): result → {Result}", apiKey, result);
		logger.LogInformation("GetBrandIdAsync(apiKey)={ApiKey}: external service call took {Arg1} ms", apiKey, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetBrandIdAsync(apiKey={ApiKey})", apiKey);
        return result?.BrandId;
    }

    /// <summary>
    /// Registers the site on the omnisend service
    /// </summary>
    public async Task SendCustomerDataAsync()
    {
		logger.LogDebug(">>> Entering SendCustomerDataAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var site = _webHelper.GetStoreLocation();

		logger.LogDebug("SendCustomerDataAsync(): site → {Site}", site);
        var data = JsonConvert.SerializeObject(new
        {
            website = site,
            platform = OmnisendDefaults.IntegrationOrigin,
            version = OmnisendDefaults.IntegrationVersion
			logger.LogDebug("SendCustomerDataAsync(): website → {Website}", website);
			logger.LogDebug("SendCustomerDataAsync(): platform → {Platform}", platform);
			logger.LogDebug("SendCustomerDataAsync(): version → {Version}", version);
        });

        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.AccountsApiUrl, data, HttpMethod.Post);
		logger.LogDebug("<<< Exiting SendCustomerDataAsync()");
		logger.LogInformation("SendCustomerDataAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Gets information about batch
    /// </summary>
    /// <param name="bathId">Batch identifier</param>
    public async ValueTask<BatchResponse> GetBatchInfoAsync(string bathId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var url = OmnisendDefaults.BatchesApiUrl + $"/{bathId}";

        var result = await _omnisendHttpClient.PerformRequestAsync<BatchResponse>(url, httpMethod: HttpMethod.Get);

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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var url = $"{OmnisendDefaults.ContactsApiUrl}/{contactId}";

        var res = await _omnisendHttpClient.PerformRequestAsync<ContactInfoDto>(url, httpMethod: HttpMethod.Get);

        return res;
    }

    /// <summary>
    /// Update or create contact information
    /// </summary>
    /// <param name="request">Create contact request</param>
    public async Task UpdateOrCreateContactAsync(CreateContactRequest request)
    {
		logger.LogDebug(">>> Entering UpdateOrCreateContactAsync(request={Request})", request);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var email = request.Identifiers.First().Id;
        var exists = !string.IsNullOrEmpty(await _omnisendCustomerService.GetContactIdAsync(email));
		logger.LogDebug("UpdateOrCreateContactAsync(request={Request}): email → {Email}", request, email);
		logger.LogDebug("UpdateOrCreateContactAsync(request={Request}): exists → {Exists}", request, exists);

        if (!exists)
        {
            var data = JsonConvert.SerializeObject(request);
            await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.ContactsApiUrl, data, HttpMethod.Post);
			logger.LogDebug("UpdateOrCreateContactAsync(request={Request}): data → {Data}", request, data);
        }
        else
        {
            var url = $"{OmnisendDefaults.ContactsApiUrl}?email={email}";
            var data = JsonConvert.SerializeObject(new { identifiers = new[] { request.Identifiers.First() } });
			logger.LogDebug("UpdateOrCreateContactAsync(request={Request}): url → {Url}", request, url);

            await _omnisendHttpClient.PerformRequestAsync(url, data, HttpMethod.Patch);
        }
		logger.LogInformation("UpdateOrCreateContactAsync(request)={Request}: external service call {Url} took {Arg2} ms", request, url, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting UpdateOrCreateContactAsync(request={Request})", request);
    }

    /// <summary>
    /// Update or create contact information
    /// </summary>
    /// <param name="subscription">Newsletter subscription</param>
    /// <param name="sendWelcomeMessage">Specifies whether to send a welcome message</param>
    public async Task UpdateOrCreateContactAsync(NewsLetterSubscription subscription, bool sendWelcomeMessage = false)
    {
		logger.LogDebug(">>> Entering UpdateOrCreateContactAsync(subscription={Subscription},sendWelcomeMessage={SendWelcomeMessage})", subscription, sendWelcomeMessage);
        var su = await PrepareNewsletterSubscribersAsync(subscription.StoreId,
            0, _omnisendSettings.PageSize,
            sendWelcomeMessage, subscription, "unsubscribed");
			logger.LogDebug("<<< Exiting UpdateOrCreateContactAsync(request={Request})", request);

        if (su.FirstOrDefault() is not CreateContactRequest request)
			logger.LogDebug("<<< Exiting UpdateOrCreateContactAsync(subscription={Subscription},sendWelcomeMessage={SendWelcomeMessage})", subscription, sendWelcomeMessage);
            return;

        await UpdateOrCreateContactAsync(request);
    }

    /// <summary>
    /// Updates contact information by customer data
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task UpdateContactAsync(Customer customer)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var contactId = await _omnisendCustomerService.GetContactIdAsync(customer.Email);

        if (string.IsNullOrEmpty(contactId))
            return;

        var url = $"{OmnisendDefaults.ContactsApiUrl}/{contactId}";

        var dto = new BaseContactInfoDto();

        await FillCustomerInfoAsync(dto, customer);

        var data = JsonConvert.SerializeObject(dto);

        await _omnisendHttpClient.PerformRequestAsync(url, data, HttpMethod.Patch);
		logger.LogDebug("<<< Exiting UpdateContactAsync(customer={Customer})", customer);
    }

    #endregion

    #region Products

    /// <summary>
    /// Adds new product
    /// </summary>
    /// <param name="product">Product to add</param>
    public async Task AddNewProductAsync(Product product)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var data = JsonConvert.SerializeObject(await ProductToDtoAsync(product));
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.ProductsApiUrl, data, HttpMethod.Post);
    }

    /// <summary>
    /// Updates the product
    /// </summary>
    /// <param name="productId">Product identifier to update</param>
    public async Task UpdateProductAsync(int productId)
    {
		logger.LogDebug(">>> Entering UpdateProductAsync(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);

		logger.LogDebug("UpdateProductAsync(productId={ProductId}): product → {Product}", productId, product);
        await CreateOrUpdateProductAsync(product);
		logger.LogDebug("<<< Exiting UpdateProductAsync(productId={ProductId})", productId);
    }

    /// <summary>
    /// Updates products
    /// </summary>
    /// /// <param name="productsId">Products identifiers list to update</param>
    public async Task<string> UpdateProductsAsync(int[] productsId)
    {
		logger.LogDebug(">>> Entering UpdateProductsAsync(productsId={ProductsId})", productsId);
        var products = await _productService.GetProductsByIdsAsync(productsId);

		logger.LogDebug("UpdateProductsAsync(productsId={ProductsId}): products → {Products}", productsId, products);
		logger.LogDebug("<<< Exiting UpdateProductsAsync(productsId={ProductsId})", productsId);
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var res = await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{cartId}", httpMethod: HttpMethod.Get);

        if (string.IsNullOrEmpty(res))
            return;

        var restoredCart = JsonConvert.DeserializeObject<CartDto>(res)
            ?? throw new NopException("Cart data can't be loaded");

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        foreach (var cartProduct in restoredCart.Products)
        {
            var combination = await _productAttributeService.GetProductAttributeCombinationBySkuAsync(cartProduct.Sku);
            var product = combination == null ? await _productService.GetProductBySkuAsync(cartProduct.Sku) : await _productService.GetProductByIdAsync(combination.ProductId);

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
		logger.LogDebug("<<< Exiting RestoreShoppingCartAsync(cartId={CartId})", cartId);
    }

    /// <summary>
    /// Adds new item to the shopping cart
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task AddShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, shoppingCartItem.StoreId);

        if (cart.Count == 1)
            await CreateCartAsync(cart);
        else
            await UpdateCartAsync(customer, shoppingCartItem);
			logger.LogDebug("<<< Exiting AddShoppingCartItemAsync(shoppingCartItem={ShoppingCartItem})", shoppingCartItem);
    }

    /// <summary>
    /// Updates the shopping cart item
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task EditShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var item = await ShoppingCartItemToDtoAsync(shoppingCartItem, customer);

        var data = JsonConvert.SerializeObject(item);

        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}/{item.CartProductId}", data, HttpMethod.Put);
		logger.LogDebug("<<< Exiting EditShoppingCartItemAsync(shoppingCartItem={ShoppingCartItem})", shoppingCartItem);
    }

    /// <summary>
    /// Deletes item from shopping cart
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, shoppingCartItem.StoreId);

        //var sendRequest = await _omnisendCustomerService.IsNeedToSendDeleteShoppingCartEventAsync(customer);

        //if (sendRequest)
        //    await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}/{shoppingCartItem.Id}", httpMethod: HttpMethod.Delete);

        if (!cart.Any())
        {
            //if (sendRequest)
            //    await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}", httpMethod: HttpMethod.Delete);

            await _omnisendCustomerService.DeleteCurrentCustomerShoppingCartIdAsync(customer);
        }
		logger.LogDebug("<<< Exiting DeleteShoppingCartItemAsync(shoppingCartItem={ShoppingCartItem})", shoppingCartItem);
    }

    #endregion

    #region Orders

    /// <summary>
    /// Sends the new order to the omnisend service
    /// </summary>
    /// <param name="order">Order</param>
    public async Task PlaceOrderAsync(Order order)
    {
		logger.LogDebug(">>> Entering PlaceOrderAsync(order={Order})", order);
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		logger.LogDebug("PlaceOrderAsync(order={Order}): customer → {Customer}", order, customer);
        if (!await CanSendRequestAsync(customer))
            return;

        //await CreateOrderAsync(order);

        await _omnisendCustomerService.DeleteStoredCustomerShoppingCartIdAsync(customer);
		logger.LogDebug("<<< Exiting PlaceOrderAsync(order={Order})", order);
    }

    /// <summary>
    /// Updates the order information
    /// </summary>
    /// <param name="order">Order</param>
    public async Task UpdateOrderAsync(Order order)
    {
		logger.LogDebug(">>> Entering UpdateOrderAsync(order={Order})", order);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

		logger.LogDebug("UpdateOrderAsync(order={Order}): customer → {Customer}", order, customer);
        if (!await CanSendRequestAsync(customer))
            return;

        var item = await OrderToDtoAsync(order);
        if (item is null)
			logger.LogDebug("UpdateOrderAsync(order={Order}): item → {Item}", order, item);
            return;

        var data = JsonConvert.SerializeObject(item);
        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.OrdersApiUrl}/{item.OrderId}", data, HttpMethod.Put);
		logger.LogInformation("UpdateOrderAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
		logger.LogDebug("UpdateOrderAsync(order={Order}): data → {Data}", order, data);
		logger.LogDebug("<<< Exiting UpdateOrderAsync(order={Order})", order);
    }

    /// <summary>
    /// Store the CartId during order placing
    /// </summary>
    /// <param name="entity">Order item</param>
    /// <returns></returns>
    public async Task OrderItemAddedAsync(OrderItem entity)
    {
		logger.LogDebug(">>> Entering OrderItemAddedAsync(entity={Entity})", entity);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		logger.LogDebug("OrderItemAddedAsync(entity={Entity}): customer → {Customer}", entity, customer);
		logger.LogDebug("OrderItemAddedAsync(entity={Entity}): store → {Store}", entity, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

		logger.LogDebug("OrderItemAddedAsync(entity={Entity}): cart → {Cart}", entity, cart);
        if (cart.Any(sci =>
                sci.ProductId == entity.ProductId &&
                sci.AttributesXml.Equals(entity.AttributesXml, StringComparison.InvariantCultureIgnoreCase) &&
                sci.Quantity == entity.Quantity))
            await _omnisendCustomerService.StoreCartIdAsync(customer);
			logger.LogDebug("<<< Exiting OrderItemAddedAsync(entity={Entity})", entity);
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
