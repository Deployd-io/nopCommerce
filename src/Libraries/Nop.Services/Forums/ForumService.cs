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
using Microsoft.Extensions.Logging;

namespace Nop.Services.Forums;

/// <summary>
/// Forum service
/// </summary>
public partial class ForumService : IForumService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogDebug(">>> Entering ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings})", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings);
        _forumSettings = forumSettings;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _forumSettings → {_forumSettings}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _forumSettings);
        _customerService = customerService;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _customerService → {_customerService}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _customerService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _genericAttributeService → {_genericAttributeService}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _genericAttributeService);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _htmlFormatter → {_htmlFormatter}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _htmlFormatter);
        _customerRepository = customerRepository;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _customerRepository → {_customerRepository}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _customerRepository);
        _forumRepository = forumRepository;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _forumRepository → {_forumRepository}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _forumRepository);
        _forumGroupRepository = forumGroupRepository;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _forumGroupRepository → {_forumGroupRepository}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _forumGroupRepository);
        _forumPostRepository = forumPostRepository;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _forumPostRepository → {_forumPostRepository}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _forumPostRepository);
        _forumPostVoteRepository = forumPostVoteRepository;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _forumPostVoteRepository → {_forumPostVoteRepository}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _forumPostVoteRepository);
        _forumSubscriptionRepository = forumSubscriptionRepository;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _forumSubscriptionRepository → {_forumSubscriptionRepository}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _forumSubscriptionRepository);
        _forumTopicRepository = forumTopicRepository;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _forumTopicRepository → {_forumTopicRepository}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _forumTopicRepository);
        _forumPrivateMessageRepository = forumPrivateMessageRepository;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _forumPrivateMessageRepository → {_forumPrivateMessageRepository}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _forumPrivateMessageRepository);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _staticCacheManager → {_staticCacheManager}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _staticCacheManager);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _urlRecordService → {_urlRecordService}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _urlRecordService);
        _workContext = workContext;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _workContext → {_workContext}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _workflowMessageService → {_workflowMessageService}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _workflowMessageService);
        _seoSettings = seoSettings;
		_logger.LogDebug("ForumService(forumSettings={ForumSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},htmlFormatter={HtmlFormatter},customerRepository={CustomerRepository},forumRepository={ForumRepository},forumGroupRepository={ForumGroupRepository},forumPostRepository={ForumPostRepository},forumPostVoteRepository={ForumPostVoteRepository},forumSubscriptionRepository={ForumSubscriptionRepository},forumTopicRepository={ForumTopicRepository},forumPrivateMessageRepository={ForumPrivateMessageRepository},staticCacheManager={StaticCacheManager},urlRecordService={UrlRecordService},workContext={WorkContext},workflowMessageService={WorkflowMessageService},seoSettings={SeoSettings}): _seoSettings → {_seoSettings}", forumSettings, customerService, genericAttributeService, htmlFormatter, customerRepository, forumRepository, forumGroupRepository, forumPostRepository, forumPostVoteRepository, forumSubscriptionRepository, forumTopicRepository, forumPrivateMessageRepository, staticCacheManager, urlRecordService, workContext, workflowMessageService, seoSettings, _seoSettings);
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
		_logger.LogDebug(">>> Entering UpdateForumStatsAsync(forumId={ForumId})", forumId);
        if (forumId == 0)
            return;

        var forum = await GetForumByIdAsync(forumId);
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum → {Forum}", forumId, forum);
        if (forum == null)
            return;

        //number of topics
        var queryNumTopics = from ft in _forumTopicRepository.Table
            where ft.ForumId == forumId
            select ft.Id;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var numTopics = await queryNumTopics.CountAsync();

		_logger.LogInformation("UpdateForumStatsAsync(forumId)={ForumId}: count query executed in {Arg1} ms", forumId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): numTopics → {NumTopics}", forumId, numTopics);
        //number of posts
        var queryNumPosts = from ft in _forumTopicRepository.Table
            join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
            where ft.ForumId == forumId
            select fp.Id;
        var numPosts = await queryNumPosts.CountAsync();
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): numPosts → {NumPosts}", forumId, numPosts);

        //last values
        var lastTopicId = 0;
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastTopicId → {LastTopicId}", forumId, lastTopicId);
        var lastPostId = 0;
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastPostId → {LastPostId}", forumId, lastPostId);
        var lastPostCustomerId = 0;
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastPostCustomerId → {LastPostCustomerId}", forumId, lastPostCustomerId);
        DateTime? lastPostTime = null;
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
				_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): LastTopicId → {LastTopicId}", forumId, LastTopicId);
				_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): LastPostId → {LastPostId}", forumId, LastPostId);
				_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): LastPostCustomerId → {LastPostCustomerId}", forumId, LastPostCustomerId);
				_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): LastPostTime → {LastPostTime}", forumId, LastPostTime);
            };
        var lastValues = await queryLastValues.FirstOrDefaultAsync();
        if (lastValues != null)
        {
			_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastValues → {LastValues}", forumId, lastValues);
            lastTopicId = lastValues.LastTopicId;
            lastPostId = lastValues.LastPostId;
            lastPostCustomerId = lastValues.LastPostCustomerId;
            lastPostTime = lastValues.LastPostTime;
			_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): lastPostTime → {LastPostTime}", forumId, lastPostTime);
        }

        //update forum
        forum.NumTopics = numTopics;
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.NumTopics → {Arg1}", forumId, forum.NumTopics);
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.NumPosts → {Arg1}", forumId, forum.NumPosts);
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.LastTopicId → {Arg1}", forumId, forum.LastTopicId);
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.LastPostId → {Arg1}", forumId, forum.LastPostId);
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.LastPostCustomerId → {Arg1}", forumId, forum.LastPostCustomerId);
		_logger.LogDebug("UpdateForumStatsAsync(forumId={ForumId}): forum.LastPostTime → {Arg1}", forumId, forum.LastPostTime);
        forum.NumPosts = numPosts;
        forum.LastTopicId = lastTopicId;
        forum.LastPostId = lastPostId;
        forum.LastPostCustomerId = lastPostCustomerId;
        forum.LastPostTime = lastPostTime;
        await UpdateForumAsync(forum);
		_logger.LogDebug("<<< Exiting UpdateForumStatsAsync(forumId={ForumId})", forumId);
    }

    /// <summary>
    /// Update forum topic stats
    /// </summary>
    /// <param name="forumTopicId">The forum topic identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateForumTopicStatsAsync(int forumTopicId)
    {
		_logger.LogDebug(">>> Entering UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId})", forumTopicId);
        if (forumTopicId == 0)
            return;

        var forumTopic = await GetTopicByIdAsync(forumTopicId);
		_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic → {ForumTopic}", forumTopicId, forumTopic);
        if (forumTopic == null)
            return;

        //number of posts
        var queryNumPosts = from fp in _forumPostRepository.Table
            where fp.TopicId == forumTopicId
            select fp.Id;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var numPosts = await queryNumPosts.CountAsync();

		_logger.LogInformation("UpdateForumTopicStatsAsync(forumTopicId)={ForumTopicId}: count query executed in {Arg1} ms", forumTopicId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): numPosts → {NumPosts}", forumTopicId, numPosts);
        //last values
        var lastPostId = 0;
		_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): lastPostId → {LastPostId}", forumTopicId, lastPostId);
        var lastPostCustomerId = 0;
		_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): lastPostCustomerId → {LastPostCustomerId}", forumTopicId, lastPostCustomerId);
        DateTime? lastPostTime = null;
        var queryLastValues = from fp in _forumPostRepository.Table
            where fp.TopicId == forumTopicId
            orderby fp.CreatedOnUtc descending
            select new
            {
                LastPostId = fp.Id,
                LastPostCustomerId = fp.CustomerId,
                LastPostTime = fp.CreatedOnUtc
				_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): LastPostId → {LastPostId}", forumTopicId, LastPostId);
				_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): LastPostCustomerId → {LastPostCustomerId}", forumTopicId, LastPostCustomerId);
				_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): LastPostTime → {LastPostTime}", forumTopicId, LastPostTime);
				_logger.LogDebug(">>> Entering GetTopicByIdAsync(forumTopicId={ForumTopicId},increaseViews={IncreaseViews})", forumTopicId, increaseViews);
				_logger.LogInformation("GetTopicByIdAsync(forumTopicId,increaseViews)={ForumTopicId},{IncreaseViews}: find query executed in {Arg2} ms", forumTopicId, increaseViews, __logStart.ElapsedMilliseconds);
				_logger.LogDebug("<<< Exiting GetTopicByIdAsync(forumTopicId={ForumTopicId},increaseViews={IncreaseViews})", forumTopicId, increaseViews);
            };
        var lastValues = await queryLastValues.FirstOrDefaultAsync();
        if (lastValues != null)
        {
            lastPostId = lastValues.LastPostId;
            lastPostCustomerId = lastValues.LastPostCustomerId;
            lastPostTime = lastValues.LastPostTime;
			_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): lastPostTime → {LastPostTime}", forumTopicId, lastPostTime);
        }

        //update topic
        forumTopic.NumPosts = numPosts;
		_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic.NumPosts → {Arg1}", forumTopicId, forumTopic.NumPosts);
		_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic.LastPostId → {Arg1}", forumTopicId, forumTopic.LastPostId);
		_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic.LastPostCustomerId → {Arg1}", forumTopicId, forumTopic.LastPostCustomerId);
		_logger.LogDebug("UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId}): forumTopic.LastPostTime → {Arg1}", forumTopicId, forumTopic.LastPostTime);
        forumTopic.LastPostId = lastPostId;
        forumTopic.LastPostCustomerId = lastPostCustomerId;
        forumTopic.LastPostTime = lastPostTime;

        await UpdateTopicAsync(forumTopic);
		_logger.LogDebug("<<< Exiting UpdateForumTopicStatsAsync(forumTopicId={ForumTopicId})", forumTopicId);
    }

    /// <summary>
    /// Update customer stats
    /// </summary>
    /// <param name="customerId">The customer identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateCustomerStatsAsync(int customerId)
    {
		_logger.LogDebug(">>> Entering UpdateCustomerStatsAsync(customerId={CustomerId})", customerId);
        if (customerId == 0)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(customerId);
		_logger.LogDebug("UpdateCustomerStatsAsync(customerId={CustomerId}): customer → {Customer}", customerId, customer);

        if (customer == null)
            return;

        var query = from fp in _forumPostRepository.Table
            where fp.CustomerId == customerId
            select fp.Id;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var numPosts = await query.CountAsync();

		_logger.LogInformation("UpdateCustomerStatsAsync(customerId)={CustomerId}: count query executed in {Arg1} ms", customerId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("UpdateCustomerStatsAsync(customerId={CustomerId}): numPosts → {NumPosts}", customerId, numPosts);
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
		_logger.LogDebug(">>> Entering GetTopicByIdAsync(forumTopicId={ForumTopicId},increaseViews={IncreaseViews})", forumTopicId, increaseViews);
        var forumTopic = await _forumTopicRepository.GetByIdAsync(forumTopicId, cache => default);

        if (forumTopic == null)
            return null;

        if (!increaseViews)
            return forumTopic;

        forumTopic.Views = ++forumTopic.Views;
		_logger.LogDebug("GetTopicByIdAsync(forumTopicId={ForumTopicId},increaseViews={IncreaseViews}): forumTopic.Views → {Arg2}", forumTopicId, increaseViews, forumTopic.Views);
        await UpdateTopicAsync(forumTopic);

		_logger.LogDebug("<<< Exiting GetTopicByIdAsync(forumTopicId={ForumTopicId},increaseViews={IncreaseViews})", forumTopicId, increaseViews);
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
		_logger.LogDebug(">>> Entering DeleteForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
        await _forumGroupRepository.DeleteAsync(forumGroup);
		_logger.LogDebug(">>> Entering UpdateTopicAsync(forumTopic={ForumTopic})", forumTopic);
    }

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
		_logger.LogDebug(">>> Entering GetForumGroupByIdAsync(forumGroupId={ForumGroupId})", forumGroupId);
		_logger.LogInformation("GetForumGroupByIdAsync(forumGroupId)={ForumGroupId}: find query executing", forumGroupId);
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
		_logger.LogDebug(">>> Entering GetAllForumGroupsAsync()");
        return await _forumGroupRepository.GetAllAsync(query =>
        {
			_logger.LogInformation("GetAllForumGroupsAsync(): find query executing");
            return from fg in query
                orderby fg.DisplayOrder, fg.Id
                select fg;
        }, cache => default);
		_logger.LogDebug("<<< Exiting GetAllForumGroupsAsync()");
    }

    /// <summary>
    /// Inserts a forum group
    /// </summary>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertForumGroupAsync(ForumGroup forumGroup)
    {
		_logger.LogDebug(">>> Entering InsertForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
        await _forumGroupRepository.InsertAsync(forumGroup);
    }

    /// <summary>
    /// Updates the forum group
    /// </summary>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateForumGroupAsync(ForumGroup forumGroup)
    {
		_logger.LogDebug(">>> Entering UpdateForumGroupAsync(forumGroup={ForumGroup})", forumGroup);
        await _forumGroupRepository.UpdateAsync(forumGroup);
    }

    /// <summary>
    /// Deletes a forum
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteForumAsync(Forum forum)
    {
		_logger.LogDebug(">>> Entering DeleteForumAsync(forum={Forum})", forum);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(forum);

        //delete forum subscriptions (topics)
        var queryTopicIds = from ft in _forumTopicRepository.Table
            where ft.ForumId == forum.Id
            select ft.Id;
        var queryFs1 = from fs in _forumSubscriptionRepository.Table
            where queryTopicIds.Contains(fs.TopicId)
            select fs;

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteForumAsync(forum)={Forum}: delete query executed in {Arg1} ms", forum, __logStartDb.ElapsedMilliseconds);
        await _forumSubscriptionRepository.DeleteAsync(await queryFs1.ToListAsync());

        //delete forum subscriptions (forum)
        var queryFs2 = from fs in _forumSubscriptionRepository.Table
            where fs.ForumId == forum.Id
            select fs;

        await _forumSubscriptionRepository.DeleteAsync(await queryFs2.ToListAsync());

        //delete forum
		_logger.LogDebug("<<< Exiting DeleteForumAsync(forum={Forum})", forum);
		_logger.LogInformation("DeleteForumAsync(forum)={Forum}: external service call took {Arg1} ms", forum, __logStart.ElapsedMilliseconds);
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
		_logger.LogDebug(">>> Entering GetForumByIdAsync(forumId={ForumId})", forumId);
		_logger.LogInformation("GetForumByIdAsync(forumId)={ForumId}: find query executing", forumId);
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
        var forums = await _forumRepository.GetAllAsync(query =>
        {
			_logger.LogDebug(">>> Entering GetAllForumsByGroupIdAsync(forumGroupId={ForumGroupId})", forumGroupId);
            return from f in query
                orderby f.DisplayOrder, f.Id
                where f.ForumGroupId == forumGroupId
                select f;
			_logger.LogDebug("<<< Exiting GetAllForumsByGroupIdAsync(forumGroupId={ForumGroupId})", forumGroupId);
				_logger.LogInformation("GetAllForumsByGroupIdAsync(forumGroupId)={ForumGroupId}: find query executed in {Arg1} ms", forumGroupId, __logStart.ElapsedMilliseconds);
        }, cache => cache.PrepareKeyForDefaultCache(NopForumDefaults.ForumByForumGroupCacheKey, forumGroupId));

        return forums;
    }

    /// <summary>
    /// Inserts a forum
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertForumAsync(Forum forum)
    {
		_logger.LogDebug(">>> Entering InsertForumAsync(forum={Forum})", forum);
        await _forumRepository.InsertAsync(forum);
    }

    /// <summary>
    /// Updates the forum
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateForumAsync(Forum forum)
    {
		_logger.LogDebug(">>> Entering UpdateForumAsync(forum={Forum})", forum);
        // if the forum group is changed then clear cache for the previous group 
        // (we can't use the event consumer because it will work after saving the changes in DB)
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var forumToUpdate = await _forumRepository.LoadOriginalCopyAsync(forum);
        if (forumToUpdate.ForumGroupId != forum.ForumGroupId)
            await _staticCacheManager.RemoveAsync(NopForumDefaults.ForumByForumGroupCacheKey, forumToUpdate.ForumGroupId);

		_logger.LogDebug("<<< Exiting UpdateForumAsync(forum={Forum})", forum);
        await _forumRepository.UpdateAsync(forum);
    }

    /// <summary>
    /// Deletes a forum topic
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteTopicAsync(ForumTopic forumTopic)
    {
		_logger.LogDebug(">>> Entering DeleteTopicAsync(forumTopic={ForumTopic})", forumTopic);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(forumTopic);

        var customerId = forumTopic.CustomerId;
		_logger.LogDebug("DeleteTopicAsync(forumTopic={ForumTopic}): customerId → {CustomerId}", forumTopic, customerId);
        var forumId = forumTopic.ForumId;

        //delete topic
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteTopicAsync(forumTopic)={ForumTopic}: delete query executed in {Arg1} ms", forumTopic, __logStartDb.ElapsedMilliseconds);
        await _forumTopicRepository.DeleteAsync(forumTopic);

        //delete forum subscriptions
        var queryFs = from ft in _forumSubscriptionRepository.Table
            where ft.TopicId == forumTopic.Id
            select ft;
        var forumSubscriptions = await queryFs.ToListAsync();

		_logger.LogInformation("DeleteTopicAsync(forumTopic)={ForumTopic}: database query executed in {Arg1} ms", forumTopic, __logStartDb.ElapsedMilliseconds);
		_logger.LogDebug("DeleteTopicAsync(forumTopic={ForumTopic}): forumSubscriptions → {ForumSubscriptions}", forumTopic, forumSubscriptions);
        await _forumSubscriptionRepository.DeleteAsync(forumSubscriptions);

        //update stats
        await UpdateForumStatsAsync(forumId);
		_logger.LogDebug("<<< Exiting DeleteTopicAsync(forumTopic={ForumTopic})", forumTopic);
		_logger.LogInformation("DeleteTopicAsync(forumTopic)={ForumTopic}: external service call took {Arg1} ms", forumTopic, __logStart.ElapsedMilliseconds);
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
		_logger.LogDebug(">>> Entering GetAllTopicsAsync(forumId={ForumId},customerId={CustomerId},keywords={Keywords},searchType={SearchType},limitDays={LimitDays},pageIndex={PageIndex},pageSize={PageSize})", forumId, customerId, keywords, searchType, limitDays, pageIndex, pageSize);
        DateTime? limitDate = null;
        if (limitDays > 0)
            limitDate = DateTime.UtcNow.AddDays(-limitDays);

		_logger.LogDebug("GetAllTopicsAsync(forumId={ForumId},customerId={CustomerId},keywords={Keywords},searchType={SearchType},limitDays={LimitDays},pageIndex={PageIndex},pageSize={PageSize}): limitDate → {LimitDate}", forumId, customerId, keywords, searchType, limitDays, pageIndex, pageSize, limitDate);
        var searchKeywords = !string.IsNullOrEmpty(keywords);
		_logger.LogDebug("GetAllTopicsAsync(forumId={ForumId},customerId={CustomerId},keywords={Keywords},searchType={SearchType},limitDays={LimitDays},pageIndex={PageIndex},pageSize={PageSize}): searchKeywords → {SearchKeywords}", forumId, customerId, keywords, searchType, limitDays, pageIndex, pageSize, searchKeywords);
        var searchTopicTitles = searchType == ForumSearchType.All || searchType == ForumSearchType.TopicTitlesOnly;
        var searchPostText = searchType == ForumSearchType.All || searchType == ForumSearchType.PostTextOnly;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
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
				_logger.LogInformation("GetAllTopicsAsync(forumId,customerId,keywords,searchType,limitDays,pageIndex,pageSize)={ForumId},{CustomerId},{Keywords},{SearchType},{LimitDays},{PageIndex},{PageSize}: find query executed in {Arg7} ms", forumId, customerId, keywords, searchType, limitDays, pageIndex, pageSize, __logStart.ElapsedMilliseconds);

            var query2 = from ft in query
                where query1.Contains(ft.Id)
                orderby ft.TopicTypeId descending, ft.LastPostTime descending, ft.Id descending
                select ft;

            return query2;
        }, pageIndex, pageSize);

		_logger.LogDebug("<<< Exiting GetAllTopicsAsync(forumId={ForumId},customerId={CustomerId},keywords={Keywords},searchType={SearchType},limitDays={LimitDays},pageIndex={PageIndex},pageSize={PageSize})", forumId, customerId, keywords, searchType, limitDays, pageIndex, pageSize);
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
		_logger.LogDebug(">>> Entering GetActiveTopicsAsync(forumId={ForumId},pageIndex={PageIndex},pageSize={PageSize})", forumId, pageIndex, pageSize);
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
		_logger.LogDebug("GetActiveTopicsAsync(forumId={ForumId},pageIndex={PageIndex},pageSize={PageSize}): topics → {Topics}", forumId, pageIndex, pageSize, topics);

		_logger.LogDebug("<<< Exiting GetActiveTopicsAsync(forumId={ForumId},pageIndex={PageIndex},pageSize={PageSize})", forumId, pageIndex, pageSize);
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
		_logger.LogDebug(">>> Entering InsertTopicAsync(forumTopic={ForumTopic},sendNotifications={SendNotifications})", forumTopic, sendNotifications);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumTopicRepository.InsertAsync(forumTopic);

		_logger.LogInformation("InsertTopicAsync(forumTopic,sendNotifications)={ForumTopic},{SendNotifications}: save query executed in {Arg2} ms", forumTopic, sendNotifications, __logStart.ElapsedMilliseconds);
        //update stats
        await UpdateForumStatsAsync(forumTopic.ForumId);

        if (!sendNotifications)
            return;

        //send notifications
        var forum = await GetForumByIdAsync(forumTopic.ForumId);
		_logger.LogDebug("InsertTopicAsync(forumTopic={ForumTopic},sendNotifications={SendNotifications}): forum → {Forum}", forumTopic, sendNotifications, forum);
        var subscriptions = await GetAllSubscriptionsAsync(forumId: forum.Id);
		_logger.LogDebug("InsertTopicAsync(forumTopic={ForumTopic},sendNotifications={SendNotifications}): subscriptions → {Subscriptions}", forumTopic, sendNotifications, subscriptions);
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
		_logger.LogDebug("InsertTopicAsync(forumTopic={ForumTopic},sendNotifications={SendNotifications}): languageId → {LanguageId}", forumTopic, sendNotifications, languageId);

        foreach (var subscription in subscriptions)
        {
            if (subscription.CustomerId == forumTopic.CustomerId)
                continue;

            var customer = await _customerService.GetCustomerByIdAsync(subscription.CustomerId);
			_logger.LogDebug("InsertTopicAsync(forumTopic={ForumTopic},sendNotifications={SendNotifications}): customer → {Customer}", forumTopic, sendNotifications, customer);

            if (!string.IsNullOrEmpty(customer?.Email))
                await _workflowMessageService.SendNewForumTopicMessageAsync(customer, forumTopic, forum, languageId);
        }
		_logger.LogDebug("<<< Exiting InsertTopicAsync(forumTopic={ForumTopic},sendNotifications={SendNotifications})", forumTopic, sendNotifications);
    }

    /// <summary>
    /// Updates the forum topic
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTopicAsync(ForumTopic forumTopic)
    {
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
		_logger.LogDebug(">>> Entering MoveTopicAsync(forumTopicId={ForumTopicId},newForumId={NewForumId})", forumTopicId, newForumId);
        var forumTopic = await GetTopicByIdAsync(forumTopicId);
        if (forumTopic == null)
            return null;

        if (!await IsCustomerAllowedToMoveTopicAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
            return forumTopic;

        var previousForumId = forumTopic.ForumId;
        var newForum = await GetForumByIdAsync(newForumId);
		_logger.LogDebug("MoveTopicAsync(forumTopicId={ForumTopicId},newForumId={NewForumId}): newForum → {NewForum}", forumTopicId, newForumId, newForum);

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
		_logger.LogDebug("<<< Exiting MoveTopicAsync(forumTopicId={ForumTopicId},newForumId={NewForumId})", forumTopicId, newForumId);
        return forumTopic;
    }

    /// <summary>
    /// Deletes a forum post
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePostAsync(ForumPost forumPost)
    {
		_logger.LogDebug(">>> Entering DeletePostAsync(forumPost={ForumPost})", forumPost);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(forumPost);

        var forumTopicId = forumPost.TopicId;
        var customerId = forumPost.CustomerId;
		_logger.LogDebug("DeletePostAsync(forumPost={ForumPost}): customerId → {CustomerId}", forumPost, customerId);
        var forumTopic = await GetTopicByIdAsync(forumTopicId);
		_logger.LogDebug("DeletePostAsync(forumPost={ForumPost}): forumTopic → {ForumTopic}", forumPost, forumTopic);
        var forumId = forumTopic.ForumId;

        //delete topic if it was the first post
        var deleteTopic = false;
		_logger.LogDebug("DeletePostAsync(forumPost={ForumPost}): deleteTopic → {DeleteTopic}", forumPost, deleteTopic);
        var firstPost = await GetFirstPostAsync(forumTopic);
		_logger.LogDebug("DeletePostAsync(forumPost={ForumPost}): firstPost → {FirstPost}", forumPost, firstPost);
        if (firstPost != null && firstPost.Id == forumPost.Id)
            deleteTopic = true;

        //delete forum post
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeletePostAsync(forumPost)={ForumPost}: delete query executed in {Arg1} ms", forumPost, __logStartDb.ElapsedMilliseconds);
        await _forumPostRepository.DeleteAsync(forumPost);

        //delete topic
        if (deleteTopic)
            await DeleteTopicAsync(forumTopic);

        //update stats
        if (!deleteTopic)
            await UpdateForumTopicStatsAsync(forumTopicId);

        await UpdateForumStatsAsync(forumId);
		_logger.LogDebug("<<< Exiting DeletePostAsync(forumPost={ForumPost})", forumPost);
		_logger.LogInformation("DeletePostAsync(forumPost)={ForumPost}: external service call took {Arg1} ms", forumPost, __logStart.ElapsedMilliseconds);
        await UpdateCustomerStatsAsync(customerId);
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
		_logger.LogDebug(">>> Entering GetPostByIdAsync(forumPostId={ForumPostId})", forumPostId);
		_logger.LogInformation("GetPostByIdAsync(forumPostId)={ForumPostId}: find query executing", forumPostId);
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
		_logger.LogDebug(">>> Entering GetAllPostsAsync(forumTopicId={ForumTopicId},customerId={CustomerId},keywords={Keywords},pageIndex={PageIndex},pageSize={PageSize})", forumTopicId, customerId, keywords, pageIndex, pageSize);
        return await GetAllPostsAsync(forumTopicId, customerId, keywords, true,
            pageIndex, pageSize);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogDebug("<<< Exiting GetAllPostsAsync(forumTopicId={ForumTopicId},customerId={CustomerId},keywords={Keywords},ascSort={AscSort},pageIndex={PageIndex},pageSize={PageSize})", forumTopicId, customerId, keywords, ascSort, pageIndex, pageSize);
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
		_logger.LogDebug(">>> Entering GetAllPostsAsync(forumTopicId={ForumTopicId},customerId={CustomerId},keywords={Keywords},ascSort={AscSort},pageIndex={PageIndex},pageSize={PageSize})", forumTopicId, customerId, keywords, ascSort, pageIndex, pageSize);
        var forumPosts = await _forumPostRepository.GetAllPagedAsync(query =>
        {
            if (forumTopicId > 0)
                query = query.Where(fp => forumTopicId == fp.TopicId);

			_logger.LogInformation("GetAllPostsAsync(forumTopicId,customerId,keywords,ascSort,pageIndex,pageSize)={ForumTopicId},{CustomerId},{Keywords},{AscSort},{PageIndex},{PageSize}: find query executed in {Arg6} ms", forumTopicId, customerId, keywords, ascSort, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("GetAllPostsAsync(forumTopicId={ForumTopicId},customerId={CustomerId},keywords={Keywords},ascSort={AscSort},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", forumTopicId, customerId, keywords, ascSort, pageIndex, pageSize, query);
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
		_logger.LogDebug(">>> Entering InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications})", forumPost, sendNotifications);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumPostRepository.InsertAsync(forumPost);

		_logger.LogInformation("InsertPostAsync(forumPost,sendNotifications)={ForumPost},{SendNotifications}: save query executed in {Arg2} ms", forumPost, sendNotifications, __logStart.ElapsedMilliseconds);
        //update stats
        var customerId = forumPost.CustomerId;
		_logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): customerId → {CustomerId}", forumPost, sendNotifications, customerId);
        var forumTopic = await GetTopicByIdAsync(forumPost.TopicId);
		_logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): forumTopic → {ForumTopic}", forumPost, sendNotifications, forumTopic);
        var forumId = forumTopic.ForumId;

        await UpdateForumTopicStatsAsync(forumPost.TopicId);
        await UpdateForumStatsAsync(forumId);
        await UpdateCustomerStatsAsync(customerId);

        //notifications
        if (!sendNotifications)
            return;

        var forum = await GetForumByIdAsync(forumTopic.ForumId);
		_logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): forum → {Forum}", forumPost, sendNotifications, forum);
        var subscriptions = await GetAllSubscriptionsAsync(topicId: forumTopic.Id);
		_logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): subscriptions → {Subscriptions}", forumPost, sendNotifications, subscriptions);

        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
		_logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): languageId → {LanguageId}", forumPost, sendNotifications, languageId);

        var friendlyTopicPageIndex = await CalculateTopicPageIndexAsync(forumPost.TopicId,
            _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10,
            forumPost.Id) + 1;

        foreach (var subscription in subscriptions)
        {
            if (subscription.CustomerId == forumPost.CustomerId)
                continue;

            var customer = await _customerService.GetCustomerByIdAsync(subscription.CustomerId);
			_logger.LogDebug("InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications}): customer → {Customer}", forumPost, sendNotifications, customer);

            if (!string.IsNullOrEmpty(customer?.Email))
                await _workflowMessageService.SendNewForumPostMessageAsync(customer, forumPost, forumTopic, forum, friendlyTopicPageIndex, languageId);
        }
		_logger.LogDebug("<<< Exiting InsertPostAsync(forumPost={ForumPost},sendNotifications={SendNotifications})", forumPost, sendNotifications);
    }

    /// <summary>
    /// Updates the forum post
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePostAsync(ForumPost forumPost)
    {
		_logger.LogDebug(">>> Entering UpdatePostAsync(forumPost={ForumPost})", forumPost);
        await _forumPostRepository.UpdateAsync(forumPost);
    }

    /// <summary>
    /// Deletes a private message
    /// </summary>
    /// <param name="privateMessage">Private message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePrivateMessageAsync(PrivateMessage privateMessage)
    {
		_logger.LogDebug(">>> Entering DeletePrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
        await _forumPrivateMessageRepository.DeleteAsync(privateMessage);
    }

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
		_logger.LogDebug(">>> Entering GetPrivateMessageByIdAsync(privateMessageId={PrivateMessageId})", privateMessageId);
		_logger.LogInformation("GetPrivateMessageByIdAsync(privateMessageId)={PrivateMessageId}: find query executing", privateMessageId);
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
			_logger.LogDebug(">>> Entering GetAllPrivateMessagesAsync(storeId={StoreId},fromCustomerId={FromCustomerId},toCustomerId={ToCustomerId},isRead={IsRead},isDeletedByAuthor={IsDeletedByAuthor},isDeletedByRecipient={IsDeletedByRecipient},keywords={Keywords},pageIndex={PageIndex},pageSize={PageSize})", storeId, fromCustomerId, toCustomerId, isRead, isDeletedByAuthor, isDeletedByRecipient, keywords, pageIndex, pageSize);
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

			_logger.LogDebug("<<< Exiting GetAllPrivateMessagesAsync(storeId={StoreId},fromCustomerId={FromCustomerId},toCustomerId={ToCustomerId},isRead={IsRead},isDeletedByAuthor={IsDeletedByAuthor},isDeletedByRecipient={IsDeletedByRecipient},keywords={Keywords},pageIndex={PageIndex},pageSize={PageSize})", storeId, fromCustomerId, toCustomerId, isRead, isDeletedByAuthor, isDeletedByRecipient, keywords, pageIndex, pageSize);
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
		_logger.LogDebug(">>> Entering InsertPrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumPrivateMessageRepository.InsertAsync(privateMessage);

		_logger.LogInformation("InsertPrivateMessageAsync(privateMessage)={PrivateMessage}: save query executed in {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
        var customerTo = await _customerService.GetCustomerByIdAsync(privateMessage.ToCustomerId)
                         ?? throw new NopException("Recipient could not be loaded");

        //UI notification
        await _genericAttributeService.SaveAttributeAsync(customerTo, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, false, privateMessage.StoreId);

        //Email notification
        if (_forumSettings.NotifyAboutPrivateMessages)
            await _workflowMessageService.SendPrivateMessageNotificationAsync(privateMessage, (await _workContext.GetWorkingLanguageAsync()).Id);
			_logger.LogDebug("<<< Exiting InsertPrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
    }

    /// <summary>
    /// Updates the private message
    /// </summary>
    /// <param name="privateMessage">Private message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePrivateMessageAsync(PrivateMessage privateMessage)
    {
		_logger.LogDebug(">>> Entering UpdatePrivateMessageAsync(privateMessage={PrivateMessage})", privateMessage);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(privateMessage);

			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
        if (privateMessage.IsDeletedByAuthor && privateMessage.IsDeletedByRecipient) {
			_logger.LogInformation("UpdatePrivateMessageAsync(privateMessage)={PrivateMessage}: delete query executed in {Arg1} ms", privateMessage, __logStartDb.ElapsedMilliseconds);
        }
			_logger.LogInformation("UpdatePrivateMessageAsync(privateMessage)={PrivateMessage}: external service call took {Arg1} ms", privateMessage, __logStart.ElapsedMilliseconds);
            await _forumPrivateMessageRepository.DeleteAsync(privateMessage);
        else
            await _forumPrivateMessageRepository.UpdateAsync(privateMessage);
    }

    /// <summary>
    /// Deletes a forum subscription
    /// </summary>
    /// <param name="forumSubscription">Forum subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSubscriptionAsync(ForumSubscription forumSubscription)
    {
		_logger.LogDebug(">>> Entering DeleteSubscriptionAsync(forumSubscription={ForumSubscription})", forumSubscription);
        await _forumSubscriptionRepository.DeleteAsync(forumSubscription);
    }

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
		_logger.LogDebug(">>> Entering GetSubscriptionByIdAsync(forumSubscriptionId={ForumSubscriptionId})", forumSubscriptionId);
		_logger.LogInformation("GetSubscriptionByIdAsync(forumSubscriptionId)={ForumSubscriptionId}: find query executing", forumSubscriptionId);
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
			_logger.LogDebug(">>> Entering GetAllSubscriptionsAsync(customerId={CustomerId},forumId={ForumId},topicId={TopicId},pageIndex={PageIndex},pageSize={PageSize})", customerId, forumId, topicId, pageIndex, pageSize);
            var fsQuery = from fs in query
                join c in _customerRepository.Table on fs.CustomerId equals c.Id
                where
                    (customerId == 0 || fs.CustomerId == customerId) &&
                    (forumId == 0 || fs.ForumId == forumId) &&
                    (topicId == 0 || fs.TopicId == topicId) &&
                    c.Active &&
                    !c.Deleted
                select fs.SubscriptionGuid;
				_logger.LogInformation("GetAllSubscriptionsAsync(customerId,forumId,topicId,pageIndex,pageSize)={CustomerId},{ForumId},{TopicId},{PageIndex},{PageSize}: find query executed in {Arg5} ms", customerId, forumId, topicId, pageIndex, pageSize, __logStart.ElapsedMilliseconds);

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
		_logger.LogDebug(">>> Entering InsertSubscriptionAsync(forumSubscription={ForumSubscription})", forumSubscription);
        await _forumSubscriptionRepository.InsertAsync(forumSubscription);
    }

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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToCreateTopicAsync(customer={Customer},forum={Forum})", customer, forum);
        if (forum == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer) && !_forumSettings.AllowGuestsToCreateTopics)
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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToEditTopicAsync(customer={Customer},topic={Topic})", customer, topic);
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

		_logger.LogDebug("<<< Exiting IsCustomerAllowedToEditTopicAsync(customer={Customer},topic={Topic})", customer, topic);
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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToMoveTopicAsync(customer={Customer},topic={Topic})", customer, topic);
        if (topic == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer))
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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToDeleteTopicAsync(customer={Customer},topic={Topic})", customer, topic);
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

		_logger.LogDebug("<<< Exiting IsCustomerAllowedToDeleteTopicAsync(customer={Customer},topic={Topic})", customer, topic);
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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToCreatePostAsync(customer={Customer},topic={Topic})", customer, topic);
        if (topic == null)
            return false;

        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer) && !_forumSettings.AllowGuestsToCreatePosts)
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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToEditPostAsync(customer={Customer},post={Post})", customer, post);
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

		_logger.LogDebug("<<< Exiting IsCustomerAllowedToEditPostAsync(customer={Customer},post={Post})", customer, post);
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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToDeletePostAsync(customer={Customer},post={Post})", customer, post);
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

		_logger.LogDebug("<<< Exiting IsCustomerAllowedToDeletePostAsync(customer={Customer},post={Post})", customer, post);
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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToSetTopicPriorityAsync(customer={Customer})", customer);
        if (customer == null)
            return false;

        if (await _customerService.IsGuestAsync(customer))
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
		_logger.LogDebug(">>> Entering IsCustomerAllowedToSubscribeAsync(customer={Customer})", customer);
        if (customer == null)
            return false;

        if (customer.IsSearchEngineAccount() || await _customerService.IsGuestAsync(customer))
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
		_logger.LogDebug(">>> Entering CalculateTopicPageIndexAsync(forumTopicId={ForumTopicId},pageSize={PageSize},postId={PostId})", forumTopicId, pageSize, postId);
        var pageIndex = 0;
        var forumPosts = await GetAllPostsAsync(forumTopicId, ascSort: true);
		_logger.LogDebug("CalculateTopicPageIndexAsync(forumTopicId={ForumTopicId},pageSize={PageSize},postId={PostId}): forumPosts → {ForumPosts}", forumTopicId, pageSize, postId, forumPosts);
		_logger.LogDebug("CalculateTopicPageIndexAsync(forumTopicId={ForumTopicId},pageSize={PageSize},postId={PostId}): pageIndex → {PageIndex}", forumTopicId, pageSize, postId, pageIndex);

        for (var i = 0; i < forumPosts.TotalCount; i++)
        {
            if (forumPosts[i].Id != postId)
                continue;

            if (pageSize > 0)
                pageIndex = i / pageSize;
        }

		_logger.LogDebug("<<< Exiting CalculateTopicPageIndexAsync(forumTopicId={ForumTopicId},pageSize={PageSize},postId={PostId})", forumTopicId, pageSize, postId);
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
		_logger.LogDebug(">>> Entering GetPostVoteAsync(postId={PostId},customer={Customer})", postId, customer);
        if (customer == null)
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
		_logger.LogDebug(">>> Entering GetNumberOfPostVotesAsync(customer={Customer},createdFromUtc={CreatedFromUtc})", customer, createdFromUtc);
        if (customer == null)
            return 0;

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
		_logger.LogDebug(">>> Entering InsertPostVoteAsync(postVote={PostVote})", postVote);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _forumPostVoteRepository.InsertAsync(postVote);

		_logger.LogInformation("InsertPostVoteAsync(postVote)={PostVote}: save query executed in {Arg1} ms", postVote, __logStart.ElapsedMilliseconds);
        //update post
        var post = await GetPostByIdAsync(postVote.ForumPostId);
		_logger.LogDebug("InsertPostVoteAsync(postVote={PostVote}): post → {Post}", postVote, post);
		_logger.LogDebug("InsertPostVoteAsync(postVote={PostVote}): post.VoteCount → {Arg1}", postVote, post.VoteCount);
        post.VoteCount = postVote.IsUp ? ++post.VoteCount : --post.VoteCount;

        await UpdatePostAsync(post);
		_logger.LogDebug("<<< Exiting InsertPostVoteAsync(postVote={PostVote})", postVote);
    }

    /// <summary>
    /// Delete a post vote
    /// </summary>
    /// <param name="postVote">Post vote</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePostVoteAsync(ForumPostVote postVote)
    {
		_logger.LogDebug(">>> Entering DeletePostVoteAsync(postVote={PostVote})", postVote);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(postVote);

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeletePostVoteAsync(postVote)={PostVote}: delete query executed in {Arg1} ms", postVote, __logStartDb.ElapsedMilliseconds);
        await _forumPostVoteRepository.DeleteAsync(postVote);

        // update post
        var post = await GetPostByIdAsync(postVote.ForumPostId);
		_logger.LogDebug("DeletePostVoteAsync(postVote={PostVote}): post → {Post}", postVote, post);
		_logger.LogDebug("DeletePostVoteAsync(postVote={PostVote}): post.VoteCount → {Arg1}", postVote, post.VoteCount);
		_logger.LogInformation("DeletePostVoteAsync(postVote)={PostVote}: external service call took {Arg1} ms", postVote, __logStart.ElapsedMilliseconds);
        post.VoteCount = postVote.IsUp ? --post.VoteCount : ++post.VoteCount;

		_logger.LogDebug("<<< Exiting DeletePostVoteAsync(postVote={PostVote})", postVote);
        await UpdatePostAsync(post);
    }

    /// <summary>
    /// Formats the forum post text
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatPostText(ForumPost forumPost)
    {
		_logger.LogDebug(">>> Entering FormatPostText(forumPost={ForumPost})", forumPost);
        var text = forumPost.Text;

		_logger.LogDebug("FormatPostText(forumPost={ForumPost}): text → {Text}", forumPost, text);
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

		_logger.LogDebug("<<< Exiting FormatPostText(forumPost={ForumPost})", forumPost);
        return text;
    }

    /// <summary>
    /// Strips the topic subject
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>Formatted subject</returns>
    public virtual string StripTopicSubject(ForumTopic forumTopic)
    {
		_logger.LogDebug(">>> Entering StripTopicSubject(forumTopic={ForumTopic})", forumTopic);
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

		_logger.LogDebug("<<< Exiting StripTopicSubject(forumTopic={ForumTopic})", forumTopic);
        return subject;
    }

    /// <summary>
    /// Formats the forum signature text
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatForumSignatureText(string text)
    {
		_logger.LogDebug(">>> Entering FormatForumSignatureText(text={Text})", text);
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);
		_logger.LogDebug("FormatForumSignatureText(text={Text}): text → {Text1}", text, text);
		_logger.LogDebug("<<< Exiting FormatForumSignatureText(text={Text})", text);
        return text;
    }

    /// <summary>
    /// Formats the private message text
    /// </summary>
    /// <param name="pm">Private message</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatPrivateMessageText(PrivateMessage pm)
    {
		_logger.LogDebug(">>> Entering FormatPrivateMessageText(pm={Pm})", pm);
        var text = pm.Text;

		_logger.LogDebug("FormatPrivateMessageText(pm={Pm}): text → {Text}", pm, text);
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = _htmlFormatter.FormatText(text, false, true, false, true, false, false);

		_logger.LogDebug("<<< Exiting FormatPrivateMessageText(pm={Pm})", pm);
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
		_logger.LogDebug(">>> Entering GetFirstPostAsync(forumTopic={ForumTopic})", forumTopic);
        ArgumentNullException.ThrowIfNull(forumTopic);

        var forumPosts = await GetAllPostsAsync(forumTopic.Id, 0, string.Empty, 0, 1);
		_logger.LogDebug("GetFirstPostAsync(forumTopic={ForumTopic}): forumPosts → {ForumPosts}", forumTopic, forumPosts);
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
		_logger.LogDebug(">>> Entering GetForumGroupSeNameAsync(forumGroup={ForumGroup})", forumGroup);
        ArgumentNullException.ThrowIfNull(forumGroup);

        var seName = await _urlRecordService.GetSeNameAsync(forumGroup.Name, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);
		_logger.LogDebug("GetForumGroupSeNameAsync(forumGroup={ForumGroup}): seName → {SeName}", forumGroup, seName);

		_logger.LogDebug("<<< Exiting GetForumGroupSeNameAsync(forumGroup={ForumGroup})", forumGroup);
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
		_logger.LogDebug(">>> Entering GetForumSeNameAsync(forum={Forum})", forum);
        ArgumentNullException.ThrowIfNull(forum);

        var seName = await _urlRecordService.GetSeNameAsync(forum.Name, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);
		_logger.LogDebug("GetForumSeNameAsync(forum={Forum}): seName → {SeName}", forum, seName);

		_logger.LogDebug("<<< Exiting GetForumSeNameAsync(forum={Forum})", forum);
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
		_logger.LogDebug(">>> Entering GetTopicSeNameAsync(forumTopic={ForumTopic})", forumTopic);
        ArgumentNullException.ThrowIfNull(forumTopic);

        var seName = await _urlRecordService.GetSeNameAsync(forumTopic.Subject, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);
		_logger.LogDebug("GetTopicSeNameAsync(forumTopic={ForumTopic}): seName → {SeName}", forumTopic, seName);

        // Trim SE name to avoid URLs that are too long
        var maxLength = NopSeoDefaults.ForumTopicLength;
        if (seName.Length > maxLength)
            seName = seName[0..maxLength];

		_logger.LogDebug("<<< Exiting GetTopicSeNameAsync(forumTopic={ForumTopic})", forumTopic);
        return seName;
    }

    #endregion
}