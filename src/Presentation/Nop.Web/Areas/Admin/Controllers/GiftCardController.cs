using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
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

public partial class GiftCardController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGiftCardModelFactory _giftCardModelFactory;
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public GiftCardController(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        IDateTimeHelper dateTimeHelper,
        IGiftCardModelFactory giftCardModelFactory,
        IGiftCardService giftCardService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderService orderService,
        IPermissionService permissionService,
        IPriceFormatter priceFormatter,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings)
    {
		_logger.LogError(">>> Entering GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings})", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings);
        _currencySettings = currencySettings;
        _currencyService = currencyService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _currencyService → {_currencyService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _currencyService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _customerActivityService → {_customerActivityService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _customerActivityService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _dateTimeHelper → {_dateTimeHelper}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _dateTimeHelper);
        _giftCardModelFactory = giftCardModelFactory;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _giftCardModelFactory → {_giftCardModelFactory}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _giftCardModelFactory);
        _giftCardService = giftCardService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _giftCardService → {_giftCardService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _giftCardService);
        _languageService = languageService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _languageService → {_languageService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _localizationService → {_localizationService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _notificationService → {_notificationService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _notificationService);
        _orderService = orderService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _orderService → {_orderService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _orderService);
        _permissionService = permissionService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _permissionService → {_permissionService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _permissionService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _priceFormatter → {_priceFormatter}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _priceFormatter);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _workflowMessageService → {_workflowMessageService}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _workflowMessageService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("GiftCardController(currencySettings={CurrencySettings},currencyService={CurrencyService},customerActivityService={CustomerActivityService},dateTimeHelper={DateTimeHelper},giftCardModelFactory={GiftCardModelFactory},giftCardService={GiftCardService},languageService={LanguageService},localizationService={LocalizationService},notificationService={NotificationService},orderService={OrderService},permissionService={PermissionService},priceFormatter={PriceFormatter},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _localizationSettings → {_localizationSettings}", currencySettings, currencyService, customerActivityService, dateTimeHelper, giftCardModelFactory, giftCardService, languageService, localizationService, notificationService, orderService, permissionService, priceFormatter, workflowMessageService, localizationSettings, _localizationSettings);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        //prepare model
        var model = await _giftCardModelFactory.PrepareGiftCardSearchModelAsync(new GiftCardSearchModel());
		_logger.LogError("List(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_VIEW)]
    public virtual async Task<IActionResult> GiftCardList(GiftCardSearchModel searchModel)
    {
		_logger.LogError(">>> Entering GiftCardList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _giftCardModelFactory.PrepareGiftCardListModelAsync(searchModel);
		_logger.LogError("GiftCardList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting GiftCardList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = await _giftCardModelFactory.PrepareGiftCardModelAsync(new GiftCardModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(GiftCardModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var giftCard = model.ToEntity<GiftCard>();
            giftCard.CreatedOnUtc = DateTime.UtcNow;
            await _giftCardService.InsertGiftCardAsync(giftCard);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewGiftCard",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewGiftCard"), giftCard.GiftCardCouponCode), giftCard);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = giftCard.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a gift card with the specified id
        var giftCard = await _giftCardService.GetGiftCardByIdAsync(id);
		_logger.LogError("Edit(id={Id}): giftCard → {GiftCard}", id, giftCard);
        if (giftCard == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _giftCardModelFactory.PrepareGiftCardModelAsync(null, giftCard);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(GiftCardModel model, bool continueEditing)
    {
        //try to get a gift card with the specified id
        var giftCard = await _giftCardService.GetGiftCardByIdAsync(model.Id);
        if (giftCard == null)
            return RedirectToAction("List");

        var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);

        model.PurchasedWithOrderId = order?.Id;
        model.RemainingAmountStr = await _priceFormatter.FormatPriceAsync(await _giftCardService.GetGiftCardRemainingAmountAsync(giftCard), true, false);
        model.AmountStr = await _priceFormatter.FormatPriceAsync(giftCard.Amount, true, false);
        model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(giftCard.CreatedOnUtc, DateTimeKind.Utc);
        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
        model.PurchasedWithOrderNumber = order?.CustomOrderNumber;

        if (ModelState.IsValid)
        {
            giftCard = model.ToEntity(giftCard);
            await _giftCardService.UpdateGiftCardAsync(giftCard);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditGiftCard",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditGiftCard"), giftCard.GiftCardCouponCode), giftCard);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = giftCard.Id });
        }

        //prepare model
        model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, giftCard, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE)]
    public virtual IActionResult GenerateCouponCode()
    {
		_logger.LogError(">>> Entering GenerateCouponCode()");
        _logger.LogWarning("<<< Exiting GenerateCouponCode()");
        return Json(new { CouponCode = _giftCardService.GenerateGiftCardCode() });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("notifyRecipient")]
    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> NotifyRecipient(GiftCardModel model)
    {
		_logger.LogError(">>> Entering NotifyRecipient(model={Model})", model);
        //try to get a gift card with the specified id
        var giftCard = await _giftCardService.GetGiftCardByIdAsync(model.Id);
		_logger.LogError("NotifyRecipient(model={Model}): giftCard → {GiftCard}", model, giftCard);
        if (giftCard == null)
            return RedirectToAction("List");

        try
        {
            if (!CommonHelper.IsValidEmail(giftCard.RecipientEmail))
				_logger.LogError("Throwing NopException from NotifyRecipient(model={Model})", model);
                throw new NopException("Recipient email is not valid");

            if (!CommonHelper.IsValidEmail(giftCard.SenderEmail))
                throw new NopException("Sender email is not valid");

            var languageId = 0;
			_logger.LogError("NotifyRecipient(model={Model}): languageId → {LanguageId}", model, languageId);
            var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);
			_logger.LogError("NotifyRecipient(model={Model}): order → {Order}", model, order);

            if (order != null)
            {
                var customerLang = await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId) ?? (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
				_logger.LogError("NotifyRecipient(model={Model}): customerLang → {CustomerLang}", model, customerLang);
                if (customerLang != null)
                    languageId = customerLang.Id;
            }
            else
            {
                languageId = _localizationSettings.DefaultAdminLanguageId;
            }

            var queuedEmailIds = await _workflowMessageService.SendGiftCardNotificationAsync(giftCard, languageId);
			_logger.LogError("NotifyRecipient(model={Model}): queuedEmailIds → {QueuedEmailIds}", model, queuedEmailIds);
            if (queuedEmailIds.Any())
            {
                giftCard.IsRecipientNotified = true;
				_logger.LogDebug("NotifyRecipient(model={Model}): giftCard.IsRecipientNotified → {Arg1}", model, giftCard.IsRecipientNotified);
                await _giftCardService.UpdateGiftCardAsync(giftCard);
                model.IsRecipientNotified = true;
				_logger.LogDebug("NotifyRecipient(model={Model}): model.IsRecipientNotified → {Arg1}", model, model.IsRecipientNotified);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.RecipientNotified"));

            return RedirectToAction("Edit", new { id = giftCard.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in NotifyRecipient(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

        //prepare model
        model = await _giftCardModelFactory.PrepareGiftCardModelAsync(model, giftCard);
		_logger.LogError("NotifyRecipient(model={Model}): model → {Model1}", model, model);

		_logger.LogWarning("<<< Exiting NotifyRecipient(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a gift card with the specified id
        var giftCard = await _giftCardService.GetGiftCardByIdAsync(id);
		_logger.LogError("Delete(id={Id}): giftCard → {GiftCard}", id, giftCard);
        if (giftCard == null)
            return RedirectToAction("List");

        await _giftCardService.DeleteGiftCardAsync(giftCard);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteGiftCard",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteGiftCard"), giftCard.GiftCardCouponCode), giftCard);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.GiftCards.Deleted"));

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.GIFT_CARDS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> UsageHistoryList(GiftCardUsageHistorySearchModel searchModel)
    {
		_logger.LogError(">>> Entering UsageHistoryList(searchModel={SearchModel})", searchModel);
        //try to get a gift card with the specified id
        var giftCard = await _giftCardService.GetGiftCardByIdAsync(searchModel.GiftCardId)
                       ?? throw new ArgumentException("No gift card found with the specified id");

        //prepare model
        var model = await _giftCardModelFactory.PrepareGiftCardUsageHistoryListModelAsync(searchModel, giftCard);
		_logger.LogError("UsageHistoryList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting UsageHistoryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion
}