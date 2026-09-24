using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Models.Order;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the return request model factory
/// </summary>
public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDownloadService _downloadService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;

    #endregion

    #region Ctor

    public ReturnRequestModelFactory(ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        IDownloadService downloadService,
        ILocalizationService localizationService,
        IOrderService orderService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IReturnRequestService returnRequestService,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        OrderSettings orderSettings)
    {
		_logger.LogError(">>> Entering ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings})", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings);
        _currencyService = currencyService;
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _dateTimeHelper → {_dateTimeHelper}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _dateTimeHelper);
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _currencyService → {_currencyService}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _currencyService);
    _logger.LogWarning("<<< Exiting ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings})", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings);
        _downloadService = downloadService;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _downloadService → {_downloadService}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _downloadService);
        _localizationService = localizationService;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _localizationService → {_localizationService}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _localizationService);
        _orderService = orderService;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _orderService → {_orderService}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _orderService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _priceFormatter → {_priceFormatter}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _priceFormatter);
        _productService = productService;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _productService → {_productService}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _productService);
        _returnRequestService = returnRequestService;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _returnRequestService → {_returnRequestService}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _returnRequestService);
        _storeContext = storeContext;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _storeContext → {_storeContext}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _storeContext);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _urlRecordService → {_urlRecordService}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _urlRecordService);
        _workContext = workContext;
		_logger.LogDebug("ReturnRequestModelFactory(currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},returnRequestService={ReturnRequestService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},orderSettings={OrderSettings}): _workContext → {_workContext}", currencyService, dateTimeHelper, downloadService, localizationService, orderService, priceFormatter, productService, returnRequestService, storeContext, urlRecordService, workContext, orderSettings, _workContext);
        _orderSettings = orderSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the submit return request model
    /// </summary>
    /// <param name="model">Submit return request model</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the submit return request model
    /// </returns>
    public virtual async Task<SubmitReturnRequestModel> PrepareSubmitReturnRequestModelAsync(SubmitReturnRequestModel model,
        Order order)
    {
		_logger.LogError(">>> Entering PrepareSubmitReturnRequestModelAsync(model={Model},order={Order})", model, order);
        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(model);

        model.OrderId = order.Id;
		_logger.LogDebug("PrepareSubmitReturnRequestModelAsync(model={Model},order={Order}): model.OrderId → {Arg2}", model, order, model.OrderId);
		_logger.LogDebug("PrepareSubmitReturnRequestModelAsync(model={Model},order={Order}): model.AllowFiles → {Arg2}", model, order, model.AllowFiles);
		_logger.LogDebug("PrepareSubmitReturnRequestModelAsync(model={Model},order={Order}): model.CustomOrderNumber → {Arg2}", model, order, model.CustomOrderNumber);
        model.AllowFiles = _orderSettings.ReturnRequestsAllowFiles;
        model.CustomOrderNumber = order.CustomOrderNumber;

        //return reasons
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        model.AvailableReturnReasons = await (await _returnRequestService.GetAllReturnRequestReasonsAsync())
            .SelectAwait(async rrr => new SubmitReturnRequestModel.ReturnRequestReasonModel
            {
                Id = rrr.Id,
                Name = await _localizationService.GetLocalizedAsync(rrr, x => x.Name)
            }).ToListAsync();

		_logger.LogError("PrepareSubmitReturnRequestModelAsync(model,order)={Model},{Order}: database query executed in {Arg2} ms", model, order, __logStart.ElapsedMilliseconds);
        //return actions
        model.AvailableReturnActions = await (await _returnRequestService.GetAllReturnRequestActionsAsync())
            .SelectAwait(async rra => new SubmitReturnRequestModel.ReturnRequestActionModel
            {
                Id = rra.Id,
                Name = await _localizationService.GetLocalizedAsync(rra, x => x.Name)
            })
            .ToListAsync();

        //returnable products
        model.Items = await PrepareSubmitReturnRequestOrderItemModelsAsync(order);

		_logger.LogWarning("<<< Exiting PrepareSubmitReturnRequestModelAsync(model={Model},order={Order})", model, order);
        return model;
    }

    /// <summary>
    /// Prepare the customer return requests model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer return requests model
    /// </returns>
    public virtual async Task<CustomerReturnRequestsModel> PrepareCustomerReturnRequestsModelAsync()
    {
		_logger.LogError(">>> Entering PrepareCustomerReturnRequestsModelAsync()");
        var model = new CustomerReturnRequestsModel();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareCustomerReturnRequestsModelAsync(): store → {Store}", store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareCustomerReturnRequestsModelAsync(): customer → {Customer}", customer);
        var returnRequests = await _returnRequestService.SearchReturnRequestsAsync(store.Id, customer.Id);
		_logger.LogError("PrepareCustomerReturnRequestsModelAsync(): returnRequests → {ReturnRequests}", returnRequests);
		_logger.LogError("PrepareCustomerReturnRequestsModelAsync(): model → {Model}", model);

        foreach (var returnRequest in returnRequests)
        {
            var orderItem = await _orderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
			_logger.LogError("PrepareCustomerReturnRequestsModelAsync(): orderItem → {OrderItem}", orderItem);
            if (orderItem != null)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
				_logger.LogError("PrepareCustomerReturnRequestsModelAsync(): product → {Product}", product);

                var download = await _downloadService.GetDownloadByIdAsync(returnRequest.UploadedFileId);
				_logger.LogError("PrepareCustomerReturnRequestsModelAsync(): download → {Download}", download);

                var itemModel = new CustomerReturnRequestsModel.ReturnRequestModel
                {
                    Id = returnRequest.Id,
                    CustomNumber = returnRequest.CustomNumber,
                    ReturnRequestStatus = await _localizationService.GetLocalizedEnumAsync(returnRequest.ReturnRequestStatus),
                    ProductId = product.Id,
                    ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    Quantity = returnRequest.Quantity,
                    ReturnAction = returnRequest.RequestedAction,
                    ReturnReason = returnRequest.ReasonForReturn,
                    Comments = returnRequest.CustomerComments,
                    UploadedFileGuid = download?.DownloadGuid ?? Guid.Empty,
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                };
                model.Items.Add(itemModel);
            }
        }

		_logger.LogWarning("<<< Exiting PrepareCustomerReturnRequestsModelAsync()");
		_logger.LogError(">>> Entering PrepareSubmitReturnRequestOrderItemModelsAsync(order={Order})", order);
		_logger.LogWarning("<<< Exiting PrepareSubmitReturnRequestOrderItemModelsAsync(order={Order})", order);
        return model;
    }

    /// <summary>
    /// Prepares the order item models for return request by specified order.
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// The <see cref="Task"/> containing the <see cref="IList{SubmitReturnRequestModel.OrderItemModel}"/>
    /// </returns>
    protected virtual async Task<IList<SubmitReturnRequestModel.OrderItemModel>> PrepareSubmitReturnRequestOrderItemModelsAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var models = new List<SubmitReturnRequestModel.OrderItemModel>();
		_logger.LogError("PrepareSubmitReturnRequestOrderItemModelsAsync(order={Order}): models → {Models}", order, models);

        var returnRequestAvailability = await _returnRequestService.GetReturnRequestAvailabilityAsync(order.Id);
		_logger.LogError("PrepareSubmitReturnRequestOrderItemModelsAsync(order={Order}): returnRequestAvailability → {ReturnRequestAvailability}", order, returnRequestAvailability);
        if (returnRequestAvailability?.IsAllowed == true)
        {
            foreach (var returnableOrderItem in returnRequestAvailability.ReturnableOrderItems)
            {
                if (returnableOrderItem.AvailableQuantityForReturn == 0)
                    continue;

                var orderItem = returnableOrderItem.OrderItem;
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
				_logger.LogError("PrepareSubmitReturnRequestOrderItemModelsAsync(order={Order}): product → {Product}", order, product);

                var model = new SubmitReturnRequestModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = product.Id,
                    ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    AttributeInfo = orderItem.AttributeDescription,
                    Quantity = returnableOrderItem.AvailableQuantityForReturn
                };

                var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
				_logger.LogError("PrepareSubmitReturnRequestOrderItemModelsAsync(order={Order}): languageId → {LanguageId}", order, languageId);

                //unit price
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
					_logger.LogError("PrepareSubmitReturnRequestOrderItemModelsAsync(order={Order}): unitPriceInclTaxInCustomerCurrency → {UnitPriceInclTaxInCustomerCurrency}", order, unitPriceInclTaxInCustomerCurrency);
                    model.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
					_logger.LogError("PrepareSubmitReturnRequestOrderItemModelsAsync(order={Order}): unitPriceExclTaxInCustomerCurrency → {UnitPriceExclTaxInCustomerCurrency}", order, unitPriceExclTaxInCustomerCurrency);
                    model.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }

                models.Add(model);
            }
        }

        return models;
    }

    #endregion
}