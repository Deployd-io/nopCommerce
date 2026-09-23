using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record ForumGroupModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ForumGroupModel()
    {
		_logger.LogError(">>> Entering ForumGroupModel()");
        Forums = new List<ForumRowModel>();
		_logger.LogDebug("ForumGroupModel(): Forums → {Forums}", Forums);
    _logger.LogError("ForumGroupModel(): Forums → {Forums}", Forums);
    _logger.LogWarning("<<< Exiting ForumGroupModel()");
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string SeName { get; set; }

    public IList<ForumRowModel> Forums { get; set; }
}