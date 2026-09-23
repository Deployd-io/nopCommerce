using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class NewsController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsModelFactory _newsModelFactory;
    protected readonly INewsService _newsService;
    protected readonly INotificationService _notificationService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public NewsController(ICustomerActivityService customerActivityService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INewsModelFactory newsModelFactory,
        INewsService newsService,
        INotificationService notificationService,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService)
    {
		_logger.LogError(">>> Entering NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService})", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService);
        _customerActivityService = customerActivityService;
        _eventPublisher = eventPublisher;
		_logger.LogDebug("NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService}): _eventPublisher → {_eventPublisher}", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService, _eventPublisher);
		_logger.LogDebug("NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService}): _customerActivityService → {_customerActivityService}", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService, _customerActivityService);
    _logger.LogWarning("<<< Exiting NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService})", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService);
        _localizationService = localizationService;
		_logger.LogDebug("NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService}): _localizationService → {_localizationService}", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService, _localizationService);
        _newsModelFactory = newsModelFactory;
		_logger.LogDebug("NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService}): _newsModelFactory → {_newsModelFactory}", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService, _newsModelFactory);
        _newsService = newsService;
		_logger.LogDebug("NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService}): _newsService → {_newsService}", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService, _newsService);
        _notificationService = notificationService;
		_logger.LogDebug("NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService}): _notificationService → {_notificationService}", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService, _notificationService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("NewsController(customerActivityService={CustomerActivityService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},notificationService={NotificationService},storeMappingService=<redacted>,urlRecordService={UrlRecordService}): _storeMappingService → {_storeMappingService}", customerActivityService, eventPublisher, localizationService, newsModelFactory, newsService, notificationService, urlRecordService, _storeMappingService);
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Methods        

    #region News items

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("NewsItems");
    }

    [CheckPermission(StandardPermission.ContentManagement.NEWS_VIEW)]
    public virtual async Task<IActionResult> NewsItems(int? filterByNewsItemId)
    {
		_logger.LogError(">>> Entering NewsItems(filterByNewsItemId={FilterByNewsItemId})", filterByNewsItemId);
        //prepare model
        var model = await _newsModelFactory.PrepareNewsContentModelAsync(new NewsContentModel(), filterByNewsItemId);
		_logger.LogError("NewsItems(filterByNewsItemId={FilterByNewsItemId}): model → {Model}", filterByNewsItemId, model);

		_logger.LogWarning("<<< Exiting NewsItems(filterByNewsItemId={FilterByNewsItemId})", filterByNewsItemId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_VIEW)]
    public virtual async Task<IActionResult> List(NewsItemSearchModel searchModel)
    {
		_logger.LogError(">>> Entering List(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemListModelAsync(searchModel);
		_logger.LogError("List(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> NewsItemCreate()
    {
		_logger.LogError(">>> Entering NewsItemCreate()");
        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemModelAsync(new NewsItemModel(), null);
		_logger.LogError("NewsItemCreate(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting NewsItemCreate()");
		_logger.LogWarning("<<< Exiting NewsItemCreate(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> NewsItemCreate(NewsItemModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var newsItem = model.ToEntity<NewsItem>();
            newsItem.CreatedOnUtc = DateTime.UtcNow;
            await _newsService.InsertNewsAsync(newsItem);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewNews",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewNews"), newsItem.Id), newsItem);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
            await _urlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(newsItem, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Added"));

            if (!continueEditing)
                return RedirectToAction("NewsItems");

            return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
        }

        //prepare model
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.NEWS_VIEW)]
    public virtual async Task<IActionResult> NewsItemEdit(int id)
    {
		_logger.LogError(">>> Entering NewsItemEdit(id={Id})", id);
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(id);
		_logger.LogError("NewsItemEdit(id={Id}): newsItem → {NewsItem}", id, newsItem);
        if (newsItem == null)
            return RedirectToAction("NewsItems");

        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemModelAsync(null, newsItem);
		_logger.LogError("NewsItemEdit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting NewsItemEdit(id={Id})", id);
		_logger.LogWarning("<<< Exiting NewsItemEdit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> NewsItemEdit(NewsItemModel model, bool continueEditing)
    {
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(model.Id);
        if (newsItem == null)
            return RedirectToAction("NewsItems");

        if (ModelState.IsValid)
        {
            newsItem = model.ToEntity(newsItem);
            await _newsService.UpdateNewsAsync(newsItem);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditNews",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNews"), newsItem.Id), newsItem);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
            await _urlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(newsItem, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Updated"));

            if (!continueEditing)
                return RedirectToAction("NewsItems");

            return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
        }

        //prepare model
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(id);
		_logger.LogError("Delete(id={Id}): newsItem → {NewsItem}", id, newsItem);
        if (newsItem == null)
            return RedirectToAction("NewsItems");

        await _newsService.DeleteNewsAsync(newsItem);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteNews",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNews"), newsItem.Id), newsItem);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Deleted"));

		_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("NewsItems");
    }

    #endregion

    #region Comments

    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_VIEW)]
    public virtual async Task<IActionResult> NewsComments(int? filterByNewsItemId)
    {
		_logger.LogError(">>> Entering NewsComments(filterByNewsItemId={FilterByNewsItemId})", filterByNewsItemId);
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
		_logger.LogError("NewsComments(filterByNewsItemId={FilterByNewsItemId}): newsItem → {NewsItem}", filterByNewsItemId, newsItem);
        if (newsItem == null && filterByNewsItemId.HasValue)
            return RedirectToAction("NewsComments");

        //prepare model
        var model = await _newsModelFactory.PrepareNewsCommentSearchModelAsync(new NewsCommentSearchModel(), newsItem);
		_logger.LogError("NewsComments(filterByNewsItemId={FilterByNewsItemId}): model → {Model}", filterByNewsItemId, model);

		_logger.LogError(">>> Entering Comments(searchModel={SearchModel})", searchModel);
		_logger.LogWarning("<<< Exiting Comments(searchModel={SearchModel})", searchModel);
		_logger.LogWarning("<<< Exiting NewsComments(filterByNewsItemId={FilterByNewsItemId})", filterByNewsItemId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_VIEW)]
    public virtual async Task<IActionResult> Comments(NewsCommentSearchModel searchModel)
    {
        //prepare model
        var model = await _newsModelFactory.PrepareNewsCommentListModelAsync(searchModel, searchModel.NewsItemId);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CommentUpdate(NewsCommentModel model)
    {
		_logger.LogError(">>> Entering CommentUpdate(model={Model})", model);
        //try to get a news comment with the specified id
        var comment = await _newsService.GetNewsCommentByIdAsync(model.Id)
            ?? throw new ArgumentException("No comment found with the specified id");

        var previousIsApproved = comment.IsApproved;

        //fill entity from model
        comment = model.ToEntity(comment);
		_logger.LogError("CommentUpdate(model={Model}): comment → {Comment}", model, comment);

        await _newsService.UpdateNewsCommentAsync(comment);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditNewsComment",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), comment.Id), comment);

        //raise event (only if it wasn't approved before and is approved now)
        if (!previousIsApproved && comment.IsApproved)
            await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

		_logger.LogWarning("<<< Exiting CommentUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CommentDelete(int id)
    {
		_logger.LogError(">>> Entering CommentDelete(id={Id})", id);
        //try to get a news comment with the specified id
        var comment = await _newsService.GetNewsCommentByIdAsync(id)
            ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

        await _newsService.DeleteNewsCommentAsync(comment);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteNewsComment",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), comment.Id), comment);

		_logger.LogWarning("<<< Exiting CommentDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DeleteSelectedComments(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var comments = await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray());
		_logger.LogError("DeleteSelectedComments(selectedIds={SelectedIds}): comments → {Comments}", selectedIds, comments);

        await _newsService.DeleteNewsCommentsAsync(comments);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment");
		_logger.LogError("DeleteSelectedComments(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("DeleteNewsComment", comments, newsComment => string.Format(activityLogFormat, newsComment.Id));

		_logger.LogWarning("<<< Exiting DeleteSelectedComments(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering ApproveSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter not approved comments
        var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved).ToList();
		_logger.LogError("ApproveSelected(selectedIds={SelectedIds}): newsComments → {NewsComments}", selectedIds, newsComments);

        foreach (var newsComment in newsComments)
        {
            newsComment.IsApproved = true;
			_logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): newsComment.IsApproved → {Arg1}", selectedIds, newsComment.IsApproved);

            await _newsService.UpdateNewsCommentAsync(newsComment);

            //raise event 
            await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(newsComment));
        }

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment");
		_logger.LogError("ApproveSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("EditNewsComment", newsComments, newsComment => string.Format(activityLogFormat, newsComment.Id));

		_logger.LogWarning("<<< Exiting ApproveSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DisapproveSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter approved comments
        var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved).ToList();
		_logger.LogError("DisapproveSelected(selectedIds={SelectedIds}): newsComments → {NewsComments}", selectedIds, newsComments);

        foreach (var newsComment in newsComments)
        {
            newsComment.IsApproved = false;
			_logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): newsComment.IsApproved → {Arg1}", selectedIds, newsComment.IsApproved);

            await _newsService.UpdateNewsCommentAsync(newsComment);
        }

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment");
		_logger.LogError("DisapproveSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("EditNewsComment", newsComments, newsComment => string.Format(activityLogFormat, newsComment.Id));

		_logger.LogWarning("<<< Exiting DisapproveSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #endregion
}