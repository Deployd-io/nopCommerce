using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Blogs;

/// <summary>
/// Blog post comment approved event
/// </summary>
public partial class BlogCommentApprovedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="blogComment">Blog comment</param>
    public BlogCommentApprovedEvent(BlogComment blogComment)
    {
		_logger.LogError(">>> Entering BlogCommentApprovedEvent(blogComment={BlogComment})", blogComment);
        BlogComment = blogComment;
		_logger.LogDebug("BlogCommentApprovedEvent(blogComment={BlogComment}): BlogComment → {BlogComment1}", blogComment, BlogComment);
    _logger.LogWarning("<<< Exiting BlogCommentApprovedEvent(blogComment={BlogComment})", blogComment);
    }

    /// <summary>
    /// Blog post comment
    /// </summary>
    public BlogComment BlogComment { get; }
}