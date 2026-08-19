using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the vendor attribute model factory implementation
/// </summary>
public partial class VendorAttributeModelFactory : IVendorAttributeModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;

    #endregion

    #region Ctor

    public VendorAttributeModelFactory(IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory)
    {
		_logger.LogDebug(">>> Entering VendorAttributeModelFactory(vendorAttributeService={VendorAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory})", vendorAttributeService, localizationService, localizedModelFactory);
        _vendorAttributeService = vendorAttributeService;
        _localizationService = localizationService;
		_logger.LogDebug("VendorAttributeModelFactory(vendorAttributeService={VendorAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory}): _localizationService → {_localizationService}", vendorAttributeService, localizationService, localizedModelFactory, _localizationService);
		_logger.LogDebug("VendorAttributeModelFactory(vendorAttributeService={VendorAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory}): _vendorAttributeService → {_vendorAttributeService}", vendorAttributeService, localizationService, localizedModelFactory, _vendorAttributeService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("VendorAttributeModelFactory(vendorAttributeService={VendorAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory}): _localizedModelFactory → {_localizedModelFactory}", vendorAttributeService, localizationService, localizedModelFactory, _localizedModelFactory);
		_logger.LogDebug("<<< Exiting VendorAttributeModelFactory(vendorAttributeService={VendorAttributeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory})", vendorAttributeService, localizationService, localizedModelFactory);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare vendor attribute value search model
    /// </summary>
    /// <param name="searchModel">Vendor attribute value search model</param>
    /// <param name="vendorAttribute">Vendor attribute</param>
    /// <returns>Vendor attribute value search model</returns>
    protected virtual VendorAttributeValueSearchModel PrepareVendorAttributeValueSearchModel(VendorAttributeValueSearchModel searchModel,
        VendorAttribute vendorAttribute)
    {
		_logger.LogDebug(">>> Entering PrepareVendorAttributeValueSearchModel(searchModel={SearchModel},vendorAttribute={VendorAttribute})", searchModel, vendorAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(vendorAttribute);

        searchModel.VendorAttributeId = vendorAttribute.Id;
		_logger.LogDebug("PrepareVendorAttributeValueSearchModel(searchModel={SearchModel},vendorAttribute={VendorAttribute}): searchModel.VendorAttributeId → {Arg2}", searchModel, vendorAttribute, searchModel.VendorAttributeId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareVendorAttributeValueSearchModel(searchModel={SearchModel},vendorAttribute={VendorAttribute})", searchModel, vendorAttribute);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare vendor attribute search model
    /// </summary>
    /// <param name="searchModel">Vendor attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor attribute search model
    /// </returns>
    public virtual Task<VendorAttributeSearchModel> PrepareVendorAttributeSearchModelAsync(VendorAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareVendorAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareVendorAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged vendor attribute list model
    /// </summary>
    /// <param name="searchModel">Vendor attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor attribute list model
    /// </returns>
    public virtual async Task<VendorAttributeListModel> PrepareVendorAttributeListModelAsync(VendorAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareVendorAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get vendor attributes
        var vendorAttributes = (await _vendorAttributeService.GetAllAttributesAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareVendorAttributeListModelAsync(searchModel={SearchModel}): vendorAttributes → {VendorAttributes}", searchModel, vendorAttributes);

        //prepare list model
        var model = await new VendorAttributeListModel().PrepareToGridAsync(searchModel, vendorAttributes, () =>
        {
            return vendorAttributes.SelectAwait(async attribute =>
            {
                //fill in model values from the entity
                var attributeModel = attribute.ToModel<VendorAttributeModel>();
				_logger.LogDebug("PrepareVendorAttributeListModelAsync(searchModel={SearchModel}): attributeModel → {AttributeModel}", searchModel, attributeModel);

                //fill in additional values (not existing in the entity)
                attributeModel.AttributeControlTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeControlType);

                return attributeModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareVendorAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare vendor attribute model
    /// </summary>
    /// <param name="model">Vendor attribute model</param>
    /// <param name="vendorAttribute">Vendor attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor attribute model
    /// </returns>
    public virtual async Task<VendorAttributeModel> PrepareVendorAttributeModelAsync(VendorAttributeModel model,
        VendorAttribute vendorAttribute, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareVendorAttributeModelAsync(model={Model},vendorAttribute={VendorAttribute},excludeProperties={ExcludeProperties})", model, vendorAttribute, excludeProperties);
        Func<VendorAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (vendorAttribute != null)
        {
            //fill in model values from the entity
            model ??= vendorAttribute.ToModel<VendorAttributeModel>();

            //prepare nested search model
            PrepareVendorAttributeValueSearchModel(model.VendorAttributeValueSearchModel, vendorAttribute);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(vendorAttribute, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareVendorAttributeModelAsync(model={Model},vendorAttribute={VendorAttribute},excludeProperties={ExcludeProperties})", model, vendorAttribute, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged vendor attribute value list model
    /// </summary>
    /// <param name="searchModel">Vendor attribute value search model</param>
    /// <param name="vendorAttribute">Vendor attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor attribute value list model
    /// </returns>
    public virtual async Task<VendorAttributeValueListModel> PrepareVendorAttributeValueListModelAsync(VendorAttributeValueSearchModel searchModel,
        VendorAttribute vendorAttribute)
    {
		_logger.LogDebug(">>> Entering PrepareVendorAttributeValueListModelAsync(searchModel={SearchModel},vendorAttribute={VendorAttribute})", searchModel, vendorAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(vendorAttribute);

        //get vendor attribute values
        var vendorAttributeValues = (await _vendorAttributeService.GetAttributeValuesAsync(vendorAttribute.Id)).ToPagedList(searchModel);
		_logger.LogDebug("PrepareVendorAttributeValueListModelAsync(searchModel={SearchModel},vendorAttribute={VendorAttribute}): vendorAttributeValues → {VendorAttributeValues}", searchModel, vendorAttribute, vendorAttributeValues);

        //prepare list model
        var model = new VendorAttributeValueListModel().PrepareToGrid(searchModel, vendorAttributeValues, () =>
        {
            //fill in model values from the entity
		_logger.LogDebug("<<< Exiting PrepareVendorAttributeValueListModelAsync(searchModel={SearchModel},vendorAttribute={VendorAttribute})", searchModel, vendorAttribute);
            return vendorAttributeValues.Select(value => value.ToModel<VendorAttributeValueModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare vendor attribute value model
    /// </summary>
    /// <param name="model">Vendor attribute value model</param>
    /// <param name="vendorAttribute">Vendor attribute</param>
    /// <param name="vendorAttributeValue">Vendor attribute value</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor attribute value model
    /// </returns>
    public virtual async Task<VendorAttributeValueModel> PrepareVendorAttributeValueModelAsync(VendorAttributeValueModel model,
        VendorAttribute vendorAttribute, VendorAttributeValue vendorAttributeValue, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareVendorAttributeValueModelAsync(model={Model},vendorAttribute={VendorAttribute},vendorAttributeValue={VendorAttributeValue},excludeProperties={ExcludeProperties})", model, vendorAttribute, vendorAttributeValue, excludeProperties);
        ArgumentNullException.ThrowIfNull(vendorAttribute);

        Func<VendorAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (vendorAttributeValue != null)
        {
            //fill in model values from the entity
            model ??= vendorAttributeValue.ToModel<VendorAttributeValueModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(vendorAttributeValue, entity => entity.Name, languageId, false, false);
            };
        }

        model.AttributeId = vendorAttribute.Id;
		_logger.LogDebug("PrepareVendorAttributeValueModelAsync(model={Model},vendorAttribute={VendorAttribute},vendorAttributeValue={VendorAttributeValue},excludeProperties={ExcludeProperties}): model.AttributeId → {Arg4}", model, vendorAttribute, vendorAttributeValue, excludeProperties, model.AttributeId);

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareVendorAttributeValueModelAsync(model={Model},vendorAttribute={VendorAttribute},vendorAttributeValue={VendorAttributeValue},excludeProperties={ExcludeProperties})", model, vendorAttribute, vendorAttributeValue, excludeProperties);
        return model;
    }

    #endregion
}