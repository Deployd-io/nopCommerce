using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record ForumPageModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ForumPageModel()
    {
		_logger.LogDebug(">>> Entering ForumPageModel()");
        ForumTopics = new List<ForumTopicRowModel>();
		_logger.LogDebug("ForumPageModel(): ForumTopics → {ForumTopics}", ForumTopics);
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string SeName { get; set; }
    public string Description { get; set; }

    public string WatchForumText { get; set; }

    public IList<ForumTopicRowModel> ForumTopics { get; set; }
    public int TopicPageSize { get; set; }
    public int TopicTotalRecords { get; set; }
    public int TopicPageIndex { get; set; }

    public bool IsCustomerAllowedToSubscribe { get; set; }

    public bool ForumFeedsEnabled { get; set; }

    public int PostsPageSize { get; set; }

    public bool AllowPostVoting { get; set; }
}