using FluentAssertions;
using Nop.Core.Domain.Blogs;
using Nop.Services.Blogs;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Blogs;

[TestFixture]
public class BlogServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IBlogService _blogService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _blogService = GetService<IBlogService>();
		_logger.LogDebug("SetUp(): _blogService → {_blogService}", _blogService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task CanParseTags()
    {
		_logger.LogDebug(">>> Entering CanParseTags()");
        var blogPost = new BlogPost
        {
            Tags = "tag1, tag2, tag 3 4,  "
        };

        var tags = await _blogService.ParseTagsAsync(blogPost);
		_logger.LogDebug("CanParseTags(): tags → {Tags}", tags);
        tags.Count.Should().Be(3);
        tags[0].Should().Be("tag1");
        tags[1].Should().Be("tag2");
        tags[2].Should().Be("tag 3 4");
		_logger.LogDebug("<<< Exiting CanParseTags()");
    }

    [Test]
    public void ShouldBeAvailableWhenStartDateIsNotSet()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenStartDateIsNotSet()");
        var blogPost = new BlogPost
        {
            StartDateUtc = null
        };

        _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenStartDateIsNotSet()");
    }

    [Test]
    public void ShouldBeAvailableWhenStartDateIsLessThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenStartDateIsLessThanSomeDate()");
        var blogPost = new BlogPost
        {
            StartDateUtc = new DateTime(2010, 01, 02)
        };

        _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenStartDateIsLessThanSomeDate()");
    }

    [Test]
    public void ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()");
        var blogPost = new BlogPost
        {
            StartDateUtc = new DateTime(2010, 01, 02)
        };

        _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 01)).Should().BeFalse();
		_logger.LogDebug("<<< Exiting ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()");
    }

    [Test]
    public void ShouldBeAvailableWhenEndDateIsNotSet()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenEndDateIsNotSet()");
        var blogPost = new BlogPost
        {
            EndDateUtc = null
        };

        _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenEndDateIsNotSet()");
    }

    [Test]
    public void ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()");
        var blogPost = new BlogPost
        {
            EndDateUtc = new DateTime(2010, 01, 02)
        };

        _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 01)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()");
    }

    [Test]
    public void ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()");
        var blogPost = new BlogPost
        {
            EndDateUtc = new DateTime(2010, 01, 02)
        };

        _blogService.BlogPostIsAvailable(blogPost, new DateTime(2010, 01, 03)).Should().BeFalse();
		_logger.LogDebug("<<< Exiting ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()");
    }

    [Test]
    public async Task CanGetAllBlogPosts()
    {
		_logger.LogDebug(">>> Entering CanGetAllBlogPosts()");
        var blogPosts = await _blogService.GetAllBlogPostsAsync();
        blogPosts.TotalCount.Should().Be(2);
		_logger.LogDebug("CanGetAllBlogPosts(): blogPosts → {BlogPosts}", blogPosts);

        blogPosts = await _blogService.GetAllBlogPostsAsync(showHidden: true);
        blogPosts.TotalCount.Should().Be(2);
		_logger.LogDebug("<<< Exiting CanGetAllBlogPosts()");
    }

    [Test]
    public async Task CanGetAllBlogPostsByTag()
    {
		_logger.LogDebug(">>> Entering CanGetAllBlogPostsByTag()");
        var blogPosts = await _blogService.GetAllBlogPostsByTagAsync(showHidden: true);
        blogPosts.TotalCount.Should().Be(0);
		_logger.LogDebug("CanGetAllBlogPostsByTag(): blogPosts → {BlogPosts}", blogPosts);

        blogPosts = await _blogService.GetAllBlogPostsByTagAsync();
        blogPosts.TotalCount.Should().Be(0);

        blogPosts = await _blogService.GetAllBlogPostsByTagAsync(tag: "e-commerce");
        blogPosts.TotalCount.Should().Be(2);

        blogPosts = await _blogService.GetAllBlogPostsByTagAsync(tag: "nopCommerce");
        blogPosts.TotalCount.Should().Be(1);

        blogPosts = await _blogService.GetAllBlogPostsByTagAsync(tag: "blog");
        blogPosts.TotalCount.Should().Be(1);

        blogPosts = await _blogService.GetAllBlogPostsByTagAsync(tag: "not exists");
        blogPosts.TotalCount.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanGetAllBlogPostsByTag()");
    }

    [Test]
    public async Task CanGetAllBlogPostTags()
    {
		_logger.LogDebug(">>> Entering CanGetAllBlogPostTags()");
        var blogPostTags = await _blogService.GetAllBlogPostTagsAsync(1, 1);
        blogPostTags.Count.Should().Be(5);
		_logger.LogDebug("CanGetAllBlogPostTags(): blogPostTags → {BlogPostTags}", blogPostTags);

        blogPostTags = await _blogService.GetAllBlogPostTagsAsync(2, 1);
        blogPostTags.Count.Should().Be(5);

        blogPostTags = await _blogService.GetAllBlogPostTagsAsync(1, 2);
        blogPostTags.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanGetAllBlogPostTags()");
    }

    [Test]
    public async Task CanGetPostsByDate()
    {
		_logger.LogDebug(">>> Entering CanGetPostsByDate()");
        var posts = (await _blogService.GetAllBlogPostsByTagAsync(tag: "e-commerce")).OrderBy(b => b.Id).ToList();
        posts[1].CreatedOnUtc = posts[1].CreatedOnUtc.AddDays(1);
		_logger.LogDebug("CanGetPostsByDate(): posts → {Posts}", posts);

        var filteredPosts = await _blogService.GetPostsByDateAsync(posts, posts[0].CreatedOnUtc, posts[0].CreatedOnUtc);
		_logger.LogDebug("CanGetPostsByDate(): filteredPosts → {FilteredPosts}", filteredPosts);
        filteredPosts.Count.Should().Be(1);

        filteredPosts = await _blogService.GetPostsByDateAsync(posts, posts[0].CreatedOnUtc, posts[1].CreatedOnUtc);
        filteredPosts.Count.Should().Be(2);
		_logger.LogDebug("<<< Exiting CanGetPostsByDate()");
    }

    [Test]
    public async Task CanGetAllComments()
    {
		_logger.LogDebug(">>> Entering CanGetAllComments()");
        var comments = await _blogService.GetAllCommentsAsync();
        comments.Count.Should().Be(2);
		_logger.LogDebug("CanGetAllComments(): comments → {Comments}", comments);
        comments = await _blogService.GetAllCommentsAsync(blogPostId: 1);
        comments.Count.Should().Be(1);
        comments = await _blogService.GetAllCommentsAsync(blogPostId: 3);
        comments.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanGetAllComments()");
    }

    [Test]
    public async Task CanGetBlogCommentsCount()
    {
		_logger.LogDebug(">>> Entering CanGetBlogCommentsCount()");
        var post = await _blogService.GetBlogPostByIdAsync(1);
        var count = await _blogService.GetBlogCommentsCountAsync(post);
		_logger.LogDebug("CanGetBlogCommentsCount(): count → {Count}", count);
		_logger.LogDebug("CanGetBlogCommentsCount(): post → {Post}", post);
        count.Should().Be(1);
		_logger.LogDebug("<<< Exiting CanGetBlogCommentsCount()");
    }

    [Test]
    public async Task TestCRUD()
    {
		_logger.LogDebug(">>> Entering TestCRUD()");
        var post = new BlogPost
        {
            Title = string.Empty,
            Body = string.Empty,
            LanguageId = 1
        };

        await _blogService.InsertBlogPostAsync(post);

        var postId = post.Id;

        postId.Should().BeGreaterThan(0);

        post.Body.Should().BeNullOrEmpty();
        post.Body = "test body";
		_logger.LogDebug("TestCRUD(): post.Body → {Arg0}", post.Body);
        await _blogService.UpdateBlogPostAsync(post);
        var testPost = await _blogService.GetBlogPostByIdAsync(postId);
		_logger.LogDebug("TestCRUD(): testPost → {TestPost}", testPost);

        testPost.Body.Should().NotBeEmpty();
        testPost.Body.Should().BeEquivalentTo(post.Body);

        var comment = new BlogComment { BlogPostId = postId, StoreId = 1, CustomerId = 1 };

        await _blogService.InsertBlogCommentAsync(comment);
        var commentId = comment.Id;

        commentId.Should().BeGreaterThan(0);
        comment.CommentText.Should().BeNullOrEmpty();
        comment.CommentText = "test comment";
		_logger.LogDebug("TestCRUD(): comment.CommentText → {Arg0}", comment.CommentText);
        await _blogService.UpdateBlogCommentAsync(comment);
        var testComment = await _blogService.GetBlogCommentByIdAsync(commentId);
		_logger.LogDebug("TestCRUD(): testComment → {TestComment}", testComment);

        testComment.CommentText.Should().NotBeEmpty();
        testComment.CommentText.Should().BeEquivalentTo(comment.CommentText);

        await _blogService.DeleteBlogCommentAsync(comment);
        testComment = await _blogService.GetBlogCommentByIdAsync(commentId);
        testComment.Should().BeNull();

        var comments = new List<BlogComment>
		_logger.LogDebug("<<< Exiting TestCRUD()");
        {
            new() {BlogPostId = postId, StoreId = 1, CustomerId = 1},
            new() {BlogPostId = postId, StoreId = 1, CustomerId = 1},
            new() {BlogPostId = postId, StoreId = 1, CustomerId = 1}
        } as IList<BlogComment>;

        foreach (var blogComment in comments)
            await _blogService.InsertBlogCommentAsync(blogComment);

        comments = await _blogService.GetBlogCommentsByIdsAsync(comments.Select(p => p.Id).ToArray());
        comments.Count.Should().Be(3);

        await _blogService.DeleteBlogCommentsAsync(comments);

        comments = await _blogService.GetBlogCommentsByIdsAsync(comments.Select(p => p.Id).ToArray());
        comments.Count.Should().Be(0);

        await _blogService.DeleteBlogPostAsync(post);

        testPost = await _blogService.GetBlogPostByIdAsync(postId);
        testPost.Should().BeNull();
    }
}