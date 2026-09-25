using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Core.Rss;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Boards;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class BoardsController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IForumModelFactory _forumModelFactory;
    protected readonly IForumService _forumService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public BoardsController(CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        ForumSettings forumSettings,
        ICustomerService customerService,
        IForumModelFactory forumModelFactory,
        IForumService forumService,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext})", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext);
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _customerSettings → {_customerSettings}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _customerSettings);
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _captchaSettings → {_captchaSettings}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _captchaSettings);
    _logger.LogWarning("<<< Exiting BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext})", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext);
        _forumSettings = forumSettings;
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _forumSettings → {_forumSettings}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _forumSettings);
        _customerService = customerService;
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _customerService → {_customerService}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _customerService);
        _forumModelFactory = forumModelFactory;
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _forumModelFactory → {_forumModelFactory}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _forumModelFactory);
        _forumService = forumService;
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _forumService → {_forumService}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _forumService);
        _localizationService = localizationService;
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _localizationService → {_localizationService}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _localizationService);
        _storeContext = storeContext;
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _storeContext → {_storeContext}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _storeContext);
        _webHelper = webHelper;
		_logger.LogDebug("BoardsController(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},customerService={CustomerService},forumModelFactory={ForumModelFactory},forumService={ForumService},localizationService={LocalizationService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", captchaSettings, customerSettings, forumSettings, customerService, forumModelFactory, forumService, localizationService, storeContext, webHelper, workContext, _webHelper);
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> Index()
    {
		_logger.LogError(">>> Entering Index()");
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _forumModelFactory.PrepareBoardsIndexModelAsync();
		_logger.LogError("Index(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Index()");
        return View(model);
    }

    public virtual async Task<IActionResult> ActiveDiscussions(int forumId = 0, int pageNumber = 1)
    {
		_logger.LogError(">>> Entering ActiveDiscussions(forumId={ForumId},pageNumber={PageNumber})", forumId, pageNumber);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync(forumId, pageNumber);
		_logger.LogError("ActiveDiscussions(forumId={ForumId},pageNumber={PageNumber}): model → {Model}", forumId, pageNumber, model);

        return View(model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> ActiveDiscussionsRss(int forumId = 0)
    {
		_logger.LogError(">>> Entering ActiveDiscussionsRss(forumId={ForumId})", forumId);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!_forumSettings.ActiveDiscussionsFeedEnabled)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var topics = await _forumService.GetActiveTopicsAsync(forumId, 0, _forumSettings.ActiveDiscussionsFeedCount);
		_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): topics → {Topics}", forumId, topics);
        var url = Url.RouteUrl(NopRouteNames.Standard.ACTIVE_DISCUSSIONS_RSS, null, _webHelper.GetCurrentRequestProtocol());
		_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): url → {Url}", forumId, url);

        var feedTitle = await _localizationService.GetResourceAsync("Forum.ActiveDiscussionsFeedTitle");
		_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): feedTitle → {FeedTitle}", forumId, feedTitle);
        var feedDescription = await _localizationService.GetResourceAsync("Forum.ActiveDiscussionsFeedDescription");
		_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): feedDescription → {FeedDescription}", forumId, feedDescription);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): store → {Store}", forumId, store);
        var feed = new RssFeed(
            string.Format(feedTitle, await _localizationService.GetLocalizedAsync(store, x => x.Name)),
            feedDescription,
            new Uri(url),
            DateTime.UtcNow);

        var items = new List<RssItem>();
		_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): items → {Items}", forumId, items);

        var viewsText = await _localizationService.GetResourceAsync("Forum.Views");
		_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): viewsText → {ViewsText}", forumId, viewsText);
        var repliesText = await _localizationService.GetResourceAsync("Forum.Replies");
		_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): repliesText → {RepliesText}", forumId, repliesText);

        foreach (var topic in topics)
        {
            var topicUrl = Url.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG, new { id = topic.Id, slug = await _forumService.GetTopicSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());
			_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): topicUrl → {TopicUrl}", forumId, topicUrl);
            var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";
			_logger.LogError("ActiveDiscussionsRss(forumId={ForumId}): content → {Content}", forumId, content);

            items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl),
                $"urn:store:{store.Id}:activeDiscussions:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
        }
        feed.Items = items;
		_logger.LogDebug("ActiveDiscussionsRss(forumId={ForumId}): feed.Items → {Arg1}", forumId, feed.Items);

        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    public virtual async Task<IActionResult> ForumGroup(int id)
    {
		_logger.LogError(">>> Entering ForumGroup(id={Id})", id);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
		_logger.LogError("ForumGroup(id={Id}): forumGroup → {ForumGroup}", id, forumGroup);
        if (forumGroup == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var model = await _forumModelFactory.PrepareForumGroupModelAsync(forumGroup);
		_logger.LogError("ForumGroup(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting ForumGroup(id={Id})", id);
        return View(model);
    }

    public virtual async Task<IActionResult> Forum(int id, int pageNumber = 1)
    {
		_logger.LogError(">>> Entering Forum(id={Id},pageNumber={PageNumber})", id, pageNumber);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forum = await _forumService.GetForumByIdAsync(id);
		_logger.LogError("Forum(id={Id},pageNumber={PageNumber}): forum → {Forum}", id, pageNumber, forum);
        if (forum == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var model = await _forumModelFactory.PrepareForumPageModelAsync(forum, pageNumber);
		_logger.LogError("Forum(id={Id},pageNumber={PageNumber}): model → {Model}", id, pageNumber, model);

        return View(model);
    }

    public virtual async Task<IActionResult> ForumRss(int id)
    {
		_logger.LogError(">>> Entering ForumRss(id={Id})", id);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!_forumSettings.ForumFeedsEnabled)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var topicLimit = _forumSettings.ForumFeedCount;
        var forum = await _forumService.GetForumByIdAsync(id);
		_logger.LogError("ForumRss(id={Id}): forum → {Forum}", id, forum);

        if (forum != null)
        {
            //Order by newest topic posts & limit the number of topics to return
            var topics = await _forumService.GetAllTopicsAsync(forum.Id, 0, string.Empty,
                ForumSearchType.All, 0, 0, topicLimit);

            var url = Url.RouteUrl(NopRouteNames.Standard.FORUM_RSS, new { id = forum.Id }, _webHelper.GetCurrentRequestProtocol());
			_logger.LogError("ForumRss(id={Id}): url → {Url}", id, url);

            var feedTitle = await _localizationService.GetResourceAsync("Forum.ForumFeedTitle");
			_logger.LogError("ForumRss(id={Id}): feedTitle → {FeedTitle}", id, feedTitle);
            var feedDescription = await _localizationService.GetResourceAsync("Forum.ForumFeedDescription");
			_logger.LogError("ForumRss(id={Id}): feedDescription → {FeedDescription}", id, feedDescription);

            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("ForumRss(id={Id}): store → {Store}", id, store);
            var feed = new RssFeed(
                string.Format(feedTitle, await _localizationService.GetLocalizedAsync(store, x => x.Name), forum.Name),
                feedDescription,
                new Uri(url),
                DateTime.UtcNow);

            var items = new List<RssItem>();
			_logger.LogError("ForumRss(id={Id}): items → {Items}", id, items);

            var viewsText = await _localizationService.GetResourceAsync("Forum.Views");
			_logger.LogError("ForumRss(id={Id}): viewsText → {ViewsText}", id, viewsText);
            var repliesText = await _localizationService.GetResourceAsync("Forum.Replies");
			_logger.LogError("ForumRss(id={Id}): repliesText → {RepliesText}", id, repliesText);

            foreach (var topic in topics)
            {
                var topicUrl = Url.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG, new { id = topic.Id, slug = await _forumService.GetTopicSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());
				_logger.LogError("ForumRss(id={Id}): topicUrl → {TopicUrl}", id, topicUrl);
                var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";
				_logger.LogError("ForumRss(id={Id}): content → {Content}", id, content);

                items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl), $"urn:store:{store.Id}:forum:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
            }

            feed.Items = items;
			_logger.LogDebug("ForumRss(id={Id}): feed.Items → {Arg1}", id, feed.Items);

            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

		_logger.LogWarning("<<< Exiting ForumRss(id={Id})", id);
        return new RssActionResult(new RssFeed(new Uri(_webHelper.GetStoreLocation())), _webHelper.GetThisPageUrl(false));
    }

    [HttpPost]
    public virtual async Task<IActionResult> ForumWatch(int id)
    {
		_logger.LogError(">>> Entering ForumWatch(id={Id})", id);
        var watchTopic = await _localizationService.GetResourceAsync("Forum.WatchForum");
        var unwatchTopic = await _localizationService.GetResourceAsync("Forum.UnwatchForum");
		_logger.LogError("ForumWatch(id={Id}): unwatchTopic → {UnwatchTopic}", id, unwatchTopic);
        var returnText = watchTopic;
		_logger.LogError("ForumWatch(id={Id}): returnText → {ReturnText}", id, returnText);
		_logger.LogError("ForumWatch(id={Id}): watchTopic → {WatchTopic}", id, watchTopic);

        var forum = await _forumService.GetForumByIdAsync(id);
		_logger.LogError("ForumWatch(id={Id}): forum → {Forum}", id, forum);
        if (forum == null)
            return Json(new { Subscribed = false, Text = returnText, Error = true });

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("ForumWatch(id={Id}): customer → {Customer}", id, customer);
        if (!await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
            return Json(new { Subscribed = false, Text = returnText, Error = true });

        var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id,
            forum.Id, 0, 0, 1)).FirstOrDefault();

        bool subscribed;
        if (forumSubscription == null)
        {
            forumSubscription = new ForumSubscription
            {
                SubscriptionGuid = Guid.NewGuid(),
                CustomerId = customer.Id,
                ForumId = forum.Id,
                CreatedOnUtc = DateTime.UtcNow
            };
            await _forumService.InsertSubscriptionAsync(forumSubscription);
            subscribed = true;
			_logger.LogError("ForumWatch(id={Id}): subscribed → {Subscribed}", id, subscribed);
            returnText = unwatchTopic;
        }
        else
        {
			_logger.LogDebug("ForumWatch(id={Id}): subscribed → {Subscribed}", id, subscribed);
            await _forumService.DeleteSubscriptionAsync(forumSubscription);
            subscribed = false;
        }

		_logger.LogWarning("<<< Exiting ForumWatch(id={Id})", id);
        return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
    }

    public virtual async Task<IActionResult> Topic(int id, int pageNumber = 1)
    {
		_logger.LogError(">>> Entering Topic(id={Id},pageNumber={PageNumber})", id, pageNumber);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
		_logger.LogError("Topic(id={Id},pageNumber={PageNumber}): forumTopic → {ForumTopic}", id, pageNumber, forumTopic);
        if (forumTopic == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var model = await _forumModelFactory.PrepareForumTopicPageModelAsync(forumTopic, pageNumber);
		_logger.LogError("Topic(id={Id},pageNumber={PageNumber}): model → {Model}", id, pageNumber, model);
        //if no posts loaded, redirect to the first page
        if (!model.ForumPostModels.Any() && pageNumber > 1)
            return RedirectToRoute(NopRouteNames.Standard.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });

        //update view count
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("Topic(id={Id},pageNumber={PageNumber}): customer → {Customer}", id, pageNumber, customer);
        if (!customer.IsSearchEngineAccount())
        {
            forumTopic.Views += 1;
            await _forumService.UpdateTopicAsync(forumTopic);
        }

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> TopicWatch(int id)
    {
		_logger.LogError(">>> Entering TopicWatch(id={Id})", id);
        var watchTopic = await _localizationService.GetResourceAsync("Forum.WatchTopic");
        var unwatchTopic = await _localizationService.GetResourceAsync("Forum.UnwatchTopic");
		_logger.LogError("TopicWatch(id={Id}): unwatchTopic → {UnwatchTopic}", id, unwatchTopic);
        var returnText = watchTopic;
		_logger.LogError("TopicWatch(id={Id}): returnText → {ReturnText}", id, returnText);
		_logger.LogError("TopicWatch(id={Id}): watchTopic → {WatchTopic}", id, watchTopic);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
		_logger.LogError("TopicWatch(id={Id}): forumTopic → {ForumTopic}", id, forumTopic);
        if (forumTopic == null)
            return Json(new { Subscribed = false, Text = returnText, Error = true });

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("TopicWatch(id={Id}): customer → {Customer}", id, customer);
        if (!await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
            return Json(new { Subscribed = false, Text = returnText, Error = true });

        var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id,
            0, forumTopic.Id, 0, 1)).FirstOrDefault();

        bool subscribed;
        if (forumSubscription == null)
        {
            forumSubscription = new ForumSubscription
            {
                SubscriptionGuid = Guid.NewGuid(),
                CustomerId = customer.Id,
                TopicId = forumTopic.Id,
                CreatedOnUtc = DateTime.UtcNow
            };
            await _forumService.InsertSubscriptionAsync(forumSubscription);
            subscribed = true;
			_logger.LogError("TopicWatch(id={Id}): subscribed → {Subscribed}", id, subscribed);
            returnText = unwatchTopic;
        }
        else
        {
			_logger.LogDebug("TopicWatch(id={Id}): subscribed → {Subscribed}", id, subscribed);
            await _forumService.DeleteSubscriptionAsync(forumSubscription);
            subscribed = false;
        }

		_logger.LogWarning("<<< Exiting TopicWatch(id={Id})", id);
        return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
    }

    public virtual async Task<IActionResult> TopicMove(int id)
    {
		_logger.LogError(">>> Entering TopicMove(id={Id})", id);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
		_logger.LogError("TopicMove(id={Id}): forumTopic → {ForumTopic}", id, forumTopic);
        if (forumTopic == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        if (!await _forumService.IsCustomerAllowedToMoveTopicAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
            return Challenge();

        var model = await _forumModelFactory.PrepareTopicMoveAsync(forumTopic);
		_logger.LogError("TopicMove(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting TopicMove(id={Id})", id);
		_logger.LogWarning("<<< Exiting TopicMove(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> TopicMove(TopicMoveModel model)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(model.Id);

        if (forumTopic == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var newForumId = model.ForumSelected;
        var forum = await _forumService.GetForumByIdAsync(newForumId);

        if (forum != null && forumTopic.ForumId != newForumId)
            await _forumService.MoveTopicAsync(forumTopic.Id, newForumId);

        return RedirectToRoute(NopRouteNames.Standard.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
    }

    [HttpPost]
    public virtual async Task<IActionResult> TopicDelete(int id)
    {
		_logger.LogError(">>> Entering TopicDelete(id={Id})", id);
        if (!_forumSettings.ForumsEnabled)
            return Json(new
            {
                redirect = Url.RouteUrl(NopRouteNames.General.HOMEPAGE),
            });

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
		_logger.LogError("TopicDelete(id={Id}): forumTopic → {ForumTopic}", id, forumTopic);
        if (forumTopic != null)
        {
            if (!await _forumService.IsCustomerAllowedToDeleteTopicAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
                return Challenge();

            var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
			_logger.LogError("TopicDelete(id={Id}): forum → {Forum}", id, forum);

            await _forumService.DeleteTopicAsync(forumTopic);

            if (forum != null)
                return Json(new
                {
                    redirect = Url.RouteUrl(NopRouteNames.Standard.FORUM_SLUG, new { id = forum.Id, slug = await _forumService.GetForumSeNameAsync(forum) }),
                });
        }

		_logger.LogWarning("<<< Exiting TopicDelete(id={Id})", id);
        return Json(new
        {
            redirect = Url.RouteUrl(NopRouteNames.General.BOARDS),
        });
    }

    public virtual async Task<IActionResult> TopicCreate(int id)
    {
		_logger.LogError(">>> Entering TopicCreate(id={Id})", id);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forum = await _forumService.GetForumByIdAsync(id);
		_logger.LogError("TopicCreate(id={Id}): forum → {Forum}", id, forum);
        if (forum == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        if (await _forumService.IsCustomerAllowedToCreateTopicAsync(await _workContext.GetCurrentCustomerAsync(), forum) == false)
            return Challenge();

        var model = new EditForumTopicModel();
		_logger.LogError("TopicCreate(id={Id}): model → {Model}", id, model);
        await _forumModelFactory.PrepareTopicCreateModelAsync(forum, model);
		_logger.LogWarning("<<< Exiting TopicCreate(id={Id})", id);
		_logger.LogWarning("<<< Exiting TopicCreate(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> TopicCreate(EditForumTopicModel model, bool captchaValid)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forum = await _forumService.GetForumByIdAsync(model.ForumId);
        if (forum == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        if (ModelState.IsValid)
        {
            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (!await _forumService.IsCustomerAllowedToCreateTopicAsync(customer, forum))
                {
                    return Challenge();
                }

                var subject = model.Subject;
                var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                {
                    subject = subject[0..maxSubjectLength];
                }

                var text = model.Text;
                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                    text = text[0..maxPostLength];

                var topicType = ForumTopicType.Normal;
                var nowUtc = DateTime.UtcNow;

                if (await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer))
                    topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                //forum topic
                var forumTopic = new ForumTopic
                {
                    ForumId = forum.Id,
                    CustomerId = customer.Id,
                    TopicTypeId = (int)topicType,
                    Subject = subject,
                    CreatedOnUtc = nowUtc,
                    UpdatedOnUtc = nowUtc
                };
                await _forumService.InsertTopicAsync(forumTopic, true);

                //forum post
                var forumPost = new ForumPost
                {
                    TopicId = forumTopic.Id,
                    CustomerId = customer.Id,
                    Text = text,
                    IPAddress = _customerSettings.StoreIpAddresses ? _webHelper.GetCurrentIpAddress() : string.Empty,
                    CreatedOnUtc = nowUtc,
                    UpdatedOnUtc = nowUtc
                };
                await _forumService.InsertPostAsync(forumPost, false);

                //update forum topic
                forumTopic.NumPosts = 1;
                forumTopic.LastPostId = forumPost.Id;
                forumTopic.LastPostCustomerId = forumPost.CustomerId;
                forumTopic.LastPostTime = forumPost.CreatedOnUtc;
                forumTopic.UpdatedOnUtc = nowUtc;
                await _forumService.UpdateTopicAsync(forumTopic);

                //subscription                
                if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
                {
                    if (model.Subscribed)
                    {
                        var forumSubscription = new ForumSubscription
                        {
                            SubscriptionGuid = Guid.NewGuid(),
                            CustomerId = customer.Id,
                            TopicId = forumTopic.Id,
                            CreatedOnUtc = nowUtc
                        };

                        await _forumService.InsertSubscriptionAsync(forumSubscription);
                    }
                }

                return RedirectToRoute(NopRouteNames.Standard.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        //redisplay form
        await _forumModelFactory.PrepareTopicCreateModelAsync(forum, model);

        return View(model);
    }

    public virtual async Task<IActionResult> TopicEdit(int id)
    {
		_logger.LogError(">>> Entering TopicEdit(id={Id})", id);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
		_logger.LogError("TopicEdit(id={Id}): forumTopic → {ForumTopic}", id, forumTopic);
        if (forumTopic == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        if (!await _forumService.IsCustomerAllowedToEditTopicAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
            return Challenge();

        var model = new EditForumTopicModel();
		_logger.LogError("TopicEdit(id={Id}): model → {Model}", id, model);
        await _forumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, false);

		_logger.LogWarning("<<< Exiting TopicEdit(id={Id})", id);
		_logger.LogWarning("<<< Exiting TopicEdit(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> TopicEdit(EditForumTopicModel model, bool captchaValid)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(model.Id);

        if (forumTopic == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
        if (forum == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        if (ModelState.IsValid)
        {
            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (!await _forumService.IsCustomerAllowedToEditTopicAsync(customer, forumTopic))
                    return Challenge();

                var subject = model.Subject;
                var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                {
                    subject = subject[0..maxSubjectLength];
                }

                var text = model.Text;
                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                    text = text[0..maxPostLength];

                var topicType = ForumTopicType.Normal;
                var nowUtc = DateTime.UtcNow;

                if (await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer))
                    topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                //forum topic
                forumTopic.TopicTypeId = (int)topicType;
                forumTopic.Subject = subject;
                forumTopic.UpdatedOnUtc = nowUtc;
                await _forumService.UpdateTopicAsync(forumTopic);

                //forum post                
                var firstPost = await _forumService.GetFirstPostAsync(forumTopic);
                if (firstPost != null)
                {
                    firstPost.Text = text;
                    firstPost.UpdatedOnUtc = nowUtc;
                    await _forumService.UpdatePostAsync(firstPost);
                }
                else
                {
                    //error (not possible)
                    firstPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = forumTopic.CustomerId,
                        Text = text,
                        IPAddress = _customerSettings.StoreIpAddresses ? _webHelper.GetCurrentIpAddress() : string.Empty,
                        UpdatedOnUtc = nowUtc
                    };

                    await _forumService.InsertPostAsync(firstPost, false);
                }

                //subscription
                if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id,
                        0, forumTopic.Id, 0, 1)).FirstOrDefault();
                    if (model.Subscribed)
                    {
                        if (forumSubscription == null)
                        {
                            forumSubscription = new ForumSubscription
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                CustomerId = customer.Id,
                                TopicId = forumTopic.Id,
                                CreatedOnUtc = nowUtc
                            };

                            await _forumService.InsertSubscriptionAsync(forumSubscription);
                        }
                    }
                    else
                    {
                        if (forumSubscription != null)
                        {
                            await _forumService.DeleteSubscriptionAsync(forumSubscription);
                        }
                    }
                }

                // redirect to the topic page with the topic slug
                return RedirectToRoute(NopRouteNames.Standard.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        //redisplay form
        await _forumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, true);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> PostDelete(int id)
    {
		_logger.LogError(">>> Entering PostDelete(id={Id})", id);
        if (!_forumSettings.ForumsEnabled)
            return Json(new
            {
                redirect = Url.RouteUrl(NopRouteNames.General.HOMEPAGE),
            });

        var forumPost = await _forumService.GetPostByIdAsync(id);
		_logger.LogError("PostDelete(id={Id}): forumPost → {ForumPost}", id, forumPost);

        if (forumPost == null)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.BOARDS) });

        if (!await _forumService.IsCustomerAllowedToDeletePostAsync(await _workContext.GetCurrentCustomerAsync(), forumPost))
            return Challenge();

        var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
		_logger.LogError("PostDelete(id={Id}): forumTopic → {ForumTopic}", id, forumTopic);
        var forumId = forumTopic.ForumId;
        var forum = await _forumService.GetForumByIdAsync(forumId);
		_logger.LogError("PostDelete(id={Id}): forum → {Forum}", id, forum);
        var forumSlug = await _forumService.GetForumSeNameAsync(forum);
		_logger.LogError("PostDelete(id={Id}): forumSlug → {ForumSlug}", id, forumSlug);

        await _forumService.DeletePostAsync(forumPost);

        //get topic one more time because it can be deleted (first or only post deleted)
        forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
        if (forumTopic == null)
            return Json(new
            {
                redirect = Url.RouteUrl(NopRouteNames.Standard.FORUM_SLUG, new { id = forumId, slug = forumSlug }),
            });

		_logger.LogWarning("<<< Exiting PostDelete(id={Id})", id);
        return Json(new
        {
            redirect = Url.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) }),
        });

    }

    public virtual async Task<IActionResult> PostCreate(int id, int? quote)
    {
		_logger.LogError(">>> Entering PostCreate(id={Id},quote={Quote})", id, quote);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
		_logger.LogError("PostCreate(id={Id},quote={Quote}): forumTopic → {ForumTopic}", id, quote, forumTopic);
        if (forumTopic == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        if (!await _forumService.IsCustomerAllowedToCreatePostAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
            return Challenge();

        var model = await _forumModelFactory.PreparePostCreateModelAsync(forumTopic, quote, false);
		_logger.LogError("PostCreate(id={Id},quote={Quote}): model → {Model}", id, quote, model);

		_logger.LogWarning("<<< Exiting PostCreate(id={Id},quote={Quote})", id, quote);
		_logger.LogWarning("<<< Exiting PostCreate(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> PostCreate(EditForumPostModel model, bool captchaValid)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(model.ForumTopicId);
        if (forumTopic == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        if (ModelState.IsValid)
        {
            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (!await _forumService.IsCustomerAllowedToCreatePostAsync(customer, forumTopic))
                    return Challenge();

                var text = model.Text;
                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                    text = text[0..maxPostLength];

                var nowUtc = DateTime.UtcNow;

                var forumPost = new ForumPost
                {
                    TopicId = forumTopic.Id,
                    CustomerId = customer.Id,
                    Text = text,
                    IPAddress = _customerSettings.StoreIpAddresses ? _webHelper.GetCurrentIpAddress() : string.Empty,
                    CreatedOnUtc = nowUtc,
                    UpdatedOnUtc = nowUtc
                };
                await _forumService.InsertPostAsync(forumPost, true);

                //subscription
                if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id,
                        0, forumPost.TopicId, 0, 1)).FirstOrDefault();
                    if (model.Subscribed)
                    {
                        if (forumSubscription == null)
                        {
                            forumSubscription = new ForumSubscription
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                CustomerId = customer.Id,
                                TopicId = forumPost.TopicId,
                                CreatedOnUtc = nowUtc
                            };

                            await _forumService.InsertSubscriptionAsync(forumSubscription);
                        }
                    }
                    else
                    {
                        if (forumSubscription != null)
                        {
                            await _forumService.DeleteSubscriptionAsync(forumSubscription);
                        }
                    }
                }

                var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;

                var pageIndex = await _forumService.CalculateTopicPageIndexAsync(forumPost.TopicId, pageSize, forumPost.Id) + 1;
                string url;
                if (pageIndex > 1)
                    url = Url.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG_PAGED, new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex });
                else
                    url = Url.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG, new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
                return LocalRedirect($"{url}#{forumPost.Id}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        //redisplay form
        model = await _forumModelFactory.PreparePostCreateModelAsync(forumTopic, 0, true);

        return View(model);
    }

    public virtual async Task<IActionResult> PostEdit(int id)
    {
		_logger.LogError(">>> Entering PostEdit(id={Id})", id);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumPost = await _forumService.GetPostByIdAsync(id);
		_logger.LogError("PostEdit(id={Id}): forumPost → {ForumPost}", id, forumPost);
        if (forumPost == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        if (!await _forumService.IsCustomerAllowedToEditPostAsync(await _workContext.GetCurrentCustomerAsync(), forumPost))
            return Challenge();

        var model = await _forumModelFactory.PreparePostEditModelAsync(forumPost, false);
		_logger.LogError("PostEdit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting PostEdit(id={Id})", id);
		_logger.LogWarning("<<< Exiting PostEdit(model={Model},captchaValid={CaptchaValid})", model, captchaValid);
        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> PostEdit(EditForumPostModel model, bool captchaValid)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumPost = await _forumService.GetPostByIdAsync(model.Id);
        if (forumPost == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _forumService.IsCustomerAllowedToEditPostAsync(customer, forumPost))
            return Challenge();

        var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
        if (forumTopic == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
        if (forum == null)
            return RedirectToRoute(NopRouteNames.General.BOARDS);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        if (ModelState.IsValid)
        {
            try
            {
                var nowUtc = DateTime.UtcNow;

                var text = model.Text;
                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                {
                    text = text[0..maxPostLength];
                }

                forumPost.UpdatedOnUtc = nowUtc;
                forumPost.Text = text;
                await _forumService.UpdatePostAsync(forumPost);

                //subscription
                if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id,
                        0, forumPost.TopicId, 0, 1)).FirstOrDefault();
                    if (model.Subscribed)
                    {
                        if (forumSubscription == null)
                        {
                            forumSubscription = new ForumSubscription
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                CustomerId = customer.Id,
                                TopicId = forumPost.TopicId,
                                CreatedOnUtc = nowUtc
                            };
                            await _forumService.InsertSubscriptionAsync(forumSubscription);
                        }
                    }
                    else
                    {
                        if (forumSubscription != null)
                        {
                            await _forumService.DeleteSubscriptionAsync(forumSubscription);
                        }
                    }
                }

                var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;
                var pageIndex = (await _forumService.CalculateTopicPageIndexAsync(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                string url;
                if (pageIndex > 1)
                {
                    url = Url.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG_PAGED, new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex });
                }
                else
                {
                    url = Url.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG, new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
                }
                return LocalRedirect($"{url}#{forumPost.Id}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        //redisplay form
        model = await _forumModelFactory.PreparePostEditModelAsync(forumPost, true);

        return View(model);
    }

    public virtual async Task<IActionResult> Search(string searchterms, bool? advs, string forumId,
        string within, string limitDays, int pageNumber = 1)
    {
		_logger.LogError(">>> Entering Search(searchterms={Searchterms},advs={Advs},forumId={ForumId},within={Within},limitDays={LimitDays},pageNumber={PageNumber})", searchterms, advs, forumId, within, limitDays, pageNumber);
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _forumModelFactory.PrepareSearchModelAsync(searchterms, advs, forumId, within, limitDays, pageNumber);
		_logger.LogError("Search(searchterms={Searchterms},advs={Advs},forumId={ForumId},within={Within},limitDays={LimitDays},pageNumber={PageNumber}): model → {Model}", searchterms, advs, forumId, within, limitDays, pageNumber, model);

        return View(model);
    }

    public virtual async Task<IActionResult> CustomerForumSubscriptions(int? pageNumber)
    {
		_logger.LogError(">>> Entering CustomerForumSubscriptions(pageNumber={PageNumber})", pageNumber);
        if (!_forumSettings.AllowCustomersToManageSubscriptions)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = await _forumModelFactory.PrepareCustomerForumSubscriptionsModelAsync(pageNumber);
		_logger.LogError("CustomerForumSubscriptions(pageNumber={PageNumber}): model → {Model}", pageNumber, model);

		_logger.LogWarning("<<< Exiting CustomerForumSubscriptions(pageNumber={PageNumber})", pageNumber);
        return View(model);
    }

    [HttpPost, ActionName("CustomerForumSubscriptions")]
    public virtual async Task<IActionResult> CustomerForumSubscriptionsPOST(IFormCollection formCollection)
    {
		_logger.LogError(">>> Entering CustomerForumSubscriptionsPOST(formCollection={FormCollection})", formCollection);
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("fs", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("fs", "").Trim();
				_logger.LogError("CustomerForumSubscriptionsPOST(formCollection={FormCollection}): id → {Id}", formCollection, id);
                if (int.TryParse(id, out var forumSubscriptionId))
                {
                    var forumSubscription = await _forumService.GetSubscriptionByIdAsync(forumSubscriptionId);
					_logger.LogError("CustomerForumSubscriptionsPOST(formCollection={FormCollection}): forumSubscription → {ForumSubscription}", formCollection, forumSubscription);
                    var customer = await _workContext.GetCurrentCustomerAsync();
					_logger.LogError("CustomerForumSubscriptionsPOST(formCollection={FormCollection}): customer → {Customer}", formCollection, customer);

                    if (forumSubscription != null && forumSubscription.CustomerId == customer.Id)
                    {
                        await _forumService.DeleteSubscriptionAsync(forumSubscription);
                    }
                }
            }
        }

		_logger.LogWarning("<<< Exiting CustomerForumSubscriptionsPOST(formCollection={FormCollection})", formCollection);
        return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_FORUM_SUBSCRIPTIONS);
    }

    [HttpPost]
    public virtual async Task<IActionResult> PostVote(int postId, bool isUp)
    {
		_logger.LogError(">>> Entering PostVote(postId={PostId},isUp={IsUp})", postId, isUp);
        if (!_forumSettings.AllowPostVoting)
            return new NullJsonResult();

        var forumPost = await _forumService.GetPostByIdAsync(postId);
		_logger.LogError("PostVote(postId={PostId},isUp={IsUp}): forumPost → {ForumPost}", postId, isUp, forumPost);
        if (forumPost == null)
            return new NullJsonResult();

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PostVote(postId={PostId},isUp={IsUp}): customer → {Customer}", postId, isUp, customer);
        if (!await _customerService.IsRegisteredAsync(customer))
            return Json(new
            {
                Error = await _localizationService.GetResourceAsync("Forum.Votes.Login"),
                VoteCount = forumPost.VoteCount
            });

        if (customer.Id == forumPost.CustomerId)
            return Json(new
            {
                Error = await _localizationService.GetResourceAsync("Forum.Votes.OwnPost"),
                VoteCount = forumPost.VoteCount
            });

        var forumPostVote = await _forumService.GetPostVoteAsync(postId, customer);
		_logger.LogError("PostVote(postId={PostId},isUp={IsUp}): forumPostVote → {ForumPostVote}", postId, isUp, forumPostVote);
        if (forumPostVote != null)
        {
            if ((forumPostVote.IsUp && isUp) || (!forumPostVote.IsUp && !isUp))
                return Json(new
                {
                    Error = await _localizationService.GetResourceAsync("Forum.Votes.AlreadyVoted"),
                    VoteCount = forumPost.VoteCount
                });

            await _forumService.DeletePostVoteAsync(forumPostVote);
			_logger.LogDebug("PostVote(postId={PostId},isUp={IsUp}): VoteCount → {VoteCount}", postId, isUp, VoteCount);
            return Json(new { VoteCount = forumPost.VoteCount });
        }

        if (await _forumService.GetNumberOfPostVotesAsync(customer, DateTime.UtcNow.AddDays(-1)) >= _forumSettings.MaxVotesPerDay)
            return Json(new
            {
                Error = string.Format(await _localizationService.GetResourceAsync("Forum.Votes.MaxVotesReached"), _forumSettings.MaxVotesPerDay),
                VoteCount = forumPost.VoteCount
            });

        await _forumService.InsertPostVoteAsync(new ForumPostVote
        {
            CustomerId = customer.Id,
            ForumPostId = postId,
            IsUp = isUp,
            CreatedOnUtc = DateTime.UtcNow
        });
		_logger.LogDebug("PostVote(postId={PostId},isUp={IsUp}): CreatedOnUtc → {CreatedOnUtc}", postId, isUp, CreatedOnUtc);

		_logger.LogWarning("<<< Exiting PostVote(postId={PostId},isUp={IsUp})", postId, isUp);
        return Json(new { VoteCount = forumPost.VoteCount, IsUp = isUp });
    }

    #endregion
}