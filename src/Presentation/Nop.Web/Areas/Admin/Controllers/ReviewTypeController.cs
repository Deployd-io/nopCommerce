using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

/// <summary>
/// Represent a review type controller
/// </summary>
public partial class ReviewTypeController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IReviewTypeModelFactory _reviewTypeModelFactory;
    protected readonly IReviewTypeService _reviewTypeService;

    #endregion

    #region Ctor

    public ReviewTypeController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IReviewTypeModelFactory reviewTypeModelFactory,
        IReviewTypeService reviewTypeService)
    {
		_logger.LogDebug(">>> Entering ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService})", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService);
        _reviewTypeModelFactory = reviewTypeModelFactory;
		_logger.LogDebug("ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService}): _reviewTypeModelFactory → {_reviewTypeModelFactory}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService, _reviewTypeModelFactory);
        _reviewTypeService = reviewTypeService;
		_logger.LogDebug("ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService}): _reviewTypeService → {_reviewTypeService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService, _reviewTypeService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService}): _customerActivityService → {_customerActivityService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService, _customerActivityService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService}): _localizedEntityService → {_localizedEntityService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService, _localizedEntityService);
        _localizationService = localizationService;
		_logger.LogDebug("ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService}): _localizationService → {_localizationService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService}): _notificationService → {_notificationService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("<<< Exiting ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService})", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService);
		_logger.LogDebug("ReviewTypeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},reviewTypeModelFactory={ReviewTypeModelFactory},reviewTypeService={ReviewTypeService}): _permissionService → {_permissionService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, reviewTypeModelFactory, reviewTypeService, _permissionService);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateReviewTypeLocalesAsync(ReviewType reviewType, ReviewTypeModel model)
    {
		_logger.LogDebug(">>> Entering UpdateReviewTypeLocalesAsync(reviewType={ReviewType},model={Model})", reviewType, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(reviewType,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(reviewType,
                x => x.Description,
                localized.Description,
                localized.LanguageId);
        }
		_logger.LogDebug("<<< Exiting UpdateReviewTypeLocalesAsync(reviewType={ReviewType},model={Model})", reviewType, model);
    }

    #endregion

    #region Review type

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual IActionResult List()
    {
		_logger.LogDebug(">>> Entering List()");
        //select an appropriate card
        SaveSelectedCardName("catalogsettings-review-types");

        //we just redirect a user to the catalog settings page
		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return RedirectToAction("Catalog", "Setting");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> List(ReviewTypeSearchModel searchModel)
    {
        //prepare model
        var model = await _reviewTypeModelFactory.PrepareReviewTypeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _reviewTypeModelFactory.PrepareReviewTypeModelAsync(new ReviewTypeModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create(ReviewTypeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var reviewType = model.ToEntity<ReviewType>();
            await _reviewTypeService.InsertReviewTypeAsync(reviewType);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewReviewType",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewReviewType"), reviewType.Id), reviewType);

            //locales                
            await UpdateReviewTypeLocalesAsync(reviewType, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Settings.ReviewType.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = reviewType.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _reviewTypeModelFactory.PrepareReviewTypeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get an product review type with the specified id
        var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): reviewType → {ReviewType}", id, reviewType);
        if (reviewType == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _reviewTypeModelFactory.PrepareReviewTypeModelAsync(null, reviewType);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(ReviewTypeModel model, bool continueEditing)
    {
        //try to get an review type with the specified id
        var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(model.Id);
        if (reviewType == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            reviewType = model.ToEntity(reviewType);
            await _reviewTypeService.UpdateReviewTypeAsync(reviewType);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditReviewType",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditReviewType"), reviewType.Id),
                reviewType);

            //locales
            await UpdateReviewTypeLocalesAsync(reviewType, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Settings.ReviewType.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = reviewType.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _reviewTypeModelFactory.PrepareReviewTypeModelAsync(model, reviewType, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get an review type with the specified id
        var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): reviewType → {ReviewType}", id, reviewType);
        if (reviewType == null)
            return RedirectToAction("List");

        try
        {
            await _reviewTypeService.DeleteReviewTypeAsync(reviewType);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteReviewType",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteReviewType"), reviewType),
                reviewType);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Settings.ReviewType.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = reviewType.Id });
        }
    }

    #endregion
}