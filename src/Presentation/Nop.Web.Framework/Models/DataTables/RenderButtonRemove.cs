namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents button render for DataTables column
/// </summary>
public partial class RenderButtonRemove : IRender
{
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the RenderButton class 
    /// </summary>
    /// <param name="title">Button title</param>
    public RenderButtonRemove(string title)
    {
		logger.LogDebug(">>> Entering RenderButtonRemove(title={Title})", title);
        Title = title;
        ClassName = NopButtonClassDefaults.Default;
		logger.LogDebug("<<< Exiting RenderButtonRemove(title={Title})", title);
		logger.LogDebug("RenderButtonRemove(title={Title}): ClassName → {ClassName}", title, ClassName);
		logger.LogDebug("RenderButtonRemove(title={Title}): Title → {Title1}", title, Title);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets button title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets button class name
    /// </summary>
    public string ClassName { get; set; }

    #endregion
}
