using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Type converted for ShippingOption
/// </summary>
public partial class ShippingOptionTypeConverter : TypeConverter
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Gets a value indicating whether this converter can        
    /// convert an object in the given source type to the native type of the converter
    /// using the context.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="sourceType">Source type</param>
    /// <returns>Result</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
		_logger.LogDebug(">>> Entering CanConvertFrom(context={Context},sourceType={SourceType})", context, sourceType);
        if (sourceType == typeof(string))
        {
		_logger.LogDebug("<<< Exiting CanConvertFrom(context={Context},sourceType={SourceType})", context, sourceType);
            return true;
        }

		_logger.LogDebug(">>> Entering ConvertFrom(context={Context},culture={Culture},value={Value})", context, culture, value);
		_logger.LogDebug("<<< Exiting ConvertFrom(context={Context},culture={Culture},value={Value})", context, culture, value);
        return base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Converts the given value object to the specified destination type using the specified context and arguments
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="culture">Culture</param>
    /// <param name="value">Value</param>
    /// <returns>Result</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is not string)
            return base.ConvertFrom(context, culture, value);

        var valueStr = value as string;

        if (string.IsNullOrEmpty(valueStr))
            return null;

        ShippingOption shippingOption = null;

        try
        {
            using var tr = new StringReader(valueStr);
            var xmlS = new XmlSerializer(typeof(ShippingOption));
            shippingOption = (ShippingOption)xmlS.Deserialize(tr);
        }
        catch
        {
            //XML error
        }

        return shippingOption;
    }

    /// <summary>
    /// Convert to
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="culture">Culture</param>
    /// <param name="value">Value</param>
    /// <param name="destinationType">Destination type</param>
    /// <returns>Result</returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
		_logger.LogDebug(">>> Entering ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType})", context, culture, value, destinationType);
        if (destinationType != typeof(string))
            return base.ConvertTo(context, culture, value, destinationType);

        if (value is not ShippingOption)
            return string.Empty;

        var sb = new StringBuilder();
		_logger.LogDebug("ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType}): sb → {Sb}", context, culture, value, destinationType, sb);
        using var tw = new StringWriter(sb);
		_logger.LogDebug("ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType}): tw → {Tw}", context, culture, value, destinationType, tw);
        var xmlS = new XmlSerializer(typeof(ShippingOption));
		_logger.LogDebug("ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType}): xmlS → {XmlS}", context, culture, value, destinationType, xmlS);
        xmlS.Serialize(tw, value);
        var serialized = sb.ToString();
		_logger.LogDebug("ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType}): serialized → {Serialized}", context, culture, value, destinationType, serialized);
		_logger.LogDebug("<<< Exiting ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType})", context, culture, value, destinationType);
        return serialized;
    }
}