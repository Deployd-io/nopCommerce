using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the payment method model factory implementation
/// </summary>
public partial class PaymentModelFactory : IPaymentModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICountryService _countryService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IStateProvinceService _stateProvinceService;

    #endregion

    #region Ctor

    public PaymentModelFactory(ICountryService countryService,
        ILocalizationService localizationService,
        IPaymentPluginManager paymentPluginManager,
        IStateProvinceService stateProvinceService)
    {
		_logger.LogDebug(">>> Entering PaymentModelFactory(countryService={CountryService},localizationService={LocalizationService},paymentPluginManager={PaymentPluginManager},stateProvinceService={StateProvinceService})", countryService, localizationService, paymentPluginManager, stateProvinceService);
        _countryService = countryService;
        _localizationService = localizationService;
		_logger.LogDebug("PaymentModelFactory(countryService={CountryService},localizationService={LocalizationService},paymentPluginManager={PaymentPluginManager},stateProvinceService={StateProvinceService}): _localizationService → {_localizationService}", countryService, localizationService, paymentPluginManager, stateProvinceService, _localizationService);
		_logger.LogDebug("PaymentModelFactory(countryService={CountryService},localizationService={LocalizationService},paymentPluginManager={PaymentPluginManager},stateProvinceService={StateProvinceService}): _countryService → {_countryService}", countryService, localizationService, paymentPluginManager, stateProvinceService, _countryService);
        _paymentPluginManager = paymentPluginManager;
		_logger.LogDebug("PaymentModelFactory(countryService={CountryService},localizationService={LocalizationService},paymentPluginManager={PaymentPluginManager},stateProvinceService={StateProvinceService}): _paymentPluginManager → {_paymentPluginManager}", countryService, localizationService, paymentPluginManager, stateProvinceService, _paymentPluginManager);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("<<< Exiting PaymentModelFactory(countryService={CountryService},localizationService={LocalizationService},paymentPluginManager={PaymentPluginManager},stateProvinceService={StateProvinceService})", countryService, localizationService, paymentPluginManager, stateProvinceService);
		_logger.LogDebug("PaymentModelFactory(countryService={CountryService},localizationService={LocalizationService},paymentPluginManager={PaymentPluginManager},stateProvinceService={StateProvinceService}): _stateProvinceService → {_stateProvinceService}", countryService, localizationService, paymentPluginManager, stateProvinceService, _stateProvinceService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare payment methods model
    /// </summary>
    /// <param name="methodsModel">Payment methods model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment methods model
    /// </returns>
    public virtual async Task<PaymentMethodsModel> PreparePaymentMethodsModelAsync(PaymentMethodsModel methodsModel)
    {
		_logger.LogDebug(">>> Entering PreparePaymentMethodsModelAsync(methodsModel={MethodsModel})", methodsModel);
        ArgumentNullException.ThrowIfNull(methodsModel);

        //prepare nested search models
        await PreparePaymentMethodSearchModelAsync(methodsModel.PaymentsMethod);
        await PreparePaymentMethodRestrictionModelAsync(methodsModel.PaymentMethodRestriction);

		_logger.LogDebug("<<< Exiting PreparePaymentMethodsModelAsync(methodsModel={MethodsModel})", methodsModel);
        return methodsModel;
    }

    /// <summary>
    /// Prepare paged payment method list model
    /// </summary>
    /// <param name="searchModel">Payment method search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment method list model
    /// </returns>
    public virtual async Task<PaymentMethodListModel> PreparePaymentMethodListModelAsync(PaymentMethodSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PreparePaymentMethodListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get payment methods
        var paymentMethods = (await _paymentPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PreparePaymentMethodListModelAsync(searchModel={SearchModel}): paymentMethods → {PaymentMethods}", searchModel, paymentMethods);

        //prepare grid model
        var model = await new PaymentMethodListModel().PrepareToGridAsync(searchModel, paymentMethods, () =>
        {
            return paymentMethods.SelectAwait(async method =>
            {
                //fill in model values from the entity
                var paymentMethodModel = method.ToPluginModel<PaymentMethodModel>();
				_logger.LogDebug("PreparePaymentMethodListModelAsync(searchModel={SearchModel}): paymentMethodModel → {PaymentMethodModel}", searchModel, paymentMethodModel);

                //fill in additional values (not existing in the entity)
                paymentMethodModel.IsActive = _paymentPluginManager.IsPluginActive(method);
                paymentMethodModel.ConfigurationUrl = method.GetConfigurationPageUrl();

                paymentMethodModel.LogoUrl = await _paymentPluginManager.GetPluginLogoUrlAsync(method);
                paymentMethodModel.RecurringPaymentType = await _localizationService.GetLocalizedEnumAsync(method.RecurringPaymentType);

                return paymentMethodModel;
            });
        });

		_logger.LogDebug("<<< Exiting PreparePaymentMethodListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare payment method search model
    /// </summary>
    /// <param name="searchModel">Payment method search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment method search model
    /// </returns>
    public virtual Task<PaymentMethodSearchModel> PreparePaymentMethodSearchModelAsync(PaymentMethodSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PreparePaymentMethodSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PreparePaymentMethodSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare payment method restriction model
    /// </summary>
    /// <param name="model">Payment method restriction model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment method restriction model
    /// </returns>
    public virtual async Task<PaymentMethodRestrictionModel> PreparePaymentMethodRestrictionModelAsync(PaymentMethodRestrictionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
        model.AvailableCountries = await countries.SelectAwait(async country =>
        {
            var countryModel = country.ToModel<CountryModel>();
            countryModel.NumberOfStates = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id))?.Count ?? 0;

            return countryModel;
        }).ToListAsync();

        foreach (var method in await _paymentPluginManager.LoadAllPluginsAsync())
        {
            var paymentMethodModel = method.ToPluginModel<PaymentMethodModel>();
            paymentMethodModel.RecurringPaymentType = await _localizationService.GetLocalizedEnumAsync(method.RecurringPaymentType);

            model.AvailablePaymentMethods.Add(paymentMethodModel);

            var restrictedCountries = await _paymentPluginManager.GetRestrictedCountryIdsAsync(method);
            foreach (var country in countries)
            {
                if (!model.Restricted.ContainsKey(method.PluginDescriptor.SystemName))
                    model.Restricted[method.PluginDescriptor.SystemName] = new Dictionary<int, bool>();

                model.Restricted[method.PluginDescriptor.SystemName][country.Id] = restrictedCountries.Contains(country.Id);
            }
        }

        return model;
    }

    #endregion
}