using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Rss;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.News;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class NewsController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsModelFactory _newsModelFactory;
    protected readonly INewsService _newsService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public NewsController(CaptchaSettings captchaSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INewsModelFactory newsModelFactory,
        INewsService newsService,
        INopUrlHelper nopUrlHelper,
        IPermissionService permissionService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IWebHelper webHelper,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        NewsSettings newsSettings)
    {
        _captchaSettings = captchaSettings;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _newsModelFactory = newsModelFactory;
        _newsService = newsService;
        _nopUrlHelper = nopUrlHelper;
        _permissionService = permissionService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _webHelper = webHelper;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
        _newsSettings = newsSettings;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> List(NewsPagingFilteringModel command)
    {
		_logger.LogDebug(">>> Entering List(command={Command})", command);
        if (!_newsSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _newsModelFactory.PrepareNewsItemListModelAsync(command);
		_logger.LogDebug("List(command={Command}): model → {Model}", command, model);
		_logger.LogDebug("<<< Exiting List(command={Command})", command);
        return View(model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> ListRss(int languageId)
    {
		_logger.LogDebug(">>> Entering ListRss(languageId={LanguageId})", languageId);
        var store = await _storeContext.GetCurrentStoreAsync();
        var feed = new RssFeed(
            $"{await _localizationService.GetLocalizedAsync(store, x => x.Name)}: News",
            "News",
            new Uri(_webHelper.GetStoreLocation()),
            DateTime.UtcNow);
			_logger.LogDebug("ListRss(languageId={LanguageId}): store → {Store}", languageId, store);

        if (!_newsSettings.Enabled)
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

        var items = new List<RssItem>();
        var newsItems = await _newsService.GetAllNewsAsync(languageId, store.Id);
		_logger.LogDebug("ListRss(languageId={LanguageId}): items → {Items}", languageId, items);
		_logger.LogDebug("ListRss(languageId={LanguageId}): newsItems → {NewsItems}", languageId, newsItems);
        foreach (var n in newsItems)
        {
            var newsUrl = await _nopUrlHelper.RouteGenericUrlAsync(n, _webHelper.GetCurrentRequestProtocol(), languageId: n.LanguageId, ensureTwoPublishedLanguages: false);
            items.Add(new RssItem(n.Title, n.Short, new Uri(newsUrl), $"urn:store:{store.Id}:news:blog:{n.Id}", n.CreatedOnUtc));
			_logger.LogDebug("ListRss(languageId={LanguageId}): newsUrl → {NewsUrl}", languageId, newsUrl);
        }
        feed.Items = items;
		_logger.LogDebug("ListRss(languageId={LanguageId}): feed.Items → {Arg1}", languageId, feed.Items);
		_logger.LogDebug("<<< Exiting ListRss(languageId={LanguageId})", languageId);
        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    public virtual async Task<IActionResult> NewsItem(int newsItemId)
    {
		_logger.LogDebug(">>> Entering NewsItem(newsItemId={NewsItemId})", newsItemId);
        if (!_newsSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
        if (newsItem == null)
			_logger.LogDebug("NewsItem(newsItemId={NewsItemId}): newsItem → {NewsItem}", newsItemId, newsItem);
            return InvokeHttp404();

        var notAvailable =
            //published?
            !newsItem.Published ||
            //availability dates
            !_newsService.IsNewsAvailable(newsItem) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(newsItem);
        //Check whether the current user has a "Manage news" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.ContentManagement.NEWS_VIEW);
        if (notAvailable && !hasAdminAccess)
			_logger.LogDebug("NewsItem(newsItemId={NewsItemId}): hasAdminAccess → {HasAdminAccess}", newsItemId, hasAdminAccess);
            return InvokeHttp404();

        var model = new NewsItemModel();
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);
		_logger.LogDebug("NewsItem(newsItemId={NewsItemId}): model → {Model}", newsItemId, model);

        //display "edit" (manage) link
        if (hasAdminAccess)
            DisplayEditLink(Url.Action("NewsItemEdit", "News", new { id = newsItem.Id, area = AreaNames.ADMIN }));

		_logger.LogDebug("<<< Exiting NewsItem(newsItemId={NewsItemId})", newsItemId);
        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> NewsCommentAdd(int newsItemId, NewsItemModel model, bool captchaValid)
    {
        if (!_newsSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
        if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && !_newsSettings.AllowNotRegisteredUsersToLeaveComments)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("News.Comments.OnlyRegisteredUsersLeaveComments"));
        }

        if (ModelState.IsValid)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            var comment = new NewsComment
            {
                NewsItemId = newsItem.Id,
                CustomerId = customer.Id,
                CommentTitle = model.AddNewComment.CommentTitle,
                CommentText = model.AddNewComment.CommentText,
                IsApproved = !_newsSettings.NewsCommentsMustBeApproved,
                StoreId = store.Id,
                CreatedOnUtc = DateTime.UtcNow,
            };

            await _newsService.InsertNewsCommentAsync(comment);

            //notify a store owner;
            if (_newsSettings.NotifyAboutNewNewsComments)
                await _workflowMessageService.SendNewsCommentStoreOwnerNotificationMessageAsync(comment, _localizationSettings.DefaultAdminLanguageId);

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.AddNewsComment",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddNewsComment"), comment);

            //raise event
            if (comment.IsApproved)
                await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

            //The text boxes should be cleared after a comment has been posted
            //That' why we reload the page
            TempData["nop.news.addcomment.result"] = comment.IsApproved
                ? await _localizationService.GetResourceAsync("News.Comments.SuccessfullyAdded")
                : await _localizationService.GetResourceAsync("News.Comments.SeeAfterApproving");

            var newsUrl = await _nopUrlHelper.RouteGenericUrlAsync(newsItem, languageId: newsItem.LanguageId, ensureTwoPublishedLanguages: false);
            return LocalRedirect(newsUrl);
        }

        //If we got this far, something failed, redisplay form
        RouteData.Values["action"] = "NewsItem";
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);
		_logger.LogDebug("NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid}): model → {Model1}", newsItemId, model, captchaValid, model);
		_logger.LogDebug("<<< Exiting NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid})", newsItemId, model, captchaValid);
        return View(model);
    }

    #endregion
}