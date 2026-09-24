using Nop.Core.Domain.Affiliates;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the affiliate model factory implementation
/// </summary>
public partial class AffiliateModelFactory : IAffiliateModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IStateProvinceService _stateProvinceService;

    #endregion

    #region Ctor

    public AffiliateModelFactory(IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAffiliateService affiliateService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IOrderService orderService,
        IPriceFormatter priceFormatter,
        IStateProvinceService stateProvinceService)
    {
		_logger.LogError(">>> Entering AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService})", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService);
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _addressService → {_addressService}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _addressService);
        _affiliateService = affiliateService;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _affiliateService → {_affiliateService}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _affiliateService);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _baseAdminModelFactory → {_baseAdminModelFactory}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _baseAdminModelFactory);
        _countryService = countryService;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _countryService → {_countryService}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _countryService);
        _customerService = customerService;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _customerService → {_customerService}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _dateTimeHelper → {_dateTimeHelper}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _dateTimeHelper);
        _localizationService = localizationService;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _localizationService → {_localizationService}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _localizationService);
        _orderService = orderService;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _orderService → {_orderService}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _orderService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _priceFormatter → {_priceFormatter}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _priceFormatter);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("AffiliateModelFactory(addressModelFactory={AddressModelFactory},addressService={AddressService},affiliateService={AffiliateService},baseAdminModelFactory={BaseAdminModelFactory},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter},stateProvinceService={StateProvinceService}): _stateProvinceService → {_stateProvinceService}", addressModelFactory, addressService, affiliateService, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, orderService, priceFormatter, stateProvinceService, _stateProvinceService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare affiliated order search model
    /// </summary>
    /// <param name="searchModel">Affiliated order search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliated order search model
    /// </returns>
    protected virtual async Task<AffiliatedOrderSearchModel> PrepareAffiliatedOrderSearchModelAsync(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
    {
		_logger.LogError(">>> Entering PrepareAffiliatedOrderSearchModelAsync(searchModel={SearchModel},affiliate={Affiliate})", searchModel, affiliate);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(affiliate);

        searchModel.AffliateId = affiliate.Id;
		_logger.LogDebug("PrepareAffiliatedOrderSearchModelAsync(searchModel={SearchModel},affiliate={Affiliate}): searchModel.AffliateId → {Arg2}", searchModel, affiliate, searchModel.AffliateId);

        //prepare available order, payment and shipping statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
        await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareAffiliatedOrderSearchModelAsync(searchModel={SearchModel},affiliate={Affiliate})", searchModel, affiliate);
        return searchModel;
    }

    /// <summary>
    /// Prepare affiliated customer search model
    /// </summary>
    /// <param name="searchModel">Affiliated customer search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>Affiliated customer search model</returns>
    protected virtual AffiliatedCustomerSearchModel PrepareAffiliatedCustomerSearchModel(AffiliatedCustomerSearchModel searchModel, Affiliate affiliate)
    {
		_logger.LogError(">>> Entering PrepareAffiliatedCustomerSearchModel(searchModel={SearchModel},affiliate={Affiliate})", searchModel, affiliate);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(affiliate);

        searchModel.AffliateId = affiliate.Id;
		_logger.LogDebug("PrepareAffiliatedCustomerSearchModel(searchModel={SearchModel},affiliate={Affiliate}): searchModel.AffliateId → {Arg2}", searchModel, affiliate, searchModel.AffliateId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareAffiliatedCustomerSearchModel(searchModel={SearchModel},affiliate={Affiliate})", searchModel, affiliate);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare affiliate search model
    /// </summary>
    /// <param name="searchModel">Affiliate search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate search model
    /// </returns>
    public virtual Task<AffiliateSearchModel> PrepareAffiliateSearchModelAsync(AffiliateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareAffiliateSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareAffiliateSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged affiliate list model
    /// </summary>
    /// <param name="searchModel">Affiliate search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate list model
    /// </returns>
    public virtual async Task<AffiliateListModel> PrepareAffiliateListModelAsync(AffiliateSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareAffiliateListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get affiliates
        var affiliates = await _affiliateService.GetAllAffiliatesAsync(searchModel.SearchFriendlyUrlName,
            searchModel.SearchFirstName,
            searchModel.SearchLastName,
            searchModel.LoadOnlyWithOrders,
            searchModel.OrdersCreatedFromUtc,
            searchModel.OrdersCreatedToUtc,
            searchModel.Page - 1, searchModel.PageSize, true);

        //prepare list model
        var model = await new AffiliateListModel().PrepareToGridAsync(searchModel, affiliates, () =>
        {
            //fill in model values from the entity
            return affiliates.SelectAwait(async affiliate =>
            {
                var address = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
				_logger.LogError("PrepareAffiliateListModelAsync(searchModel={SearchModel}): address → {Address}", searchModel, address);

                var affiliateModel = affiliate.ToModel<AffiliateModel>();
				_logger.LogError("PrepareAffiliateListModelAsync(searchModel={SearchModel}): affiliateModel → {AffiliateModel}", searchModel, affiliateModel);
                affiliateModel.Address = address.ToModel<AddressModel>();
                affiliateModel.Address.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
                affiliateModel.Address.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

                return affiliateModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareAffiliateListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare affiliate model
    /// </summary>
    /// <param name="model">Affiliate model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate model
    /// </returns>
    public virtual async Task<AffiliateModel> PrepareAffiliateModelAsync(AffiliateModel model, Affiliate affiliate, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareAffiliateModelAsync(model={Model},affiliate={Affiliate},excludeProperties={ExcludeProperties})", model, affiliate, excludeProperties);
        //fill in model values from the entity
        if (affiliate != null)
        {
            model ??= affiliate.ToModel<AffiliateModel>();
            model.Url = await _affiliateService.GenerateUrlAsync(affiliate);

            //prepare nested search models
            await PrepareAffiliatedOrderSearchModelAsync(model.AffiliatedOrderSearchModel, affiliate);
            PrepareAffiliatedCustomerSearchModel(model.AffiliatedCustomerSearchModel, affiliate);

            //whether to fill in some of properties
            if (!excludeProperties)
            {
                model.AdminComment = affiliate.AdminComment;
				_logger.LogDebug("PrepareAffiliateModelAsync(model={Model},affiliate={Affiliate},excludeProperties={ExcludeProperties}): model.AdminComment → {Arg3}", model, affiliate, excludeProperties, model.AdminComment);
				_logger.LogDebug("PrepareAffiliateModelAsync(model={Model},affiliate={Affiliate},excludeProperties={ExcludeProperties}): model.FriendlyUrlName → {Arg3}", model, affiliate, excludeProperties, model.FriendlyUrlName);
				_logger.LogDebug("PrepareAffiliateModelAsync(model={Model},affiliate={Affiliate},excludeProperties={ExcludeProperties}): model.Active → {Arg3}", model, affiliate, excludeProperties, model.Active);
                model.FriendlyUrlName = affiliate.FriendlyUrlName;
                model.Active = affiliate.Active;
            }
        }

        //prepare address model
        var address = await _addressService.GetAddressByIdAsync(affiliate?.AddressId ?? 0);
		_logger.LogError("PrepareAffiliateModelAsync(model={Model},affiliate={Affiliate},excludeProperties={ExcludeProperties}): address → {Address}", model, affiliate, excludeProperties, address);
        if (!excludeProperties && address != null)
            model.Address = address.ToModel(model.Address);
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        model.Address.FirstNameRequired = true;
        model.Address.LastNameRequired = true;
        model.Address.EmailRequired = true;
        model.Address.CountryRequired = true;
        model.Address.CountyRequired = true;
        model.Address.CityRequired = true;
        model.Address.StreetAddressRequired = true;
        model.Address.ZipPostalCodeRequired = true;
        model.Address.PhoneRequired = true;

		_logger.LogWarning("<<< Exiting PrepareAffiliateModelAsync(model={Model},affiliate={Affiliate},excludeProperties={ExcludeProperties})", model, affiliate, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged affiliated order list model
    /// </summary>
    /// <param name="searchModel">Affiliated order search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliated order list model
    /// </returns>
    public virtual async Task<AffiliatedOrderListModel> PrepareAffiliatedOrderListModelAsync(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
    {
		_logger.LogError(">>> Entering PrepareAffiliatedOrderListModelAsync(searchModel={SearchModel},affiliate={Affiliate})", searchModel, affiliate);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(affiliate);

        //get parameters to filter orders
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var orderStatusIds = searchModel.OrderStatusId > 0 ? new List<int> { searchModel.OrderStatusId } : null;
        var paymentStatusIds = searchModel.PaymentStatusId > 0 ? new List<int> { searchModel.PaymentStatusId } : null;
        var shippingStatusIds = searchModel.ShippingStatusId > 0 ? new List<int> { searchModel.ShippingStatusId } : null;

        //get orders
        var orders = await _orderService.SearchOrdersAsync(createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            affiliateId: affiliate.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new AffiliatedOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
        {
            //fill in model values from the entity
            return orders.SelectAwait(async order =>
            {
                var affiliatedOrderModel = order.ToModel<AffiliatedOrderModel>();
				_logger.LogError("PrepareAffiliatedOrderListModelAsync(searchModel={SearchModel},affiliate={Affiliate}): affiliatedOrderModel → {AffiliatedOrderModel}", searchModel, affiliate, affiliatedOrderModel);

                //fill in additional values (not existing in the entity)
                affiliatedOrderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                affiliatedOrderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                affiliatedOrderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                affiliatedOrderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                affiliatedOrderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                return affiliatedOrderModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareAffiliatedOrderListModelAsync(searchModel={SearchModel},affiliate={Affiliate})", searchModel, affiliate);
        return model;
    }

    /// <summary>
    /// Prepare paged affiliated customer list model
    /// </summary>
    /// <param name="searchModel">Affiliated customer search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliated customer list model
    /// </returns>
    public virtual async Task<AffiliatedCustomerListModel> PrepareAffiliatedCustomerListModelAsync(AffiliatedCustomerSearchModel searchModel,
        Affiliate affiliate)
    {
		_logger.LogError(">>> Entering PrepareAffiliatedCustomerListModelAsync(searchModel={SearchModel},affiliate={Affiliate})", searchModel, affiliate);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(affiliate);

        //get customers
        var customers = await _customerService.GetAllCustomersAsync(affiliateId: affiliate.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = new AffiliatedCustomerListModel().PrepareToGrid(searchModel, customers, () =>
        {
            //fill in model values from the entity
            return customers.Select(customer =>
            {
                var affiliatedCustomerModel = customer.ToModel<AffiliatedCustomerModel>();
				_logger.LogError("PrepareAffiliatedCustomerListModelAsync(searchModel={SearchModel},affiliate={Affiliate}): affiliatedCustomerModel → {AffiliatedCustomerModel}", searchModel, affiliate, affiliatedCustomerModel);
				_logger.LogDebug("PrepareAffiliatedCustomerListModelAsync(searchModel={SearchModel},affiliate={Affiliate}): affiliatedCustomerModel.Name → {Arg2}", searchModel, affiliate, affiliatedCustomerModel.Name);
                affiliatedCustomerModel.Name = customer.Email;

                return affiliatedCustomerModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareAffiliatedCustomerListModelAsync(searchModel={SearchModel},affiliate={Affiliate})", searchModel, affiliate);
        return model;
    }

    #endregion
}