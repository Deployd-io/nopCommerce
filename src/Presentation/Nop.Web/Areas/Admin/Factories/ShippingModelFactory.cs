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

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the shipping model factory implementation
/// </summary>
public partial class ShippingModelFactory : IShippingModelFactory
{
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
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _countryService = countryService;
        _dateRangeService = dateRangeService;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _pickupPluginManager = pickupPluginManager;
        _shippingPluginManager = shippingPluginManager;
        _shippingMethodsService = shippingMethodsService;
        _stateProvinceService = stateProvinceService;
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
		logger.LogDebug(">>> Entering PrepareDeliveryDateSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareDeliveryDateSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare product availability range search model
    /// </summary>
    /// <param name="searchModel">Product availability range search model</param>
    /// <returns>Product availability range search model</returns>
    protected virtual ProductAvailabilityRangeSearchModel PrepareProductAvailabilityRangeSearchModel(ProductAvailabilityRangeSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PrepareProductAvailabilityRangeSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareProductAvailabilityRangeSearchModel(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareShippingProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareShippingProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareShippingProviderListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get shipping providers
        var shippingProviders = (await _shippingPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

		logger.LogDebug("PrepareShippingProviderListModelAsync(searchModel={SearchModel}): shippingProviders → {ShippingProviders}", searchModel, shippingProviders);
        //prepare grid model
        var model = await new ShippingProviderListModel().PrepareToGridAsync(searchModel, shippingProviders, () =>
        {
            return shippingProviders.SelectAwait(async provider =>
            {
                //fill in model values from the entity
                var shippingProviderModel = provider.ToPluginModel<ShippingProviderModel>();

				logger.LogDebug("PrepareShippingProviderListModelAsync(searchModel={SearchModel}): shippingProviderModel → {ShippingProviderModel}", searchModel, shippingProviderModel);
                //fill in additional values (not existing in the entity)
                shippingProviderModel.IsActive = _shippingPluginManager.IsPluginActive(provider);
                shippingProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();

                shippingProviderModel.LogoUrl = await _shippingPluginManager.GetPluginLogoUrlAsync(provider);

                return shippingProviderModel;
            });
        });

		logger.LogDebug("<<< Exiting PrepareShippingProviderListModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PreparePickupPointProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PreparePickupPointProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PreparePickupPointProviderListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get pickup point providers
        var pickupPointProviders = (await _pickupPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

		logger.LogDebug("PreparePickupPointProviderListModelAsync(searchModel={SearchModel}): pickupPointProviders → {PickupPointProviders}", searchModel, pickupPointProviders);
        //prepare grid model
        var model = await new PickupPointProviderListModel().PrepareToGridAsync(searchModel, pickupPointProviders, () =>
        {
            return pickupPointProviders.SelectAwait(async provider =>
            {
                //fill in model values from the entity
                var pickupPointProviderModel = provider.ToPluginModel<PickupPointProviderModel>();

				logger.LogDebug("PreparePickupPointProviderListModelAsync(searchModel={SearchModel}): pickupPointProviderModel → {PickupPointProviderModel}", searchModel, pickupPointProviderModel);
                //fill in additional values (not existing in the entity)
                pickupPointProviderModel.IsActive = _pickupPluginManager.IsPluginActive(provider);
                pickupPointProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();

                pickupPointProviderModel.LogoUrl = await _pickupPluginManager.GetPluginLogoUrlAsync(provider);

                return pickupPointProviderModel;
            });
        });

		logger.LogDebug("<<< Exiting PreparePickupPointProviderListModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareShippingMethodSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareShippingMethodSearchModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareShippingMethodListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get shipping methods
        var shippingMethods = (await _shippingMethodsService.GetAllShippingMethodsAsync()).ToPagedList(searchModel);

		logger.LogDebug("PrepareShippingMethodListModelAsync(searchModel={SearchModel}): shippingMethods → {ShippingMethods}", searchModel, shippingMethods);
        //prepare grid model
        var model = new ShippingMethodListModel().PrepareToGrid(searchModel, shippingMethods, () =>
        {
            return shippingMethods.Select(method => method.ToModel<ShippingMethodModel>());
        });

		logger.LogDebug("<<< Exiting PrepareShippingMethodListModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareDatesRangesSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare nested search models
        PrepareDeliveryDateSearchModel(searchModel.DeliveryDateSearchModel);
        PrepareProductAvailabilityRangeSearchModel(searchModel.ProductAvailabilityRangeSearchModel);

		logger.LogDebug("<<< Exiting PrepareDatesRangesSearchModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareDeliveryDateListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get delivery dates
        var deliveryDates = (await _dateRangeService.GetAllDeliveryDatesAsync()).ToPagedList(searchModel);

		logger.LogDebug("PrepareDeliveryDateListModelAsync(searchModel={SearchModel}): deliveryDates → {DeliveryDates}", searchModel, deliveryDates);
        //prepare grid model
        var model = new DeliveryDateListModel().PrepareToGrid(searchModel, deliveryDates, () =>
        {
            //fill in model values from the entity
            return deliveryDates.Select(date => date.ToModel<DeliveryDateModel>());
        });

		logger.LogDebug("<<< Exiting PrepareDeliveryDateListModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareProductAvailabilityRangeListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get product availability ranges
        var productAvailabilityRanges = (await _dateRangeService.GetAllProductAvailabilityRangesAsync()).ToPagedList(searchModel);

		logger.LogDebug("PrepareProductAvailabilityRangeListModelAsync(searchModel={SearchModel}): productAvailabilityRanges → {ProductAvailabilityRanges}", searchModel, productAvailabilityRanges);
        //prepare grid model
        var model = new ProductAvailabilityRangeListModel().PrepareToGrid(searchModel, productAvailabilityRanges, () =>
        {
            //fill in model values from the entity
            return productAvailabilityRanges.Select(range => range.ToModel<ProductAvailabilityRangeModel>());
        });

		logger.LogDebug("<<< Exiting PrepareProductAvailabilityRangeListModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareWarehouseSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareWarehouseSearchModelAsync(searchModel={SearchModel})", searchModel);
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
		logger.LogDebug(">>> Entering PrepareWarehouseListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get warehouses
        var warehouses = (await _warehouseService.GetAllWarehousesAsync(
                name: searchModel.SearchName))
            .ToPagedList(searchModel);

        //prepare list model
        var model = new WarehouseListModel().PrepareToGrid(searchModel, warehouses, () =>
        {
            //fill in model values from the entity
            return warehouses.Select(warehouse => warehouse.ToModel<WarehouseModel>());
        });

		logger.LogDebug("<<< Exiting PrepareWarehouseListModelAsync(searchModel={SearchModel})", searchModel);
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
        if (warehouse != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = warehouse.ToModel<WarehouseModel>();
            }
			logger.LogDebug("PrepareWarehouseModelAsync(model={Model},warehouse={Warehouse},excludeProperties={ExcludeProperties}): model → {Model1}", model, warehouse, excludeProperties, model);
        }

        //prepare address model
        var address = await _addressService.GetAddressByIdAsync(warehouse?.AddressId ?? 0);
        if (!excludeProperties && address != null)
			logger.LogDebug("PrepareWarehouseModelAsync(model={Model},warehouse={Warehouse},excludeProperties={ExcludeProperties}): address → {Address}", model, warehouse, excludeProperties, address);
            model.Address = address.ToModel(model.Address);
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        model.Address.CountryRequired = true;
        model.Address.ZipPostalCodeRequired = true;

		logger.LogDebug("<<< Exiting PrepareWarehouseModelAsync(model={Model},warehouse={Warehouse},excludeProperties={ExcludeProperties})", model, warehouse, excludeProperties);
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(model);

        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
        model.AvailableCountries = await countries.SelectAwait(async country =>
        {
            var countryModel = country.ToModel<CountryModel>();
            countryModel.NumberOfStates = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id))?.Count ?? 0;

            return countryModel;
        }).ToListAsync();

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

        return model;
    }

    #endregion
}
