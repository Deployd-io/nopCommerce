using Nop.Core.Domain.Topics;
using Nop.Data;

namespace Nop.Services.Topics;

/// <summary>
/// Topic template service
/// </summary>
public partial class TopicTemplateService : ITopicTemplateService
{
    #region Fields

    protected readonly IRepository<TopicTemplate> _topicTemplateRepository;

    #endregion

    #region Ctor

    public TopicTemplateService(IRepository<TopicTemplate> topicTemplateRepository)
    {
		logger.LogDebug(">>> Entering TopicTemplateService(topicTemplateRepository={TopicTemplateRepository})", topicTemplateRepository);
        _topicTemplateRepository = topicTemplateRepository;
		logger.LogDebug("<<< Exiting TopicTemplateService(topicTemplateRepository={TopicTemplateRepository})", topicTemplateRepository);
		logger.LogDebug("TopicTemplateService(topicTemplateRepository={TopicTemplateRepository}): _topicTemplateRepository → {_topicTemplateRepository}", topicTemplateRepository, _topicTemplateRepository);
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
		logger.LogDebug(">>> Entering DeleteTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _topicTemplateRepository.DeleteAsync(topicTemplate);
		logger.LogDebug("<<< Exiting DeleteTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTopicTemplateAsync(topicTemplate)={TopicTemplate}: external service call took {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTopicTemplateAsync(topicTemplate)={TopicTemplate}: delete query executed in {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteTopicTemplateAsync(topicTemplate)={TopicTemplate}: external service call took {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteTopicTemplateAsync(topicTemplate)={TopicTemplate}: delete query executed in {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets all topic templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic templates
    /// </returns>
    public virtual async Task<IList<TopicTemplate>> GetAllTopicTemplatesAsync()
    {
		logger.LogDebug(">>> Entering GetAllTopicTemplatesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _topicTemplateRepository.GetAllAsync(query =>
        {
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
				logger.LogInformation("GetAllTopicTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllTopicTemplatesAsync()");
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
		logger.LogDebug(">>> Entering GetTopicTemplateByIdAsync(topicTemplateId={TopicTemplateId})", topicTemplateId);
		logger.LogDebug("<<< Exiting GetTopicTemplateByIdAsync(topicTemplateId={TopicTemplateId})", topicTemplateId);
        return await _topicTemplateRepository.GetByIdAsync(topicTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts topic template
    /// </summary>
    /// <param name="topicTemplate">Topic template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTopicTemplateAsync(TopicTemplate topicTemplate)
    {
		logger.LogDebug(">>> Entering InsertTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _topicTemplateRepository.InsertAsync(topicTemplate);
		logger.LogDebug("<<< Exiting InsertTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertTopicTemplateAsync(topicTemplate)={TopicTemplate}: save query executed in {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertTopicTemplateAsync(topicTemplate)={TopicTemplate}: save query executed in {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the topic template
    /// </summary>
    /// <param name="topicTemplate">Topic template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTopicTemplateAsync(TopicTemplate topicTemplate)
    {
		logger.LogDebug(">>> Entering UpdateTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _topicTemplateRepository.UpdateAsync(topicTemplate);
		logger.LogDebug("<<< Exiting UpdateTopicTemplateAsync(topicTemplate={TopicTemplate})", topicTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateTopicTemplateAsync(topicTemplate)={TopicTemplate}: update query executed in {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateTopicTemplateAsync(topicTemplate)={TopicTemplate}: update query executed in {Arg1} ms", topicTemplate, __logStart.ElapsedMilliseconds);
    #endregion
}
