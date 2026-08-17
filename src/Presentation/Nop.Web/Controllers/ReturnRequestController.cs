using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Models.Order;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class ReturnRequestController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ICustomNumberFormatter _customNumberFormatter;
    protected readonly IDownloadService _downloadService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IReturnRequestModelFactory _returnRequestModelFactory;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly OrderSettings _orderSettings;

    #endregion

    #region Ctor

    public ReturnRequestController(ICustomerService customerService,
        ICustomNumberFormatter customNumberFormatter,
        IDownloadService downloadService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IReturnRequestModelFactory returnRequestModelFactory,
        IReturnRequestService returnRequestService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        OrderSettings orderSettings)
    {
		_logger.LogDebug(">>> Entering ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings})", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings);
        _customerService = customerService;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _customerService → {_customerService}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _customerService);
        _customNumberFormatter = customNumberFormatter;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _customNumberFormatter → {_customNumberFormatter}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _customNumberFormatter);
        _downloadService = downloadService;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _downloadService → {_downloadService}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _downloadService);
        _localizationService = localizationService;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _localizationService → {_localizationService}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _localizationService);
        _fileProvider = fileProvider;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _fileProvider → {_fileProvider}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _fileProvider);
        _orderProcessingService = orderProcessingService;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _orderProcessingService → {_orderProcessingService}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _orderProcessingService);
        _orderService = orderService;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _orderService → {_orderService}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _orderService);
        _returnRequestModelFactory = returnRequestModelFactory;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _returnRequestModelFactory → {_returnRequestModelFactory}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _returnRequestModelFactory);
        _returnRequestService = returnRequestService;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _returnRequestService → {_returnRequestService}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _returnRequestService);
        _storeContext = storeContext;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _storeContext → {_storeContext}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _workContext → {_workContext}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _workflowMessageService → {_workflowMessageService}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _workflowMessageService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _localizationSettings → {_localizationSettings}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _localizationSettings);
        _orderSettings = orderSettings;
		_logger.LogDebug("<<< Exiting ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings})", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings);
		_logger.LogDebug("ReturnRequestController(customerService={CustomerService},customNumberFormatter={CustomNumberFormatter},downloadService={DownloadService},localizationService={LocalizationService},fileProvider={FileProvider},orderProcessingService={OrderProcessingService},orderService={OrderService},returnRequestModelFactory={ReturnRequestModelFactory},returnRequestService={ReturnRequestService},storeContext={StoreContext},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},orderSettings={OrderSettings}): _orderSettings → {_orderSettings}", customerService, customNumberFormatter, downloadService, localizationService, fileProvider, orderProcessingService, orderService, returnRequestModelFactory, returnRequestService, storeContext, workContext, workflowMessageService, localizationSettings, orderSettings, _orderSettings);
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> CustomerReturnRequests()
    {
		_logger.LogDebug(">>> Entering CustomerReturnRequests()");
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = await _returnRequestModelFactory.PrepareCustomerReturnRequestsModelAsync();
		_logger.LogDebug("CustomerReturnRequests(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting CustomerReturnRequests()");
        return View(model);
    }

    public virtual async Task<IActionResult> ReturnRequest(int orderId)
    {
		_logger.LogDebug(">>> Entering ReturnRequest(orderId={OrderId})", orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("ReturnRequest(orderId={OrderId}): customer → {Customer}", orderId, customer);
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
			_logger.LogDebug("ReturnRequest(orderId={OrderId}): order → {Order}", orderId, order);
            return Challenge();

        if (!await _orderProcessingService.IsReturnRequestAllowedAsync(order))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new SubmitReturnRequestModel();
		_logger.LogDebug("ReturnRequest(orderId={OrderId}): model → {Model}", orderId, model);
        model = await _returnRequestModelFactory.PrepareSubmitReturnRequestModelAsync(model, order);
		_logger.LogDebug("<<< Exiting ReturnRequest(orderId={OrderId})", orderId);
        return View(model);
    }

    [HttpPost, ActionName("ReturnRequest")]
    public virtual async Task<IActionResult> ReturnRequestSubmit(int orderId, SubmitReturnRequestModel model, IFormCollection form)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        if (!await _orderProcessingService.IsReturnRequestAllowedAsync(order))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var count = 0;

        var downloadId = 0;
        if (_orderSettings.ReturnRequestsAllowFiles)
        {
            var download = await _downloadService.GetDownloadByGuidAsync(model.UploadedFileGuid);
            if (download != null)
                downloadId = download.Id;
        }

        //returnable products
        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isNotReturnable: false);
        foreach (var orderItem in orderItems)
        {
            var quantity = 0; //parse quantity
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"quantity{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out quantity);
                    break;
                }
            if (quantity > 0)
            {
                var rrr = await _returnRequestService.GetReturnRequestReasonByIdAsync(model.ReturnRequestReasonId);
                var rra = await _returnRequestService.GetReturnRequestActionByIdAsync(model.ReturnRequestActionId);
                var store = await _storeContext.GetCurrentStoreAsync();

                var rr = new ReturnRequest
                {
                    CustomNumber = "",
                    StoreId = store.Id,
                    OrderItemId = orderItem.Id,
                    Quantity = quantity,
                    CustomerId = customer.Id,
                    ReasonForReturn = rrr != null ? await _localizationService.GetLocalizedAsync(rrr, x => x.Name) : "not available",
                    RequestedAction = rra != null ? await _localizationService.GetLocalizedAsync(rra, x => x.Name) : "not available",
                    CustomerComments = model.Comments,
                    UploadedFileId = downloadId,
                    StaffNotes = string.Empty,
                    ReturnRequestStatus = ReturnRequestStatus.Pending,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };

                await _returnRequestService.InsertReturnRequestAsync(rr);

                //set return request custom number
                rr.CustomNumber = _customNumberFormatter.GenerateReturnRequestCustomNumber(rr);
                await _customerService.UpdateCustomerAsync(customer);
                await _returnRequestService.UpdateReturnRequestAsync(rr);

                //notify store owner
                await _workflowMessageService.SendNewReturnRequestStoreOwnerNotificationAsync(rr, orderItem, order, _localizationSettings.DefaultAdminLanguageId);
                //notify customer
                await _workflowMessageService.SendNewReturnRequestCustomerNotificationAsync(rr, orderItem, order);

                count++;
            }
        }

        model = await _returnRequestModelFactory.PrepareSubmitReturnRequestModelAsync(model, order);
		_logger.LogDebug("ReturnRequestSubmit(orderId={OrderId},model={Model},form={Form}): model → {Model1}", orderId, model, form, model);
        if (count > 0)
            model.Result = await _localizationService.GetResourceAsync("ReturnRequests.Submitted");
        else
            model.Result = await _localizationService.GetResourceAsync("ReturnRequests.NoItemsSubmitted");

		_logger.LogDebug("<<< Exiting ReturnRequestSubmit(orderId={OrderId},model={Model},form={Form})", orderId, model, form);
        return View(model);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> UploadFileReturnRequest()
    {
		_logger.LogDebug(">>> Entering UploadFileReturnRequest()");
        if (!_orderSettings.ReturnRequestsEnabled || !_orderSettings.ReturnRequestsAllowFiles)
        {
            return Json(new
            {
                success = false,
                downloadGuid = Guid.Empty,
            });
				_logger.LogDebug("UploadFileReturnRequest(): success → {Success}", success);
				_logger.LogDebug("UploadFileReturnRequest(): downloadGuid → {DownloadGuid}", downloadGuid);
        }

        var httpPostedFile = await Request.GetFirstOrDefaultFileAsync();
		_logger.LogDebug("UploadFileReturnRequest(): httpPostedFile → {HttpPostedFile}", httpPostedFile);
        if (httpPostedFile == null)
        {
            return Json(new
            {
                success = false,
                message = "No file uploaded",
                downloadGuid = Guid.Empty,
            });
				_logger.LogDebug("UploadFileReturnRequest(): message → {Message}", message);
        }

        var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);
		_logger.LogDebug("UploadFileReturnRequest(): fileBinary → {FileBinary}", fileBinary);

        var fileName = httpPostedFile.FileName;
		_logger.LogDebug("UploadFileReturnRequest(): fileName → {FileName}", fileName);

        //remove path (passed in IE)
        fileName = _fileProvider.GetFileName(fileName);

        var contentType = httpPostedFile.ContentType;

        var fileExtension = _fileProvider.GetFileExtension(fileName);
		_logger.LogDebug("UploadFileReturnRequest(): fileExtension → {FileExtension}", fileExtension);
        if (!string.IsNullOrEmpty(fileExtension))
            fileExtension = fileExtension.ToLowerInvariant();

        var validationFileMaximumSize = _orderSettings.ReturnRequestsFileMaximumSize;
        if (validationFileMaximumSize > 0)
        {
            //compare in bytes
            var maxFileSizeBytes = validationFileMaximumSize * 1024;
            if (fileBinary.Length > maxFileSizeBytes)
            {
                return Json(new
                {
                    success = false,
                    message = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), validationFileMaximumSize),
                    downloadGuid = Guid.Empty,
                });
            }
        }

        var download = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            UseDownloadUrl = false,
            DownloadUrl = "",
            DownloadBinary = fileBinary,
            ContentType = contentType,
            //we store filename without extension for downloads
            Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
            Extension = fileExtension,
            IsNew = true
        };
        await _downloadService.InsertDownloadAsync(download);

        //when returning JSON the mime-type must be set to text/plain
        //otherwise some browsers will pop-up a "Save As" dialog.
		_logger.LogDebug("<<< Exiting UploadFileReturnRequest()");
        return Json(new
        {
            success = true,
            message = await _localizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
            downloadUrl = Url.RouteUrl(NopRouteNames.Standard.DOWNLOAD_GET_FILE_UPLOAD, new { downloadId = download.DownloadGuid }),
            downloadGuid = download.DownloadGuid,
        });
    }

    #endregion
}