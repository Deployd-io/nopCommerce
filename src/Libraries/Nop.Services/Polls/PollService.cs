using Nop.Core;
using Nop.Core.Domain.Polls;
using Nop.Data;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Polls;

/// <summary>
/// Poll service
/// </summary>
public partial class PollService : IPollService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<Poll> _pollRepository;
    protected readonly IRepository<PollAnswer> _pollAnswerRepository;
    protected readonly IRepository<PollVotingRecord> _pollVotingRecordRepository;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public PollService(
        IRepository<Poll> pollRepository,
        IRepository<PollAnswer> pollAnswerRepository,
        IRepository<PollVotingRecord> pollVotingRecordRepository,
        IStoreMappingService storeMappingService)
    {
		_logger.LogDebug(">>> Entering PollService(pollRepository={PollRepository},pollAnswerRepository={PollAnswerRepository},pollVotingRecordRepository={PollVotingRecordRepository},storeMappingService=<redacted>)", pollRepository, pollAnswerRepository, pollVotingRecordRepository);
        _pollRepository = pollRepository;
		_logger.LogDebug("PollService(pollRepository={PollRepository},pollAnswerRepository={PollAnswerRepository},pollVotingRecordRepository={PollVotingRecordRepository},storeMappingService=<redacted>): _pollRepository → {_pollRepository}", pollRepository, pollAnswerRepository, pollVotingRecordRepository, _pollRepository);
        _pollAnswerRepository = pollAnswerRepository;
		_logger.LogDebug("PollService(pollRepository={PollRepository},pollAnswerRepository={PollAnswerRepository},pollVotingRecordRepository={PollVotingRecordRepository},storeMappingService=<redacted>): _pollAnswerRepository → {_pollAnswerRepository}", pollRepository, pollAnswerRepository, pollVotingRecordRepository, _pollAnswerRepository);
        _pollVotingRecordRepository = pollVotingRecordRepository;
		_logger.LogDebug("PollService(pollRepository={PollRepository},pollAnswerRepository={PollAnswerRepository},pollVotingRecordRepository={PollVotingRecordRepository},storeMappingService=<redacted>): _pollVotingRecordRepository → {_pollVotingRecordRepository}", pollRepository, pollAnswerRepository, pollVotingRecordRepository, _pollVotingRecordRepository);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("<<< Exiting PollService(pollRepository={PollRepository},pollAnswerRepository={PollAnswerRepository},pollVotingRecordRepository={PollVotingRecordRepository},storeMappingService=<redacted>)", pollRepository, pollAnswerRepository, pollVotingRecordRepository);
		_logger.LogDebug("PollService(pollRepository={PollRepository},pollAnswerRepository={PollAnswerRepository},pollVotingRecordRepository={PollVotingRecordRepository},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", pollRepository, pollAnswerRepository, pollVotingRecordRepository, _storeMappingService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a poll
    /// </summary>
    /// <param name="pollId">The poll identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll
    /// </returns>
    public virtual async Task<Poll> GetPollByIdAsync(int pollId)
    {
		_logger.LogDebug(">>> Entering GetPollByIdAsync(pollId={PollId})", pollId);
		_logger.LogDebug("<<< Exiting GetPollByIdAsync(pollId={PollId})", pollId);
        return await _pollRepository.GetByIdAsync(pollId, cache => default);
    }

    /// <summary>
    /// Gets polls
    /// </summary>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <param name="languageId">Language identifier; pass 0 to load all records</param>
    /// <param name="showHidden">Whether to show hidden records (not published, not started and expired)</param>
    /// <param name="loadShownOnHomepageOnly">Retrieve only shown on home page polls</param>
    /// <param name="systemKeyword">The poll system keyword; pass null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the polls
    /// </returns>
    public virtual async Task<IPagedList<Poll>> GetPollsAsync(int storeId, int languageId = 0, bool showHidden = false,
        bool loadShownOnHomepageOnly = false, string systemKeyword = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetPollsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden},loadShownOnHomepageOnly={LoadShownOnHomepageOnly},systemKeyword={SystemKeyword},pageIndex={PageIndex},pageSize={PageSize})", storeId, languageId, showHidden, loadShownOnHomepageOnly, systemKeyword, pageIndex, pageSize);
        var query = _pollRepository.Table;

		_logger.LogDebug("GetPollsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden},loadShownOnHomepageOnly={LoadShownOnHomepageOnly},systemKeyword={SystemKeyword},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", storeId, languageId, showHidden, loadShownOnHomepageOnly, systemKeyword, pageIndex, pageSize, query);
        if (!showHidden || storeId > 0)
        {
            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);
        }

        //whether to load not published, not started and expired polls
        if (!showHidden)
        {
            var utcNow = DateTime.UtcNow;
            query = query.Where(poll => poll.Published);
            query = query.Where(poll => !poll.StartDateUtc.HasValue || poll.StartDateUtc <= utcNow);
            query = query.Where(poll => !poll.EndDateUtc.HasValue || poll.EndDateUtc >= utcNow);
        }

        //load homepage polls only
        if (loadShownOnHomepageOnly)
            query = query.Where(poll => poll.ShowOnHomepage);

        //filter by language
        if (languageId > 0)
            query = query.Where(poll => poll.LanguageId == languageId);

        //filter by system keyword
        if (!string.IsNullOrEmpty(systemKeyword))
            query = query.Where(poll => poll.SystemKeyword == systemKeyword);

        //order records by display order
        query = query.OrderBy(poll => poll.DisplayOrder).ThenBy(poll => poll.Id);

        //return paged list of polls
		_logger.LogDebug("<<< Exiting GetPollsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden},loadShownOnHomepageOnly={LoadShownOnHomepageOnly},systemKeyword={SystemKeyword},pageIndex={PageIndex},pageSize={PageSize})", storeId, languageId, showHidden, loadShownOnHomepageOnly, systemKeyword, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Deletes a poll
    /// </summary>
    /// <param name="poll">The poll</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePollAsync(Poll poll)
    {
		_logger.LogDebug(">>> Entering DeletePollAsync(poll={Poll})", poll);
		_logger.LogInformation("DeletePollAsync(poll)={Poll}: external service call took {Arg1} ms", poll, __logStart.ElapsedMilliseconds);
        await _pollRepository.DeleteAsync(poll);
		_logger.LogDebug("<<< Exiting DeletePollAsync(poll={Poll})", poll);
    }

    /// <summary>
    /// Inserts a poll
    /// </summary>
    /// <param name="poll">Poll</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPollAsync(Poll poll)
    {
		_logger.LogDebug(">>> Entering InsertPollAsync(poll={Poll})", poll);
        await _pollRepository.InsertAsync(poll);
		_logger.LogDebug("<<< Exiting InsertPollAsync(poll={Poll})", poll);
    }

    /// <summary>
    /// Updates the poll
    /// </summary>
    /// <param name="poll">Poll</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePollAsync(Poll poll)
    {
		_logger.LogDebug(">>> Entering UpdatePollAsync(poll={Poll})", poll);
        await _pollRepository.UpdateAsync(poll);
		_logger.LogDebug("<<< Exiting UpdatePollAsync(poll={Poll})", poll);
    }

    /// <summary>
    /// Gets a poll answer
    /// </summary>
    /// <param name="pollAnswerId">Poll answer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the poll answer
    /// </returns>
    public virtual async Task<PollAnswer> GetPollAnswerByIdAsync(int pollAnswerId)
    {
		_logger.LogDebug(">>> Entering GetPollAnswerByIdAsync(pollAnswerId={PollAnswerId})", pollAnswerId);
		_logger.LogDebug("<<< Exiting GetPollAnswerByIdAsync(pollAnswerId={PollAnswerId})", pollAnswerId);
        return await _pollAnswerRepository.GetByIdAsync(pollAnswerId, cache => default);
    }

    /// <summary>
    /// Deletes a poll answer
    /// </summary>
    /// <param name="pollAnswer">Poll answer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePollAnswerAsync(PollAnswer pollAnswer)
    {
		_logger.LogDebug(">>> Entering DeletePollAnswerAsync(pollAnswer={PollAnswer})", pollAnswer);
		_logger.LogInformation("DeletePollAnswerAsync(pollAnswer)={PollAnswer}: external service call took {Arg1} ms", pollAnswer, __logStart.ElapsedMilliseconds);
        await _pollAnswerRepository.DeleteAsync(pollAnswer);
		_logger.LogDebug("<<< Exiting DeletePollAnswerAsync(pollAnswer={PollAnswer})", pollAnswer);
    }

    /// <summary>
    /// Gets a poll answers by parent poll
    /// </summary>
    /// <param name="pollId">The poll identifier</param>
    /// <returns>Poll answer</returns>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IPagedList<PollAnswer>> GetPollAnswerByPollAsync(int pollId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetPollAnswerByPollAsync(pollId={PollId},pageIndex={PageIndex},pageSize={PageSize})", pollId, pageIndex, pageSize);
        var query = _pollAnswerRepository.Table.Where(pa => pa.PollId == pollId);

		_logger.LogDebug("GetPollAnswerByPollAsync(pollId={PollId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", pollId, pageIndex, pageSize, query);
        //order records by display order
        query = query.OrderBy(pa => pa.DisplayOrder).ThenBy(pa => pa.Id);

        //return paged list of polls
		_logger.LogDebug("<<< Exiting GetPollAnswerByPollAsync(pollId={PollId},pageIndex={PageIndex},pageSize={PageSize})", pollId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Inserts a poll answer
    /// </summary>
    /// <param name="pollAnswer">Poll answer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPollAnswerAsync(PollAnswer pollAnswer)
    {
		_logger.LogDebug(">>> Entering InsertPollAnswerAsync(pollAnswer={PollAnswer})", pollAnswer);
        await _pollAnswerRepository.InsertAsync(pollAnswer);
		_logger.LogDebug("<<< Exiting InsertPollAnswerAsync(pollAnswer={PollAnswer})", pollAnswer);
    }

    /// <summary>
    /// Updates the poll answer
    /// </summary>
    /// <param name="pollAnswer">Poll answer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePollAnswerAsync(PollAnswer pollAnswer)
    {
		_logger.LogDebug(">>> Entering UpdatePollAnswerAsync(pollAnswer={PollAnswer})", pollAnswer);
        await _pollAnswerRepository.UpdateAsync(pollAnswer);
		_logger.LogDebug("<<< Exiting UpdatePollAnswerAsync(pollAnswer={PollAnswer})", pollAnswer);
    }

    /// <summary>
    /// Gets a value indicating whether customer already voted for this poll
    /// </summary>
    /// <param name="pollId">Poll identifier</param>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> AlreadyVotedAsync(int pollId, int customerId)
    {
		_logger.LogDebug(">>> Entering AlreadyVotedAsync(pollId={PollId},customerId={CustomerId})", pollId, customerId);
        if (pollId == 0 || customerId == 0)
            return false;

        var result = await
            (from pa in _pollAnswerRepository.Table
                join pvr in _pollVotingRecordRepository.Table on pa.Id equals pvr.PollAnswerId
                where pa.PollId == pollId && pvr.CustomerId == customerId
                select pvr)
            .AnyAsync();
		_logger.LogDebug("<<< Exiting AlreadyVotedAsync(pollId={PollId},customerId={CustomerId})", pollId, customerId);
        return result;
    }

    /// <summary>
    /// Inserts a poll voting record
    /// </summary>
    /// <param name="pollVotingRecord">Voting record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPollVotingRecordAsync(PollVotingRecord pollVotingRecord)
    {
		_logger.LogDebug(">>> Entering InsertPollVotingRecordAsync(pollVotingRecord={PollVotingRecord})", pollVotingRecord);
        await _pollVotingRecordRepository.InsertAsync(pollVotingRecord);
		_logger.LogDebug("<<< Exiting InsertPollVotingRecordAsync(pollVotingRecord={PollVotingRecord})", pollVotingRecord);
    }

    /// <summary>
    /// Gets a poll voting records by parent answer
    /// </summary>
    /// <param name="pollAnswerId">Poll answer identifier</param>
    /// <returns>Poll answer</returns>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IPagedList<PollVotingRecord>> GetPollVotingRecordsByPollAnswerAsync(int pollAnswerId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetPollVotingRecordsByPollAnswerAsync(pollAnswerId={PollAnswerId},pageIndex={PageIndex},pageSize={PageSize})", pollAnswerId, pageIndex, pageSize);
        var query = _pollVotingRecordRepository.Table.Where(pa => pa.PollAnswerId == pollAnswerId);

		_logger.LogDebug("GetPollVotingRecordsByPollAnswerAsync(pollAnswerId={PollAnswerId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", pollAnswerId, pageIndex, pageSize, query);
        //return paged list of poll voting records
		_logger.LogDebug("<<< Exiting GetPollVotingRecordsByPollAnswerAsync(pollAnswerId={PollAnswerId},pageIndex={PageIndex},pageSize={PageSize})", pollAnswerId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion
}