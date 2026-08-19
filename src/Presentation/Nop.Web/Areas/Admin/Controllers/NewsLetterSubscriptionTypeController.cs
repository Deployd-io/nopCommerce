using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class NewsLetterSubscriptionTypeController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INewsLetterSubscriptionTypeModelFactory _newsletterSubscriptionTypeModelFactory;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly INotificationService _notificationService;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionTypeController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INewsLetterSubscriptionTypeModelFactory newsletterSubscriptionTypeModelFactory,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        INotificationService notificationService,
        IStoreMappingService storeMappingService)
    {
		_logger.LogDebug(">>> Entering NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>)", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>): _customerActivityService → {_customerActivityService}", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>): _localizationService → {_localizationService}", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>): _localizedEntityService → {_localizedEntityService}", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService, _localizedEntityService);
        _newsletterSubscriptionTypeModelFactory = newsletterSubscriptionTypeModelFactory;
		_logger.LogDebug("NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>): _newsletterSubscriptionTypeModelFactory → {_newsletterSubscriptionTypeModelFactory}", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService, _newsletterSubscriptionTypeModelFactory);
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
		_logger.LogDebug("NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>): _newsLetterSubscriptionTypeService → {_newsLetterSubscriptionTypeService}", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService, _newsLetterSubscriptionTypeService);
        _notificationService = notificationService;
		_logger.LogDebug("NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>): _notificationService → {_notificationService}", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService, _notificationService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService, _storeMappingService);
		_logger.LogDebug("<<< Exiting NewsLetterSubscriptionTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},newsletterSubscriptionTypeModelFactory={NewsletterSubscriptionTypeModelFactory},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},storeMappingService=<redacted>)", customerActivityService, localizationService, localizedEntityService, newsletterSubscriptionTypeModelFactory, newsLetterSubscriptionTypeService, notificationService);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateSubscriptionTypeLocalesAsync(NewsLetterSubscriptionType subscriptionType, NewsLetterSubscriptionTypeModel model)
    {
		_logger.LogDebug(">>> Entering UpdateSubscriptionTypeLocalesAsync(subscriptionType={SubscriptionType},model={Model})", subscriptionType, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(subscriptionType,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogDebug("<<< Exiting UpdateSubscriptionTypeLocalesAsync(subscriptionType={SubscriptionType},model={Model})", subscriptionType, model);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsLetterSubscriptionTypeSearchModelAsync(new NewsLetterSubscriptionTypeSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_VIEW)]
    public virtual async Task<IActionResult> List(NewsLetterSubscriptionTypeSearchModel searchModel)
    {
        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsLetterSubscriptionTypeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsLetterSubscriptionTypeModelAsync(new NewsLetterSubscriptionTypeModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(NewsLetterSubscriptionTypeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var subscriptionType = model.ToEntity<NewsLetterSubscriptionType>();
            await _newsLetterSubscriptionTypeService.InsertNewsLetterSubscriptionTypeAsync(subscriptionType);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddSubscriptionType",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddSubscriptionType"), subscriptionType.Id), subscriptionType);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(subscriptionType, model.SelectedStoreIds);

            //locales                
            await UpdateSubscriptionTypeLocalesAsync(subscriptionType, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = subscriptionType.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsLetterSubscriptionTypeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get an product subscription type with the specified id
        var subscriptionType = await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): subscriptionType → {SubscriptionType}", id, subscriptionType);
        if (subscriptionType == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsLetterSubscriptionTypeModelAsync(null, subscriptionType);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(NewsLetterSubscriptionTypeModel model, bool continueEditing)
    {
        //try to get an product subscription type with the specified id
        var subscriptionType = await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(model.Id);
        if (subscriptionType == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            //fill entity from model
            subscriptionType = model.ToEntity(subscriptionType);
            await _newsLetterSubscriptionTypeService.UpdateNewsLetterSubscriptionTypeAsync(subscriptionType);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditSubscriptionType",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditSubscriptionType"), subscriptionType.Id),
                subscriptionType);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(subscriptionType, model.SelectedStoreIds);

            //locales                
            await UpdateSubscriptionTypeLocalesAsync(subscriptionType, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = subscriptionType.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsLetterSubscriptionTypeModelAsync(model, subscriptionType, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get an product subscription type with the specified id
        var subscriptionType = await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): subscriptionType → {SubscriptionType}", id, subscriptionType);
        if (subscriptionType == null)
            return RedirectToAction("List");

        try
        {
            var allTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync();
			_logger.LogDebug("Delete(id={Id}): allTypes → {AllTypes}", id, allTypes);
            if (allTypes.Count == 1)
            {
                var locale = await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.NotDeleted");
				_logger.LogDebug("Delete(id={Id}): locale → {Locale}", id, locale);
                _notificationService.WarningNotification(string.Format(locale, Url.Action("CustomerUser", "Setting")), false);
            }
            else
            {
                await _newsLetterSubscriptionTypeService.DeleteNewsLetterSubscriptionTypeAsync(subscriptionType);

                //activity log
                await _customerActivityService.InsertActivityAsync("DeleteSubscriptionType",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSubscriptionType"), subscriptionType.Id),
                    subscriptionType);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.Deleted"));
            }

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = subscriptionType.Id });
        }
    }

    #endregion
}
