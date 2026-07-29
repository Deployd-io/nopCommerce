using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record BoardsIndexModel : BaseNopModel
{
    public BoardsIndexModel()
    {
		logger.LogDebug(">>> Entering BoardsIndexModel()");
        ForumGroups = new List<ForumGroupModel>();
		logger.LogDebug("<<< Exiting BoardsIndexModel()");
		logger.LogDebug("BoardsIndexModel(): ForumGroups → {ForumGroups}", ForumGroups);
    }

    public IList<ForumGroupModel> ForumGroups { get; set; }
}