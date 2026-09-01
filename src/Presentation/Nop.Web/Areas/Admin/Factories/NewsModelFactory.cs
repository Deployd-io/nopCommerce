using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the news model factory implementation
/// </summary>
public partial class NewsModelFactory : INewsModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsService _newsService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly IStoreService _storeService;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public NewsModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IHtmlFormatter htmlFormatter,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INewsService newsService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        IStoreService storeService,
        IUrlRecordService urlRecordService)
    {
		_logger.LogDebug(">>> Entering NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService})", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _catalogSettings → {_catalogSettings}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _catalogSettings);
        _customerService = customerService;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _customerService → {_customerService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _customerService);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _baseAdminModelFactory → {_baseAdminModelFactory}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _baseAdminModelFactory);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _dateTimeHelper → {_dateTimeHelper}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _dateTimeHelper);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _htmlFormatter → {_htmlFormatter}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _htmlFormatter);
        _languageService = languageService;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _languageService → {_languageService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _localizationService → {_localizationService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _localizationService);
        _newsService = newsService;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _newsService → {_newsService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _newsService);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _storeMappingSupportedModelFactory → {_storeMappingSupportedModelFactory}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _storeMappingSupportedModelFactory);
        _storeService = storeService;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _storeService → {_storeService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _storeService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("NewsModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},newsService={NewsService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _urlRecordService → {_urlRecordService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, newsService, storeService, urlRecordService, _urlRecordService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare news content model
    /// </summary>
    /// <param name="newsContentModel">News content model</param>
    /// <param name="filterByNewsItemId">Filter by news item ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news content model
    /// </returns>
    public virtual async Task<NewsContentModel> PrepareNewsContentModelAsync(NewsContentModel newsContentModel, int? filterByNewsItemId)
    {
		_logger.LogDebug(">>> Entering PrepareNewsContentModelAsync(newsContentModel={NewsContentModel},filterByNewsItemId={FilterByNewsItemId})", newsContentModel, filterByNewsItemId);
        ArgumentNullException.ThrowIfNull(newsContentModel);

        //prepare nested search models
        await PrepareNewsItemSearchModelAsync(newsContentModel.NewsItems);
        var newsItem = await _newsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
		_logger.LogDebug("PrepareNewsContentModelAsync(newsContentModel={NewsContentModel},filterByNewsItemId={FilterByNewsItemId}): newsItem → {NewsItem}", newsContentModel, filterByNewsItemId, newsItem);
        await PrepareNewsCommentSearchModelAsync(newsContentModel.NewsComments, newsItem);

		_logger.LogDebug("<<< Exiting PrepareNewsContentModelAsync(newsContentModel={NewsContentModel},filterByNewsItemId={FilterByNewsItemId})", newsContentModel, filterByNewsItemId);
        return newsContentModel;
    }

    /// <summary>
    /// Prepare paged news item list model
    /// </summary>
    /// <param name="searchModel">News item search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news item list model
    /// </returns>
    public virtual async Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsItemSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareNewsItemListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get news items
        var newsItems = await _newsService.GetAllNewsAsync(showHidden: true,
            storeId: searchModel.SearchStoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
            title: searchModel.SearchTitle);

        //prepare list model
        var model = await new NewsItemListModel().PrepareToGridAsync(searchModel, newsItems, () =>
        {
            return newsItems.SelectAwait(async newsItem =>
            {
                //fill in model values from the entity
                var newsItemModel = newsItem.ToModel<NewsItemModel>();
				_logger.LogDebug("PrepareNewsItemListModelAsync(searchModel={SearchModel}): newsItemModel → {NewsItemModel}", searchModel, newsItemModel);

                //little performance optimization: ensure that "Full" is not returned
                newsItemModel.Full = string.Empty;
				_logger.LogDebug("PrepareNewsItemListModelAsync(searchModel={SearchModel}): newsItemModel.Full → {Arg1}", searchModel, newsItemModel.Full);

                //convert dates to the user time
                if (newsItem.StartDateUtc.HasValue)
                    newsItemModel.StartDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.StartDateUtc.Value, DateTimeKind.Utc);
                if (newsItem.EndDateUtc.HasValue)
                    newsItemModel.EndDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.EndDateUtc.Value, DateTimeKind.Utc);
                newsItemModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                newsItemModel.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, true, false);
                newsItemModel.LanguageName = (await _languageService.GetLanguageByIdAsync(newsItem.LanguageId))?.Name;
                newsItemModel.ApprovedComments = await _newsService.GetNewsCommentsCountAsync(newsItem, isApproved: true);
                newsItemModel.NotApprovedComments = await _newsService.GetNewsCommentsCountAsync(newsItem, isApproved: false);

                return newsItemModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareNewsItemListModelAsync(searchModel={SearchModel})", searchModel);
		_logger.LogDebug(">>> Entering PrepareNewsItemSearchModelAsync(searchModel={SearchModel})", searchModel);
		_logger.LogDebug("<<< Exiting PrepareNewsItemSearchModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare news item model
    /// </summary>
    /// <param name="model">News item model</param>
    /// <param name="newsItem">News item</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news item model
    /// </returns>
    public virtual async Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},excludeProperties={ExcludeProperties})", model, newsItem, excludeProperties);
        //fill in model values from the entity
        if (newsItem != null)
        {
            if (model == null)
            {
                model = newsItem.ToModel<NewsItemModel>();
				_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},excludeProperties={ExcludeProperties}): model → {Model1}", model, newsItem, excludeProperties, model);
                model.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, true, false);
            }

            model.StartDateUtc = newsItem.StartDateUtc;
			_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},excludeProperties={ExcludeProperties}): model.StartDateUtc → {Arg3}", model, newsItem, excludeProperties, model.StartDateUtc);
			_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},excludeProperties={ExcludeProperties}): model.EndDateUtc → {Arg3}", model, newsItem, excludeProperties, model.EndDateUtc);
            model.EndDateUtc = newsItem.EndDateUtc;
        }

        //set default values for the new model
        if (newsItem == null)
        {
            model.Published = true;
			_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},excludeProperties={ExcludeProperties}): model.Published → {Arg3}", model, newsItem, excludeProperties, model.Published);
			_logger.LogDebug("PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},excludeProperties={ExcludeProperties}): model.AllowComments → {Arg3}", model, newsItem, excludeProperties, model.AllowComments);
            model.AllowComments = true;
        }

        //prepare available languages
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, newsItem, excludeProperties);

		_logger.LogDebug("<<< Exiting PrepareNewsItemModelAsync(model={Model},newsItem={NewsItem},excludeProperties={ExcludeProperties})", model, newsItem, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare news comment search model
    /// </summary>
    /// <param name="searchModel">News comment search model</param>
    /// <param name="newsItem">News item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comment search model
    /// </returns>
    public virtual async Task<NewsCommentSearchModel> PrepareNewsCommentSearchModelAsync(NewsCommentSearchModel searchModel, NewsItem newsItem)
    {
		_logger.LogDebug(">>> Entering PrepareNewsCommentSearchModelAsync(searchModel={SearchModel},newsItem={NewsItem})", searchModel, newsItem);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.All"),
            Value = "0"
        });
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.ApprovedOnly"),
            Value = "1"
        });
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.News.Comments.List.SearchApproved.DisapprovedOnly"),
            Value = "2"
        });

        searchModel.NewsItemId = newsItem?.Id;
		_logger.LogDebug("PrepareNewsCommentSearchModelAsync(searchModel={SearchModel},newsItem={NewsItem}): searchModel.NewsItemId → {Arg2}", searchModel, newsItem, searchModel.NewsItemId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareNewsCommentSearchModelAsync(searchModel={SearchModel},newsItem={NewsItem})", searchModel, newsItem);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged news comment list model
    /// </summary>
    /// <param name="searchModel">News comment search model</param>
    /// <param name="newsItemId">News item Id; pass null to prepare comment models for all news items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comment list model
    /// </returns>
    public virtual async Task<NewsCommentListModel> PrepareNewsCommentListModelAsync(NewsCommentSearchModel searchModel, int? newsItemId)
    {
		_logger.LogDebug(">>> Entering PrepareNewsCommentListModelAsync(searchModel={SearchModel},newsItemId={NewsItemId})", searchModel, newsItemId);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter comments
        var createdOnFromValue = searchModel.CreatedOnFrom == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdOnToValue = searchModel.CreatedOnTo == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;
		_logger.LogDebug("PrepareNewsCommentListModelAsync(searchModel={SearchModel},newsItemId={NewsItemId}): isApprovedOnly → {IsApprovedOnly}", searchModel, newsItemId, isApprovedOnly);

        //get comments
        var comments = (await _newsService.GetAllCommentsAsync(newsItemId: newsItemId,
            approved: isApprovedOnly,
            fromUtc: createdOnFromValue,
            toUtc: createdOnToValue,
            commentText: searchModel.SearchText)).ToPagedList(searchModel);

        //prepare store names (to avoid loading for each comment)
        var storeNames = (await _storeService.GetAllStoresAsync())
            .ToDictionary(store => store.Id, store => store.Name);

        //prepare list model
        var model = await new NewsCommentListModel().PrepareToGridAsync(searchModel, comments, () =>
        {
            return comments.SelectAwait(async newsComment =>
            {
                //fill in model values from the entity
                var commentModel = newsComment.ToModel<NewsCommentModel>();
				_logger.LogDebug("PrepareNewsCommentListModelAsync(searchModel={SearchModel},newsItemId={NewsItemId}): commentModel → {CommentModel}", searchModel, newsItemId, commentModel);

                //convert dates to the user time
                commentModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                commentModel.NewsItemTitle = (await _newsService.GetNewsByIdAsync(newsComment.NewsItemId))?.Title;

                if ((await _customerService.GetCustomerByIdAsync(newsComment.CustomerId)) is Customer customer)
                {
                    commentModel.CustomerInfo = (await _customerService.IsRegisteredAsync(customer))
                        ? customer.Email
                        : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                }

                commentModel.CommentText = _htmlFormatter.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                commentModel.StoreName = storeNames.TryGetValue(newsComment.StoreId, out var value) ? value : "Deleted";

                return commentModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareNewsCommentListModelAsync(searchModel={SearchModel},newsItemId={NewsItemId})", searchModel, newsItemId);
        return model;
    }

    /// <summary>
    /// Prepare news item search model
    /// </summary>
    /// <param name="searchModel">News item search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news item search model
    /// </returns>
    public virtual async Task<NewsItemSearchModel> PrepareNewsItemSearchModelAsync(NewsItemSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion
}