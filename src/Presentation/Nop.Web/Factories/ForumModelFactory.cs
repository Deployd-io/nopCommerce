using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Extensions;
using Nop.Web.Infrastructure;
using Nop.Web.Models.Boards;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the forum model factory
/// </summary>
public partial class ForumModelFactory : IForumModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly IBBCodeHelper _bbCodeHelper;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IForumService _forumService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IJsonLdModelFactory _jsonLdModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly SeoSettings _seoSettings;

    #endregion

    #region Ctor

    public ForumModelFactory(CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        ForumSettings forumSettings,
        IBBCodeHelper bbCodeHelper,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IForumService forumService,
        IGenericAttributeService genericAttributeService,
        IJsonLdModelFactory jsonLdModelFactory,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        SeoSettings seoSettings)
    {
		_logger.LogDebug(">>> Entering ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings})", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings);
        _captchaSettings = captchaSettings;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _captchaSettings → {_captchaSettings}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _captchaSettings);
        _customerSettings = customerSettings;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _customerSettings → {_customerSettings}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _customerSettings);
        _forumSettings = forumSettings;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _forumSettings → {_forumSettings}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _forumSettings);
        _bbCodeHelper = bbCodeHelper;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _bbCodeHelper → {_bbCodeHelper}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _bbCodeHelper);
        _countryService = countryService;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _countryService → {_countryService}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _countryService);
        _customerService = customerService;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _customerService → {_customerService}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _dateTimeHelper → {_dateTimeHelper}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _dateTimeHelper);
        _forumService = forumService;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _forumService → {_forumService}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _forumService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _genericAttributeService → {_genericAttributeService}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _genericAttributeService);
        _jsonLdModelFactory = jsonLdModelFactory;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _jsonLdModelFactory → {_jsonLdModelFactory}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _jsonLdModelFactory);
        _localizationService = localizationService;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _localizationService → {_localizationService}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _localizationService);
        _pictureService = pictureService;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _pictureService → {_pictureService}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _pictureService);
        _workContext = workContext;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _workContext → {_workContext}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _workContext);
        _mediaSettings = mediaSettings;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _mediaSettings → {_mediaSettings}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _mediaSettings);
        _seoSettings = seoSettings;
		_logger.LogDebug("ForumModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},forumSettings={ForumSettings},bbCodeHelper={BbCodeHelper},countryService={CountryService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},forumService={ForumService},genericAttributeService={GenericAttributeService},jsonLdModelFactory={JsonLdModelFactory},localizationService={LocalizationService},pictureService={PictureService},workContext={WorkContext},mediaSettings={MediaSettings},seoSettings={SeoSettings}): _seoSettings → {_seoSettings}", captchaSettings, customerSettings, forumSettings, bbCodeHelper, countryService, customerService, dateTimeHelper, forumService, genericAttributeService, jsonLdModelFactory, localizationService, pictureService, workContext, mediaSettings, seoSettings, _seoSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get the list of forum topic types
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of the select list item
    /// </returns>
    protected virtual async Task<IEnumerable<SelectListItem>> ForumTopicTypesListAsync()
    {
		_logger.LogDebug(">>> Entering ForumTopicTypesListAsync()");
        var list = new List<SelectListItem>
        {
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Normal"),
                Value = ((int)ForumTopicType.Normal).ToString()
            },

            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Sticky"),
                Value = ((int)ForumTopicType.Sticky).ToString()
            },

            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Announcement"),
                Value = ((int)ForumTopicType.Announcement).ToString()
            }
        };

		_logger.LogDebug("<<< Exiting ForumTopicTypesListAsync()");
        return list;
    }

    /// <summary>
    /// Get the list of forum groups
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of the select list item
    /// </returns>
    protected virtual async Task<IEnumerable<SelectListItem>> ForumGroupsForumsListAsync()
    {
		_logger.LogDebug(">>> Entering ForumGroupsForumsListAsync()");
        var forumsList = new List<SelectListItem>();
        var separator = "--";
		_logger.LogDebug("ForumGroupsForumsListAsync(): separator → {Separator}", separator);
        var forumGroups = await _forumService.GetAllForumGroupsAsync();
		_logger.LogDebug("ForumGroupsForumsListAsync(): forumGroups → {ForumGroups}", forumGroups);
		_logger.LogDebug("ForumGroupsForumsListAsync(): forumsList → {ForumsList}", forumsList);

        foreach (var fg in forumGroups)
        {
            // Add the forum group with Value of 0 so it won't be used as a target forum
            forumsList.Add(new SelectListItem { Text = fg.Name, Value = "0" });

            var forums = await _forumService.GetAllForumsByGroupIdAsync(fg.Id);
			_logger.LogDebug("ForumGroupsForumsListAsync(): forums → {Forums}", forums);
            foreach (var f in forums)
            {
                forumsList.Add(new SelectListItem { Text = $"{separator}{f.Name}", Value = f.Id.ToString() });
            }
        }

		_logger.LogDebug("<<< Exiting ForumGroupsForumsListAsync()");
        return forumsList;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the forum group model
    /// </summary>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group model
    /// </returns>
    public virtual async Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroup forumGroup)
    {
		_logger.LogDebug(">>> Entering PrepareForumGroupModelAsync(forumGroup={ForumGroup})", forumGroup);
        ArgumentNullException.ThrowIfNull(forumGroup);

        var forumGroupModel = new ForumGroupModel
        {
            Id = forumGroup.Id,
            Name = forumGroup.Name,
            SeName = await _forumService.GetForumGroupSeNameAsync(forumGroup),
        };
        var forums = await _forumService.GetAllForumsByGroupIdAsync(forumGroup.Id);
		_logger.LogDebug("PrepareForumGroupModelAsync(forumGroup={ForumGroup}): forums → {Forums}", forumGroup, forums);
        foreach (var forum in forums)
        {
            var forumModel = await PrepareForumRowModelAsync(forum);
			_logger.LogDebug("PrepareForumGroupModelAsync(forumGroup={ForumGroup}): forumModel → {ForumModel}", forumGroup, forumModel);
            forumGroupModel.Forums.Add(forumModel);
        }

		_logger.LogDebug("<<< Exiting PrepareForumGroupModelAsync(forumGroup={ForumGroup})", forumGroup);
        return forumGroupModel;
    }

    /// <summary>
    /// Prepare the boards index model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the boards index model
    /// </returns>
    public virtual async Task<BoardsIndexModel> PrepareBoardsIndexModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareBoardsIndexModelAsync()");
        var model = new BoardsIndexModel();

		_logger.LogDebug("PrepareBoardsIndexModelAsync(): model → {Model}", model);
        var forumGroups = await _forumService.GetAllForumGroupsAsync();
		_logger.LogDebug("PrepareBoardsIndexModelAsync(): forumGroups → {ForumGroups}", forumGroups);
        foreach (var forumGroup in forumGroups)
        {
            var forumGroupModel = await PrepareForumGroupModelAsync(forumGroup);
			_logger.LogDebug("PrepareBoardsIndexModelAsync(): forumGroupModel → {ForumGroupModel}", forumGroupModel);
            model.ForumGroups.Add(forumGroupModel);
        }
		_logger.LogDebug("<<< Exiting PrepareBoardsIndexModelAsync()");
		_logger.LogDebug(">>> Entering PrepareForumRowModelAsync(forum={Forum})", forum);
		_logger.LogDebug("<<< Exiting PrepareForumRowModelAsync(forum={Forum})", forum);
        return model;
    }

    /// <summary>
    /// Prepare the active discussions model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the active discussions model
    /// </returns>
    public virtual async Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareActiveDiscussionsModelAsync()");
        var model = new ActiveDiscussionsModel
        {
            ViewAllLinkEnabled = true,
            ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled,
            PostsPageSize = _forumSettings.PostsPageSize,
            AllowPostVoting = _forumSettings.AllowPostVoting
        };

        var topics = await _forumService.GetActiveTopicsAsync(0, 0, _forumSettings.HomepageActiveDiscussionsTopicCount);
		_logger.LogDebug("PrepareActiveDiscussionsModelAsync(): topics → {Topics}", topics);
        foreach (var topic in topics)
        {
            var topicModel = await PrepareForumTopicRowModelAsync(topic);
			_logger.LogDebug("PrepareActiveDiscussionsModelAsync(): topicModel → {TopicModel}", topicModel);
            model.ForumTopics.Add(topicModel);
        }

		_logger.LogDebug("<<< Exiting PrepareActiveDiscussionsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare the active discussions model
    /// </summary>
    /// <param name="forumId">Forum identifier</param>
    /// <param name="page">Number of forum topics page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the active discussions model
    /// </returns>
    public virtual async Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync(int forumId, int page)
    {
        var model = new ActiveDiscussionsModel
        {
            ViewAllLinkEnabled = false,
            ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled,
            PostsPageSize = _forumSettings.PostsPageSize,
            AllowPostVoting = _forumSettings.AllowPostVoting
        };

        var pageSize = _forumSettings.ActiveDiscussionsPageSize > 0 ? _forumSettings.ActiveDiscussionsPageSize : 50;

        var topics = await _forumService.GetActiveTopicsAsync(forumId, (page - 1), pageSize);
        model.TopicPageSize = topics.PageSize;
        model.TopicTotalRecords = topics.TotalCount;
        model.TopicPageIndex = topics.PageIndex;
        foreach (var topic in topics)
        {
            var topicModel = await PrepareForumTopicRowModelAsync(topic);
            model.ForumTopics.Add(topicModel);
        }

		_logger.LogDebug(">>> Entering PrepareForumTopicRowModelAsync(topic={Topic})", topic);
		_logger.LogDebug("<<< Exiting PrepareForumTopicRowModelAsync(topic={Topic})", topic);
        return model;
    }

    /// <summary>
    /// Prepare the forum page model
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <param name="page">Number of forum topics page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum page model
    /// </returns>
    public virtual async Task<ForumPageModel> PrepareForumPageModelAsync(Forum forum, int page)
    {
		_logger.LogDebug(">>> Entering PrepareForumPageModelAsync(forum={Forum},page={Page})", forum, page);
        ArgumentNullException.ThrowIfNull(forum);

        var model = new ForumPageModel
        {
            Id = forum.Id,
            Name = forum.Name,
            SeName = await _forumService.GetForumSeNameAsync(forum),
            Description = forum.Description
        };

        var pageSize = _forumSettings.TopicsPageSize > 0 ? _forumSettings.TopicsPageSize : 10;

        model.AllowPostVoting = _forumSettings.AllowPostVoting;
		_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): model.AllowPostVoting → {Arg2}", forum, page, model.AllowPostVoting);

        //subscription
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): customer → {Customer}", forum, page, customer);
        if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
        {
            model.WatchForumText = await _localizationService.GetResourceAsync("Forum.WatchForum");

            var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, forum.Id, 0, 0, 1)).FirstOrDefault();
			_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): forumSubscription → {ForumSubscription}", forum, page, forumSubscription);
            if (forumSubscription != null)
            {
                model.WatchForumText = await _localizationService.GetResourceAsync("Forum.UnwatchForum");
            }
        }

        var topics = await _forumService.GetAllTopicsAsync(forum.Id, 0, string.Empty, ForumSearchType.All, 0, (page - 1), pageSize);
		_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): topics → {Topics}", forum, page, topics);
		_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): model.TopicPageSize → {Arg2}", forum, page, model.TopicPageSize);
		_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): model.TopicTotalRecords → {Arg2}", forum, page, model.TopicTotalRecords);
		_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): model.TopicPageIndex → {Arg2}", forum, page, model.TopicPageIndex);
        model.TopicPageSize = topics.PageSize;
        model.TopicTotalRecords = topics.TotalCount;
        model.TopicPageIndex = topics.PageIndex;
        foreach (var topic in topics)
        {
            var topicModel = await PrepareForumTopicRowModelAsync(topic);
			_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): topicModel → {TopicModel}", forum, page, topicModel);
            model.ForumTopics.Add(topicModel);
        }
        model.IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer);
        model.ForumFeedsEnabled = _forumSettings.ForumFeedsEnabled;
		_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): model.ForumFeedsEnabled → {Arg2}", forum, page, model.ForumFeedsEnabled);
		_logger.LogDebug("PrepareForumPageModelAsync(forum={Forum},page={Page}): model.PostsPageSize → {Arg2}", forum, page, model.PostsPageSize);
        model.PostsPageSize = _forumSettings.PostsPageSize;
		_logger.LogDebug("<<< Exiting PrepareForumPageModelAsync(forum={Forum},page={Page})", forum, page);
        return model;
    }

    /// <summary>
    /// Prepare the forum topic page model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="page">Number of forum posts page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum topic page model
    /// </returns>
    public virtual async Task<ForumTopicPageModel> PrepareForumTopicPageModelAsync(ForumTopic forumTopic, int page)
    {
		_logger.LogDebug(">>> Entering PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page})", forumTopic, page);
        ArgumentNullException.ThrowIfNull(forumTopic);

        //load first post
        var firstPost = (await _forumService.GetAllPostsAsync(forumTopic.Id, 0, string.Empty,
            0, 1)).FirstOrDefault();

        //load posts
        var posts = await _forumService.GetAllPostsAsync(forumTopic.Id, 0, string.Empty,
            page - 1, _forumSettings.PostsPageSize);

        //prepare model
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): currentCustomer → {CurrentCustomer}", forumTopic, page, currentCustomer);

        var firstPostText = posts.FirstOrDefault()?.Text?.Replace(Environment.NewLine, string.Empty);
		_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): firstPostText → {FirstPostText}", forumTopic, page, firstPostText);

        var model = new ForumTopicPageModel
        {
            Id = forumTopic.Id,
            Subject = forumTopic.Subject,
            SeName = await _forumService.GetTopicSeNameAsync(forumTopic),

            IsCustomerAllowedToEditTopic = await _forumService.IsCustomerAllowedToEditTopicAsync(currentCustomer, forumTopic),
            IsCustomerAllowedToDeleteTopic = await _forumService.IsCustomerAllowedToDeleteTopicAsync(currentCustomer, forumTopic),
            IsCustomerAllowedToMoveTopic = await _forumService.IsCustomerAllowedToMoveTopicAsync(currentCustomer, forumTopic),
            IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(currentCustomer),

            MetaTitle = forumTopic.Subject,
            MetaDescription = CommonHelper.EnsureMaximumLength(firstPostText, _forumSettings.TopicMetaDescriptionLength, await _localizationService.GetResourceAsync("Forum.TruncatePostfix"))
        };

        if (model.IsCustomerAllowedToSubscribe)
        {
            model.WatchTopicText = await _localizationService.GetResourceAsync("Forum.WatchTopic");

            var forumTopicSubscription = (await _forumService.GetAllSubscriptionsAsync(currentCustomer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
			_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): forumTopicSubscription → {ForumTopicSubscription}", forumTopic, page, forumTopicSubscription);
            if (forumTopicSubscription != null)
            {
                model.WatchTopicText = await _localizationService.GetResourceAsync("Forum.UnwatchTopic");
            }
        }
        model.ForumEditor = _forumSettings.ForumEditor;
		_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): model.ForumEditor → {Arg2}", forumTopic, page, model.ForumEditor);
		_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): model.PostsPageIndex → {Arg2}", forumTopic, page, model.PostsPageIndex);
		_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): model.PostsPageSize → {Arg2}", forumTopic, page, model.PostsPageSize);
		_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): model.PostsTotalRecords → {Arg2}", forumTopic, page, model.PostsTotalRecords);
        model.PostsPageIndex = posts.PageIndex;
        model.PostsPageSize = posts.PageSize;
        model.PostsTotalRecords = posts.TotalCount;
        foreach (var post in posts)
        {
            var customer = await _customerService.GetCustomerByIdAsync(post.CustomerId);
			_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): customer → {Customer}", forumTopic, page, customer);

            var customerIsGuest = await _customerService.IsGuestAsync(customer);
			_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): customerIsGuest → {CustomerIsGuest}", forumTopic, page, customerIsGuest);
            var customerIsModerator = !customerIsGuest && await _customerService.IsForumModeratorAsync(customer);
			_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): customerIsModerator → {CustomerIsModerator}", forumTopic, page, customerIsModerator);

            var forumPostModel = new ForumPostModel
            {
                Id = post.Id,
                ForumTopicId = post.TopicId,
                ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
                FormattedText = _forumService.FormatPostText(post),
                IsCurrentCustomerAllowedToEditPost = await _forumService.IsCustomerAllowedToEditPostAsync(currentCustomer, post),
                IsCurrentCustomerAllowedToDeletePost = await _forumService.IsCustomerAllowedToDeletePostAsync(currentCustomer, post),
                CustomerId = post.CustomerId,
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !customerIsGuest,
                CustomerName = await _customerService.FormatUsernameAsync(customer),
                IsCustomerForumModerator = customerIsModerator,
                ShowCustomersPostCount = _forumSettings.ShowCustomersPostCount,
                ForumPostCount = await _genericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, NopCustomerDefaults.ForumPostCountAttribute),
                ShowCustomersJoinDate = _customerSettings.ShowCustomersJoinDate && !customerIsGuest,
                CustomerJoinDate = customer?.CreatedOnUtc ?? DateTime.Now,
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages && !customerIsGuest,
                SignaturesEnabled = _forumSettings.SignaturesEnabled,
                FormattedSignature = _forumService.FormatForumSignatureText(await _genericAttributeService.GetAttributeAsync<Customer, string>(post.CustomerId, NopCustomerDefaults.SignatureAttribute)),
            };

            //created on string
            forumPostModel.PostCreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(post.CreatedOnUtc, DateTimeKind.Utc);
            if (_forumSettings.RelativeDateTimeFormattingEnabled)
            {
                var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
				_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): languageCode → {LanguageCode}", forumTopic, page, languageCode);
                var postCreatedAgo = post.CreatedOnUtc.RelativeFormat(languageCode);
				_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): postCreatedAgo → {PostCreatedAgo}", forumTopic, page, postCreatedAgo);
                forumPostModel.PostCreatedOnStr = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
            }
            else
            {
                forumPostModel.PostCreatedOnStr = forumPostModel.PostCreatedOn.ToString("f");
            }

            //avatar
            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                forumPostModel.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                    await _genericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize,
                    _customerSettings.DefaultAvatarEnabled,
                    defaultPictureType: PictureType.Avatar);
            }
            //location
            forumPostModel.ShowCustomersLocation = _customerSettings.ShowCustomersLocation && !customerIsGuest;
			_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): forumPostModel.ShowCustomersLocation → {Arg2}", forumTopic, page, forumPostModel.ShowCustomersLocation);
            if (_customerSettings.ShowCustomersLocation)
            {
                var country = await _countryService.GetCountryByIdAsync(customer.CountryId);
				_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): country → {Country}", forumTopic, page, country);
                forumPostModel.CustomerLocation = country != null ? await _localizationService.GetLocalizedAsync(country, x => x.Name) : string.Empty;
            }

            //votes
            if (_forumSettings.AllowPostVoting)
            {
                forumPostModel.AllowPostVoting = true;
				_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): forumPostModel.AllowPostVoting → {Arg2}", forumTopic, page, forumPostModel.AllowPostVoting);
				_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): forumPostModel.VoteCount → {Arg2}", forumTopic, page, forumPostModel.VoteCount);
                forumPostModel.VoteCount = post.VoteCount;
                var postVote = await _forumService.GetPostVoteAsync(post.Id, currentCustomer);
				_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): postVote → {PostVote}", forumTopic, page, postVote);
                if (postVote != null)
                    forumPostModel.VoteIsUp = postVote.IsUp;
					_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): forumPostModel.VoteIsUp → {Arg2}", forumTopic, page, forumPostModel.VoteIsUp);
            }

            // page number is needed for creating post link in _ForumPost partial view
            forumPostModel.CurrentTopicPage = page;
			_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): forumPostModel.CurrentTopicPage → {Arg2}", forumTopic, page, forumPostModel.CurrentTopicPage);
            model.ForumPostModels.Add(forumPostModel);
        }

        if (_seoSettings.MicrodataEnabled)
        {
            var jsonLdModel = await _jsonLdModelFactory.PrepareJsonLdForumTopicAsync(forumTopic, firstPost, model);
			_logger.LogDebug("PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page}): jsonLdModel → {JsonLdModel}", forumTopic, page, jsonLdModel);
            model.JsonLd = JsonConvert.SerializeObject(jsonLdModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

		_logger.LogDebug("<<< Exiting PrepareForumTopicPageModelAsync(forumTopic={ForumTopic},page={Page})", forumTopic, page);
        return model;
    }

    /// <summary>
    /// Prepare the topic move model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic move model
    /// </returns>
    public virtual async Task<TopicMoveModel> PrepareTopicMoveAsync(ForumTopic forumTopic)
    {
		_logger.LogDebug(">>> Entering PrepareTopicMoveAsync(forumTopic={ForumTopic})", forumTopic);
        ArgumentNullException.ThrowIfNull(forumTopic);

        var model = new TopicMoveModel
        {
            ForumList = await ForumGroupsForumsListAsync(),
            Id = forumTopic.Id,
            TopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
            ForumSelected = forumTopic.ForumId
        };

		_logger.LogDebug("<<< Exiting PrepareTopicMoveAsync(forumTopic={ForumTopic})", forumTopic);
        return model;
    }

    /// <summary>
    /// Prepare the forum topic create model
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <param name="model">Edit forum topic model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareTopicCreateModelAsync(Forum forum, EditForumTopicModel model)
    {
		_logger.LogDebug(">>> Entering PrepareTopicCreateModelAsync(forum={Forum},model={Model})", forum, model);
        ArgumentNullException.ThrowIfNull(forum);

        ArgumentNullException.ThrowIfNull(model);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareTopicCreateModelAsync(forum={Forum},model={Model}): customer → {Customer}", forum, model, customer);
		_logger.LogDebug("PrepareTopicCreateModelAsync(forum={Forum},model={Model}): model.IsEdit → {Arg2}", forum, model, model.IsEdit);
		_logger.LogDebug("PrepareTopicCreateModelAsync(forum={Forum},model={Model}): model.ForumId → {Arg2}", forum, model, model.ForumId);
		_logger.LogDebug("PrepareTopicCreateModelAsync(forum={Forum},model={Model}): model.ForumName → {Arg2}", forum, model, model.ForumName);
        model.IsEdit = false;
        model.ForumId = forum.Id;
        model.ForumName = forum.Name;
        model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
        model.ForumEditor = _forumSettings.ForumEditor;
		_logger.LogDebug("PrepareTopicCreateModelAsync(forum={Forum},model={Model}): model.ForumEditor → {Arg2}", forum, model, model.ForumEditor);
        model.IsCustomerAllowedToSetTopicPriority = await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer);
        model.TopicPriorities = await ForumTopicTypesListAsync();
        model.IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer);
		_logger.LogDebug("<<< Exiting PrepareTopicCreateModelAsync(forum={Forum},model={Model})", forum, model);
		_logger.LogDebug("PrepareTopicCreateModelAsync(forum={Forum},model={Model}): model.DisplayCaptcha → {Arg2}", forum, model, model.DisplayCaptcha);
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum;
    }

    /// <summary>
    /// Prepare the forum topic edit model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="model">Edit forum topic model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareTopicEditModelAsync(ForumTopic forumTopic, EditForumTopicModel model, bool excludeProperties)
    {
		_logger.LogDebug(">>> Entering PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties})", forumTopic, model, excludeProperties);
        ArgumentNullException.ThrowIfNull(forumTopic);

        ArgumentNullException.ThrowIfNull(model);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId) ?? throw new ArgumentException("forum cannot be loaded");
		_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): forum → {Forum}", forumTopic, model, excludeProperties, forum);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): customer → {Customer}", forumTopic, model, excludeProperties, customer);
		_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.IsEdit → {Arg3}", forumTopic, model, excludeProperties, model.IsEdit);
		_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.Id → {Arg3}", forumTopic, model, excludeProperties, model.Id);
        model.IsEdit = true;
        model.Id = forumTopic.Id;
        model.TopicPriorities = await ForumTopicTypesListAsync();
        model.ForumName = forum.Name;
		_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.ForumName → {Arg3}", forumTopic, model, excludeProperties, model.ForumName);
        model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
        model.ForumId = forum.Id;
		_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.ForumId → {Arg3}", forumTopic, model, excludeProperties, model.ForumId);
		_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.ForumEditor → {Arg3}", forumTopic, model, excludeProperties, model.ForumEditor);
		_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.DisplayCaptcha → {Arg3}", forumTopic, model, excludeProperties, model.DisplayCaptcha);
        model.ForumEditor = _forumSettings.ForumEditor;
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum;
        model.IsCustomerAllowedToSetTopicPriority = await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer);
        model.IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer);

        if (!excludeProperties)
        {
            var firstPost = await _forumService.GetFirstPostAsync(forumTopic);
			_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): firstPost → {FirstPost}", forumTopic, model, excludeProperties, firstPost);
			_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.Text → {Arg3}", forumTopic, model, excludeProperties, model.Text);
			_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.Subject → {Arg3}", forumTopic, model, excludeProperties, model.Subject);
			_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): model.TopicTypeId → {Arg3}", forumTopic, model, excludeProperties, model.TopicTypeId);
            model.Text = firstPost.Text;
            model.Subject = forumTopic.Subject;
            model.TopicTypeId = forumTopic.TopicTypeId;
            //subscription            
            if (model.IsCustomerAllowedToSubscribe)
            {
                var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
				_logger.LogDebug("PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties}): forumSubscription → {ForumSubscription}", forumTopic, model, excludeProperties, forumSubscription);
                model.Subscribed = forumSubscription != null;
            }
        }
		_logger.LogDebug("<<< Exiting PrepareTopicEditModelAsync(forumTopic={ForumTopic},model={Model},excludeProperties={ExcludeProperties})", forumTopic, model, excludeProperties);
    }

    /// <summary>
    /// Prepare the forum post create model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="quote">Identifier of the quoted post; pass null to load the empty text</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the edit forum post model
    /// </returns>
    public virtual async Task<EditForumPostModel> PreparePostCreateModelAsync(ForumTopic forumTopic, int? quote, bool excludeProperties)
    {
		_logger.LogDebug(">>> Entering PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties})", forumTopic, quote, excludeProperties);
        ArgumentNullException.ThrowIfNull(forumTopic);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId) ?? throw new ArgumentException("forum cannot be loaded");
		_logger.LogDebug("PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties}): forum → {Forum}", forumTopic, quote, excludeProperties, forum);

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties}): currentCustomer → {CurrentCustomer}", forumTopic, quote, excludeProperties, currentCustomer);
        var model = new EditForumPostModel
        {
            ForumTopicId = forumTopic.Id,
            IsEdit = false,
            ForumEditor = _forumSettings.ForumEditor,
            ForumName = forum.Name,
            ForumTopicSubject = forumTopic.Subject,
            ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
            IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(currentCustomer),
            DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum
        };

        if (!excludeProperties)
        {
            //subscription            
            if (model.IsCustomerAllowedToSubscribe)
            {
                var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(currentCustomer.Id,
                    0, forumTopic.Id, 0, 1)).FirstOrDefault();
                model.Subscribed = forumSubscription != null;
            }

            // Insert the quoted text
            var text = string.Empty;
			_logger.LogDebug("PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties}): text → {Text}", forumTopic, quote, excludeProperties, text);
            if (quote.HasValue)
            {
                var quotePost = await _forumService.GetPostByIdAsync(quote.Value);
				_logger.LogDebug("PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties}): quotePost → {QuotePost}", forumTopic, quote, excludeProperties, quotePost);

                if (quotePost != null && quotePost.TopicId == forumTopic.Id)
                {
                    var customer = await _customerService.GetCustomerByIdAsync(quotePost.CustomerId);
					_logger.LogDebug("PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties}): customer → {Customer}", forumTopic, quote, excludeProperties, customer);
                    var username = await _customerService.FormatUsernameAsync(customer);
					_logger.LogDebug("PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties}): username → {Username}", forumTopic, quote, excludeProperties, username);
                    var quotePostText = quotePost.Text;

                    switch (_forumSettings.ForumEditor)
                    {
                        case EditorType.SimpleTextBox:
                            text = $"{username}:\n{quotePostText}\n";
                            break;
                        case EditorType.BBCodeEditor:
                            text = $"[quote={username}]{_bbCodeHelper.RemoveQuotes(quotePostText)}[/quote]";
                            break;
                        case EditorType.MarkdownEditor:
                            var quotedLines = quotePostText.Split('\n').Select(line => $"> {line}");
							_logger.LogDebug("PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties}): quotedLines → {QuotedLines}", forumTopic, quote, excludeProperties, quotedLines);
                            text = $"**{username}:**\n\n{string.Join("\n", quotedLines)}\n\n";
                            break;
                    }
                    model.Text = text;
					_logger.LogDebug("PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties}): model.Text → {Arg3}", forumTopic, quote, excludeProperties, model.Text);
                }
            }
        }

		_logger.LogDebug("<<< Exiting PreparePostCreateModelAsync(forumTopic={ForumTopic},quote={Quote},excludeProperties={ExcludeProperties})", forumTopic, quote, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare the forum post edit model
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the edit forum post model
    /// </returns>
    public virtual async Task<EditForumPostModel> PreparePostEditModelAsync(ForumPost forumPost, bool excludeProperties)
    {
		_logger.LogDebug(">>> Entering PreparePostEditModelAsync(forumPost={ForumPost},excludeProperties={ExcludeProperties})", forumPost, excludeProperties);
        ArgumentNullException.ThrowIfNull(forumPost);

        var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId) ?? throw new ArgumentException("forum topic cannot be loaded");
		_logger.LogDebug("PreparePostEditModelAsync(forumPost={ForumPost},excludeProperties={ExcludeProperties}): forumTopic → {ForumTopic}", forumPost, excludeProperties, forumTopic);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId) ?? throw new ArgumentException("forum cannot be loaded");
		_logger.LogDebug("PreparePostEditModelAsync(forumPost={ForumPost},excludeProperties={ExcludeProperties}): forum → {Forum}", forumPost, excludeProperties, forum);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PreparePostEditModelAsync(forumPost={ForumPost},excludeProperties={ExcludeProperties}): customer → {Customer}", forumPost, excludeProperties, customer);
        var model = new EditForumPostModel
        {
            Id = forumPost.Id,
            ForumTopicId = forumTopic.Id,
            IsEdit = true,
            ForumEditor = _forumSettings.ForumEditor,
            ForumName = forum.Name,
            ForumTopicSubject = forumTopic.Subject,
            ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
            IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer),
            DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum
        };

        if (!excludeProperties)
        {
            model.Text = forumPost.Text;
			_logger.LogDebug("PreparePostEditModelAsync(forumPost={ForumPost},excludeProperties={ExcludeProperties}): model.Text → {Arg2}", forumPost, excludeProperties, model.Text);
            //subscription
            if (model.IsCustomerAllowedToSubscribe)
            {
                var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
				_logger.LogDebug("PreparePostEditModelAsync(forumPost={ForumPost},excludeProperties={ExcludeProperties}): forumSubscription → {ForumSubscription}", forumPost, excludeProperties, forumSubscription);
                model.Subscribed = forumSubscription != null;
            }
        }

		_logger.LogDebug("<<< Exiting PreparePostEditModelAsync(forumPost={ForumPost},excludeProperties={ExcludeProperties})", forumPost, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare the search model
    /// </summary>
    /// <param name="searchterms">Search terms</param>
    /// <param name="advs">Whether to use the advanced search</param>
    /// <param name="forumId">Forum identifier</param>
    /// <param name="within">String representation of int value of ForumSearchType</param>
    /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
    /// <param name="page">Number of items page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search model
    /// </returns>
    public virtual async Task<SearchModel> PrepareSearchModelAsync(string searchterms, bool? advs, string forumId,
        string within, string limitDays, int page)
    {
		_logger.LogDebug(">>> Entering PrepareSearchModelAsync(searchterms={Searchterms},advs={Advs},forumId={ForumId},within={Within},limitDays={LimitDays},page={Page})", searchterms, advs, forumId, within, limitDays, page);
        var model = new SearchModel();

		_logger.LogDebug("PrepareSearchModelAsync(searchterms={Searchterms},advs={Advs},forumId={ForumId},within={Within},limitDays={LimitDays},page={Page}): model → {Model}", searchterms, advs, forumId, within, limitDays, page, model);
        var pageSize = 10;

        // Create the values for the "Limit results to previous" select list
        var limitList = new List<SelectListItem>
        {
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.AllResults"),
                Value = "0"
            },
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1day"),
                Value = "1"
            },
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.7days"),
                Value = "7"
            },
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.2weeks"),
                Value = "14"
            },
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1month"),
                Value = "30"
            },
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.3months"),
                Value = "92"
            },
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.6months"),
                Value = "183"
            },
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1year"),
                Value = "365"
            }
        };
        model.LimitList = limitList;

        // Create the values for the "Search in forum" select list
        var forumsSelectList = new List<SelectListItem>
        {
            new() {
                Text = await _localizationService.GetResourceAsync("Forum.Search.SearchInForum.All"),
                Value = "0",
                Selected = true,
            }
        };

        var separator = "--";
        var forumGroups = await _forumService.GetAllForumGroupsAsync();
        foreach (var fg in forumGroups)
        {
            // Add the forum group with value as '-' so it can't be used as a target forum id
            forumsSelectList.Add(new SelectListItem { Text = fg.Name, Value = "-" });

            var forums = await _forumService.GetAllForumsByGroupIdAsync(fg.Id);
            foreach (var f in forums)
            {
                forumsSelectList.Add(
                    new SelectListItem
                    {
                        Text = $"{separator}{f.Name}",
                        Value = f.Id.ToString()
                    });
            }
        }
        model.ForumList = forumsSelectList;

        // Create the values for "Search within" select list            
        var withinList = new List<SelectListItem>
        {
            new() {
                Value = ((int) ForumSearchType.All).ToString(),
                Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.All")
            },
            new() {
                Value = ((int) ForumSearchType.TopicTitlesOnly).ToString(),
                Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.TopicTitlesOnly")
            },
            new() {
                Value = ((int) ForumSearchType.PostTextOnly).ToString(),
                Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.PostTextOnly")
            }
        };
        model.WithinList = withinList;

        _ = int.TryParse(forumId, out var forumIdSelected);
        model.ForumIdSelected = forumIdSelected;

        _ = int.TryParse(within, out var withinSelected);
        model.WithinSelected = withinSelected;

        _ = int.TryParse(limitDays, out var limitDaysSelected);
        model.LimitDaysSelected = limitDaysSelected;

        var searchTermMinimumLength = _forumSettings.ForumSearchTermMinimumLength;

        model.ShowAdvancedSearch = advs.GetValueOrDefault();
        model.SearchResultsVisible = false;
        model.NoResultsVisisble = false;
        model.PostsPageSize = _forumSettings.PostsPageSize;

        model.AllowPostVoting = _forumSettings.AllowPostVoting;

        try
        {
            if (!string.IsNullOrWhiteSpace(searchterms))
            {
                searchterms = searchterms.Trim();
                model.SearchTerms = searchterms;

                if (searchterms.Length < searchTermMinimumLength)
                {
                    throw new NopException(string.Format(await _localizationService.GetResourceAsync("Forum.SearchTermMinimumLengthIsNCharacters"),
                        searchTermMinimumLength));
                }

                ForumSearchType searchWithin = 0;
                var limitResultsToPrevious = 0;
                if (advs.GetValueOrDefault())
                {
                    searchWithin = (ForumSearchType)withinSelected;
                    limitResultsToPrevious = limitDaysSelected;
                }

                if (_forumSettings.SearchResultsPageSize > 0)
                {
                    pageSize = _forumSettings.SearchResultsPageSize;
                }

                var topics = await _forumService.GetAllTopicsAsync(forumIdSelected, 0, searchterms, searchWithin,
                    limitResultsToPrevious, page - 1, pageSize);
                model.TopicPageSize = topics.PageSize;
                model.TopicTotalRecords = topics.TotalCount;
                model.TopicPageIndex = topics.PageIndex;
                foreach (var topic in topics)
                {
                    var topicModel = await PrepareForumTopicRowModelAsync(topic);
                    model.ForumTopics.Add(topicModel);
                }

                model.SearchResultsVisible = (topics.Any());
                model.NoResultsVisisble = !(model.SearchResultsVisible);

                return model;
            }
            model.SearchResultsVisible = false;
        }
        catch (Exception ex)
        {
            model.Error = ex.Message;
        }

        //some exception raised
        model.TopicPageSize = pageSize;
        model.TopicTotalRecords = 0;
        model.TopicPageIndex = page - 1;

		_logger.LogDebug("<<< Exiting PrepareSearchModelAsync(searchterms={Searchterms},advs={Advs},forumId={ForumId},within={Within},limitDays={LimitDays},page={Page})", searchterms, advs, forumId, within, limitDays, page);
        return model;
    }

    /// <summary>
    /// Prepare the last post model
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <param name="showTopic">Whether to show topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the last post model
    /// </returns>
    public virtual async Task<LastPostModel> PrepareLastPostModelAsync(ForumPost forumPost, bool showTopic)
    {
		_logger.LogDebug(">>> Entering PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic})", forumPost, showTopic);
        var model = new LastPostModel
        {
            ShowTopic = showTopic
        };

        //do not throw an exception here
        if (forumPost == null)
            return model;

        var topic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
		_logger.LogDebug("PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic}): topic → {Topic}", forumPost, showTopic, topic);

        if (topic is null)
            return model;

        var customer = await _customerService.GetCustomerByIdAsync(forumPost.CustomerId);
		_logger.LogDebug("PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic}): customer → {Customer}", forumPost, showTopic, customer);

        model.Id = forumPost.Id;
		_logger.LogDebug("PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic}): model.Id → {Arg2}", forumPost, showTopic, model.Id);
		_logger.LogDebug("PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic}): model.ForumTopicId → {Arg2}", forumPost, showTopic, model.ForumTopicId);
        model.ForumTopicId = topic.Id;
        model.ForumTopicSeName = await _forumService.GetTopicSeNameAsync(topic);
        model.ForumTopicSubject = _forumService.StripTopicSubject(topic);
        model.CustomerId = forumPost.CustomerId;
		_logger.LogDebug("PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic}): model.CustomerId → {Arg2}", forumPost, showTopic, model.CustomerId);
        model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(customer);
        model.CustomerName = await _customerService.FormatUsernameAsync(customer);
        //created on string
        var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
		_logger.LogDebug("PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic}): languageCode → {LanguageCode}", forumPost, showTopic, languageCode);
        if (_forumSettings.RelativeDateTimeFormattingEnabled)
        {
            var postCreatedAgo = forumPost.CreatedOnUtc.RelativeFormat(languageCode);
			_logger.LogDebug("PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic}): postCreatedAgo → {PostCreatedAgo}", forumPost, showTopic, postCreatedAgo);
            model.PostCreatedOnStr = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
        }
        else
            model.PostCreatedOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");

		_logger.LogDebug("<<< Exiting PrepareLastPostModelAsync(forumPost={ForumPost},showTopic={ShowTopic})", forumPost, showTopic);
        return model;
    }

    /// <summary>
    /// Prepare the forum breadcrumb model
    /// </summary>
    /// <param name="forumGroupId">Forum group identifier; pass null to load nothing</param>
    /// <param name="forumId">Forum identifier; pass null to load breadcrumbs up to forum group</param>
    /// <param name="forumTopicId">Forum topic identifier; pass null to load breadcrumbs up to forum</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum breadcrumb model
    /// </returns>
    public virtual async Task<ForumBreadcrumbModel> PrepareForumBreadcrumbModelAsync(int? forumGroupId, int? forumId, int? forumTopicId)
    {
		_logger.LogDebug(">>> Entering PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId})", forumGroupId, forumId, forumTopicId);
        var model = new ForumBreadcrumbModel();

		_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model → {Model}", forumGroupId, forumId, forumTopicId, model);
        ForumTopic forumTopic = null;
		_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): forumTopic → {ForumTopic}", forumGroupId, forumId, forumTopicId, forumTopic);
        if (forumTopicId.HasValue)
        {
            forumTopic = await _forumService.GetTopicByIdAsync(forumTopicId.Value);
            if (forumTopic != null)
            {
                model.ForumTopicId = forumTopic.Id;
				_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model.ForumTopicId → {Arg3}", forumGroupId, forumId, forumTopicId, model.ForumTopicId);
				_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model.ForumTopicSubject → {Arg3}", forumGroupId, forumId, forumTopicId, model.ForumTopicSubject);
                model.ForumTopicSubject = forumTopic.Subject;
                model.ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic);
            }
        }

        var forum = await _forumService.GetForumByIdAsync(forumTopic != null ? forumTopic.ForumId : (forumId ?? 0));
		_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): forum → {Forum}", forumGroupId, forumId, forumTopicId, forum);
        if (forum != null)
        {
            model.ForumId = forum.Id;
			_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model.ForumId → {Arg3}", forumGroupId, forumId, forumTopicId, model.ForumId);
			_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model.ForumName → {Arg3}", forumGroupId, forumId, forumTopicId, model.ForumName);
            model.ForumName = forum.Name;
            model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
        }

        var forumGroup = await _forumService.GetForumGroupByIdAsync(forum != null ? forum.ForumGroupId : (forumGroupId ?? 0));
		_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): forumGroup → {ForumGroup}", forumGroupId, forumId, forumTopicId, forumGroup);
        if (forumGroup != null)
        {
            model.ForumGroupId = forumGroup.Id;
			_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model.ForumGroupId → {Arg3}", forumGroupId, forumId, forumTopicId, model.ForumGroupId);
			_logger.LogDebug("PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model.ForumGroupName → {Arg3}", forumGroupId, forumId, forumTopicId, model.ForumGroupName);
            model.ForumGroupName = forumGroup.Name;
            model.ForumGroupSeName = await _forumService.GetForumGroupSeNameAsync(forumGroup);
        }

		_logger.LogDebug("<<< Exiting PrepareForumBreadcrumbModelAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId})", forumGroupId, forumId, forumTopicId);
        return model;
    }

    /// <summary>
    /// Prepare the customer forum subscriptions model
    /// </summary>
    /// <param name="page">Number of items page; pass null to load the first page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer forum subscriptions model
    /// </returns>
    public virtual async Task<CustomerForumSubscriptionsModel> PrepareCustomerForumSubscriptionsModelAsync(int? page)
    {
		_logger.LogDebug(">>> Entering PrepareCustomerForumSubscriptionsModelAsync(page={Page})", page);
        var pageIndex = 0;
        if (page > 0)
        {
            pageIndex = page.Value - 1;
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareCustomerForumSubscriptionsModelAsync(page={Page}): customer → {Customer}", page, customer);

        var pageSize = _forumSettings.ForumSubscriptionsPageSize;

        var list = await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, 0, pageIndex, pageSize);
		_logger.LogDebug("PrepareCustomerForumSubscriptionsModelAsync(page={Page}): list → {List}", page, list);

        var model = new CustomerForumSubscriptionsModel();
		_logger.LogDebug("PrepareCustomerForumSubscriptionsModelAsync(page={Page}): model → {Model}", page, model);

        foreach (var forumSubscription in list)
        {
            var forumTopicId = forumSubscription.TopicId;
            var forumId = forumSubscription.ForumId;
            var topicSubscription = false;
			_logger.LogDebug("PrepareCustomerForumSubscriptionsModelAsync(page={Page}): topicSubscription → {TopicSubscription}", page, topicSubscription);
            var title = string.Empty;
			_logger.LogDebug("PrepareCustomerForumSubscriptionsModelAsync(page={Page}): title → {Title}", page, title);
            var slug = string.Empty;
			_logger.LogDebug("PrepareCustomerForumSubscriptionsModelAsync(page={Page}): slug → {Slug}", page, slug);

            if (forumTopicId > 0)
            {
                topicSubscription = true;
                var forumTopic = await _forumService.GetTopicByIdAsync(forumTopicId);
				_logger.LogDebug("PrepareCustomerForumSubscriptionsModelAsync(page={Page}): forumTopic → {ForumTopic}", page, forumTopic);
                if (forumTopic != null)
                {
                    title = forumTopic.Subject;
                    slug = await _forumService.GetTopicSeNameAsync(forumTopic);
                }
            }
            else
            {
                var forum = await _forumService.GetForumByIdAsync(forumId);
				_logger.LogDebug("PrepareCustomerForumSubscriptionsModelAsync(page={Page}): forum → {Forum}", page, forum);
                if (forum != null)
                {
                    title = forum.Name;
                    slug = await _forumService.GetForumSeNameAsync(forum);
                }
            }

            model.ForumSubscriptions.Add(new CustomerForumSubscriptionsModel.ForumSubscriptionModel
            {
                Id = forumSubscription.Id,
                ForumTopicId = forumTopicId,
                ForumId = forumSubscription.ForumId,
                TopicSubscription = topicSubscription,
                Title = title,
                Slug = slug,
            });
        }

        model.PagerModel = new PagerModel(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = NopRouteNames.Standard.CUSTOMER_FORUM_SUBSCRIPTIONS,
            UseRouteLinks = true,
            RouteValues = new ForumSubscriptionsRouteValues { PageNumber = pageIndex }
        };

		_logger.LogDebug("<<< Exiting PrepareCustomerForumSubscriptionsModelAsync(page={Page})", page);
        return model;
    }

    /// <summary>
    /// Prepare the forum topic row model
    /// </summary>
    /// <param name="topic">Forum topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum topic row model
    /// </returns>
    public virtual async Task<ForumTopicRowModel> PrepareForumTopicRowModelAsync(ForumTopic topic)
    {
        ArgumentNullException.ThrowIfNull(topic);

        var customer = await _customerService.GetCustomerByIdAsync(topic.CustomerId);
		_logger.LogDebug("PrepareForumTopicRowModelAsync(topic={Topic}): customer → {Customer}", topic, customer);
        var firstPost = await _forumService.GetFirstPostAsync(topic);
		_logger.LogDebug("PrepareForumTopicRowModelAsync(topic={Topic}): firstPost → {FirstPost}", topic, firstPost);

        var topicModel = new ForumTopicRowModel
        {
            Id = topic.Id,
            Subject = topic.Subject,
            SeName = await _forumService.GetTopicSeNameAsync(topic),
            LastPostId = topic.LastPostId,
            NumPosts = topic.NumPosts,
            Views = topic.Views,
            NumReplies = topic.NumPosts > 0 ? topic.NumPosts - 1 : 0,
            ForumTopicType = topic.ForumTopicType,
            CustomerId = topic.CustomerId,
            AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(customer),
            CustomerName = await _customerService.FormatUsernameAsync(customer),
            TotalPostPages = (topic.NumPosts / _forumSettings.PostsPageSize) + 1,
            Votes = firstPost?.VoteCount ?? 0
        };

        return topicModel;
    }

    /// <summary>
    /// Prepare the forum row model
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum row model
    /// </returns>
    public virtual async Task<ForumRowModel> PrepareForumRowModelAsync(Forum forum)
    {
        ArgumentNullException.ThrowIfNull(forum);

        var forumModel = new ForumRowModel
        {
            Id = forum.Id,
            Name = forum.Name,
            SeName = await _forumService.GetForumSeNameAsync(forum),
            Description = forum.Description,
            NumTopics = forum.NumTopics,
            NumPosts = forum.NumPosts,
            LastPostId = forum.LastPostId,
        };

        return forumModel;
    }

    #endregion

    #region Nested class

    /// <summary>
    /// record that has only page for route value. Used for (My Account) Forum Subscriptions pagination
    /// </summary>
    public partial record ForumSubscriptionsRouteValues : BaseRouteValues;

    /// <summary>
    /// record that has search options for route values. Used for Search result pagination
    /// </summary>
    public partial record ForumSearchRouteValues : BaseRouteValues
    {
        public string Searchterms { get; set; }
        public string Advs { get; set; }
        public string ForumId { get; set; }
        public string Within { get; set; }
        public string LimitDays { get; set; }
    }

    /// <summary>
    /// record that has only page for route value. Used for Active Discussions (forums) pagination
    /// </summary>
    public partial record ForumActiveDiscussionsRouteValues : BaseRouteValues;

    #endregion
}