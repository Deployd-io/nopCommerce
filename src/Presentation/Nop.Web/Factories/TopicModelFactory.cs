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
		_logger.LogError(">>> Entering TopicModelFactory(localizationService={LocalizationService},storeContext={StoreContext},topicService={TopicService},topicTemplateService={TopicTemplateService},urlRecordService={UrlRecordService})", localizationService, storeContext, topicService, topicTemplateService, urlRecordService);
        _localizationService = localizationService;
        _storeContext = storeContext;
		_logger.LogDebug("TopicModelFactory(localizationService={LocalizationService},storeContext={StoreContext},topicService={TopicService},topicTemplateService={TopicTemplateService},urlRecordService={UrlRecordService}): _storeContext → {_storeContext}", localizationService, storeContext, topicService, topicTemplateService, urlRecordService, _storeContext);
		_logger.LogDebug("TopicModelFactory(localizationService={LocalizationService},storeContext={StoreContext},topicService={TopicService},topicTemplateService={TopicTemplateService},urlRecordService={UrlRecordService}): _localizationService → {_localizationService}", localizationService, storeContext, topicService, topicTemplateService, urlRecordService, _localizationService);
    _logger.LogWarning("<<< Exiting TopicModelFactory(localizationService={LocalizationService},storeContext={StoreContext},topicService={TopicService},topicTemplateService={TopicTemplateService},urlRecordService={UrlRecordService})", localizationService, storeContext, topicService, topicTemplateService, urlRecordService);
        _topicService = topicService;
		_logger.LogDebug("TopicModelFactory(localizationService={LocalizationService},storeContext={StoreContext},topicService={TopicService},topicTemplateService={TopicTemplateService},urlRecordService={UrlRecordService}): _topicService → {_topicService}", localizationService, storeContext, topicService, topicTemplateService, urlRecordService, _topicService);
        _topicTemplateService = topicTemplateService;
		_logger.LogDebug("TopicModelFactory(localizationService={LocalizationService},storeContext={StoreContext},topicService={TopicService},topicTemplateService={TopicTemplateService},urlRecordService={UrlRecordService}): _topicTemplateService → {_topicTemplateService}", localizationService, storeContext, topicService, topicTemplateService, urlRecordService, _topicTemplateService);
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
		_logger.LogError(">>> Entering PrepareTopicModelAsync(topic={Topic})", topic);
        ArgumentNullException.ThrowIfNull(topic);

		_logger.LogWarning("<<< Exiting PrepareTopicModelAsync(topic={Topic})", topic);
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
		_logger.LogError(">>> Entering PrepareTopicModelBySystemNameAsync(systemName={SystemName})", systemName);
        //load by store
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareTopicModelBySystemNameAsync(systemName={SystemName}): store → {Store}", systemName, store);
        var topic = await _topicService.GetTopicBySystemNameAsync(systemName, store.Id);
		_logger.LogError("PrepareTopicModelBySystemNameAsync(systemName={SystemName}): topic → {Topic}", systemName, topic);
		_logger.LogWarning("<<< Exiting PrepareTopicModelBySystemNameAsync(systemName={SystemName})", systemName);
        if (topic == null) {
            return null;
        }

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
		_logger.LogError(">>> Entering PrepareTemplateViewPathAsync(topicTemplateId={TopicTemplateId})", topicTemplateId);
        var template = (await _topicTemplateService.GetTopicTemplateByIdAsync(topicTemplateId) ??
                        (await _topicTemplateService.GetAllTopicTemplatesAsync()).FirstOrDefault()) ?? throw new Exception("No default template could be loaded");

		_logger.LogWarning("<<< Exiting PrepareTemplateViewPathAsync(topicTemplateId={TopicTemplateId})", topicTemplateId);
        return template.ViewPath;
    }

    #endregion
}