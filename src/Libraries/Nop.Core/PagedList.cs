using Microsoft.Extensions.Logging;
namespace Nop.Core;

/// <summary>
/// Paged list
/// </summary>
/// <typeparam name="T">T</typeparam>
[Serializable]
public partial class PagedList<T> : List<T>, IPagedList<T>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="source">source</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="totalCount">Total count</param>
    public PagedList(IList<T> source, int pageIndex, int pageSize, int? totalCount = null)
    {
		_logger.LogError(">>> Entering PagedList(source={Source},pageIndex={PageIndex},pageSize={PageSize},totalCount={TotalCount})", source, pageIndex, pageSize, totalCount);
        //min allowed page size is 1
        pageSize = Math.Max(pageSize, 1);
		_logger.LogError("PagedList(source={Source},pageIndex={PageIndex},pageSize={PageSize},totalCount={TotalCount}): pageSize → {PageSize1}", source, pageIndex, pageSize, totalCount, pageSize);

        TotalCount = totalCount ?? source.Count;
		_logger.LogDebug("PagedList(source={Source},pageIndex={PageIndex},pageSize={PageSize},totalCount={TotalCount}): TotalCount → {TotalCount1}", source, pageIndex, pageSize, totalCount, TotalCount);
        TotalPages = TotalCount / pageSize;
		_logger.LogDebug("PagedList(source={Source},pageIndex={PageIndex},pageSize={PageSize},totalCount={TotalCount}): TotalPages → {TotalPages}", source, pageIndex, pageSize, totalCount, TotalPages);

        if (TotalCount % pageSize > 0)
            TotalPages++;

        PageSize = pageSize;
		_logger.LogDebug("PagedList(source={Source},pageIndex={PageIndex},pageSize={PageSize},totalCount={TotalCount}): PageSize → {PageSize1}", source, pageIndex, pageSize, totalCount, PageSize);
        PageIndex = pageIndex;
		_logger.LogDebug("PagedList(source={Source},pageIndex={PageIndex},pageSize={PageSize},totalCount={TotalCount}): PageIndex → {PageIndex1}", source, pageIndex, pageSize, totalCount, PageIndex);
        AddRange(totalCount != null ? source : source.Skip(pageIndex * pageSize).Take(pageSize));
		_logger.LogWarning("<<< Exiting PagedList(source={Source},pageIndex={PageIndex},pageSize={PageSize},totalCount={TotalCount})", source, pageIndex, pageSize, totalCount);
    }

    /// <summary>
    /// Page index
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Total count
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Has previous page
    /// </summary>
    public bool HasPreviousPage => PageIndex > 0;

    /// <summary>
    /// Has next page
    /// </summary>
    public bool HasNextPage => PageIndex + 1 < TotalPages;
}