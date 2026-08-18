using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Polls;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the poll model factory implementation
/// </summary>
public partial class PollModelFactory : IPollModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILanguageService _languageService;
    protected readonly IPollService _pollService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

    #endregion

    #region Ctor

    public PollModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        IDateTimeHelper dateTimeHelper,
        ILanguageService languageService,
        IPollService pollService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
    {
		_logger.LogDebug(">>> Entering PollModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},languageService={LanguageService},pollService={PollService},storeMappingSupportedModelFactory=<redacted>)", catalogSettings, baseAdminModelFactory, dateTimeHelper, languageService, pollService);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("PollModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},languageService={LanguageService},pollService={PollService},storeMappingSupportedModelFactory=<redacted>): _catalogSettings → {_catalogSettings}", catalogSettings, baseAdminModelFactory, dateTimeHelper, languageService, pollService, _catalogSettings);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("PollModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},languageService={LanguageService},pollService={PollService},storeMappingSupportedModelFactory=<redacted>): _baseAdminModelFactory → {_baseAdminModelFactory}", catalogSettings, baseAdminModelFactory, dateTimeHelper, languageService, pollService, _baseAdminModelFactory);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("PollModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},languageService={LanguageService},pollService={PollService},storeMappingSupportedModelFactory=<redacted>): _dateTimeHelper → {_dateTimeHelper}", catalogSettings, baseAdminModelFactory, dateTimeHelper, languageService, pollService, _dateTimeHelper);
        _languageService = languageService;
		_logger.LogDebug("PollModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},languageService={LanguageService},pollService={PollService},storeMappingSupportedModelFactory=<redacted>): _languageService → {_languageService}", catalogSettings, baseAdminModelFactory, dateTimeHelper, languageService, pollService, _languageService);
        _pollService = pollService;
		_logger.LogDebug("PollModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},languageService={LanguageService},pollService={PollService},storeMappingSupportedModelFactory=<redacted>): _pollService → {_pollService}", catalogSettings, baseAdminModelFactory, dateTimeHelper, languageService, pollService, _pollService);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
		_logger.LogDebug("<<< Exiting PollModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},languageService={LanguageService},pollService={PollService},storeMappingSupportedModelFactory=<redacted>)", catalogSettings, baseAdminModelFactory, dateTimeHelper, languageService, pollService);
		_logger.LogDebug("PollModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},dateTimeHelper={DateTimeHelper},languageService={LanguageService},pollService={PollService},storeMappingSupportedModelFactory=<redacted>): _storeMappingSupportedModelFactory → {_storeMappingSupportedModelFactory}", catalogSettings, baseAdminModelFactory, dateTimeHelper, languageService, pollService, _storeMappingSupportedModelFactory);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare poll answer search model
    /// </summary>
    /// <param name="searchModel">Poll answer search model</param>
    /// <param name="poll">Poll</param>
    /// <returns>Poll answer search model</returns>
    protected virtual PollAnswerSearchModel PreparePollAnswerSearchModel(PollAnswerSearchModel searchModel, Poll poll)
    {
		_logger.LogDebug(">>> Entering PreparePollAnswerSearchModel(searchModel={SearchModel},poll={Poll})", searchModel, poll);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(poll);

        searchModel.PollId = poll.Id;
		_logger.LogDebug("PreparePollAnswerSearchModel(searchModel={SearchModel},poll={Poll}): searchModel.PollId → {Arg2}", searchModel, poll, searchModel.PollId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PreparePollAnswerSearchModel(searchModel={SearchModel},poll={Poll})", searchModel, poll);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare poll search model
    /// </summary>
    /// <param name="searchModel">Poll search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll search model
    /// </returns>
    public virtual async Task<PollSearchModel> PreparePollSearchModelAsync(PollSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PreparePollSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PreparePollSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged poll list model
    /// </summary>
    /// <param name="searchModel">Poll search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll list model
    /// </returns>
    public virtual async Task<PollListModel> PreparePollListModelAsync(PollSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PreparePollListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get polls
        var polls = await _pollService.GetPollsAsync(showHidden: true,
            storeId: searchModel.SearchStoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new PollListModel().PrepareToGridAsync(searchModel, polls, () =>
        {
            return polls.SelectAwait(async poll =>
            {
                //fill in model values from the entity
                var pollModel = poll.ToModel<PollModel>();
				_logger.LogDebug("PreparePollListModelAsync(searchModel={SearchModel}): pollModel → {PollModel}", searchModel, pollModel);

                //convert dates to the user time
                if (poll.StartDateUtc.HasValue)
                    pollModel.StartDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(poll.StartDateUtc.Value, DateTimeKind.Utc);
                if (poll.EndDateUtc.HasValue)
                    pollModel.EndDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(poll.EndDateUtc.Value, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                pollModel.LanguageName = (await _languageService.GetLanguageByIdAsync(poll.LanguageId))?.Name;

                return pollModel;
            });
        });

		_logger.LogDebug("<<< Exiting PreparePollListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare poll model
    /// </summary>
    /// <param name="model">Poll model</param>
    /// <param name="poll">Poll</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll model
    /// </returns>
    public virtual async Task<PollModel> PreparePollModelAsync(PollModel model, Poll poll, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PreparePollModelAsync(model={Model},poll={Poll},excludeProperties={ExcludeProperties})", model, poll, excludeProperties);
        if (poll != null)
        {
            //fill in model values from the entity
            model ??= poll.ToModel<PollModel>();

            model.StartDateUtc = poll.StartDateUtc;
			_logger.LogDebug("PreparePollModelAsync(model={Model},poll={Poll},excludeProperties={ExcludeProperties}): model.StartDateUtc → {Arg3}", model, poll, excludeProperties, model.StartDateUtc);
			_logger.LogDebug("PreparePollModelAsync(model={Model},poll={Poll},excludeProperties={ExcludeProperties}): model.EndDateUtc → {Arg3}", model, poll, excludeProperties, model.EndDateUtc);
            model.EndDateUtc = poll.EndDateUtc;

            //prepare nested search model
            PreparePollAnswerSearchModel(model.PollAnswerSearchModel, poll);
        }

        //set default values for the new model
        if (poll == null)
        {
            model.Published = true;
			_logger.LogDebug("PreparePollModelAsync(model={Model},poll={Poll},excludeProperties={ExcludeProperties}): model.Published → {Arg3}", model, poll, excludeProperties, model.Published);
			_logger.LogDebug("PreparePollModelAsync(model={Model},poll={Poll},excludeProperties={ExcludeProperties}): model.ShowOnHomepage → {Arg3}", model, poll, excludeProperties, model.ShowOnHomepage);
            model.ShowOnHomepage = true;
        }

        //prepare available languages
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, poll, excludeProperties);

		_logger.LogDebug("<<< Exiting PreparePollModelAsync(model={Model},poll={Poll},excludeProperties={ExcludeProperties})", model, poll, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged poll answer list model
    /// </summary>
    /// <param name="searchModel">Poll answer search model</param>
    /// <param name="poll">Poll</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll answer list model
    /// </returns>
    public virtual async Task<PollAnswerListModel> PreparePollAnswerListModelAsync(PollAnswerSearchModel searchModel, Poll poll)
    {
		_logger.LogDebug(">>> Entering PreparePollAnswerListModelAsync(searchModel={SearchModel},poll={Poll})", searchModel, poll);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(poll);

        //get poll answers
        var pollAnswers = await _pollService.GetPollAnswerByPollAsync(poll.Id, searchModel.Page - 1, searchModel.PageSize);
		_logger.LogDebug("PreparePollAnswerListModelAsync(searchModel={SearchModel},poll={Poll}): pollAnswers → {PollAnswers}", searchModel, poll, pollAnswers);

        //prepare list model
        var model = new PollAnswerListModel().PrepareToGrid(searchModel, pollAnswers,
            () => pollAnswers.Select(pollAnswer => pollAnswer.ToModel<PollAnswerModel>()));

		_logger.LogDebug("<<< Exiting PreparePollAnswerListModelAsync(searchModel={SearchModel},poll={Poll})", searchModel, poll);
        return model;
    }

    #endregion
}