using Nop.Core;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Models.Topics;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the topic model factory
/// </summary>
public partial class TopicModelFactory : ITopicModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreContext _storeContext;
    protected readonly ITopicService _topicService;
    protected readonly ITopicTemplateService _topicTemplateService;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public TopicModelFactory(ILocalizationService localizationService,
        IStoreContext storeContext,
        ITopicService topicService,
        ITopicTemplateService topicTemplateService,
        IUrlRecordService urlRecordService)
    {
        _localizationService = localizationService;
        _storeContext = storeContext;
        _topicService = topicService;
        _topicTemplateService = topicTemplateService;
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the topic model
    /// </summary>
    /// <param name="topic">Topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic model
    /// </returns>
    public virtual async Task<TopicModel> PrepareTopicModelAsync(Topic topic)
    {
        ArgumentNullException.ThrowIfNull(topic);

        return new TopicModel
        {
            Id = topic.Id,
            SystemName = topic.SystemName,
            IncludeInSitemap = topic.IncludeInSitemap,
            IsPasswordProtected = topic.IsPasswordProtected,
            Title = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalizedAsync(topic, x => x.Title),
            Body = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalizedAsync(topic, x => x.Body),
            MetaKeywords = await _localizationService.GetLocalizedAsync(topic, x => x.MetaKeywords),
            MetaDescription = await _localizationService.GetLocalizedAsync(topic, x => x.MetaDescription),
            MetaTitle = await _localizationService.GetLocalizedAsync(topic, x => x.MetaTitle),
            SeName = await _urlRecordService.GetSeNameAsync(topic),
            TopicTemplateId = topic.TopicTemplateId
        };
    }

    /// <summary>
    /// Get the topic model by topic system name
    /// </summary>
    /// <param name="systemName">Topic system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic model
    /// </returns>
    public virtual async Task<TopicModel> PrepareTopicModelBySystemNameAsync(string systemName)
    {
		_logger.LogDebug(">>> Entering PrepareTopicModelBySystemNameAsync(systemName={SystemName})", systemName);
        //load by store
        var store = await _storeContext.GetCurrentStoreAsync();
        var topic = await _topicService.GetTopicBySystemNameAsync(systemName, store.Id);
		_logger.LogDebug("PrepareTopicModelBySystemNameAsync(systemName={SystemName}): store → {Store}", systemName, store);
        if (topic == null)
			_logger.LogDebug("PrepareTopicModelBySystemNameAsync(systemName={SystemName}): topic → {Topic}", systemName, topic);
            return null;

        return await PrepareTopicModelAsync(topic);
    }

    /// <summary>
    /// Get topic template view path
    /// </summary>
    /// <param name="topicTemplateId">Topic template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view path
    /// </returns>
    public virtual async Task<string> PrepareTemplateViewPathAsync(int topicTemplateId)
    {
		_logger.LogDebug(">>> Entering PrepareTemplateViewPathAsync(topicTemplateId={TopicTemplateId})", topicTemplateId);
        var template = (await _topicTemplateService.GetTopicTemplateByIdAsync(topicTemplateId) ??
                        (await _topicTemplateService.GetAllTopicTemplatesAsync()).FirstOrDefault()) ?? throw new Exception("No default template could be loaded");

		_logger.LogDebug("<<< Exiting PrepareTemplateViewPathAsync(topicTemplateId={TopicTemplateId})", topicTemplateId);
        return template.ViewPath;
    }

    #endregion
}