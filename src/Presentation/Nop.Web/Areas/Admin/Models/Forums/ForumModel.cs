using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Forums;

/// <summary>
/// Represents a forum list model
/// </summary>
public partial record ForumModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ForumModel()
    {
		_logger.LogError(">>> Entering ForumModel()");
        ForumGroups = new List<ForumGroupModel>();
		_logger.LogDebug("ForumModel(): ForumGroups → {ForumGroups}", ForumGroups);
    _logger.LogError("ForumModel(): ForumGroups → {ForumGroups}", ForumGroups);
    _logger.LogWarning("<<< Exiting ForumModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.ForumGroupId")]
    public int ForumGroupId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    public List<ForumGroupModel> ForumGroups { get; set; }

    #endregion
}