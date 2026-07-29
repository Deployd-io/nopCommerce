using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record ActiveDiscussionsModel : BaseNopModel
{
    public ActiveDiscussionsModel()
    {
		logger.LogDebug(">>> Entering ActiveDiscussionsModel()");
        ForumTopics = new List<ForumTopicRowModel>();
		logger.LogDebug("<<< Exiting ActiveDiscussionsModel()");
		logger.LogDebug("ActiveDiscussionsModel(): ForumTopics → {ForumTopics}", ForumTopics);
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