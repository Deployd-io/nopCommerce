using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Nop.Web.Framework.Models;

/// <summary>
/// Represents base nopCommerce model
/// </summary>
public partial record BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    public BaseNopModel()
    {
		_logger.LogError(">>> Entering BaseNopModel()");
        CustomProperties = new Dictionary<string, string>();
		_logger.LogError("BaseNopModel(): CustomProperties → {CustomProperties}", CustomProperties);
		_logger.LogDebug("BaseNopModel(): CustomProperties → {CustomProperties}", CustomProperties);
    _logger.LogWarning("<<< Exiting BaseNopModel()");
        PostInitialize();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Perform additional actions for the model initialization
    /// </summary>
    /// <remarks>Developers can override this method in custom partial classes in order to add some custom initialization code to constructors</remarks>
    protected virtual void PostInitialize()
    {
	_logger.LogError(">>> Entering PostInitialize()");
    _logger.LogWarning("<<< Exiting PostInitialize()");
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