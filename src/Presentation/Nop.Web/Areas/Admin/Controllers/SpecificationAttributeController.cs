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
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _specificationAttributeModelFactory = specificationAttributeModelFactory;
        _specificationAttributeService = specificationAttributeService;
        _translationModelFactory = translationModelFactory;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateAttributeLocalesAsync(SpecificationAttribute specificationAttribute, SpecificationAttributeModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(specificationAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateAttributeGroupLocalesAsync(SpecificationAttributeGroup specificationAttributeGroup, SpecificationAttributeGroupModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(specificationAttributeGroup,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateOptionLocalesAsync(SpecificationAttributeOption specificationAttributeOption, SpecificationAttributeOptionModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(specificationAttributeOption,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    #endregion

    #region Specification attributes

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeSearchModelAsync(new SpecificationAttributeSearchModel());

		_logger.LogDebug("List(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> SpecificationAttributeGroupList(SpecificationAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering SpecificationAttributeGroupList(searchModel={SearchModel})", searchModel);
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupListModelAsync(searchModel);

		_logger.LogDebug("SpecificationAttributeGroupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting SpecificationAttributeGroupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> SpecificationAttributeList(SpecificationAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering SpecificationAttributeList(searchModel={SearchModel})", searchModel);
        SpecificationAttributeGroup group = null;

        if (searchModel.SpecificationAttributeGroupId > 0)
        {
            group = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(searchModel.SpecificationAttributeGroupId)
                ?? throw new ArgumentException("No specification attribute group found with the specified id");
        }

        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeListModelAsync(searchModel, group);

		_logger.LogDebug("SpecificationAttributeList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting SpecificationAttributeList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateSpecificationAttributeGroup()
    {
		_logger.LogDebug(">>> Entering CreateSpecificationAttributeGroup()");
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(new SpecificationAttributeGroupModel(), null);

		_logger.LogDebug("CreateSpecificationAttributeGroup(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting CreateSpecificationAttributeGroup()");
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
		_logger.LogDebug("<<< Exiting CreateSpecificationAttributeGroup(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateSpecificationAttribute()
    {
		_logger.LogDebug(">>> Entering CreateSpecificationAttribute()");
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(new SpecificationAttributeModel(), null);

		_logger.LogDebug("CreateSpecificationAttribute(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting CreateSpecificationAttribute()");
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
		_logger.LogDebug("<<< Exiting CreateSpecificationAttribute(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> EditSpecificationAttributeGroup(int id)
    {
		_logger.LogDebug(">>> Entering EditSpecificationAttributeGroup(id={Id})", id);
        var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(id);
        if (specificationAttributeGroup == null)
			_logger.LogDebug("EditSpecificationAttributeGroup(id={Id}): specificationAttributeGroup → {SpecificationAttributeGroup}", id, specificationAttributeGroup);
            return RedirectToAction("List");

        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeGroupModelAsync(null, specificationAttributeGroup);

		_logger.LogDebug("EditSpecificationAttributeGroup(id={Id}): model → {Model}", id, model);
		_logger.LogDebug("<<< Exiting EditSpecificationAttributeGroup(id={Id})", id);
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
		_logger.LogDebug("<<< Exiting EditSpecificationAttributeGroup(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> EditSpecificationAttribute(int id)
    {
		_logger.LogDebug(">>> Entering EditSpecificationAttribute(id={Id})", id);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);
        if (specificationAttribute == null)
			_logger.LogDebug("EditSpecificationAttribute(id={Id}): specificationAttribute → {SpecificationAttribute}", id, specificationAttribute);
            return RedirectToAction("List");

        //prepare model
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(null, specificationAttribute);

		_logger.LogDebug("EditSpecificationAttribute(id={Id}): model → {Model}", id, model);
		_logger.LogDebug("<<< Exiting EditSpecificationAttribute(id={Id})", id);
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
		_logger.LogDebug("<<< Exiting EditSpecificationAttribute(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PreTranslate(int itemId)
    {
		_logger.LogDebug(">>> Entering PreTranslate(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		_logger.LogDebug("PreTranslate(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(itemId);
        if (specificationAttribute == null)
			_logger.LogDebug("PreTranslate(itemId={ItemId}): specificationAttribute → {SpecificationAttribute}", itemId, specificationAttribute);
            return Json(translationModel);

        //prepare model
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeModelAsync(null, specificationAttribute);

		_logger.LogDebug("PreTranslate(itemId={ItemId}): model → {Model}", itemId, model);
        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model, nameof(SpecificationAttributeLocalizedModel.Name));

		_logger.LogDebug("<<< Exiting PreTranslate(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSpecificationAttributeGroup(int id)
    {
		_logger.LogDebug(">>> Entering DeleteSpecificationAttributeGroup(id={Id})", id);
        var specificationAttributeGroup = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(id);
        if (specificationAttributeGroup == null)
			_logger.LogDebug("DeleteSpecificationAttributeGroup(id={Id}): specificationAttributeGroup → {SpecificationAttributeGroup}", id, specificationAttributeGroup);
            return RedirectToAction("List");

        await _specificationAttributeService.DeleteSpecificationAttributeGroupAsync(specificationAttributeGroup);

        await _customerActivityService.InsertActivityAsync("DeleteSpecAttributeGroup",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttributeGroup"), specificationAttributeGroup.Name), specificationAttributeGroup);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Deleted"));

		_logger.LogDebug("<<< Exiting DeleteSpecificationAttributeGroup(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSpecificationAttribute(int id)
    {
		_logger.LogDebug(">>> Entering DeleteSpecificationAttribute(id={Id})", id);
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);

		_logger.LogDebug("DeleteSpecificationAttribute(id={Id}): specificationAttribute → {SpecificationAttribute}", id, specificationAttribute);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        await _specificationAttributeService.DeleteSpecificationAttributeAsync(specificationAttribute);

        await _customerActivityService.InsertActivityAsync("DeleteSpecAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute"), specificationAttribute.Name), specificationAttribute);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Deleted"));

		_logger.LogDebug("<<< Exiting DeleteSpecificationAttribute(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelectedSpecificationAttributes(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering DeleteSelectedSpecificationAttributes(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var specificationAttributes = await _specificationAttributeService.GetSpecificationAttributeByIdsAsync(selectedIds.ToArray());
        await _specificationAttributeService.DeleteSpecificationAttributesAsync(specificationAttributes);
		_logger.LogDebug("DeleteSelectedSpecificationAttributes(selectedIds={SelectedIds}): specificationAttributes → {SpecificationAttributes}", selectedIds, specificationAttributes);
        
        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute");
        await _customerActivityService.InsertActivitiesAsync("DeleteSpecAttribute", specificationAttributes, specificationAttribute => string.Format(activityLogFormat, specificationAttribute.Name));
		_logger.LogDebug("DeleteSelectedSpecificationAttributes(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		_logger.LogDebug("<<< Exiting DeleteSelectedSpecificationAttributes(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Specification attribute options

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> OptionList(SpecificationAttributeOptionSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering OptionList(searchModel={SearchModel})", searchModel);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(searchModel.SpecificationAttributeId)
            ?? throw new ArgumentException("No specification attribute found with the specified id");

        //prepare model
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeOptionListModelAsync(searchModel, specificationAttribute);

		_logger.LogDebug("OptionList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting OptionList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OptionCreatePopup(int specificationAttributeId)
    {
		_logger.LogDebug(">>> Entering OptionCreatePopup(specificationAttributeId={SpecificationAttributeId})", specificationAttributeId);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(specificationAttributeId);
        if (specificationAttribute == null)
			_logger.LogDebug("OptionCreatePopup(specificationAttributeId={SpecificationAttributeId}): specificationAttribute → {SpecificationAttribute}", specificationAttributeId, specificationAttribute);
            return RedirectToAction("List");

        //prepare model
        var model = await _specificationAttributeModelFactory
            .PrepareSpecificationAttributeOptionModelAsync(new SpecificationAttributeOptionModel(), specificationAttribute, null);

		_logger.LogDebug("<<< Exiting OptionCreatePopup(specificationAttributeId={SpecificationAttributeId})", specificationAttributeId);
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
		_logger.LogDebug("<<< Exiting OptionCreatePopup(model={Model})", model);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> OptionEditPopup(int id)
    {
		_logger.LogDebug(">>> Entering OptionEditPopup(id={Id})", id);
        //try to get a specification attribute option with the specified id
        var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id);
        if (specificationAttributeOption == null)
			_logger.LogDebug("OptionEditPopup(id={Id}): specificationAttributeOption → {SpecificationAttributeOption}", id, specificationAttributeOption);
            return RedirectToAction("List");

        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService
            .GetSpecificationAttributeByIdAsync(specificationAttributeOption.SpecificationAttributeId);
        if (specificationAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _specificationAttributeModelFactory
            .PrepareSpecificationAttributeOptionModelAsync(null, specificationAttribute, specificationAttributeOption);

		_logger.LogDebug("<<< Exiting OptionEditPopup(id={Id})", id);
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
		_logger.LogDebug("<<< Exiting OptionEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> OptionDelete(int id, int specificationAttributeId)
    {
		_logger.LogDebug(">>> Entering OptionDelete(id={Id},specificationAttributeId={SpecificationAttributeId})", id, specificationAttributeId);
        //try to get a specification attribute option with the specified id
        var specificationAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id)
            ?? throw new ArgumentException("No specification attribute option found with the specified id", nameof(id));

        await _specificationAttributeService.DeleteSpecificationAttributeOptionAsync(specificationAttributeOption);

		_logger.LogDebug("<<< Exiting OptionDelete(id={Id},specificationAttributeId={SpecificationAttributeId})", id, specificationAttributeId);
        return new NullJsonResult();
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetOptionsByAttributeId(string attributeId)
    {
		_logger.LogDebug(">>> Entering GetOptionsByAttributeId(attributeId={AttributeId})", attributeId);
        //do not make any permission validation here 
        //because this method could be used on some other pages (such as product editing)
        //if (!await _permissionService.AuthorizeAsync(StandardPermission.ManageAttributes))
        //    return await AccessDeniedJsonAsync();

        //this action method gets called via an ajax request
        ArgumentException.ThrowIfNullOrEmpty(attributeId);

        var options = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(Convert.ToInt32(attributeId));
        var result = (from o in options
            select new { id = o.Id, name = o.Name }).ToList();
			_logger.LogDebug("GetOptionsByAttributeId(attributeId={AttributeId}): options → {Options}", attributeId, options);
		_logger.LogDebug("<<< Exiting GetOptionsByAttributeId(attributeId={AttributeId})", attributeId);
        return Json(result);
    }

    #endregion

    #region Mapped products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> UsedByProducts(SpecificationAttributeProductSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering UsedByProducts(searchModel={SearchModel})", searchModel);
        //try to get a specification attribute with the specified id
        var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(searchModel.SpecificationAttributeId)
            ?? throw new ArgumentException("No specification attribute found with the specified id");

        //prepare model
        var model = await _specificationAttributeModelFactory.PrepareSpecificationAttributeProductListModelAsync(searchModel, specificationAttribute);

		_logger.LogDebug("UsedByProducts(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting UsedByProducts(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion
}