using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.News;

/// <summary>
/// News service
/// </summary>
public partial class NewsService : INewsService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<NewsComment> _newsCommentRepository;
    protected readonly IRepository<NewsItem> _newsItemRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public NewsService(
        IRepository<NewsComment> newsCommentRepository,
        IRepository<NewsItem> newsItemRepository,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService)
    {
		_logger.LogError(">>> Entering NewsService(newsCommentRepository={NewsCommentRepository},newsItemRepository={NewsItemRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>)", newsCommentRepository, newsItemRepository, staticCacheManager);
        _newsCommentRepository = newsCommentRepository;
		_logger.LogWarning("<<< Exiting NewsService(newsCommentRepository={NewsCommentRepository},newsItemRepository={NewsItemRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>)", newsCommentRepository, newsItemRepository, staticCacheManager);
		_logger.LogDebug("NewsService(newsCommentRepository={NewsCommentRepository},newsItemRepository={NewsItemRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>): _newsCommentRepository → {_newsCommentRepository}", newsCommentRepository, newsItemRepository, staticCacheManager, _newsCommentRepository);
        _newsItemRepository = newsItemRepository;
		_logger.LogDebug("NewsService(newsCommentRepository={NewsCommentRepository},newsItemRepository={NewsItemRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>): _newsItemRepository → {_newsItemRepository}", newsCommentRepository, newsItemRepository, staticCacheManager, _newsItemRepository);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("NewsService(newsCommentRepository={NewsCommentRepository},newsItemRepository={NewsItemRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>): _staticCacheManager → {_staticCacheManager}", newsCommentRepository, newsItemRepository, staticCacheManager, _staticCacheManager);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("NewsService(newsCommentRepository={NewsCommentRepository},newsItemRepository={NewsItemRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", newsCommentRepository, newsItemRepository, staticCacheManager, _storeMappingService);

    }

    #endregion

    #region Methods

    #region News

    /// <summary>
    /// Deletes a news
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteNewsAsync(NewsItem newsItem)
    {
		_logger.LogError(">>> Entering DeleteNewsAsync(newsItem={NewsItem})", newsItem);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _newsItemRepository.DeleteAsync(newsItem);
		_logger.LogError("DeleteNewsAsync(newsItem)={NewsItem}: external service call took {Arg1} ms", newsItem, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteNewsAsync(newsItem={NewsItem})", newsItem);
    }

    /// <summary>
    /// Gets a news
    /// </summary>
    /// <param name="newsId">The news identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news
    /// </returns>
    public virtual async Task<NewsItem> GetNewsByIdAsync(int newsId)
    {
		_logger.LogError(">>> Entering GetNewsByIdAsync(newsId={NewsId})", newsId);
		_logger.LogError("GetNewsByIdAsync(newsId)={NewsId}: find query executing", newsId);
    _logger.LogWarning("<<< Exiting GetNewsByIdAsync(newsId={NewsId})", newsId);
        return await _newsItemRepository.GetByIdAsync(newsId, cache => default);
    }

    /// <summary>
    /// Gets all news
    /// </summary>
    /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="title">Filter by news item title</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news items
    /// </returns>
    public virtual async Task<IPagedList<NewsItem>> GetAllNewsAsync(int languageId = 0, int storeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string title = null)
    {
		_logger.LogError(">>> Entering GetAllNewsAsync(languageId={LanguageId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden},title={Title})", languageId, storeId, pageIndex, pageSize, showHidden, title);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var news = await _newsItemRepository.GetAllPagedAsync(async query =>
        {
            if (languageId > 0)
                query = query.Where(n => languageId == n.LanguageId);

			_logger.LogError("GetAllNewsAsync(languageId={LanguageId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden},title={Title}): query → {Query}", languageId, storeId, pageIndex, pageSize, showHidden, title, query);
            if (!string.IsNullOrEmpty(title))
                query = query.Where(n => n.Title.Contains(title));

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
                query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
            }

            return query.OrderByDescending(n => n.StartDateUtc ?? n.CreatedOnUtc);
        }, pageIndex, pageSize);
		_logger.LogError("GetAllNewsAsync(languageId,storeId,pageIndex,pageSize,showHidden,title)={LanguageId},{StoreId},{PageIndex},{PageSize},{ShowHidden},{Title}: find query executed in {Arg6} ms", languageId, storeId, pageIndex, pageSize, showHidden, title, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllNewsAsync(languageId={LanguageId},storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden},title={Title})", languageId, storeId, pageIndex, pageSize, showHidden, title);
        return news;
    }

    /// <summary>
    /// Inserts a news item
    /// </summary>
    /// <param name="news">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertNewsAsync(NewsItem news)
    {
		_logger.LogError(">>> Entering InsertNewsAsync(news={News})", news);
        await _newsItemRepository.InsertAsync(news);
		_logger.LogWarning("<<< Exiting InsertNewsAsync(news={News})", news);
    }

    /// <summary>
    /// Updates the news item
    /// </summary>
    /// <param name="news">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateNewsAsync(NewsItem news)
    {
		_logger.LogError(">>> Entering UpdateNewsAsync(news={News})", news);
        await _newsItemRepository.UpdateAsync(news);
		_logger.LogWarning("<<< Exiting UpdateNewsAsync(news={News})", news);
    }

    /// <summary>
    /// Get a value indicating whether a news item is available now (availability dates)
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <param name="dateTime">Datetime to check; pass null to use current date</param>
    /// <returns>Result</returns>
    public virtual bool IsNewsAvailable(NewsItem newsItem, DateTime? dateTime = null)
    {
		_logger.LogError(">>> Entering IsNewsAvailable(newsItem={NewsItem},dateTime={DateTime})", newsItem, dateTime);
        ArgumentNullException.ThrowIfNull(newsItem);

        if (newsItem.StartDateUtc.HasValue && newsItem.StartDateUtc.Value >= dateTime)
            return false;

        if (newsItem.EndDateUtc.HasValue && newsItem.EndDateUtc.Value <= dateTime)
            return false;

        return true;
    }
    #endregion

    #region News comments

    /// <summary>
    /// Gets all comments
    /// </summary>
    /// <param name="customerId">Customer identifier; 0 to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="newsItemId">News item ID; 0 or null to load all records</param>
    /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
    /// <param name="fromUtc">Item creation from; null to load all records</param>
    /// <param name="toUtc">Item creation to; null to load all records</param>
    /// <param name="commentText">Search comment text; null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the comments
    /// </returns>
    public virtual async Task<IList<NewsComment>> GetAllCommentsAsync(int customerId = 0, int storeId = 0, int? newsItemId = null,
        bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null)
    {
		_logger.LogError(">>> Entering GetAllCommentsAsync(customerId={CustomerId},storeId={StoreId},newsItemId={NewsItemId},approved={Approved},fromUtc={FromUtc},toUtc={ToUtc},commentText={CommentText})", customerId, storeId, newsItemId, approved, fromUtc, toUtc, commentText);
        return await _newsCommentRepository.GetAllAsync(query =>
        {
            if (approved.HasValue)
                query = query.Where(comment => comment.IsApproved == approved);
				_logger.LogError("GetAllCommentsAsync(customerId,storeId,newsItemId,approved,fromUtc,toUtc,commentText)={CustomerId},{StoreId},{NewsItemId},{Approved},{FromUtc},{ToUtc},{CommentText}: find query executing", customerId, storeId, newsItemId, approved, fromUtc, toUtc, commentText);

            if (newsItemId > 0)
                query = query.Where(comment => comment.NewsItemId == newsItemId);

            if (customerId > 0)
                query = query.Where(comment => comment.CustomerId == customerId);

            if (storeId > 0)
                query = query.Where(comment => comment.StoreId == storeId);

            if (fromUtc.HasValue)
                query = query.Where(comment => fromUtc.Value <= comment.CreatedOnUtc);

            if (toUtc.HasValue)
                query = query.Where(comment => toUtc.Value >= comment.CreatedOnUtc);

            if (!string.IsNullOrEmpty(commentText))
                query = query.Where(
                    c => c.CommentText.Contains(commentText) || c.CommentTitle.Contains(commentText));

            query = query.OrderBy(nc => nc.CreatedOnUtc);

            return query;
        });
    }

    /// <summary>
    /// Gets a news comment
    /// </summary>
    /// <param name="newsCommentId">News comment identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comment
    /// </returns>
    public virtual async Task<NewsComment> GetNewsCommentByIdAsync(int newsCommentId)
    {
		_logger.LogError(">>> Entering GetNewsCommentByIdAsync(newsCommentId={NewsCommentId})", newsCommentId);
		_logger.LogError("GetNewsCommentByIdAsync(newsCommentId)={NewsCommentId}: find query executing", newsCommentId);
    _logger.LogWarning("<<< Exiting GetNewsCommentByIdAsync(newsCommentId={NewsCommentId})", newsCommentId);
        return await _newsCommentRepository.GetByIdAsync(newsCommentId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Get news comments by identifiers
    /// </summary>
    /// <param name="commentIds">News comment identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comments
    /// </returns>
    public virtual async Task<IList<NewsComment>> GetNewsCommentsByIdsAsync(int[] commentIds)
    {
		_logger.LogError(">>> Entering GetNewsCommentsByIdsAsync(commentIds={CommentIds})", commentIds);
        _logger.LogWarning("<<< Exiting GetNewsCommentsByIdsAsync(commentIds={CommentIds})", commentIds);
        return await _newsCommentRepository.GetByIdsAsync(commentIds);
    }

    /// <summary>
    /// Get the count of news comments
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="isApproved">A value indicating whether to count only approved or not approved comments; pass null to get number of all comments</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of news comments
    /// </returns>
    public virtual async Task<int> GetNewsCommentsCountAsync(NewsItem newsItem, int storeId = 0, bool? isApproved = null)
    {
		_logger.LogError(">>> Entering GetNewsCommentsCountAsync(newsItem={NewsItem},storeId={StoreId},isApproved={IsApproved})", newsItem, storeId, isApproved);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = _newsCommentRepository.Table.Where(comment => comment.NewsItemId == newsItem.Id);

		_logger.LogError("GetNewsCommentsCountAsync(newsItem={NewsItem},storeId={StoreId},isApproved={IsApproved}): query → {Query}", newsItem, storeId, isApproved, query);
        if (storeId > 0)
            query = query.Where(comment => comment.StoreId == storeId);

        if (isApproved.HasValue)
            query = query.Where(comment => comment.IsApproved == isApproved.Value);

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopNewsDefaults.NewsCommentsNumberCacheKey, newsItem, storeId, isApproved);
		_logger.LogError("GetNewsCommentsCountAsync(newsItem={NewsItem},storeId={StoreId},isApproved={IsApproved}): cacheKey → {CacheKey}", newsItem, storeId, isApproved, cacheKey);
		_logger.LogError("GetNewsCommentsCountAsync(newsItem,storeId,isApproved)={NewsItem},{StoreId},{IsApproved}: find query executing", newsItem, storeId, isApproved);
		_logger.LogError("GetNewsCommentsCountAsync(newsItem,storeId,isApproved)={NewsItem},{StoreId},{IsApproved}: external service call took {Arg3} ms", newsItem, storeId, isApproved, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetNewsCommentsCountAsync(newsItem={NewsItem},storeId={StoreId},isApproved={IsApproved})", newsItem, storeId, isApproved);
        return await _staticCacheManager.GetAsync(cacheKey, async () => await query.CountAsync());
    }

    /// <summary>
    /// Deletes a news comment
    /// </summary>
    /// <param name="newsComment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteNewsCommentAsync(NewsComment newsComment)
    {
		_logger.LogError(">>> Entering DeleteNewsCommentAsync(newsComment={NewsComment})", newsComment);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _newsCommentRepository.DeleteAsync(newsComment);
		_logger.LogError("DeleteNewsCommentAsync(newsComment)={NewsComment}: external service call took {Arg1} ms", newsComment, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteNewsCommentAsync(newsComment={NewsComment})", newsComment);
    }

    /// <summary>
    /// Deletes a news comments
    /// </summary>
    /// <param name="newsComments">News comments</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteNewsCommentsAsync(IList<NewsComment> newsComments)
    {
		_logger.LogError(">>> Entering DeleteNewsCommentsAsync(newsComments={NewsComments})", newsComments);
        ArgumentNullException.ThrowIfNull(newsComments);

        foreach (var newsComment in newsComments)
            await DeleteNewsCommentAsync(newsComment);
			_logger.LogWarning("<<< Exiting DeleteNewsCommentsAsync(newsComments={NewsComments})", newsComments);
    }

    /// <summary>
    /// Inserts a news comment
    /// </summary>
    /// <param name="comment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertNewsCommentAsync(NewsComment comment)
    {
		_logger.LogError(">>> Entering InsertNewsCommentAsync(comment={Comment})", comment);
        await _newsCommentRepository.InsertAsync(comment);
		_logger.LogWarning("<<< Exiting InsertNewsCommentAsync(comment={Comment})", comment);
    }

    /// <summary>
    /// Update a news comment
    /// </summary>
    /// <param name="comment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateNewsCommentAsync(NewsComment comment)
    {
		_logger.LogError(">>> Entering UpdateNewsCommentAsync(comment={Comment})", comment);
        await _newsCommentRepository.UpdateAsync(comment);
		_logger.LogWarning("<<< Exiting UpdateNewsCommentAsync(comment={Comment})", comment);
    }

    #endregion

    #endregion
}