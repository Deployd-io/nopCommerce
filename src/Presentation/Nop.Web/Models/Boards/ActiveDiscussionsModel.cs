using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record ActiveDiscussionsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ActiveDiscussionsModel()
    {
		_logger.LogDebug(">>> Entering ActiveDiscussionsModel()");
        ForumTopics = new List<ForumTopicRowModel>();
		_logger.LogDebug("ActiveDiscussionsModel(): ForumTopics → {ForumTopics}", ForumTopics);
		_logger.LogDebug("<<< Exiting ActiveDiscussionsModel()");
    }

    public IList<ForumTopicRowModel> ForumTopics { get; protected set; }

    public bool ViewAllLinkEnabled { get; set; }

    public bool ActiveDiscussionsFeedEnabled { get; set; }

    public int TopicPageSize { get; set; }
    public int TopicTotalRecords { get; set; }
    public int TopicPageIndex { get; set; }

    public int PostsPageSize { get; set; }

    public bool AllowPostVoting { get; set; }
}