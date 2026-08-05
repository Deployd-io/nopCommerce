using System.Xml.Serialization;

namespace Nop.Web.Framework.Models;

/// <summary>
/// Represents base nopCommerce model
/// </summary>
public partial record BaseNopModel
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    public BaseNopModel()
    {
		logger.LogDebug(">>> Entering BaseNopModel()");
        CustomProperties = new Dictionary<string, string>();
        PostInitialize();
		logger.LogDebug("<<< Exiting BaseNopModel()");
		logger.LogDebug("BaseNopModel(): CustomProperties → {CustomProperties}", CustomProperties);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Perform additional actions for the model initialization
    /// </summary>
    /// <remarks>Developers can override this method in custom partial classes in order to add some custom initialization code to constructors</remarks>
    protected virtual void PostInitialize()
    {
	logger.LogDebug(">>> Entering PostInitialize()");
	logger.LogDebug("<<< Exiting PostInitialize()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets property to store any custom values for models 
    /// </summary>
    [XmlIgnore]
    public Dictionary<string, string> CustomProperties { get; set; }

    #endregion
}