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

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class BlogModelFactoryTests : BaseNopTest
{
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
		logger.LogDebug(">>> Entering SetUp()");
        _blogModelFactory = GetService<IBlogModelFactory>();
        _blogService = GetService<IBlogService>();
		logger.LogDebug("SetUp(): _blogModelFactory → {_blogModelFactory}", _blogModelFactory);
		logger.LogDebug("SetUp(): _blogService → {_blogService}", _blogService);
        _customerService = GetService<ICustomerService>();
        _dateTimeHelper = GetService<IDateTimeHelper>();
		logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
		logger.LogDebug("SetUp(): _dateTimeHelper → {_dateTimeHelper}", _dateTimeHelper);
        _customerSettings = GetService<CustomerSettings>();
        _captchaSettings = GetService<CaptchaSettings>();
		logger.LogDebug("SetUp(): _customerSettings → {_customerSettings}", _customerSettings);
		logger.LogDebug("SetUp(): _captchaSettings → {_captchaSettings}", _captchaSettings);
        _blogSettings = GetService<BlogSettings>();
        _settingsService = GetService<ISettingService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _blogSettings → {_blogSettings}", _blogSettings);
		logger.LogDebug("SetUp(): _settingsService → {_settingsService}", _settingsService);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        _customerSettings.AllowCustomersToUploadAvatars = false;
        _customerSettings.AllowViewingProfiles = false;
        await _settingsService.SaveSettingAsync(_customerSettings);
        _captchaSettings.Enabled = false;
        _captchaSettings.ShowOnBlogCommentPage = false;
        await _settingsService.SaveSettingAsync(_captchaSettings);
        _blogSettings.NumberOfTags = 15;
		logger.LogDebug("TearDown(): _blogSettings.NumberOfTags → {Arg0}", _blogSettings.NumberOfTags);
		logger.LogDebug("TearDown(): _captchaSettings.ShowOnBlogCommentPage → {Arg0}", _captchaSettings.ShowOnBlogCommentPage);
		logger.LogDebug("TearDown(): _customerSettings.AllowViewingProfiles → {Arg0}", _customerSettings.AllowViewingProfiles);
		logger.LogDebug("TearDown(): _customerSettings.AllowCustomersToUploadAvatars → {Arg0}", _customerSettings.AllowCustomersToUploadAvatars);
		logger.LogDebug("TearDown(): _captchaSettings.Enabled → {Arg0}", _captchaSettings.Enabled);
        await _settingsService.SaveSettingAsync(_blogSettings);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public void PrepareBlogPostModelShouldRaiseExceptionIfBlogPostModelOrBlogPostIsNull()
    {
		logger.LogDebug(">>> Entering PrepareBlogPostModelShouldRaiseExceptionIfBlogPostModelOrBlogPostIsNull()");
        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostModelAsync(null, new BlogPost(), false).Wait());

        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostModelAsync(null, new BlogPost(), true).Wait());

        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null, false).Wait());

        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null, true).Wait());
			logger.LogDebug("<<< Exiting PrepareBlogPostModelShouldRaiseExceptionIfBlogPostModelOrBlogPostIsNull()");
    }

    [Test]
    public async Task CanPrepareBlogPostModelWithComets()
    {
		logger.LogDebug(">>> Entering CanPrepareBlogPostModelWithComets()");
        var model = new BlogPostModel();
        var blogPost = await _blogService.GetBlogPostByIdAsync(1);
		logger.LogDebug("CanPrepareBlogPostModelWithComets(): model → {Model}", model);
		logger.LogDebug("CanPrepareBlogPostModelWithComets(): blogPost → {BlogPost}", blogPost);

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
		logger.LogDebug("<<< Exiting CanPrepareBlogPostModelWithComets()");
    }

    [Test]
    public async Task CanPrepareBlogPostModelWithOutComets()
    {
		logger.LogDebug(">>> Entering CanPrepareBlogPostModelWithOutComets()");
        var model = new BlogPostModel();
        var blogPost = await _blogService.GetBlogPostByIdAsync(1);
		logger.LogDebug("CanPrepareBlogPostModelWithOutComets(): model → {Model}", model);
		logger.LogDebug("CanPrepareBlogPostModelWithOutComets(): blogPost → {BlogPost}", blogPost);

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
		logger.LogDebug("<<< Exiting CanPrepareBlogPostModelWithOutComets()");
    }

    [Test]
    public async Task PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings()
    {
		logger.LogDebug(">>> Entering PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings()");
        var model = new BlogPostModel();
        var blogPost = await _blogService.GetBlogPostByIdAsync(1);
		logger.LogDebug("PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings(): model → {Model}", model);
		logger.LogDebug("PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings(): blogPost → {BlogPost}", blogPost);
        await _blogModelFactory.PrepareBlogPostModelAsync(model, blogPost, false);

        model.AddNewComment.DisplayCaptcha.Should().BeFalse();

        _captchaSettings.Enabled = _captchaSettings.ShowOnBlogCommentPage = true;
		logger.LogDebug("PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings(): _captchaSettings.Enabled → {Arg0}", _captchaSettings.Enabled);
        await _settingsService.SaveSettingAsync(_captchaSettings);

        await GetService<IBlogModelFactory>().PrepareBlogPostModelAsync(model, blogPost, false);
        _captchaSettings.Enabled = _captchaSettings.ShowOnBlogCommentPage = false;
        await _settingsService.SaveSettingAsync(_captchaSettings);

        model.AddNewComment.DisplayCaptcha.Should().BeTrue();
		logger.LogDebug("<<< Exiting PrepareBlogPostModelDisplayCaptchaShouldBeDependOnSettings()");
    }

    [Test]
    public async Task CanPrepareBlogPostListModel()
    {
		logger.LogDebug(">>> Entering CanPrepareBlogPostListModel()");
        var model = await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel());

		logger.LogDebug("CanPrepareBlogPostListModel(): model → {Model}", model);
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

		logger.LogDebug("CanPrepareBlogPostListModel(): date → {Date}", date);
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
		logger.LogDebug("<<< Exiting CanPrepareBlogPostListModel()");
    }

    [Test]
    public void PrepareBlogPostListModelShouldRaiseExceptionIfCommandIsNull()
    {
		logger.LogDebug(">>> Entering PrepareBlogPostListModelShouldRaiseExceptionIfCommandIsNull()");
        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostListModelAsync(null).Wait());
			logger.LogDebug("<<< Exiting PrepareBlogPostListModelShouldRaiseExceptionIfCommandIsNull()");
    }

    [Test]
    public async Task CanPrepareBlogPostTagListModel()
    {
		logger.LogDebug(">>> Entering CanPrepareBlogPostTagListModel()");
        var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
        model.Tags.Count.Should().Be(5);
		logger.LogDebug("<<< Exiting CanPrepareBlogPostTagListModel()");
		logger.LogDebug("CanPrepareBlogPostTagListModel(): model → {Model}", model);
    }

    [Test]
    public async Task PrepareBlogPostTagListModelCountShouldDependOnSettings()
    {
		logger.LogDebug(">>> Entering PrepareBlogPostTagListModelCountShouldDependOnSettings()");
        var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
        model.Tags.Count.Should().Be(5);
		logger.LogDebug("PrepareBlogPostTagListModelCountShouldDependOnSettings(): model → {Model}", model);

        _blogSettings.NumberOfTags = 1;
		logger.LogDebug("PrepareBlogPostTagListModelCountShouldDependOnSettings(): _blogSettings.NumberOfTags → {Arg0}", _blogSettings.NumberOfTags);
        await _settingsService.SaveSettingAsync(_blogSettings);

        model = await GetService<IBlogModelFactory>().PrepareBlogPostTagListModelAsync();

        _blogSettings.NumberOfTags = 15;
        await _settingsService.SaveSettingAsync(_blogSettings);

        model.Tags.Count.Should().Be(1);
		logger.LogDebug("<<< Exiting PrepareBlogPostTagListModelCountShouldDependOnSettings()");
    }

    [Test]
    public async Task CanPrepareBlogPostYearModel()
    {
		logger.LogDebug(">>> Entering CanPrepareBlogPostYearModel()");
        var model = await _blogModelFactory.PrepareBlogPostYearModelAsync();
        var date = DateTime.Now.ToUniversalTime();
		logger.LogDebug("CanPrepareBlogPostYearModel(): model → {Model}", model);
		logger.LogDebug("CanPrepareBlogPostYearModel(): date → {Date}", date);

        model.Count.Should().Be(1);
        model[0].Year.Should().Be(date.Year);
        var months = model[0].Months;

        months.Count.Should().Be(1);

        var month = months.First();

		logger.LogDebug("CanPrepareBlogPostYearModel(): month → {Month}", month);
        month.BlogPostCount.Should().Be(2);
        month.Month.Should().Be(date.Month);
		logger.LogDebug("<<< Exiting CanPrepareBlogPostYearModel()");
    }

    [Test]
    public void PrepareBlogPostCommentModelShouldRaiseExceptionIfBlogCommentIsNull()
    {
		logger.LogDebug(">>> Entering PrepareBlogPostCommentModelShouldRaiseExceptionIfBlogCommentIsNull()");
        Assert.Throws<AggregateException>(() =>
            _blogModelFactory.PrepareBlogPostCommentModelAsync(null).Wait());
			logger.LogDebug("<<< Exiting PrepareBlogPostCommentModelShouldRaiseExceptionIfBlogCommentIsNull()");
    }

    [Test]
    public async Task PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue()
    {
		logger.LogDebug(">>> Entering PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue()");
        var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
        _customerSettings.AllowCustomersToUploadAvatars = true;
		logger.LogDebug("PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue(): blogComment → {BlogComment}", blogComment);
		logger.LogDebug("PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue(): _customerSettings.AllowCustomersToUploadAvatars → {Arg0}", _customerSettings.AllowCustomersToUploadAvatars);
        await _settingsService.SaveSettingAsync(_customerSettings);
        var blogModelFactory = GetService<IBlogModelFactory>();
        var model = await blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
		logger.LogDebug("PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue(): blogModelFactory → {BlogModelFactory}", blogModelFactory);
		logger.LogDebug("PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue(): model → {Model}", model);
        _customerSettings.AllowCustomersToUploadAvatars = false;
        await _settingsService.SaveSettingAsync(_customerSettings);

        model.CustomerAvatarUrl.Should().NotBeNullOrEmpty();
        model.CustomerAvatarUrl.Should()
            .Be($"http://{NopTestsDefaults.HostIpAddress}/images/thumbs/default-avatar_{GetService<MediaSettings>().AvatarPictureSize}.jpg");
			logger.LogDebug("<<< Exiting PrepareBlogPostCommentCustomerAvatarUrlShouldNotBeNullIfAllowCustomersToUploadAvatarsIsTrue()");
    }

    [Test]
    public async Task PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings()
    {
		logger.LogDebug(">>> Entering PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings()");
        var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
        var model = await _blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
		logger.LogDebug("PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings(): blogComment → {BlogComment}", blogComment);
		logger.LogDebug("PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings(): model → {Model}", model);
		logger.LogDebug("PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings(): _customerSettings.AllowViewingProfiles → {Arg0}", _customerSettings.AllowViewingProfiles);
        model.AllowViewingProfiles.Should().BeFalse();
        _customerSettings.AllowViewingProfiles = true;
        await _settingsService.SaveSettingAsync(_customerSettings);
        var blogModelFactory = GetService<IBlogModelFactory>();
        model = await blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
		logger.LogDebug("PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings(): blogModelFactory → {BlogModelFactory}", blogModelFactory);
        _customerSettings.AllowViewingProfiles = false;
        await _settingsService.SaveSettingAsync(_customerSettings);
        model.AllowViewingProfiles.Should().BeTrue();
		logger.LogDebug("<<< Exiting PrepareBlogPostCommentCustomerAllowViewingProfilesShouldBeDependOnSettings()");
    }

    [Test]
    public async Task CanPrepareBlogPostComment()
    {
		logger.LogDebug(">>> Entering CanPrepareBlogPostComment()");
        var blogComment = await _blogService.GetBlogCommentByIdAsync(1);
        var model = await _blogModelFactory.PrepareBlogPostCommentModelAsync(blogComment);
		logger.LogDebug("CanPrepareBlogPostComment(): blogComment → {BlogComment}", blogComment);
		logger.LogDebug("CanPrepareBlogPostComment(): model → {Model}", model);

        var customer = await _customerService.GetCustomerByIdAsync(1);

		logger.LogDebug("CanPrepareBlogPostComment(): customer → {Customer}", customer);
        model.Id.Should().Be(blogComment.Id);
        model.CustomerId.Should().Be(blogComment.CustomerId);
        model.CustomerName.Should().Be(await _customerService.FormatUsernameAsync(customer));
        model.CommentText.Should().Be(blogComment.CommentText);
        model.CreatedOn.Should().Be(await _dateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc));
        model.AllowViewingProfiles.Should().BeFalse();
        model.CustomerAvatarUrl.Should().BeNull();
		logger.LogDebug("<<< Exiting CanPrepareBlogPostComment()");
    }
}
