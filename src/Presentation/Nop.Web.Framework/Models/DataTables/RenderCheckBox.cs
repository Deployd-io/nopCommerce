namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents checkbox render for DataTables column
/// </summary>
public partial class RenderCheckBox : IRender
{
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the RenderCheckBox class 
    /// </summary>
    /// <param name="name">Checkbox name</param>
    /// <param name="propertyKeyName">Property key name ("Id" by default). This property must be defined in the row dataset.</param>
    public RenderCheckBox(string name, string propertyKeyName = "Id")
    {
		logger.LogDebug(">>> Entering RenderCheckBox(name={Name},propertyKeyName={PropertyKeyName})", name, propertyKeyName);
        Name = name;
        PropertyKeyName = propertyKeyName;
		logger.LogDebug("<<< Exiting RenderCheckBox(name={Name},propertyKeyName={PropertyKeyName})", name, propertyKeyName);
		logger.LogDebug("RenderCheckBox(name={Name},propertyKeyName={PropertyKeyName}): Name → {Name1}", name, propertyKeyName, Name);
		logger.LogDebug("RenderCheckBox(name={Name},propertyKeyName={PropertyKeyName}): PropertyKeyName → {PropertyKeyName1}", name, propertyKeyName, PropertyKeyName);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets name checkbox
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets identificator for row 
    /// </summary>
    public string PropertyKeyName { get; set; }

    #endregion
}
