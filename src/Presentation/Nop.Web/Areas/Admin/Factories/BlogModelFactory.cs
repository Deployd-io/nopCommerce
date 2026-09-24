using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Blogs;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the blog model factory implementation
/// </summary>
public partial class BlogModelFactory : IBlogModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly IBlogService _blogService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly IStoreService _storeService;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public BlogModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        IBlogService blogService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IHtmlFormatter htmlFormatter,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        IStoreService storeService,
        IUrlRecordService urlRecordService)
    {
		_logger.LogError(">>> Entering BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService})", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService);
        _catalogSettings = catalogSettings;
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _baseAdminModelFactory → {_baseAdminModelFactory}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _baseAdminModelFactory);
        _blogService = blogService;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _blogService → {_blogService}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _blogService);
        _customerService = customerService;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _customerService → {_customerService}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _dateTimeHelper → {_dateTimeHelper}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _dateTimeHelper);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _htmlFormatter → {_htmlFormatter}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _htmlFormatter);
        _languageService = languageService;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _languageService → {_languageService}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _localizationService → {_localizationService}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _localizationService);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _storeMappingSupportedModelFactory → {_storeMappingSupportedModelFactory}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _storeMappingSupportedModelFactory);
        _storeService = storeService;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _storeService → {_storeService}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _storeService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("BlogModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},languageService={LanguageService},localizationService={LocalizationService},storeMappingSupportedModelFactory=<redacted>,storeService={StoreService},urlRecordService={UrlRecordService}): _urlRecordService → {_urlRecordService}", catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeService, urlRecordService, _urlRecordService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare blog content model
    /// </summary>
    /// <param name="blogContentModel">Blog content model</param>
    /// <param name="filterByBlogPostId">Blog post ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog content model
    /// </returns>
    public virtual async Task<BlogContentModel> PrepareBlogContentModelAsync(BlogContentModel blogContentModel, int? filterByBlogPostId)
    {
		_logger.LogError(">>> Entering PrepareBlogContentModelAsync(blogContentModel={BlogContentModel},filterByBlogPostId={FilterByBlogPostId})", blogContentModel, filterByBlogPostId);
        ArgumentNullException.ThrowIfNull(blogContentModel);

        //prepare nested search models
        await PrepareBlogPostSearchModelAsync(blogContentModel.BlogPosts);
        var blogPost = await _blogService.GetBlogPostByIdAsync(filterByBlogPostId ?? 0);
		_logger.LogError("PrepareBlogContentModelAsync(blogContentModel={BlogContentModel},filterByBlogPostId={FilterByBlogPostId}): blogPost → {BlogPost}", blogContentModel, filterByBlogPostId, blogPost);
        await PrepareBlogCommentSearchModelAsync(blogContentModel.BlogComments, blogPost);

		_logger.LogWarning("<<< Exiting PrepareBlogContentModelAsync(blogContentModel={BlogContentModel},filterByBlogPostId={FilterByBlogPostId})", blogContentModel, filterByBlogPostId);
        return blogContentModel;
    }

    /// <summary>
    /// Prepare paged blog post list model
    /// </summary>
    /// <param name="searchModel">Blog post search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post list model
    /// </returns>
    public virtual async Task<BlogPostListModel> PrepareBlogPostListModelAsync(BlogPostSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareBlogPostListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get blog posts
        var blogPosts = await _blogService.GetAllBlogPostsAsync(storeId: searchModel.SearchStoreId, showHidden: true,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, title: searchModel.SearchTitle);

        //prepare list model
        var model = await new BlogPostListModel().PrepareToGridAsync(searchModel, blogPosts, () =>
        {
            return blogPosts.SelectAwait(async blogPost =>
            {
                //fill in model values from the entity
                var blogPostModel = blogPost.ToModel<BlogPostModel>();
				_logger.LogError("PrepareBlogPostListModelAsync(searchModel={SearchModel}): blogPostModel → {BlogPostModel}", searchModel, blogPostModel);

                //little performance optimization: ensure that "Body" is not returned
                blogPostModel.Body = string.Empty;
				_logger.LogDebug("PrepareBlogPostListModelAsync(searchModel={SearchModel}): blogPostModel.Body → {Arg1}", searchModel, blogPostModel.Body);

                //convert dates to the user time
                if (blogPost.StartDateUtc.HasValue)
                    blogPostModel.StartDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(blogPost.StartDateUtc.Value, DateTimeKind.Utc);
                if (blogPost.EndDateUtc.HasValue)
                    blogPostModel.EndDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(blogPost.EndDateUtc.Value, DateTimeKind.Utc);
                blogPostModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogPost.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                blogPostModel.LanguageName = (await _languageService.GetLanguageByIdAsync(blogPost.LanguageId))?.Name;
                blogPostModel.ApprovedComments = await _blogService.GetBlogCommentsCountAsync(blogPost, isApproved: true);
                blogPostModel.NotApprovedComments = await _blogService.GetBlogCommentsCountAsync(blogPost, isApproved: false);
                blogPostModel.SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, true, false);

                return blogPostModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareBlogPostListModelAsync(searchModel={SearchModel})", searchModel);
		_logger.LogError(">>> Entering PrepareBlogPostSearchModelAsync(searchModel={SearchModel})", searchModel);
		_logger.LogWarning("<<< Exiting PrepareBlogPostSearchModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare blog post model
    /// </summary>
    /// <param name="model">Blog post model</param>
    /// <param name="blogPost">Blog post</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post model
    /// </returns>
    public virtual async Task<BlogPostModel> PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties})", model, blogPost, excludeProperties);
        //fill in model values from the entity
        if (blogPost != null)
        {
            if (model == null)
            {
                model = blogPost.ToModel<BlogPostModel>();
				_logger.LogError("PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties}): model → {Model1}", model, blogPost, excludeProperties, model);
                model.SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, true, false);
            }
            model.StartDateUtc = blogPost.StartDateUtc;
			_logger.LogDebug("PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties}): model.StartDateUtc → {Arg3}", model, blogPost, excludeProperties, model.StartDateUtc);
			_logger.LogDebug("PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties}): model.EndDateUtc → {Arg3}", model, blogPost, excludeProperties, model.EndDateUtc);
            model.EndDateUtc = blogPost.EndDateUtc;
        }

        //set default values for the new model
        if (blogPost == null)
        {
            model.AllowComments = true;
			_logger.LogDebug("PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties}): model.AllowComments → {Arg3}", model, blogPost, excludeProperties, model.AllowComments);
			_logger.LogDebug("PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties}): model.IncludeInSitemap → {Arg3}", model, blogPost, excludeProperties, model.IncludeInSitemap);
            model.IncludeInSitemap = true;
        }

        var blogTags = await _blogService.GetAllBlogPostTagsAsync(0, 0, true);
		_logger.LogError("PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties}): blogTags → {BlogTags}", model, blogPost, excludeProperties, blogTags);
        var blogTagsSb = new StringBuilder();
		_logger.LogError("PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties}): blogTagsSb → {BlogTagsSb}", model, blogPost, excludeProperties, blogTagsSb);
        blogTagsSb.Append("var initialBlogTags = [");
		_logger.LogError("PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties}): initialBlogTags → {InitialBlogTags}", model, blogPost, excludeProperties, initialBlogTags);
        for (var i = 0; i < blogTags.Count; i++)
        {
            var tag = blogTags[i];
            blogTagsSb.Append('\'');
            blogTagsSb.Append(JavaScriptEncoder.Default.Encode(tag.Name));
            blogTagsSb.Append('\'');
            if (i != blogTags.Count - 1)
                blogTagsSb.Append(',');
        }
        blogTagsSb.Append(']');

        model.InitialBlogTags = blogTagsSb.ToString();

        //prepare available languages
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, blogPost, excludeProperties);

		_logger.LogWarning("<<< Exiting PrepareBlogPostModelAsync(model={Model},blogPost={BlogPost},excludeProperties={ExcludeProperties})", model, blogPost, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare blog comment search model
    /// </summary>
    /// <param name="searchModel">Blog comment search model</param>
    /// <param name="blogPost">Blog post</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog comment search model
    /// </returns>
    public virtual async Task<BlogCommentSearchModel> PrepareBlogCommentSearchModelAsync(BlogCommentSearchModel searchModel, BlogPost blogPost)
    {
		_logger.LogError(">>> Entering PrepareBlogCommentSearchModelAsync(searchModel={SearchModel},blogPost={BlogPost})", searchModel, blogPost);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.Comments.List.SearchApproved.All"),
            Value = "0"
			_logger.LogDebug("PrepareBlogCommentSearchModelAsync(searchModel={SearchModel},blogPost={BlogPost}): Value → {Value}", searchModel, blogPost, Value);
        });
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.Comments.List.SearchApproved.ApprovedOnly"),
            Value = "1"
        });
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.Comments.List.SearchApproved.DisapprovedOnly"),
            Value = "2"
        });

        searchModel.BlogPostId = blogPost?.Id;
		_logger.LogDebug("PrepareBlogCommentSearchModelAsync(searchModel={SearchModel},blogPost={BlogPost}): searchModel.BlogPostId → {Arg2}", searchModel, blogPost, searchModel.BlogPostId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareBlogCommentSearchModelAsync(searchModel={SearchModel},blogPost={BlogPost})", searchModel, blogPost);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged blog comment list model
    /// </summary>
    /// <param name="searchModel">Blog comment search model</param>
    /// <param name="blogPostId">Blog post ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog comment list model
    /// </returns>
    public virtual async Task<BlogCommentListModel> PrepareBlogCommentListModelAsync(BlogCommentSearchModel searchModel, int? blogPostId)
    {
		_logger.LogError(">>> Entering PrepareBlogCommentListModelAsync(searchModel={SearchModel},blogPostId={BlogPostId})", searchModel, blogPostId);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter comments
        var createdOnFromValue = searchModel.CreatedOnFrom == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdOnToValue = searchModel.CreatedOnTo == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;
		_logger.LogError("PrepareBlogCommentListModelAsync(searchModel={SearchModel},blogPostId={BlogPostId}): isApprovedOnly → {IsApprovedOnly}", searchModel, blogPostId, isApprovedOnly);

        //get comments
        var comments = (await _blogService.GetAllCommentsAsync(blogPostId: blogPostId,
            approved: isApprovedOnly,
            fromUtc: createdOnFromValue,
            toUtc: createdOnToValue,
            commentText: searchModel.SearchText)).ToPagedList(searchModel);

        //prepare store names (to avoid loading for each comment)
        var storeNames = (await _storeService.GetAllStoresAsync())
            .ToDictionary(store => store.Id, store => store.Name);

        //prepare list model
        var model = await new BlogCommentListModel().PrepareToGridAsync(searchModel, comments, () =>
        {
            return comments.SelectAwait(async blogComment =>
            {
                //fill in model values from the entity
                var commentModel = blogComment.ToModel<BlogCommentModel>();
				_logger.LogError("PrepareBlogCommentListModelAsync(searchModel={SearchModel},blogPostId={BlogPostId}): commentModel → {CommentModel}", searchModel, blogPostId, commentModel);

                //set title from linked blog post
                commentModel.BlogPostTitle = (await _blogService.GetBlogPostByIdAsync(blogComment.BlogPostId))?.Title;

                if ((await _customerService.GetCustomerByIdAsync(blogComment.CustomerId)) is Customer customer)
                {
                    commentModel.CustomerInfo = (await _customerService.IsRegisteredAsync(customer))
                        ? customer.Email
                        : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                }
                //fill in additional values (not existing in the entity)
                commentModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc);
                commentModel.Comment = _htmlFormatter.FormatText(blogComment.CommentText, false, true, false, false, false, false);
                commentModel.StoreName = storeNames.TryGetValue(blogComment.StoreId, out var value) ? value : "Deleted";

                return commentModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareBlogCommentListModelAsync(searchModel={SearchModel},blogPostId={BlogPostId})", searchModel, blogPostId);
        return model;
    }

    /// <summary>
    /// Prepare blog post search model
    /// </summary>
    /// <param name="searchModel">Blog post search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post search model
    /// </returns>
    public virtual async Task<BlogPostSearchModel> PrepareBlogPostSearchModelAsync(BlogPostSearchModel searchModel)
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