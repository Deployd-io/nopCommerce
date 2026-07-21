using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;

namespace Nop.Services.Common;

/// <summary>
/// Search term service
/// </summary>
public partial class SearchTermService : ISearchTermService
{
    #region Fields

    protected readonly IRepository<SearchTerm> _searchTermRepository;

    #endregion

    #region Ctor

    public SearchTermService(IRepository<SearchTerm> searchTermRepository)
    {
		logger.LogDebug(">>> Entering SearchTermService(searchTermRepository={SearchTermRepository})", searchTermRepository);
        _searchTermRepository = searchTermRepository;
		logger.LogDebug("<<< Exiting SearchTermService(searchTermRepository={SearchTermRepository})", searchTermRepository);
		logger.LogDebug("SearchTermService(searchTermRepository={SearchTermRepository}): _searchTermRepository → {_searchTermRepository}", searchTermRepository, _searchTermRepository);
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
		logger.LogDebug(">>> Entering GetSearchTermByKeywordAsync(keyword={Keyword},storeId={StoreId})", keyword, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(keyword))
            return null;

        var query = from st in _searchTermRepository.Table
            where st.Keyword == keyword && st.StoreId == storeId
            orderby st.Id
            select st;
        var searchTerm = await query.FirstOrDefaultAsync();

		logger.LogDebug("GetSearchTermByKeywordAsync(keyword={Keyword},storeId={StoreId}): searchTerm → {SearchTerm}", keyword, storeId, searchTerm);
		logger.LogDebug("<<< Exiting GetSearchTermByKeywordAsync(keyword={Keyword},storeId={StoreId})", keyword, storeId);
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
		logger.LogDebug(">>> Entering GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
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
					logger.LogDebug("GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize}): Keyword → {Keyword}", pageIndex, pageSize, Keyword);
					logger.LogDebug("GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize}): Count → {Count}", pageIndex, pageSize, Count);
                Keyword = r.Keyword,
                Count = r.Count
            });

        var result = await query.ToPagedListAsync(pageIndex, pageSize);

		logger.LogDebug("GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize}): result → {Result}", pageIndex, pageSize, result);
		logger.LogDebug("<<< Exiting GetStatsAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
        return result;
    }

    /// <summary>
    /// Inserts a search term record
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSearchTermAsync(SearchTerm searchTerm)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _searchTermRepository.InsertAsync(searchTerm);
    }

    /// <summary>
    /// Updates the search term record
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSearchTermAsync(SearchTerm searchTerm)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _searchTermRepository.UpdateAsync(searchTerm);
    }

    #endregion
}
