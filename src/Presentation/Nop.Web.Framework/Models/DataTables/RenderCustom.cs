namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents custom render for DataTables column
/// </summary>
public partial class RenderCustom : IRender
{
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the RenderCustom class 
    /// </summary>
    /// <param name="functionName">Custom render function name that is used in js</param>
    public RenderCustom(string functionName)
    {
		logger.LogDebug(">>> Entering RenderCustom(functionName={FunctionName})", functionName);
        FunctionName = functionName;
		logger.LogDebug("<<< Exiting RenderCustom(functionName={FunctionName})", functionName);
		logger.LogDebug("RenderCustom(functionName={FunctionName}): FunctionName → {FunctionName1}", functionName, FunctionName);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets custom render function name(js)
    /// See also https://datatables.net/reference/option/columns.render
    /// </summary>
    public string FunctionName { get; set; }

    #endregion
}
