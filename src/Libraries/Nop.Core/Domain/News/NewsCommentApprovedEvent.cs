namespace Nop.Core.Domain.News;

/// <summary>
/// News comment approved event
/// </summary>
public partial class NewsCommentApprovedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="newsComment">News comment</param>
    public NewsCommentApprovedEvent(NewsComment newsComment)
    {
		logger.LogDebug(">>> Entering NewsCommentApprovedEvent(newsComment={NewsComment})", newsComment);
        NewsComment = newsComment;
		logger.LogDebug("<<< Exiting NewsCommentApprovedEvent(newsComment={NewsComment})", newsComment);
		logger.LogDebug("NewsCommentApprovedEvent(newsComment={NewsComment}): NewsComment → {NewsComment1}", newsComment, NewsComment);
    }

    /// <summary>
    /// News comment
    /// </summary>
    public NewsComment NewsComment { get; }
}
