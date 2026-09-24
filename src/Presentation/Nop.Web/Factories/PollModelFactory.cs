using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Polls;
using Nop.Services.Polls;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Polls;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the poll model factory
/// </summary>
public partial class PollModelFactory : IPollModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IPollService _pollService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PollModelFactory(IPollService pollService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering PollModelFactory(pollService={PollService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},workContext={WorkContext})", pollService, staticCacheManager, storeContext, workContext);
        _pollService = pollService;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("PollModelFactory(pollService={PollService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", pollService, staticCacheManager, storeContext, workContext, _staticCacheManager);
        _storeContext = storeContext;
		_logger.LogDebug("PollModelFactory(pollService={PollService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},workContext={WorkContext}): _storeContext → {_storeContext}", pollService, staticCacheManager, storeContext, workContext, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("PollModelFactory(pollService={PollService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},workContext={WorkContext}): _workContext → {_workContext}", pollService, staticCacheManager, storeContext, workContext, _workContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the poll model
    /// </summary>
    /// <param name="poll">Poll</param>
    /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that customer already voted for this poll</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll model
    /// </returns>
    public virtual async Task<PollModel> PreparePollModelAsync(Poll poll, bool setAlreadyVotedProperty)
    {
		_logger.LogError(">>> Entering PreparePollModelAsync(poll={Poll},setAlreadyVotedProperty={SetAlreadyVotedProperty})", poll, setAlreadyVotedProperty);
        ArgumentNullException.ThrowIfNull(poll);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PreparePollModelAsync(poll={Poll},setAlreadyVotedProperty={SetAlreadyVotedProperty}): customer → {Customer}", poll, setAlreadyVotedProperty, customer);

        var model = new PollModel
        {
            Id = poll.Id,
            AlreadyVoted = setAlreadyVotedProperty && await _pollService.AlreadyVotedAsync(poll.Id, customer.Id),
            Name = poll.Name
        };
        var answers = await _pollService.GetPollAnswerByPollAsync(poll.Id);
		_logger.LogError("PreparePollModelAsync(poll={Poll},setAlreadyVotedProperty={SetAlreadyVotedProperty}): answers → {Answers}", poll, setAlreadyVotedProperty, answers);

        foreach (var answer in answers)
            model.TotalVotes += answer.NumberOfVotes;
        foreach (var pa in answers)
        {
            model.Answers.Add(new PollAnswerModel
            {
                Id = pa.Id,
                Name = pa.Name,
                NumberOfVotes = pa.NumberOfVotes,
                PercentOfTotalVotes = model.TotalVotes > 0 ? ((Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes)) * Convert.ToDouble(100)) : 0,
            });
        }

		_logger.LogWarning("<<< Exiting PreparePollModelAsync(poll={Poll},setAlreadyVotedProperty={SetAlreadyVotedProperty})", poll, setAlreadyVotedProperty);
        return model;
    }

    /// <summary>
    /// Get the poll model by poll system keyword
    /// </summary>
    /// <param name="systemKeyword">Poll system keyword</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll model
    /// </returns>
    public virtual async Task<PollModel> PreparePollModelBySystemNameAsync(string systemKeyword)
    {
		_logger.LogError(">>> Entering PreparePollModelBySystemNameAsync(systemKeyword={SystemKeyword})", systemKeyword);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrWhiteSpace(systemKeyword))
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PreparePollModelBySystemNameAsync(systemKeyword={SystemKeyword}): store → {Store}", systemKeyword, store);
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
		_logger.LogError("PreparePollModelBySystemNameAsync(systemKeyword={SystemKeyword}): currentLanguage → {CurrentLanguage}", systemKeyword, currentLanguage);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.PollBySystemNameModelKey,
            systemKeyword, currentLanguage, store);

        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var poll = (await _pollService
                    .GetPollsAsync(store.Id, currentLanguage.Id, systemKeyword: systemKeyword))
                .FirstOrDefault();

            //we do not cache nulls. that's why let's return an empty record (ID = 0)
            if (poll == null)
                return new PollModel { Id = 0 };

            return await PreparePollModelAsync(poll, false);
        });

        if ((cachedModel?.Id ?? 0) == 0)
            return null;

        //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
        //But first we need to clone the cached model (the updated one should not be cached)
        var model = cachedModel with { };
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PreparePollModelBySystemNameAsync(systemKeyword={SystemKeyword}): customer → {Customer}", systemKeyword, customer);
        model.AlreadyVoted = await _pollService.AlreadyVotedAsync(model.Id, customer.Id);

		_logger.LogError("PreparePollModelBySystemNameAsync(systemKeyword)={SystemKeyword}: external service call took {Arg1} ms", systemKeyword, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PreparePollModelBySystemNameAsync(systemKeyword={SystemKeyword})", systemKeyword);
        return model;
    }

    /// <summary>
    /// Prepare the home page poll models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the poll model
    /// </returns>
    public virtual async Task<List<PollModel>> PrepareHomepagePollModelsAsync()
    {
		_logger.LogError(">>> Entering PrepareHomepagePollModelsAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareHomepagePollModelsAsync(): customer → {Customer}", customer);
		_logger.LogError("PrepareHomepagePollModelsAsync(): store → {Store}", store);
        var language = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.HomepagePollsModelKey, language, store);
		_logger.LogError("PrepareHomepagePollModelsAsync(): language → {Language}", language);
		_logger.LogError("PrepareHomepagePollModelsAsync(): cacheKey → {CacheKey}", cacheKey);

        var cachedPolls = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var polls = await _pollService.GetPollsAsync(store.Id, language.Id, loadShownOnHomepageOnly: true);
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
            var pollsModels = await polls.SelectAwait(async poll => await PreparePollModelAsync(poll, false)).ToListAsync();
			_logger.LogError("PrepareHomepagePollModelsAsync(): polls → {Polls}", polls);
			_logger.LogError("PrepareHomepagePollModelsAsync(): database query executed in {Arg0} ms", __logStartDb.ElapsedMilliseconds);
            return pollsModels;
        });
		_logger.LogError("PrepareHomepagePollModelsAsync(): database query executed in {Arg0} ms", __logStartDb.ElapsedMilliseconds);

        //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
        //But first we need to clone the cached model (the updated one should not be cached)
        var model = new List<PollModel>();
		_logger.LogError("PrepareHomepagePollModelsAsync(): model → {Model}", model);
        foreach (var poll in cachedPolls)
        {
            var pollModel = poll with { };
            pollModel.AlreadyVoted = await _pollService.AlreadyVotedAsync(pollModel.Id, customer.Id);
            model.Add(pollModel);
        }

		_logger.LogError("PrepareHomepagePollModelsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareHomepagePollModelsAsync()");
        return model;
    }

    #endregion
}