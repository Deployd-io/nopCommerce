namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents button view render for DataTables column
/// </summary>
public partial class RenderButtonView : IRender
{
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the RenderButtonEdit class 
    /// </summary>
    /// <param name="url">URL to the edit action</param>
    public RenderButtonView(DataUrl url)
    {
		logger.LogDebug(">>> Entering RenderButtonView(url={Url})", url);
        Url = url;
        ClassName = NopButtonClassDefaults.Default;
		logger.LogDebug("<<< Exiting RenderButtonView(url={Url})", url);
		logger.LogDebug("RenderButtonView(url={Url}): ClassName → {ClassName}", url, ClassName);
		logger.LogInformation("RenderButtonView(url={Url}): Url → {Url1}", url, Url);
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
