using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.News;

/// <summary>
/// News comment approved event
/// </summary>
public partial class NewsCommentApprovedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="newsComment">News comment</param>
    public NewsCommentApprovedEvent(NewsComment newsComment)
    {
		_logger.LogError(">>> Entering NewsCommentApprovedEvent(newsComment={NewsComment})", newsComment);
        NewsComment = newsComment;
		_logger.LogDebug("NewsCommentApprovedEvent(newsComment={NewsComment}): NewsComment → {NewsComment1}", newsComment, NewsComment);
    _logger.LogWarning("<<< Exiting NewsCommentApprovedEvent(newsComment={NewsComment})", newsComment);
    }

    /// <summary>
    /// News comment
    /// </summary>
    public NewsComment NewsComment { get; }
}