using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the checkout attribute model factory implementation
/// </summary>
public partial class CheckoutAttributeModelFactory : ICheckoutAttributeModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IMeasureService _measureService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly MeasureSettings _measureSettings;

    #endregion

    #region Ctor

    public CheckoutAttributeModelFactory(CurrencySettings currencySettings,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICurrencyService currencyService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IMeasureService measureService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        MeasureSettings measureSettings)
    {
		_logger.LogError(">>> Entering CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings})", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings);
        _currencySettings = currencySettings;
        _checkoutAttributeParser = checkoutAttributeParser;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _checkoutAttributeParser → {_checkoutAttributeParser}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _checkoutAttributeParser);
        _checkoutAttributeService = checkoutAttributeService;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _checkoutAttributeService → {_checkoutAttributeService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _checkoutAttributeService);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _baseAdminModelFactory → {_baseAdminModelFactory}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _baseAdminModelFactory);
        _currencyService = currencyService;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _currencyService → {_currencyService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _currencyService);
        _localizationService = localizationService;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _localizationService → {_localizationService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _localizedModelFactory → {_localizedModelFactory}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _localizedModelFactory);
        _measureService = measureService;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _measureService → {_measureService}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _measureService);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _storeMappingSupportedModelFactory → {_storeMappingSupportedModelFactory}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _storeMappingSupportedModelFactory);
        _measureSettings = measureSettings;
		_logger.LogDebug("CheckoutAttributeModelFactory(currencySettings={CurrencySettings},checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},measureService={MeasureService},storeMappingSupportedModelFactory=<redacted>,measureSettings={MeasureSettings}): _measureSettings → {_measureSettings}", currencySettings, checkoutAttributeParser, checkoutAttributeService, baseAdminModelFactory, currencyService, localizationService, localizedModelFactory, measureService, measureSettings, _measureSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare condition attributes model
    /// </summary>
    /// <param name="model">Condition attributes model</param>
    /// <param name="checkoutAttribute">Checkout attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareConditionAttributesModelAsync(ConditionModel model, CheckoutAttribute checkoutAttribute)
    {
		_logger.LogError(">>> Entering PrepareConditionAttributesModelAsync(model={Model},checkoutAttribute={CheckoutAttribute})", model, checkoutAttribute);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(checkoutAttribute);

        model.EnableCondition = !string.IsNullOrEmpty(checkoutAttribute.ConditionAttributeXml);

        //get selected checkout attribute
        var selectedAttribute = (await _checkoutAttributeParser.ParseAttributesAsync(checkoutAttribute.ConditionAttributeXml)).FirstOrDefault();
		_logger.LogError("PrepareConditionAttributesModelAsync(model={Model},checkoutAttribute={CheckoutAttribute}): selectedAttribute → {SelectedAttribute}", model, checkoutAttribute, selectedAttribute);
		_logger.LogDebug("PrepareConditionAttributesModelAsync(model={Model},checkoutAttribute={CheckoutAttribute}): model.SelectedAttributeId → {Arg2}", model, checkoutAttribute, model.SelectedAttributeId);
        model.SelectedAttributeId = selectedAttribute?.Id ?? 0;

        //get selected checkout attribute values identifiers
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var selectedValuesIds = await _checkoutAttributeParser
            .ParseAttributeValues(checkoutAttribute.ConditionAttributeXml).SelectMany(ta => ta.values.Select(v => v.Id)).ToListAsync();
			_logger.LogError("PrepareConditionAttributesModelAsync(model,checkoutAttribute)={Model},{CheckoutAttribute}: find query executed in {Arg2} ms", model, checkoutAttribute, __logStart.ElapsedMilliseconds);

        //get available condition checkout attributes (ignore this attribute and non-combinable attributes)
        var availableConditionAttributes = (await _checkoutAttributeService.GetAllAttributesAsync())
            .Where(attribute => attribute.Id != checkoutAttribute.Id && attribute.CanBeUsedAsCondition);

        model.ConditionAttributes = await availableConditionAttributes.SelectAwait(async attribute => new AttributeConditionModel
        {
            Id = attribute.Id,
            Name = attribute.Name,
            AttributeControlType = attribute.AttributeControlType,
            Values = (await _checkoutAttributeService.GetAttributeValuesAsync(attribute.Id)).Select(value => new SelectListItem
            {
                Text = value.Name,
                Value = value.Id.ToString(),
                Selected = selectedAttribute?.Id == attribute.Id && selectedValuesIds.Contains(value.Id)
            }).ToList()
        }).ToListAsync();
		_logger.LogWarning("<<< Exiting PrepareConditionAttributesModelAsync(model={Model},checkoutAttribute={CheckoutAttribute})", model, checkoutAttribute);
    }

    /// <summary>
    /// Prepare checkout attribute value search model
    /// </summary>
    /// <param name="searchModel">Checkout attribute value search model</param>
    /// <param name="checkoutAttribute">Checkout attribute</param>
    /// <returns>Checkout attribute value search model</returns>
    protected virtual CheckoutAttributeValueSearchModel PrepareCheckoutAttributeValueSearchModel(CheckoutAttributeValueSearchModel searchModel,
        CheckoutAttribute checkoutAttribute)
    {
		_logger.LogError(">>> Entering PrepareCheckoutAttributeValueSearchModel(searchModel={SearchModel},checkoutAttribute={CheckoutAttribute})", searchModel, checkoutAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(checkoutAttribute);

        searchModel.CheckoutAttributeId = checkoutAttribute.Id;
		_logger.LogDebug("PrepareCheckoutAttributeValueSearchModel(searchModel={SearchModel},checkoutAttribute={CheckoutAttribute}): searchModel.CheckoutAttributeId → {Arg2}", searchModel, checkoutAttribute, searchModel.CheckoutAttributeId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCheckoutAttributeValueSearchModel(searchModel={SearchModel},checkoutAttribute={CheckoutAttribute})", searchModel, checkoutAttribute);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare checkout attribute search model
    /// </summary>
    /// <param name="searchModel">Checkout attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkout attribute search model
    /// </returns>
    public virtual Task<CheckoutAttributeSearchModel> PrepareCheckoutAttributeSearchModelAsync(CheckoutAttributeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareCheckoutAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareCheckoutAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged checkout attribute list model
    /// </summary>
    /// <param name="searchModel">Checkout attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkout attribute list model
    /// </returns>
    public virtual async Task<CheckoutAttributeListModel> PrepareCheckoutAttributeListModelAsync(CheckoutAttributeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareCheckoutAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get checkout attributes
        var checkoutAttributes = (await _checkoutAttributeService.GetAllAttributesAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareCheckoutAttributeListModelAsync(searchModel={SearchModel}): checkoutAttributes → {CheckoutAttributes}", searchModel, checkoutAttributes);

        //prepare list model
        var model = await new CheckoutAttributeListModel().PrepareToGridAsync(searchModel, checkoutAttributes, () =>
        {
            return checkoutAttributes.SelectAwait(async attribute =>
            {
                //fill in model values from the entity
                var attributeModel = attribute.ToModel<CheckoutAttributeModel>();
				_logger.LogError("PrepareCheckoutAttributeListModelAsync(searchModel={SearchModel}): attributeModel → {AttributeModel}", searchModel, attributeModel);

                //fill in additional values (not existing in the entity)
                attributeModel.AttributeControlTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeControlType);

                return attributeModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareCheckoutAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare checkout attribute model
    /// </summary>
    /// <param name="model">Checkout attribute model</param>
    /// <param name="checkoutAttribute">Checkout attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkout attribute model
    /// </returns>
    public virtual async Task<CheckoutAttributeModel> PrepareCheckoutAttributeModelAsync(CheckoutAttributeModel model,
        CheckoutAttribute checkoutAttribute, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareCheckoutAttributeModelAsync(model={Model},checkoutAttribute={CheckoutAttribute},excludeProperties={ExcludeProperties})", model, checkoutAttribute, excludeProperties);
        Func<CheckoutAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (checkoutAttribute != null)
        {
            //fill in model values from the entity
            model ??= checkoutAttribute.ToModel<CheckoutAttributeModel>();

            //prepare nested search model
            PrepareCheckoutAttributeValueSearchModel(model.CheckoutAttributeValueSearchModel, checkoutAttribute);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(checkoutAttribute, entity => entity.Name, languageId, false, false);
                locale.TextPrompt = await _localizationService.GetLocalizedAsync(checkoutAttribute, entity => entity.TextPrompt, languageId, false, false);
                locale.DefaultValue = await _localizationService.GetLocalizedAsync(checkoutAttribute, entity => entity.DefaultValue, languageId, false, false);
            };

            //whether to fill in some of properties
            if (!excludeProperties)
                model.TaxCategoryId = checkoutAttribute.TaxCategoryId;
				_logger.LogDebug("PrepareCheckoutAttributeModelAsync(model={Model},checkoutAttribute={CheckoutAttribute},excludeProperties={ExcludeProperties}): model.TaxCategoryId → {Arg3}", model, checkoutAttribute, excludeProperties, model.TaxCategoryId);

            //prepare condition attributes model
            await PrepareConditionAttributesModelAsync(model.ConditionModel, checkoutAttribute);
        }

        //currently any checkout attribute can have condition
        model.ConditionAllowed = true;
		_logger.LogDebug("PrepareCheckoutAttributeModelAsync(model={Model},checkoutAttribute={CheckoutAttribute},excludeProperties={ExcludeProperties}): model.ConditionAllowed → {Arg3}", model, checkoutAttribute, excludeProperties, model.ConditionAllowed);

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available tax categories
        await _baseAdminModelFactory.PrepareTaxCategoriesAsync(model.AvailableTaxCategories);

        //prepare model stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, checkoutAttribute, excludeProperties);

		_logger.LogWarning("<<< Exiting PrepareCheckoutAttributeModelAsync(model={Model},checkoutAttribute={CheckoutAttribute},excludeProperties={ExcludeProperties})", model, checkoutAttribute, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged checkout attribute value list model
    /// </summary>
    /// <param name="searchModel">Checkout attribute value search model</param>
    /// <param name="checkoutAttribute">Checkout attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkout attribute value list model
    /// </returns>
    public virtual async Task<CheckoutAttributeValueListModel> PrepareCheckoutAttributeValueListModelAsync(CheckoutAttributeValueSearchModel searchModel,
        CheckoutAttribute checkoutAttribute)
    {
		_logger.LogError(">>> Entering PrepareCheckoutAttributeValueListModelAsync(searchModel={SearchModel},checkoutAttribute={CheckoutAttribute})", searchModel, checkoutAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(checkoutAttribute);

        //get checkout attribute values
        var checkoutAttributeValues = (await _checkoutAttributeService
            .GetAttributeValuesAsync(checkoutAttribute.Id)).ToPagedList(searchModel);

        //prepare list model
        var model = new CheckoutAttributeValueListModel().PrepareToGrid(searchModel, checkoutAttributeValues, () =>
        {
            return checkoutAttributeValues.Select(value =>
            {
                //fill in model values from the entity
                var checkoutAttributeValueModel = value.ToModel<CheckoutAttributeValueModel>();
				_logger.LogError("PrepareCheckoutAttributeValueListModelAsync(searchModel={SearchModel},checkoutAttribute={CheckoutAttribute}): checkoutAttributeValueModel → {CheckoutAttributeValueModel}", searchModel, checkoutAttribute, checkoutAttributeValueModel);

                //fill in additional values (not existing in the entity)
                checkoutAttributeValueModel.Name =
                    checkoutAttribute.AttributeControlType != AttributeControlType.ColorSquares
                        ? value.Name
                        : $"{value.Name} - {value.ColorSquaresRgb}";

                return checkoutAttributeValueModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareCheckoutAttributeValueListModelAsync(searchModel={SearchModel},checkoutAttribute={CheckoutAttribute})", searchModel, checkoutAttribute);
        return model;
    }

    /// <summary>
    /// Prepare checkout attribute value model
    /// </summary>
    /// <param name="model">Checkout attribute value model</param>
    /// <param name="checkoutAttribute">Checkout attribute</param>
    /// <param name="checkoutAttributeValue">Checkout attribute value</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkout attribute value model
    /// </returns>
    public virtual async Task<CheckoutAttributeValueModel> PrepareCheckoutAttributeValueModelAsync(CheckoutAttributeValueModel model,
        CheckoutAttribute checkoutAttribute, CheckoutAttributeValue checkoutAttributeValue, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareCheckoutAttributeValueModelAsync(model={Model},checkoutAttribute={CheckoutAttribute},checkoutAttributeValue={CheckoutAttributeValue},excludeProperties={ExcludeProperties})", model, checkoutAttribute, checkoutAttributeValue, excludeProperties);
        ArgumentNullException.ThrowIfNull(checkoutAttribute);

        Func<CheckoutAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (checkoutAttributeValue != null)
        {
            //fill in model values from the entity
            model ??= checkoutAttributeValue.ToModel<CheckoutAttributeValueModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(checkoutAttributeValue, entity => entity.Name, languageId, false, false);
            };
        }

        model.AttributeId = checkoutAttribute.Id;
		_logger.LogDebug("PrepareCheckoutAttributeValueModelAsync(model={Model},checkoutAttribute={CheckoutAttribute},checkoutAttributeValue={CheckoutAttributeValue},excludeProperties={ExcludeProperties}): model.AttributeId → {Arg4}", model, checkoutAttribute, checkoutAttributeValue, excludeProperties, model.AttributeId);
        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
        model.BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name;
        model.DisplayColorSquaresRgb = checkoutAttribute.AttributeControlType == AttributeControlType.ColorSquares;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareCheckoutAttributeValueModelAsync(model={Model},checkoutAttribute={CheckoutAttribute},checkoutAttributeValue={CheckoutAttributeValue},excludeProperties={ExcludeProperties})", model, checkoutAttribute, checkoutAttributeValue, excludeProperties);
        return model;
    }

    #endregion
}