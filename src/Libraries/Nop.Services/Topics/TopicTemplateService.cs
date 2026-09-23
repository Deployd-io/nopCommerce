using Nop.Core.Domain.Topics;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Topics;

/// <summary>
/// Topic template service
/// </summary>
public partial class TopicTemplateService : ITopicTemplateService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<TopicTemplate> _topicTemplateRepository;

    #endregion

    #region Ctor

    public TopicTemplateService(IRepository<TopicTemplate> topicTemplateRepository)
    {
		_logger.LogError(">>> Entering TopicTemplateService(topicTemplateRepository={TopicTemplateRepository})", topicTemplateRepository);
        _topicTemplateRepository = topicTemplateRepository;
		_logger.LogDebug("TopicTemplateService(topicTemplateRepository={TopicTemplateRepository}): _topicTemplateRepository → {_topicTemplateRepository}", topicTemplateRepository, _topicTemplateRepository);
    _logger.LogWarning("<<< Exiting TopicTemplateService(topicTemplateRepository={TopicTemplateRepository})", topicTemplateRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete topic template
    /// </summary>
    /// <param name="topicTemplate">Topic template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteTopicTemplateAsync(TopicTemplate topicTemplate)
    {
		_logger.LogError(">>> Entering DeleteTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _topicTemplateRepository.DeleteAsync(topicTemplate);
		_logger.LogError("DeleteTopicTemplateAsync(topicTemplate)={TopicTemplate}: external service call took {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
    }

    /// <summary>
    /// Gets all topic templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic templates
    /// </returns>
    public virtual async Task<IList<TopicTemplate>> GetAllTopicTemplatesAsync()
    {
		_logger.LogError(">>> Entering GetAllTopicTemplatesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _topicTemplateRepository.GetAllAsync(query =>
        {
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
        }, cache => default);
		_logger.LogError("GetAllTopicTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllTopicTemplatesAsync()");
        return templates;
    }

    /// <summary>
    /// Gets a topic template
    /// </summary>
    /// <param name="topicTemplateId">Topic template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic template
    /// </returns>
    public virtual async Task<TopicTemplate> GetTopicTemplateByIdAsync(int topicTemplateId)
    {
		_logger.LogError(">>> Entering GetTopicTemplateByIdAsync(topicTemplateId={TopicTemplateId})", topicTemplateId);
		_logger.LogError("GetTopicTemplateByIdAsync(topicTemplateId)={TopicTemplateId}: find query executing", topicTemplateId);
    _logger.LogWarning("<<< Exiting GetTopicTemplateByIdAsync(topicTemplateId={TopicTemplateId})", topicTemplateId);
        return await _topicTemplateRepository.GetByIdAsync(topicTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts topic template
    /// </summary>
    /// <param name="topicTemplate">Topic template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTopicTemplateAsync(TopicTemplate topicTemplate)
    {
		_logger.LogError(">>> Entering InsertTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
        await _topicTemplateRepository.InsertAsync(topicTemplate);
		_logger.LogWarning("<<< Exiting InsertTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
    }

    /// <summary>
    /// Updates the topic template
    /// </summary>
    /// <param name="topicTemplate">Topic template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTopicTemplateAsync(TopicTemplate topicTemplate)
    {
		_logger.LogError(">>> Entering UpdateTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
        await _topicTemplateRepository.UpdateAsync(topicTemplate);
		_logger.LogWarning("<<< Exiting UpdateTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
    }

    #endregion
}