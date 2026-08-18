using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Boards;

public partial record TopicMoveModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TopicMoveModel()
    {
		_logger.LogDebug(">>> Entering TopicMoveModel()");
        ForumList = new List<SelectListItem>();
		_logger.LogDebug("TopicMoveModel(): ForumList → {ForumList}", ForumList);
		_logger.LogDebug("<<< Exiting TopicMoveModel()");
    }

    public int ForumSelected { get; set; }
    public string TopicSeName { get; set; }

    public IEnumerable<SelectListItem> ForumList { get; set; }
}