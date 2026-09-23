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
		_logger.LogError(">>> Entering NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings})", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings);
        _captchaSettings = captchaSettings;
        _customerActivityService = customerActivityService;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _customerActivityService → {_customerActivityService}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _customerActivityService);
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _captchaSettings → {_captchaSettings}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _captchaSettings);
    _logger.LogWarning("<<< Exiting NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings})", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings);
        _customerService = customerService;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _customerService → {_customerService}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _customerService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _eventPublisher → {_eventPublisher}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _eventPublisher);
        _localizationService = localizationService;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _localizationService → {_localizationService}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _localizationService);
        _newsModelFactory = newsModelFactory;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _newsModelFactory → {_newsModelFactory}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _newsModelFactory);
        _newsService = newsService;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _newsService → {_newsService}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _newsService);
        _nopUrlHelper = nopUrlHelper;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _nopUrlHelper → {_nopUrlHelper}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _nopUrlHelper);
        _permissionService = permissionService;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _permissionService → {_permissionService}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _permissionService);
        _storeContext = storeContext;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _storeContext → {_storeContext}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _storeContext);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _storeMappingService → {_storeMappingService}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _storeMappingService);
        _webHelper = webHelper;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _webHelper → {_webHelper}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _webHelper);
        _workContext = workContext;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _workContext → {_workContext}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _workflowMessageService → {_workflowMessageService}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _workflowMessageService);
        _localizationSettings = localizationSettings;
		_logger.LogDebug("NewsController(captchaSettings={CaptchaSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},newsModelFactory={NewsModelFactory},newsService={NewsService},nopUrlHelper={NopUrlHelper},permissionService={PermissionService},storeContext={StoreContext},storeMappingService=<redacted>,webHelper={WebHelper},workContext={WorkContext},workflowMessageService={WorkflowMessageService},localizationSettings={LocalizationSettings},newsSettings={NewsSettings}): _localizationSettings → {_localizationSettings}", captchaSettings, customerActivityService, customerService, eventPublisher, localizationService, newsModelFactory, newsService, nopUrlHelper, permissionService, storeContext, webHelper, workContext, workflowMessageService, localizationSettings, newsSettings, _localizationSettings);
        _newsSettings = newsSettings;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> List(NewsPagingFilteringModel command)
    {
		_logger.LogError(">>> Entering List(command={Command})", command);
        if (!_newsSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _newsModelFactory.PrepareNewsItemListModelAsync(command);
		_logger.LogError("List(command={Command}): model → {Model}", command, model);
		_logger.LogWarning("<<< Exiting List(command={Command})", command);
        return View(model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> ListRss(int languageId)
    {
		_logger.LogError(">>> Entering ListRss(languageId={LanguageId})", languageId);
        var store = await _storeContext.GetCurrentStoreAsync();
        var feed = new RssFeed(
            $"{await _localizationService.GetLocalizedAsync(store, x => x.Name)}: News",
            "News",
            new Uri(_webHelper.GetStoreLocation()),
            DateTime.UtcNow);
			_logger.LogError("ListRss(languageId={LanguageId}): store → {Store}", languageId, store);

        if (!_newsSettings.Enabled)
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

        var items = new List<RssItem>();
		_logger.LogError("ListRss(languageId={LanguageId}): items → {Items}", languageId, items);
        var newsItems = await _newsService.GetAllNewsAsync(languageId, store.Id);
		_logger.LogError("ListRss(languageId={LanguageId}): newsItems → {NewsItems}", languageId, newsItems);
        foreach (var n in newsItems)
        {
            var newsUrl = await _nopUrlHelper.RouteGenericUrlAsync(n, _webHelper.GetCurrentRequestProtocol(), languageId: n.LanguageId, ensureTwoPublishedLanguages: false);
			_logger.LogError("ListRss(languageId={LanguageId}): newsUrl → {NewsUrl}", languageId, newsUrl);
            items.Add(new RssItem(n.Title, n.Short, new Uri(newsUrl), $"urn:store:{store.Id}:news:blog:{n.Id}", n.CreatedOnUtc));
        }
        feed.Items = items;
		_logger.LogDebug("ListRss(languageId={LanguageId}): feed.Items → {Arg1}", languageId, feed.Items);
		_logger.LogWarning("<<< Exiting ListRss(languageId={LanguageId})", languageId);
        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    public virtual async Task<IActionResult> NewsItem(int newsItemId)
    {
		_logger.LogError(">>> Entering NewsItem(newsItemId={NewsItemId})", newsItemId);
        if (!_newsSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
		_logger.LogError("NewsItem(newsItemId={NewsItemId}): newsItem → {NewsItem}", newsItemId, newsItem);
        if (newsItem == null)
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
		_logger.LogError("NewsItem(newsItemId={NewsItemId}): hasAdminAccess → {HasAdminAccess}", newsItemId, hasAdminAccess);
        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        var model = new NewsItemModel();
		_logger.LogError("NewsItem(newsItemId={NewsItemId}): model → {Model}", newsItemId, model);
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

        //display "edit" (manage) link
        if (hasAdminAccess)
            DisplayEditLink(Url.Action("NewsItemEdit", "News", new { id = newsItem.Id, area = AreaNames.ADMIN }));

		_logger.LogWarning("<<< Exiting NewsItem(newsItemId={NewsItemId})", newsItemId);
        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> NewsCommentAdd(int newsItemId, NewsItemModel model, bool captchaValid)
    {
		_logger.LogError(">>> Entering NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid})", newsItemId, model, captchaValid);
        if (!_newsSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
		_logger.LogError("NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid}): newsItem → {NewsItem}", newsItemId, model, captchaValid, newsItem);
        if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid}): customer → {Customer}", newsItemId, model, captchaValid, customer);
        if (await _customerService.IsGuestAsync(customer) && !_newsSettings.AllowNotRegisteredUsersToLeaveComments)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("News.Comments.OnlyRegisteredUsersLeaveComments"));
        }

        if (ModelState.IsValid)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid}): store → {Store}", newsItemId, model, captchaValid, store);

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
			_logger.LogError("NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid}): newsUrl → {NewsUrl}", newsItemId, model, captchaValid, newsUrl);
            return LocalRedirect(newsUrl);
        }

        //If we got this far, something failed, redisplay form
        RouteData.Values["action"] = "NewsItem";
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);
		_logger.LogError("NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid}): model → {Model1}", newsItemId, model, captchaValid, model);
		_logger.LogWarning("<<< Exiting NewsCommentAdd(newsItemId={NewsItemId},model={Model},captchaValid={CaptchaValid})", newsItemId, model, captchaValid);
        return View(model);
    }

    #endregion
}