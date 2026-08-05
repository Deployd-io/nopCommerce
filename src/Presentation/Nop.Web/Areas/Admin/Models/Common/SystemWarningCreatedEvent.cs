using Microsoft.Extensions.Logging;
﻿namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an event that occurs when system warnings are creating
/// </summary>
public partial class SystemWarningCreatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SystemWarningCreatedEvent()
    {
		_logger.LogDebug(">>> Entering SystemWarningCreatedEvent()");
        SystemWarnings = new List<SystemWarningModel>();
		_logger.LogDebug("<<< Exiting SystemWarningCreatedEvent()");
		_logger.LogDebug("SystemWarningCreatedEvent(): SystemWarnings → {SystemWarnings}", SystemWarnings);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a list of warnings
    /// </summary>
    public IList<SystemWarningModel> SystemWarnings { get; }

    #endregion
}