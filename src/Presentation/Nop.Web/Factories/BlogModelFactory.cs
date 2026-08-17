using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Services.Blogs;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Blogs;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the blog model factory
/// </summary>
public partial class BlogModelFactory : IBlogModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly BlogSettings _blogSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly IBlogService _blogService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IPictureService _pictureService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public BlogModelFactory(BlogSettings blogSettings,
        CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        IBlogService blogService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        IPictureService pictureService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings)
    {
		_logger.LogDebug(">>> Entering BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings})", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings);
        _blogSettings = blogSettings;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _blogSettings → {_blogSettings}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _blogSettings);
        _captchaSettings = captchaSettings;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _captchaSettings → {_captchaSettings}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _captchaSettings);
        _customerSettings = customerSettings;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _customerSettings → {_customerSettings}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _customerSettings);
        _blogService = blogService;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _blogService → {_blogService}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _blogService);
        _customerService = customerService;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _customerService → {_customerService}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _dateTimeHelper → {_dateTimeHelper}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _dateTimeHelper);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _genericAttributeService → {_genericAttributeService}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _genericAttributeService);
        _pictureService = pictureService;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _pictureService → {_pictureService}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _pictureService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _staticCacheManager → {_staticCacheManager}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _staticCacheManager);
        _storeContext = storeContext;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _storeContext → {_storeContext}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _storeContext);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _urlRecordService → {_urlRecordService}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _urlRecordService);
        _workContext = workContext;
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _workContext → {_workContext}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _workContext);
        _mediaSettings = mediaSettings;
		_logger.LogDebug("<<< Exiting BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings})", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings);
		_logger.LogDebug("BlogModelFactory(blogSettings={BlogSettings},captchaSettings={CaptchaSettings},customerSettings={CustomerSettings},blogService={BlogService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},genericAttributeService={GenericAttributeService},pictureService={PictureService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext},mediaSettings={MediaSettings}): _mediaSettings → {_mediaSettings}", blogSettings, captchaSettings, customerSettings, blogService, customerService, dateTimeHelper, genericAttributeService, pictureService, staticCacheManager, storeContext, urlRecordService, workContext, mediaSettings, _mediaSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare blog post model
    /// </summary>
    /// <param name="model">Blog post model</param>
    /// <param name="blogPost">Blog post entity</param>
    /// <param name="prepareComments">Whether to prepare blog comments</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool prepareComments)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(blogPost);

        model.Id = blogPost.Id;
        model.MetaTitle = blogPost.MetaTitle;
        model.MetaDescription = blogPost.MetaDescription;
        model.MetaKeywords = blogPost.MetaKeywords;
        model.SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
        model.Title = blogPost.Title;
        model.Body = blogPost.Body;
        model.BodyOverview = blogPost.BodyOverview;
        model.AllowComments = blogPost.AllowComments;

        model.PreventNotRegisteredUsersToLeaveComments =
            await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) &&
            !_blogSettings.AllowNotRegisteredUsersToLeaveComments;

        model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogPost.StartDateUtc ?? blogPost.CreatedOnUtc, DateTimeKind.Utc);
        model.Tags = await _blogService.ParseTagsAsync(blogPost);
        model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage;

        //number of blog comments
        var store = await _storeContext.GetCurrentStoreAsync();
        var storeId = _blogSettings.ShowBlogCommentsPerStore ? store.Id : 0;

        model.NumberOfComments = await _blogService.GetBlogCommentsCountAsync(blogPost, storeId, true);

        if (prepareComments)
        {
            var blogComments = await _blogService.GetAllCommentsAsync(
                blogPostId: blogPost.Id,
                approved: true,
                storeId: storeId);

            foreach (var bc in blogComments)
            {
                var commentModel = await PrepareBlogPostCommentModelAsync(bc);
                model.Comments.Add(commentModel);
            }
        }
    }

    /// <summary>
    /// Prepare blog post list model
    /// </summary>
    /// <param name="command">Blog paging filtering model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post list model
    /// </returns>
    public virtual async Task<BlogPostListModel> PrepareBlogPostListModelAsync(BlogPagingFilteringModel command)
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostListModelAsync(command={Command})", command);
        ArgumentNullException.ThrowIfNull(command);

        if (command.PageSize <= 0)
            command.PageSize = _blogSettings.PostsPageSize;
			_logger.LogDebug("PrepareBlogPostListModelAsync(command={Command}): command.PageSize → {Arg1}", command, command.PageSize);
        if (command.PageNumber <= 0)
            command.PageNumber = 1;
			_logger.LogDebug("PrepareBlogPostListModelAsync(command={Command}): command.PageNumber → {Arg1}", command, command.PageNumber);

        var dateFrom = command.GetFromMonth();
		_logger.LogDebug("PrepareBlogPostListModelAsync(command={Command}): dateFrom → {DateFrom}", command, dateFrom);
        var dateTo = command.GetToMonth();
		_logger.LogDebug("PrepareBlogPostListModelAsync(command={Command}): dateTo → {DateTo}", command, dateTo);

        var language = await _workContext.GetWorkingLanguageAsync();
		_logger.LogDebug("PrepareBlogPostListModelAsync(command={Command}): language → {Language}", command, language);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareBlogPostListModelAsync(command={Command}): store → {Store}", command, store);
        var blogPosts = string.IsNullOrEmpty(command.Tag)
            ? await _blogService.GetAllBlogPostsAsync(store.Id, language.Id, dateFrom, dateTo, command.PageNumber - 1, command.PageSize)
            : await _blogService.GetAllBlogPostsByTagAsync(store.Id, language.Id, command.Tag, command.PageNumber - 1, command.PageSize);

        var model = new BlogPostListModel
        {
            PagingFilteringContext = { Tag = command.Tag, Month = command.Month },
            WorkingLanguageId = language.Id,
            BlogPosts = await blogPosts.SelectAwait(async blogPost =>
            {
                var blogPostModel = new BlogPostModel();
                await PrepareBlogPostModelAsync(blogPostModel, blogPost, false);
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
                return blogPostModel;
            }).ToListAsync()
        };
		_logger.LogInformation("PrepareBlogPostListModelAsync(command)={Command}: database query executed in {Arg1} ms", command, __logStart.ElapsedMilliseconds);
        model.PagingFilteringContext.LoadPagedList(blogPosts);

		_logger.LogDebug("<<< Exiting PrepareBlogPostListModelAsync(command={Command})", command);
		_logger.LogDebug(">>> Entering PrepareBlogPostCommentModelAsync(blogComment={BlogComment})", blogComment);
		_logger.LogDebug("<<< Exiting PrepareBlogPostCommentModelAsync(blogComment={BlogComment})", blogComment);
        return model;
    }

    /// <summary>
    /// Prepare blog post tag list model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post tag list model
    /// </returns>
    public virtual async Task<BlogPostTagListModel> PrepareBlogPostTagListModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostTagListModelAsync()");
        var model = new BlogPostTagListModel();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareBlogPostTagListModelAsync(): store → {Store}", store);
		_logger.LogDebug("PrepareBlogPostTagListModelAsync(): model → {Model}", model);

        //get tags
        var tags = (await _blogService
                .GetAllBlogPostTagsAsync(store.Id, (await _workContext.GetWorkingLanguageAsync()).Id))
            .OrderByDescending(x => x.BlogPostCount)
            .Take(_blogSettings.NumberOfTags);

        //sorting and setting into the model
        model.Tags.AddRange(tags.OrderBy(x => x.Name).Select(tag => new BlogPostTagModel
        {
            Name = tag.Name,
            BlogPostCount = tag.BlogPostCount
        }));

		_logger.LogDebug("<<< Exiting PrepareBlogPostTagListModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare blog post year models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of blog post year model
    /// </returns>
    public virtual async Task<List<BlogPostYearModel>> PrepareBlogPostYearModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostYearModelAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
		_logger.LogDebug("PrepareBlogPostYearModelAsync(): store → {Store}", store);
		_logger.LogDebug("PrepareBlogPostYearModelAsync(): currentLanguage → {CurrentLanguage}", currentLanguage);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.BlogMonthsModelKey, currentLanguage, store);
        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
		_logger.LogDebug("PrepareBlogPostYearModelAsync(): cacheKey → {CacheKey}", cacheKey);
            var model = new List<BlogPostYearModel>();
			_logger.LogDebug("PrepareBlogPostYearModelAsync(): model → {Model}", model);

            var blogPosts = await _blogService.GetAllBlogPostsAsync(store.Id,
                currentLanguage.Id);
            if (blogPosts.Any())
            {
                var months = new SortedDictionary<DateTime, int>();
				_logger.LogDebug("PrepareBlogPostYearModelAsync(): months → {Months}", months);

                var blogPost = blogPosts[blogPosts.Count - 1];
                var first = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc;
				_logger.LogDebug("PrepareBlogPostYearModelAsync(): first → {First}", first);
                while (DateTime.SpecifyKind(first, DateTimeKind.Utc) <= DateTime.UtcNow.AddMonths(1))
                {
                    var list = await _blogService.GetPostsByDateAsync(blogPosts, new DateTime(first.Year, first.Month, 1),
                        new DateTime(first.Year, first.Month, 1).AddMonths(1).AddSeconds(-1));
                    if (list.Any())
                    {
                        var date = new DateTime(first.Year, first.Month, 1);
                        months.Add(date, list.Count);
                    }

                    first = first.AddMonths(1);
                }

                var current = 0;
				_logger.LogDebug("PrepareBlogPostYearModelAsync(): current → {Current}", current);
                foreach (var kvp in months)
                {
                    var date = kvp.Key;
                    var blogPostCount = kvp.Value;
                    if (current == 0)
                        current = date.Year;

                    if (date.Year > current || !model.Any())
                    {
                        var yearModel = new BlogPostYearModel
                        {
                            Year = date.Year
                        };
                        model.Insert(0, yearModel);
                    }

                    model.First().Months.Insert(0, new BlogPostMonthModel
                    {
                        Month = date.Month,
                        BlogPostCount = blogPostCount
                    });

                    current = date.Year;
                }
            }

            return model;
        });

		_logger.LogInformation("PrepareBlogPostYearModelAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareBlogPostYearModelAsync()");
        return cachedModel;
    }

    /// <summary>
    /// Prepare blog comment model
    /// </summary>
    /// <param name="blogComment">Blog comment entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog comment model
    /// </returns>
    public virtual async Task<BlogCommentModel> PrepareBlogPostCommentModelAsync(BlogComment blogComment)
    {
        ArgumentNullException.ThrowIfNull(blogComment);

        var customer = await _customerService.GetCustomerByIdAsync(blogComment.CustomerId);
		_logger.LogDebug("PrepareBlogPostCommentModelAsync(blogComment={BlogComment}): customer → {Customer}", blogComment, customer);

        var model = new BlogCommentModel
        {
            Id = blogComment.Id,
            CustomerId = blogComment.CustomerId,
            CustomerName = await _customerService.FormatUsernameAsync(customer),
            CommentText = blogComment.CommentText,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc),
            AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !await _customerService.IsGuestAsync(customer)
        };

        if (_customerSettings.AllowCustomersToUploadAvatars)
        {
            model.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
        }

        return model;
    }

    #endregion
}