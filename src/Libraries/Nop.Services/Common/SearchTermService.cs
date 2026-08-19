using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common;

/// <summary>
/// Search term service
/// </summary>
public partial class SearchTermService : ISearchTermService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<SearchTerm> _searchTermRepository;

    #endregion

    #region Ctor

    public SearchTermService(IRepository<SearchTerm> searchTermRepository)
    {
		_logger.LogDebug(">>> Entering SearchTermService(searchTermRepository={SearchTermRepository})", searchTermRepository);
        _searchTermRepository = searchTermRepository;
		_logger.LogDebug("SearchTermService(searchTermRepository={SearchTermRepository}): _searchTermRepository → {_searchTermRepository}", searchTermRepository, _searchTermRepository);
		_logger.LogDebug("<<< Exiting SearchTermService(searchTermRepository={SearchTermRepository})", searchTermRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a search term record by keyword
    /// </summary>
    /// <param name="keyword">Search term keyword</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search term
    /// </returns>
    public virtual async Task<SearchTerm> GetSearchTermByKeywordAsync(string keyword, int storeId)
    {
		_logger.LogDebug(">>> Entering GetSearchTermByKeywordAsync(keyword={Keyword},storeId={StoreId})", keyword, storeId);
        if (string.IsNullOrEmpty(keyword))
            return null;

        var query = from st in _searchTermRepository.Table
            where st.Keyword == keyword && st.StoreId == storeId
            orderby st.Id
            select st;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var searchTerm = await query.FirstOrDefaultAsync();

		_logger.LogDebug("GetSearchTermByKeywordAsync(keyword={Keyword},storeId={StoreId}): searchTerm → {SearchTerm}", keyword, storeId, searchTerm);
		_logger.LogInformation("GetSearchTermByKeywordAsync(keyword,storeId)={Keyword},{StoreId}: database query executed in {Arg2} ms", keyword, storeId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetSearchTermByKeywordAsync(keyword={Keyword},storeId={StoreId})", keyword, storeId);
        return searchTerm;
    }

    /// <summary>
    /// Gets a search term statistics
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a list search term report lines
    /// </returns>
    public virtual async Task<IPagedList<SearchTermReportLine>> GetStatsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
        var query = (from st in _searchTermRepository.Table
                group st by st.Keyword into groupedResult
                select new
                {
                    Keyword = groupedResult.Key,
                    Count = groupedResult.Sum(o => o.Count)
                })
            .OrderByDescending(m => m.Count)
            .Select(r => new SearchTermReportLine
            {
					_logger.LogDebug("GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize}): Count → {Count}", pageIndex, pageSize, Count);
					_logger.LogDebug("GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize}): Keyword → {Keyword}", pageIndex, pageSize, Keyword);
                Keyword = r.Keyword,
                Count = r.Count
            });

        var result = await query.ToPagedListAsync(pageIndex, pageSize);
		_logger.LogDebug("GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize}): result → {Result}", pageIndex, pageSize, result);

		_logger.LogDebug("<<< Exiting GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
        return result;
    }

    /// <summary>
    /// Inserts a search term record
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSearchTermAsync(SearchTerm searchTerm)
    {
		_logger.LogDebug(">>> Entering InsertSearchTermAsync(searchTerm={SearchTerm})", searchTerm);
        await _searchTermRepository.InsertAsync(searchTerm);
		_logger.LogDebug("<<< Exiting InsertSearchTermAsync(searchTerm={SearchTerm})", searchTerm);
    }

    /// <summary>
    /// Updates the search term record
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSearchTermAsync(SearchTerm searchTerm)
    {
		_logger.LogDebug(">>> Entering UpdateSearchTermAsync(searchTerm={SearchTerm})", searchTerm);
        await _searchTermRepository.UpdateAsync(searchTerm);
		_logger.LogDebug("<<< Exiting UpdateSearchTermAsync(searchTerm={SearchTerm})", searchTerm);
    }

    #endregion
}