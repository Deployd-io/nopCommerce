namespace Nop.Services.Orders;

/// <summary>
/// Represents the custom value
/// </summary>
public partial class CustomValue
{
    #region Ctor

    public CustomValue(string name, string value, CustomValueDisplayLocation displayLocation = CustomValueDisplayLocation.Payment, bool displayToCustomer = true, DateTime? createdOnUtc = null)
    {
		logger.LogDebug(">>> Entering CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc})", name, value, displayLocation, displayToCustomer, createdOnUtc);
        Name = name;
        Value = value;
		logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): Name → {Name1}", name, value, displayLocation, displayToCustomer, createdOnUtc, Name);
		logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): Value → {Value1}", name, value, displayLocation, displayToCustomer, createdOnUtc, Value);
        DisplayLocation = displayLocation;
        DisplayToCustomer = displayToCustomer;
		logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): DisplayLocation → {DisplayLocation1}", name, value, displayLocation, displayToCustomer, createdOnUtc, DisplayLocation);
		logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): DisplayToCustomer → {DisplayToCustomer1}", name, value, displayLocation, displayToCustomer, createdOnUtc, DisplayToCustomer);
        CreatedOnUtc = createdOnUtc;
		logger.LogDebug("<<< Exiting CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc})", name, value, displayLocation, displayToCustomer, createdOnUtc);
    }

	logger.LogDebug("CustomValue(name={Name},value={Value},displayLocation={DisplayLocation},displayToCustomer={DisplayToCustomer},createdOnUtc={CreatedOnUtc}): CreatedOnUtc → {CreatedOnUtc1}", name, value, displayLocation, displayToCustomer, createdOnUtc, CreatedOnUtc);
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
		logger.LogDebug(">>> Entering ToString()");
		logger.LogDebug("<<< Exiting ToString()");
        return Value;
    }

    #endregion
}
