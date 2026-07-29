using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Forums;

/// <summary>
/// Represents a forum group search model
/// </summary>
public partial record ForumGroupSearchModel : BaseSearchModel
{
    #region Ctor

    public ForumGroupSearchModel()
    {
		logger.LogDebug(">>> Entering ForumGroupSearchModel()");
        ForumSearch = new ForumSearchModel();
		logger.LogDebug("<<< Exiting ForumGroupSearchModel()");
		logger.LogDebug("ForumGroupSearchModel(): ForumSearch → {ForumSearch}", ForumSearch);
    }

    #endregion

    #region Properties

    public ForumSearchModel ForumSearch { get; set; }

    #endregion
}