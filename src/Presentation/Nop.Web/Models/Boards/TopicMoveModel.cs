using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record TopicMoveModel : BaseNopEntityModel
{
    public TopicMoveModel()
    {
		logger.LogDebug(">>> Entering TopicMoveModel()");
        ForumList = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting TopicMoveModel()");
		logger.LogDebug("TopicMoveModel(): ForumList → {ForumList}", ForumList);
    }

    public int ForumSelected { get; set; }
    public string TopicSeName { get; set; }

    public IEnumerable<SelectListItem> ForumList { get; set; }
}