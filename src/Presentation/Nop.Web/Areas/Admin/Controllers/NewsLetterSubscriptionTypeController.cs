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
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _newsletterSubscriptionTypeModelFactory = newsletterSubscriptionTypeModelFactory;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _notificationService = notificationService;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateSubscriptionTypeLocalesAsync(NewsLetterSubscriptionType subscriptionType, NewsLetterSubscriptionTypeModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(subscriptionType,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
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
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_VIEW)]
    public virtual async Task<IActionResult> List(NewsLetterSubscriptionTypeSearchModel searchModel)
    {
        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsLetterSubscriptionTypeListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
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
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get an product subscription type with the specified id
        var subscriptionType = await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(id);
        if (subscriptionType == null)
			_logger.LogDebug("Edit(id={Id}): subscriptionType → {SubscriptionType}", id, subscriptionType);
            return RedirectToAction("List");

        //prepare model
        var model = await _newsletterSubscriptionTypeModelFactory.PrepareNewsLetterSubscriptionTypeModelAsync(null, subscriptionType);

		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);
		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
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
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIPTION_TYPE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get an product subscription type with the specified id
        var subscriptionType = await _newsLetterSubscriptionTypeService.GetNewsLetterSubscriptionTypeByIdAsync(id);
        if (subscriptionType == null)
			_logger.LogDebug("Delete(id={Id}): subscriptionType → {SubscriptionType}", id, subscriptionType);
            return RedirectToAction("List");

        try
        {
            var allTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync();
            if (allTypes.Count == 1)
			_logger.LogDebug("Delete(id={Id}): allTypes → {AllTypes}", id, allTypes);
            {
                var locale = await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.NotDeleted");
                _notificationService.WarningNotification(string.Format(locale, Url.Action("CustomerUser", "Setting")), false);
				_logger.LogDebug("Delete(id={Id}): locale → {Locale}", id, locale);
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
			_logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = subscriptionType.Id });
        }
    }

    #endregion
}
