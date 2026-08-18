using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Boards;

public partial record CustomerForumSubscriptionsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerForumSubscriptionsModel()
    {
		_logger.LogDebug(">>> Entering CustomerForumSubscriptionsModel()");
        ForumSubscriptions = new List<ForumSubscriptionModel>();
		_logger.LogDebug("CustomerForumSubscriptionsModel(): ForumSubscriptions → {ForumSubscriptions}", ForumSubscriptions);
		_logger.LogDebug("<<< Exiting CustomerForumSubscriptionsModel()");
    }

    public IList<ForumSubscriptionModel> ForumSubscriptions { get; set; }
    public PagerModel PagerModel { get; set; }

    #region Nested classes

    public partial record ForumSubscriptionModel : BaseNopEntityModel
    {
        public int ForumId { get; set; }
        public int ForumTopicId { get; set; }
        public bool TopicSubscription { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }

    #endregion
}