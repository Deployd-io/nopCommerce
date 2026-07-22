namespace Nop.Core.Domain.Blogs;

/// <summary>
/// Blog post comment approved event
/// </summary>
public partial class BlogCommentApprovedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="blogComment">Blog comment</param>
    public BlogCommentApprovedEvent(BlogComment blogComment)
    {
		logger.LogDebug(">>> Entering BlogCommentApprovedEvent(blogComment={BlogComment})", blogComment);
        BlogComment = blogComment;
		logger.LogDebug("<<< Exiting BlogCommentApprovedEvent(blogComment={BlogComment})", blogComment);
		logger.LogDebug("BlogCommentApprovedEvent(blogComment={BlogComment}): BlogComment → {BlogComment1}", blogComment, BlogComment);
    }

    /// <summary>
    /// Blog post comment
    /// </summary>
    public BlogComment BlogComment { get; }
}
