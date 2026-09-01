using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Nop.Services.Orders;

/// <summary>
/// Represents the list of custom value
/// </summary>
public partial class CustomValues : List<CustomValue>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Get custom values by specified display location
    /// </summary>
    /// <param name="displayLocation">Display location</param>
    /// <returns>List of custom values filtered by display location</returns>
    public virtual List<CustomValue> GetValuesByDisplayLocation(CustomValueDisplayLocation displayLocation)
    {
		_logger.LogDebug(">>> Entering GetValuesByDisplayLocation(displayLocation={DisplayLocation})", displayLocation);
		_logger.LogDebug("<<< Exiting GetValuesByDisplayLocation(displayLocation={DisplayLocation})", displayLocation);
        return this.Where(cv => cv.DisplayLocation == displayLocation).ToList();
    }

    /// <summary>
    /// Serialize list of custom values into XML format
    /// </summary>
    /// <returns>XML of custom values</returns>
    public virtual string SerializeToXml()
    {
		_logger.LogDebug(">>> Entering SerializeToXml()");
        if (!this.Any())
            return string.Empty;

        using var textWriter = new StringWriter();
		_logger.LogDebug("SerializeToXml(): textWriter → {TextWriter}", textWriter);

        var root = new XElement("DictionarySerializer");
		_logger.LogWarning("SerializeToXml(): root → {Root}", root);
        root.Add(this.Select(value => new XElement("item",
            new XElement("key", value.Name),
            new XElement("value", value.Value),
            new XElement("displayToCustomer", value.DisplayToCustomer),
            new XElement("location", (int)value.DisplayLocation),
            new XElement("createdOn", value.CreatedOnUtc?.Ticks ?? 0))));

        var document = new XDocument();
		_logger.LogDebug("SerializeToXml(): document → {Document}", document);
        document.Add(root);
        document.Save(textWriter, SaveOptions.DisableFormatting);

        var result = textWriter.ToString();
		_logger.LogDebug("SerializeToXml(): result → {Result}", result);

		_logger.LogDebug("<<< Exiting SerializeToXml()");
        return result;
    }

    /// <summary>
    /// Fill order custom values by XML
    /// </summary>
    /// <param name="customValuesXml">XML of custom values</param>
    /// <param name="displayToCustomerOnly">The flag indicates whether we should use only values that are allowed for customers</param>
    /// <returns>Custom values</returns>
    public virtual void FillByXml(string customValuesXml, bool displayToCustomerOnly = false)
    {
		_logger.LogDebug(">>> Entering FillByXml(customValuesXml={CustomValuesXml},displayToCustomerOnly={DisplayToCustomerOnly})", customValuesXml, displayToCustomerOnly);
        Clear();

        if (string.IsNullOrWhiteSpace(customValuesXml))
            return;

        //use the 'Payment' value as default for compatibility with previous versions
        var defaultLocation = CustomValueDisplayLocation.Payment;

        //display a custom value by default for compatibility with previous versions
        var defaultDisplayToCustomer = true;

        try
        {
            using var textReader = new StringReader(customValuesXml);
			_logger.LogDebug("FillByXml(customValuesXml={CustomValuesXml},displayToCustomerOnly={DisplayToCustomerOnly}): textReader → {TextReader}", customValuesXml, displayToCustomerOnly, textReader);
            var rootElement = XDocument.Load(textReader).Root;
			_logger.LogDebug("FillByXml(customValuesXml={CustomValuesXml},displayToCustomerOnly={DisplayToCustomerOnly}): rootElement → {RootElement}", customValuesXml, displayToCustomerOnly, rootElement);
            AddRange(rootElement!.Elements("item").Select(i =>
            {
                var name = i.Element("key")!.Value;
				_logger.LogDebug("FillByXml(customValuesXml={CustomValuesXml},displayToCustomerOnly={DisplayToCustomerOnly}): name → {Name}", customValuesXml, displayToCustomerOnly, name);
                var value = i.Element("value")!.Value;
				_logger.LogDebug("FillByXml(customValuesXml={CustomValuesXml},displayToCustomerOnly={DisplayToCustomerOnly}): value → {Value}", customValuesXml, displayToCustomerOnly, value);
                var displayLocation = int.TryParse(i.Element("location")?.Value, out var location) && Enum.IsDefined(typeof(CustomValueDisplayLocation), location)
                    ? (CustomValueDisplayLocation)location
                    : defaultLocation;
                var displayToCustomer = bool.TryParse(i.Element("displayToCustomer")?.Value, out var display)
                    ? display
                    : defaultDisplayToCustomer;
                var createdOn = long.TryParse(i.Element("createdOn")?.Value, out var ticks)
                    ? (ticks > 0 ? new DateTime(ticks) : (DateTime?)null)
                    : null;

                return new CustomValue(name, value, displayLocation, displayToCustomer, createdOn);
            }).Where(value => value.DisplayToCustomer || !displayToCustomerOnly));
        }
        catch
        {
			_logger.LogError("Exception in FillByXml(customValuesXml={CustomValuesXml},displayToCustomerOnly={DisplayToCustomerOnly}): {Arg2}", customValuesXml, displayToCustomerOnly, ex.Message);
            //ignore
        }
		_logger.LogDebug("<<< Exiting FillByXml(customValuesXml={CustomValuesXml},displayToCustomerOnly={DisplayToCustomerOnly})", customValuesXml, displayToCustomerOnly);
    }

    /// <summary>
    /// Removes the custom value with the specified name
    /// </summary>
    /// <param name="name">The name of the custom value.</param>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="name" /> was not found.
    /// </returns>
    public virtual bool Remove(string name)
    {
		_logger.LogDebug(">>> Entering Remove(name={Name})", name);
        return TryGetValue(name, out var value) && Remove(value);
    }

    /// <summary>
    /// Gets the value associated with the specified name
    /// </summary>
    /// <param name="name">The name whose value to get.</param>
    /// <param name="customValue">When this method returns, the value associated with the specified name, if the name is found; otherwise, the null value</param>
    /// <returns>
    /// <see langword="true" /> if the custom values list contains an element with the specified name; otherwise, <see langword="false" />.
    /// </returns>
    public virtual bool TryGetValue(string name, out CustomValue customValue)
    {
		_logger.LogDebug(">>> Entering TryGetValue(name={Name})", name);
        customValue = this.FirstOrDefault(cv => cv.Name.Equals(name));

		_logger.LogDebug("TryGetValue(name={Name}): customValue → {CustomValue}", name, customValue);
		_logger.LogDebug("<<< Exiting TryGetValue(name={Name})", name);
        return customValue != null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the value of custom value with the specified name
    /// </summary>
    /// <param name="name">The name of the custom value to get or set</param>
    /// <returns>The value of custom value with the specified name</returns>
    public virtual string this[string name]
    {
        get
        {
            var value = this.First(cv => cv.Name.Equals(name));

            return value.Value;
        }
        set
        {
            Remove(name);
            Add(new CustomValue(name, value));
        }
    }

    #endregion
}