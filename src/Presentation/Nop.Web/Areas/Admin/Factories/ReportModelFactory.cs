using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the report model factory implementation
/// </summary>
public partial class ReportModelFactory : IReportModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerReportService _customerReportService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderReportService _orderReportService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductService _productService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ReportModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICustomerReportService customerReportService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IOrderReportService orderReportService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductService productService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext})", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _baseAdminModelFactory → {_baseAdminModelFactory}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _baseAdminModelFactory);
        _countryService = countryService;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _countryService → {_countryService}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _countryService);
        _customerReportService = customerReportService;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _customerReportService → {_customerReportService}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _customerReportService);
        _customerService = customerService;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _customerService → {_customerService}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _dateTimeHelper → {_dateTimeHelper}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _dateTimeHelper);
        _localizationService = localizationService;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _localizationService → {_localizationService}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _localizationService);
        _orderReportService = orderReportService;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _orderReportService → {_orderReportService}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _orderReportService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _priceFormatter → {_priceFormatter}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _priceFormatter);
        _productAttributeFormatter = productAttributeFormatter;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _productAttributeFormatter → {_productAttributeFormatter}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _productAttributeFormatter);
        _productService = productService;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _productService → {_productService}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _productService);
        _storeContext = storeContext;
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _storeContext → {_storeContext}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext})", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext);
		_logger.LogDebug("ReportModelFactory(baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerReportService={CustomerReportService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderReportService={OrderReportService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productService={ProductService},storeContext={StoreContext},workContext={WorkContext}): _workContext → {_workContext}", baseAdminModelFactory, countryService, customerReportService, customerService, dateTimeHelper, localizationService, orderReportService, priceFormatter, productAttributeFormatter, productService, storeContext, workContext, _workContext);
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<IPagedList<SalesSummaryReportLine>> GetSalesSummaryReportAsync(SalesSummarySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering GetSalesSummaryReportAsync(searchModel={SearchModel})", searchModel);
        //get parameters to filter orders
        var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
		_logger.LogDebug("GetSalesSummaryReportAsync(searchModel={SearchModel}): orderStatusIds → {OrderStatusIds}", searchModel, orderStatusIds);
        var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
		_logger.LogDebug("GetSalesSummaryReportAsync(searchModel={SearchModel}): paymentStatusIds → {PaymentStatusIds}", searchModel, paymentStatusIds);

        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("GetSalesSummaryReportAsync(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);

        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get sales summary
        var salesSummary = await _orderReportService.SalesSummaryReportAsync(
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            billingCountryId: searchModel.BillingCountryId,
            groupBy: (GroupByOptions)searchModel.SearchGroupId,
            categoryId: searchModel.CategoryId,
            productId: searchModel.ProductId,
            manufacturerId: searchModel.ManufacturerId,
            vendorId: currentVendor?.Id ?? searchModel.VendorId,
            storeId: searchModel.StoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

		_logger.LogDebug("<<< Exiting GetSalesSummaryReportAsync(searchModel={SearchModel})", searchModel);
        return salesSummary;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<IPagedList<BestsellersReportLine>> GetBestsellersReportAsync(BestsellerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering GetBestsellersReportAsync(searchModel={SearchModel})", searchModel);
        //get parameters to filter bestsellers
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
		_logger.LogDebug("GetBestsellersReportAsync(searchModel={SearchModel}): orderStatus → {OrderStatus}", searchModel, orderStatus);
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
		_logger.LogDebug("GetBestsellersReportAsync(searchModel={SearchModel}): paymentStatus → {PaymentStatus}", searchModel, paymentStatus);
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("GetBestsellersReportAsync(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
        if (currentVendor != null)
            searchModel.VendorId = currentVendor.Id;
			_logger.LogDebug("GetBestsellersReportAsync(searchModel={SearchModel}): searchModel.VendorId → {Arg1}", searchModel, searchModel.VendorId);
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get bestsellers
        var bestsellers = await _orderReportService.BestSellersReportAsync(showHidden: true,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            billingCountryId: searchModel.BillingCountryId,
            orderBy: OrderByEnum.OrderByTotalAmount,
            vendorId: searchModel.VendorId,
            categoryId: searchModel.CategoryId,
            manufacturerId: searchModel.ManufacturerId,
            storeId: searchModel.StoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

		_logger.LogDebug("<<< Exiting GetBestsellersReportAsync(searchModel={SearchModel})", searchModel);
        return bestsellers;
    }

    #endregion

    #region Methods

    #region Sales summary

    /// <summary>
    /// Prepare sales summary search model
    /// </summary>
    /// <param name="searchModel">Sales summary search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sales summary search model
    /// </returns>
    public virtual async Task<SalesSummarySearchModel> PrepareSalesSummarySearchModelAsync(SalesSummarySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareSalesSummarySearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available order statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
        if (searchModel.AvailableOrderStatuses.Any())
        {
            if (searchModel.OrderStatusIds?.Any() ?? false)
            {
                var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
				_logger.LogDebug("PrepareSalesSummarySearchModelAsync(searchModel={SearchModel}): ids → {Ids}", searchModel, ids);
                var statusItems = searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
				_logger.LogDebug("PrepareSalesSummarySearchModelAsync(searchModel={SearchModel}): statusItems → {StatusItems}", searchModel, statusItems);
                foreach (var statusItem in statusItems)
                {
                    statusItem.Selected = true;
					_logger.LogDebug("PrepareSalesSummarySearchModelAsync(searchModel={SearchModel}): statusItem.Selected → {Arg1}", searchModel, statusItem.Selected);
                }
            }
            else
                searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
        }

        //prepare available payment statuses
        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
        if (searchModel.AvailablePaymentStatuses.Any())
        {
            if (searchModel.PaymentStatusIds?.Any() ?? false)
            {
                var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                var statusItems = searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                foreach (var statusItem in statusItems)
                {
                    statusItem.Selected = true;
                }
            }
            else
                searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
        }

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available billing countries
        searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
            .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
        searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

        //prepare "group by" filter
        searchModel.GroupByOptions = (await GroupByOptions.Day.ToSelectListAsync()).ToList();

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareSalesSummarySearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare sales summary list model
    /// </summary>
    /// <param name="searchModel">Sales summary search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sales summary list model
    /// </returns>
    public virtual async Task<SalesSummaryListModel> PrepareSalesSummaryListModelAsync(SalesSummarySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareSalesSummaryListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        var salesSummary = await GetSalesSummaryReportAsync(searchModel);
		_logger.LogDebug("PrepareSalesSummaryListModelAsync(searchModel={SearchModel}): salesSummary → {SalesSummary}", searchModel, salesSummary);

        //prepare list model
        var model = new SalesSummaryListModel().PrepareToGrid(searchModel, salesSummary, () =>
        {
            return salesSummary.Select(sale =>
            {
                //fill in model values from the entity
                var salesSummaryModel = new SalesSummaryModel
                {
                    Summary = sale.Summary,
                    NumberOfOrders = sale.NumberOfOrders,
                    ProfitStr = sale.ProfitStr,
                    Shipping = sale.Shipping,
                    Tax = sale.Tax,
                    OrderTotal = sale.OrderTotal
                };

                return salesSummaryModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareSalesSummaryListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion

    #region LowStock

    /// <summary>
    /// Prepare low stock product search model
    /// </summary>
    /// <param name="searchModel">Low stock product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the low stock product search model
    /// </returns>
    public virtual async Task<LowStockProductSearchModel> PrepareLowStockProductSearchModelAsync(LowStockProductSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareLowStockProductSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Reports.LowStock.SearchPublished.All")
        });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.Reports.LowStock.SearchPublished.PublishedOnly")
        });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.Reports.LowStock.SearchPublished.UnpublishedOnly")
        });

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareLowStockProductSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged low stock product list model
    /// </summary>
    /// <param name="searchModel">Low stock product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the low stock product list model
    /// </returns>
    public virtual async Task<LowStockProductListModel> PrepareLowStockProductListModelAsync(LowStockProductSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareLowStockProductListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter comments
        var publishedOnly = searchModel.SearchPublishedId == 0 ? null : searchModel.SearchPublishedId == 1 ? true : (bool?)false;
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): publishedOnly → {PublishedOnly}", searchModel, publishedOnly);
        var vendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): vendor → {Vendor}", searchModel, vendor);
        var vendorId = vendor?.Id ?? 0;

        //get low stock product and product combinations
        var products = await _productService.GetLowStockProductsAsync(vendorId: vendorId, loadPublishedOnly: publishedOnly);
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): products → {Products}", searchModel, products);
        var combinations = await _productService.GetLowStockProductCombinationsAsync(vendorId: vendorId, loadPublishedOnly: publishedOnly);
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): combinations → {Combinations}", searchModel, combinations);

        //prepare low stock product models
        var lowStockProductModels = new List<LowStockProductModel>();
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): lowStockProductModels → {LowStockProductModels}", searchModel, lowStockProductModels);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        lowStockProductModels.AddRange(await products.SelectAwait(async product => new LowStockProductModel
        {
			_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): product → {Product}", searchModel, product);
            Id = product.Id,
            Name = product.Name,

            ManageInventoryMethod = await _localizationService.GetLocalizedEnumAsync(product.ManageInventoryMethod),
            StockQuantity = await _productService.GetTotalStockQuantityAsync(product),
            Published = product.Published
        }).ToListAsync());
		_logger.LogInformation("PrepareLowStockProductListModelAsync(searchModel)={SearchModel}: database query executed in {Arg1} ms", searchModel, __logStart.ElapsedMilliseconds);

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): currentCustomer → {CurrentCustomer}", searchModel, currentCustomer);
        var currentStore = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): currentStore → {CurrentStore}", searchModel, currentStore);

        lowStockProductModels.AddRange(await combinations.SelectAwait(async combination =>
        {
            var product = await _productService.GetProductByIdAsync(combination.ProductId);

            return new LowStockProductModel
            {
                Id = combination.ProductId,
                Name = product.Name,

                Attributes = await _productAttributeFormatter
                    .FormatAttributesAsync(product, combination.AttributesXml, currentCustomer, currentStore, "<br />", true, true, true, false),
                ManageInventoryMethod = await _localizationService.GetLocalizedEnumAsync(product.ManageInventoryMethod),

                StockQuantity = combination.StockQuantity,
                Published = product.Published
            };
        }).ToListAsync());

        var pagesList = lowStockProductModels.ToPagedList(searchModel);
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): pagesList → {PagesList}", searchModel, pagesList);

        //prepare list model
        var model = new LowStockProductListModel().PrepareToGrid(searchModel, pagesList, () => pagesList);
		_logger.LogDebug("PrepareLowStockProductListModelAsync(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting PrepareLowStockProductListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion

    #region Bestsellers

    /// <summary>
    /// Prepare bestseller search model
    /// </summary>
    /// <param name="searchModel">Bestseller search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the bestseller search model
    /// </returns>
    public virtual async Task<BestsellerSearchModel> PrepareBestsellerSearchModelAsync(BestsellerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareBestsellerSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available order statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);

        //prepare available payment statuses
        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available billing countries
        searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
            .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
        searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareBestsellerSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged bestseller list model
    /// </summary>
    /// <param name="searchModel">Bestseller search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the bestseller list model
    /// </returns>
    public virtual async Task<BestsellerListModel> PrepareBestsellerListModelAsync(BestsellerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareBestsellerListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        var bestsellers = await GetBestsellersReportAsync(searchModel);
		_logger.LogDebug("PrepareBestsellerListModelAsync(searchModel={SearchModel}): bestsellers → {Bestsellers}", searchModel, bestsellers);

        //prepare list model
        var model = await new BestsellerListModel().PrepareToGridAsync(searchModel, bestsellers, () =>
        {
            return bestsellers.SelectAwait(async bestseller =>
            {
                //fill in model values from the entity
                var bestsellerModel = new BestsellerModel
                {
                    ProductId = bestseller.ProductId,
                    TotalQuantity = bestseller.TotalQuantity,
                    ProductName = bestseller.ProductName
                };

                //fill in additional values (not existing in the entity)
                bestsellerModel.TotalAmount = await _priceFormatter.FormatPriceAsync(bestseller.TotalAmount, true, false);

                return bestsellerModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareBestsellerListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Get a formatted bestsellers total amount
    /// </summary>
    /// <param name="searchModel">Bestseller search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the bestseller total amount
    /// </returns>
    public virtual async Task<string> GetBestsellerTotalAmountAsync(BestsellerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering GetBestsellerTotalAmountAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter bestsellers
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
		_logger.LogDebug("GetBestsellerTotalAmountAsync(searchModel={SearchModel}): orderStatus → {OrderStatus}", searchModel, orderStatus);
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
		_logger.LogDebug("GetBestsellerTotalAmountAsync(searchModel={SearchModel}): paymentStatus → {PaymentStatus}", searchModel, paymentStatus);
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("GetBestsellerTotalAmountAsync(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
        if (currentVendor != null)
            searchModel.VendorId = currentVendor.Id;
			_logger.LogDebug("GetBestsellerTotalAmountAsync(searchModel={SearchModel}): searchModel.VendorId → {Arg1}", searchModel, searchModel.VendorId);
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get a total amount
        var totalAmount = await _orderReportService.BestSellersReportTotalAmountAsync(
            showHidden: true,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            billingCountryId: searchModel.BillingCountryId,
            vendorId: searchModel.VendorId,
            categoryId: searchModel.CategoryId,
            manufacturerId: searchModel.ManufacturerId,
            storeId: searchModel.StoreId);

		_logger.LogDebug("<<< Exiting GetBestsellerTotalAmountAsync(searchModel={SearchModel})", searchModel);
        return await _priceFormatter.FormatPriceAsync(totalAmount, true, false);
    }

    #endregion

    #region NeverSold

    /// <summary>
    /// Prepare never sold report search model
    /// </summary>
    /// <param name="searchModel">Never sold report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the never sold report search model
    /// </returns>
    public virtual async Task<NeverSoldReportSearchModel> PrepareNeverSoldSearchModelAsync(NeverSoldReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareNeverSoldSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareNeverSoldSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged never sold report list model
    /// </summary>
    /// <param name="searchModel">Never sold report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the never sold report list model
    /// </returns>
    public virtual async Task<NeverSoldReportListModel> PrepareNeverSoldListModelAsync(NeverSoldReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareNeverSoldListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter neverSoldReports
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareNeverSoldListModelAsync(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
        if (currentVendor != null)
            searchModel.SearchVendorId = currentVendor.Id;
			_logger.LogDebug("PrepareNeverSoldListModelAsync(searchModel={SearchModel}): searchModel.SearchVendorId → {Arg1}", searchModel, searchModel.SearchVendorId);
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get report items
        var items = await _orderReportService.ProductsNeverSoldAsync(showHidden: true,
            vendorId: searchModel.SearchVendorId,
            storeId: searchModel.SearchStoreId,
            categoryId: searchModel.SearchCategoryId,
            manufacturerId: searchModel.SearchManufacturerId,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = new NeverSoldReportListModel().PrepareToGrid(searchModel, items, () =>
        {
            //fill in model values from the entity
            return items.Select(item => new NeverSoldReportModel
            {
                ProductId = item.Id,
                ProductName = item.Name
            });
        });

		_logger.LogDebug("<<< Exiting PrepareNeverSoldListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion

    #region Country sales

    /// <summary>
    /// Prepare country report search model
    /// </summary>
    /// <param name="searchModel">Country report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country report search model
    /// </returns>
    public virtual async Task<CountryReportSearchModel> PrepareCountrySalesSearchModelAsync(CountryReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCountrySalesSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available order statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);

        //prepare available payment statuses
        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareCountrySalesSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged country report list model
    /// </summary>
    /// <param name="searchModel">Country report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country report list model
    /// </returns>
    public virtual async Task<CountryReportListModel> PrepareCountrySalesListModelAsync(CountryReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCountrySalesListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter countryReports
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
		_logger.LogDebug("PrepareCountrySalesListModelAsync(searchModel={SearchModel}): orderStatus → {OrderStatus}", searchModel, orderStatus);
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
		_logger.LogDebug("PrepareCountrySalesListModelAsync(searchModel={SearchModel}): paymentStatus → {PaymentStatus}", searchModel, paymentStatus);
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get items
        var items = (await _orderReportService.GetCountryReportAsync(os: orderStatus,
            ps: paymentStatus,
            storeId: searchModel.SearchStoreId,
            startTimeUtc: startDateValue,
            endTimeUtc: endDateValue)).ToPagedList(searchModel);

        //prepare list model
        var model = await new CountryReportListModel().PrepareToGridAsync(searchModel, items, () =>
        {
            return items.SelectAwait(async item =>
            {
                //fill in model values from the entity
                var countryReportModel = new CountryReportModel
                {
                    TotalOrders = item.TotalOrders
                };

                //fill in additional values (not existing in the entity)
                countryReportModel.SumOrders = await _priceFormatter.FormatPriceAsync(item.SumOrders, true, false);
                countryReportModel.CountryName = (await _countryService.GetCountryByIdAsync(item.CountryId ?? 0))?.Name;

                return countryReportModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareCountrySalesListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion

    #region Customer reports

    /// <summary>
    /// Prepare customer reports search model
    /// </summary>
    /// <param name="searchModel">Customer reports search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer reports search model
    /// </returns>
    public virtual async Task<CustomerReportsSearchModel> PrepareCustomerReportsSearchModelAsync(CustomerReportsSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCustomerReportsSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare nested search models
        await PrepareBestCustomersReportByOrderTotalSearchModelAsync(searchModel.BestCustomersByOrderTotal);
        await PrepareBestCustomersReportSearchModelAsync(searchModel.BestCustomersByNumberOfOrders);
        await PrepareRegisteredCustomersReportSearchModelAsync(searchModel.RegisteredCustomers);

		_logger.LogDebug("<<< Exiting PrepareCustomerReportsSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare best customers by number of orders report search model
    /// </summary>
    /// <param name="searchModel">Best customers report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the best customers report search model
    /// </returns>
    protected virtual async Task<BestCustomersReportSearchModel> PrepareBestCustomersReportSearchModelAsync(BestCustomersReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareBestCustomersReportSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available order, payment and shipping statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
        await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareBestCustomersReportSearchModelAsync(searchModel={SearchModel})", searchModel);
		_logger.LogDebug(">>> Entering PrepareRegisteredCustomersReportSearchModelAsync(searchModel={SearchModel})", searchModel);
		_logger.LogDebug("<<< Exiting PrepareRegisteredCustomersReportSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare best customers by order total report search model
    /// </summary>
    /// <param name="searchModel">Best customers report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the best customers report search model
    /// </returns>
    protected virtual async Task<BestCustomersReportSearchModel> PrepareBestCustomersReportByOrderTotalSearchModelAsync(BestCustomersReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareBestCustomersReportByOrderTotalSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available order, payment and shipping statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
        await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareBestCustomersReportByOrderTotalSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }


    /// <summary>
    /// Prepare registered customers report search model
    /// </summary>
    /// <param name="searchModel">Registered customers report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the registered customers report search model
    /// </returns>
    protected virtual Task<RegisteredCustomersReportSearchModel> PrepareRegisteredCustomersReportSearchModelAsync(RegisteredCustomersReportSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged best customers report list model
    /// </summary>
    /// <param name="searchModel">Best customers report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the best customers report list model
    /// </returns>
    public virtual async Task<BestCustomersReportListModel> PrepareBestCustomersReportListModelAsync(BestCustomersReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareBestCustomersReportListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
		_logger.LogDebug("PrepareBestCustomersReportListModelAsync(searchModel={SearchModel}): orderStatus → {OrderStatus}", searchModel, orderStatus);
        var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
		_logger.LogDebug("PrepareBestCustomersReportListModelAsync(searchModel={SearchModel}): paymentStatus → {PaymentStatus}", searchModel, paymentStatus);
        var shippingStatus = searchModel.ShippingStatusId > 0 ? (ShippingStatus?)searchModel.ShippingStatusId : null;
		_logger.LogDebug("PrepareBestCustomersReportListModelAsync(searchModel={SearchModel}): shippingStatus → {ShippingStatus}", searchModel, shippingStatus);

        //get report items
        var reportItems = await _customerReportService.GetBestCustomersReportAsync(createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            os: orderStatus,
            ps: paymentStatus,
            ss: shippingStatus,
            orderBy: searchModel.OrderBy,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new BestCustomersReportListModel().PrepareToGridAsync(searchModel, reportItems, () =>
        {
            return reportItems.SelectAwait(async item =>
            {
                //fill in model values from the entity
                var bestCustomersReportModel = new BestCustomersReportModel
                {
                    CustomerId = item.CustomerId,

                    OrderTotal = await _priceFormatter.FormatPriceAsync(item.OrderTotal, true, false),
                    OrderCount = item.OrderCount
                };

                //fill in additional values (not existing in the entity)
                var customer = await _customerService.GetCustomerByIdAsync(item.CustomerId);
				_logger.LogDebug("PrepareBestCustomersReportListModelAsync(searchModel={SearchModel}): customer → {Customer}", searchModel, customer);
                if (customer != null)
                {
                    bestCustomersReportModel.CustomerName = (await _customerService.IsRegisteredAsync(customer))
                        ? customer.Email
                        : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                }

                return bestCustomersReportModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareBestCustomersReportListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare paged registered customers report list model
    /// </summary>
    /// <param name="searchModel">Registered customers report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the registered customers report list model
    /// </returns>
    public virtual async Task<RegisteredCustomersReportListModel> PrepareRegisteredCustomersReportListModelAsync(RegisteredCustomersReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareRegisteredCustomersReportListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get report items
        var reportItems = new List<RegisteredCustomersReportModel>
        {
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.7days"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(7)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.14days"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(14)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.month"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(30)
            },
            new() {
                Period = await _localizationService.GetResourceAsync("Admin.Reports.Customers.RegisteredCustomers.Fields.Period.year"),
                Customers = await _customerReportService.GetRegisteredCustomersReportAsync(365)
            }
        };

        var pagedList = reportItems.ToPagedList(searchModel);

        //prepare list model
        var model = new RegisteredCustomersReportListModel().PrepareToGrid(searchModel, pagedList, () => pagedList);

		_logger.LogDebug("<<< Exiting PrepareRegisteredCustomersReportListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion

    #endregion
}