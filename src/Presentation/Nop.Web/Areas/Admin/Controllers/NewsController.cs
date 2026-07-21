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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class NewsController : BaseAdminController
{
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
        _customerActivityService = customerActivityService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _newsModelFactory = newsModelFactory;
        _newsService = newsService;
        _notificationService = notificationService;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Methods        

    #region News items

    public virtual IActionResult Index()
    {
		logger.LogDebug(">>> Entering Index()");
		logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("NewsItems");
    }

    [CheckPermission(StandardPermission.ContentManagement.NEWS_VIEW)]
    public virtual async Task<IActionResult> NewsItems(int? filterByNewsItemId)
    {
		logger.LogDebug(">>> Entering NewsItems(filterByNewsItemId={FilterByNewsItemId})", filterByNewsItemId);
        //prepare model
        var model = await _newsModelFactory.PrepareNewsContentModelAsync(new NewsContentModel(), filterByNewsItemId);

		logger.LogDebug("NewsItems(filterByNewsItemId={FilterByNewsItemId}): model → {Model}", filterByNewsItemId, model);
		logger.LogDebug("<<< Exiting NewsItems(filterByNewsItemId={FilterByNewsItemId})", filterByNewsItemId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_VIEW)]
    public virtual async Task<IActionResult> List(NewsItemSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering List(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemListModelAsync(searchModel);

		logger.LogDebug("List(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> NewsItemCreate()
    {
		logger.LogDebug(">>> Entering NewsItemCreate()");
        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemModelAsync(new NewsItemModel(), null);

		logger.LogDebug("NewsItemCreate(): model → {Model}", model);
		logger.LogDebug("<<< Exiting NewsItemCreate()");
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
		logger.LogDebug("<<< Exiting NewsItemCreate(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.NEWS_VIEW)]
    public virtual async Task<IActionResult> NewsItemEdit(int id)
    {
		logger.LogDebug(">>> Entering NewsItemEdit(id={Id})", id);
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(id);
        if (newsItem == null)
			logger.LogDebug("NewsItemEdit(id={Id}): newsItem → {NewsItem}", id, newsItem);
            return RedirectToAction("NewsItems");

        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemModelAsync(null, newsItem);

		logger.LogDebug("NewsItemEdit(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting NewsItemEdit(id={Id})", id);
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
		logger.LogDebug("<<< Exiting NewsItemEdit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(id);
        if (newsItem == null)
			logger.LogDebug("Delete(id={Id}): newsItem → {NewsItem}", id, newsItem);
            return RedirectToAction("NewsItems");

        await _newsService.DeleteNewsAsync(newsItem);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteNews",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNews"), newsItem.Id), newsItem);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Deleted"));

		logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("NewsItems");
    }

    #endregion

    #region Comments

    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_VIEW)]
    public virtual async Task<IActionResult> NewsComments(int? filterByNewsItemId)
    {
		logger.LogDebug(">>> Entering NewsComments(filterByNewsItemId={FilterByNewsItemId})", filterByNewsItemId);
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
        if (newsItem == null && filterByNewsItemId.HasValue)
			logger.LogDebug("NewsComments(filterByNewsItemId={FilterByNewsItemId}): newsItem → {NewsItem}", filterByNewsItemId, newsItem);
            return RedirectToAction("NewsComments");

        //prepare model
        var model = await _newsModelFactory.PrepareNewsCommentSearchModelAsync(new NewsCommentSearchModel(), newsItem);

		logger.LogDebug("NewsComments(filterByNewsItemId={FilterByNewsItemId}): model → {Model}", filterByNewsItemId, model);
		logger.LogDebug(">>> Entering Comments(searchModel={SearchModel})", searchModel);
		logger.LogDebug("<<< Exiting Comments(searchModel={SearchModel})", searchModel);
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
        //try to get a news comment with the specified id
        var comment = await _newsService.GetNewsCommentByIdAsync(model.Id)
            ?? throw new ArgumentException("No comment found with the specified id");

        var previousIsApproved = comment.IsApproved;

        //fill entity from model
        comment = model.ToEntity(comment);

        await _newsService.UpdateNewsCommentAsync(comment);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditNewsComment",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), comment.Id), comment);

        //raise event (only if it wasn't approved before and is approved now)
        if (!previousIsApproved && comment.IsApproved)
            await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CommentDelete(int id)
    {
		logger.LogDebug(">>> Entering CommentDelete(id={Id})", id);
        //try to get a news comment with the specified id
        var comment = await _newsService.GetNewsCommentByIdAsync(id)
            ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

        await _newsService.DeleteNewsCommentAsync(comment);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteNewsComment",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), comment.Id), comment);

		logger.LogDebug("<<< Exiting CommentDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering DeleteSelectedComments(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var comments = await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray());

		logger.LogDebug("DeleteSelectedComments(selectedIds={SelectedIds}): comments → {Comments}", selectedIds, comments);
        await _newsService.DeleteNewsCommentsAsync(comments);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment");
        await _customerActivityService.InsertActivitiesAsync("DeleteNewsComment", comments, newsComment => string.Format(activityLogFormat, newsComment.Id));
		logger.LogDebug("DeleteSelectedComments(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting DeleteSelectedComments(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering ApproveSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter not approved comments
        var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved).ToList();

		logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): newsComments → {NewsComments}", selectedIds, newsComments);
        foreach (var newsComment in newsComments)
        {
            newsComment.IsApproved = true;
			logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): newsComment.IsApproved → {Arg1}", selectedIds, newsComment.IsApproved);

            await _newsService.UpdateNewsCommentAsync(newsComment);

            //raise event 
            await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(newsComment));
        }

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment");
        await _customerActivityService.InsertActivitiesAsync("EditNewsComment", newsComments, newsComment => string.Format(activityLogFormat, newsComment.Id));
		logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting ApproveSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.NEWS_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering DisapproveSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter approved comments
        var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved).ToList();

		logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): newsComments → {NewsComments}", selectedIds, newsComments);
        foreach (var newsComment in newsComments)
        {
            newsComment.IsApproved = false;
			logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): newsComment.IsApproved → {Arg1}", selectedIds, newsComment.IsApproved);

            await _newsService.UpdateNewsCommentAsync(newsComment);
        }

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment");
        await _customerActivityService.InsertActivitiesAsync("EditNewsComment", newsComments, newsComment => string.Format(activityLogFormat, newsComment.Id));
		logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting DisapproveSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #endregion
}
