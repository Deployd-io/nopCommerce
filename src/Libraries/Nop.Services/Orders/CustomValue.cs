using Microsoft.Extensions.Logging;
namespace Nop.Services.Orders;

/// <summary>
/// Represents the custom value
/// </summary>
public partial class CustomValue
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomValue(string name, string value, CustomValueDisplayLocation displayLocation = CustomValueDisplayLocation.Payment, bool displayToCustomer = true, DateTime? createdOnUtc = null)
    {
		_logger.LogDebug(">>> Entering CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc})", name, value, displayLocation, displayToCustomer, createdOnUtc);
        Name = name;
		_logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): Name → {Name1}", name, value, displayLocation, displayToCustomer, createdOnUtc, Name);
        Value = value;
        DisplayLocation = displayLocation;
		_logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): Value → {Value1}", name, value, displayLocation, displayToCustomer, createdOnUtc, Value);
		_logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): DisplayLocation → {DisplayLocation1}", name, value, displayLocation, displayToCustomer, createdOnUtc, DisplayLocation);
        DisplayToCustomer = displayToCustomer;
        CreatedOnUtc = createdOnUtc;
		_logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): DisplayToCustomer → {DisplayToCustomer1}", name, value, displayLocation, displayToCustomer, createdOnUtc, DisplayToCustomer);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value of custom value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets a name of custom value
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a customer can see a custom value
    /// </summary>
    public bool DisplayToCustomer { get; set; }

    /// <summary>
    /// Gets or sets a display location of custom value
    /// </summary>
    public CustomValueDisplayLocation DisplayLocation { get; set; }

    /// <summary>
    /// Gets or sets a created on date and time
    /// </summary>
    public DateTime? CreatedOnUtc { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Returns a string that represents the current object
    /// </summary>
    /// <returns>A string that represents the current object</returns>
    public override string ToString()
    {
		_logger.LogDebug(">>> Entering ToString()");
        return Value;
    }

    #endregion
}