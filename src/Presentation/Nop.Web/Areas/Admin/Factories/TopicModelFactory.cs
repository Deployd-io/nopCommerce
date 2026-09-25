using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the topic model factory implementation
/// </summary>
public partial class TopicModelFactory : ITopicModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly ITopicService _topicService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public TopicModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        INopUrlHelper nopUrlHelper,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        ITopicService topicService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper})", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper);
        _catalogSettings = catalogSettings;
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper}): _baseAdminModelFactory → {_baseAdminModelFactory}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper, _baseAdminModelFactory);
        _localizationService = localizationService;
		_logger.LogDebug("TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper}): _localizationService → {_localizationService}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper}): _localizedModelFactory → {_localizedModelFactory}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper, _localizedModelFactory);
        _nopUrlHelper = nopUrlHelper;
		_logger.LogDebug("TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper}): _nopUrlHelper → {_nopUrlHelper}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper, _nopUrlHelper);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
		_logger.LogDebug("TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper}): _storeMappingSupportedModelFactory → {_storeMappingSupportedModelFactory}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper, _storeMappingSupportedModelFactory);
        _topicService = topicService;
		_logger.LogDebug("TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper}): _topicService → {_topicService}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper, _topicService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper}): _urlRecordService → {_urlRecordService}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper, _urlRecordService);
        _webHelper = webHelper;
		_logger.LogDebug("TopicModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},nopUrlHelper={NopUrlHelper},storeMappingSupportedModelFactory=<redacted>,topicService={TopicService},urlRecordService={UrlRecordService},webHelper={WebHelper}): _webHelper → {_webHelper}", catalogSettings, baseAdminModelFactory, localizationService, localizedModelFactory, nopUrlHelper, topicService, urlRecordService, webHelper, _webHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare topic search model
    /// </summary>
    /// <param name="searchModel">Topic search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic search model
    /// </returns>
    public virtual async Task<TopicSearchModel> PrepareTopicSearchModelAsync(TopicSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareTopicSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareTopicSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged topic list model
    /// </summary>
    /// <param name="searchModel">Topic search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic list model
    /// </returns>
    public virtual async Task<TopicListModel> PrepareTopicListModelAsync(TopicSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareTopicListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get topics
        var topics = await _topicService.GetAllTopicsAsync(showHidden: true,
            keywords: searchModel.SearchKeywords,
            storeId: searchModel.SearchStoreId,
            ignoreAcl: true);

        var pagedTopics = topics.ToPagedList(searchModel);
		_logger.LogError("PrepareTopicListModelAsync(searchModel={SearchModel}): pagedTopics → {PagedTopics}", searchModel, pagedTopics);

        //prepare grid model
        var model = await new TopicListModel().PrepareToGridAsync(searchModel, pagedTopics, () =>
        {
            return pagedTopics.SelectAwait(async topic =>
            {
                //fill in model values from the entity
                var topicModel = topic.ToModel<TopicModel>();
				_logger.LogError("PrepareTopicListModelAsync(searchModel={SearchModel}): topicModel → {TopicModel}", searchModel, topicModel);

                //little performance optimization: ensure that "Body" is not returned
                topicModel.Body = string.Empty;
				_logger.LogDebug("PrepareTopicListModelAsync(searchModel={SearchModel}): topicModel.Body → {Arg1}", searchModel, topicModel.Body);

                topicModel.SeName = await _urlRecordService.GetSeNameAsync(topic, 0, true, false);

                if (!string.IsNullOrEmpty(topicModel.SystemName))
                    topicModel.TopicName = topicModel.SystemName;
                else
                    topicModel.TopicName = topicModel.Title;
					_logger.LogDebug("PrepareTopicListModelAsync(searchModel={SearchModel}): topicModel.TopicName → {Arg1}", searchModel, topicModel.TopicName);

                return topicModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareTopicListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare topic model
    /// </summary>
    /// <param name="model">Topic model</param>
    /// <param name="topic">Topic</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic model
    /// </returns>
    public virtual async Task<TopicModel> PrepareTopicModelAsync(TopicModel model, Topic topic, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareTopicModelAsync(model={Model},topic={Topic},excludeProperties={ExcludeProperties})", model, topic, excludeProperties);
        Func<TopicLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (topic != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = topic.ToModel<TopicModel>();
				_logger.LogError("PrepareTopicModelAsync(model={Model},topic={Topic},excludeProperties={ExcludeProperties}): model → {Model1}", model, topic, excludeProperties, model);
                model.SeName = await _urlRecordService.GetSeNameAsync(topic, 0, true, false);
            }

            model.Url = await _nopUrlHelper.RouteGenericUrlAsync(topic, _webHelper.GetCurrentRequestProtocol());

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Title = await _localizationService.GetLocalizedAsync(topic, entity => entity.Title, languageId, false, false);
                locale.Body = await _localizationService.GetLocalizedAsync(topic, entity => entity.Body, languageId, false, false);
                locale.MetaKeywords = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaKeywords, languageId, false, false);
                locale.MetaDescription = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaDescription, languageId, false, false);
                locale.MetaTitle = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaTitle, languageId, false, false);
                locale.SeName = await _urlRecordService.GetSeNameAsync(topic, languageId, false, false);
            };
        }

        //set default values for the new model
        if (topic == null)
        {
            model.DisplayOrder = 1;
			_logger.LogDebug("PrepareTopicModelAsync(model={Model},topic={Topic},excludeProperties={ExcludeProperties}): model.DisplayOrder → {Arg3}", model, topic, excludeProperties, model.DisplayOrder);
            model.Published = true;
			_logger.LogDebug("PrepareTopicModelAsync(model={Model},topic={Topic},excludeProperties={ExcludeProperties}): model.Published → {Arg3}", model, topic, excludeProperties, model.Published);
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available topic templates
        await _baseAdminModelFactory.PrepareTopicTemplatesAsync(model.AvailableTopicTemplates, false);

        //prepare model stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, topic, excludeProperties);

		_logger.LogWarning("<<< Exiting PrepareTopicModelAsync(model={Model},topic={Topic},excludeProperties={ExcludeProperties})", model, topic, excludeProperties);
        return model;
    }

    #endregion
}