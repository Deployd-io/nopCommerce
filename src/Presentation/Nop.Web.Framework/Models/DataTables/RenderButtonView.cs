using Microsoft.Extensions.Logging;
﻿namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents button view render for DataTables column
/// </summary>
public partial class RenderButtonView : IRender
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the RenderButtonEdit class 
    /// </summary>
    /// <param name="url">URL to the edit action</param>
    public RenderButtonView(DataUrl url)
    {
		_logger.LogDebug(">>> Entering RenderButtonView(url={Url})", url);
        Url = url;
        ClassName = NopButtonClassDefaults.Default;
		_logger.LogDebug("<<< Exiting RenderButtonView(url={Url})", url);
		_logger.LogDebug("RenderButtonView(url={Url}): ClassName → {ClassName}", url, ClassName);
		_logger.LogInformation("RenderButtonView(url={Url}): Url → {Url1}", url, Url);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Url to action edit
    /// </summary>
    public DataUrl Url { get; set; }

    /// <summary>
    /// Gets or sets button class name
    /// </summary>
    public string ClassName { get; set; }

    #endregion
}