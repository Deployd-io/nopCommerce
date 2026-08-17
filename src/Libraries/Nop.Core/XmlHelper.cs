using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Nop.Core;

/// <summary>
/// XML helper class
/// </summary>
public partial class XmlHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// XML Encode
    /// </summary>
    /// <param name="str">String</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the encoded string
    /// </returns>
    public static async Task<string> XmlEncodeAsync(string str)
    {
		_logger.LogDebug(">>> Entering XmlEncodeAsync(str={Str})", str);
        if (str == null)
            return null;
        str = Regex.Replace(str, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", string.Empty, RegexOptions.Compiled);
		_logger.LogDebug("XmlEncodeAsync(str={Str}): str → {Str1}", str, str);

		_logger.LogDebug("<<< Exiting XmlEncodeAsync(str={Str})", str);
        return await XmlEncodeAsIsAsync(str);
    }

    /// <summary>
    /// XML Encode as is
    /// </summary>
    /// <param name="str">String</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the encoded string
    /// </returns>
    public static async Task<string> XmlEncodeAsIsAsync(string str)
    {
		_logger.LogDebug(">>> Entering XmlEncodeAsIsAsync(str={Str})", str);
        if (str == null)
            return null;

        var settings = new XmlWriterSettings
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        await using var sw = new StringWriter();
		_logger.LogDebug("XmlEncodeAsIsAsync(str={Str}): sw → {Sw}", str, sw);
        await using var xwr = XmlWriter.Create(sw, settings);
		_logger.LogDebug("XmlEncodeAsIsAsync(str={Str}): xwr → {Xwr}", str, xwr);
        await xwr.WriteStringAsync(str);
        await xwr.FlushAsync();

		_logger.LogDebug("<<< Exiting XmlEncodeAsIsAsync(str={Str})", str);
        return sw.ToString();
    }

    /// <summary>
    /// Decodes an attribute
    /// </summary>
    /// <param name="str">Attribute</param>
    /// <returns>Decoded attribute</returns>
    public static string XmlDecode(string str)
    {
		_logger.LogDebug(">>> Entering XmlDecode(str={Str})", str);
        var sb = new StringBuilder(str);

		_logger.LogDebug("XmlDecode(str={Str}): sb → {Sb}", str, sb);
        var rez = sb.Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").ToString();
		_logger.LogDebug("XmlDecode(str={Str}): rez → {Rez}", str, rez);

		_logger.LogDebug("<<< Exiting XmlDecode(str={Str})", str);
        return rez;
    }

    /// <summary>
    /// Serializes a datetime
    /// </summary>
    /// <param name="dateTime">Datetime</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the serialized datetime
    /// </returns>
    public static async Task<string> SerializeDateTimeAsync(DateTime dateTime)
    {
		_logger.LogDebug(">>> Entering SerializeDateTimeAsync(dateTime={DateTime})", dateTime);
        var xmlS = new XmlSerializer(typeof(DateTime));
        var sb = new StringBuilder();
		_logger.LogDebug("SerializeDateTimeAsync(dateTime={DateTime}): sb → {Sb}", dateTime, sb);
        await using var sw = new StringWriter(sb);
		_logger.LogDebug("SerializeDateTimeAsync(dateTime={DateTime}): sw → {Sw}", dateTime, sw);
		_logger.LogDebug("SerializeDateTimeAsync(dateTime={DateTime}): xmlS → {XmlS}", dateTime, xmlS);
        xmlS.Serialize(sw, dateTime);

		_logger.LogDebug("<<< Exiting SerializeDateTimeAsync(dateTime={DateTime})", dateTime);
        return sb.ToString();
    }

    #endregion
}