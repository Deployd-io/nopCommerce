using FluentAssertions;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Services.Blogs;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Web.Factories;
using Nop.Web.Models.Blogs;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class BlogModelFactoryTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IBlogModelFactory _blogModelFactory;
    private IBlogService _blogService;
    private ICustomerService _customerService;
    private IDateTimeHelper _dateTimeHelper;
    private CustomerSettings _customerSettings;
    private CaptchaSettings _captchaSettings;
    private BlogSettings _blogSettings;
    private ISettingService _settingsService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _blogModelFactory = GetService<IBlogModelFactory>();
        _blogService = GetService<IBlogService>();
		_logger.LogDebug("SetUp(): _blogService → {_blogService}", _blogService);
		_logger.LogDebug("SetUp(): _blogModelFactory → {_blogModelFactory}", _blogModelFactory);
        _customerService = GetService<ICustomerService>();
		_logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
        _dateTimeHelper = GetService<IDateTimeHelper>();
		_logger.LogDebug("SetUp(): _dateTimeHelper → {_dateTimeHelper}", _dateTimeHelper);
        _customerSettings = GetService<CustomerSettings>();
		_logger.LogDebug("SetUp(): _customerSettings → {_customerSettings}", _customerSettings);
        _captchaSettings = GetService<CaptchaSettings>();
		_logger.LogDebug("SetUp(): _captchaSettings → {_captchaSettings}", _captchaSettings);
        _blogSettings = GetService<BlogSettings>();
		_logger.LogDebug("SetUp(): _blogSettings → {_blogSettings}", _blogSettings);
        _settingsService = GetService<ISettingService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _settingsService → {_settingsService}", _settingsService);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        _customerSettings.AllowCustomersToUploadAvatars = false;
        _customerSettings.AllowViewingProfiles = false;
        await _settingsService.SaveSettingAsync(_customerSettings);
        _captchaSettings.Enabled = false;
        _captchaSettings.ShowOnBlogCommentPage = false;
        await _settingsService.SaveSettingAsync(_captchaSettings);
        _blogSettings.NumberOfTags = 15;
		_logger.LogDebug("TearDown(): _blogSettings.NumberOfTags → {Arg0}", _blogSettings.NumberOfTags);
		_logger.LogDebug("TearDown(): _customerSettings.AllowViewingProfiles → {Arg0}", _customerSettings.AllowViewingProfiles);
		_logger.LogDebug("TearDown(): _customerSettings.AllowCustomersToUploadAvatars → {Arg0}", _customerSettings.AllowCustomersToUploadAvatars);
        await _settingsService.SaveSettingAsync(_blogSettings);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public void PrepareBlogPostModelShouldRaiseExceptionIfBlogPostModelOrBlogPostIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostModelShouldRaiseExceptionIfBlogPostModelOrBlogPostIsNull()");
        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostModelAsync(null, new BlogPost(), false).Wait());

        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostModelAsync(null, new BlogPost(), true).Wait());

        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null, false).Wait());

        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null, true).Wait());
			_logger.LogDebug("<<< Exiting PrepareBlogPostModelShouldRaiseExceptionIfBlogPostModelOrBlogPostIsNull()");
    }

    [Test]
    public async Task CanPrepareBlogPostModelWithComets()
    {
		_logger.LogDebug(">>> Entering CanPrepareBlogPostModelWithComets()");
        var model = new BlogPostModel();
        var blogPost = await _blogService.GetBlogPostByIdAsync(1);
		_logger.LogDebug("CanPrepareBlogPostModelWithComets(): blogPost → {BlogPost}", blogPost);
		_logger.LogDebug("CanPrepareBlogPostModelWithComets(): model → {Model}", model);

        await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

        model.Id.Should().Be(blogPost.Id);
        model.MetaTitle.Should().Be(blogPost.MetaTitle);
        model.MetaDescription.Should().Be(blogPost.MetaDescription);
        model.MetaKeywords.Should().Be(blogPost.MetaKeywords);
        model.SeName = model.Title.Replace(" ", "-").ToLowerInvariant();
        model.Title.Should().Be(blogPost.Title);
        model.Body.Should().Be(blogPost.Body);
        model.BodyOverview.Should().Be(blogPost.BodyOverview);
        model.AllowComments.Should().Be(blogPost.AllowComments);
        model.CreatedOn.ToUniversalTime().Should().Be(blogPost.CreatedOnUtc);
        model.Tags.Should().BeEquivalentTo(await _blogService.ParseTagsAsync(blogPost));
        model.NumberOfComments.Should().Be(await _blogService.GetBlogCommentsCountAsync(blogPost, 0, true));
        model.Comments.Count.Should().Be(model.NumberOfComments);
		_logger.LogDebug("<<< Exiting CanPrepareBlogPostModelWithComets()");
    }

    [Test]
    public async Task CanPrepareBlogPostModelWithOutComets()
    {
		_logger.LogDebug(">>> Entering CanPrepareBlogPostModelWithOutComets()");
        var model = new BlogPostModel();
        var blogPost = await _blogService.GetBlogPostByIdAsync(1);
		_logger.LogDebug("CanPrepareBlogPostModelWithOutComets(): blogPost → {BlogPost}", blogPost);
		_logger.LogDebug("CanPrepareBlogPostModelWithOutComets(): model → {Model}", model);

        await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, false);

        model.Id.Should().Be(blogPost.Id);
        model.MetaTitle.Should().Be(blogPost.MetaTitle);
        model.MetaDescription.Should().Be(blogPost.MetaDescription);
        model.MetaKeywords.Should().Be(blogPost.MetaKeywords);
        model.SeName = model.Title.Replace(" ", "-").ToLowerInvariant();
        model.Title.Should().Be(blogPost.Title);
        model.Body.Should().Be(blogPost.Body);
        model.BodyOverview.Should().Be(blogPost.BodyOverview);
        model.AllowComments.Should().Be(blogPost.AllowComments);
        model.CreatedOn.ToUniversalTime().Should().Be(blogPost.CreatedOnUtc);
        model.Tags.Should().BeEquivalentTo(await _blogService.ParseTagsAsync(blogPost));
        model.NumberOfComments.Should().Be(await _blogService.GetBlogCommentsCountAsync(blogPost, 0, true));
        model.Comments.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanPrepareBlogPostModelWithOutComets()");
    }

    [Test]
    public async Task PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings()");
        var model = new BlogPostModel();
        var blogPost = await _blogService.GetBlogPostByIdAsync(1);
		_logger.LogDebug("PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings(): blogPost → {BlogPost}", blogPost);
		_logger.LogDebug("PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings(): model → {Model}", model);
        await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, false);

        model.AddNewComment.DisplayCaptcha.Should().BeFalse();

        _captchaSettings.Enabled = _captchaSettings.ShowOnBlogCommentPage = true;
		_logger.LogDebug("PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings(): _captchaSettings.Enabled → {Arg0}", _captchaSettings.Enabled);
        await _settingsService.SaveSettingAsync(_captchaSettings);

        await GetService<IBlogModelFactory>().PrepareBlogPostModelAsync(model, blogPost, false);
        _captchaSettings.Enabled = _captchaSettings.ShowOnBlogCommentPage = false;
        await _settingsService.SaveSettingAsync(_captchaSettings);

        model.AddNewComment.DisplayCaptcha.Should().BeTrue();
		_logger.LogDebug("<<< Exiting PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings()");
    }

    [Test]
    public async Task CanPrepareBlogPostListModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareBlogPostListModel()");
        var model = await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel());

		_logger.LogDebug("CanPrepareBlogPostListModel(): model → {Model}", model);
        model.PagingFilteringContext.FirstItem.Should().Be(1);
        model.PagingFilteringContext.HasNextPage.Should().BeFalse();
        model.PagingFilteringContext.HasPreviousPage.Should().BeFalse();
        model.PagingFilteringContext.LastItem.Should().Be(2);
        model.PagingFilteringContext.PageNumber.Should().Be(1);
        model.PagingFilteringContext.PageSize.Should().Be(10);
        model.PagingFilteringContext.TotalItems.Should().Be(2);
        model.PagingFilteringContext.TotalPages.Should().Be(1);
        model.PagingFilteringContext.Tag.Should().BeNull();
        model.PagingFilteringContext.Month.Should().BeNull();

        model.BlogPosts.Should().NotBeNull();
        model.BlogPosts.Count.Should().Be(2);

        var date = DateTime.Now.ToUniversalTime();
		_logger.LogDebug("CanPrepareBlogPostListModel(): date → {Date}", date);

        model = await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel
        {
            Month = $"{date.Year}-{date.Month}"
        });

        model.PagingFilteringContext.Month.Should().NotBeNullOrEmpty();
        model.PagingFilteringContext.Tag.Should().BeNull();
        model.BlogPosts.Should().NotBeNull();
        model.BlogPosts.Count.Should().Be(2);

        model = await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel
        {
            Month = $"{date.Year}-{date.Month}",
            Tag = "nopCommerce"
        });

        model.PagingFilteringContext.Month.Should().NotBeNullOrEmpty();
        model.PagingFilteringContext.Tag.Should().NotBeNullOrEmpty();
        model.BlogPosts.Should().NotBeNull();
        model.BlogPosts.Count.Should().Be(1);
		_logger.LogDebug("<<< Exiting CanPrepareBlogPostListModel()");
    }

    [Test]
    public void PrepareBlogPostListModelShouldRaiseExceptionIfCommandIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostListModelShouldRaiseExceptionIfCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostListModelAsync(null).Wait());
			_logger.LogDebug("<<< Exiting PrepareBlogPostListModelShouldRaiseExceptionIfCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareBlogPostTagListModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareBlogPostTagListModel()");
        var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
        model.Tags.Count.Should().Be(5);
		_logger.LogDebug("<<< Exiting CanPrepareBlogPostTagListModel()");
		_logger.LogDebug("CanPrepareBlogPostTagListModel(): model → {Model}", model);
    }

    [Test]
    public async Task PrepareBlogPostTagListModelCountShouldDependOnSettings()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostTagListModelCountShouldDependOnSettings()");
        var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
        model.Tags.Count.Should().Be(5);
		_logger.LogDebug("PrepareBlogPostTagListModelCountShouldDependOnSettings(): model → {Model}", model);

        _blogSettings.NumberOfTags = 1;
		_logger.LogDebug("PrepareBlogPostTagListModelCountShouldDependOnSettings(): _blogSettings.NumberOfTags → {Arg0}", _blogSettings.NumberOfTags);
        await _settingsService.SaveSettingAsync(_blogSettings);

        model = await GetService<IBlogModelFactory>().PrepareBlogPostTagListModelAsync();

        _blogSettings.NumberOfTags = 15;
        await _settingsService.SaveSettingAsync(_blogSettings);

        model.Tags.Count.Should().Be(1);
		_logger.LogDebug("<<< Exiting PrepareBlogPostTagListModelCountShouldDependOnSettings()");
    }

    [Test]
    public async Task CanPrepareBlogPostYearModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareBlogPostYearModel()");
        var model = await _blogModelFactory.PrepareBlogPostYearModelAsync();
        var date = DateTime.Now.ToUniversalTime();
		_logger.LogDebug("CanPrepareBlogPostYearModel(): date → {Date}", date);
		_logger.LogDebug("CanPrepareBlogPostYearModel(): model → {Model}", model);

        model.Count.Should().Be(1);
        model[0].Year.Should().Be(date.Year);
        var months = model[0].Months;

        months.Count.Should().Be(1);

        var month = months.First();
		_logger.LogDebug("CanPrepareBlogPostYearModel(): month → {Month}", month);

        month.BlogPostCount.Should().Be(2);
        month.Month.Should().Be(date.Month);
		_logger.LogDebug("<<< Exiting CanPrepareBlogPostYearModel()");
    }

    [Test]
    public void PrepareBlogPostCommentModelShouldRaiseExceptionIfBlogCommentIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostCommentModelShouldRaiseExceptionIfBlogCommentIsNull()");
        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostCommentModelAsync(null).Wait());
			_logger.LogDebug("<<< Exiting PrepareBlogPostCommentModelShouldRaiseExceptionIfBlogCommentIsNull()");
    }

    [Test]
    public async Task PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue()");
        var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
        _customerSettings.AllowCustomersToUploadAvatars = true;
		_logger.LogDebug("PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue(): _customerSettings.AllowCustomersToUploadAvatars → {Arg0}", _customerSettings.AllowCustomersToUploadAvatars);
		_logger.LogDebug("PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue(): blogComment → {BlogComment}", blogComment);
        await _settingsService.SaveSettingAsync(_customerSettings);
        var blogModelFactory = GetService<IBlogModelFactory>();
		_logger.LogDebug("PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue(): blogModelFactory → {BlogModelFactory}", blogModelFactory);
        var model = await blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
		_logger.LogDebug("PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue(): model → {Model}", model);
        _customerSettings.AllowCustomersToUploadAvatars = false;
        await _settingsService.SaveSettingAsync(_customerSettings);

        model.CustomerAvatarUrl.Should().NotBeNullOrEmpty();
        model.CustomerAvatarUrl.Should()
            .Be($"http://{NopTestsDefaults.HostIpAddress}/images/thumbs/default-avatar_{GetService<MediaSettings>().AvatarPictureSize}.jpg");
			_logger.LogDebug("<<< Exiting PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue()");
    }

    [Test]
    public async Task PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings()
    {
		_logger.LogDebug(">>> Entering PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings()");
        var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
        var model = await _blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
		_logger.LogDebug("PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings(): model → {Model}", model);
		_logger.LogDebug("PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings(): blogComment → {BlogComment}", blogComment);
		_logger.LogDebug("PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings(): _customerSettings.AllowViewingProfiles → {Arg0}", _customerSettings.AllowViewingProfiles);
        model.AllowViewingProfiles.Should().BeFalse();
        _customerSettings.AllowViewingProfiles = true;
        await _settingsService.SaveSettingAsync(_customerSettings);
        var blogModelFactory = GetService<IBlogModelFactory>();
		_logger.LogDebug("PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings(): blogModelFactory → {BlogModelFactory}", blogModelFactory);
        model = await blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
        _customerSettings.AllowViewingProfiles = false;
        await _settingsService.SaveSettingAsync(_customerSettings);
        model.AllowViewingProfiles.Should().BeTrue();
		_logger.LogDebug("<<< Exiting PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings()");
    }

    [Test]
    public async Task CanPrepareBlogPostComment()
    {
		_logger.LogDebug(">>> Entering CanPrepareBlogPostComment()");
        var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
        var model = await _blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
		_logger.LogDebug("CanPrepareBlogPostComment(): model → {Model}", model);
		_logger.LogDebug("CanPrepareBlogPostComment(): blogComment → {BlogComment}", blogComment);

        var customer = await _customerService.GetCustomerByIdAsync(1);
		_logger.LogDebug("CanPrepareBlogPostComment(): customer → {Customer}", customer);

        model.Id.Should().Be(blogComment.Id);
        model.CustomerId.Should().Be(blogComment.CustomerId);
        model.CustomerName.Should().Be(await _customerService.FormatUsernameAsync(customer));
        model.CommentText.Should().Be(blogComment.CommentText);
        model.CreatedOn.Should().Be(await _dateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc));
        model.AllowViewingProfiles.Should().BeFalse();
        model.CustomerAvatarUrl.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanPrepareBlogPostComment()");
    }
}