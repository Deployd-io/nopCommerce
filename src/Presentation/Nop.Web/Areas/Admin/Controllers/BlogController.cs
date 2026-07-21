using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Core.Events;
using Nop.Services.Blogs;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class BlogController : BaseAdminController
{
    #region Fields

    protected readonly IBlogModelFactory _blogModelFactory;
    protected readonly IBlogService _blogService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public BlogController(IBlogModelFactory blogModelFactory,
        IBlogService blogService,
        ICustomerActivityService customerActivityService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        IUrlRecordService urlRecordService)
    {
        _blogModelFactory = blogModelFactory;
        _blogService = blogService;
        _customerActivityService = customerActivityService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Methods        

    #region Blog posts

    public virtual IActionResult Index()
    {
		logger.LogDebug(">>> Entering Index()");
		logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("BlogPosts");
    }

    [CheckPermission(StandardPermission.ContentManagement.BLOG_VIEW)]
    public virtual async Task<IActionResult> BlogPosts(int? filterByBlogPostId)
    {
		logger.LogDebug(">>> Entering BlogPosts(filterByBlogPostId={FilterByBlogPostId})", filterByBlogPostId);
        //prepare model
        var model = await _blogModelFactory.PrepareBlogContentModelAsync(new BlogContentModel(), filterByBlogPostId);

		logger.LogDebug("BlogPosts(filterByBlogPostId={FilterByBlogPostId}): model → {Model}", filterByBlogPostId, model);
		logger.LogDebug("<<< Exiting BlogPosts(filterByBlogPostId={FilterByBlogPostId})", filterByBlogPostId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_VIEW)]
    public virtual async Task<IActionResult> List(BlogPostSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering List(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _blogModelFactory.PrepareBlogPostListModelAsync(searchModel);

		logger.LogDebug("List(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> BlogPostCreate()
    {
		logger.LogDebug(">>> Entering BlogPostCreate()");
        //prepare model
        var model = await _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null);

		logger.LogDebug("BlogPostCreate(): model → {Model}", model);
		logger.LogDebug("<<< Exiting BlogPostCreate()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> BlogPostCreate(BlogPostModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var blogPost = model.ToEntity<BlogPost>();
            blogPost.CreatedOnUtc = DateTime.UtcNow;
            await _blogService.InsertBlogPostAsync(blogPost);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewBlogPost",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewBlogPost"), blogPost.Id), blogPost);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
            await _urlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(blogPost, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Added"));

            if (!continueEditing)
                return RedirectToAction("BlogPosts");

            return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
        }

        //prepare model
        model = await _blogModelFactory.PrepareBlogPostModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting BlogPostCreate(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.ContentManagement.BLOG_VIEW)]
    public virtual async Task<IActionResult> BlogPostEdit(int id)
    {
		logger.LogDebug(">>> Entering BlogPostEdit(id={Id})", id);
        //try to get a blog post with the specified id
        var blogPost = await _blogService.GetBlogPostByIdAsync(id);
        if (blogPost == null)
			logger.LogDebug("BlogPostEdit(id={Id}): blogPost → {BlogPost}", id, blogPost);
            return RedirectToAction("BlogPosts");

        //prepare model
        var model = await _blogModelFactory.PrepareBlogPostModelAsync(null, blogPost);

		logger.LogDebug("BlogPostEdit(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting BlogPostEdit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> BlogPostEdit(BlogPostModel model, bool continueEditing)
    {
        //try to get a blog post with the specified id
        var blogPost = await _blogService.GetBlogPostByIdAsync(model.Id);
        if (blogPost == null)
            return RedirectToAction("BlogPosts");

        if (ModelState.IsValid)
        {
            blogPost = model.ToEntity(blogPost);
            await _blogService.UpdateBlogPostAsync(blogPost);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditBlogPost",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogPost"), blogPost.Id), blogPost);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
            await _urlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(blogPost, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Updated"));

            if (!continueEditing)
                return RedirectToAction("BlogPosts");

            return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
        }

        //prepare model
        model = await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting BlogPostEdit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a blog post with the specified id
        var blogPost = await _blogService.GetBlogPostByIdAsync(id);
        if (blogPost == null)
			logger.LogDebug("Delete(id={Id}): blogPost → {BlogPost}", id, blogPost);
            return RedirectToAction("BlogPosts");

        await _blogService.DeleteBlogPostAsync(blogPost);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteBlogPost",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPost"), blogPost.Id), blogPost);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Deleted"));

		logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("BlogPosts");
    }

    #endregion

    #region Comments

    [CheckPermission(StandardPermission.ContentManagement.BLOG_COMMENTS_VIEW)]
    public virtual async Task<IActionResult> BlogComments(int? filterByBlogPostId)
    {
		logger.LogDebug(">>> Entering BlogComments(filterByBlogPostId={FilterByBlogPostId})", filterByBlogPostId);
        //try to get a blog post with the specified id
        var blogPost = await _blogService.GetBlogPostByIdAsync(filterByBlogPostId ?? 0);
        if (blogPost == null && filterByBlogPostId.HasValue)
			logger.LogDebug("BlogComments(filterByBlogPostId={FilterByBlogPostId}): blogPost → {BlogPost}", filterByBlogPostId, blogPost);
            return RedirectToAction("BlogComments");

        //prepare model
        var model = await _blogModelFactory.PrepareBlogCommentSearchModelAsync(new BlogCommentSearchModel(), blogPost);

		logger.LogDebug("BlogComments(filterByBlogPostId={FilterByBlogPostId}): model → {Model}", filterByBlogPostId, model);
		logger.LogDebug(">>> Entering Comments(searchModel={SearchModel})", searchModel);
		logger.LogDebug("<<< Exiting Comments(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_COMMENTS_VIEW)]
    public virtual async Task<IActionResult> Comments(BlogCommentSearchModel searchModel)
    {
        //prepare model
        var model = await _blogModelFactory.PrepareBlogCommentListModelAsync(searchModel, searchModel.BlogPostId);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CommentUpdate(BlogCommentModel model)
    {
        //try to get a blog comment with the specified id
        var comment = await _blogService.GetBlogCommentByIdAsync(model.Id)
            ?? throw new ArgumentException("No comment found with the specified id");

        var previousIsApproved = comment.IsApproved;

        //fill entity from model
        comment = model.ToEntity(comment);

        await _blogService.UpdateBlogCommentAsync(comment);

        //raise event (only if it wasn't approved before and is approved now)
        if (!previousIsApproved && comment.IsApproved)
            await _eventPublisher.PublishAsync(new BlogCommentApprovedEvent(comment));

        //activity log
        await _customerActivityService.InsertActivityAsync("EditBlogComment",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment"), comment.Id), comment);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CommentDelete(int id)
    {
		logger.LogDebug(">>> Entering CommentDelete(id={Id})", id);
        //try to get a blog comment with the specified id
        var comment = await _blogService.GetBlogCommentByIdAsync(id)
            ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

        await _blogService.DeleteBlogCommentAsync(comment);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteBlogPostComment",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPostComment"), comment.Id), comment);

		logger.LogDebug("<<< Exiting CommentDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering DeleteSelectedComments(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var comments = await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray());

		logger.LogDebug("DeleteSelectedComments(selectedIds={SelectedIds}): comments → {Comments}", selectedIds, comments);
        await _blogService.DeleteBlogCommentsAsync(comments);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPostComment");
        await _customerActivityService.InsertActivitiesAsync("DeleteBlogPostComment", comments, blogComment => string.Format(activityLogFormat, blogComment.Id));
		logger.LogDebug("DeleteSelectedComments(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting DeleteSelectedComments(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering ApproveSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter not approved comments
        var blogComments = (await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved).ToList();

		logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): blogComments → {BlogComments}", selectedIds, blogComments);
        foreach (var blogComment in blogComments)
        {
            blogComment.IsApproved = true;
			logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): blogComment.IsApproved → {Arg1}", selectedIds, blogComment.IsApproved);

            await _blogService.UpdateBlogCommentAsync(blogComment);

            //raise event 
            await _eventPublisher.PublishAsync(new BlogCommentApprovedEvent(blogComment));
        }

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment");
        await _customerActivityService.InsertActivitiesAsync("EditBlogComment", blogComments, blogComment => string.Format(activityLogFormat, blogComment.Id));
		logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting ApproveSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering DisapproveSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter approved comments
        var blogComments = (await _blogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved).ToList();

		logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): blogComments → {BlogComments}", selectedIds, blogComments);
        foreach (var blogComment in blogComments)
        {
            blogComment.IsApproved = false;
			logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): blogComment.IsApproved → {Arg1}", selectedIds, blogComment.IsApproved);

            await _blogService.UpdateBlogCommentAsync(blogComment);
        }

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.EditBlogComment");
        await _customerActivityService.InsertActivitiesAsync("EditBlogComment", blogComments, blogComment => string.Format(activityLogFormat, blogComment.Id));
		logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);

		logger.LogDebug("<<< Exiting DisapproveSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #endregion
}
