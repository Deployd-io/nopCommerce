using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class OrderController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEncryptionService _encryptionService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IExportManager _exportManager;
    protected readonly IGiftCardService _giftCardService;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderModelFactory _orderModelFactory;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentService _paymentService;
    protected readonly IPdfService _pdfService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly OrderSettings _orderSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public OrderController(IAddressService addressService,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IEncryptionService encryptionService,
        IEventPublisher eventPublisher,
        IExportManager exportManager,
        IGiftCardService giftCardService,
        IImportManager importManager,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderModelFactory orderModelFactory,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPaymentService paymentService,
        IPdfService pdfService,
        IPermissionService permissionService,
        IPriceCalculationService priceCalculationService,
        IProductAttributeFormatter productAttributeFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IShipmentService shipmentService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        OrderSettings orderSettings)
    {
        _addressService = addressService;
        _addressAttributeParser = addressAttributeParser;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _encryptionService = encryptionService;
        _eventPublisher = eventPublisher;
        _exportManager = exportManager;
        _giftCardService = giftCardService;
        _importManager = importManager;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _orderModelFactory = orderModelFactory;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _paymentService = paymentService;
        _pdfService = pdfService;
        _permissionService = permissionService;
        _priceCalculationService = priceCalculationService;
        _productAttributeFormatter = productAttributeFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _shipmentService = shipmentService;
        _shippingService = shippingService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _orderSettings = orderSettings;
    }

    #endregion

    #region Utilities

    protected virtual async ValueTask<bool> HasAccessToOrderAsync(Order order)
    {
		_logger.LogDebug(">>> Entering HasAccessToOrderAsync(order={Order})", order);
		_logger.LogDebug("<<< Exiting HasAccessToOrderAsync(order={Order})", order);
		_logger.LogDebug("<<< Exiting HasAccessToOrderAsync(orderId={OrderId})", orderId);
        return order != null && await HasAccessToOrderAsync(order.Id);
    }

    protected virtual async Task<bool> HasAccessToOrderAsync(int orderId)
    {
        if (orderId == 0)
            return false;

        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor == null)
            //not a vendor; has access
            return true;

        var vendorId = currentVendor.Id;
        var hasVendorProducts = (await _orderService.GetOrderItemsAsync(orderId, vendorId: vendorId)).Any();

        return hasVendorProducts;
    }

    protected virtual async ValueTask<bool> HasAccessToProductAsync(OrderItem orderItem)
    {
		_logger.LogDebug(">>> Entering HasAccessToProductAsync(orderItem={OrderItem})", orderItem);
        if (orderItem == null || orderItem.ProductId == 0)
            return false;

        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("HasAccessToProductAsync(orderItem={OrderItem}): currentVendor → {CurrentVendor}", orderItem, currentVendor);
        if (currentVendor == null)
            //not a vendor; has access
            return true;

        var vendorId = currentVendor.Id;

		_logger.LogDebug("<<< Exiting HasAccessToProductAsync(orderItem={OrderItem})", orderItem);
        return (await _productService.GetProductByIdAsync(orderItem.ProductId))?.VendorId == vendorId;
    }

    protected virtual async ValueTask<bool> HasAccessToShipmentAsync(Shipment shipment)
    {
		_logger.LogDebug(">>> Entering HasAccessToShipmentAsync(shipment={Shipment})", shipment);
        ArgumentNullException.ThrowIfNull(shipment);

        if (await _workContext.GetCurrentVendorAsync() == null)
            //not a vendor; has access
		_logger.LogDebug("<<< Exiting HasAccessToShipmentAsync(shipment={Shipment})", shipment);
            return true;

        return await HasAccessToOrderAsync(shipment.OrderId);
    }

    protected virtual async Task LogEditOrderAsync(int orderId)
    {
		_logger.LogDebug(">>> Entering LogEditOrderAsync(orderId={OrderId})", orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);

		_logger.LogDebug("LogEditOrderAsync(orderId={OrderId}): order → {Order}", orderId, order);
        await _customerActivityService.InsertActivityAsync("EditOrder",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditOrder"), order.CustomOrderNumber), order);
			_logger.LogDebug("<<< Exiting LogEditOrderAsync(orderId={OrderId})", orderId);
    }

    #endregion

    #region Order list

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
    {
		_logger.LogDebug(">>> Entering List(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses},shippingStatuses=<redacted>)", orderStatuses, paymentStatuses);
        //prepare model
        var model = await _orderModelFactory.PrepareOrderSearchModelAsync(new OrderSearchModel
        {
            OrderStatusIds = orderStatuses,
            PaymentStatusIds = paymentStatuses,
            ShippingStatusIds = shippingStatuses
        });

		_logger.LogDebug("<<< Exiting List(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses},shippingStatuses=<redacted>)", orderStatuses, paymentStatuses);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> OrderList(OrderSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering OrderList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _orderModelFactory.PrepareOrderListModelAsync(searchModel);
		_logger.LogDebug("OrderList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting OrderList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> ReportAggregates(OrderSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ReportAggregates(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _orderModelFactory.PrepareOrderAggregatorModelAsync(searchModel);
		_logger.LogDebug("ReportAggregates(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ReportAggregates(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("go-to-order-by-number")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> GoToOrderId(OrderSearchModel model)
    {
		_logger.LogDebug(">>> Entering GoToOrderId(model={Model})", model);
        var order = await _orderService.GetOrderByCustomOrderNumberAsync(model.GoDirectlyToCustomOrderNumber);

		_logger.LogDebug("GoToOrderId(model={Model}): order → {Order}", model, order);
        if (order == null)
            return await List();

        return RedirectToAction("Edit", new { id = order.Id });
    }

    #endregion

    #region Export / Import

    [HttpPost, ActionName("ExportXml")]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXmlAll(OrderSearchModel model)
    {
		_logger.LogDebug(">>> Entering ExportXmlAll(model={Model})", model);
        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("ExportXmlAll(model={Model}): currentVendor → {CurrentVendor}", model, currentVendor);
        if (currentVendor != null)
        {
            model.VendorId = currentVendor.Id;
			_logger.LogDebug("ExportXmlAll(model={Model}): model.VendorId → {Arg1}", model, model.VendorId);
        }

        var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
            ? model.OrderStatusIds.ToList()
            : null;
        var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
            ? model.PaymentStatusIds.ToList()
            : null;
        var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
            ? model.ShippingStatusIds.ToList()
            : null;

        var filterByProductId = 0;
		_logger.LogDebug("ExportXmlAll(model={Model}): filterByProductId → {FilterByProductId}", model, filterByProductId);
        var product = await _productService.GetProductByIdAsync(model.ProductId);
		_logger.LogDebug("ExportXmlAll(model={Model}): product → {Product}", model, product);
        if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
            filterByProductId = model.ProductId;

        //load orders
        var orders = await _orderService.SearchOrdersAsync(storeId: model.StoreId,
            vendorId: model.VendorId,
            productId: filterByProductId,
            warehouseId: model.WarehouseId,
            paymentMethodSystemName: model.PaymentMethodSystemName,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            billingPhone: model.BillingPhone,
            billingEmail: model.BillingEmail,
            billingLastName: model.BillingLastName,
            billingCountryId: model.BillingCountryId,
            orderNotes: model.OrderNotes);

        //ensure that we at least one order selected
        if (!orders.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.NoOrders"));
            return RedirectToAction("List");
        }

        try
        {
            var xml = await _exportManager.ExportOrdersToXmlAsync(orders);
			_logger.LogDebug("ExportXmlAll(model={Model}): xml → {Xml}", model, xml);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "orders.xml");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportXmlAll(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ExportXmlAll(model={Model})", model);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
    {
		_logger.LogDebug(">>> Entering ExportXmlSelected(selectedIds={SelectedIds})", selectedIds);
        var orders = new List<Order>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
            orders.AddRange(await (await _orderService.GetOrdersByIdsAsync(ids))
                .WhereAwait(HasAccessToOrderAsync).ToListAsync());
				_logger.LogInformation("ExportXmlSelected(selectedIds)={SelectedIds}: database query executed in {Arg1} ms", selectedIds, __logStart.ElapsedMilliseconds);
        }

        try
        {
            var xml = await _exportManager.ExportOrdersToXmlAsync(orders);
			_logger.LogDebug("ExportXmlSelected(selectedIds={SelectedIds}): xml → {Xml}", selectedIds, xml);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "orders.xml");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportXmlSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ExportXmlSelected(selectedIds={SelectedIds})", selectedIds);
            return RedirectToAction("List");
        }
    }

    [HttpPost, ActionName("ExportExcel")]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelAll(OrderSearchModel model)
    {
		_logger.LogDebug(">>> Entering ExportExcelAll(model={Model})", model);
        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("ExportExcelAll(model={Model}): currentVendor → {CurrentVendor}", model, currentVendor);
        if (currentVendor != null)
        {
            model.VendorId = currentVendor.Id;
			_logger.LogDebug("ExportExcelAll(model={Model}): model.VendorId → {Arg1}", model, model.VendorId);
        }

        var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
            ? model.OrderStatusIds.ToList()
            : null;
        var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
            ? model.PaymentStatusIds.ToList()
            : null;
        var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
            ? model.ShippingStatusIds.ToList()
            : null;

        var filterByProductId = 0;
		_logger.LogDebug("ExportExcelAll(model={Model}): filterByProductId → {FilterByProductId}", model, filterByProductId);
        var product = await _productService.GetProductByIdAsync(model.ProductId);
		_logger.LogDebug("ExportExcelAll(model={Model}): product → {Product}", model, product);
        if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
            filterByProductId = model.ProductId;

        //load orders
        var orders = await _orderService.SearchOrdersAsync(storeId: model.StoreId,
            vendorId: model.VendorId,
            productId: filterByProductId,
            warehouseId: model.WarehouseId,
            paymentMethodSystemName: model.PaymentMethodSystemName,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            billingPhone: model.BillingPhone,
            billingEmail: model.BillingEmail,
            billingLastName: model.BillingLastName,
            billingCountryId: model.BillingCountryId,
            orderNotes: model.OrderNotes);

        //ensure that we at least one order selected
        if (!orders.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.NoOrders"));
            return RedirectToAction("List");
        }

        try
        {
            var bytes = await _exportManager.ExportOrdersToXlsxAsync(orders);
			_logger.LogDebug("ExportExcelAll(model={Model}): bytes → {Bytes}", model, bytes);
            return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportExcelAll(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ExportExcelAll(model={Model})", model);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
    {
		_logger.LogDebug(">>> Entering ExportExcelSelected(selectedIds={SelectedIds})", selectedIds);
        var orders = new List<Order>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogInformation("ExportExcelSelected(selectedIds)={SelectedIds}: find query executed in {Arg1} ms", selectedIds, __logStart.ElapsedMilliseconds);
            orders.AddRange(await (await _orderService.GetOrdersByIdsAsync(ids)).WhereAwait(HasAccessToOrderAsync).ToListAsync());
        }

        try
        {
            var bytes = await _exportManager.ExportOrdersToXlsxAsync(orders);
			_logger.LogDebug("ExportExcelSelected(selectedIds={SelectedIds}): bytes → {Bytes}", selectedIds, bytes);
            return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportExcelSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ExportExcelSelected(selectedIds={SelectedIds})", selectedIds);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
		_logger.LogDebug(">>> Entering ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
        //a vendor cannot import orders
        if (await _workContext.GetCurrentVendorAsync() != null)
            return AccessDeniedView();

        try
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                await _importManager.ImportOrdersFromXlsxAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Orders.Imported"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ImportFromXlsx(importexcelfile={Importexcelfile}): {Arg1}", importexcelfile, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ImportFromXlsx(importexcelfile={Importexcelfile})", importexcelfile);
            return RedirectToAction("List");
        }
    }

    #endregion

    #region Order details

    #region Payments and other order workflow

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("cancelorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CancelOrder(int id)
    {
		_logger.LogDebug(">>> Entering CancelOrder(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("CancelOrder(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            await _orderProcessingService.CancelOrderAsync(order, true);
            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in CancelOrder(id={Id}): {Arg1}", id, exc.Message);
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
			_logger.LogDebug("CancelOrder(id={Id}): model → {Model}", id, model);

            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting CancelOrder(id={Id})", id);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("captureorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CaptureOrder(int id)
    {
		_logger.LogDebug(">>> Entering CaptureOrder(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("CaptureOrder(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var errors = await _orderProcessingService.CaptureAsync(order);
			_logger.LogDebug("CaptureOrder(id={Id}): errors → {Errors}", id, errors);
            await LogEditOrderAsync(order.Id);

            foreach (var error in errors)
                _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in CaptureOrder(id={Id}): {Arg1}", id, exc.Message);
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
			_logger.LogDebug("CaptureOrder(id={Id}): model → {Model}", id, model);

            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting CaptureOrder(id={Id})", id);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("markorderaspaid")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MarkOrderAsPaid(int id)
    {
		_logger.LogDebug(">>> Entering MarkOrderAsPaid(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("MarkOrderAsPaid(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            await _orderProcessingService.MarkOrderAsPaidAsync(order);
            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in MarkOrderAsPaid(id={Id}): {Arg1}", id, exc.Message);
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
			_logger.LogDebug("MarkOrderAsPaid(id={Id}): model → {Model}", id, model);

            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting MarkOrderAsPaid(id={Id})", id);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("refundorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RefundOrder(int id)
    {
		_logger.LogDebug(">>> Entering RefundOrder(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("RefundOrder(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var errors = await _orderProcessingService.RefundAsync(order);
			_logger.LogDebug("RefundOrder(id={Id}): errors → {Errors}", id, errors);
            await LogEditOrderAsync(order.Id);

            foreach (var error in errors)
                _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in RefundOrder(id={Id}): {Arg1}", id, exc.Message);
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
			_logger.LogDebug("RefundOrder(id={Id}): model → {Model}", id, model);

            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting RefundOrder(id={Id})", id);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("refundorderoffline")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RefundOrderOffline(int id)
    {
		_logger.LogDebug(">>> Entering RefundOrderOffline(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("RefundOrderOffline(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            await _orderProcessingService.RefundOfflineAsync(order);
            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in RefundOrderOffline(id={Id}): {Arg1}", id, exc.Message);
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
			_logger.LogDebug("RefundOrderOffline(id={Id}): model → {Model}", id, model);

            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting RefundOrderOffline(id={Id})", id);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("voidorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> VoidOrder(int id)
    {
		_logger.LogDebug(">>> Entering VoidOrder(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("VoidOrder(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var errors = await _orderProcessingService.VoidAsync(order);
			_logger.LogDebug("VoidOrder(id={Id}): errors → {Errors}", id, errors);
            await LogEditOrderAsync(order.Id);

            foreach (var error in errors)
                _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in VoidOrder(id={Id}): {Arg1}", id, exc.Message);
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
			_logger.LogDebug("VoidOrder(id={Id}): model → {Model}", id, model);

            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting VoidOrder(id={Id})", id);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("voidorderoffline")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> VoidOrderOffline(int id)
    {
		_logger.LogDebug(">>> Entering VoidOrderOffline(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("VoidOrderOffline(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            await _orderProcessingService.VoidOfflineAsync(order);
            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in VoidOrderOffline(id={Id}): {Arg1}", id, exc.Message);
            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
			_logger.LogDebug("VoidOrderOffline(id={Id}): model → {Model}", id, model);

            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting VoidOrderOffline(id={Id})", id);
            return View(model);
        }
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PartiallyRefundOrderPopup(int id, bool online)
    {
		_logger.LogDebug(">>> Entering PartiallyRefundOrderPopup(id={Id},online={Online})", id, online);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("PartiallyRefundOrderPopup(id={Id},online={Online}): order → {Order}", id, online, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        //prepare model
        var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
		_logger.LogDebug("PartiallyRefundOrderPopup(id={Id},online={Online}): model → {Model}", id, online, model);

		_logger.LogDebug("<<< Exiting PartiallyRefundOrderPopup(id={Id},online={Online})", id, online);
		_logger.LogDebug("<<< Exiting PartiallyRefundOrderPopup(id={Id},online={Online},model={Model})", id, online, model);
        return View(model);
    }

    [HttpPost]
    [FormValueRequired("partialrefundorder")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PartiallyRefundOrderPopup(int id, bool online, OrderModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var amountToRefund = model.AmountToRefund;
            if (amountToRefund <= decimal.Zero)
                throw new NopException("Enter amount to refund");

            var maxAmountToRefund = order.OrderTotal - order.RefundedAmount;
            if (amountToRefund > maxAmountToRefund)
                amountToRefund = maxAmountToRefund;

            var errors = new List<string>();
            if (online)
                errors = (await _orderProcessingService.PartiallyRefundAsync(order, amountToRefund)).ToList();
            else
                await _orderProcessingService.PartiallyRefundOfflineAsync(order, amountToRefund);

            await LogEditOrderAsync(order.Id);

            if (!errors.Any())
            {
                //success
                ViewBag.RefreshPage = true;

                //prepare model
                model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

                return View(model);
            }

            //prepare model
            model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

            foreach (var error in errors)
                _notificationService.ErrorNotification(error);

            return View(model);
        }
        catch (Exception exc)
        {
            //prepare model
            model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

            await _notificationService.ErrorNotificationAsync(exc);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("btnSaveOrderStatus")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ChangeOrderStatus(int id, OrderModel model)
    {
		_logger.LogDebug(">>> Entering ChangeOrderStatus(id={Id},model={Model})", id, model);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("ChangeOrderStatus(id={Id},model={Model}): order → {Order}", id, model, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        try
        {
            var prevOrderStatus = order.OrderStatus;

            order.OrderStatusId = model.OrderStatusId;
			_logger.LogDebug("ChangeOrderStatus(id={Id},model={Model}): order.OrderStatusId → {Arg2}", id, model, order.OrderStatusId);
            await _orderService.UpdateOrderAsync(order);

            await _eventPublisher.PublishAsync(new OrderStatusChangedEvent(order, prevOrderStatus));

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = $"Order status has been edited. New status: {await _localizationService.GetLocalizedEnumAsync(order.OrderStatus)}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ChangeOrderStatus(id={Id},model={Model}): {Arg2}", id, model, exc.Message);
            //prepare model
            model = await _orderModelFactory.PrepareOrderModelAsync(model, order);
			_logger.LogDebug("ChangeOrderStatus(id={Id},model={Model}): model → {Model1}", id, model, model);

            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ChangeOrderStatus(id={Id},model={Model})", id, model);
            return View(model);
        }
    }

    #endregion

    #region Edit, delete

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): order → {Order}", id, order);
        if (order == null || order.Deleted)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
            return RedirectToAction("List");

        //prepare model
        var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        await _orderProcessingService.DeleteOrderAsync(order);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteOrder",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteOrder"), order.Id), order);

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> PdfInvoice(int orderId)
    {
		_logger.LogDebug(">>> Entering PdfInvoice(orderId={OrderId})", orderId);
        //a vendor should have access only to their orders
        if (!await HasAccessToOrderAsync(orderId))
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PdfInvoice(orderId={OrderId}): currentVendor → {CurrentVendor}", orderId, currentVendor);

        var order = await _orderService.GetOrderByIdAsync(orderId);
		_logger.LogDebug("PdfInvoice(orderId={OrderId}): order → {Order}", orderId, order);

        byte[] bytes;
        await using var stream = new MemoryStream();
		_logger.LogDebug("PdfInvoice(orderId={OrderId}): stream → {Stream}", orderId, stream);

        await _pdfService.PrintOrderToPdfAsync(stream, order, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), store: null, vendor: currentVendor);
        bytes = stream.ToArray();

		_logger.LogDebug("PdfInvoice(orderId={OrderId}): bytes → {Bytes}", orderId, bytes);
		_logger.LogDebug("<<< Exiting PdfInvoice(orderId={OrderId})", orderId);
        return File(bytes, MimeTypes.ApplicationPdf, string.Format(await _localizationService.GetResourceAsync("PDFInvoice.FileName"), order.CustomOrderNumber) + ".pdf");
    }

    [HttpPost, ActionName("PdfInvoice")]
    [FormValueRequired("pdf-invoice-all")]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> PdfInvoiceAll(OrderSearchModel model)
    {
		_logger.LogDebug(">>> Entering PdfInvoiceAll(model={Model})", model);
        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PdfInvoiceAll(model={Model}): currentVendor → {CurrentVendor}", model, currentVendor);
        if (currentVendor != null)
        {
            model.VendorId = currentVendor.Id;
			_logger.LogDebug("PdfInvoiceAll(model={Model}): model.VendorId → {Arg1}", model, model.VendorId);
        }

        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
            ? model.OrderStatusIds.ToList()
            : null;
        var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
            ? model.PaymentStatusIds.ToList()
            : null;
        var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
            ? model.ShippingStatusIds.ToList()
            : null;

        var filterByProductId = 0;
		_logger.LogDebug("PdfInvoiceAll(model={Model}): filterByProductId → {FilterByProductId}", model, filterByProductId);
        var product = await _productService.GetProductByIdAsync(model.ProductId);
		_logger.LogDebug("PdfInvoiceAll(model={Model}): product → {Product}", model, product);
        if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
            filterByProductId = model.ProductId;

        //load orders
        var orders = await _orderService.SearchOrdersAsync(storeId: model.StoreId,
            vendorId: model.VendorId,
            productId: filterByProductId,
            warehouseId: model.WarehouseId,
            paymentMethodSystemName: model.PaymentMethodSystemName,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            billingPhone: model.BillingPhone,
            billingEmail: model.BillingEmail,
            billingLastName: model.BillingLastName,
            billingCountryId: model.BillingCountryId,
            orderNotes: model.OrderNotes);

        //ensure that we at least one order selected
        if (!orders.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.NoOrders"));
            return RedirectToAction("List");
        }

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintOrdersToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), currentVendor);
                bytes = stream.ToArray();
            }

            return File(bytes, "application/zip", "orders.zip");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in PdfInvoiceAll(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting PdfInvoiceAll(model={Model})", model);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> PdfInvoiceSelected(string selectedIds)
    {
		_logger.LogDebug(">>> Entering PdfInvoiceSelected(selectedIds={SelectedIds})", selectedIds);
        var orders = new List<Order>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            orders.AddRange(await _orderService.GetOrdersByIdsAsync(ids));
        }

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PdfInvoiceSelected(selectedIds={SelectedIds}): currentVendor → {CurrentVendor}", selectedIds, currentVendor);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (currentVendor != null)
        {
            orders = await orders.WhereAwait(HasAccessToOrderAsync).ToListAsync();
        }
		_logger.LogInformation("PdfInvoiceSelected(selectedIds)={SelectedIds}: database query executed in {Arg1} ms", selectedIds, __logStart.ElapsedMilliseconds);

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintOrdersToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), currentVendor);
                bytes = stream.ToArray();
            }

            return File(bytes, "application/zip", "orders.zip");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in PdfInvoiceSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting PdfInvoiceSelected(selectedIds={SelectedIds})", selectedIds);
            return RedirectToAction("List");
        }
    }

    //currently we use this method on the add product to order details pages
    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> ProductDetails_AttributeChange(int productId, bool validateAttributeConditions, IFormCollection form)
    {
		_logger.LogDebug(">>> Entering ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form})", productId, validateAttributeConditions, form);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return new NullJsonResult();

        var errors = new List<string>();
		_logger.LogDebug("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): errors → {Errors}", productId, validateAttributeConditions, form, errors);
        var attributeXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);
		_logger.LogDebug("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): attributeXml → {AttributeXml}", productId, validateAttributeConditions, form, attributeXml);

        //conditional attributes
        var enabledAttributeMappingIds = new List<int>();
		_logger.LogDebug("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): enabledAttributeMappingIds → {EnabledAttributeMappingIds}", productId, validateAttributeConditions, form, enabledAttributeMappingIds);
        var disabledAttributeMappingIds = new List<int>();
		_logger.LogDebug("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): disabledAttributeMappingIds → {DisabledAttributeMappingIds}", productId, validateAttributeConditions, form, disabledAttributeMappingIds);
        if (validateAttributeConditions)
        {
            var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
			_logger.LogDebug("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): attributes → {Attributes}", productId, validateAttributeConditions, form, attributes);
            foreach (var attribute in attributes)
            {
                var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);
				_logger.LogDebug("ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): conditionMet → {ConditionMet}", productId, validateAttributeConditions, form, conditionMet);
                if (!conditionMet.HasValue)
                    continue;

                if (conditionMet.Value)
                    enabledAttributeMappingIds.Add(attribute.Id);
                else
                    disabledAttributeMappingIds.Add(attribute.Id);
            }
        }

		_logger.LogDebug("<<< Exiting ProductDetails_AttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form})", productId, validateAttributeConditions, form);
        return Json(new
        {
            enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
            disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
            message = errors.Any() ? errors.ToArray() : null
        });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("btnSaveCC")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditCreditCardInfo(int id, OrderModel model)
    {
		_logger.LogDebug(">>> Entering EditCreditCardInfo(id={Id},model={Model})", id, model);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("EditCreditCardInfo(id={Id},model={Model}): order → {Order}", id, model, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        if (order.AllowStoringCreditCardNumber)
        {
            var cardType = model.CardType;
            var cardName = model.CardName;
            var cardNumber = model.CardNumber;
            var cardCvv2 = model.CardCvv2;
            var cardExpirationMonth = model.CardExpirationMonth;
            var cardExpirationYear = model.CardExpirationYear;

            order.CardType = _encryptionService.EncryptText(cardType);
            order.CardName = _encryptionService.EncryptText(cardName);
            order.CardNumber = _encryptionService.EncryptText(cardNumber);
            order.MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(cardNumber));
            order.CardCvv2 = _encryptionService.EncryptText(cardCvv2);
            order.CardExpirationMonth = _encryptionService.EncryptText(cardExpirationMonth);
            order.CardExpirationYear = _encryptionService.EncryptText(cardExpirationYear);
            await _orderService.UpdateOrderAsync(order);
        }

        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Credit card info has been edited",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

		_logger.LogDebug("<<< Exiting EditCreditCardInfo(id={Id},model={Model})", id, model);
        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("btnSaveOrderTotals")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditOrderTotals(int id, OrderModel model)
    {
		_logger.LogDebug(">>> Entering EditOrderTotals(id={Id},model={Model})", id, model);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order → {Order}", id, model, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        order.OrderSubtotalInclTax = model.OrderSubtotalInclTaxValue;
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderSubtotalInclTax → {Arg2}", id, model, order.OrderSubtotalInclTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderSubtotalExclTax → {Arg2}", id, model, order.OrderSubtotalExclTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderSubTotalDiscountInclTax → {Arg2}", id, model, order.OrderSubTotalDiscountInclTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderSubTotalDiscountExclTax → {Arg2}", id, model, order.OrderSubTotalDiscountExclTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderShippingInclTax → {Arg2}", id, model, order.OrderShippingInclTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderShippingExclTax → {Arg2}", id, model, order.OrderShippingExclTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.PaymentMethodAdditionalFeeInclTax → {Arg2}", id, model, order.PaymentMethodAdditionalFeeInclTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.PaymentMethodAdditionalFeeExclTax → {Arg2}", id, model, order.PaymentMethodAdditionalFeeExclTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.TaxRates → {Arg2}", id, model, order.TaxRates);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderTax → {Arg2}", id, model, order.OrderTax);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderDiscount → {Arg2}", id, model, order.OrderDiscount);
		_logger.LogDebug("EditOrderTotals(id={Id},model={Model}): order.OrderTotal → {Arg2}", id, model, order.OrderTotal);
        order.OrderSubtotalExclTax = model.OrderSubtotalExclTaxValue;
        order.OrderSubTotalDiscountInclTax = model.OrderSubTotalDiscountInclTaxValue;
        order.OrderSubTotalDiscountExclTax = model.OrderSubTotalDiscountExclTaxValue;
        order.OrderShippingInclTax = model.OrderShippingInclTaxValue;
        order.OrderShippingExclTax = model.OrderShippingExclTaxValue;
        order.PaymentMethodAdditionalFeeInclTax = model.PaymentMethodAdditionalFeeInclTaxValue;
        order.PaymentMethodAdditionalFeeExclTax = model.PaymentMethodAdditionalFeeExclTaxValue;
        order.TaxRates = model.TaxRatesValue;
        order.OrderTax = model.TaxValue;
        order.OrderDiscount = model.OrderTotalDiscountValue;
        order.OrderTotal = model.OrderTotalValue;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Order totals have been edited",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

		_logger.LogDebug("<<< Exiting EditOrderTotals(id={Id},model={Model})", id, model);
        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("save-shipping-method")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditShippingMethod(int id, OrderModel model)
    {
		_logger.LogDebug(">>> Entering EditShippingMethod(id={Id},model={Model})", id, model);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("EditShippingMethod(id={Id},model={Model}): order → {Order}", id, model, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        order.ShippingMethod = model.ShippingMethod;
		_logger.LogDebug("EditShippingMethod(id={Id},model={Model}): order.ShippingMethod → {Arg2}", id, model, order.ShippingMethod);
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Shipping method has been edited",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

        //selected card
        SaveSelectedCardName("order-billing-shipping");

		_logger.LogDebug("<<< Exiting EditShippingMethod(id={Id},model={Model})", id, model);
        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOrderItem")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditOrderItem(int id, IFormCollection form)
    {
		_logger.LogDebug(">>> Entering EditOrderItem(id={Id},form={Form})", id, form);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("EditOrderItem(id={Id},form={Form}): order → {Order}", id, form, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        //get order item identifier
        var orderItemId = 0;
		_logger.LogDebug("EditOrderItem(id={Id},form={Form}): orderItemId → {OrderItemId}", id, form, orderItemId);
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("btnSaveOrderItem", StringComparison.InvariantCultureIgnoreCase))
                orderItemId = Convert.ToInt32(formValue["btnSaveOrderItem".Length..]);

        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        if (!decimal.TryParse(form["pvUnitPriceInclTax" + orderItemId], NumberStyles.Any, CultureInfo.InvariantCulture, out var unitPriceInclTax))
            unitPriceInclTax = orderItem.UnitPriceInclTax;
			_logger.LogDebug("EditOrderItem(id={Id},form={Form}): unitPriceInclTax → {UnitPriceInclTax}", id, form, unitPriceInclTax);
        if (!decimal.TryParse(form["pvUnitPriceExclTax" + orderItemId], NumberStyles.Any, CultureInfo.InvariantCulture, out var unitPriceExclTax))
            unitPriceExclTax = orderItem.UnitPriceExclTax;
			_logger.LogDebug("EditOrderItem(id={Id},form={Form}): unitPriceExclTax → {UnitPriceExclTax}", id, form, unitPriceExclTax);
        if (!int.TryParse(form["pvQuantity" + orderItemId], out var quantity))
            quantity = orderItem.Quantity;
			_logger.LogDebug("EditOrderItem(id={Id},form={Form}): quantity → {Quantity}", id, form, quantity);
        if (!decimal.TryParse(form["pvDiscountInclTax" + orderItemId], NumberStyles.Any, CultureInfo.InvariantCulture, out var discountInclTax))
            discountInclTax = orderItem.DiscountAmountInclTax;
			_logger.LogDebug("EditOrderItem(id={Id},form={Form}): discountInclTax → {DiscountInclTax}", id, form, discountInclTax);
        if (!decimal.TryParse(form["pvDiscountExclTax" + orderItemId], NumberStyles.Any, CultureInfo.InvariantCulture, out var discountExclTax))
            discountExclTax = orderItem.DiscountAmountExclTax;
			_logger.LogDebug("EditOrderItem(id={Id},form={Form}): discountExclTax → {DiscountExclTax}", id, form, discountExclTax);
        if (!decimal.TryParse(form["pvPriceInclTax" + orderItemId], NumberStyles.Any, CultureInfo.InvariantCulture, out var priceInclTax))
            priceInclTax = orderItem.PriceInclTax;
			_logger.LogDebug("EditOrderItem(id={Id},form={Form}): priceInclTax → {PriceInclTax}", id, form, priceInclTax);
        if (!decimal.TryParse(form["pvPriceExclTax" + orderItemId], NumberStyles.Any, CultureInfo.InvariantCulture, out var priceExclTax))
            priceExclTax = orderItem.PriceExclTax;
			_logger.LogDebug("EditOrderItem(id={Id},form={Form}): priceExclTax → {PriceExclTax}", id, form, priceExclTax);

        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
		_logger.LogDebug("EditOrderItem(id={Id},form={Form}): product → {Product}", id, form, product);

        if (quantity > 0)
        {
            var qtyDifference = orderItem.Quantity - quantity;

            if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
            {
                orderItem.UnitPriceInclTax = unitPriceInclTax;
				_logger.LogDebug("EditOrderItem(id={Id},form={Form}): orderItem.UnitPriceInclTax → {Arg2}", id, form, orderItem.UnitPriceInclTax);
				_logger.LogDebug("EditOrderItem(id={Id},form={Form}): orderItem.UnitPriceExclTax → {Arg2}", id, form, orderItem.UnitPriceExclTax);
				_logger.LogDebug("EditOrderItem(id={Id},form={Form}): orderItem.Quantity → {Arg2}", id, form, orderItem.Quantity);
				_logger.LogDebug("EditOrderItem(id={Id},form={Form}): orderItem.DiscountAmountInclTax → {Arg2}", id, form, orderItem.DiscountAmountInclTax);
				_logger.LogDebug("EditOrderItem(id={Id},form={Form}): orderItem.DiscountAmountExclTax → {Arg2}", id, form, orderItem.DiscountAmountExclTax);
				_logger.LogDebug("EditOrderItem(id={Id},form={Form}): orderItem.PriceInclTax → {Arg2}", id, form, orderItem.PriceInclTax);
				_logger.LogDebug("EditOrderItem(id={Id},form={Form}): orderItem.PriceExclTax → {Arg2}", id, form, orderItem.PriceExclTax);
                orderItem.UnitPriceExclTax = unitPriceExclTax;
                orderItem.Quantity = quantity;
                orderItem.DiscountAmountInclTax = discountInclTax;
                orderItem.DiscountAmountExclTax = discountExclTax;
                orderItem.PriceInclTax = priceInclTax;
                orderItem.PriceExclTax = priceExclTax;
                await _orderService.UpdateOrderItemAsync(orderItem);
            }

            //adjust inventory
            await _productService.AdjustInventoryAsync(product, qtyDifference, orderItem.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));
        }
        else
        {
            //adjust inventory
            await _productService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

            //delete item
            await _orderService.DeleteOrderItemAsync(orderItem);
        }

        //update order totals
        var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
        {
            PriceInclTax = unitPriceInclTax,
            PriceExclTax = unitPriceExclTax,
            DiscountAmountInclTax = discountInclTax,
            DiscountAmountExclTax = discountExclTax,
            SubTotalInclTax = priceInclTax,
            SubTotalExclTax = priceExclTax,
            Quantity = quantity
        };
        await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Order item has been edited",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

        foreach (var warning in updateOrderParameters.Warnings)
            _notificationService.WarningNotification(warning);

        //selected card
        SaveSelectedCardName("order-products");

		_logger.LogDebug("<<< Exiting EditOrderItem(id={Id},form={Form})", id, form);
        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteOrderItem")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteOrderItem(int id, IFormCollection form)
    {
		_logger.LogDebug(">>> Entering DeleteOrderItem(id={Id},form={Form})", id, form);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("DeleteOrderItem(id={Id},form={Form}): order → {Order}", id, form, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        //get order item identifier
        var orderItemId = 0;
		_logger.LogDebug("DeleteOrderItem(id={Id},form={Form}): orderItemId → {OrderItemId}", id, form, orderItemId);
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("btnDeleteOrderItem", StringComparison.InvariantCultureIgnoreCase))
                orderItemId = Convert.ToInt32(formValue["btnDeleteOrderItem".Length..]);

        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        if ((await _giftCardService.GetGiftCardsByPurchasedWithOrderItemIdAsync(orderItem.Id)).Any())
        {
            //we cannot delete an order item with associated gift cards
            //a store owner should delete them first

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.OrderItem.DeleteAssociatedGiftCardRecordError"));
        }
        else
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
			_logger.LogDebug("DeleteOrderItem(id={Id},form={Form}): product → {Product}", id, form, product);

            //adjust inventory
            await _productService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

            //delete item
            await _orderService.DeleteOrderItemAsync(orderItem);

            //update order totals
            var updateOrderParameters = new UpdateOrderParameters(order, orderItem);
			_logger.LogDebug("DeleteOrderItem(id={Id},form={Form}): updateOrderParameters → {UpdateOrderParameters}", id, form, updateOrderParameters);
            await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order item has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            foreach (var warning in updateOrderParameters.Warnings)
                _notificationService.WarningNotification(warning);
        }

        //selected card
        SaveSelectedCardName("order-products");

		_logger.LogDebug("<<< Exiting DeleteOrderItem(id={Id},form={Form})", id, form);
        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ResetDownloadCount(int id, IFormCollection form)
    {
		_logger.LogDebug(">>> Entering ResetDownloadCount(id={Id},form={Form})", id, form);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("ResetDownloadCount(id={Id},form={Form}): order → {Order}", id, form, order);
        if (order == null)
            return RedirectToAction("List");

        //get order item identifier
        var orderItemId = 0;
		_logger.LogDebug("ResetDownloadCount(id={Id},form={Form}): orderItemId → {OrderItemId}", id, form, orderItemId);
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                orderItemId = Convert.ToInt32(formValue["btnResetDownloadCount".Length..]);

        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        orderItem.DownloadCount = 0;
		_logger.LogDebug("ResetDownloadCount(id={Id},form={Form}): orderItem.DownloadCount → {Arg2}", id, form, orderItem.DownloadCount);
        await _orderService.UpdateOrderItemAsync(orderItem);
        await LogEditOrderAsync(order.Id);

        //selected card
        SaveSelectedCardName("order-products");

		_logger.LogDebug("<<< Exiting ResetDownloadCount(id={Id},form={Form})", id, form);
        return RedirectToAction("Edit", new { id = order.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ActivateDownloadItem(int id, IFormCollection form)
    {
		_logger.LogDebug(">>> Entering ActivateDownloadItem(id={Id},form={Form})", id, form);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("ActivateDownloadItem(id={Id},form={Form}): order → {Order}", id, form, order);
        if (order == null)
            return RedirectToAction("List");

        //get order item identifier
        var orderItemId = 0;
		_logger.LogDebug("ActivateDownloadItem(id={Id},form={Form}): orderItemId → {OrderItemId}", id, form, orderItemId);
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                orderItemId = Convert.ToInt32(formValue["btnPvActivateDownload".Length..]);

        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        orderItem.IsDownloadActivated = !orderItem.IsDownloadActivated;
		_logger.LogDebug("ActivateDownloadItem(id={Id},form={Form}): orderItem.IsDownloadActivated → {Arg2}", id, form, orderItem.IsDownloadActivated);
        await _orderService.UpdateOrderItemAsync(orderItem);

        await LogEditOrderAsync(order.Id);

        //selected card
        SaveSelectedCardName("order-products");

		_logger.LogDebug("<<< Exiting ActivateDownloadItem(id={Id},form={Form})", id, form);
        return RedirectToAction("Edit", new { id = order.Id });
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> UploadLicenseFilePopup(int id, int orderItemId)
    {
		_logger.LogDebug(">>> Entering UploadLicenseFilePopup(id={Id},orderItemId={OrderItemId})", id, orderItemId);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("UploadLicenseFilePopup(id={Id},orderItemId={OrderItemId}): order → {Order}", id, orderItemId, order);
        if (order == null)
            return RedirectToAction("List");

        //try to get an order item with the specified id
        var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        var product = await _productService.GetProductByIdAsync(orderItem.ProductId)
            ?? throw new ArgumentException("No product found with the specified order item id");

        if (!product.IsDownload)
			_logger.LogWarning("Throwing ArgumentException from UploadLicenseFilePopup(id={Id},orderItemId={OrderItemId})", id, orderItemId);
            throw new ArgumentException("Product is not downloadable");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        //prepare model
        var model = await _orderModelFactory.PrepareUploadLicenseModelAsync(new UploadLicenseModel(), order, orderItem);
		_logger.LogDebug("UploadLicenseFilePopup(id={Id},orderItemId={OrderItemId}): model → {Model}", id, orderItemId, model);

		_logger.LogDebug("<<< Exiting UploadLicenseFilePopup(id={Id},orderItemId={OrderItemId})", id, orderItemId);
		_logger.LogDebug("<<< Exiting UploadLicenseFilePopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [FormValueRequired("uploadlicense")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> UploadLicenseFilePopup(UploadLicenseModel model)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(model.OrderId);
        if (order == null)
            return RedirectToAction("List");

        var orderItem = await _orderService.GetOrderItemByIdAsync(model.OrderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        //attach license
        if (model.LicenseDownloadId > 0)
            orderItem.LicenseDownloadId = model.LicenseDownloadId;
        else
            orderItem.LicenseDownloadId = null;

        await _orderService.UpdateOrderItemAsync(orderItem);

        await LogEditOrderAsync(order.Id);

        //success
        ViewBag.RefreshPage = true;

        return View(model);
    }

    [HttpPost, ActionName("UploadLicenseFilePopup")]
    [FormValueRequired("deletelicense")]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteLicenseFilePopup(UploadLicenseModel model)
    {
		_logger.LogDebug(">>> Entering DeleteLicenseFilePopup(model={Model})", model);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(model.OrderId);
		_logger.LogDebug("DeleteLicenseFilePopup(model={Model}): order → {Order}", model, order);
        if (order == null)
            return RedirectToAction("List");

        var orderItem = await _orderService.GetOrderItemByIdAsync(model.OrderItemId)
            ?? throw new ArgumentException("No order item found with the specified id");

        //ensure a vendor has access only to his products 
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
            return RedirectToAction("List");

        //attach license
        orderItem.LicenseDownloadId = null;
		_logger.LogDebug("DeleteLicenseFilePopup(model={Model}): orderItem.LicenseDownloadId → {Arg1}", model, orderItem.LicenseDownloadId);

        await _orderService.UpdateOrderItemAsync(orderItem);

        await LogEditOrderAsync(order.Id);

        //success
        ViewBag.RefreshPage = true;
		_logger.LogDebug("DeleteLicenseFilePopup(model={Model}): ViewBag.RefreshPage → {Arg1}", model, ViewBag.RefreshPage);

		_logger.LogDebug("<<< Exiting DeleteLicenseFilePopup(model={Model})", model);
        return View(model);
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddProductToOrder(int orderId)
    {
		_logger.LogDebug(">>> Entering AddProductToOrder(orderId={OrderId})", orderId);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId);
		_logger.LogDebug("AddProductToOrder(orderId={OrderId}): order → {Order}", orderId, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //prepare model
        var model = await _orderModelFactory.PrepareAddProductToOrderSearchModelAsync(new AddProductToOrderSearchModel(), order);
		_logger.LogDebug("AddProductToOrder(orderId={OrderId}): model → {Model}", orderId, model);

		_logger.LogDebug("<<< Exiting AddProductToOrder(orderId={OrderId})", orderId);
		_logger.LogDebug("<<< Exiting AddProductToOrder(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddProductToOrder(AddProductToOrderSearchModel searchModel)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(searchModel.OrderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareAddProductToOrderListModelAsync(searchModel, order);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddProductToOrderDetails(int orderId, int productId)
    {
		_logger.LogDebug(">>> Entering AddProductToOrderDetails(orderId={OrderId},productId={ProductId})", orderId, productId);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //prepare model
        var model = await _orderModelFactory.PrepareAddProductToOrderModelAsync(new AddProductToOrderModel(), order, product);
		_logger.LogDebug("AddProductToOrderDetails(orderId={OrderId},productId={ProductId}): model → {Model}", orderId, productId, model);

		_logger.LogDebug("<<< Exiting AddProductToOrderDetails(orderId={OrderId},productId={ProductId})", orderId, productId);
		_logger.LogDebug("<<< Exiting AddProductToOrderDetails(orderId={OrderId},productId={ProductId},model={Model},form={Form})", orderId, productId, model, form);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddProductToOrderDetails(int orderId, int productId, AddProductToOrderModel model, IFormCollection form)
    {
        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId)
            ?? throw new ArgumentException("No customer found with the specified id");

        //warnings
        var warnings = new List<string>();

        //attributes
        var attributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, warnings);

        //rental product
        _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);

        //warnings
        warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer, ShoppingCartType.ShoppingCart, product, model.Quantity, attributesXml));
        warnings.AddRange(await _shoppingCartService.GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType.ShoppingCart, product, attributesXml));
        warnings.AddRange(await _shoppingCartService.GetRentalProductWarningsAsync(product, rentalStartDate, rentalEndDate));
        if (!warnings.Any())
        {
            //no errors
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            //attributes
            var attributeDescription = await _productAttributeFormatter.FormatAttributesAsync(product, attributesXml, customer, currentStore);

            //weight
            var itemWeight = await _shippingService.GetShoppingCartItemWeightAsync(product, attributesXml);

            //save item
            var orderItem = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                UnitPriceInclTax = model.UnitPriceInclTax,
                UnitPriceExclTax = model.UnitPriceExclTax,
                PriceInclTax = model.SubTotalInclTax,
                PriceExclTax = model.SubTotalExclTax,
                OriginalProductCost = await _priceCalculationService.GetProductCostAsync(product, attributesXml),
                AttributeDescription = attributeDescription,
                AttributesXml = attributesXml,
                Quantity = model.Quantity,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = itemWeight,
                RentalStartDateUtc = rentalStartDate,
                RentalEndDateUtc = rentalEndDate
            };

            await _orderService.InsertOrderItemAsync(orderItem);

            //adjust inventory
            await _productService.AdjustInventoryAsync(product, -orderItem.Quantity, orderItem.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));

            //update order totals
            var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
            {
                PriceInclTax = model.UnitPriceInclTax,
                PriceExclTax = model.UnitPriceExclTax,
                SubTotalInclTax = model.SubTotalInclTax,
                SubTotalExclTax = model.SubTotalExclTax,
                Quantity = model.Quantity
            };
            await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "A new order item has been added",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            //gift cards
            if (product.IsGiftCard)
            {
                _productAttributeParser.GetGiftCardAttribute(
                    attributesXml, out var recipientName, out var recipientEmail, out var senderName, out var senderEmail, out var giftCardMessage);

                for (var i = 0; i < orderItem.Quantity; i++)
                {
                    var gc = new GiftCard
                    {
                        GiftCardType = product.GiftCardType,
                        PurchasedWithOrderItemId = orderItem.Id,
                        Amount = model.UnitPriceExclTax,
                        IsGiftCardActivated = false,
                        GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                        RecipientName = recipientName,
                        RecipientEmail = recipientEmail,
                        SenderName = senderName,
                        SenderEmail = senderEmail,
                        Message = giftCardMessage,
                        IsRecipientNotified = false,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    await _giftCardService.InsertGiftCardAsync(gc);
                }
            }

            //redirect to order details page
            foreach (var warning in updateOrderParameters.Warnings)
                _notificationService.WarningNotification(warning);

            //selected card
            SaveSelectedCardName("order-products");
            return RedirectToAction("Edit", new { id = order.Id });
        }

        //prepare model
        model.Warnings.AddRange(warnings);

        return View(model);
    }

    #endregion

    #endregion

    #region Addresses

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressEdit(int addressId, int orderId)
    {
		_logger.LogDebug(">>> Entering AddressEdit(addressId={AddressId},orderId={OrderId})", addressId, orderId);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId);
		_logger.LogDebug("AddressEdit(addressId={AddressId},orderId={OrderId}): order → {Order}", addressId, orderId, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //try to get an address with the specified id
        var address = await _addressService.GetAddressByIdAsync(addressId)
            ?? throw new ArgumentException("No address found with the specified id", nameof(addressId));

        //prepare model
        var model = await _orderModelFactory.PrepareOrderAddressModelAsync(new OrderAddressModel(), order, address);
		_logger.LogDebug("AddressEdit(addressId={AddressId},orderId={OrderId}): model → {Model}", addressId, orderId, model);

		_logger.LogDebug("<<< Exiting AddressEdit(addressId={AddressId},orderId={OrderId})", addressId, orderId);
		_logger.LogDebug("<<< Exiting AddressEdit(model={Model},form={Form})", model, form);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressEdit(OrderAddressModel model, IFormCollection form)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(model.OrderId);
        if (order == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", new { id = order.Id });

        //try to get an address with the specified id
        var address = await _addressService.GetAddressByIdAsync(model.Address.Id)
            ?? throw new ArgumentException("No address found with the specified id");

        //custom address attributes
        var customAttributes = await _addressAttributeParser.ParseCustomAttributesAsync(form, NopCommonDefaults.AddressAttributeControlName);
        var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
        foreach (var error in customAttributeWarnings)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        if (ModelState.IsValid)
        {
            address = model.Address.ToEntity(address);
            address.CustomAttributes = customAttributes;
            await _addressService.UpdateAddressAsync(address);

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Address has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Addresses.Updated"));

            return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, orderId = model.OrderId });
        }

        //prepare model
        model = await _orderModelFactory.PrepareOrderAddressModelAsync(model, order, address);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    #endregion

    #region Shipments

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentList()
    {
		_logger.LogDebug(">>> Entering ShipmentList()");
        //prepare model
        var model = await _orderModelFactory.PrepareShipmentSearchModelAsync(new ShipmentSearchModel());
		_logger.LogDebug("ShipmentList(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting ShipmentList()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentListSelect(ShipmentSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ShipmentListSelect(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _orderModelFactory.PrepareShipmentListModelAsync(searchModel);
		_logger.LogDebug("ShipmentListSelect(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ShipmentListSelect(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> ShipmentsByOrder(OrderShipmentSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ShipmentsByOrder(searchModel={SearchModel})", searchModel);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(searchModel.OrderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareOrderShipmentListModelAsync(searchModel, order);
		_logger.LogDebug("ShipmentsByOrder(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ShipmentsByOrder(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentsItemsByShipmentId(ShipmentItemSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ShipmentsItemsByShipmentId(searchModel={SearchModel})", searchModel);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(searchModel.ShipmentId)
            ?? throw new ArgumentException("No shipment found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("ShipmentsItemsByShipmentId(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
        if (currentVendor != null && !await HasAccessToShipmentAsync(shipment))
            return Content(string.Empty);

        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor should have access only to his products
        if (currentVendor != null && !await HasAccessToOrderAsync(order))
            return Content(string.Empty);

        //prepare model
        searchModel.SetGridPageSize();
        var model = await _orderModelFactory.PrepareShipmentItemListModelAsync(searchModel, shipment);
		_logger.LogDebug("ShipmentsItemsByShipmentId(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ShipmentsItemsByShipmentId(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddShipment(int id)
    {
		_logger.LogDebug(">>> Entering AddShipment(id={Id})", id);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(id);
		_logger.LogDebug("AddShipment(id={Id}): order → {Order}", id, order);
        if (order == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
            return RedirectToAction("List");

        //prepare model
        var model = await _orderModelFactory.PrepareShipmentModelAsync(new ShipmentModel(), null, order);
		_logger.LogDebug("AddShipment(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting AddShipment(id={Id})", id);
		_logger.LogDebug("<<< Exiting AddShipment(model={Model},form={Form},continueEditing={ContinueEditing})", model, form, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddShipment(ShipmentModel model, IFormCollection form, bool continueEditing)
    {
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(model.OrderId);
        if (order == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && !await HasAccessToOrderAsync(order))
            return RedirectToAction("List");

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);
        //a vendor should have access only to his products
        if (currentVendor != null)
        {
            orderItems = await orderItems.WhereAwait(HasAccessToProductAsync).ToListAsync();
        }

        var shipment = new Shipment
        {
            OrderId = order.Id,
            TrackingNumber = model.TrackingNumber,
            TotalWeight = null,
            AdminComment = model.AdminComment,
            CreatedOnUtc = DateTime.UtcNow
        };

        var shipmentItems = new List<ShipmentItem>();

        decimal? totalWeight = null;

        foreach (var orderItem in orderItems)
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            //ensure that this product can be shipped (have at least one item to ship)
            var maxQtyToAdd = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
            if (maxQtyToAdd <= 0)
                continue;

            var qtyToAdd = 0; //parse quantity
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"qtyToAdd{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out qtyToAdd);
                    break;
                }

            var warehouseId = 0;
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.UseMultipleWarehouses)
            {
                //multiple warehouses supported
                //warehouse is chosen by a store owner
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"warehouse_{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(form[formKey], out warehouseId);
                        break;
                    }
            }
            else
            {
                //multiple warehouses are not supported
                warehouseId = product.WarehouseId;
            }

            //validate quantity
            if (qtyToAdd <= 0)
                continue;
            if (qtyToAdd > maxQtyToAdd)
                qtyToAdd = maxQtyToAdd;

            //ok. we have at least one item. let's create a shipment (if it does not exist)

            var orderItemTotalWeight = orderItem.ItemWeight * qtyToAdd;
            if (orderItemTotalWeight.HasValue)
            {
                if (!totalWeight.HasValue)
                    totalWeight = 0;
                totalWeight += orderItemTotalWeight.Value;
            }

            //create a shipment item
            shipmentItems.Add(new ShipmentItem
            {
                OrderItemId = orderItem.Id,
                Quantity = qtyToAdd,
                WarehouseId = warehouseId
            });
        }

        //if we have at least one item in the shipment, then save it
        if (shipmentItems.Any())
        {
            shipment.TotalWeight = totalWeight;
            await _shipmentService.InsertShipmentAsync(shipment);

            foreach (var shipmentItem in shipmentItems)
            {
                shipmentItem.ShipmentId = shipment.Id;
                await _shipmentService.InsertShipmentItemAsync(shipmentItem);
            }

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "A shipment has been added",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await _eventPublisher.PublishAsync(new ShipmentCreatedEvent(shipment));

            if (!string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                await _eventPublisher.PublishAsync(new ShipmentTrackingNumberSetEvent(shipment));

            var canShip = !order.PickupInStore && model.CanShip;
            if (canShip)
                await _orderProcessingService.ShipAsync(shipment, true);

            var canMarkAsReadyForPickup = order.PickupInStore && model.CanMarkAsReadyForPickup;
            if (canMarkAsReadyForPickup)
                await _orderProcessingService.ReadyForPickupAsync(shipment, true);

            if ((canShip || canMarkAsReadyForPickup) && model.CanDeliver)
                await _orderProcessingService.DeliverAsync(shipment, true);

            await LogEditOrderAsync(order.Id);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.Added"));
            return continueEditing
                ? RedirectToAction("ShipmentDetails", new { id = shipment.Id })
                : RedirectToAction("Edit", new { id = model.OrderId });
        }

        _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.NoProductsSelected"));

        return RedirectToAction("AddShipment", model);
    }

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> ShipmentDetails(int id)
    {
		_logger.LogDebug(">>> Entering ShipmentDetails(id={Id})", id);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
		_logger.LogDebug("ShipmentDetails(id={Id}): shipment → {Shipment}", id, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        //prepare model
        var model = await _orderModelFactory.PrepareShipmentModelAsync(null, shipment, null);
		_logger.LogDebug("ShipmentDetails(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting ShipmentDetails(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteShipment(int id)
    {
		_logger.LogDebug(">>> Entering DeleteShipment(id={Id})", id);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
		_logger.LogDebug("DeleteShipment(id={Id}): shipment → {Shipment}", id, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        foreach (var shipmentItem in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
        {
            var orderItem = await _orderService.GetOrderItemByIdAsync(shipmentItem.OrderItemId);
			_logger.LogDebug("DeleteShipment(id={Id}): orderItem → {OrderItem}", id, orderItem);
            if (orderItem == null)
                continue;

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
			_logger.LogDebug("DeleteShipment(id={Id}): product → {Product}", id, product);

            await _productService.ReverseBookedInventoryAsync(product, shipmentItem,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteShipment"), shipment.OrderId));
        }

        var orderId = shipment.OrderId;
        await _shipmentService.DeleteShipmentAsync(shipment);

        var order = await _orderService.GetOrderByIdAsync(orderId);
		_logger.LogDebug("DeleteShipment(id={Id}): order → {Order}", id, order);
        //add a note
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "A shipment has been deleted",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        await LogEditOrderAsync(order.Id);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.Deleted"));
		_logger.LogDebug("<<< Exiting DeleteShipment(id={Id})", id);
        return RedirectToAction("Edit", new { id = orderId });
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("settrackingnumber")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetTrackingNumber(ShipmentModel model)
    {
		_logger.LogDebug(">>> Entering SetTrackingNumber(model={Model})", model);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
		_logger.LogDebug("SetTrackingNumber(model={Model}): shipment → {Shipment}", model, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        if (shipment.TrackingNumber == model.TrackingNumber)
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });

        shipment.TrackingNumber = model.TrackingNumber;
		_logger.LogDebug("SetTrackingNumber(model={Model}): shipment.TrackingNumber → {Arg1}", model, shipment.TrackingNumber);
        await _shipmentService.UpdateShipmentAsync(shipment);

        await _eventPublisher.PublishAsync(new ShipmentTrackingNumberSetEvent(shipment));

		_logger.LogDebug("<<< Exiting SetTrackingNumber(model={Model})", model);
        return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("setadmincomment")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetShipmentAdminComment(ShipmentModel model)
    {
		_logger.LogDebug(">>> Entering SetShipmentAdminComment(model={Model})", model);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
		_logger.LogDebug("SetShipmentAdminComment(model={Model}): shipment → {Shipment}", model, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        shipment.AdminComment = model.AdminComment;
		_logger.LogDebug("SetShipmentAdminComment(model={Model}): shipment.AdminComment → {Arg1}", model, shipment.AdminComment);
        await _shipmentService.UpdateShipmentAsync(shipment);

		_logger.LogDebug("<<< Exiting SetShipmentAdminComment(model={Model})", model);
        return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("setasshipped")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsShipped(int id)
    {
		_logger.LogDebug(">>> Entering SetAsShipped(id={Id})", id);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
		_logger.LogDebug("SetAsShipped(id={Id}): shipment → {Shipment}", id, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            await _orderProcessingService.ShipAsync(shipment, true);
            await LogEditOrderAsync(shipment.OrderId);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in SetAsShipped(id={Id}): {Arg1}", id, exc.Message);
            //error
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting SetAsShipped(id={Id})", id);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("saveshippeddate")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditShippedDate(ShipmentModel model)
    {
		_logger.LogDebug(">>> Entering EditShippedDate(model={Model})", model);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
		_logger.LogDebug("EditShippedDate(model={Model}): shipment → {Shipment}", model, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            if (!model.ShippedDateUtc.HasValue)
            {
				_logger.LogWarning("Throwing Exception from EditShippedDate(model={Model})", model);
                throw new Exception("Enter shipped date");
            }

            shipment.ShippedDateUtc = model.ShippedDateUtc;
			_logger.LogDebug("EditShippedDate(model={Model}): shipment.ShippedDateUtc → {Arg1}", model, shipment.ShippedDateUtc);
            await _shipmentService.UpdateShipmentAsync(shipment);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in EditShippedDate(model={Model}): {Arg1}", model, exc.Message);
            //error
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting EditShippedDate(model={Model})", model);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("setasreadyforpickup")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsReadyForPickup(int id)
    {
		_logger.LogDebug(">>> Entering SetAsReadyForPickup(id={Id})", id);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
		_logger.LogDebug("SetAsReadyForPickup(id={Id}): shipment → {Shipment}", id, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            await _orderProcessingService.ReadyForPickupAsync(shipment, true);
            await LogEditOrderAsync(shipment.OrderId);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in SetAsReadyForPickup(id={Id}): {Arg1}", id, exc.Message);
            //error
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting SetAsReadyForPickup(id={Id})", id);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("savereadyforpickupdate")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditReadyForPickupDate(ShipmentModel model)
    {
		_logger.LogDebug(">>> Entering EditReadyForPickupDate(model={Model})", model);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
		_logger.LogDebug("EditReadyForPickupDate(model={Model}): shipment → {Shipment}", model, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            if (!model.ReadyForPickupDateUtc.HasValue)
				_logger.LogWarning("Throwing Exception from EditReadyForPickupDate(model={Model})", model);
                throw new Exception("Enter ready for pickup date");

            shipment.ReadyForPickupDateUtc = model.ReadyForPickupDateUtc;
			_logger.LogDebug("EditReadyForPickupDate(model={Model}): shipment.ReadyForPickupDateUtc → {Arg1}", model, shipment.ReadyForPickupDateUtc);
            await _shipmentService.UpdateShipmentAsync(shipment);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in EditReadyForPickupDate(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting EditReadyForPickupDate(model={Model})", model);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("setasdelivered")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsDelivered(int id)
    {
		_logger.LogDebug(">>> Entering SetAsDelivered(id={Id})", id);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(id);
		_logger.LogDebug("SetAsDelivered(id={Id}): shipment → {Shipment}", id, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            await _orderProcessingService.DeliverAsync(shipment, true);
            await LogEditOrderAsync(shipment.OrderId);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in SetAsDelivered(id={Id}): {Arg1}", id, exc.Message);
            //error
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting SetAsDelivered(id={Id})", id);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [HttpPost, ActionName("ShipmentDetails")]
    [FormValueRequired("savedeliverydate")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditDeliveryDate(ShipmentModel model)
    {
		_logger.LogDebug(">>> Entering EditDeliveryDate(model={Model})", model);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
		_logger.LogDebug("EditDeliveryDate(model={Model}): shipment → {Shipment}", model, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        try
        {
            if (!model.DeliveryDateUtc.HasValue)
            {
				_logger.LogWarning("Throwing Exception from EditDeliveryDate(model={Model})", model);
                throw new Exception("Enter delivery date");
            }

            shipment.DeliveryDateUtc = model.DeliveryDateUtc;
			_logger.LogDebug("EditDeliveryDate(model={Model}): shipment.DeliveryDateUtc → {Arg1}", model, shipment.DeliveryDateUtc);
            await _shipmentService.UpdateShipmentAsync(shipment);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in EditDeliveryDate(model={Model}): {Arg1}", model, exc.Message);
            //error
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting EditDeliveryDate(model={Model})", model);
            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }
    }

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> PdfPackagingSlip(int shipmentId)
    {
		_logger.LogDebug(">>> Entering PdfPackagingSlip(shipmentId={ShipmentId})", shipmentId);
        //try to get a shipment with the specified id
        var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
		_logger.LogDebug("PdfPackagingSlip(shipmentId={ShipmentId}): shipment → {Shipment}", shipmentId, shipment);
        if (shipment == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
            return RedirectToAction("List");

        byte[] bytes;
        await using (var stream = new MemoryStream())
        {
            await _pdfService.PrintPackagingSlipToPdfAsync(stream, shipment, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync());
            bytes = stream.ToArray();
        }

		_logger.LogDebug("<<< Exiting PdfPackagingSlip(shipmentId={ShipmentId})", shipmentId);
        return File(bytes, MimeTypes.ApplicationPdf, $"packagingslip_{shipment.Id}.pdf");
    }

    [HttpPost, ActionName("ShipmentList")]
    [FormValueRequired("exportpackagingslips-all")]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> PdfPackagingSlipAll(ShipmentSearchModel model)
    {
		_logger.LogDebug(">>> Entering PdfPackagingSlipAll(model={Model})", model);
        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PdfPackagingSlipAll(model={Model}): currentVendor → {CurrentVendor}", model, currentVendor);
        var vendorId = 0;
		_logger.LogDebug("PdfPackagingSlipAll(model={Model}): vendorId → {VendorId}", model, vendorId);
        if (currentVendor != null)
            vendorId = currentVendor.Id;

        //load shipments
        var shipments = await _shipmentService.GetAllShipmentsAsync(vendorId: vendorId,
            warehouseId: model.WarehouseId,
            shippingCountryId: model.CountryId,
            shippingStateId: model.StateProvinceId,
            shippingCounty: model.County,
            shippingCity: model.City,
            trackingNumber: model.TrackingNumber,
            loadNotShipped: model.LoadNotShipped,
            loadNotReadyForPickup: model.LoadNotReadyForPickup,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue);

        //ensure that we at least one shipment selected
        if (!shipments.Any())
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.NoShipmentsSelected"));
            return RedirectToAction("ShipmentList");
        }

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintPackagingSlipsToPdfAsync(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync());
                bytes = stream.ToArray();
            }

            return File(bytes, "application/zip", "packagingslips.zip");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in PdfPackagingSlipAll(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting PdfPackagingSlipAll(model={Model})", model);
            return RedirectToAction("ShipmentList");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public virtual async Task<IActionResult> PdfPackagingSlipSelected(string selectedIds)
    {
		_logger.LogDebug(">>> Entering PdfPackagingSlipSelected(selectedIds={SelectedIds})", selectedIds);
        var shipments = new List<Shipment>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            shipments.AddRange(await _shipmentService.GetShipmentsByIdsAsync(ids));
        }
        //a vendor should have access only to his products
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (await _workContext.GetCurrentVendorAsync() != null)
        {
            shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
        }
		_logger.LogInformation("PdfPackagingSlipSelected(selectedIds)={SelectedIds}: database query executed in {Arg1} ms", selectedIds, __logStart.ElapsedMilliseconds);

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintPackagingSlipsToPdfAsync(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync());
                bytes = stream.ToArray();
            }

            return File(bytes, "application/zip", "packagingslips.zip");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in PdfPackagingSlipSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting PdfPackagingSlipSelected(selectedIds={SelectedIds})", selectedIds);
            return RedirectToAction("ShipmentList");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsShippedSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering SetAsShippedSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var shipments = await _shipmentService.GetShipmentsByIdsAsync(selectedIds.ToArray());
		_logger.LogDebug("SetAsShippedSelected(selectedIds={SelectedIds}): shipments → {Shipments}", selectedIds, shipments);

        //a vendor should have access only to his products
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (await _workContext.GetCurrentVendorAsync() != null)
        {
            shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
        }
		_logger.LogInformation("SetAsShippedSelected(selectedIds)={SelectedIds}: database query executed in {Arg1} ms", selectedIds, __logStart.ElapsedMilliseconds);

        foreach (var shipment in shipments)
        {
            try
            {
                await _orderProcessingService.ShipAsync(shipment, true);
            }
            catch
            {
				_logger.LogError("Exception in SetAsShippedSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, exc.Message);
                //ignore any exception
            }
        }

		_logger.LogDebug("<<< Exiting SetAsShippedSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsReadyForPickupSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering SetAsReadyForPickupSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var shipments = await _shipmentService.GetShipmentsByIdsAsync(selectedIds.ToArray());
		_logger.LogDebug("SetAsReadyForPickupSelected(selectedIds={SelectedIds}): shipments → {Shipments}", selectedIds, shipments);

        //a vendor should have access only to his products
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (await _workContext.GetCurrentVendorAsync() != null)
        {
            shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
        }
		_logger.LogInformation("SetAsReadyForPickupSelected(selectedIds)={SelectedIds}: database query executed in {Arg1} ms", selectedIds, __logStart.ElapsedMilliseconds);

        foreach (var shipment in shipments)
        {
            try
            {
                await _orderProcessingService.ReadyForPickupAsync(shipment, true);
            }
            catch
            {
				_logger.LogError("Exception in SetAsReadyForPickupSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, exc.Message);
                //ignore any exception
            }
        }

		_logger.LogDebug("<<< Exiting SetAsReadyForPickupSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SetAsDeliveredSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering SetAsDeliveredSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var shipments = await _shipmentService.GetShipmentsByIdsAsync(selectedIds.ToArray());
		_logger.LogDebug("SetAsDeliveredSelected(selectedIds={SelectedIds}): shipments → {Shipments}", selectedIds, shipments);

        //a vendor should have access only to his products
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (await _workContext.GetCurrentVendorAsync() != null)
        {
            shipments = await shipments.WhereAwait(HasAccessToShipmentAsync).ToListAsync();
        }
		_logger.LogInformation("SetAsDeliveredSelected(selectedIds)={SelectedIds}: database query executed in {Arg1} ms", selectedIds, __logStart.ElapsedMilliseconds);

        foreach (var shipment in shipments)
        {
            try
            {
                await _orderProcessingService.DeliverAsync(shipment, true);
            }
            catch
            {
				_logger.LogError("Exception in SetAsDeliveredSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, exc.Message);
                //ignore any exception
            }
        }

		_logger.LogDebug("<<< Exiting SetAsDeliveredSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Order notes

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> OrderNotesSelect(OrderNoteSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering OrderNotesSelect(searchModel={SearchModel})", searchModel);
        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(searchModel.OrderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareOrderNoteListModelAsync(searchModel, order);
		_logger.LogDebug("OrderNotesSelect(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting OrderNotesSelect(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OrderNoteAdd(int orderId, int downloadId, bool displayToCustomer, string message)
    {
		_logger.LogDebug(">>> Entering OrderNoteAdd(orderId={OrderId},downloadId={DownloadId},displayToCustomer={DisplayToCustomer},message={Message})", orderId, downloadId, displayToCustomer, message);
        if (string.IsNullOrEmpty(message))
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.Orders.OrderNotes.Fields.Note.Validation"));

        //try to get an order with the specified id
        var order = await _orderService.GetOrderByIdAsync(orderId);
		_logger.LogDebug("OrderNoteAdd(orderId={OrderId},downloadId={DownloadId},displayToCustomer={DisplayToCustomer},message={Message}): order → {Order}", orderId, downloadId, displayToCustomer, message, order);
        if (order == null)
            return ErrorJson("Order cannot be loaded");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return ErrorJson("No access for vendors");

        var orderNote = new OrderNote
        {
            OrderId = order.Id,
            DisplayToCustomer = displayToCustomer,
            Note = message,
            DownloadId = downloadId,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _orderService.InsertOrderNoteAsync(orderNote);

        //new order notification
        if (displayToCustomer)
        {
            //email
            await _workflowMessageService.SendNewOrderNoteAddedCustomerNotificationAsync(orderNote, (await _workContext.GetWorkingLanguageAsync()).Id);
        }

		_logger.LogDebug("<<< Exiting OrderNoteAdd(orderId={OrderId},downloadId={DownloadId},displayToCustomer={DisplayToCustomer},message={Message})", orderId, downloadId, displayToCustomer, message);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OrderNoteDelete(int id, int orderId)
    {
		_logger.LogDebug(">>> Entering OrderNoteDelete(id={Id},orderId={OrderId})", id, orderId);
        //try to get an order with the specified id
        _ = await _orderService.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("No order found with the specified id");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("Edit", "Order", new { id = orderId });

        //try to get an order note with the specified id
        var orderNote = await _orderService.GetOrderNoteByIdAsync(id)
            ?? throw new ArgumentException("No order note found with the specified id");

        await _orderService.DeleteOrderNoteAsync(orderNote);

		_logger.LogDebug("<<< Exiting OrderNoteDelete(id={Id},orderId={OrderId})", id, orderId);
        return new NullJsonResult();
    }

    #endregion

    #region Reports

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> BestsellersBriefReportByQuantityList(BestsellerBriefSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering BestsellersBriefReportByQuantityList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _orderModelFactory.PrepareBestsellerBriefListModelAsync(searchModel);
		_logger.LogDebug("BestsellersBriefReportByQuantityList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting BestsellersBriefReportByQuantityList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> BestsellersBriefReportByAmountList(BestsellerBriefSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering BestsellersBriefReportByAmountList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _orderModelFactory.PrepareBestsellerBriefListModelAsync(searchModel);
		_logger.LogDebug("BestsellersBriefReportByAmountList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting BestsellersBriefReportByAmountList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> OrderAverageReportList(OrderAverageReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering OrderAverageReportList(searchModel={SearchModel})", searchModel);
        //a vendor doesn't have access to this report
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareOrderAverageReportListModelAsync(searchModel);
		_logger.LogDebug("OrderAverageReportList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting OrderAverageReportList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> OrderIncompleteReportList(OrderIncompleteReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering OrderIncompleteReportList(searchModel={SearchModel})", searchModel);
        //a vendor doesn't have access to this report
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        //prepare model
        var model = await _orderModelFactory.PrepareOrderIncompleteReportListModelAsync(searchModel);
		_logger.LogDebug("OrderIncompleteReportList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting OrderIncompleteReportList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    public virtual async Task<IActionResult> LoadOrderStatistics(string period)
    {
		_logger.LogDebug(">>> Entering LoadOrderStatistics(period={Period})", period);
        //a vendor doesn't have access to this report
        if (await _workContext.GetCurrentVendorAsync() != null)
            return Content(string.Empty);

        var result = new List<object>();
		_logger.LogDebug("LoadOrderStatistics(period={Period}): result → {Result}", period, result);

        var nowDt = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
		_logger.LogDebug("LoadOrderStatistics(period={Period}): nowDt → {NowDt}", period, nowDt);
        var timeZone = await _dateTimeHelper.GetCurrentTimeZoneAsync();
		_logger.LogDebug("LoadOrderStatistics(period={Period}): timeZone → {TimeZone}", period, timeZone);

        var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);
		_logger.LogDebug("LoadOrderStatistics(period={Period}): culture → {Culture}", period, culture);

        switch (period)
        {
            case "year":
                //year statistics
                var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
				_logger.LogDebug("LoadOrderStatistics(period={Period}): yearAgoDt → {YearAgoDt}", period, yearAgoDt);
                var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
				_logger.LogDebug("LoadOrderStatistics(period={Period}): searchYearDateUser → {SearchYearDateUser}", period, searchYearDateUser);
                for (var i = 0; i <= 12; i++)
                {
                    result.Add(new
                    {
                        date = searchYearDateUser.Date.ToString("Y", culture),
                        value = (await _orderService.SearchOrdersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });
							_logger.LogDebug("LoadOrderStatistics(period={Period}): date → {Date}", period, date);
							_logger.LogDebug("LoadOrderStatistics(period={Period}): value → {Value}", period, value);

                    searchYearDateUser = searchYearDateUser.AddMonths(1);
                }

                break;
            case "month":
                //month statistics
                var monthAgoDt = nowDt.AddDays(-30);
				_logger.LogDebug("LoadOrderStatistics(period={Period}): monthAgoDt → {MonthAgoDt}", period, monthAgoDt);
                var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
				_logger.LogDebug("LoadOrderStatistics(period={Period}): searchMonthDateUser → {SearchMonthDateUser}", period, searchMonthDateUser);
                for (var i = 0; i <= 30; i++)
                {
                    result.Add(new
                    {
                        date = searchMonthDateUser.Date.ToString("M", culture),
                        value = (await _orderService.SearchOrdersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });

                    searchMonthDateUser = searchMonthDateUser.AddDays(1);
                }

                break;
            case "week":
            default:
                //week statistics
                var weekAgoDt = nowDt.AddDays(-7);
				_logger.LogDebug("LoadOrderStatistics(period={Period}): weekAgoDt → {WeekAgoDt}", period, weekAgoDt);
                var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
				_logger.LogDebug("LoadOrderStatistics(period={Period}): searchWeekDateUser → {SearchWeekDateUser}", period, searchWeekDateUser);
                for (var i = 0; i <= 7; i++)
                {
                    result.Add(new
                    {
                        date = searchWeekDateUser.Date.ToString("d dddd", culture),
                        value = (await _orderService.SearchOrdersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });

                    searchWeekDateUser = searchWeekDateUser.AddDays(1);
                }

                break;
        }

		_logger.LogDebug("<<< Exiting LoadOrderStatistics(period={Period})", period);
        return Json(result);
    }

    #endregion
}
