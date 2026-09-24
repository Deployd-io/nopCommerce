using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Data;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Blogs;

/// <summary>
/// Blog service
/// </summary>
public partial class BlogService : IBlogService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<BlogComment> _blogCommentRepository;
    protected readonly IRepository<BlogPost> _blogPostRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public BlogService(
        IRepository<BlogComment> blogCommentRepository,
        IRepository<BlogPost> blogPostRepository,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService)
    {
		_logger.LogError(">>> Entering BlogService(blogCommentRepository={BlogCommentRepository},blogPostRepository={BlogPostRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>)", blogCommentRepository, blogPostRepository, staticCacheManager);
        _blogCommentRepository = blogCommentRepository;
        _blogPostRepository = blogPostRepository;
		_logger.LogDebug("BlogService(blogCommentRepository={BlogCommentRepository},blogPostRepository={BlogPostRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>): _blogPostRepository → {_blogPostRepository}", blogCommentRepository, blogPostRepository, staticCacheManager, _blogPostRepository);
		_logger.LogDebug("BlogService(blogCommentRepository={BlogCommentRepository},blogPostRepository={BlogPostRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>): _blogCommentRepository → {_blogCommentRepository}", blogCommentRepository, blogPostRepository, staticCacheManager, _blogCommentRepository);
    _logger.LogWarning("<<< Exiting BlogService(blogCommentRepository={BlogCommentRepository},blogPostRepository={BlogPostRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>)", blogCommentRepository, blogPostRepository, staticCacheManager);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("BlogService(blogCommentRepository={BlogCommentRepository},blogPostRepository={BlogPostRepository},staticCacheManager={StaticCacheManager},storeMappingService=<redacted>): _staticCacheManager → {_staticCacheManager}", blogCommentRepository, blogPostRepository, staticCacheManager, _staticCacheManager);
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Methods

    #region Blog posts

    /// <summary>
    /// Deletes a blog post
    /// </summary>
    /// <param name="blogPost">Blog post</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteBlogPostAsync(BlogPost blogPost)
    {
		_logger.LogError(">>> Entering DeleteBlogPostAsync(blogPost={BlogPost})", blogPost);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _blogPostRepository.DeleteAsync(blogPost);
		_logger.LogError("DeleteBlogPostAsync(blogPost)={BlogPost}: external service call took {Arg1} ms", blogPost, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteBlogPostAsync(blogPost={BlogPost})", blogPost);
    }

    /// <summary>
    /// Gets a blog post
    /// </summary>
    /// <param name="blogPostId">Blog post identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post
    /// </returns>
    public virtual async Task<BlogPost> GetBlogPostByIdAsync(int blogPostId)
    {
		_logger.LogError(">>> Entering GetBlogPostByIdAsync(blogPostId={BlogPostId})", blogPostId);
		_logger.LogError("GetBlogPostByIdAsync(blogPostId)={BlogPostId}: find query executing", blogPostId);
    _logger.LogWarning("<<< Exiting GetBlogPostByIdAsync(blogPostId={BlogPostId})", blogPostId);
        return await _blogPostRepository.GetByIdAsync(blogPostId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets all blog posts
    /// </summary>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
    /// <param name="dateFrom">Filter by created date; null if you want to get all records</param>
    /// <param name="dateTo">Filter by created date; null if you want to get all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="title">Filter by blog post title</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog posts
    /// </returns>
    public virtual async Task<IPagedList<BlogPost>> GetAllBlogPostsAsync(int storeId = 0, int languageId = 0,
        DateTime? dateFrom = null, DateTime? dateTo = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string title = null)
    {
		_logger.LogError(">>> Entering GetAllBlogPostsAsync(storeId={StoreId},languageId={LanguageId},dateFrom={DateFrom},dateTo={DateTo},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden},title={Title})", storeId, languageId, dateFrom, dateTo, pageIndex, pageSize, showHidden, title);
        return await _blogPostRepository.GetAllPagedAsync(async query =>
        {
			_logger.LogWarning("<<< Exiting GetAllBlogPostsAsync(storeId={StoreId},languageId={LanguageId},dateFrom={DateFrom},dateTo={DateTo},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden},title={Title})", storeId, languageId, dateFrom, dateTo, pageIndex, pageSize, showHidden, title);
            if (dateFrom.HasValue)
                query = query.Where(b => dateFrom.Value <= (b.StartDateUtc ?? b.CreatedOnUtc));
				_logger.LogError("GetAllBlogPostsAsync(storeId,languageId,dateFrom,dateTo,pageIndex,pageSize,showHidden,title)={StoreId},{LanguageId},{DateFrom},{DateTo},{PageIndex},{PageSize},{ShowHidden},{Title}: find query executing", storeId, languageId, dateFrom, dateTo, pageIndex, pageSize, showHidden, title);

            if (dateTo.HasValue)
                query = query.Where(b => dateTo.Value >= (b.StartDateUtc ?? b.CreatedOnUtc));

            if (languageId > 0)
                query = query.Where(b => languageId == b.LanguageId);

            if (!string.IsNullOrEmpty(title))
                query = query.Where(b => b.Title.Contains(title));

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            if (!showHidden)
            {
                query = query.Where(b => !b.StartDateUtc.HasValue || b.StartDateUtc <= DateTime.UtcNow);
                query = query.Where(b => !b.EndDateUtc.HasValue || b.EndDateUtc >= DateTime.UtcNow);
            }

            query = query.OrderByDescending(b => b.StartDateUtc ?? b.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Gets all blog posts
    /// </summary>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
    /// <param name="tag">Tag</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog posts
    /// </returns>
    public virtual async Task<IPagedList<BlogPost>> GetAllBlogPostsByTagAsync(int storeId = 0,
        int languageId = 0, string tag = "",
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
    {
		_logger.LogError(">>> Entering GetAllBlogPostsByTagAsync(storeId={StoreId},languageId={LanguageId},tag={Tag},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", storeId, languageId, tag, pageIndex, pageSize, showHidden);
        tag = tag.Trim();
		_logger.LogError("GetAllBlogPostsByTagAsync(storeId={StoreId},languageId={LanguageId},tag={Tag},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): tag → {Tag1}", storeId, languageId, tag, pageIndex, pageSize, showHidden, tag);

        //we load all records and only then filter them by tag
        var blogPostsAll = await GetAllBlogPostsAsync(storeId: storeId, languageId: languageId, showHidden: showHidden);
		_logger.LogError("GetAllBlogPostsByTagAsync(storeId={StoreId},languageId={LanguageId},tag={Tag},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): blogPostsAll → {BlogPostsAll}", storeId, languageId, tag, pageIndex, pageSize, showHidden, blogPostsAll);
        var taggedBlogPosts = new List<BlogPost>();
		_logger.LogError("GetAllBlogPostsByTagAsync(storeId={StoreId},languageId={LanguageId},tag={Tag},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): taggedBlogPosts → {TaggedBlogPosts}", storeId, languageId, tag, pageIndex, pageSize, showHidden, taggedBlogPosts);
        foreach (var blogPost in blogPostsAll)
        {
            var tags = await ParseTagsAsync(blogPost);
			_logger.LogError("GetAllBlogPostsByTagAsync(storeId={StoreId},languageId={LanguageId},tag={Tag},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): tags → {Tags}", storeId, languageId, tag, pageIndex, pageSize, showHidden, tags);
            if (!string.IsNullOrEmpty(tags.FirstOrDefault(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase))))
                taggedBlogPosts.Add(blogPost);
        }

        //server-side paging
        var result = new PagedList<BlogPost>(taggedBlogPosts, pageIndex, pageSize);
		_logger.LogError("GetAllBlogPostsByTagAsync(storeId={StoreId},languageId={LanguageId},tag={Tag},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): result → {Result}", storeId, languageId, tag, pageIndex, pageSize, showHidden, result);
		_logger.LogWarning("<<< Exiting GetAllBlogPostsByTagAsync(storeId={StoreId},languageId={LanguageId},tag={Tag},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", storeId, languageId, tag, pageIndex, pageSize, showHidden);
        return result;
    }

    /// <summary>
    /// Gets all blog post tags
    /// </summary>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <param name="languageId">Language identifier. 0 if you want to get all blog posts</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post tags
    /// </returns>
    public virtual async Task<IList<BlogPostTag>> GetAllBlogPostTagsAsync(int storeId, int languageId, bool showHidden = false)
    {
		_logger.LogError(">>> Entering GetAllBlogPostTagsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden})", storeId, languageId, showHidden);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopBlogsDefaults.BlogTagsCacheKey, languageId, storeId, showHidden);

		_logger.LogError("GetAllBlogPostTagsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden}): cacheKey → {CacheKey}", storeId, languageId, showHidden, cacheKey);
        var blogPostTags = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var rezBlogPostTags = new List<BlogPostTag>();
			_logger.LogError("GetAllBlogPostTagsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden}): rezBlogPostTags → {RezBlogPostTags}", storeId, languageId, showHidden, rezBlogPostTags);

            var blogPosts = await GetAllBlogPostsAsync(storeId, languageId, showHidden: showHidden);
			_logger.LogError("GetAllBlogPostTagsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden}): blogPosts → {BlogPosts}", storeId, languageId, showHidden, blogPosts);

            foreach (var blogPost in blogPosts)
            {
                var tags = await ParseTagsAsync(blogPost);
				_logger.LogError("GetAllBlogPostTagsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden}): tags → {Tags}", storeId, languageId, showHidden, tags);
                foreach (var tag in tags)
                {
                    var foundBlogPostTag = rezBlogPostTags.Find(bpt =>
                        bpt.Name.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
                    if (foundBlogPostTag == null)
                    {
                        foundBlogPostTag = new BlogPostTag
                        {
                            Name = tag,
                            BlogPostCount = 1
                        };
                        rezBlogPostTags.Add(foundBlogPostTag);
                    }
                    else
                        foundBlogPostTag.BlogPostCount++;
                }
            }

            return rezBlogPostTags;
        });

		_logger.LogError("GetAllBlogPostTagsAsync(storeId,languageId,showHidden)={StoreId},{LanguageId},{ShowHidden}: external service call took {Arg3} ms", storeId, languageId, showHidden, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetAllBlogPostTagsAsync(storeId={StoreId},languageId={LanguageId},showHidden={ShowHidden})", storeId, languageId, showHidden);
		__logStart = System.Diagnostics.Stopwatch.StartNew();
        return blogPostTags;
    }

    /// <summary>
    /// Inserts a blog post
    /// </summary>
    /// <param name="blogPost">Blog post</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertBlogPostAsync(BlogPost blogPost)
    {
		_logger.LogError(">>> Entering InsertBlogPostAsync(blogPost={BlogPost})", blogPost);
        await _blogPostRepository.InsertAsync(blogPost);
		_logger.LogWarning("<<< Exiting InsertBlogPostAsync(blogPost={BlogPost})", blogPost);
    }

    /// <summary>
    /// Updates the blog post
    /// </summary>
    /// <param name="blogPost">Blog post</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateBlogPostAsync(BlogPost blogPost)
    {
		_logger.LogError(">>> Entering UpdateBlogPostAsync(blogPost={BlogPost})", blogPost);
        await _blogPostRepository.UpdateAsync(blogPost);
		_logger.LogWarning("<<< Exiting UpdateBlogPostAsync(blogPost={BlogPost})", blogPost);
    }

    /// <summary>
    /// Returns all posts published between the two dates.
    /// </summary>
    /// <param name="blogPosts">Source</param>
    /// <param name="dateFrom">Date from</param>
    /// <param name="dateTo">Date to</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filtered posts
    /// </returns>
    public virtual async Task<IList<BlogPost>> GetPostsByDateAsync(IList<BlogPost> blogPosts, DateTime dateFrom, DateTime dateTo)
    {
		_logger.LogError(">>> Entering GetPostsByDateAsync(blogPosts={BlogPosts},dateFrom={DateFrom},dateTo={DateTo})", blogPosts, dateFrom, dateTo);
        ArgumentNullException.ThrowIfNull(blogPosts);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var rez = await blogPosts
            .Where(p => dateFrom.Date <= (p.StartDateUtc ?? p.CreatedOnUtc) && (p.StartDateUtc ?? p.CreatedOnUtc).Date <= dateTo)
            .ToListAsync();
			_logger.LogError("GetPostsByDateAsync(blogPosts,dateFrom,dateTo)={BlogPosts},{DateFrom},{DateTo}: database query executed in {Arg3} ms", blogPosts, dateFrom, dateTo, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetPostsByDateAsync(blogPosts={BlogPosts},dateFrom={DateFrom},dateTo={DateTo})", blogPosts, dateFrom, dateTo);
        return rez;
    }

    /// <summary>
    /// Parse tags
    /// </summary>
    /// <param name="blogPost">Blog post</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ags
    /// </returns>
    public virtual async Task<IList<string>> ParseTagsAsync(BlogPost blogPost)
    {
		_logger.LogError(">>> Entering ParseTagsAsync(blogPost={BlogPost})", blogPost);
        ArgumentNullException.ThrowIfNull(blogPost);

        if (blogPost.Tags == null)
            return new List<string>();

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var tags = await blogPost.Tags.Split(_separator, StringSplitOptions.RemoveEmptyEntries)
            .Select(tag => tag.Trim())
            .Where(tag => !string.IsNullOrEmpty(tag))
            .ToListAsync();
			_logger.LogError("ParseTagsAsync(blogPost)={BlogPost}: database query executed in {Arg1} ms", blogPost, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting ParseTagsAsync(blogPost={BlogPost})", blogPost);
        return tags;
    }

    /// <summary>
    /// Get a value indicating whether a blog post is available now (availability dates)
    /// </summary>
    /// <param name="blogPost">Blog post</param>
    /// <param name="dateTime">Datetime to check; pass null to use current date</param>
    /// <returns>Result</returns>
    public virtual bool BlogPostIsAvailable(BlogPost blogPost, DateTime? dateTime = null)
    {
		_logger.LogError(">>> Entering BlogPostIsAvailable(blogPost={BlogPost},dateTime={DateTime})", blogPost, dateTime);
        ArgumentNullException.ThrowIfNull(blogPost);

        if (blogPost.StartDateUtc.HasValue && blogPost.StartDateUtc.Value >= (dateTime ?? DateTime.UtcNow))
            return false;

        if (blogPost.EndDateUtc.HasValue && blogPost.EndDateUtc.Value <= (dateTime ?? DateTime.UtcNow))
            return false;

        return true;
    }

    #endregion

    #region Blog comments

    /// <summary>
    /// Gets all comments
    /// </summary>
    /// <param name="customerId">Customer identifier; 0 to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="blogPostId">Blog post ID; 0 or null to load all records</param>
    /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
    /// <param name="fromUtc">Item creation from; null to load all records</param>
    /// <param name="toUtc">Item creation to; null to load all records</param>
    /// <param name="commentText">Search comment text; null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the comments
    /// </returns>
    public virtual async Task<IList<BlogComment>> GetAllCommentsAsync(int customerId = 0, int storeId = 0, int? blogPostId = null,
        bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null)
    {
		_logger.LogError(">>> Entering GetAllCommentsAsync(customerId={CustomerId},storeId={StoreId},blogPostId={BlogPostId},approved={Approved},fromUtc={FromUtc},toUtc={ToUtc},commentText={CommentText})", customerId, storeId, blogPostId, approved, fromUtc, toUtc, commentText);
        return await _blogCommentRepository.GetAllAsync(query =>
        {
            if (approved.HasValue)
                query = query.Where(comment => comment.IsApproved == approved);
				_logger.LogError("GetAllCommentsAsync(customerId,storeId,blogPostId,approved,fromUtc,toUtc,commentText)={CustomerId},{StoreId},{BlogPostId},{Approved},{FromUtc},{ToUtc},{CommentText}: find query executing", customerId, storeId, blogPostId, approved, fromUtc, toUtc, commentText);

            if (blogPostId > 0)
                query = query.Where(comment => comment.BlogPostId == blogPostId);

            if (customerId > 0)
                query = query.Where(comment => comment.CustomerId == customerId);

            if (storeId > 0)
                query = query.Where(comment => comment.StoreId == storeId);

            if (fromUtc.HasValue)
                query = query.Where(comment => fromUtc.Value <= comment.CreatedOnUtc);

            if (toUtc.HasValue)
                query = query.Where(comment => toUtc.Value >= comment.CreatedOnUtc);

            if (!string.IsNullOrEmpty(commentText))
                query = query.Where(c => c.CommentText.Contains(commentText));

            query = query.OrderBy(comment => comment.CreatedOnUtc);

		_logger.LogWarning("<<< Exiting GetAllCommentsAsync(customerId={CustomerId},storeId={StoreId},blogPostId={BlogPostId},approved={Approved},fromUtc={FromUtc},toUtc={ToUtc},commentText={CommentText})", customerId, storeId, blogPostId, approved, fromUtc, toUtc, commentText);
            return query;
        });
    }

    /// <summary>
    /// Gets a blog comment
    /// </summary>
    /// <param name="blogCommentId">Blog comment identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog comment
    /// </returns>
    public virtual async Task<BlogComment> GetBlogCommentByIdAsync(int blogCommentId)
    {
		_logger.LogError(">>> Entering GetBlogCommentByIdAsync(blogCommentId={BlogCommentId})", blogCommentId);
		_logger.LogError("GetBlogCommentByIdAsync(blogCommentId)={BlogCommentId}: find query executing", blogCommentId);
    _logger.LogWarning("<<< Exiting GetBlogCommentByIdAsync(blogCommentId={BlogCommentId})", blogCommentId);
        return await _blogCommentRepository.GetByIdAsync(blogCommentId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Get blog comments by identifiers
    /// </summary>
    /// <param name="commentIds">Blog comment identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog comments
    /// </returns>
    public virtual async Task<IList<BlogComment>> GetBlogCommentsByIdsAsync(int[] commentIds)
    {
		_logger.LogError(">>> Entering GetBlogCommentsByIdsAsync(commentIds={CommentIds})", commentIds);
        _logger.LogWarning("<<< Exiting GetBlogCommentsByIdsAsync(commentIds={CommentIds})", commentIds);
        return await _blogCommentRepository.GetByIdsAsync(commentIds);
    }

    /// <summary>
    /// Get the count of blog comments
    /// </summary>
    /// <param name="blogPost">Blog post</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="isApproved">A value indicating whether to count only approved or not approved comments; pass null to get number of all comments</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of blog comments
    /// </returns>
    public virtual async Task<int> GetBlogCommentsCountAsync(BlogPost blogPost, int storeId = 0, bool? isApproved = null)
    {
		_logger.LogError(">>> Entering GetBlogCommentsCountAsync(blogPost={BlogPost},storeId={StoreId},isApproved={IsApproved})", blogPost, storeId, isApproved);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var query = _blogCommentRepository.Table.Where(comment => comment.BlogPostId == blogPost.Id);

		_logger.LogError("GetBlogCommentsCountAsync(blogPost={BlogPost},storeId={StoreId},isApproved={IsApproved}): query → {Query}", blogPost, storeId, isApproved, query);
        if (storeId > 0)
            query = query.Where(comment => comment.StoreId == storeId);

        if (isApproved.HasValue)
            query = query.Where(comment => comment.IsApproved == isApproved.Value);

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopBlogsDefaults.BlogCommentsNumberCacheKey, blogPost, storeId, isApproved);
		_logger.LogError("GetBlogCommentsCountAsync(blogPost={BlogPost},storeId={StoreId},isApproved={IsApproved}): cacheKey → {CacheKey}", blogPost, storeId, isApproved, cacheKey);
		_logger.LogError("GetBlogCommentsCountAsync(blogPost,storeId,isApproved)={BlogPost},{StoreId},{IsApproved}: find query executing", blogPost, storeId, isApproved);
		_logger.LogError("GetBlogCommentsCountAsync(blogPost,storeId,isApproved)={BlogPost},{StoreId},{IsApproved}: external service call took {Arg3} ms", blogPost, storeId, isApproved, __logStart.ElapsedMilliseconds);

        return await _staticCacheManager.GetAsync(cacheKey, async () => await query.CountAsync());
    }

    /// <summary>
    /// Deletes a blog comment
    /// </summary>
    /// <param name="blogComment">Blog comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteBlogCommentAsync(BlogComment blogComment)
    {
		_logger.LogError(">>> Entering DeleteBlogCommentAsync(blogComment={BlogComment})", blogComment);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _blogCommentRepository.DeleteAsync(blogComment);
		_logger.LogError("DeleteBlogCommentAsync(blogComment)={BlogComment}: external service call took {Arg1} ms", blogComment, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteBlogCommentAsync(blogComment={BlogComment})", blogComment);
    }

    /// <summary>
    /// Deletes blog comments
    /// </summary>
    /// <param name="blogComments">Blog comments</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteBlogCommentsAsync(IList<BlogComment> blogComments)
    {
		_logger.LogError(">>> Entering DeleteBlogCommentsAsync(blogComments={BlogComments})", blogComments);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _blogCommentRepository.DeleteAsync(blogComments);
		_logger.LogError("DeleteBlogCommentsAsync(blogComments)={BlogComments}: external service call took {Arg1} ms", blogComments, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteBlogCommentsAsync(blogComments={BlogComments})", blogComments);
    }

    /// <summary>
    /// Inserts a blog comment
    /// </summary>
    /// <param name="blogComment">Blog comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertBlogCommentAsync(BlogComment blogComment)
    {
		_logger.LogError(">>> Entering InsertBlogCommentAsync(blogComment={BlogComment})", blogComment);
        await _blogCommentRepository.InsertAsync(blogComment);
		_logger.LogWarning("<<< Exiting InsertBlogCommentAsync(blogComment={BlogComment})", blogComment);
    }

    /// <summary>
    /// Update a blog comment
    /// </summary>
    /// <param name="blogComment">Blog comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateBlogCommentAsync(BlogComment blogComment)
    {
		_logger.LogError(">>> Entering UpdateBlogCommentAsync(blogComment={BlogComment})", blogComment);
        await _blogCommentRepository.UpdateAsync(blogComment);
		_logger.LogWarning("<<< Exiting UpdateBlogCommentAsync(blogComment={BlogComment})", blogComment);
    }

    #endregion

    #endregion
}