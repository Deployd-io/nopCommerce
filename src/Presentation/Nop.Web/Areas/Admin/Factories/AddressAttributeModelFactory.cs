using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the address attribute model factory implementation
/// </summary>
public partial class AddressAttributeModelFactory : IAddressAttributeModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;

    #endregion

    #region Ctor

    public AddressAttributeModelFactory(IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory)
    {
		_logger.LogDebug(">>> Entering AddressAttributeModelFactory(addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory})", addressAttributeParser, addressAttributeService, localizationService, localizedModelFactory);
        _addressAttributeParser = addressAttributeParser;
		_logger.LogDebug("AddressAttributeModelFactory(addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory}): _addressAttributeParser → {_addressAttributeParser}", addressAttributeParser, addressAttributeService, localizationService, localizedModelFactory, _addressAttributeParser);
        _addressAttributeService = addressAttributeService;
		_logger.LogDebug("AddressAttributeModelFactory(addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory}): _addressAttributeService → {_addressAttributeService}", addressAttributeParser, addressAttributeService, localizationService, localizedModelFactory, _addressAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("AddressAttributeModelFactory(addressAttributeParser={AddressAttributeParser},addressAttributeService={AddressAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory}): _localizationService → {_localizationService}", addressAttributeParser, addressAttributeService, localizationService, localizedModelFactory, _localizationService);
        _localizedModelFactory = localizedModelFactory;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare address attribute value search model
    /// </summary>
    /// <param name="searchModel">Address attribute value search model</param>
    /// <param name="addressAttribute">Address attribute</param>
    /// <returns>Address attribute value search model</returns>
    protected virtual AddressAttributeValueSearchModel PrepareAddressAttributeValueSearchModel(AddressAttributeValueSearchModel searchModel, AddressAttribute addressAttribute)
    {
		_logger.LogDebug(">>> Entering PrepareAddressAttributeValueSearchModel(searchModel={SearchModel},addressAttribute={AddressAttribute})", searchModel, addressAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(addressAttribute);

        searchModel.AddressAttributeId = addressAttribute.Id;
		_logger.LogDebug("PrepareAddressAttributeValueSearchModel(searchModel={SearchModel},addressAttribute={AddressAttribute}): searchModel.AddressAttributeId → {Arg2}", searchModel, addressAttribute, searchModel.AddressAttributeId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareAddressAttributeValueSearchModel(searchModel={SearchModel},addressAttribute={AddressAttribute})", searchModel, addressAttribute);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare address attribute search model
    /// </summary>
    /// <param name="searchModel">Address attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address attribute search model
    /// </returns>
    public virtual Task<AddressAttributeSearchModel> PrepareAddressAttributeSearchModelAsync(AddressAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddressAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareAddressAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged address attribute list model
    /// </summary>
    /// <param name="searchModel">Address attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address attribute list model
    /// </returns>
    public virtual async Task<AddressAttributeListModel> PrepareAddressAttributeListModelAsync(AddressAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddressAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get address attributes
        var addressAttributes = (await _addressAttributeService.GetAllAttributesAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareAddressAttributeListModelAsync(searchModel={SearchModel}): addressAttributes → {AddressAttributes}", searchModel, addressAttributes);

        //prepare grid model
        var model = await new AddressAttributeListModel().PrepareToGridAsync(searchModel, addressAttributes, () =>
        {
            return addressAttributes.SelectAwait(async attribute =>
            {
                //fill in model values from the entity
                var attributeModel = attribute.ToModel<AddressAttributeModel>();
				_logger.LogDebug("PrepareAddressAttributeListModelAsync(searchModel={SearchModel}): attributeModel → {AttributeModel}", searchModel, attributeModel);

                //fill in additional values (not existing in the entity)
                attributeModel.AttributeControlTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeControlType);

                return attributeModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareAddressAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare address attribute model
    /// </summary>
    /// <param name="model">Address attribute model</param>
    /// <param name="addressAttribute">Address attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address attribute model
    /// </returns>
    public virtual async Task<AddressAttributeModel> PrepareAddressAttributeModelAsync(AddressAttributeModel model,
        AddressAttribute addressAttribute, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareAddressAttributeModelAsync(model={Model},addressAttribute={AddressAttribute},excludeProperties={ExcludeProperties})", model, addressAttribute, excludeProperties);
        Func<AddressAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (addressAttribute != null)
        {
            //fill in model values from the entity
            model ??= addressAttribute.ToModel<AddressAttributeModel>();

            //prepare nested search model
            PrepareAddressAttributeValueSearchModel(model.AddressAttributeValueSearchModel, addressAttribute);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(addressAttribute, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareAddressAttributeModelAsync(model={Model},addressAttribute={AddressAttribute},excludeProperties={ExcludeProperties})", model, addressAttribute, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged address attribute value list model
    /// </summary>
    /// <param name="searchModel">Address attribute value search model</param>
    /// <param name="addressAttribute">Address attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address attribute value list model
    /// </returns>
    public virtual async Task<AddressAttributeValueListModel> PrepareAddressAttributeValueListModelAsync(AddressAttributeValueSearchModel searchModel,
        AddressAttribute addressAttribute)
    {
		_logger.LogDebug(">>> Entering PrepareAddressAttributeValueListModelAsync(searchModel={SearchModel},addressAttribute={AddressAttribute})", searchModel, addressAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(addressAttribute);

        //get address attribute values
        var addressAttributeValues = (await _addressAttributeService.GetAttributeValuesAsync(addressAttribute.Id)).ToPagedList(searchModel);
		_logger.LogDebug("PrepareAddressAttributeValueListModelAsync(searchModel={SearchModel},addressAttribute={AddressAttribute}): addressAttributeValues → {AddressAttributeValues}", searchModel, addressAttribute, addressAttributeValues);

        //prepare grid model
        var model = new AddressAttributeValueListModel().PrepareToGrid(searchModel, addressAttributeValues, () =>
        {
            //fill in model values from the entity
		_logger.LogDebug("<<< Exiting PrepareAddressAttributeValueListModelAsync(searchModel={SearchModel},addressAttribute={AddressAttribute})", searchModel, addressAttribute);
            return addressAttributeValues.Select(value => value.ToModel<AddressAttributeValueModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare address attribute value model
    /// </summary>
    /// <param name="model">Address attribute value model</param>
    /// <param name="addressAttribute">Address attribute</param>
    /// <param name="addressAttributeValue">Address attribute value</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address attribute value model
    /// </returns>
    public virtual async Task<AddressAttributeValueModel> PrepareAddressAttributeValueModelAsync(AddressAttributeValueModel model,
        AddressAttribute addressAttribute, AddressAttributeValue addressAttributeValue, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareAddressAttributeValueModelAsync(model={Model},addressAttribute={AddressAttribute},addressAttributeValue={AddressAttributeValue},excludeProperties={ExcludeProperties})", model, addressAttribute, addressAttributeValue, excludeProperties);
        ArgumentNullException.ThrowIfNull(addressAttribute);

        Func<AddressAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (addressAttributeValue != null)
        {
            //fill in model values from the entity
            model ??= addressAttributeValue.ToModel<AddressAttributeValueModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(addressAttributeValue, entity => entity.Name, languageId, false, false);
            };
        }

        model.AttributeId = addressAttribute.Id;
		_logger.LogDebug("PrepareAddressAttributeValueModelAsync(model={Model},addressAttribute={AddressAttribute},addressAttributeValue={AddressAttributeValue},excludeProperties={ExcludeProperties}): model.AttributeId → {Arg4}", model, addressAttribute, addressAttributeValue, excludeProperties, model.AttributeId);

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareAddressAttributeValueModelAsync(model={Model},addressAttribute={AddressAttribute},addressAttributeValue={AddressAttributeValue},excludeProperties={ExcludeProperties})", model, addressAttribute, addressAttributeValue, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare custom address attributes
    /// </summary>
    /// <param name="models">List of address attribute models</param>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareCustomAddressAttributesAsync(IList<AddressModel.AddressAttributeModel> models, Address address)
    {
		_logger.LogDebug(">>> Entering PrepareCustomAddressAttributesAsync(models={Models},address={Address})", models, address);
        ArgumentNullException.ThrowIfNull(models);

        var attributes = await _addressAttributeService.GetAllAttributesAsync();
		_logger.LogDebug("PrepareCustomAddressAttributesAsync(models={Models},address={Address}): attributes → {Attributes}", models, address, attributes);
        foreach (var attribute in attributes)
        {
            var attributeModel = new AddressModel.AddressAttributeModel
            {
                Id = attribute.Id,
                Name = attribute.Name,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _addressAttributeService.GetAttributeValuesAsync(attribute.Id);
				_logger.LogDebug("PrepareCustomAddressAttributesAsync(models={Models},address={Address}): attributeValues → {AttributeValues}", models, address, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new AddressModel.AddressAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(attributeValueModel);
                }
            }

            //set already selected attributes
            var selectedAddressAttributes = address?.CustomAttributes;
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                {
                    if (!string.IsNullOrEmpty(selectedAddressAttributes))
                    {
                        //clear default selection
                        foreach (var item in attributeModel.Values)
                            item.IsPreSelected = false;
							_logger.LogDebug("PrepareCustomAddressAttributesAsync(models={Models},address={Address}): item.IsPreSelected → {Arg2}", models, address, item.IsPreSelected);

                        //select new values
                        var selectedValues = await _addressAttributeParser.ParseAttributeValuesAsync(selectedAddressAttributes);
						_logger.LogDebug("PrepareCustomAddressAttributesAsync(models={Models},address={Address}): selectedValues → {SelectedValues}", models, address, selectedValues);
                        foreach (var attributeValue in selectedValues)
                        foreach (var item in attributeModel.Values)
                            if (attributeValue.Id == item.Id)
                                item.IsPreSelected = true;
                    }
                }
                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //do nothing
                    //values are already pre-set
                }
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    if (!string.IsNullOrEmpty(selectedAddressAttributes))
                    {
                        var enteredText = _addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
						_logger.LogDebug("PrepareCustomAddressAttributesAsync(models={Models},address={Address}): enteredText → {EnteredText}", models, address, enteredText);
                        if (enteredText.Any())
                            attributeModel.DefaultValue = enteredText[0];
							_logger.LogDebug("PrepareCustomAddressAttributesAsync(models={Models},address={Address}): attributeModel.DefaultValue → {Arg2}", models, address, attributeModel.DefaultValue);
                    }
                }
                    break;
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.Datepicker:
                case AttributeControlType.FileUpload:
                default:
                    //not supported attribute control types
                    break;
            }

            models.Add(attributeModel);
        }
		_logger.LogDebug("<<< Exiting PrepareCustomAddressAttributesAsync(models={Models},address={Address})", models, address);
    }

    #endregion
}