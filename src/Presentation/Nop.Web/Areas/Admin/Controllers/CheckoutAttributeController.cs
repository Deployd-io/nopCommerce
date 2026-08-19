using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CheckoutAttributeController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly ICheckoutAttributeModelFactory _checkoutAttributeModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IMeasureService _measureService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly MeasureSettings _measureSettings;

    #endregion

    #region Ctor

    public CheckoutAttributeController(CurrencySettings currencySettings,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICheckoutAttributeModelFactory checkoutAttributeModelFactory,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IMeasureService measureService,
        IStoreMappingService storeMappingService,
        MeasureSettings measureSettings)
    {
		_logger.LogDebug(">>> Entering CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings})", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings);
        _currencySettings = currencySettings;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _currencySettings → {_currencySettings}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _currencySettings);
        _checkoutAttributeParser = checkoutAttributeParser;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _checkoutAttributeParser → {_checkoutAttributeParser}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _checkoutAttributeParser);
        _checkoutAttributeService = checkoutAttributeService;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _checkoutAttributeService → {_checkoutAttributeService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _checkoutAttributeService);
        _checkoutAttributeModelFactory = checkoutAttributeModelFactory;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _checkoutAttributeModelFactory → {_checkoutAttributeModelFactory}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _checkoutAttributeModelFactory);
        _currencyService = currencyService;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _currencyService → {_currencyService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _currencyService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _customerActivityService → {_customerActivityService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _localizationService → {_localizationService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _localizedEntityService → {_localizedEntityService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _notificationService → {_notificationService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _notificationService);
        _measureService = measureService;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _measureService → {_measureService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _measureService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _storeMappingService → {_storeMappingService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _storeMappingService);
        _measureSettings = measureSettings;
		_logger.LogDebug("<<< Exiting CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings})", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings);
		_logger.LogDebug("CheckoutAttributeController(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},checkoutAttributeModelFactory={CheckoutAttributeModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},measureService={MeasureService},storeMappingService=<redacted>,measureSettings={MeasureSettings}): _measureSettings → {_measureSettings}", currencySettings, checkoutAttributeParser, checkoutAttributeService, checkoutAttributeModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, measureService, measureSettings, _measureSettings);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateAttributeLocalesAsync(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
    {
		_logger.LogDebug(">>> Entering UpdateAttributeLocalesAsync(checkoutAttribute={CheckoutAttribute},model={Model})", checkoutAttribute, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                x => x.TextPrompt,
                localized.TextPrompt,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                x => x.DefaultValue,
                localized.DefaultValue,
                localized.LanguageId);
        }
		_logger.LogDebug("<<< Exiting UpdateAttributeLocalesAsync(checkoutAttribute={CheckoutAttribute},model={Model})", checkoutAttribute, model);
    }

    protected virtual async Task UpdateValueLocalesAsync(CheckoutAttributeValue checkoutAttributeValue, CheckoutAttributeValueModel model)
    {
		_logger.LogDebug(">>> Entering UpdateValueLocalesAsync(checkoutAttributeValue={CheckoutAttributeValue},model={Model})", checkoutAttributeValue, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(checkoutAttributeValue,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
		_logger.LogDebug("<<< Exiting UpdateValueLocalesAsync(checkoutAttributeValue={CheckoutAttributeValue},model={Model})", checkoutAttributeValue, model);
    }

    protected virtual async Task SaveConditionAttributesAsync(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
    {
		_logger.LogDebug(">>> Entering SaveConditionAttributesAsync(checkoutAttribute={CheckoutAttribute},model={Model})", checkoutAttribute, model);
        string attributesXml = null;

		_logger.LogDebug("SaveConditionAttributesAsync(checkoutAttribute={CheckoutAttribute},model={Model}): attributesXml → {AttributesXml}", checkoutAttribute, model, attributesXml);
        if (model.ConditionModel.EnableCondition)
        {
            var attribute = await _checkoutAttributeService.GetAttributeByIdAsync(model.ConditionModel.SelectedAttributeId);
			_logger.LogDebug("SaveConditionAttributesAsync(checkoutAttribute={CheckoutAttribute},model={Model}): attribute → {Attribute}", checkoutAttribute, model, attribute);
            if (attribute != null)
            {
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    {
                        var selectedAttribute = model.ConditionModel.ConditionAttributes
                            .FirstOrDefault(x => x.Id == model.ConditionModel.SelectedAttributeId);
                        var selectedValue = selectedAttribute?.SelectedValueId;

                        //for conditions we should empty values save even when nothing is selected
                        //otherwise "attributesXml" will be empty
                        //hence we won't be able to find a selected attribute
                        attributesXml = _checkoutAttributeParser.AddAttribute(null, attribute, string.IsNullOrEmpty(selectedValue) ? string.Empty : selectedValue);
                    }
                    break;

                    case AttributeControlType.Checkboxes:
                    {
                        var selectedAttribute = model.ConditionModel.ConditionAttributes
                            .FirstOrDefault(x => x.Id == model.ConditionModel.SelectedAttributeId);
                        var selectedValues = selectedAttribute?.Values
                            .Where(x => x.Selected)
                            .Select(x => x.Value)
                            .ToList();

                        if (selectedValues?.Any() ?? false)
                            foreach (var value in selectedValues)
                                attributesXml = _checkoutAttributeParser.AddAttribute(attributesXml, attribute, value);
                        else
                            attributesXml = _checkoutAttributeParser.AddAttribute(null, attribute, string.Empty);
                    }
                    break;

                    case AttributeControlType.ReadonlyCheckboxes:
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //these attribute types are not supported as conditions
                        break;
                }
            }
        }

        checkoutAttribute.ConditionAttributeXml = attributesXml;
		_logger.LogDebug("SaveConditionAttributesAsync(checkoutAttribute={CheckoutAttribute},model={Model}): checkoutAttribute.ConditionAttributeXml → {Arg2}", checkoutAttribute, model, checkoutAttribute.ConditionAttributeXml);
		_logger.LogDebug("<<< Exiting SaveConditionAttributesAsync(checkoutAttribute={CheckoutAttribute},model={Model})", checkoutAttribute, model);
    }

    #endregion

    #region Checkout attributes

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeSearchModelAsync(new CheckoutAttributeSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> List(CheckoutAttributeSearchModel searchModel)
    {
        //prepare model
        var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(new CheckoutAttributeModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(CheckoutAttributeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var checkoutAttribute = model.ToEntity<CheckoutAttribute>();
            await _checkoutAttributeService.InsertAttributeAsync(checkoutAttribute);

            //locales
            await UpdateAttributeLocalesAsync(checkoutAttribute, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(checkoutAttribute, model.SelectedStoreIds);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCheckoutAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
        }

        //prepare model
        model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a checkout attribute with the specified id
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): checkoutAttribute → {CheckoutAttribute}", id, checkoutAttribute);
        if (checkoutAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(null, checkoutAttribute);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(CheckoutAttributeModel model, bool continueEditing)
    {
        //try to get a checkout attribute with the specified id
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(model.Id);
        if (checkoutAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            checkoutAttribute = model.ToEntity(checkoutAttribute);
            await SaveConditionAttributesAsync(checkoutAttribute, model);
            await _checkoutAttributeService.UpdateAttributeAsync(checkoutAttribute);

            //locales
            await UpdateAttributeLocalesAsync(checkoutAttribute, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(checkoutAttribute, model.SelectedStoreIds);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCheckoutAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
        }

        //prepare model
        model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(model, checkoutAttribute, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a checkout attribute with the specified id
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): checkoutAttribute → {CheckoutAttribute}", id, checkoutAttribute);
        if (checkoutAttribute == null)
            return RedirectToAction("List");

        await _checkoutAttributeService.DeleteAttributeAsync(checkoutAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCheckoutAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var checkoutAttributes = await _checkoutAttributeService.GetAttributeByIdsAsync(selectedIds.ToArray());
		_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): checkoutAttributes → {CheckoutAttributes}", selectedIds, checkoutAttributes);
        await _checkoutAttributeService.DeleteAttributesAsync(checkoutAttributes);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteCheckoutAttribute");
		_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("DeleteCheckoutAttribute", checkoutAttributes, checkoutAttribute => string.Format(activityLogFormat, checkoutAttribute.Name));

		_logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region Checkout attribute values

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> ValueList(CheckoutAttributeValueSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ValueList(searchModel={SearchModel})", searchModel);
        //try to get a checkout attribute with the specified id
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(searchModel.CheckoutAttributeId)
            ?? throw new ArgumentException("No checkout attribute found with the specified id");

        //prepare model
        var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueListModelAsync(searchModel, checkoutAttribute);
		_logger.LogDebug("ValueList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ValueList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ValueCreatePopup(int checkoutAttributeId)
    {
		_logger.LogDebug(">>> Entering ValueCreatePopup(checkoutAttributeId={CheckoutAttributeId})", checkoutAttributeId);
        //try to get a checkout attribute with the specified id
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(checkoutAttributeId);
		_logger.LogDebug("ValueCreatePopup(checkoutAttributeId={CheckoutAttributeId}): checkoutAttribute → {CheckoutAttribute}", checkoutAttributeId, checkoutAttribute);
        if (checkoutAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _checkoutAttributeModelFactory
            .PrepareCheckoutAttributeValueModelAsync(new CheckoutAttributeValueModel(), checkoutAttribute, null);

		_logger.LogDebug("<<< Exiting ValueCreatePopup(checkoutAttributeId={CheckoutAttributeId})", checkoutAttributeId);
		_logger.LogDebug("<<< Exiting ValueCreatePopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ValueCreatePopup(CheckoutAttributeValueModel model)
    {
        //try to get a checkout attribute with the specified id
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(model.AttributeId);
        if (checkoutAttribute == null)
            return RedirectToAction("List");

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
        model.BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name;

        if (checkoutAttribute.AttributeControlType == AttributeControlType.ColorSquares)
        {
            //ensure valid color is chosen/entered
            if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                ModelState.AddModelError(string.Empty, "Color is required");

            try
            {
                //ensure color is valid (can be instantiated)
                System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError(string.Empty, exc.Message);
            }
        }

        if (ModelState.IsValid)
        {
            var checkoutAttributeValue = model.ToEntity<CheckoutAttributeValue>();
            await _checkoutAttributeService.InsertAttributeValueAsync(checkoutAttributeValue);

            await UpdateValueLocalesAsync(checkoutAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(model, checkoutAttribute, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_VIEW)]
    public virtual async Task<IActionResult> ValueEditPopup(int id)
    {
		_logger.LogDebug(">>> Entering ValueEditPopup(id={Id})", id);
        //try to get a checkout attribute value with the specified id
        var checkoutAttributeValue = await _checkoutAttributeService.GetAttributeValueByIdAsync(id);
		_logger.LogDebug("ValueEditPopup(id={Id}): checkoutAttributeValue → {CheckoutAttributeValue}", id, checkoutAttributeValue);
        if (checkoutAttributeValue == null)
            return RedirectToAction("List");

        //try to get a checkout attribute with the specified id
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(checkoutAttributeValue.AttributeId);
		_logger.LogDebug("ValueEditPopup(id={Id}): checkoutAttribute → {CheckoutAttribute}", id, checkoutAttribute);
        if (checkoutAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(null, checkoutAttribute, checkoutAttributeValue);
		_logger.LogDebug("ValueEditPopup(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting ValueEditPopup(id={Id})", id);
		_logger.LogDebug("<<< Exiting ValueEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ValueEditPopup(CheckoutAttributeValueModel model)
    {
        //try to get a checkout attribute value with the specified id
        var checkoutAttributeValue = await _checkoutAttributeService.GetAttributeValueByIdAsync(model.Id);
        if (checkoutAttributeValue == null)
            return RedirectToAction("List");

        //try to get a checkout attribute with the specified id
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(checkoutAttributeValue.AttributeId);
        if (checkoutAttribute == null)
            return RedirectToAction("List");

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
        model.BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name;

        if (checkoutAttribute.AttributeControlType == AttributeControlType.ColorSquares)
        {
            //ensure valid color is chosen/entered
            if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                ModelState.AddModelError(string.Empty, "Color is required");

            try
            {
                //ensure color is valid (can be instantiated)
                System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError(string.Empty, exc.Message);
            }
        }

        if (ModelState.IsValid)
        {
            checkoutAttributeValue = model.ToEntity(checkoutAttributeValue);
            await _checkoutAttributeService.UpdateAttributeValueAsync(checkoutAttributeValue);

            await UpdateValueLocalesAsync(checkoutAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(model, checkoutAttribute, checkoutAttributeValue, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CHECKOUT_ATTRIBUTES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ValueDelete(int id)
    {
		_logger.LogDebug(">>> Entering ValueDelete(id={Id})", id);
        //try to get a checkout attribute value with the specified id
        var checkoutAttributeValue = await _checkoutAttributeService.GetAttributeValueByIdAsync(id)
            ?? throw new ArgumentException("No checkout attribute value found with the specified id", nameof(id));

        await _checkoutAttributeService.DeleteAttributeValueAsync(checkoutAttributeValue);

		_logger.LogDebug("<<< Exiting ValueDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion
}