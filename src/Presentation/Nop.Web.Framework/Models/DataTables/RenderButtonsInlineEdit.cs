using Microsoft.Extensions.Logging;
﻿namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents button render for DataTables column
/// </summary>
public partial class RenderButtonsInlineEdit : IRender
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the RenderButton class 
    /// </summary>
    public RenderButtonsInlineEdit()
    {
		_logger.LogDebug(">>> Entering RenderButtonsInlineEdit()");
        ClassName = NopButtonClassDefaults.Default;
		_logger.LogDebug("<<< Exiting RenderButtonsInlineEdit()");
		_logger.LogDebug("RenderButtonsInlineEdit(): ClassName → {ClassName}", ClassName);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets button class name
    /// </summary>
    public string ClassName { get; set; }

    #endregion
}