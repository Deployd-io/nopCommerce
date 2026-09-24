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
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class SpecificationAttributeController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISpecificationAttributeModelFactory _specificationAttributeModelFactory;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly ITranslationModelFactory _translationModelFactory;

    #endregion Fields

    #region Ctor

    public SpecificationAttributeController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISpecificationAttributeModelFactory specificationAttributeModelFactory,
        ISpecificationAttributeService specificationAttributeService,
        ITranslationModelFactory translationModelFactory)
    {
		_logger.LogError(">>> Entering SpecificationAttributeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},specificationAttributeModelFactory={SpecificationAttributeModelFactory},specificationAttributeService={SpecificationAttributeService},translationModelFactory={TranslationModelFactory})", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, specificationAttributeModelFactory, specificationAttributeService, translationModelFactory);
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
		_logger.LogDebug("SpecificationAttributeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},specificationAttributeModelFactory={SpecificationAttributeModelFactory},specificationAttributeService={SpecificationAttributeService},translationModelFactory={TranslationModelFactory}): _localizationService → {_localizationService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, specificationAttributeModelFactory, specificationAttributeService, translationModelFactory, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("SpecificationAttributeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},specificationAttributeModelFactory={SpecificationAttributeModelFactory},specificationAttributeService={SpecificationAttributeService},translationModelFactory={TranslationModelFactory}): _localizedEntityService → {_localizedEntityService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, specificationAttributeModelFactory, specificationAttributeService, translationModelFactory, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("SpecificationAttributeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},specificationAttributeModelFactory={SpecificationAttributeModelFactory},specificationAttributeService={SpecificationAttributeService},translationModelFactory={TranslationModelFactory}): _notificationService → {_notificationService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, specificationAttributeModelFactory, specificationAttributeService, translationModelFactory, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("SpecificationAttributeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},specificationAttributeModelFactory={SpecificationAttributeModelFactory},specificationAttributeService={SpecificationAttributeService},translationModelFactory={TranslationModelFactory}): _permissionService → {_permissionService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, specificationAttributeModelFactory, specificationAttributeService, translationModelFactory, _permissionService);
        _specificationAttributeModelFactory = specificationAttributeModelFactory;
		_logger.LogDebug("SpecificationAttributeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},specificationAttributeModelFactory={SpecificationAttributeModelFactory},specificationAttributeService={SpecificationAttributeService},translationModelFactory={TranslationModelFactory}): _specificationAttributeModelFactory → {_specificationAttributeModelFactory}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, specificationAttributeModelFactory, specificationAttributeService, translationModelFactory, _specificationAttributeModelFactory);
        _specificationAttributeService = specificationAttributeService;
		_logger.LogDebug("SpecificationAttributeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},specificationAttributeModelFactory={SpecificationAttributeModelFactory},specificationAttributeService={SpecificationAttributeService},translationModelFactory={TranslationModelFactory}): _specificationAttributeService → {_specificationAttributeService}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, specificationAttributeModelFactory, specificationAttributeService, translationModelFactory, _specificationAttributeService);
        _translationModelFactory = translationModelFactory;
		_logger.LogDebug("SpecificationAttributeController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},permissionService={PermissionService},specificationAttributeModelFactory={SpecificationAttributeModelFactory},specificationAttributeService={SpecificationAttributeService},translationModelFactory={TranslationModelFactory}): _translationModelFactory → {_translationModelFactory}", customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, specificationAttributeModelFactory, specificationAttributeService, translationModelFactory, _translationModelFactory);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateAttributeLocalesAsync(SpecificationAttribute specificationAttribute, SpecificationAttributeModel model)
    {
		_logger.LogError(">>> Entering UpdateAttributeLocalesAsync(specificationAttribute={SpecificationAttribute},model={Model})", specificationAttribute, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(specificationAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateAttributeLocalesAsync(specificationAttribute={SpecificationAttribute},model={Model})", specificationAttribute, model);
    }

    protected virtual async Task UpdateAttributeGroupLocalesAsync(SpecificationAttributeGroup specificationAttributeGroup, SpecificationAttributeGroupModel model)
    {
		_logger.LogError(">>> Entering UpdateAttributeGroupLocalesAsync(specificationAttributeGroup={SpecificationAttributeGroup},model={Model})", specificationAttributeGroup, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(specificationAttributeGroup,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateAttributeGroupLocalesAsync(specificationAttributeGroup={SpecificationAttributeGroup},model={Model})", specificationAttributeGroup, model);
    }

    protected virtual async Task UpdateOptionLocalesAsync(SpecificationAttributeOption specificationAttributeOption, SpecificationAttributeOptionModel model)
    {
		_logger.LogError(">>> Entering UpdateOptionLocalesAsync(specificationAttributeOption={SpecificationAttributeOption},model={Model})", specificationAttributeOption, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(specificationAttributeOption,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateOptionLocalesAsync(specificationAttributeOption={SpecificationAttributeOption},model={Model})", specificationAttributeOption, model);
    }

    #endregion

    #region Specification attributes

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogError(">>> Entering List()");
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeSearchModelAsync(new SpecificationAttributeSearchModel());

		_logger.LogError("List(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> SpecificationAttributeGroupList(SpecificationAttributeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering SpecificationAttributeGroupList(searchModel={SearchModel})", searchModel);
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupListModelAsync(searchModel);

		_logger.LogError("SpecificationAttributeGroupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogWarning("<<< Exiting SpecificationAttributeGroupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> SpecificationAttributeList(SpecificationAttributeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering SpecificationAttributeList(searchModel={SearchModel})", searchModel);
        SpecificationAttributeGroup group = null;

        if (searchModel.SpecificationAttributeGroupId > 0)
        {
            group = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(searchModel.SpecificationAttributeGroupId)
                ?? throw new ArgumentException("No specification attribute group found with the specified id");
        }

        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeListModelAsync(searchModel, group);
		_logger.LogError("SpecificationAttributeList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting SpecificationAttributeList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateSpecificationAttributeGroup()
    {
		_logger.LogError(">>> Entering CreateSpecificationAttributeGroup()");
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(new SpecificationAttributeGroupModel(), null);

		_logger.LogError("CreateSpecificationAttributeGroup(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting CreateSpecificationAttributeGroup()");
		_logger.LogWarning("<<< Exiting CreateSpecificationAttributeGroup(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var specificationAttributeGroup = model.ToEntity<SpecificationAttributeGroup>();
            await _specificationAttributeService.InsertSpecificationAttributeGroupAsync(specificationAttributeGroup);
            await UpdateAttributeGroupLocalesAsync(specificationAttributeGroup, model);

            await _customerActivityService.InsertActivityAsync("AddNewSpecAttributeGroup",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
        }

        model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateSpecificationAttribute()
    {
		_logger.LogError(">>> Entering CreateSpecificationAttribute()");
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(new SpecificationAttributeModel(), null);

		_logger.LogError("CreateSpecificationAttribute(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting CreateSpecificationAttribute()");
		_logger.LogWarning("<<< Exiting CreateSpecificationAttribute(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var specificationAttribute = model.ToEntity<SpecificationAttribute>();
            await _specificationAttributeService.InsertSpecificationAttributeAsync(specificationAttribute);
            await UpdateAttributeLocalesAsync(specificationAttribute, model);

            await _customerActivityService.InsertActivityAsync("AddNewSpecAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewSpecAttribute"), specificationAttribute.Name), specificationAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
        }

        model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> EditSpecificationAttributeGroup(int id)
    {
		_logger.LogError(">>> Entering EditSpecificationAttributeGroup(id={Id})", id);
        var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(id);
        if (specificationAttributeGroup == null)
            return RedirectToAction("List");

        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(null, specificationAttributeGroup);
		_logger.LogError("EditSpecificationAttributeGroup(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting EditSpecificationAttributeGroup(id={Id})", id);
		_logger.LogWarning("<<< Exiting EditSpecificationAttributeGroup(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditSpecificationAttributeGroup(SpecificationAttributeGroupModel model, bool continueEditing)
    {
        var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(model.Id);
        if (specificationAttributeGroup == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            specificationAttributeGroup = model.ToEntity(specificationAttributeGroup);
            await _specificationAttributeService.UpdateSpecificationAttributeGroupAsync(specificationAttributeGroup);
            await UpdateAttributeGroupLocalesAsync(specificationAttributeGroup, model);

            await _customerActivityService.InsertActivityAsync("EditSpecAttributeGroup",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("EditSpecificationAttributeGroup", new { id = specificationAttributeGroup.Id });
        }

        model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(model, specificationAttributeGroup, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> EditSpecificationAttribute(int id)
    {
		_logger.LogError(">>> Entering EditSpecificationAttribute(id={Id})", id);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);
		_logger.LogError("EditSpecificationAttribute(id={Id}): specificationAttribute → {SpecificationAttribute}", id, specificationAttribute);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(null, specificationAttribute);
		_logger.LogError("EditSpecificationAttribute(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting EditSpecificationAttribute(id={Id})", id);
		_logger.LogWarning("<<< Exiting EditSpecificationAttribute(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditSpecificationAttribute(SpecificationAttributeModel model, bool continueEditing)
    {
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(model.Id);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            specificationAttribute = model.ToEntity(specificationAttribute);
            await _specificationAttributeService.UpdateSpecificationAttributeAsync(specificationAttribute);

            await UpdateAttributeLocalesAsync(specificationAttribute, model);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditSpecAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditSpecAttribute"), specificationAttribute.Name), specificationAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("EditSpecificationAttribute", new { id = specificationAttribute.Id });
        }

        //prepare model
        model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(model, specificationAttribute, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PreTranslate(int itemId)
    {
		_logger.LogError(">>> Entering PreTranslate(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		_logger.LogError("PreTranslate(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(itemId);
		_logger.LogError("PreTranslate(itemId={ItemId}): specificationAttribute → {SpecificationAttribute}", itemId, specificationAttribute);
        if (specificationAttribute == null)
            return Json(translationModel);

        //prepare model
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(null, specificationAttribute);
		_logger.LogError("PreTranslate(itemId={ItemId}): model → {Model}", itemId, model);

        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model, nameof(SpecificationAttributeLocalizedModel.Name));

		_logger.LogWarning("<<< Exiting PreTranslate(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSpecificationAttributeGroup(int id)
    {
		_logger.LogError(">>> Entering DeleteSpecificationAttributeGroup(id={Id})", id);
        var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(id);
        if (specificationAttributeGroup == null)
            return RedirectToAction("List");

        await _specificationAttributeService.DeleteSpecificationAttributeGroupAsync(specificationAttributeGroup);

        await _customerActivityService.InsertActivityAsync("DeleteSpecAttributeGroup",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteSpecificationAttributeGroup(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSpecificationAttribute(int id)
    {
		_logger.LogError(">>> Entering DeleteSpecificationAttribute(id={Id})", id);
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);

		_logger.LogError("DeleteSpecificationAttribute(id={Id}): specificationAttribute → {SpecificationAttribute}", id, specificationAttribute);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        await _specificationAttributeService.DeleteSpecificationAttributeAsync(specificationAttribute);

        await _customerActivityService.InsertActivityAsync("DeleteSpecAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteSpecificationAttribute(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelectedSpecificationAttributes(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DeleteSelectedSpecificationAttributes(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var specificationAttributes = await _specificationAttributeService.GetSpecificationAttributeByIdsAsync(selectedIds.ToArray());
		_logger.LogError("DeleteSelectedSpecificationAttributes(selectedIds={SelectedIds}): specificationAttributes → {SpecificationAttributes}", selectedIds, specificationAttributes);
        await _specificationAttributeService.DeleteSpecificationAttributesAsync(specificationAttributes);
        
        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute");
		_logger.LogError("DeleteSelectedSpecificationAttributes(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("DeleteSpecAttribute", specificationAttributes, specificationAttribute => string.Format(activityLogFormat, specificationAttribute.Name));

		_logger.LogWarning("<<< Exiting DeleteSelectedSpecificationAttributes(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Specification attribute options

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> OptionList(SpecificationAttributeOptionSearchModel searchModel)
    {
		_logger.LogError(">>> Entering OptionList(searchModel={SearchModel})", searchModel);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(searchModel.SpecificationAttributeId)
            ?? throw new ArgumentException("No specification attribute found with the specified id");

        //prepare model
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionListModelAsync(searchModel, specificationAttribute);
		_logger.LogError("OptionList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting OptionList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OptionCreatePopup(int specificationAttributeId)
    {
		_logger.LogError(">>> Entering OptionCreatePopup(specificationAttributeId={SpecificationAttributeId})", specificationAttributeId);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(specificationAttributeId);
		_logger.LogError("OptionCreatePopup(specificationAttributeId={SpecificationAttributeId}): specificationAttribute → {SpecificationAttribute}", specificationAttributeId, specificationAttribute);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _specificationAttributeModelFactory
            .PrepareSpecificationAttributeOptionModelAsync(new SpecificationAttributeOptionModel(), specificationAttribute, null);

		_logger.LogWarning("<<< Exiting OptionCreatePopup(specificationAttributeId={SpecificationAttributeId})", specificationAttributeId);
		_logger.LogWarning("<<< Exiting OptionCreatePopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OptionCreatePopup(SpecificationAttributeOptionModel model)
    {
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(model.SpecificationAttributeId);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var sao = model.ToEntity<SpecificationAttributeOption>();

            //clear "Color" values if it's disabled
            if (!model.EnableColorSquaresRgb)
                sao.ColorSquaresRgb = null;

            await _specificationAttributeService.InsertSpecificationAttributeOptionAsync(sao);

            await UpdateOptionLocalesAsync(sao, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionModelAsync(model, specificationAttribute, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> OptionEditPopup(int id)
    {
		_logger.LogError(">>> Entering OptionEditPopup(id={Id})", id);
        //try to get a specification attribute option with the specified id
        var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id);
		_logger.LogError("OptionEditPopup(id={Id}): specificationAttributeOption → {SpecificationAttributeOption}", id, specificationAttributeOption);
        if (specificationAttributeOption == null)
            return RedirectToAction("List");

        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService
            .GetSpecificationAttributeByIdAsync(specificationAttributeOption.SpecificationAttributeId);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _specificationAttributeModelFactory
            .PrepareSpecificationAttributeOptionModelAsync(null, specificationAttribute, specificationAttributeOption);

		_logger.LogWarning("<<< Exiting OptionEditPopup(id={Id})", id);
		_logger.LogWarning("<<< Exiting OptionEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OptionEditPopup(SpecificationAttributeOptionModel model)
    {
        //try to get a specification attribute option with the specified id
        var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(model.Id);
        if (specificationAttributeOption == null)
            return RedirectToAction("List");

        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService
            .GetSpecificationAttributeByIdAsync(specificationAttributeOption.SpecificationAttributeId);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            specificationAttributeOption = model.ToEntity(specificationAttributeOption);

            //clear "Color" values if it's disabled
            if (!model.EnableColorSquaresRgb)
                specificationAttributeOption.ColorSquaresRgb = null;

            await _specificationAttributeService.UpdateSpecificationAttributeOptionAsync(specificationAttributeOption);

            await UpdateOptionLocalesAsync(specificationAttributeOption, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _specificationAttributeModelFactory
            .PrepareSpecificationAttributeOptionModelAsync(model, specificationAttribute, specificationAttributeOption, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OptionDelete(int id, int specificationAttributeId)
    {
		_logger.LogError(">>> Entering OptionDelete(id={Id},specificationAttributeId={SpecificationAttributeId})", id, specificationAttributeId);
        //try to get a specification attribute option with the specified id
        var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id)
            ?? throw new ArgumentException("No specification attribute option found with the specified id", nameof(id));

        await _specificationAttributeService.DeleteSpecificationAttributeOptionAsync(specificationAttributeOption);

		_logger.LogWarning("<<< Exiting OptionDelete(id={Id},specificationAttributeId={SpecificationAttributeId})", id, specificationAttributeId);
        return new NullJsonResult();
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetOptionsByAttributeId(string attributeId)
    {
		_logger.LogError(">>> Entering GetOptionsByAttributeId(attributeId={AttributeId})", attributeId);
        //do not make any permission validation here 
        //because this method could be used on some other pages (such as product editing)
        //if (!await _permissionService.AuthorizeAsync(StandardPermission.ManageAttributes))
        //    return await AccessDeniedJsonAsync();

        //this action method gets called via an ajax request
        ArgumentException.ThrowIfNullOrEmpty(attributeId);

        var options = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(Convert.ToInt32(attributeId));
		_logger.LogError("GetOptionsByAttributeId(attributeId={AttributeId}): options → {Options}", attributeId, options);
        var result = (from o in options
            select new { id = o.Id, name = o.Name }).ToList();
		_logger.LogWarning("<<< Exiting GetOptionsByAttributeId(attributeId={AttributeId})", attributeId);
        return Json(result);
    }

    #endregion

    #region Mapped products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> UsedByProducts(SpecificationAttributeProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering UsedByProducts(searchModel={SearchModel})", searchModel);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(searchModel.SpecificationAttributeId)
            ?? throw new ArgumentException("No specification attribute found with the specified id");

        //prepare model
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeProductListModelAsync(searchModel, specificationAttribute);
		_logger.LogError("UsedByProducts(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting UsedByProducts(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion
}