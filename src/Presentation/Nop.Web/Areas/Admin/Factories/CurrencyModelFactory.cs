using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the currency model factory implementation
/// </summary>
public partial class CurrencyModelFactory : ICurrencyModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExchangeRatePluginManager _exchangeRatePluginManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

    #endregion

    #region Ctor

    public CurrencyModelFactory(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        IExchangeRatePluginManager exchangeRatePluginManager,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
    {
		_logger.LogError(">>> Entering CurrencyModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},exchangeRatePluginManager={ExchangeRatePluginManager},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeMappingSupportedModelFactory=<redacted>)", currencySettings, currencyService, dateTimeHelper, exchangeRatePluginManager, localizationService, localizedModelFactory);
        _currencySettings = currencySettings;
        _currencyService = currencyService;
		_logger.LogDebug("CurrencyModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},exchangeRatePluginManager={ExchangeRatePluginManager},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeMappingSupportedModelFactory=<redacted>): _currencyService → {_currencyService}", currencySettings, currencyService, dateTimeHelper, exchangeRatePluginManager, localizationService, localizedModelFactory, _currencyService);
		_logger.LogDebug("CurrencyModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},exchangeRatePluginManager={ExchangeRatePluginManager},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeMappingSupportedModelFactory=<redacted>): _currencySettings → {_currencySettings}", currencySettings, currencyService, dateTimeHelper, exchangeRatePluginManager, localizationService, localizedModelFactory, _currencySettings);
    _logger.LogWarning("<<< Exiting CurrencyModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},exchangeRatePluginManager={ExchangeRatePluginManager},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeMappingSupportedModelFactory=<redacted>)", currencySettings, currencyService, dateTimeHelper, exchangeRatePluginManager, localizationService, localizedModelFactory);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("CurrencyModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},exchangeRatePluginManager={ExchangeRatePluginManager},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeMappingSupportedModelFactory=<redacted>): _dateTimeHelper → {_dateTimeHelper}", currencySettings, currencyService, dateTimeHelper, exchangeRatePluginManager, localizationService, localizedModelFactory, _dateTimeHelper);
        _exchangeRatePluginManager = exchangeRatePluginManager;
		_logger.LogDebug("CurrencyModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},exchangeRatePluginManager={ExchangeRatePluginManager},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeMappingSupportedModelFactory=<redacted>): _exchangeRatePluginManager → {_exchangeRatePluginManager}", currencySettings, currencyService, dateTimeHelper, exchangeRatePluginManager, localizationService, localizedModelFactory, _exchangeRatePluginManager);
        _localizationService = localizationService;
		_logger.LogDebug("CurrencyModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},exchangeRatePluginManager={ExchangeRatePluginManager},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeMappingSupportedModelFactory=<redacted>): _localizationService → {_localizationService}", currencySettings, currencyService, dateTimeHelper, exchangeRatePluginManager, localizationService, localizedModelFactory, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("CurrencyModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},exchangeRatePluginManager={ExchangeRatePluginManager},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeMappingSupportedModelFactory=<redacted>): _localizedModelFactory → {_localizedModelFactory}", currencySettings, currencyService, dateTimeHelper, exchangeRatePluginManager, localizationService, localizedModelFactory, _localizedModelFactory);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare exchange rate provider model
    /// </summary>
    /// <param name="model">Currency exchange rate provider model</param>
    /// <param name="prepareExchangeRates">Whether to prepare exchange rate models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareExchangeRateProviderModelAsync(CurrencyExchangeRateProviderModel model, bool prepareExchangeRates)
    {
		_logger.LogError(">>> Entering PrepareExchangeRateProviderModelAsync(model={Model},prepareExchangeRates={PrepareExchangeRates})", model, prepareExchangeRates);
        ArgumentNullException.ThrowIfNull(model);

        model.AutoUpdateEnabled = _currencySettings.AutoUpdateEnabled;
		_logger.LogDebug("PrepareExchangeRateProviderModelAsync(model={Model},prepareExchangeRates={PrepareExchangeRates}): model.AutoUpdateEnabled → {Arg2}", model, prepareExchangeRates, model.AutoUpdateEnabled);

        //prepare available exchange rate providers
        var availableExchangeRateProviders = await _exchangeRatePluginManager.LoadAllPluginsAsync();
		_logger.LogError("PrepareExchangeRateProviderModelAsync(model={Model},prepareExchangeRates={PrepareExchangeRates}): availableExchangeRateProviders → {AvailableExchangeRateProviders}", model, prepareExchangeRates, availableExchangeRateProviders);

        model.ExchangeRateProviders = availableExchangeRateProviders.Select(provider => new SelectListItem
        {
            Text = provider.PluginDescriptor.FriendlyName,
            Value = provider.PluginDescriptor.SystemName,
            Selected = _exchangeRatePluginManager.IsPluginActive(provider)
        }).ToList();
			_logger.LogDebug("PrepareExchangeRateProviderModelAsync(model={Model},prepareExchangeRates={PrepareExchangeRates}): Selected → {Selected}", model, prepareExchangeRates, Selected);

        //prepare exchange rates
        if (prepareExchangeRates)
            await PrepareExchangeRateModelsAsync(model.ExchangeRates);
			_logger.LogWarning("<<< Exiting PrepareExchangeRateProviderModelAsync(model={Model},prepareExchangeRates={PrepareExchangeRates})", model, prepareExchangeRates);
    }

    /// <summary>
    /// Prepare exchange rate models
    /// </summary>
    /// <param name="models">List of currency exchange rate model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareExchangeRateModelsAsync(IList<CurrencyExchangeRateModel> models)
    {
		_logger.LogError(">>> Entering PrepareExchangeRateModelsAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //get exchange rates
        var exchangeRates = await _currencyService.GetCurrencyLiveRatesAsync();
		_logger.LogError("PrepareExchangeRateModelsAsync(models={Models}): exchangeRates → {ExchangeRates}", models, exchangeRates);

        //filter by existing currencies
        var currencies = await _currencyService.GetAllCurrenciesAsync(true);
		_logger.LogError("PrepareExchangeRateModelsAsync(models={Models}): currencies → {Currencies}", models, currencies);
        exchangeRates = exchangeRates
            .Where(rate => currencies
                .Any(currency => currency.CurrencyCode.Equals(rate.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))).ToList();

        //prepare models
        foreach (var rate in exchangeRates)
        {
            models.Add(new CurrencyExchangeRateModel { CurrencyCode = rate.CurrencyCode, Rate = rate.Rate });
        }
		_logger.LogWarning("<<< Exiting PrepareExchangeRateModelsAsync(models={Models})", models);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare currency search model
    /// </summary>
    /// <param name="searchModel">Currency search model</param>
    /// <param name="prepareExchangeRates">Whether to prepare exchange rate models</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency search model
    /// </returns>
    public virtual async Task<CurrencySearchModel> PrepareCurrencySearchModelAsync(CurrencySearchModel searchModel, bool prepareExchangeRates = false)
    {
		_logger.LogError(">>> Entering PrepareCurrencySearchModelAsync(searchModel={SearchModel},prepareExchangeRates={PrepareExchangeRates})", searchModel, prepareExchangeRates);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare exchange rate provider model
        await PrepareExchangeRateProviderModelAsync(searchModel.ExchangeRateProviderModel, prepareExchangeRates);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged currency list model
    /// </summary>
    /// <param name="searchModel">Currency search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency list model
    /// </returns>
    public virtual async Task<CurrencyListModel> PrepareCurrencyListModelAsync(CurrencySearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareCurrencyListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get currencies
        var currencies = (await _currencyService.GetAllCurrenciesAsync(showHidden: true)).ToPagedList(searchModel);
		_logger.LogError("PrepareCurrencyListModelAsync(searchModel={SearchModel}): currencies → {Currencies}", searchModel, currencies);

        //prepare list model
        var model = new CurrencyListModel().PrepareToGrid(searchModel, currencies, () =>
        {
            return currencies.Select(currency =>
            {
                //fill in model values from the entity
                var currencyModel = currency.ToModel<CurrencyModel>();
				_logger.LogError("PrepareCurrencyListModelAsync(searchModel={SearchModel}): currencyModel → {CurrencyModel}", searchModel, currencyModel);

                //fill in additional values (not existing in the entity)
                currencyModel.IsPrimaryExchangeRateCurrency = currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId;
                currencyModel.IsPrimaryStoreCurrency = currency.Id == _currencySettings.PrimaryStoreCurrencyId;

                return currencyModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareCurrencyListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare currency model
    /// </summary>
    /// <param name="model">Currency model</param>
    /// <param name="currency">Currency</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency model
    /// </returns>
    public virtual async Task<CurrencyModel> PrepareCurrencyModelAsync(CurrencyModel model, Currency currency, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareCurrencyModelAsync(model={Model},currency={Currency},excludeProperties={ExcludeProperties})", model, currency, excludeProperties);
        Func<CurrencyLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (currency != null)
        {
            //fill in model values from the entity
            model ??= currency.ToModel<CurrencyModel>();

            //convert dates to the user time
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(currency.CreatedOnUtc, DateTimeKind.Utc);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(currency, entity => entity.Name, languageId, false, false);
            };
        }

        //set default values for the new model
        if (currency == null)
        {
            model.Published = true;
			_logger.LogDebug("PrepareCurrencyModelAsync(model={Model},currency={Currency},excludeProperties={ExcludeProperties}): model.Published → {Arg3}", model, currency, excludeProperties, model.Published);
			_logger.LogDebug("PrepareCurrencyModelAsync(model={Model},currency={Currency},excludeProperties={ExcludeProperties}): model.Rate → {Arg3}", model, currency, excludeProperties, model.Rate);
            model.Rate = 1;
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, currency, excludeProperties);

		_logger.LogWarning("<<< Exiting PrepareCurrencyModelAsync(model={Model},currency={Currency},excludeProperties={ExcludeProperties})", model, currency, excludeProperties);
        return model;
    }

    #endregion
}