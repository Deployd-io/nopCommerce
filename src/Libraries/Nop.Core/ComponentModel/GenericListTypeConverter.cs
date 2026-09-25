using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Nop.Core.ComponentModel;

/// <summary>
/// Generic List type converted
/// </summary>
/// <typeparam name="T">Type</typeparam>
public partial class GenericListTypeConverter<T> : TypeConverter
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Type converter
    /// </summary>
    protected readonly TypeConverter typeConverter;

    public GenericListTypeConverter()
    {
		_logger.LogError(">>> Entering GenericListTypeConverter()");
        typeConverter = TypeDescriptor.GetConverter(typeof(T));
		_logger.LogDebug("GenericListTypeConverter(): typeConverter → {TypeConverter}", typeConverter);
			_logger.LogWarning("<<< Exiting GenericListTypeConverter()");
        if (typeConverter == null) {
			_logger.LogError("Throwing InvalidOperationException from GenericListTypeConverter()");
    _logger.LogError("GenericListTypeConverter(): typeConverter → {TypeConverter}", typeConverter);
        }
            throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);
    }

    /// <summary>
    /// Get string array from a comma-separate string
    /// </summary>
    /// <param name="input">Input</param>
    /// <returns>Array</returns>
    protected virtual string[] GetStringArray(string input)
    {
		_logger.LogError(">>> Entering GetStringArray(input={Input})", input);
		_logger.LogWarning("<<< Exiting GetStringArray(input={Input})", input);
        return string.IsNullOrEmpty(input) ? Array.Empty<string>() : input.Split(',').Select(x => x.Trim()).ToArray();
    }

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
		_logger.LogError(">>> Entering CanConvertFrom(context={Context},sourceType={SourceType})", context, sourceType);
        if (sourceType != typeof(string))
            return base.CanConvertFrom(context, sourceType);

        var items = GetStringArray(sourceType.ToString());
		_logger.LogError("CanConvertFrom(context={Context},sourceType={SourceType}): items → {Items}", context, sourceType, items);
		_logger.LogWarning("<<< Exiting CanConvertFrom(context={Context},sourceType={SourceType})", context, sourceType);
        return items.Any();
    }

    /// <summary>
    /// Converts the given object to the converter's native type.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="culture">Culture</param>
    /// <param name="value">Value</param>
    /// <returns>Result</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is not string && value != null)
            return base.ConvertFrom(context, culture, value);

        var items = GetStringArray((string)value);

        return items.Select(typeConverter.ConvertFromInvariantString)
            .Where(item => item != null)
            .Cast<T>()
            .ToList();
    }

    /// <summary>
    /// Converts the given value object to the specified destination type using the specified context and arguments
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="culture">Culture</param>
    /// <param name="value">Value</param>
    /// <param name="destinationType">Destination type</param>
    /// <returns>Result</returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
		_logger.LogError(">>> Entering ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType})", context, culture, value, destinationType);
        if (destinationType != typeof(string))
            return base.ConvertTo(context, culture, value, destinationType);

        var result = string.Empty;
		_logger.LogError("ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType}): result → {Result}", context, culture, value, destinationType, result);

        if (value == null)
            return result;

        var cultureInvariantStrings = ((IList<T>)value)
            .Select(o => Convert.ToString(o, CultureInfo.InvariantCulture));

        result = string.Join(',', cultureInvariantStrings);

		_logger.LogWarning("<<< Exiting ConvertTo(context={Context},culture={Culture},value={Value},destinationType={DestinationType})", context, culture, value, destinationType);
        return result;
    }
}