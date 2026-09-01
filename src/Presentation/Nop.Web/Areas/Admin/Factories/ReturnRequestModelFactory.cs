using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the return request model factory implementation
/// </summary>
public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDownloadService _downloadService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;

    #endregion

    #region Ctor

    public ReturnRequestModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        IDateTimeHelper dateTimeHelper,
        IDownloadService downloadService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IOrderService orderService,
        IProductService productService,
        IReturnRequestService returnRequestService)
    {
		_logger.LogDebug(">>> Entering ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService})", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _baseAdminModelFactory → {_baseAdminModelFactory}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _baseAdminModelFactory);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _dateTimeHelper → {_dateTimeHelper}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _dateTimeHelper);
        _downloadService = downloadService;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _downloadService → {_downloadService}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _downloadService);
        _customerService = customerService;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _customerService → {_customerService}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _customerService);
        _localizationService = localizationService;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _localizationService → {_localizationService}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _localizedModelFactory → {_localizedModelFactory}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _localizedModelFactory);
        _orderService = orderService;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _orderService → {_orderService}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _orderService);
        _productService = productService;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _productService → {_productService}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _productService);
        _returnRequestService = returnRequestService;
		_logger.LogDebug("ReturnRequestModelFactory(baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},downloadService={DownloadService},customerService={CustomerService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},orderService={OrderService},productService={ProductService},returnRequestService={ReturnRequestService}): _returnRequestService → {_returnRequestService}", baseAdminModelFactory, dateTimeHelper, downloadService, customerService, localizationService, localizedModelFactory, orderService, productService, returnRequestService, _returnRequestService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare return request search model
    /// </summary>
    /// <param name="searchModel">Return request search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request search model
    /// </returns>
    public virtual async Task<ReturnRequestSearchModel> PrepareReturnRequestSearchModelAsync(ReturnRequestSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available return request statuses
        await _baseAdminModelFactory.PrepareReturnRequestStatusesAsync(searchModel.ReturnRequestStatusList, false);

        //for some reason, the standard default value (0) for the "All" item is already used for the "Pending" status, so here we use -1
        searchModel.ReturnRequestStatusId = -1;
		_logger.LogDebug("PrepareReturnRequestSearchModelAsync(searchModel={SearchModel}): searchModel.ReturnRequestStatusId → {Arg1}", searchModel, searchModel.ReturnRequestStatusId);
        searchModel.ReturnRequestStatusList.Insert(0, new SelectListItem
        {
            Value = "-1",
            Text = await _localizationService.GetResourceAsync("Admin.ReturnRequests.SearchReturnRequestStatus.All")
        });

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareReturnRequestSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged return request list model
    /// </summary>
    /// <param name="searchModel">Return request search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request list model
    /// </returns>
    public virtual async Task<ReturnRequestListModel> PrepareReturnRequestListModelAsync(ReturnRequestSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter emails
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var returnRequestStatus = searchModel.ReturnRequestStatusId == -1 ? null : (ReturnRequestStatus?)searchModel.ReturnRequestStatusId;
		_logger.LogDebug("PrepareReturnRequestListModelAsync(searchModel={SearchModel}): returnRequestStatus → {ReturnRequestStatus}", searchModel, returnRequestStatus);

        //get return requests
        var returnRequests = await _returnRequestService.SearchReturnRequestsAsync(customNumber: searchModel.CustomNumber,
            rs: returnRequestStatus,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new ReturnRequestListModel().PrepareToGridAsync(searchModel, returnRequests, () =>
        {
			_logger.LogDebug("PrepareReturnRequestListModelAsync(searchModel={SearchModel}): returnRequest → {ReturnRequest}", searchModel, returnRequest);
		_logger.LogDebug("<<< Exiting PrepareReturnRequestListModelAsync(searchModel={SearchModel})", searchModel);
            return returnRequests.SelectAwait(async returnRequest => await PrepareReturnRequestModelAsync(null, returnRequest));
        });

        return model;
    }

    /// <summary>
    /// Prepare return request model
    /// </summary>
    /// <param name="model">Return request model</param>
    /// <param name="returnRequest">Return request</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request model
    /// </returns>
    public virtual async Task<ReturnRequestModel> PrepareReturnRequestModelAsync(ReturnRequestModel model,
        ReturnRequest returnRequest, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties})", model, returnRequest, excludeProperties);
        if (returnRequest == null)
            return model;

        //fill in model values from the entity
        model ??= returnRequest.ToModel<ReturnRequestModel>();

        var customer = await _customerService.GetCustomerByIdAsync(returnRequest.CustomerId);
		_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): customer → {Customer}", model, returnRequest, excludeProperties, customer);

        model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(returnRequest.CreatedOnUtc, DateTimeKind.Utc);

        model.CustomerInfo = await _customerService.IsRegisteredAsync(customer)
            ? customer.Email : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
        model.UploadedFileGuid = (await _downloadService.GetDownloadByIdAsync(returnRequest.UploadedFileId))?.DownloadGuid ?? Guid.Empty;
        model.ReturnRequestStatusStr = await _localizationService.GetLocalizedEnumAsync(returnRequest.ReturnRequestStatus);
        var orderItem = await _orderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
		_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): orderItem → {OrderItem}", model, returnRequest, excludeProperties, orderItem);
        if (orderItem != null)
        {
            var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);
			_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): order → {Order}", model, returnRequest, excludeProperties, order);
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
			_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): product → {Product}", model, returnRequest, excludeProperties, product);

            model.ProductId = product.Id;
			_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.ProductId → {Arg3}", model, returnRequest, excludeProperties, model.ProductId);
			_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.ProductName → {Arg3}", model, returnRequest, excludeProperties, model.ProductName);
			_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.OrderId → {Arg3}", model, returnRequest, excludeProperties, model.OrderId);
			_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.AttributeInfo → {Arg3}", model, returnRequest, excludeProperties, model.AttributeInfo);
			_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.CustomOrderNumber → {Arg3}", model, returnRequest, excludeProperties, model.CustomOrderNumber);
            model.ProductName = product.Name;
            model.OrderId = order.Id;
            model.AttributeInfo = orderItem.AttributeDescription;
            model.CustomOrderNumber = order.CustomOrderNumber;
        }

        if (excludeProperties)
            return model;

        model.ReasonForReturn = returnRequest.ReasonForReturn;
		_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.ReasonForReturn → {Arg3}", model, returnRequest, excludeProperties, model.ReasonForReturn);
		_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.RequestedAction → {Arg3}", model, returnRequest, excludeProperties, model.RequestedAction);
		_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.CustomerComments → {Arg3}", model, returnRequest, excludeProperties, model.CustomerComments);
		_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.StaffNotes → {Arg3}", model, returnRequest, excludeProperties, model.StaffNotes);
		_logger.LogDebug("PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties}): model.ReturnRequestStatusId → {Arg3}", model, returnRequest, excludeProperties, model.ReturnRequestStatusId);
        model.RequestedAction = returnRequest.RequestedAction;
        model.CustomerComments = returnRequest.CustomerComments;
        model.StaffNotes = returnRequest.StaffNotes;
        model.ReturnRequestStatusId = returnRequest.ReturnRequestStatusId;

		_logger.LogDebug("<<< Exiting PrepareReturnRequestModelAsync(model={Model},returnRequest={ReturnRequest},excludeProperties={ExcludeProperties})", model, returnRequest, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare return request reason search model
    /// </summary>
    /// <param name="searchModel">Return request reason search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request reason search model
    /// </returns>
    public virtual Task<ReturnRequestReasonSearchModel> PrepareReturnRequestReasonSearchModelAsync(ReturnRequestReasonSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestReasonSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareReturnRequestReasonSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged return request reason list model
    /// </summary>
    /// <param name="searchModel">Return request reason search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request reason list model
    /// </returns>
    public virtual async Task<ReturnRequestReasonListModel> PrepareReturnRequestReasonListModelAsync(ReturnRequestReasonSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestReasonListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get return request reasons
        var reasons = (await _returnRequestService.GetAllReturnRequestReasonsAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareReturnRequestReasonListModelAsync(searchModel={SearchModel}): reasons → {Reasons}", searchModel, reasons);

        //prepare list model
        var model = new ReturnRequestReasonListModel().PrepareToGrid(searchModel, reasons, () =>
        {
		_logger.LogDebug("<<< Exiting PrepareReturnRequestReasonListModelAsync(searchModel={SearchModel})", searchModel);
            return reasons.Select(reason => reason.ToModel<ReturnRequestReasonModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare return request reason model
    /// </summary>
    /// <param name="model">Return request reason model</param>
    /// <param name="returnRequestReason">Return request reason</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request reason model
    /// </returns>
    public virtual async Task<ReturnRequestReasonModel> PrepareReturnRequestReasonModelAsync(ReturnRequestReasonModel model,
        ReturnRequestReason returnRequestReason, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestReasonModelAsync(model={Model},returnRequestReason={ReturnRequestReason},excludeProperties={ExcludeProperties})", model, returnRequestReason, excludeProperties);
        Func<ReturnRequestReasonLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (returnRequestReason != null)
        {
            //fill in model values from the entity
            model ??= returnRequestReason.ToModel<ReturnRequestReasonModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(returnRequestReason, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareReturnRequestReasonModelAsync(model={Model},returnRequestReason={ReturnRequestReason},excludeProperties={ExcludeProperties})", model, returnRequestReason, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare return request action search model
    /// </summary>
    /// <param name="searchModel">Return request action search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request action search model
    /// </returns>
    public virtual Task<ReturnRequestActionSearchModel> PrepareReturnRequestActionSearchModelAsync(ReturnRequestActionSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestActionSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareReturnRequestActionSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged return request action list model
    /// </summary>
    /// <param name="searchModel">Return request action search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request action list model
    /// </returns>
    public virtual async Task<ReturnRequestActionListModel> PrepareReturnRequestActionListModelAsync(ReturnRequestActionSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestActionListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get return request actions
        var actions = (await _returnRequestService.GetAllReturnRequestActionsAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareReturnRequestActionListModelAsync(searchModel={SearchModel}): actions → {Actions}", searchModel, actions);

        //prepare list model
        var model = new ReturnRequestActionListModel().PrepareToGrid(searchModel, actions, () =>
        {
		_logger.LogDebug("<<< Exiting PrepareReturnRequestActionListModelAsync(searchModel={SearchModel})", searchModel);
            return actions.Select(reason => reason.ToModel<ReturnRequestActionModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare return request action model
    /// </summary>
    /// <param name="model">Return request action model</param>
    /// <param name="returnRequestAction">Return request action</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request action model
    /// </returns>
    public virtual async Task<ReturnRequestActionModel> PrepareReturnRequestActionModelAsync(ReturnRequestActionModel model,
        ReturnRequestAction returnRequestAction, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareReturnRequestActionModelAsync(model={Model},returnRequestAction={ReturnRequestAction},excludeProperties={ExcludeProperties})", model, returnRequestAction, excludeProperties);
        Func<ReturnRequestActionLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (returnRequestAction != null)
        {
            //fill in model values from the entity
            model ??= returnRequestAction.ToModel<ReturnRequestActionModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(returnRequestAction, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareReturnRequestActionModelAsync(model={Model},returnRequestAction={ReturnRequestAction},excludeProperties={ExcludeProperties})", model, returnRequestAction, excludeProperties);
        return model;
    }

    #endregion
}