using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Html;
using Nop.Services.Messages;
using Nop.Services.Seo;

namespace Nop.Services.Forums;

/// <summary>
/// Forum service
/// </summary>
public partial class ForumService : IForumService
{
    #region Fields

    protected readonly ForumSettings _forumSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<Forum> _forumRepository;
    protected readonly IRepository<ForumGroup> _forumGroupRepository;
    protected readonly IRepository<ForumPost> _forumPostRepository;
    protected readonly IRepository<ForumPostVote> _forumPostVoteRepository;
    protected readonly IRepository<ForumSubscription> _forumSubscriptionRepository;
    protected readonly IRepository<ForumTopic> _forumTopicRepository;
    protected readonly IRepository<PrivateMessage> _forumPrivateMessageRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly SeoSettings _seoSettings;

    #endregion

    #region Ctor

    public ForumService(ForumSettings forumSettings,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IHtmlFormatter htmlFormatter,
        IRepository<Customer> customerRepository,
        IRepository<Forum> forumRepository,
        IRepository<ForumGroup> forumGroupRepository,
        IRepository<ForumPost> forumPostRepository,
        IRepository<ForumPostVote> forumPostVoteRepository,
        IRepository<ForumSubscription> forumSubscriptionRepository,
        IRepository<ForumTopic> forumTopicRepository,
        IRepository<PrivateMessage> forumPrivateMessageRepository,
        IStaticCacheManager staticCacheManager,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        SeoSettings seoSettings)
    {
        _forumSettings = forumSettings;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _htmlFormatter = htmlFormatter;
        _customerRepository = customerRepository;
        _forumRepository = forumRepository;
        _forumGroupRepository = forumGroupRepository;
        _forumPostRepository = forumPostRepository;
        _forumPostVoteRepository = forumPostVoteRepository;
        _forumSubscriptionRepository = forumSubscriptionRepository;
        _forumTopicRepository = forumTopicRepository;
        _forumPrivateMessageRepository = forumPrivateMessageRepository;
        _staticCacheManager = staticCacheManager;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _seoSettings = seoSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Update forum stats
    /// </summary>
    /// <param name="forumId">The forum identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateForumStatsAsync(int forumId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (forumId == 0)
            return;

        var forum = await GetForumByIdAsync(forumId);
        if (forum == null)
			logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum → {Forum}", forumId, forum);
            return;

        //number of topics
        var queryNumTopics = from ft in _forumTopicRepository.Table
            where ft.ForumId == forumId
            select ft.Id;
        var numTopics = await queryNumTopics.CountAsync();

		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): numTopics → {NumTopics}", forumId, numTopics);
		logger.LogInformation("UpdateForumStatsAsync(forumId)={ForumId}: count query executed in {Arg1} ms", forumId, __logStart.ElapsedMilliseconds);
        //number of posts
        var queryNumPosts = from ft in _forumTopicRepository.Table
            join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
            where ft.ForumId == forumId
            select fp.Id;
        var numPosts = await queryNumPosts.CountAsync();

		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): numPosts → {NumPosts}", forumId, numPosts);
        //last values
        var lastTopicId = 0;
        var lastPostId = 0;
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastTopicId → {LastTopicId}", forumId, lastTopicId);
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastPostId → {LastPostId}", forumId, lastPostId);
        var lastPostCustomerId = 0;
        DateTime? lastPostTime = null;
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastPostCustomerId → {LastPostCustomerId}", forumId, lastPostCustomerId);
        var queryLastValues = from ft in _forumTopicRepository.Table
            join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
            where ft.ForumId == forumId
            orderby fp.CreatedOnUtc descending, ft.CreatedOnUtc descending
            select new
            {
                LastTopicId = ft.Id,
                LastPostId = fp.Id,
                LastPostCustomerId = fp.CustomerId,
                LastPostTime = fp.CreatedOnUtc
				logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): LastTopicId → {LastTopicId}", forumId, LastTopicId);
				logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): LastPostId → {LastPostId}", forumId, LastPostId);
				logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): LastPostCustomerId → {LastPostCustomerId}", forumId, LastPostCustomerId);
				logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): LastPostTime → {LastPostTime}", forumId, LastPostTime);
            };
        var lastValues = await queryLastValues.FirstOrDefaultAsync();
        if (lastValues != null)
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastValues → {LastValues}", forumId, lastValues);
        {
            lastTopicId = lastValues.LastTopicId;
            lastPostId = lastValues.LastPostId;
			logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastPostTime → {LastPostTime}", forumId, lastPostTime);
            lastPostCustomerId = lastValues.LastPostCustomerId;
            lastPostTime = lastValues.LastPostTime;
        }

        //update forum
        forum.NumTopics = numTopics;
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.NumTopics → {Arg1}", forumId, forum.NumTopics);
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.NumPosts → {Arg1}", forumId, forum.NumPosts);
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.LastTopicId → {Arg1}", forumId, forum.LastTopicId);
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.LastPostId → {Arg1}", forumId, forum.LastPostId);
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.LastPostCustomerId → {Arg1}", forumId, forum.LastPostCustomerId);
		logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.LastPostTime → {Arg1}", forumId, forum.LastPostTime);
        forum.NumPosts = numPosts;
        forum.LastTopicId = lastTopicId;
        forum.LastPostId = lastPostId;
        forum.LastPostCustomerId = lastPostCustomerId;
        forum.LastPostTime = lastPostTime;
        await UpdateForumAsync(forum);
		logger.LogDebug("<<< Exiting UpdateForumStatsAsync(forumId={ForumId})", forumId);
    }

    /// <summary>
    /// Update forum topic stats
    /// </summary>
    /// <param name="forumTopicId">The forum topic identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateForumTopicStatsAsync(int forumTopicId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (forumTopicId == 0)
            return;

        var forumTopic = await GetTopicByIdAsync(forumTopicId);
        if (forumTopic == null)
			logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic → {ForumTopic}", forumTopicId, forumTopic);
            return;

        //number of posts
        var queryNumPosts = from fp in _forumPostRepository.Table
            where fp.TopicId == forumTopicId
            select fp.Id;
        var numPosts = await queryNumPosts.CountAsync();

		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): numPosts → {NumPosts}", forumTopicId, numPosts);
		logger.LogInformation("UpdateForumTopicStatsAsync(forumTopicId)={ForumTopicId}: count query executed in {Arg1} ms", forumTopicId, __logStart.ElapsedMilliseconds);
        //last values
        var lastPostId = 0;
        var lastPostCustomerId = 0;
		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): lastPostId → {LastPostId}", forumTopicId, lastPostId);
		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): lastPostCustomerId → {LastPostCustomerId}", forumTopicId, lastPostCustomerId);
        DateTime? lastPostTime = null;
        var queryLastValues = from fp in _forumPostRepository.Table
            where fp.TopicId == forumTopicId
            orderby fp.CreatedOnUtc descending
            select new
            {
                LastPostId = fp.Id,
                LastPostCustomerId = fp.CustomerId,
                LastPostTime = fp.CreatedOnUtc
				logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): LastPostId → {LastPostId}", forumTopicId, LastPostId);
				logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): LastPostCustomerId → {LastPostCustomerId}", forumTopicId, LastPostCustomerId);
				logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): LastPostTime → {LastPostTime}", forumTopicId, LastPostTime);
				logger.LogDebug(">>> Entering GetTopicByIdAsync(forumTopicId={ForumTopicId},increaseViews={IncreaseViews})", forumTopicId, increaseViews);
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
				logger.LogInformation("GetTopicByIdAsync(forumTopicId,increaseViews)={ForumTopicId},{IncreaseViews}: find query executed in {Arg2} ms", forumTopicId, increaseViews, __logStart.ElapsedMilliseconds);
				logger.LogDebug("<<< Exiting GetTopicByIdAsync(forumTopicId={ForumTopicId},increaseViews={IncreaseViews})", forumTopicId, increaseViews);
            };
        var lastValues = await queryLastValues.FirstOrDefaultAsync();
        if (lastValues != null)
		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): lastValues → {LastValues}", forumTopicId, lastValues);
        {
            lastPostId = lastValues.LastPostId;
            lastPostCustomerId = lastValues.LastPostCustomerId;
            lastPostTime = lastValues.LastPostTime;
        }
		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): lastPostTime → {LastPostTime}", forumTopicId, lastPostTime);

        //update topic
        forumTopic.NumPosts = numPosts;
		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic.NumPosts → {Arg1}", forumTopicId, forumTopic.NumPosts);
		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic.LastPostId → {Arg1}", forumTopicId, forumTopic.LastPostId);
		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic.LastPostCustomerId → {Arg1}", forumTopicId, forumTopic.LastPostCustomerId);
		logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic.LastPostTime → {Arg1}", forumTopicId, forumTopic.LastPostTime);
        forumTopic.LastPostId = lastPostId;
        forumTopic.LastPostCustomerId = lastPostCustomerId;
        forumTopic.LastPostTime = lastPostTime;

        await UpdateTopicAsync(forumTopic);
		logger.LogDebug("<<< Exiting UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId})", forumTopicId);
    }

    /// <summary>
    /// Update customer stats
    /// </summary>
    /// <param name="customerId">The customer identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateCustomerStatsAsync(int customerId)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (customerId == 0)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(customerId);

		logger.LogDebug("UpdateCustomerStatsAsync(customerId={CustomerId}): customer → {Customer}", customerId, customer);
        if (customer == null)
            return;

        var query = from fp in _forumPostRepository.Table
            where fp.CustomerId == customerId
            select fp.Id;
        var numPosts = await query.CountAsync();

		logger.LogDebug("UpdateCustomerStatsAsync(customerId={CustomerId}): numPosts → {NumPosts}", customerId, numPosts);
		logger.LogInformation("UpdateCustomerStatsAsync(customerId)={CustomerId}: count query executed in {Arg1} ms", customerId, __logStart.ElapsedMilliseconds);
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.ForumPostCountAttribute, numPosts);
    }

    /// <summary>
    /// Gets a forum topic
    /// </summary>
    /// <param name="forumTopicId">The forum topic identifier</param>
    /// <param name="increaseViews">The value indicating whether to increase forum topic views</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum Topic
    /// </returns>
    protected virtual async Task<ForumTopic> GetTopicByIdAsync(int forumTopicId, bool increaseViews)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var forumTopic = await _forumTopicRepository.GetByIdAsync(forumTopicId, cache => default);

        if (forumTopic == null)
            return null;

        if (!increaseViews)
            return forumTopic;

        forumTopic.Views = ++forumTopic.Views;
        await UpdateTopicAsync(forumTopic);

        return forumTopic;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a forum group
    /// </summary>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteForumGroupAsync(ForumGroup forumGroup)
    {
		logger.LogDebug(">>> Entering DeleteForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumGroupRepository.DeleteAsync(forumGroup);
		logger.LogDebug("<<< Exiting DeleteForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
    }

	logger.LogInformation("DeleteForumGroupAsync(forumGroup)={ForumGroup}: external service call took {Arg1} ms", forumGroup, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteForumGroupAsync(forumGroup)={ForumGroup}: delete query executed in {Arg1} ms", forumGroup, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a forum group
    /// </summary>
    /// <param name="forumGroupId">The forum group identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group
    /// </returns>
    public virtual async Task<ForumGroup> GetForumGroupByIdAsync(int forumGroupId)
    {
		logger.LogDebug(">>> Entering GetForumGroupByIdAsync(forumGroupId={ForumGroupId})", forumGroupId);
		logger.LogDebug("<<< Exiting GetForumGroupByIdAsync(forumGroupId={ForumGroupId})", forumGroupId);
        return await _forumGroupRepository.GetByIdAsync(forumGroupId, cache => default);
    }

    /// <summary>
    /// Gets all forum groups
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum groups
    /// </returns>
    public virtual async Task<IList<ForumGroup>> GetAllForumGroupsAsync()
    {
		logger.LogDebug(">>> Entering GetAllForumGroupsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await _forumGroupRepository.GetAllAsync(query =>
        {
            return from fg in query
                orderby fg.DisplayOrder, fg.Id
                select fg;
				logger.LogInformation("GetAllForumGroupsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);
		logger.LogDebug("<<< Exiting GetAllForumGroupsAsync()");
    }

    /// <summary>
    /// Inserts a forum group
    /// </summary>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertForumGroupAsync(ForumGroup forumGroup)
    {
		logger.LogDebug(">>> Entering InsertForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumGroupRepository.InsertAsync(forumGroup);
		logger.LogDebug("<<< Exiting InsertForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertForumGroupAsync(forumGroup)={ForumGroup}: save query executed in {Arg1} ms", forumGroup, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertForumGroupAsync(forumGroup)={ForumGroup}: save query executed in {Arg1} ms", forumGroup, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the forum group
    /// </summary>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateForumGroupAsync(ForumGroup forumGroup)
    {
		logger.LogDebug(">>> Entering UpdateForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumGroupRepository.UpdateAsync(forumGroup);
		logger.LogDebug("<<< Exiting UpdateForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateForumGroupAsync(forumGroup)={ForumGroup}: update query executed in {Arg1} ms", forumGroup, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateForumGroupAsync(forumGroup)={ForumGroup}: update query executed in {Arg1} ms", forumGroup, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deletes a forum
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteForumAsync(Forum forum)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(forum);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //delete forum subscriptions (topics)
        var queryTopicIds = from ft in _forumTopicRepository.Table
            where ft.ForumId == forum.Id
            select ft.Id;
        var queryFs1 = from fs in _forumSubscriptionRepository.Table
            where queryTopicIds.Contains(fs.TopicId)
            select fs;

        await _forumSubscriptionRepository.DeleteAsync(await queryFs1.ToListAsync());

        //delete forum subscriptions (forum)
        var queryFs2 = from fs in _forumSubscriptionRepository.Table
            where fs.ForumId == forum.Id
            select fs;

        await _forumSubscriptionRepository.DeleteAsync(await queryFs2.ToListAsync());

        //delete forum
        await _forumRepository.DeleteAsync(forum);
    }

    /// <summary>
    /// Gets a forum
    /// </summary>
    /// <param name="forumId">The forum identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum
    /// </returns>
    public virtual async Task<Forum> GetForumByIdAsync(int forumId)
    {
        return await _forumRepository.GetByIdAsync(forumId, cache => default);
    }

    /// <summary>
    /// Gets forums by forum group identifier
    /// </summary>
    /// <param name="forumGroupId">The forum group identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forums
    /// </returns>
    public virtual async Task<IList<Forum>> GetAllForumsByGroupIdAsync(int forumGroupId)
    {
		logger.LogDebug(">>> Entering GetAllForumsByGroupIdAsync(forumGroupId={ForumGroupId})", forumGroupId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var forums = await _forumRepository.GetAllAsync(query =>
        {
            return from f in query
                orderby f.DisplayOrder, f.Id
                where f.ForumGroupId == forumGroupId
                select f;
				logger.LogInformation("GetAllForumsByGroupIdAsync(forumGroupId)={ForumGroupId}: find query executed in {Arg1} ms", forumGroupId, __logStart.ElapsedMilliseconds);
        }, cache => cache.PrepareKeyForDefaultCache(NopForumDefaults.ForumByForumGroupCacheKey, forumGroupId));

		logger.LogDebug("<<< Exiting GetAllForumsByGroupIdAsync(forumGroupId={ForumGroupId})", forumGroupId);
        return forums;
    }

    /// <summary>
    /// Inserts a forum
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertForumAsync(Forum forum)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumRepository.InsertAsync(forum);
    }

    /// <summary>
    /// Updates the forum
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateForumAsync(Forum forum)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        // if the forum group is changed then clear cache for the previous group 
        // (we can't use the event consumer because it will work after saving the changes in DB)
        var forumToUpdate = await _forumRepository.LoadOriginalCopyAsync(forum);
        if (forumToUpdate.ForumGroupId != forum.ForumGroupId)
            await _staticCacheManager.RemoveAsync(NopForumDefaults.ForumByForumGroupCacheKey, forumToUpdate.ForumGroupId);

        await _forumRepository.UpdateAsync(forum);
    }

    /// <summary>
    /// Deletes a forum topic
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteTopicAsync(ForumTopic forumTopic)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(forumTopic);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customerId = forumTopic.CustomerId;
        var forumId = forumTopic.ForumId;

        //delete topic
        await _forumTopicRepository.DeleteAsync(forumTopic);

        //delete forum subscriptions
        var queryFs = from ft in _forumSubscriptionRepository.Table
            where ft.TopicId == forumTopic.Id
            select ft;
        var forumSubscriptions = await queryFs.ToListAsync();

        await _forumSubscriptionRepository.DeleteAsync(forumSubscriptions);

        //update stats
        await UpdateForumStatsAsync(forumId);
        await UpdateCustomerStatsAsync(customerId);
    }

    /// <summary>
    /// Gets a forum topic
    /// </summary>
    /// <param name="forumTopicId">The forum topic identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum Topic
    /// </returns>
    public virtual async Task<ForumTopic> GetTopicByIdAsync(int forumTopicId)
    {
        return await GetTopicByIdAsync(forumTopicId, false);
    }

    /// <summary>
    /// Gets all forum topics
    /// </summary>
    /// <param name="forumId">The forum identifier</param>
    /// <param name="customerId">The customer identifier</param>
    /// <param name="keywords">Keywords</param>
    /// <param name="searchType">Search type</param>
    /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum Topics
    /// </returns>
    public virtual async Task<IPagedList<ForumTopic>> GetAllTopicsAsync(int forumId = 0,
        int customerId = 0, string keywords = "", ForumSearchType searchType = ForumSearchType.All,
        int limitDays = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        DateTime? limitDate = null;
        if (limitDays > 0)
            limitDate = DateTime.UtcNow.AddDays(-limitDays);

        var searchKeywords = !string.IsNullOrEmpty(keywords);
        var searchTopicTitles = searchType == ForumSearchType.All || searchType == ForumSearchType.TopicTitlesOnly;
        var searchPostText = searchType == ForumSearchType.All || searchType == ForumSearchType.PostTextOnly;

        var topics = await _forumTopicRepository.GetAllPagedAsync(query =>
        {
            var query1 = from ft in query
                join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                where
                    (forumId == 0 || ft.ForumId == forumId) &&
                    (customerId == 0 || ft.CustomerId == customerId) &&
                    (!searchKeywords ||
                     (searchTopicTitles && ft.Subject.Contains(keywords)) ||
                     (searchPostText && fp.Text.Contains(keywords))) &&
                    (!limitDate.HasValue || limitDate.Value <= ft.LastPostTime)
                select ft.Id;

            var query2 = from ft in query
                where query1.Contains(ft.Id)
                orderby ft.TopicTypeId descending, ft.LastPostTime descending, ft.Id descending
                select ft;

            return query2;
        }, pageIndex, pageSize);

        return topics;
    }

    /// <summary>
    /// Gets active forum topics
    /// </summary>
    /// <param name="forumId">The forum identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum Topics
    /// </returns>
    public virtual async Task<IPagedList<ForumTopic>> GetActiveTopicsAsync(int forumId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query1 = from ft in _forumTopicRepository.Table
            where
                (forumId == 0 || ft.ForumId == forumId) &&
                ft.LastPostTime.HasValue
            select ft.Id;

        var query2 = from ft in _forumTopicRepository.Table
            where query1.Contains(ft.Id)
            orderby ft.LastPostTime descending
            select ft;

        var topics = await query2.ToPagedListAsync(pageIndex, pageSize);

        return topics;
    }

    /// <summary>
    /// Inserts a forum topic
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed customers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTopicAsync(ForumTopic forumTopic, bool sendNotifications)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumTopicRepository.InsertAsync(forumTopic);

        //update stats
        await UpdateForumStatsAsync(forumTopic.ForumId);

        if (!sendNotifications)
            return;

        //send notifications
        var forum = await GetForumByIdAsync(forumTopic.ForumId);
        var subscriptions = await GetAllSubscriptionsAsync(forumId: forum.Id);
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

        foreach (var subscription in subscriptions)
        {
            if (subscription.CustomerId == forumTopic.CustomerId)
                continue;

            var customer = await _customerService.GetCustomerByIdAsync(subscription.CustomerId);

            if (!string.IsNullOrEmpty(customer?.Email))
                await _workflowMessageService.SendNewForumTopicMessageAsync(customer, forumTopic, forum, languageId);
        }
		logger.LogDebug("<<< Exiting InsertTopicAsync(forumTopic={ForumTopic},sendNotifications={SendNotifications})", forumTopic, sendNotifications);
    }

    /// <summary>
    /// Updates the forum topic
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTopicAsync(ForumTopic forumTopic)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumTopicRepository.UpdateAsync(forumTopic);
    }

    /// <summary>
    /// Moves the forum topic
    /// </summary>
    /// <param name="forumTopicId">The forum topic identifier</param>
    /// <param name="newForumId">New forum identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the moved forum topic
    /// </returns>
    public virtual async Task<ForumTopic> MoveTopicAsync(int forumTopicId, int newForumId)
    {
        var forumTopic = await GetTopicByIdAsync(forumTopicId);
        if (forumTopic == null)
            return null;

        if (!await IsCustomerAllowedToMoveTopicAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
            return forumTopic;

        var previousForumId = forumTopic.ForumId;
        var newForum = await GetForumByIdAsync(newForumId);

        if (newForum == null)
            return forumTopic;

        if (previousForumId == newForumId)
            return forumTopic;

        forumTopic.ForumId = newForum.Id;
        forumTopic.UpdatedOnUtc = DateTime.UtcNow;
        await UpdateTopicAsync(forumTopic);

        //update forum stats
        await UpdateForumStatsAsync(previousForumId);
        await UpdateForumStatsAsync(newForumId);
        return forumTopic;
    }

    /// <summary>
    /// Deletes a forum post
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePostAsync(ForumPost forumPost)
    {
		logger.LogDebug(">>> Entering DeletePostAsync(forumPost={ForumPost})", forumPost);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(forumPost);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var forumTopicId = forumPost.TopicId;
        var customerId = forumPost.CustomerId;
        var forumTopic = await GetTopicByIdAsync(forumTopicId);
		logger.LogDebug("DeletePostAsync(forumPost={ForumPost}): customerId → {CustomerId}", forumPost, customerId);
		logger.LogDebug("DeletePostAsync(forumPost={ForumPost}): forumTopic → {ForumTopic}", forumPost, forumTopic);
        var forumId = forumTopic.ForumId;

        //delete topic if it was the first post
        var deleteTopic = false;
        var firstPost = await GetFirstPostAsync(forumTopic);
		logger.LogDebug("DeletePostAsync(forumPost={ForumPost}): deleteTopic → {DeleteTopic}", forumPost, deleteTopic);
		logger.LogDebug("DeletePostAsync(forumPost={ForumPost}): firstPost → {FirstPost}", forumPost, firstPost);
        if (firstPost != null && firstPost.Id == forumPost.Id)
            deleteTopic = true;

        //delete forum post
        await _forumPostRepository.DeleteAsync(forumPost);
		logger.LogInformation("DeletePostAsync(forumPost)={ForumPost}: delete query executed in {Arg1} ms", forumPost, __logStart.ElapsedMilliseconds);

        //delete topic
        if (deleteTopic)
            await DeleteTopicAsync(forumTopic);

        //update stats
        if (!deleteTopic)
            await UpdateForumTopicStatsAsync(forumTopicId);

        await UpdateForumStatsAsync(forumId);
        await UpdateCustomerStatsAsync(customerId);
		logger.LogDebug("<<< Exiting DeletePostAsync(forumPost={ForumPost})", forumPost);
		logger.LogInformation("DeletePostAsync(forumPost)={ForumPost}: external service call took {Arg1} ms", forumPost, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Gets a forum post
    /// </summary>
    /// <param name="forumPostId">The forum post identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum Post
    /// </returns>
    public virtual async Task<ForumPost> GetPostByIdAsync(int forumPostId)
    {
		logger.LogDebug(">>> Entering GetPostByIdAsync(forumPostId={ForumPostId})", forumPostId);
		logger.LogDebug("<<< Exiting GetPostByIdAsync(forumPostId={ForumPostId})", forumPostId);
        return await _forumPostRepository.GetByIdAsync(forumPostId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets all forum posts
    /// </summary>
    /// <param name="forumTopicId">The forum topic identifier</param>
    /// <param name="customerId">The customer identifier</param>
    /// <param name="keywords">Keywords</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the posts
    /// </returns>
    public virtual async Task<IPagedList<ForumPost>> GetAllPostsAsync(int forumTopicId = 0,
        int customerId = 0, string keywords = "",
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        return await GetAllPostsAsync(forumTopicId, customerId, keywords, true,
            pageIndex, pageSize);
    }

    /// <summary>
    /// Gets all forum posts
    /// </summary>
    /// <param name="forumTopicId">The forum topic identifier</param>
    /// <param name="customerId">The customer identifier</param>
    /// <param name="keywords">Keywords</param>
    /// <param name="ascSort">Sort order</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum Posts
    /// </returns>
    public virtual async Task<IPagedList<ForumPost>> GetAllPostsAsync(int forumTopicId = 0, int customerId = 0,
        string keywords = "", bool ascSort = false,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var forumPosts = await _forumPostRepository.GetAllPagedAsync(query =>
        {
            if (forumTopicId > 0)
                query = query.Where(fp => forumTopicId == fp.TopicId);

            if (customerId > 0)
                query = query.Where(fp => customerId == fp.CustomerId);

            if (!string.IsNullOrEmpty(keywords))
                query = query.Where(fp => fp.Text.Contains(keywords));

            query = ascSort
                ? query.OrderBy(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id)
                : query.OrderByDescending(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id);

            return query;
        }, pageIndex, pageSize);

        return forumPosts;
    }

    /// <summary>
    /// Inserts a forum post
    /// </summary>
    /// <param name="forumPost">The forum post</param>
    /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed customers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPostAsync(ForumPost forumPost, bool sendNotifications)
    {
		logger.LogDebug(">>> Entering InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications})", forumPost, sendNotifications);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumPostRepository.InsertAsync(forumPost);

		logger.LogInformation("InsertPostAsync(forumPost,sendNotifications)={ForumPost},{SendNotifications}: save query executed in {Arg2} ms", forumPost, sendNotifications, __logStart.ElapsedMilliseconds);
        //update stats
        var customerId = forumPost.CustomerId;
        var forumTopic = await GetTopicByIdAsync(forumPost.TopicId);
		logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): customerId → {CustomerId}", forumPost, sendNotifications, customerId);
		logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): forumTopic → {ForumTopic}", forumPost, sendNotifications, forumTopic);
        var forumId = forumTopic.ForumId;

        await UpdateForumTopicStatsAsync(forumPost.TopicId);
        await UpdateForumStatsAsync(forumId);
        await UpdateCustomerStatsAsync(customerId);

        //notifications
        if (!sendNotifications)
            return;

        var forum = await GetForumByIdAsync(forumTopic.ForumId);
        var subscriptions = await GetAllSubscriptionsAsync(topicId: forumTopic.Id);
		logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): forum → {Forum}", forumPost, sendNotifications, forum);
		logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): subscriptions → {Subscriptions}", forumPost, sendNotifications, subscriptions);

        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

		logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): languageId → {LanguageId}", forumPost, sendNotifications, languageId);
        var friendlyTopicPageIndex = await CalculateTopicPageIndexAsync(forumPost.TopicId,
            _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10,
            forumPost.Id) + 1;

        foreach (var subscription in subscriptions)
        {
            if (subscription.CustomerId == forumPost.CustomerId)
                continue;

            var customer = await _customerService.GetCustomerByIdAsync(subscription.CustomerId);

			logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): customer → {Customer}", forumPost, sendNotifications, customer);
            if (!string.IsNullOrEmpty(customer?.Email))
                await _workflowMessageService.SendNewForumPostMessageAsync(customer, forumPost, forumTopic, forum, friendlyTopicPageIndex, languageId);
        }
		logger.LogDebug("<<< Exiting InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications})", forumPost, sendNotifications);
    }

    /// <summary>
    /// Updates the forum post
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePostAsync(ForumPost forumPost)
    {
		logger.LogDebug(">>> Entering UpdatePostAsync(forumPost={ForumPost})", forumPost);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumPostRepository.UpdateAsync(forumPost);
		logger.LogDebug("<<< Exiting UpdatePostAsync(forumPost={ForumPost})", forumPost);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdatePostAsync(forumPost)={ForumPost}: update query executed in {Arg1} ms", forumPost, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdatePostAsync(forumPost)={ForumPost}: update query executed in {Arg1} ms", forumPost, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Deletes a private message
    /// </summary>
    /// <param name="privateMessage">Private message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePrivateMessageAsync(PrivateMessage privateMessage)
    {
		logger.LogDebug(">>> Entering DeletePrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumPrivateMessageRepository.DeleteAsync(privateMessage);
		logger.LogDebug("<<< Exiting DeletePrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeletePrivateMessageAsync(privateMessage)={PrivateMessage}: external service call took {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeletePrivateMessageAsync(privateMessage)={PrivateMessage}: delete query executed in {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeletePrivateMessageAsync(privateMessage)={PrivateMessage}: external service call took {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeletePrivateMessageAsync(privateMessage)={PrivateMessage}: delete query executed in {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a private message
    /// </summary>
    /// <param name="privateMessageId">The private message identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the private message
    /// </returns>
    public virtual async Task<PrivateMessage> GetPrivateMessageByIdAsync(int privateMessageId)
    {
		logger.LogDebug(">>> Entering GetPrivateMessageByIdAsync(privateMessageId={PrivateMessageId})", privateMessageId);
		logger.LogDebug("<<< Exiting GetPrivateMessageByIdAsync(privateMessageId={PrivateMessageId})", privateMessageId);
        return await _forumPrivateMessageRepository.GetByIdAsync(privateMessageId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets private messages
    /// </summary>
    /// <param name="storeId">The store identifier; pass 0 to load all messages</param>
    /// <param name="fromCustomerId">The customer identifier who sent the message</param>
    /// <param name="toCustomerId">The customer identifier who should receive the message</param>
    /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
    /// <param name="isDeletedByAuthor">A value indicating whether loaded messages are deleted by author. false - messages are not deleted by author, null to load all messages</param>
    /// <param name="isDeletedByRecipient">A value indicating whether loaded messages are deleted by recipient. false - messages are not deleted by recipient, null to load all messages</param>
    /// <param name="keywords">Keywords</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the private messages
    /// </returns>
    public virtual async Task<IPagedList<PrivateMessage>> GetAllPrivateMessagesAsync(int storeId, int fromCustomerId,
        int toCustomerId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
        string keywords, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var privateMessages = await _forumPrivateMessageRepository.GetAllPagedAsync(query =>
        {
            if (storeId > 0)
                query = query.Where(pm => storeId == pm.StoreId);
            if (fromCustomerId > 0)
                query = query.Where(pm => fromCustomerId == pm.FromCustomerId);
            if (toCustomerId > 0)
                query = query.Where(pm => toCustomerId == pm.ToCustomerId);
            if (isRead.HasValue)
                query = query.Where(pm => isRead.Value == pm.IsRead);
            if (isDeletedByAuthor.HasValue)
                query = query.Where(pm => isDeletedByAuthor.Value == pm.IsDeletedByAuthor);
            if (isDeletedByRecipient.HasValue)
                query = query.Where(pm => isDeletedByRecipient.Value == pm.IsDeletedByRecipient);
            if (!string.IsNullOrEmpty(keywords))
            {
                query = query.Where(pm => pm.Subject.Contains(keywords));
                query = query.Where(pm => pm.Text.Contains(keywords));
            }

            query = query.OrderByDescending(pm => pm.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);

        return privateMessages;
    }

    /// <summary>
    /// Inserts a private message
    /// </summary>
    /// <param name="privateMessage">Private message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPrivateMessageAsync(PrivateMessage privateMessage)
    {
		logger.LogDebug(">>> Entering InsertPrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumPrivateMessageRepository.InsertAsync(privateMessage);

		logger.LogInformation("InsertPrivateMessageAsync(privateMessage)={PrivateMessage}: save query executed in {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
        var customerTo = await _customerService.GetCustomerByIdAsync(privateMessage.ToCustomerId)
                         ?? throw new NopException("Recipient could not be loaded");

        //UI notification
        await _genericAttributeService.SaveAttributeAsync(customerTo, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, false, privateMessage.StoreId);

        //Email notification
        if (_forumSettings.NotifyAboutPrivateMessages)
            await _workflowMessageService.SendPrivateMessageNotificationAsync(privateMessage, (await _workContext.GetWorkingLanguageAsync()).Id);
			logger.LogDebug("<<< Exiting InsertPrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
    }

    /// <summary>
    /// Updates the private message
    /// </summary>
    /// <param name="privateMessage">Private message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePrivateMessageAsync(PrivateMessage privateMessage)
    {
		logger.LogDebug(">>> Entering UpdatePrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(privateMessage);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (privateMessage.IsDeletedByAuthor && privateMessage.IsDeletedByRecipient)
            await _forumPrivateMessageRepository.DeleteAsync(privateMessage);
			logger.LogInformation("UpdatePrivateMessageAsync(privateMessage)={PrivateMessage}: delete query executed in {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
        else
            await _forumPrivateMessageRepository.UpdateAsync(privateMessage);
			logger.LogDebug("<<< Exiting UpdatePrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
			logger.LogInformation("UpdatePrivateMessageAsync(privateMessage)={PrivateMessage}: external service call took {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
			logger.LogInformation("UpdatePrivateMessageAsync(privateMessage)={PrivateMessage}: update query executed in {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Deletes a forum subscription
    /// </summary>
    /// <param name="forumSubscription">Forum subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSubscriptionAsync(ForumSubscription forumSubscription)
    {
		logger.LogDebug(">>> Entering DeleteSubscriptionAsync(forumSubscription={ForumSubscription})", forumSubscription);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumSubscriptionRepository.DeleteAsync(forumSubscription);
		logger.LogDebug("<<< Exiting DeleteSubscriptionAsync(forumSubscription={ForumSubscription})", forumSubscription);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteSubscriptionAsync(forumSubscription)={ForumSubscription}: external service call took {Arg1} ms", forumSubscription, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteSubscriptionAsync(forumSubscription)={ForumSubscription}: delete query executed in {Arg1} ms", forumSubscription, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteSubscriptionAsync(forumSubscription)={ForumSubscription}: external service call took {Arg1} ms", forumSubscription, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteSubscriptionAsync(forumSubscription)={ForumSubscription}: delete query executed in {Arg1} ms", forumSubscription, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a forum subscription
    /// </summary>
    /// <param name="forumSubscriptionId">The forum subscription identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum subscription
    /// </returns>
    public virtual async Task<ForumSubscription> GetSubscriptionByIdAsync(int forumSubscriptionId)
    {
		logger.LogDebug(">>> Entering GetSubscriptionByIdAsync(forumSubscriptionId={ForumSubscriptionId})", forumSubscriptionId);
		logger.LogDebug("<<< Exiting GetSubscriptionByIdAsync(forumSubscriptionId={ForumSubscriptionId})", forumSubscriptionId);
        return await _forumSubscriptionRepository.GetByIdAsync(forumSubscriptionId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets forum subscriptions
    /// </summary>
    /// <param name="customerId">The customer identifier</param>
    /// <param name="forumId">The forum identifier</param>
    /// <param name="topicId">The topic identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum subscriptions
    /// </returns>
    public virtual async Task<IPagedList<ForumSubscription>> GetAllSubscriptionsAsync(int customerId = 0, int forumId = 0,
        int topicId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var forumSubscriptions = await _forumSubscriptionRepository.GetAllPagedAsync(query =>
        {
            var fsQuery = from fs in query
                join c in _customerRepository.Table on fs.CustomerId equals c.Id
                where
                    (customerId == 0 || fs.CustomerId == customerId) &&
                    (forumId == 0 || fs.ForumId == forumId) &&
                    (topicId == 0 || fs.TopicId == topicId) &&
                    c.Active &&
                    !c.Deleted
                select fs.SubscriptionGuid;

            var rez = from fs in query
                where fsQuery.Contains(fs.SubscriptionGuid)
                orderby fs.CreatedOnUtc descending, fs.SubscriptionGuid descending
                select fs;

            return rez;
        }, pageIndex, pageSize);

        return forumSubscriptions;
    }

    /// <summary>
    /// Inserts a forum subscription
    /// </summary>
    /// <param name="forumSubscription">Forum subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSubscriptionAsync(ForumSubscription forumSubscription)
    {
		logger.LogDebug(">>> Entering InsertSubscriptionAsync(forumSubscription={ForumSubscription})", forumSubscription);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumSubscriptionRepository.InsertAsync(forumSubscription);
		logger.LogDebug("<<< Exiting InsertSubscriptionAsync(forumSubscription={ForumSubscription})", forumSubscription);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertSubscriptionAsync(forumSubscription)={ForumSubscription}: save query executed in {Arg1} ms", forumSubscription, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertSubscriptionAsync(forumSubscription)={ForumSubscription}: save query executed in {Arg1} ms", forumSubscription, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Check whether customer is allowed to create new topics
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="forum">Forum</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToCreateTopicAsync(Customer customer, Forum forum)
    {
        if (forum == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer) && !_forumSettings.AllowGuestsToCreateTopics)
		logger.LogDebug("<<< Exiting IsCustomerAllowedToCreateTopicAsync(customer={Customer},forum={Forum})", customer, forum);
            return false;

        return true;
    }

    /// <summary>
    /// Check whether customer is allowed to edit topic
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="topic">Topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToEditTopicAsync(Customer customer, ForumTopic topic)
    {
        if (topic == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer))
            return false;

        if (await _customerService.IsForumModeratorAsync(customer))
            return true;

        if (!_forumSettings.AllowCustomersToEditPosts)
            return false;

        var ownTopic = customer.Id == topic.CustomerId;

		logger.LogDebug("<<< Exiting IsCustomerAllowedToEditTopicAsync(customer={Customer},topic={Topic})", customer, topic);
        return ownTopic;
    }

    /// <summary>
    /// Check whether customer is allowed to move topic
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="topic">Topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToMoveTopicAsync(Customer customer, ForumTopic topic)
    {
        if (topic == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer))
		logger.LogDebug("<<< Exiting IsCustomerAllowedToMoveTopicAsync(customer={Customer},topic={Topic})", customer, topic);
            return false;

        return await _customerService.IsForumModeratorAsync(customer);
    }

    /// <summary>
    /// Check whether customer is allowed to delete topic
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="topic">Topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToDeleteTopicAsync(Customer customer, ForumTopic topic)
    {
        if (topic == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer))
            return false;

        if (await _customerService.IsForumModeratorAsync(customer))
            return true;

        if (!_forumSettings.AllowCustomersToDeletePosts)
            return false;

        var ownTopic = customer.Id == topic.CustomerId;

		logger.LogDebug("<<< Exiting IsCustomerAllowedToDeleteTopicAsync(customer={Customer},topic={Topic})", customer, topic);
        return ownTopic;
    }

    /// <summary>
    /// Check whether customer is allowed to create new post
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="topic">Topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToCreatePostAsync(Customer customer, ForumTopic topic)
    {
        if (topic == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer) && !_forumSettings.AllowGuestsToCreatePosts)
		logger.LogDebug("<<< Exiting IsCustomerAllowedToCreatePostAsync(customer={Customer},topic={Topic})", customer, topic);
            return false;

        return true;
    }

    /// <summary>
    /// Check whether customer is allowed to edit post
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="post">Topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToEditPostAsync(Customer customer, ForumPost post)
    {
        if (post == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer))
            return false;

        if (await _customerService.IsForumModeratorAsync(customer))
            return true;

        if (!_forumSettings.AllowCustomersToEditPosts)
            return false;

        var ownPost = customer.Id == post.CustomerId;

		logger.LogDebug("<<< Exiting IsCustomerAllowedToEditPostAsync(customer={Customer},post={Post})", customer, post);
        return ownPost;
    }

    /// <summary>
    /// Check whether customer is allowed to delete post
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="post">Topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToDeletePostAsync(Customer customer, ForumPost post)
    {
        if (post == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer))
            return false;

        if (await _customerService.IsForumModeratorAsync(customer))
            return true;

        if (!_forumSettings.AllowCustomersToDeletePosts)
            return false;

        var ownPost = customer.Id == post.CustomerId;

		logger.LogDebug("<<< Exiting IsCustomerAllowedToDeletePostAsync(customer={Customer},post={Post})", customer, post);
        return ownPost;
    }

    /// <summary>
    /// Check whether customer is allowed to set topic priority
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToSetTopicPriorityAsync(Customer customer)
    {
        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer))
		logger.LogDebug("<<< Exiting IsCustomerAllowedToSetTopicPriorityAsync(customer={Customer})", customer);
            return false;

        return await _customerService.IsForumModeratorAsync(customer);
    }

    /// <summary>
    /// Check whether customer is allowed to watch topics
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if allowed, otherwise false
    /// </returns>
    public virtual async Task<bool> IsCustomerAllowedToSubscribeAsync(Customer customer)
    {
        if (customer == null)
            return false;

        if (customer.IsSearchEngineAccount() || await _customerService.IsGuestAsync(customer))
		logger.LogDebug("<<< Exiting IsCustomerAllowedToSubscribeAsync(customer={Customer})", customer);
            return false;

        return true;
    }

    /// <summary>
    /// Calculates topic page index by post identifier
    /// </summary>
    /// <param name="forumTopicId">Forum topic identifier</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="postId">Post identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the page index
    /// </returns>
    public virtual async Task<int> CalculateTopicPageIndexAsync(int forumTopicId, int pageSize, int postId)
    {
        var pageIndex = 0;
        var forumPosts = await GetAllPostsAsync(forumTopicId, ascSort: true);

        for (var i = 0; i < forumPosts.TotalCount; i++)
        {
            if (forumPosts[i].Id != postId)
                continue;

            if (pageSize > 0)
                pageIndex = i / pageSize;
        }

        return pageIndex;
    }

    /// <summary>
    /// Get a post vote 
    /// </summary>
    /// <param name="postId">Post identifier</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the post vote
    /// </returns>
    public virtual async Task<ForumPostVote> GetPostVoteAsync(int postId, Customer customer)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (customer == null)
		logger.LogDebug("<<< Exiting GetPostVoteAsync(postId={PostId},customer={Customer})", postId, customer);
            return null;

        return await _forumPostVoteRepository.Table
            .FirstOrDefaultAsync(pv => pv.ForumPostId == postId && pv.CustomerId == customer.Id);
    }

    /// <summary>
    /// Get post vote made since the parameter date
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="createdFromUtc">Date</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the post votes count
    /// </returns>
    public virtual async Task<int> GetNumberOfPostVotesAsync(Customer customer, DateTime createdFromUtc)
    {
        if (customer == null)
            return 0;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

		logger.LogDebug("<<< Exiting GetNumberOfPostVotesAsync(customer={Customer},createdFromUtc={CreatedFromUtc})", customer, createdFromUtc);
        return await _forumPostVoteRepository.Table
            .CountAsync(pv => pv.CustomerId == customer.Id && pv.CreatedOnUtc > createdFromUtc);
    }

    /// <summary>
    /// Insert a post vote
    /// </summary>
    /// <param name="postVote">Post vote</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPostVoteAsync(ForumPostVote postVote)
    {
		logger.LogDebug(">>> Entering InsertPostVoteAsync(postVote={PostVote})", postVote);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumPostVoteRepository.InsertAsync(postVote);

		logger.LogInformation("InsertPostVoteAsync(postVote)={PostVote}: save query executed in {Arg1} ms", postVote, __logStart.ElapsedMilliseconds);
        //update post
        var post = await GetPostByIdAsync(postVote.ForumPostId);
        post.VoteCount = postVote.IsUp ? ++post.VoteCount : --post.VoteCount;
		logger.LogDebug("InsertPostVoteAsync(postVote={PostVote}): post → {Post}", postVote, post);
		logger.LogDebug("InsertPostVoteAsync(postVote={PostVote}): post.VoteCount → {Arg1}", postVote, post.VoteCount);

        await UpdatePostAsync(post);
		logger.LogDebug("<<< Exiting InsertPostVoteAsync(postVote={PostVote})", postVote);
    }

    /// <summary>
    /// Delete a post vote
    /// </summary>
    /// <param name="postVote">Post vote</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePostVoteAsync(ForumPostVote postVote)
    {
		logger.LogDebug(">>> Entering DeletePostVoteAsync(postVote={PostVote})", postVote);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(postVote);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeletePostVoteAsync(postVote)={PostVote}: delete query executed in {Arg1} ms", postVote, __logStart.ElapsedMilliseconds);
        await _forumPostVoteRepository.DeleteAsync(postVote);

        // update post
        var post = await GetPostByIdAsync(postVote.ForumPostId);
        post.VoteCount = postVote.IsUp ? --post.VoteCount : ++post.VoteCount;
		logger.LogDebug("DeletePostVoteAsync(postVote={PostVote}): post → {Post}", postVote, post);
		logger.LogDebug("DeletePostVoteAsync(postVote={PostVote}): post.VoteCount → {Arg1}", postVote, post.VoteCount);
		logger.LogInformation("DeletePostVoteAsync(postVote)={PostVote}: external service call took {Arg1} ms", postVote, __logStart.ElapsedMilliseconds);

        await UpdatePostAsync(post);
		logger.LogDebug("<<< Exiting DeletePostVoteAsync(postVote={PostVote})", postVote);
    }

    /// <summary>
    /// Formats the forum post text
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatPostText(ForumPost forumPost)
    {
		logger.LogDebug(">>> Entering FormatPostText(forumPost={ForumPost})", forumPost);
        var text = forumPost.Text;

		logger.LogDebug("FormatPostText(forumPost={ForumPost}): text → {Text}", forumPost, text);
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        switch (_forumSettings.ForumEditor)
        {
            case EditorType.SimpleTextBox:
            {
                text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);
            }

                break;
            case EditorType.BBCodeEditor:
            {
                text = _htmlFormatter.FormatText(text, false, true, false, true, false, false);
            }

                break;
            default:
                break;
        }

		logger.LogDebug("<<< Exiting FormatPostText(forumPost={ForumPost})", forumPost);
        return text;
    }

    /// <summary>
    /// Strips the topic subject
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>Formatted subject</returns>
    public virtual string StripTopicSubject(ForumTopic forumTopic)
    {
        var subject = forumTopic.Subject;
        if (string.IsNullOrEmpty(subject))
            return subject;

        var strippedTopicMaxLength = _forumSettings.StrippedTopicMaxLength;
        if (strippedTopicMaxLength <= 0)
            return subject;

        if (subject.Length <= strippedTopicMaxLength)
            return subject;

        var index = subject.IndexOf(" ", strippedTopicMaxLength, StringComparison.Ordinal);

        if (index <= 0)
            return subject;

        subject = subject[0..index];
        subject += "...";

        return subject;
    }

    /// <summary>
    /// Formats the forum signature text
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatForumSignatureText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);
		logger.LogDebug("FormatForumSignatureText(text={Text}): text → {Text1}", text, text);
		logger.LogDebug("<<< Exiting FormatForumSignatureText(text={Text})", text);
        return text;
    }

    /// <summary>
    /// Formats the private message text
    /// </summary>
    /// <param name="pm">Private message</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatPrivateMessageText(PrivateMessage pm)
    {
		logger.LogDebug(">>> Entering FormatPrivateMessageText(pm={Pm})", pm);
        var text = pm.Text;

		logger.LogDebug("FormatPrivateMessageText(pm={Pm}): text → {Text}", pm, text);
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = _htmlFormatter.FormatText(text, false, true, false, true, false, false);

		logger.LogDebug("<<< Exiting FormatPrivateMessageText(pm={Pm})", pm);
        return text;
    }

    /// <summary>
    /// Get first post
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum post
    /// </returns>
    public virtual async Task<ForumPost> GetFirstPostAsync(ForumTopic forumTopic)
    {
        ArgumentNullException.ThrowIfNull(forumTopic);

        var forumPosts = await GetAllPostsAsync(forumTopic.Id, 0, string.Empty, 0, 1);
        if (forumPosts.Any())
            return forumPosts[0];

        return null;
    }

    /// <summary>
    /// Gets ForumGroup SE (search engine) name
    /// </summary>
    /// <param name="forumGroup">ForumGroup</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forumGroup SE (search engine) name
    /// </returns>
    public virtual async Task<string> GetForumGroupSeNameAsync(ForumGroup forumGroup)
    {
		logger.LogDebug(">>> Entering GetForumGroupSeNameAsync(forumGroup={ForumGroup})", forumGroup);
        ArgumentNullException.ThrowIfNull(forumGroup);

        var seName = await _urlRecordService.GetSeNameAsync(forumGroup.Name, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

		logger.LogDebug("GetForumGroupSeNameAsync(forumGroup={ForumGroup}): seName → {SeName}", forumGroup, seName);
		logger.LogDebug("<<< Exiting GetForumGroupSeNameAsync(forumGroup={ForumGroup})", forumGroup);
        return seName;
    }

    /// <summary>
    /// Gets Forum SE (search engine) name
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum SE (search engine) name
    /// </returns>
    public virtual async Task<string> GetForumSeNameAsync(Forum forum)
    {
        ArgumentNullException.ThrowIfNull(forum);

        var seName = await _urlRecordService.GetSeNameAsync(forum.Name, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

        return seName;
    }

    /// <summary>
    /// Gets ForumTopic SE (search engine) name
    /// </summary>
    /// <param name="forumTopic">ForumTopic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forumTopic SE (search engine) name
    /// </returns>
    public virtual async Task<string> GetTopicSeNameAsync(ForumTopic forumTopic)
    {
        ArgumentNullException.ThrowIfNull(forumTopic);

        var seName = await _urlRecordService.GetSeNameAsync(forumTopic.Subject, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

        // Trim SE name to avoid URLs that are too long
        var maxLength = NopSeoDefaults.ForumTopicLength;
        if (seName.Length > maxLength)
            seName = seName[0..maxLength];

        return seName;
    }

    #endregion
}