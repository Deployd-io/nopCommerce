using Microsoft.Extensions.Logging;
﻿namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents picture render for DataTables column
/// </summary>
public partial class RenderPicture : IRender
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public RenderPicture(string srcPrefix = "", int width = 0)
    {
		_logger.LogDebug(">>> Entering RenderPicture(srcPrefix={SrcPrefix},width={Width})", srcPrefix, width);
        SrcPrefix = srcPrefix;
        Width = width;
		_logger.LogDebug("<<< Exiting RenderPicture(srcPrefix={SrcPrefix},width={Width})", srcPrefix, width);
		_logger.LogDebug("RenderPicture(srcPrefix={SrcPrefix},width={Width}): Width → {Width1}", srcPrefix, width, Width);
		_logger.LogDebug("RenderPicture(srcPrefix={SrcPrefix},width={Width}): SrcPrefix → {SrcPrefix1}", srcPrefix, width, SrcPrefix);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets picture URL prefix
    /// </summary>
    public string SrcPrefix { get; set; }

    /// <summary>
    /// Gets or sets picture source
    /// </summary>
    public string Src { get; set; }

    /// <summary>
    /// Gets or sets picture width
    /// </summary>
    public int Width { get; set; }

    #endregion
}