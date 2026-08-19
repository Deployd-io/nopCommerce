using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Rss;
using Nop.Services.Blogs;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Blogs;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class BlogController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly BlogSettings _blogSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly IBlogModelFactory _blogModelFactory;
    protected readonly IBlogService _blogService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public BlogController(BlogSettings blogSettings,
        CaptchaSettings captchaSettings,
        IBlogModelFactory blogModelFactory,
        IBlogService blogService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IPermissionService permissionService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IWebHelper webHelper,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings)
    {
		_logger.LogDebug(">>> Entering BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings})", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings);
        _blogSettings = blogSettings;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _blogSettings → {_blogSettings}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _blogSettings);
        _captchaSettings = captchaSettings;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _captchaSettings → {_captchaSettings}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _captchaSettings);
        _blogModelFactory = blogModelFactory;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _blogModelFactory → {_blogModelFactory}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _blogModelFactory);
        _blogService = blogService;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _blogService → {_blogService}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _blogService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _customerActivityService → {_customerActivityService}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _customerActivityService);
        _customerService = customerService;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _customerService → {_customerService}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _customerService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _eventPublisher → {_eventPublisher}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _eventPublisher);
        _localizationService = localizationService;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _localizationService → {_localizationService}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _localizationService);
        _nopUrlHelper = nopUrlHelper;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _nopUrlHelper → {_nopUrlHelper}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _nopUrlHelper);
        _permissionService = permissionService;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _permissionService → {_permissionService}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _permissionService);
        _storeContext = storeContext;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _storeContext → {_storeContext}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _storeContext);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _storeMappingService → {_storeMappingService}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _storeMappingService);
        _webHelper = webHelper;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _webHelper → {_webHelper}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _webHelper);
        _workContext = workContext;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _workContext → {_workContext}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _workflowMessageService → {_workflowMessageService}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _workflowMessageService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("<<< Exiting BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings})", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings);
		_logger.LogDebug("BlogController(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},blogModelFactory={BlogModelFactory},blogService={BlogService},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings}): _localizationSettings → {_localizationSettings}", blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, _localizationSettings);
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> List(BlogPagingFilteringModel command)
    {
		_logger.LogDebug(">>> Entering List(command={Command})", command);
        if (!_blogSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _blogModelFactory.PrepareBlogPostListModelAsync(command);
		_logger.LogDebug("List(command={Command}): model → {Model}", command, model);
		_logger.LogDebug("<<< Exiting List(command={Command})", command);
        return View("List", model);
    }

    public virtual async Task<IActionResult> BlogByTag(BlogPagingFilteringModel command)
    {
		_logger.LogDebug(">>> Entering BlogByTag(command={Command})", command);
        if (!_blogSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _blogModelFactory.PrepareBlogPostListModelAsync(command);
		_logger.LogDebug("BlogByTag(command={Command}): model → {Model}", command, model);
		_logger.LogDebug("<<< Exiting BlogByTag(command={Command})", command);
        return View("List", model);
    }

    public virtual async Task<IActionResult> BlogByMonth(BlogPagingFilteringModel command)
    {
		_logger.LogDebug(">>> Entering BlogByMonth(command={Command})", command);
        if (!_blogSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _blogModelFactory.PrepareBlogPostListModelAsync(command);
		_logger.LogDebug("BlogByMonth(command={Command}): model → {Model}", command, model);
		_logger.LogDebug("<<< Exiting BlogByMonth(command={Command})", command);
        return View("List", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> ListRss(int languageId)
    {
		_logger.LogDebug(">>> Entering ListRss(languageId={LanguageId})", languageId);
        var store = await _storeContext.GetCurrentStoreAsync();
        var feed = new RssFeed(
            $"{await _localizationService.GetLocalizedAsync(store, x => x.Name)}: Blog",
            "Blog",
            new Uri(_webHelper.GetStoreLocation()),
            DateTime.UtcNow);
			_logger.LogDebug("ListRss(languageId={LanguageId}): store → {Store}", languageId, store);

        if (!_blogSettings.Enabled)
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

        var items = new List<RssItem>();
		_logger.LogDebug("ListRss(languageId={LanguageId}): items → {Items}", languageId, items);
        var blogPosts = await _blogService.GetAllBlogPostsAsync(store.Id, languageId);
		_logger.LogDebug("ListRss(languageId={LanguageId}): blogPosts → {BlogPosts}", languageId, blogPosts);
        foreach (var blogPost in blogPosts)
        {
            var blogPostUrl = await _nopUrlHelper.RouteGenericUrlAsync(blogPost, _webHelper.GetCurrentRequestProtocol(), languageId: blogPost.LanguageId, ensureTwoPublishedLanguages: false);
			_logger.LogDebug("ListRss(languageId={LanguageId}): blogPostUrl → {BlogPostUrl}", languageId, blogPostUrl);
            items.Add(new RssItem(blogPost.Title, blogPost.Body, new Uri(blogPostUrl), $"urn:store:{store.Id}:blog:post:{blogPost.Id}", blogPost.CreatedOnUtc));
        }
        feed.Items = items;
		_logger.LogDebug("ListRss(languageId={LanguageId}): feed.Items → {Arg1}", languageId, feed.Items);
		_logger.LogDebug("<<< Exiting ListRss(languageId={LanguageId})", languageId);
        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    public virtual async Task<IActionResult> BlogPost(int blogPostId)
    {
		_logger.LogDebug(">>> Entering BlogPost(blogPostId={BlogPostId})", blogPostId);
        if (!_blogSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var blogPost = await _blogService.GetBlogPostByIdAsync(blogPostId);
		_logger.LogDebug("BlogPost(blogPostId={BlogPostId}): blogPost → {BlogPost}", blogPostId, blogPost);
        if (blogPost == null)
            return InvokeHttp404();

        var notAvailable =
            //availability dates
            !_blogService.BlogPostIsAvailable(blogPost) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(blogPost);
        //Check whether the current user has a "Manage blog" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.ContentManagement.BLOG_VIEW);
		_logger.LogDebug("BlogPost(blogPostId={BlogPostId}): hasAdminAccess → {HasAdminAccess}", blogPostId, hasAdminAccess);
        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        //display "edit" (manage) link
        if (hasAdminAccess)
            DisplayEditLink(Url.Action("BlogPostEdit", "Blog", new { id = blogPost.Id, area = AreaNames.ADMIN }));

        var model = new BlogPostModel();
		_logger.LogDebug("BlogPost(blogPostId={BlogPostId}): model → {Model}", blogPostId, model);
        await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

		_logger.LogDebug("<<< Exiting BlogPost(blogPostId={BlogPostId})", blogPostId);
        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> BlogCommentAdd(int blogPostId, BlogPostModel model, bool captchaValid)
    {
		_logger.LogDebug(">>> Entering BlogCommentAdd(blogPostId={BlogPostId},model={Model},captchaValid={CaptchaValid})", blogPostId, model, captchaValid);
        if (!_blogSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var blogPost = await _blogService.GetBlogPostByIdAsync(blogPostId);
		_logger.LogDebug("BlogCommentAdd(blogPostId={BlogPostId},model={Model},captchaValid={CaptchaValid}): blogPost → {BlogPost}", blogPostId, model, captchaValid, blogPost);
        if (blogPost == null || !blogPost.AllowComments)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("BlogCommentAdd(blogPostId={BlogPostId},model={Model},captchaValid={CaptchaValid}): customer → {Customer}", blogPostId, model, captchaValid, customer);
        if (await _customerService.IsGuestAsync(customer) && !_blogSettings.AllowNotRegisteredUsersToLeaveComments)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Blog.Comments.OnlyRegisteredUsersLeaveComments"));
        }

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        if (ModelState.IsValid)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("BlogCommentAdd(blogPostId={BlogPostId},model={Model},captchaValid={CaptchaValid}): store → {Store}", blogPostId, model, captchaValid, store);
            var comment = new BlogComment
            {
                BlogPostId = blogPost.Id,
                CustomerId = customer.Id,
                CommentText = model.AddNewComment.CommentText,
                IsApproved = !_blogSettings.BlogCommentsMustBeApproved,
                StoreId = store.Id,
                CreatedOnUtc = DateTime.UtcNow,
            };

            await _blogService.InsertBlogCommentAsync(comment);

            //notify a store owner
            if (_blogSettings.NotifyAboutNewBlogComments)
                await _workflowMessageService.SendBlogCommentStoreOwnerNotificationMessageAsync(comment, _localizationSettings.DefaultAdminLanguageId);

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.AddBlogComment",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddBlogComment"), comment);

            //raise event
            if (comment.IsApproved)
                await _eventPublisher.PublishAsync(new BlogCommentApprovedEvent(comment));

            //The text boxes should be cleared after a comment has been posted
            //That' why we reload the page
            TempData["nop.blog.addcomment.result"] = comment.IsApproved
                ? await _localizationService.GetResourceAsync("Blog.Comments.SuccessfullyAdded")
                : await _localizationService.GetResourceAsync("Blog.Comments.SeeAfterApproving");

            var blogPostUrl = await _nopUrlHelper.RouteGenericUrlAsync(blogPost, languageId: blogPost.LanguageId, ensureTwoPublishedLanguages: false);
			_logger.LogDebug("BlogCommentAdd(blogPostId={BlogPostId},model={Model},captchaValid={CaptchaValid}): blogPostUrl → {BlogPostUrl}", blogPostId, model, captchaValid, blogPostUrl);
            return LocalRedirect(blogPostUrl);
        }

        //If we got this far, something failed, redisplay form
        RouteData.Values["action"] = "BlogPost";
        await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);
		_logger.LogDebug("<<< Exiting BlogCommentAdd(blogPostId={BlogPostId},model={Model},captchaValid={CaptchaValid})", blogPostId, model, captchaValid);
        return View(model);
    }

    #endregion
}