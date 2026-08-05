using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Forums;

/// <summary>
/// Represents a forum group search model
/// </summary>
public partial record ForumGroupSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ForumGroupSearchModel()
    {
		_logger.LogDebug(">>> Entering ForumGroupSearchModel()");
        ForumSearch = new ForumSearchModel();
		_logger.LogDebug("<<< Exiting ForumGroupSearchModel()");
		_logger.LogDebug("ForumGroupSearchModel(): ForumSearch → {ForumSearch}", ForumSearch);
    }

    #endregion

    #region Properties

    public ForumSearchModel ForumSearch { get; set; }

    #endregion
}