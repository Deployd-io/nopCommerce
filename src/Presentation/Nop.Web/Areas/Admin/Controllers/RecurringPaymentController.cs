using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class RecurringPaymentController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IRecurringPaymentModelFactory _recurringPaymentModelFactory;

    #endregion Fields

    #region Ctor

    public RecurringPaymentController(ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPermissionService permissionService,
        IRecurringPaymentModelFactory recurringPaymentModelFactory)
    {
		_logger.LogError(">>> Entering RecurringPaymentController(localizationService={LocalizationService},notificationService={NotificationService},orderProcessingService={OrderProcessingService},orderService={OrderService},permissionService={PermissionService},recurringPaymentModelFactory={RecurringPaymentModelFactory})", localizationService, notificationService, orderProcessingService, orderService, permissionService, recurringPaymentModelFactory);
        _localizationService = localizationService;
        _notificationService = notificationService;
		_logger.LogDebug("RecurringPaymentController(localizationService={LocalizationService},notificationService={NotificationService},orderProcessingService={OrderProcessingService},orderService={OrderService},permissionService={PermissionService},recurringPaymentModelFactory={RecurringPaymentModelFactory}): _notificationService → {_notificationService}", localizationService, notificationService, orderProcessingService, orderService, permissionService, recurringPaymentModelFactory, _notificationService);
		_logger.LogDebug("RecurringPaymentController(localizationService={LocalizationService},notificationService={NotificationService},orderProcessingService={OrderProcessingService},orderService={OrderService},permissionService={PermissionService},recurringPaymentModelFactory={RecurringPaymentModelFactory}): _localizationService → {_localizationService}", localizationService, notificationService, orderProcessingService, orderService, permissionService, recurringPaymentModelFactory, _localizationService);
    _logger.LogWarning("<<< Exiting RecurringPaymentController(localizationService={LocalizationService},notificationService={NotificationService},orderProcessingService={OrderProcessingService},orderService={OrderService},permissionService={PermissionService},recurringPaymentModelFactory={RecurringPaymentModelFactory})", localizationService, notificationService, orderProcessingService, orderService, permissionService, recurringPaymentModelFactory);
        _orderProcessingService = orderProcessingService;
		_logger.LogDebug("RecurringPaymentController(localizationService={LocalizationService},notificationService={NotificationService},orderProcessingService={OrderProcessingService},orderService={OrderService},permissionService={PermissionService},recurringPaymentModelFactory={RecurringPaymentModelFactory}): _orderProcessingService → {_orderProcessingService}", localizationService, notificationService, orderProcessingService, orderService, permissionService, recurringPaymentModelFactory, _orderProcessingService);
        _orderService = orderService;
		_logger.LogDebug("RecurringPaymentController(localizationService={LocalizationService},notificationService={NotificationService},orderProcessingService={OrderProcessingService},orderService={OrderService},permissionService={PermissionService},recurringPaymentModelFactory={RecurringPaymentModelFactory}): _orderService → {_orderService}", localizationService, notificationService, orderProcessingService, orderService, permissionService, recurringPaymentModelFactory, _orderService);
        _permissionService = permissionService;
		_logger.LogDebug("RecurringPaymentController(localizationService={LocalizationService},notificationService={NotificationService},orderProcessingService={OrderProcessingService},orderService={OrderService},permissionService={PermissionService},recurringPaymentModelFactory={RecurringPaymentModelFactory}): _permissionService → {_permissionService}", localizationService, notificationService, orderProcessingService, orderService, permissionService, recurringPaymentModelFactory, _permissionService);
        _recurringPaymentModelFactory = recurringPaymentModelFactory;
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentSearchModelAsync(new RecurringPaymentSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_VIEW)]
    public virtual async Task<IActionResult> List(RecurringPaymentSearchModel searchModel)
    {
        //prepare model
        var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
		_logger.LogError("Edit(id={Id}): payment → {Payment}", id, payment);
        if (payment == null || payment.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(RecurringPaymentModel model, bool continueEditing)
    {
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(model.Id);
        if (payment == null || payment.Deleted)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            payment = model.ToEntity(payment);
            await _orderService.UpdateRecurringPaymentAsync(payment);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = payment.Id });
        }

        //prepare model
        model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(model, payment, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
		_logger.LogError("Delete(id={Id}): payment → {Payment}", id, payment);
        if (payment == null)
            return RedirectToAction("List");

        await _orderService.DeleteRecurringPaymentAsync(payment);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.Deleted"));

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_VIEW)]
    public virtual async Task<IActionResult> HistoryList(RecurringPaymentHistorySearchModel searchModel)
    {
		_logger.LogError(">>> Entering HistoryList(searchModel={SearchModel})", searchModel);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(searchModel.RecurringPaymentId)
                      ?? throw new ArgumentException("No recurring payment found with the specified id");

        //prepare model
        var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentHistoryListModelAsync(searchModel, payment);
		_logger.LogError("HistoryList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting HistoryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("processnextpayment")]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProcessNextPayment(int id)
    {
		_logger.LogError(">>> Entering ProcessNextPayment(id={Id})", id);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
		_logger.LogError("ProcessNextPayment(id={Id}): payment → {Payment}", id, payment);
        if (payment == null)
            return RedirectToAction("List");

        try
        {
            var errors = (await _orderProcessingService.ProcessNextRecurringPaymentAsync(payment)).ToList();
			_logger.LogError("ProcessNextPayment(id={Id}): errors → {Errors}", id, errors);
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    _notificationService.ErrorNotification(error);
                }
            }
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.NextPaymentProcessed"));

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);
			_logger.LogError("ProcessNextPayment(id={Id}): model → {Model}", id, model);

            //selected card
            SaveSelectedCardName("recurringpayment-history");

            return RedirectToAction("Edit", new { id = payment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ProcessNextPayment(id={Id}): {Arg1}", id, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

            //selected card
            SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

			_logger.LogWarning("<<< Exiting ProcessNextPayment(id={Id})", id);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("cancelpayment")]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CancelRecurringPayment(int id)
    {
		_logger.LogError(">>> Entering CancelRecurringPayment(id={Id})", id);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
		_logger.LogError("CancelRecurringPayment(id={Id}): payment → {Payment}", id, payment);
        if (payment == null)
            return RedirectToAction("List");

        try
        {
            var errors = await _orderProcessingService.CancelRecurringPaymentAsync(payment);
			_logger.LogError("CancelRecurringPayment(id={Id}): errors → {Errors}", id, errors);
            if (errors.Any())
            {
                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);
            }
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.Cancelled"));

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);
			_logger.LogError("CancelRecurringPayment(id={Id}): model → {Model}", id, model);

            //selected card
            SaveSelectedCardName("recurringpayment-history");

            return RedirectToAction("Edit", new { id = payment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in CancelRecurringPayment(id={Id}): {Arg1}", id, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

            //selected card
            SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

			_logger.LogWarning("<<< Exiting CancelRecurringPayment(id={Id})", id);
            return View(model);
        }
    }

    #endregion
}