using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class TopicController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITopicModelFactory _topicModelFactory;
    protected readonly ITopicService _topicService;
    protected readonly IUrlRecordService _urlRecordService;


    #endregion Fields

    #region Ctor

    public TopicController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IStoreMappingService storeMappingService,
        ITopicModelFactory topicModelFactory,
        ITopicService topicService,
        IUrlRecordService urlRecordService)
    {
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _storeMappingService = storeMappingService;
        _topicModelFactory = topicModelFactory;
        _topicService = topicService;
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Topic topic, TopicModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(topic,
                x => x.Title,
                localized.Title,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(topic,
                x => x.Body,
                localized.Body,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(topic,
                x => x.MetaKeywords,
                localized.MetaKeywords,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(topic,
                x => x.MetaDescription,
                localized.MetaDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(topic,
                x => x.MetaTitle,
                localized.MetaTitle,
                localized.LanguageId);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(topic, localized.SeName, localized.Title, false);
            await _urlRecordService.SaveSlugAsync(topic, seName, localized.LanguageId);
        }
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.ContentManagement.TOPICS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _topicModelFactory.PrepareTopicSearchModelAsync(new TopicSearchModel());

		_logger.LogDebug("List(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.TOPICS_VIEW)]
    public virtual async Task<IActionResult> List(TopicSearchModel searchModel)
    {
        //prepare model
        var model = await _topicModelFactory.PrepareTopicListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.ContentManagement.TOPICS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _topicModelFactory.PrepareTopicModelAsync(new TopicModel(), null);

		_logger.LogDebug("Create(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.TOPICS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(TopicModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            if (!model.IsPasswordProtected)
                model.Password = null;

            var topic = model.ToEntity<Topic>();
            await _topicService.InsertTopicAsync(topic);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(topic, model.SeName, topic.Title ?? topic.SystemName, true);
            await _urlRecordService.SaveSlugAsync(topic, model.SeName, 0);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(topic, model.SelectedStoreIds);

            //locales
            await UpdateLocalesAsync(topic, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Topics.Added"));

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewTopic",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTopic"), topic.Title ?? topic.SystemName), topic);

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = topic.Id });
        }

        //prepare model
        model = await _topicModelFactory.PrepareTopicModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.TOPICS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a topic with the specified id
        var topic = await _topicService.GetTopicByIdAsync(id);
        if (topic == null)
			_logger.LogDebug("Edit(id={Id}): topic → {Topic}", id, topic);
            return RedirectToAction("List");

        //prepare model
        var model = await _topicModelFactory.PrepareTopicModelAsync(null, topic);

		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);
		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.TOPICS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(TopicModel model, bool continueEditing)
    {
        //try to get a topic with the specified id
        var topic = await _topicService.GetTopicByIdAsync(model.Id);
        if (topic == null)
            return RedirectToAction("List");

        if (!model.IsPasswordProtected)
            model.Password = null;

        if (ModelState.IsValid)
        {
            topic = model.ToEntity(topic);
            await _topicService.UpdateTopicAsync(topic);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(topic, model.SeName, topic.Title ?? topic.SystemName, true);
            await _urlRecordService.SaveSlugAsync(topic, model.SeName, 0);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(topic, model.SelectedStoreIds);

            //locales
            await UpdateLocalesAsync(topic, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Topics.Updated"));

            //activity log
            await _customerActivityService.InsertActivityAsync("EditTopic",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTopic"), topic.Title ?? topic.SystemName), topic);

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = topic.Id });
        }

        //prepare model
        model = await _topicModelFactory.PrepareTopicModelAsync(model, topic, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.TOPICS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a topic with the specified id
        var topic = await _topicService.GetTopicByIdAsync(id);
        if (topic == null)
			_logger.LogDebug("Delete(id={Id}): topic → {Topic}", id, topic);
            return RedirectToAction("List");

        await _topicService.DeleteTopicAsync(topic);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Topics.Deleted"));

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteTopic",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTopic"), topic.Title ?? topic.SystemName), topic);

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    #endregion
}