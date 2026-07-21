using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the language model factory implementation
/// </summary>
public partial class LanguageModelFactory : ILanguageModelFactory
{
    #region Fields

    private readonly IBaseAdminModelFactory _baseAdminModelFactory;
    private readonly INopFileProvider _fileProvider;
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    private readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public LanguageModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        INopFileProvider fileProvider,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        MediaSettings mediaSettings)
    {
        _baseAdminModelFactory = baseAdminModelFactory;
        _fileProvider = fileProvider;
        _languageService = languageService;
        _localizationService = localizationService;
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        _mediaSettings = mediaSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare locale resource search model
    /// </summary>
    /// <param name="searchModel">Locale resource search model</param>
    /// <param name="language">Language</param>
    /// <returns>Locale resource search model</returns>
    protected virtual LocaleResourceSearchModel PrepareLocaleResourceSearchModel(LocaleResourceSearchModel searchModel, Language language)
    {
		logger.LogDebug(">>> Entering PrepareLocaleResourceSearchModel(searchModel={SearchModel},language={Language})", searchModel, language);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(language);

        searchModel.LanguageId = language.Id;
		logger.LogDebug("PrepareLocaleResourceSearchModel(searchModel={SearchModel},language={Language}): searchModel.LanguageId → {Arg2}", searchModel, language, searchModel.LanguageId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareLocaleResourceSearchModel(searchModel={SearchModel},language={Language})", searchModel, language);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare language search model
    /// </summary>
    /// <param name="searchModel">Language search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language search model
    /// </returns>
    public virtual Task<LanguageSearchModel> PrepareLanguageSearchModelAsync(LanguageSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PrepareLanguageSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareLanguageSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged language list model
    /// </summary>
    /// <param name="searchModel">Language search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language list model
    /// </returns>
    public virtual async Task<LanguageListModel> PrepareLanguageListModelAsync(LanguageSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PrepareLanguageListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get languages
        var languages = (await _languageService.GetAllLanguagesAsync(showHidden: true)).ToPagedList(searchModel);

		logger.LogDebug("PrepareLanguageListModelAsync(searchModel={SearchModel}): languages → {Languages}", searchModel, languages);
        //prepare list model
        var model = new LanguageListModel().PrepareToGrid(searchModel, languages, () =>
        {
            return languages.Select(language => language.ToModel<LanguageModel>());
        });

		logger.LogDebug("<<< Exiting PrepareLanguageListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare language model
    /// </summary>
    /// <param name="model">Language model</param>
    /// <param name="language">Language</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language model
    /// </returns>
    public virtual async Task<LanguageModel> PrepareLanguageModelAsync(LanguageModel model, Language language, bool excludeProperties = false)
    {
        if (language != null)
        {
            //fill in model values from the entity
            model ??= language.ToModel<LanguageModel>();

            //prepare nested search model
            PrepareLocaleResourceSearchModel(model.LocaleResourceSearchModel, language);
        }

        //set default values for the new model
        if (language == null)
        {
            model.DisplayOrder = (await _languageService.GetAllLanguagesAsync()).Max(l => l.DisplayOrder) + 1;
            model.Published = true;
        }
        var flagNames = _fileProvider
            .EnumerateFiles(_fileProvider.Combine(_fileProvider.GetLocalImagesPath(_mediaSettings), "flags"), "*.png")
            .Select(_fileProvider.GetFileName)
            .ToList();

        model.AvailableFlagImages = flagNames.ConvertAll(flagName => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Text = flagName,
            Value = flagName
        });

        //prepare available currencies
        await _baseAdminModelFactory.PrepareCurrenciesAsync(model.AvailableCurrencies,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Common.EmptyItemText"));

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, language, excludeProperties);

        return model;
    }

    /// <summary>
    /// Prepare paged locale resource list model
    /// </summary>
    /// <param name="searchModel">Locale resource search model</param>
    /// <param name="language">Language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the locale resource list model
    /// </returns>
    public virtual async Task<LocaleResourceListModel> PrepareLocaleResourceListModelAsync(LocaleResourceSearchModel searchModel, Language language)
    {
		logger.LogDebug(">>> Entering PrepareLocaleResourceListModelAsync(searchModel={SearchModel},language={Language})", searchModel, language);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(language);

        //get locale resources
        var localeResources = (await _localizationService.GetAllResourceValuesAsync(language.Id, loadPublicLocales: null))
            .OrderBy(localeResource => localeResource.Key).AsQueryable();
			logger.LogDebug("PrepareLocaleResourceListModelAsync(searchModel={SearchModel},language={Language}): localeResources → {LocaleResources}", searchModel, language, localeResources);

        //filter locale resources
        if (!string.IsNullOrEmpty(searchModel.SearchResourceName))
            localeResources = localeResources.Where(l => l.Key.ToLowerInvariant().Contains(searchModel.SearchResourceName.ToLowerInvariant()));
        if (!string.IsNullOrEmpty(searchModel.SearchResourceValue))
            localeResources = localeResources.Where(l => l.Value.Value.ToLowerInvariant().Contains(searchModel.SearchResourceValue.ToLowerInvariant()));

        var pagedLocaleResources = await localeResources.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);

		logger.LogDebug("PrepareLocaleResourceListModelAsync(searchModel={SearchModel},language={Language}): pagedLocaleResources → {PagedLocaleResources}", searchModel, language, pagedLocaleResources);
        //prepare list model
        var model = new LocaleResourceListModel().PrepareToGrid(searchModel, pagedLocaleResources, () =>
        {
            //fill in model values from the entity
            return pagedLocaleResources.Select(localeResource => new LocaleResourceModel
            {
                LanguageId = language.Id,
                Id = localeResource.Value.Key,
                ResourceName = localeResource.Key,
                ResourceValue = localeResource.Value.Value
            });
        });

		logger.LogDebug("<<< Exiting PrepareLocaleResourceListModelAsync(searchModel={SearchModel},language={Language})", searchModel, language);
        return model;
    }

    #endregion
}
