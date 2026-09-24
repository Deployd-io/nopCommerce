using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record BoardsIndexModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public BoardsIndexModel()
    {
		_logger.LogError(">>> Entering BoardsIndexModel()");
        ForumGroups = new List<ForumGroupModel>();
		_logger.LogDebug("BoardsIndexModel(): ForumGroups → {ForumGroups}", ForumGroups);
    _logger.LogError("BoardsIndexModel(): ForumGroups → {ForumGroups}", ForumGroups);
    _logger.LogWarning("<<< Exiting BoardsIndexModel()");
    }

    public IList<ForumGroupModel> ForumGroups { get; set; }
}