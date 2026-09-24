using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.News;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the news model factory
/// </summary>
public partial class NewsModelFactory : INewsModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly INewsService _newsService;
    protected readonly IPictureService _pictureService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public NewsModelFactory(CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        INewsService newsService,
        IPictureService pictureService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        NewsSettings newsSettings)
    {
		_logger.LogError(">>> Entering NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings})", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings);
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _customerSettings → {_customerSettings}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _customerSettings);
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _captchaSettings → {_captchaSettings}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _captchaSettings);
    _logger.LogWarning("<<< Exiting NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings})", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings);
        _customerService = customerService;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _customerService → {_customerService}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _dateTimeHelper → {_dateTimeHelper}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _dateTimeHelper);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _genericAttributeService → {_genericAttributeService}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _genericAttributeService);
        _newsService = newsService;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _newsService → {_newsService}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _newsService);
        _pictureService = pictureService;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _pictureService → {_pictureService}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _pictureService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _staticCacheManager → {_staticCacheManager}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _staticCacheManager);
        _storeContext = storeContext;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _storeContext → {_storeContext}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _storeContext);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _urlRecordService → {_urlRecordService}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _urlRecordService);
        _workContext = workContext;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _workContext → {_workContext}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _workContext);
        _mediaSettings = mediaSettings;
		_logger.LogDebug("NewsModelFactory(captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},newsService={NewsService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings},newsSettings={NewsSettings}): _mediaSettings → {_mediaSettings}", captchaSettings, customerSettings, customerService, dateTimeHelper, genericAttributeService, newsService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, newsSettings, _mediaSettings);
        _newsSettings = newsSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the news item model
    /// </summary>
    /// <param name="model">News item model</param>
    /// <param name="newsItem">News item</param>
    /// <param name="prepareComments">Whether to prepare news comment models</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news item model
    /// </returns>
    public virtual async Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool prepareComments)
    {
		_logger.LogError(">>> Entering PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments})", model, newsItem, prepareComments);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(newsItem);

        model.Id = newsItem.Id;
		_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): model.Id → {Arg3}", model, newsItem, prepareComments, model.Id);
		_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): model.MetaTitle → {Arg3}", model, newsItem, prepareComments, model.MetaTitle);
		_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): model.MetaDescription → {Arg3}", model, newsItem, prepareComments, model.MetaDescription);
		_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): model.MetaKeywords → {Arg3}", model, newsItem, prepareComments, model.MetaKeywords);
        model.MetaTitle = newsItem.MetaTitle;
        model.MetaDescription = newsItem.MetaDescription;
        model.MetaKeywords = newsItem.MetaKeywords;
        model.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false);
        model.Title = newsItem.Title;
		_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): model.Title → {Arg3}", model, newsItem, prepareComments, model.Title);
		_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): model.Short → {Arg3}", model, newsItem, prepareComments, model.Short);
		_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): model.Full → {Arg3}", model, newsItem, prepareComments, model.Full);
		_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): model.AllowComments → {Arg3}", model, newsItem, prepareComments, model.AllowComments);
        model.Short = newsItem.Short;
        model.Full = newsItem.Full;
        model.AllowComments = newsItem.AllowComments;

        model.PreventNotRegisteredUsersToLeaveComments =
            await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) &&
            !_newsSettings.AllowNotRegisteredUsersToLeaveComments;

        model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.StartDateUtc ?? newsItem.CreatedOnUtc, DateTimeKind.Utc);
        model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage;

        //number of news comments
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): store → {Store}", model, newsItem, prepareComments, store);
        var storeId = _newsSettings.ShowNewsCommentsPerStore ? store.Id : 0;

        model.NumberOfComments = await _newsService.GetNewsCommentsCountAsync(newsItem, storeId, true);

        if (prepareComments)
        {
            var newsComments = await _newsService.GetAllCommentsAsync(
                newsItemId: newsItem.Id,
                approved: true,
                storeId: _newsSettings.ShowNewsCommentsPerStore ? store.Id : 0);

            foreach (var nc in newsComments.OrderBy(comment => comment.CreatedOnUtc))
            {
                var commentModel = await PrepareNewsCommentModelAsync(nc);
				_logger.LogError("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments}): commentModel → {CommentModel}", model, newsItem, prepareComments, commentModel);
                model.Comments.Add(commentModel);
            }
        }

		_logger.LogWarning("<<< Exiting PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},prepareComments={PrepareComments})", model, newsItem, prepareComments);
        return model;
    }

    /// <summary>
    /// Prepare the home page news items model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the home page news items model
    /// </returns>
    public virtual async Task<HomepageNewsItemsModel> PrepareHomepageNewsItemsModelAsync()
    {
		_logger.LogError(">>> Entering PrepareHomepageNewsItemsModelAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
		_logger.LogError("PrepareHomepageNewsItemsModelAsync(): store → {Store}", store);
		_logger.LogError("PrepareHomepageNewsItemsModelAsync(): language → {Language}", language);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.HomepageNewsModelKey, language, store);
        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
			_logger.LogError("PrepareHomepageNewsItemsModelAsync(): cacheKey → {CacheKey}", cacheKey);
            var newsItems = await _newsService.GetAllNewsAsync(language.Id, store.Id, 0, _newsSettings.MainPageNewsCount);
			_logger.LogError("PrepareHomepageNewsItemsModelAsync(): newsItems → {NewsItems}", newsItems);

            return new HomepageNewsItemsModel
            {
                WorkingLanguageId = language.Id,
                NewsItems = await newsItems.SelectAwait(async newsItem =>
                {
                    var newsModel = new NewsItemModel();
                    await PrepareNewsItemModelAsync(newsModel, newsItem, false);
					var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
                    return newsModel;
                }).ToListAsync()
            };
			_logger.LogError("PrepareHomepageNewsItemsModelAsync(): database query executed in {Arg0} ms", __logStartDb.ElapsedMilliseconds);
        });

        //"Comments" property of "NewsItemModel" object depends on the current customer.
        //Furthermore, we just don't need it for home page news. So let's reset it.
        //But first we need to clone the cached model (the updated one should not be cached)
        var model = cachedModel with { };
        foreach (var newsItemModel in model.NewsItems)
            newsItemModel.Comments.Clear();

		_logger.LogError("PrepareHomepageNewsItemsModelAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareHomepageNewsItemsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare the news item list model
    /// </summary>
    /// <param name="command">News paging filtering model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news item list model
    /// </returns>
    public virtual async Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsPagingFilteringModel command)
    {
		_logger.LogError(">>> Entering PrepareNewsItemListModelAsync(command={Command})", command);
        if (command.PageSize <= 0)
            command.PageSize = _newsSettings.NewsArchivePageSize;
			_logger.LogDebug("PrepareNewsItemListModelAsync(command={Command}): command.PageSize → {Arg1}", command, command.PageSize);
        if (command.PageNumber <= 0)
            command.PageNumber = 1;
			_logger.LogDebug("PrepareNewsItemListModelAsync(command={Command}): command.PageNumber → {Arg1}", command, command.PageNumber);

        var language = await _workContext.GetWorkingLanguageAsync();
		_logger.LogError("PrepareNewsItemListModelAsync(command={Command}): language → {Language}", command, language);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareNewsItemListModelAsync(command={Command}): store → {Store}", command, store);
        var newsItems = await _newsService.GetAllNewsAsync(language.Id, store.Id, command.PageNumber - 1, command.PageSize);
		_logger.LogError("PrepareNewsItemListModelAsync(command={Command}): newsItems → {NewsItems}", command, newsItems);

        var model = new NewsItemListModel
        {
            WorkingLanguageId = language.Id,
            NewsItems = await newsItems.SelectAwait(async newsItem =>
            {
                var newsModel = new NewsItemModel();
                await PrepareNewsItemModelAsync(newsModel, newsItem, false);
                return newsModel;
            }).ToListAsync()
        };
		_logger.LogError("PrepareNewsItemListModelAsync(command)={Command}: database query executed in {Arg1} ms", command, __logStart.ElapsedMilliseconds);
        model.PagingFilteringContext.LoadPagedList(newsItems);

		_logger.LogWarning("<<< Exiting PrepareNewsItemListModelAsync(command={Command})", command);
        return model;
    }

    /// <summary>
    /// Prepare the news comment model
    /// </summary>
    /// <param name="newsComment">News comment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comment model
    /// </returns>
    public virtual async Task<NewsCommentModel> PrepareNewsCommentModelAsync(NewsComment newsComment)
    {
		_logger.LogError(">>> Entering PrepareNewsCommentModelAsync(newsComment={NewsComment})", newsComment);
        ArgumentNullException.ThrowIfNull(newsComment);

        var customer = await _customerService.GetCustomerByIdAsync(newsComment.CustomerId);
		_logger.LogError("PrepareNewsCommentModelAsync(newsComment={NewsComment}): customer → {Customer}", newsComment, customer);

        var model = new NewsCommentModel
        {
            Id = newsComment.Id,
            CustomerId = newsComment.CustomerId,
            CustomerName = await _customerService.FormatUsernameAsync(customer),
            CommentTitle = newsComment.CommentTitle,
            CommentText = newsComment.CommentText,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc),
            AllowViewingProfiles = _customerSettings.AllowViewingProfiles && newsComment.CustomerId != 0 && !await _customerService.IsGuestAsync(customer),
        };

        if (_customerSettings.AllowCustomersToUploadAvatars)
        {
            model.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<Customer, int>(newsComment.CustomerId, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
        }

		_logger.LogWarning("<<< Exiting PrepareNewsCommentModelAsync(newsComment={NewsComment})", newsComment);
        return model;
    }

    #endregion
}