using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Shipping.Pickup;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the shipping model factory implementation
/// </summary>
public partial class ShippingModelFactory : IShippingModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly IShippingPluginManager _shippingPluginManager;
    protected readonly IShippingMethodsService _shippingMethodsService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IWarehouseService _warehouseService;

    #endregion

    #region Ctor

    public ShippingModelFactory(IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        ICountryService countryService,
        IDateRangeService dateRangeService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IPickupPluginManager pickupPluginManager,
        IShippingPluginManager shippingPluginManager,
        IShippingMethodsService shippingMethodsService,
        IStateProvinceService stateProvinceService,
        IWarehouseService warehouseService)
    {
		_logger.LogError(">>> Entering ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService})", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService);
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _addressService → {_addressService}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _addressService);
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _addressModelFactory → {_addressModelFactory}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _addressModelFactory);
    _logger.LogWarning("<<< Exiting ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService})", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService);
        _countryService = countryService;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _countryService → {_countryService}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _countryService);
        _dateRangeService = dateRangeService;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _dateRangeService → {_dateRangeService}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _dateRangeService);
        _localizationService = localizationService;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _localizationService → {_localizationService}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _localizedModelFactory → {_localizedModelFactory}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _localizedModelFactory);
        _pickupPluginManager = pickupPluginManager;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _pickupPluginManager → {_pickupPluginManager}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _pickupPluginManager);
        _shippingPluginManager = shippingPluginManager;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _shippingPluginManager → {_shippingPluginManager}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _shippingPluginManager);
        _shippingMethodsService = shippingMethodsService;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _shippingMethodsService → {_shippingMethodsService}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _shippingMethodsService);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("ShippingModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},countryService={CountryService},dateRangeService={DateRangeService},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},pickupPluginManager={PickupPluginManager},shippingPluginManager=<redacted>,shippingMethodsService=<redacted>,stateProvinceService={StateProvinceService},warehouseService={WarehouseService}): _stateProvinceService → {_stateProvinceService}", addressModelFactory, addressService, countryService, dateRangeService, localizationService, localizedModelFactory, pickupPluginManager, stateProvinceService, warehouseService, _stateProvinceService);
        _warehouseService = warehouseService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare delivery date search model
    /// </summary>
    /// <param name="searchModel">Delivery date search model</param>
    /// <returns>Delivery date search model</returns>
    protected virtual DeliveryDateSearchModel PrepareDeliveryDateSearchModel(DeliveryDateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareDeliveryDateSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareDeliveryDateSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare product availability range search model
    /// </summary>
    /// <param name="searchModel">Product availability range search model</param>
    /// <returns>Product availability range search model</returns>
    protected virtual ProductAvailabilityRangeSearchModel PrepareProductAvailabilityRangeSearchModel(ProductAvailabilityRangeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareProductAvailabilityRangeSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareProductAvailabilityRangeSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare shipping provider search model
    /// </summary>
    /// <param name="searchModel">Shipping provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping provider search model
    /// </returns>
    public virtual Task<ShippingProviderSearchModel> PrepareShippingProviderSearchModelAsync(ShippingProviderSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareShippingProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareShippingProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged shipping provider list model
    /// </summary>
    /// <param name="searchModel">Shipping provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping provider list model
    /// </returns>
    public virtual async Task<ShippingProviderListModel> PrepareShippingProviderListModelAsync(ShippingProviderSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareShippingProviderListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get shipping providers
        var shippingProviders = (await _shippingPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareShippingProviderListModelAsync(searchModel={SearchModel}): shippingProviders → {ShippingProviders}", searchModel, shippingProviders);

        //prepare grid model
        var model = await new ShippingProviderListModel().PrepareToGridAsync(searchModel, shippingProviders, () =>
        {
            return shippingProviders.SelectAwait(async provider =>
            {
                //fill in model values from the entity
                var shippingProviderModel = provider.ToPluginModel<ShippingProviderModel>();
				_logger.LogError("PrepareShippingProviderListModelAsync(searchModel={SearchModel}): shippingProviderModel → {ShippingProviderModel}", searchModel, shippingProviderModel);

                //fill in additional values (not existing in the entity)
                shippingProviderModel.IsActive = _shippingPluginManager.IsPluginActive(provider);
                shippingProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();

                shippingProviderModel.LogoUrl = await _shippingPluginManager.GetPluginLogoUrlAsync(provider);

                return shippingProviderModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareShippingProviderListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare pickup point provider search model
    /// </summary>
    /// <param name="searchModel">Pickup point provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pickup point provider search model
    /// </returns>
    public virtual Task<PickupPointProviderSearchModel> PreparePickupPointProviderSearchModelAsync(PickupPointProviderSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PreparePickupPointProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PreparePickupPointProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged pickup point provider list model
    /// </summary>
    /// <param name="searchModel">Pickup point provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pickup point provider list model
    /// </returns>
    public virtual async Task<PickupPointProviderListModel> PreparePickupPointProviderListModelAsync(PickupPointProviderSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PreparePickupPointProviderListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get pickup point providers
        var pickupPointProviders = (await _pickupPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);
		_logger.LogError("PreparePickupPointProviderListModelAsync(searchModel={SearchModel}): pickupPointProviders → {PickupPointProviders}", searchModel, pickupPointProviders);

        //prepare grid model
        var model = await new PickupPointProviderListModel().PrepareToGridAsync(searchModel, pickupPointProviders, () =>
        {
            return pickupPointProviders.SelectAwait(async provider =>
            {
                //fill in model values from the entity
                var pickupPointProviderModel = provider.ToPluginModel<PickupPointProviderModel>();
				_logger.LogError("PreparePickupPointProviderListModelAsync(searchModel={SearchModel}): pickupPointProviderModel → {PickupPointProviderModel}", searchModel, pickupPointProviderModel);

                //fill in additional values (not existing in the entity)
                pickupPointProviderModel.IsActive = _pickupPluginManager.IsPluginActive(provider);
                pickupPointProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();

                pickupPointProviderModel.LogoUrl = await _pickupPluginManager.GetPluginLogoUrlAsync(provider);

                return pickupPointProviderModel;
            });
        });

		_logger.LogWarning("<<< Exiting PreparePickupPointProviderListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare shipping method search model
    /// </summary>
    /// <param name="searchModel">Shipping method search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping method search model
    /// </returns>
    public virtual Task<ShippingMethodSearchModel> PrepareShippingMethodSearchModelAsync(ShippingMethodSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareShippingMethodSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareShippingMethodSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged shipping method list model
    /// </summary>
    /// <param name="searchModel">Shipping method search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping method list model
    /// </returns>
    public virtual async Task<ShippingMethodListModel> PrepareShippingMethodListModelAsync(ShippingMethodSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareShippingMethodListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get shipping methods
        var shippingMethods = (await _shippingMethodsService.GetAllShippingMethodsAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareShippingMethodListModelAsync(searchModel={SearchModel}): shippingMethods → {ShippingMethods}", searchModel, shippingMethods);

        //prepare grid model
        var model = new ShippingMethodListModel().PrepareToGrid(searchModel, shippingMethods, () =>
        {
		_logger.LogWarning("<<< Exiting PrepareShippingMethodListModelAsync(searchModel={SearchModel})", searchModel);
            return shippingMethods.Select(method => method.ToModel<ShippingMethodModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare shipping method model
    /// </summary>
    /// <param name="model">Shipping method model</param>
    /// <param name="shippingMethod">Shipping method</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping method model
    /// </returns>
    public virtual async Task<ShippingMethodModel> PrepareShippingMethodModelAsync(ShippingMethodModel model,
        ShippingMethod shippingMethod, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareShippingMethodModelAsync(model={Model},shippingMethod=<redacted>,excludeProperties={ExcludeProperties})", model, excludeProperties);
        Func<ShippingMethodLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (shippingMethod != null)
        {
            //fill in model values from the entity
            model ??= shippingMethod.ToModel<ShippingMethodModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(shippingMethod, entity => entity.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(shippingMethod, entity => entity.Description, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareShippingMethodModelAsync(model={Model},shippingMethod=<redacted>,excludeProperties={ExcludeProperties})", model, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare dates and ranges search model
    /// </summary>
    /// <param name="searchModel">Dates and ranges search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dates and ranges search model
    /// </returns>
    public virtual Task<DatesRangesSearchModel> PrepareDatesRangesSearchModelAsync(DatesRangesSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareDatesRangesSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare nested search models
        PrepareDeliveryDateSearchModel(searchModel.DeliveryDateSearchModel);
        PrepareProductAvailabilityRangeSearchModel(searchModel.ProductAvailabilityRangeSearchModel);

		_logger.LogWarning("<<< Exiting PrepareDatesRangesSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged delivery date list model
    /// </summary>
    /// <param name="searchModel">Delivery date search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the delivery date list model
    /// </returns>
    public virtual async Task<DeliveryDateListModel> PrepareDeliveryDateListModelAsync(DeliveryDateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareDeliveryDateListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get delivery dates
        var deliveryDates = (await _dateRangeService.GetAllDeliveryDatesAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareDeliveryDateListModelAsync(searchModel={SearchModel}): deliveryDates → {DeliveryDates}", searchModel, deliveryDates);

        //prepare grid model
        var model = new DeliveryDateListModel().PrepareToGrid(searchModel, deliveryDates, () =>
        {
            //fill in model values from the entity
		_logger.LogWarning("<<< Exiting PrepareDeliveryDateListModelAsync(searchModel={SearchModel})", searchModel);
            return deliveryDates.Select(date => date.ToModel<DeliveryDateModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare delivery date model
    /// </summary>
    /// <param name="model">Delivery date model</param>
    /// <param name="deliveryDate">Delivery date</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the delivery date model
    /// </returns>
    public virtual async Task<DeliveryDateModel> PrepareDeliveryDateModelAsync(DeliveryDateModel model, DeliveryDate deliveryDate, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareDeliveryDateModelAsync(model={Model},deliveryDate={DeliveryDate},excludeProperties={ExcludeProperties})", model, deliveryDate, excludeProperties);
        Func<DeliveryDateLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (deliveryDate != null)
        {
            //fill in model values from the entity
            model ??= deliveryDate.ToModel<DeliveryDateModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(deliveryDate, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareDeliveryDateModelAsync(model={Model},deliveryDate={DeliveryDate},excludeProperties={ExcludeProperties})", model, deliveryDate, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged product availability range list model
    /// </summary>
    /// <param name="searchModel">Product availability range search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product availability range list model
    /// </returns>
    public virtual async Task<ProductAvailabilityRangeListModel> PrepareProductAvailabilityRangeListModelAsync(ProductAvailabilityRangeSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareProductAvailabilityRangeListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get product availability ranges
        var productAvailabilityRanges = (await _dateRangeService.GetAllProductAvailabilityRangesAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareProductAvailabilityRangeListModelAsync(searchModel={SearchModel}): productAvailabilityRanges → {ProductAvailabilityRanges}", searchModel, productAvailabilityRanges);

        //prepare grid model
        var model = new ProductAvailabilityRangeListModel().PrepareToGrid(searchModel, productAvailabilityRanges, () =>
        {
            //fill in model values from the entity
		_logger.LogWarning("<<< Exiting PrepareProductAvailabilityRangeListModelAsync(searchModel={SearchModel})", searchModel);
            return productAvailabilityRanges.Select(range => range.ToModel<ProductAvailabilityRangeModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare product availability range model
    /// </summary>
    /// <param name="model">Product availability range model</param>
    /// <param name="productAvailabilityRange">Product availability range</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product availability range model
    /// </returns>
    public virtual async Task<ProductAvailabilityRangeModel> PrepareProductAvailabilityRangeModelAsync(ProductAvailabilityRangeModel model,
        ProductAvailabilityRange productAvailabilityRange, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareProductAvailabilityRangeModelAsync(model={Model},productAvailabilityRange={ProductAvailabilityRange},excludeProperties={ExcludeProperties})", model, productAvailabilityRange, excludeProperties);
        Func<ProductAvailabilityRangeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (productAvailabilityRange != null)
        {
            //fill in model values from the entity
            model ??= productAvailabilityRange.ToModel<ProductAvailabilityRangeModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(productAvailabilityRange, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareProductAvailabilityRangeModelAsync(model={Model},productAvailabilityRange={ProductAvailabilityRange},excludeProperties={ExcludeProperties})", model, productAvailabilityRange, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare warehouse search model
    /// </summary>
    /// <param name="searchModel">Warehouse search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouse search model
    /// </returns>
    public virtual Task<WarehouseSearchModel> PrepareWarehouseSearchModelAsync(WarehouseSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareWarehouseSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareWarehouseSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged warehouse list model
    /// </summary>
    /// <param name="searchModel">Warehouse search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouse list model
    /// </returns>
    public virtual async Task<WarehouseListModel> PrepareWarehouseListModelAsync(WarehouseSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareWarehouseListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get warehouses
        var warehouses = (await _warehouseService.GetAllWarehousesAsync(
                name: searchModel.SearchName))
            .ToPagedList(searchModel);

        //prepare list model
        var model = new WarehouseListModel().PrepareToGrid(searchModel, warehouses, () =>
        {
            //fill in model values from the entity
		_logger.LogWarning("<<< Exiting PrepareWarehouseListModelAsync(searchModel={SearchModel})", searchModel);
            return warehouses.Select(warehouse => warehouse.ToModel<WarehouseModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare warehouse model
    /// </summary>
    /// <param name="model">Warehouse model</param>
    /// <param name="warehouse">Warehouse</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouse model
    /// </returns>
    public virtual async Task<WarehouseModel> PrepareWarehouseModelAsync(WarehouseModel model, Warehouse warehouse, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareWarehouseModelAsync(model={Model},warehouse={Warehouse},excludeProperties={ExcludeProperties})", model, warehouse, excludeProperties);
        if (warehouse != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = warehouse.ToModel<WarehouseModel>();
				_logger.LogError("PrepareWarehouseModelAsync(model={Model},warehouse={Warehouse},excludeProperties={ExcludeProperties}): model → {Model1}", model, warehouse, excludeProperties, model);
            }
        }

        //prepare address model
        var address = await _addressService.GetAddressByIdAsync(warehouse?.AddressId ?? 0);
		_logger.LogError("PrepareWarehouseModelAsync(model={Model},warehouse={Warehouse},excludeProperties={ExcludeProperties}): address → {Address}", model, warehouse, excludeProperties, address);
        if (!excludeProperties && address != null)
            model.Address = address.ToModel(model.Address);
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        model.Address.CountryRequired = true;
        model.Address.ZipPostalCodeRequired = true;

		_logger.LogWarning("<<< Exiting PrepareWarehouseModelAsync(model={Model},warehouse={Warehouse},excludeProperties={ExcludeProperties})", model, warehouse, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare shipping method restriction model
    /// </summary>
    /// <param name="model">Shipping method restriction model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping method restriction model
    /// </returns>
    public virtual async Task<ShippingMethodRestrictionModel> PrepareShippingMethodRestrictionModelAsync(ShippingMethodRestrictionModel model)
    {
		_logger.LogError(">>> Entering PrepareShippingMethodRestrictionModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        model.AvailableCountries = await countries.SelectAwait(async country =>
        {
			_logger.LogError("PrepareShippingMethodRestrictionModelAsync(model={Model}): countries → {Countries}", model, countries);
            var countryModel = country.ToModel<CountryModel>();
			_logger.LogError("PrepareShippingMethodRestrictionModelAsync(model={Model}): countryModel → {CountryModel}", model, countryModel);
            countryModel.NumberOfStates = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id))?.Count ?? 0;

            return countryModel;
        }).ToListAsync();
		_logger.LogError("PrepareShippingMethodRestrictionModelAsync(model)={Model}: database query executed in {Arg1} ms", model, __logStart.ElapsedMilliseconds);

        foreach (var shippingMethod in await _shippingMethodsService.GetAllShippingMethodsAsync())
        {
            model.AvailableShippingMethods.Add(shippingMethod.ToModel<ShippingMethodModel>());
            foreach (var country in countries)
            {
                if (!model.Restricted.ContainsKey(country.Id))
                    model.Restricted[country.Id] = new Dictionary<int, bool>();

                model.Restricted[country.Id][shippingMethod.Id] = await _shippingMethodsService.CountryRestrictionExistsAsync(shippingMethod, country.Id);
            }
        }

		_logger.LogWarning("<<< Exiting PrepareShippingMethodRestrictionModelAsync(model={Model})", model);
        return model;
    }

    #endregion
}