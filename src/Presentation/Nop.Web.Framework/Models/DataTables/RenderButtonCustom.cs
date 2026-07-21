namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents button custom render for DataTables column
/// </summary>
public partial class RenderButtonCustom : IRender
{
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the RenderButtonEdit class 
    /// </summary>
    /// <param name="className">Class name of button</param>
    /// <param name="title">Title button</param>
    public RenderButtonCustom(string className, string title)
    {
		logger.LogDebug(">>> Entering RenderButtonCustom(className={ClassName},title={Title})", className, title);
        ClassName = className;
        Title = title;
		logger.LogDebug("<<< Exiting RenderButtonCustom(className={ClassName},title={Title})", className, title);
		logger.LogDebug("RenderButtonCustom(className={ClassName},title={Title}): Title → {Title1}", className, title, Title);
		logger.LogDebug("RenderButtonCustom(className={ClassName},title={Title}): ClassName → {ClassName1}", className, title, ClassName);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Url to action
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets button class name
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// Gets or sets button title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets function name on click button
    /// </summary>
    public string OnClickFunctionName { get; set; }

    #endregion
}
