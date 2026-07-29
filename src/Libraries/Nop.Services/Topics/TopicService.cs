using System.Data.SqlTypes;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Topics;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Topics;

/// <summary>
/// Topic service
/// </summary>
public partial class TopicService : ITopicService
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICustomerService _customerService;
    protected readonly IRepository<Topic> _topicRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public TopicService(
        IAclService aclService,
        ICustomerService customerService,
        IRepository<Topic> topicRepository,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService,
        IWorkContext workContext)
    {
        _aclService = aclService;
        _customerService = customerService;
        _topicRepository = topicRepository;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a topic
    /// </summary>
    /// <param name="topic">Topic</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteTopicAsync(Topic topic)
    {
		logger.LogDebug(">>> Entering DeleteTopicAsync(topic={Topic})", topic);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _topicRepository.DeleteAsync(topic);
		logger.LogDebug("<<< Exiting DeleteTopicAsync(topic={Topic})", topic);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTopicAsync(topic)={Topic}: external service call took {Arg1} ms", topic, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTopicAsync(topic)={Topic}: delete query executed in {Arg1} ms", topic, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteTopicAsync(topic)={Topic}: external service call took {Arg1} ms", topic, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteTopicAsync(topic)={Topic}: delete query executed in {Arg1} ms", topic, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a topic
    /// </summary>
    /// <param name="topicId">The topic identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic
    /// </returns>
    public virtual async Task<Topic> GetTopicByIdAsync(int topicId)
    {
		logger.LogDebug(">>> Entering GetTopicByIdAsync(topicId={TopicId})", topicId);
		logger.LogDebug("<<< Exiting GetTopicByIdAsync(topicId={TopicId})", topicId);
        return await _topicRepository.GetByIdAsync(topicId, cache => default);
    }

    /// <summary>
    /// Gets a topic
    /// </summary>
    /// <param name="systemName">The topic system name</param>
    /// <param name="storeId">Store identifier; pass 0 to ignore filtering by store and load the first one</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic
    /// </returns>
    public virtual async Task<Topic> GetTopicBySystemNameAsync(string systemName, int storeId = 0)
    {
		logger.LogDebug(">>> Entering GetTopicBySystemNameAsync(systemName={SystemName},storeId={StoreId})", systemName, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrEmpty(systemName))
            return null;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		logger.LogDebug("GetTopicBySystemNameAsync(systemName={SystemName},storeId={StoreId}): customer → {Customer}", systemName, storeId, customer);
		logger.LogDebug("GetTopicBySystemNameAsync(systemName={SystemName},storeId={StoreId}): customerRoleIds → {CustomerRoleIds}", systemName, storeId, customerRoleIds);

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopTopicDefaults.TopicBySystemNameCacheKey, systemName, storeId, customerRoleIds);

		logger.LogDebug("GetTopicBySystemNameAsync(systemName={SystemName},storeId={StoreId}): cacheKey → {CacheKey}", systemName, storeId, cacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var query = _topicRepository.Table
                .Where(t => t.Published);

            query = query.Where(t =>
                DateTime.UtcNow >= (t.AvailableStartDateTimeUtc ?? SqlDateTime.MinValue.Value) &&
                DateTime.UtcNow <= (t.AvailableEndDateTimeUtc ?? SqlDateTime.MaxValue.Value));

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            query = await _aclService.ApplyAcl(query, customerRoleIds);

            return await query.Where(t => t.SystemName == systemName)
                .OrderBy(t => t.Id)
                .FirstOrDefaultAsync();
				logger.LogInformation("GetTopicBySystemNameAsync(systemName,storeId)={SystemName},{StoreId}: database query executed in {Arg2} ms", systemName, storeId, __logStart.ElapsedMilliseconds);
        });
		logger.LogInformation("GetTopicBySystemNameAsync(systemName,storeId)={SystemName},{StoreId}: external service call took {Arg2} ms", systemName, storeId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting GetTopicBySystemNameAsync(systemName={SystemName},storeId={StoreId})", systemName, storeId);
    }

    /// <summary>
    /// Gets all topics
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
    /// <param name="showHidden">A value indicating whether to show hidden topics</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topics
    /// </returns>
    public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId, bool ignoreAcl = false, bool showHidden = false)
    {
		logger.LogDebug(">>> Entering GetAllTopicsAsync(storeId={StoreId},ignoreAcl={IgnoreAcl},showHidden={ShowHidden})", storeId, ignoreAcl, showHidden);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		logger.LogDebug("GetAllTopicsAsync(storeId={StoreId},ignoreAcl={IgnoreAcl},showHidden={ShowHidden}): customer → {Customer}", storeId, ignoreAcl, showHidden, customer);
		logger.LogDebug("GetAllTopicsAsync(storeId={StoreId},ignoreAcl={IgnoreAcl},showHidden={ShowHidden}): customerRoleIds → {CustomerRoleIds}", storeId, ignoreAcl, showHidden, customerRoleIds);

        return await _topicRepository.GetAllAsync(async query =>
        {
		logger.LogDebug("GetAllTopicsAsync(storeId={StoreId},ignoreAcl={IgnoreAcl},showHidden={ShowHidden}): query → {Query}", storeId, ignoreAcl, showHidden, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
				logger.LogInformation("GetAllTopicsAsync(storeId,ignoreAcl,showHidden)={StoreId},{IgnoreAcl},{ShowHidden}: find query executed in {Arg3} ms", storeId, ignoreAcl, showHidden, __logStart.ElapsedMilliseconds);
            }

            if (!showHidden)
            {
                query = query.Where(t => t.Published);

                query = query.Where(t =>
                    DateTime.UtcNow >= (t.AvailableStartDateTimeUtc ?? SqlDateTime.MinValue.Value) &&
                    DateTime.UtcNow <= (t.AvailableEndDateTimeUtc ?? SqlDateTime.MaxValue.Value));

                //apply ACL constraints
                if (!ignoreAcl)
                    query = await _aclService.ApplyAcl(query, customerRoleIds);
            }

            return query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
        }, cache =>
        {
            return ignoreAcl
                ? cache.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllCacheKey, storeId, showHidden)
                : cache.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllWithACLCacheKey, storeId, showHidden, customerRoleIds);
        });
		logger.LogDebug("<<< Exiting GetAllTopicsAsync(storeId={StoreId},ignoreAcl={IgnoreAcl},showHidden={ShowHidden})", storeId, ignoreAcl, showHidden);
    }

    /// <summary>
    /// Gets all topics
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="keywords">Keywords to search into body or title</param>
    /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
    /// <param name="showHidden">A value indicating whether to show hidden topics</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topics
    /// </returns>
    public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId, string keywords,
        bool ignoreAcl = false, bool showHidden = false)
    {
        var topics = await GetAllTopicsAsync(storeId,
            ignoreAcl: ignoreAcl,
            showHidden: showHidden);

        if (!string.IsNullOrWhiteSpace(keywords))
        {
            return topics
                .Where(topic => (topic.Title?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                                (topic.Body?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false))
                .ToList();
        }

        return topics;
    }

    /// <summary>
    /// Inserts a topic
    /// </summary>
    /// <param name="topic">Topic</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTopicAsync(Topic topic)
    {
		logger.LogDebug(">>> Entering InsertTopicAsync(topic={Topic})", topic);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _topicRepository.InsertAsync(topic);
		logger.LogDebug("<<< Exiting InsertTopicAsync(topic={Topic})", topic);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertTopicAsync(topic)={Topic}: save query executed in {Arg1} ms", topic, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertTopicAsync(topic)={Topic}: save query executed in {Arg1} ms", topic, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Get a value indicating whether a topic is available (availability dates)
    /// </summary>
    /// <param name="topic">Topic</param>
    /// <param name="dateTime">Datetime to check; pass null to use current date</param>
    /// <returns>Result</returns>
    public virtual bool TopicIsAvailable(Topic topic, DateTime? dateTime = null)
    {
		logger.LogDebug(">>> Entering TopicIsAvailable(topic={Topic},dateTime={DateTime})", topic, dateTime);
        ArgumentNullException.ThrowIfNull(topic);

        dateTime ??= DateTime.UtcNow;

        if (topic.AvailableStartDateTimeUtc.HasValue && topic.AvailableStartDateTimeUtc.Value > dateTime)
            return false;

        if (topic.AvailableEndDateTimeUtc.HasValue && topic.AvailableEndDateTimeUtc.Value < dateTime)
		logger.LogDebug("<<< Exiting TopicIsAvailable(topic={Topic},dateTime={DateTime})", topic, dateTime);
            return false;

        return true;
    }

    /// <summary>
    /// Updates the topic
    /// </summary>
    /// <param name="topic">Topic</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTopicAsync(Topic topic)
    {
		logger.LogDebug(">>> Entering UpdateTopicAsync(topic={Topic})", topic);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _topicRepository.UpdateAsync(topic);
		logger.LogDebug("<<< Exiting UpdateTopicAsync(topic={Topic})", topic);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateTopicAsync(topic)={Topic}: update query executed in {Arg1} ms", topic, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateTopicAsync(topic)={Topic}: update query executed in {Arg1} ms", topic, __logStart.ElapsedMilliseconds);
    #endregion
}