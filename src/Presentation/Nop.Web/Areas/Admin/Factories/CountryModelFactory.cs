using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the country model factory implementation
/// </summary>
public partial class CountryModelFactory : ICountryModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICountryService _countryService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly IStateProvinceService _stateProvinceService;

    #endregion

    #region Ctor

    public CountryModelFactory(ICountryService countryService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        IStateProvinceService stateProvinceService)
    {
        _countryService = countryService;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        _stateProvinceService = stateProvinceService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare state and province search model
    /// </summary>
    /// <param name="searchModel">State and province search model</param>
    /// <param name="country">Country</param>
    /// <returns>State and province search model</returns>
    protected virtual StateProvinceSearchModel PrepareStateProvinceSearchModel(StateProvinceSearchModel searchModel, Country country)
    {
		_logger.LogDebug(">>> Entering PrepareStateProvinceSearchModel(searchModel={SearchModel},country={Country})", searchModel, country);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(country);

        searchModel.CountryId = country.Id;
		_logger.LogDebug("PrepareStateProvinceSearchModel(searchModel={SearchModel},country={Country}): searchModel.CountryId → {Arg2}", searchModel, country, searchModel.CountryId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareStateProvinceSearchModel(searchModel={SearchModel},country={Country})", searchModel, country);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare country search model
    /// </summary>
    /// <param name="searchModel">Country search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country search model
    /// </returns>
    public virtual Task<CountrySearchModel> PrepareCountrySearchModelAsync(CountrySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCountrySearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareCountrySearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged country list model
    /// </summary>
    /// <param name="searchModel">Country search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country list model
    /// </returns>
    public virtual async Task<CountryListModel> PrepareCountryListModelAsync(CountrySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCountryListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get countries
        var countries = (await _countryService.GetAllCountriesAsync(showHidden: true)).ToPagedList(searchModel);
		_logger.LogDebug("PrepareCountryListModelAsync(searchModel={SearchModel}): countries → {Countries}", searchModel, countries);

        //prepare list model
        var model = await new CountryListModel().PrepareToGridAsync(searchModel, countries, () =>
        {
            //fill in model values from the entity
            return countries.SelectAwait(async country =>
            {
                var countryModel = country.ToModel<CountryModel>();
				_logger.LogDebug("PrepareCountryListModelAsync(searchModel={SearchModel}): countryModel → {CountryModel}", searchModel, countryModel);

                countryModel.NumberOfStates = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id))?.Count ?? 0;

                return countryModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareCountryListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare country model
    /// </summary>
    /// <param name="model">Country model</param>
    /// <param name="country">Country</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country model
    /// </returns>
    public virtual async Task<CountryModel> PrepareCountryModelAsync(CountryModel model, Country country, bool excludeProperties = false)
    {
        Func<CountryLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (country != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = country.ToModel<CountryModel>();
				_logger.LogDebug("PrepareCountryModelAsync(model={Model},country={Country},excludeProperties={ExcludeProperties}): model → {Model1}", model, country, excludeProperties, model);
                model.NumberOfStates = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id))?.Count ?? 0;
            }

            //prepare nested search model
            PrepareStateProvinceSearchModel(model.StateProvinceSearchModel, country);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(country, entity => entity.Name, languageId, false, false);
            };
        }

        //set default values for the new model
        if (country == null)
        {
            model.Published = true;
            model.AllowsBilling = true;
            model.AllowsShipping = true;
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, country, excludeProperties);

        return model;
    }

    /// <summary>
    /// Prepare paged state and province list model
    /// </summary>
    /// <param name="searchModel">State and province search model</param>
    /// <param name="country">Country</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the state and province list model
    /// </returns>
    public virtual async Task<StateProvinceListModel> PrepareStateProvinceListModelAsync(StateProvinceSearchModel searchModel, Country country)
    {
		_logger.LogDebug(">>> Entering PrepareStateProvinceListModelAsync(searchModel={SearchModel},country={Country})", searchModel, country);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(country);

        //get comments
        var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true)).ToPagedList(searchModel);
		_logger.LogDebug("PrepareStateProvinceListModelAsync(searchModel={SearchModel},country={Country}): states → {States}", searchModel, country, states);

        //prepare list model
        var model = new StateProvinceListModel().PrepareToGrid(searchModel, states, () =>
        {
            //fill in model values from the entity
            return states.Select(state => state.ToModel<StateProvinceModel>());
        });

		_logger.LogDebug("<<< Exiting PrepareStateProvinceListModelAsync(searchModel={SearchModel},country={Country})", searchModel, country);
        return model;
    }

    /// <summary>
    /// Prepare state and province model
    /// </summary>
    /// <param name="model">State and province model</param>
    /// <param name="country">Country</param>
    /// <param name="state">State or province</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the state and province model
    /// </returns>
    public virtual async Task<StateProvinceModel> PrepareStateProvinceModelAsync(StateProvinceModel model,
        Country country, StateProvince state, bool excludeProperties = false)
    {
        Func<StateProvinceLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (state != null)
        {
            //fill in model values from the entity
            model ??= state.ToModel<StateProvinceModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(state, entity => entity.Name, languageId, false, false);
            };
        }

        model.CountryId = country.Id;

        //set default values for the new model
        if (state == null)
            model.Published = true;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    #endregion
}