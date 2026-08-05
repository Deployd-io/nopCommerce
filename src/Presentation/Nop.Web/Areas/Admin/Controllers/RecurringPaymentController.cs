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
        _localizationService = localizationService;
        _notificationService = notificationService;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _permissionService = permissionService;
        _recurringPaymentModelFactory = recurringPaymentModelFactory;
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentSearchModelAsync(new RecurringPaymentSearchModel());

		_logger.LogDebug("List(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_VIEW)]
    public virtual async Task<IActionResult> List(RecurringPaymentSearchModel searchModel)
    {
        //prepare model
        var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
        if (payment == null || payment.Deleted)
			_logger.LogWarning("Edit(id={Id}): payment → {Payment}", id, payment);
            return RedirectToAction("List");

        //prepare model
        var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);
		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
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
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
        if (payment == null)
			_logger.LogWarning("Delete(id={Id}): payment → {Payment}", id, payment);
            return RedirectToAction("List");

        await _orderService.DeleteRecurringPaymentAsync(payment);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_VIEW)]
    public virtual async Task<IActionResult> HistoryList(RecurringPaymentHistorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering HistoryList(searchModel={SearchModel})", searchModel);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(searchModel.RecurringPaymentId)
                      ?? throw new ArgumentException("No recurring payment found with the specified id");

        //prepare model
        var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentHistoryListModelAsync(searchModel, payment);

		_logger.LogDebug("HistoryList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting HistoryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("processnextpayment")]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProcessNextPayment(int id)
    {
		_logger.LogDebug(">>> Entering ProcessNextPayment(id={Id})", id);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
        if (payment == null)
			_logger.LogWarning("ProcessNextPayment(id={Id}): payment → {Payment}", id, payment);
            return RedirectToAction("List");

        try
        {
            var errors = (await _orderProcessingService.ProcessNextRecurringPaymentAsync(payment)).ToList();
            if (errors.Any())
			_logger.LogDebug("ProcessNextPayment(id={Id}): errors → {Errors}", id, errors);
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

			_logger.LogDebug("ProcessNextPayment(id={Id}): model → {Model}", id, model);
            //selected card
            SaveSelectedCardName("recurringpayment-history");

            return RedirectToAction("Edit", new { id = payment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ProcessNextPayment(id={Id}): {Arg1}", id, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

            //selected card
            SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

			_logger.LogDebug("<<< Exiting ProcessNextPayment(id={Id})", id);
            return View(model);
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("cancelpayment")]
    [CheckPermission(StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CancelRecurringPayment(int id)
    {
		_logger.LogDebug(">>> Entering CancelRecurringPayment(id={Id})", id);
        //try to get a recurring payment with the specified id
        var payment = await _orderService.GetRecurringPaymentByIdAsync(id);
        if (payment == null)
			_logger.LogWarning("CancelRecurringPayment(id={Id}): payment → {Payment}", id, payment);
            return RedirectToAction("List");

        try
        {
            var errors = await _orderProcessingService.CancelRecurringPaymentAsync(payment);
            if (errors.Any())
			_logger.LogDebug("CancelRecurringPayment(id={Id}): errors → {Errors}", id, errors);
            {
                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);
            }
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.RecurringPayments.Cancelled"));

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

			_logger.LogDebug("CancelRecurringPayment(id={Id}): model → {Model}", id, model);
            //selected card
            SaveSelectedCardName("recurringpayment-history");

            return RedirectToAction("Edit", new { id = payment.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in CancelRecurringPayment(id={Id}): {Arg1}", id, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);

            //prepare model
            var model = await _recurringPaymentModelFactory.PrepareRecurringPaymentModelAsync(null, payment);

            //selected card
            SaveSelectedCardName("recurringpayment-history", persistForTheNextRequest: false);

			_logger.LogDebug("<<< Exiting CancelRecurringPayment(id={Id})", id);
            return View(model);
        }
    }

    #endregion
}