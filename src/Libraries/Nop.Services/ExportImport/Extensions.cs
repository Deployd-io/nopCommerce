using Microsoft.Extensions.Logging;
using System.Xml;

namespace Nop.Services.ExportImport;

/// <summary>
/// Extensions
/// </summary>
public static partial class Extensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Write string async
    /// </summary>
    /// <param name="xmlWriter">XML writer</param>
    /// <param name="nodeName">Node name</param>
    /// <param name="nodeValue">Node value</param>
    /// <param name="ignore">Ignore</param>
    /// <param name="defaulValue">Default value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task WriteStringAsync(this XmlWriter xmlWriter, string nodeName, object nodeValue, bool ignore = false, string defaulValue = "")
    {
		_logger.LogDebug(">>> Entering WriteStringAsync(xmlWriter={XmlWriter},nodeName={NodeName},nodeValue={NodeValue},ignore={Ignore},defaulValue={DefaulValue})", xmlWriter, nodeName, nodeValue, ignore, defaulValue);
        if (ignore)
            return;

        await xmlWriter.WriteElementStringAsync(null, nodeName, null, nodeValue?.ToString() ?? defaulValue);
		_logger.LogDebug("<<< Exiting WriteStringAsync(xmlWriter={XmlWriter},nodeName={NodeName},nodeValue={NodeValue},ignore={Ignore},defaulValue={DefaulValue})", xmlWriter, nodeName, nodeValue, ignore, defaulValue);
    }

    /// <summary>
    /// Write start element async
    /// </summary>
    /// <param name="xmlWriter">XML writer</param>
    /// <param name="nodeName">Node name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task WriteStartElementAsync(this XmlWriter xmlWriter, string nodeName)
    {
		_logger.LogDebug(">>> Entering WriteStartElementAsync(xmlWriter={XmlWriter},nodeName={NodeName})", xmlWriter, nodeName);
        await xmlWriter.WriteStartElementAsync(null, nodeName, null);
		_logger.LogDebug("<<< Exiting WriteStartElementAsync(xmlWriter={XmlWriter},nodeName={NodeName})", xmlWriter, nodeName);
    }

    /// <summary>
    /// Write attribute string async
    /// </summary>
    /// <param name="xmlWriter">XML writer</param>
    /// <param name="nodeName">Node name</param>
    /// <param name="nodeValue">Node value</param>
    /// <param name="defaulValue">Default value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task WriteAttributeStringAsync(this XmlWriter xmlWriter, string nodeName, object nodeValue, string defaulValue = "")
    {
		_logger.LogDebug(">>> Entering WriteAttributeStringAsync(xmlWriter={XmlWriter},nodeName={NodeName},nodeValue={NodeValue},defaulValue={DefaulValue})", xmlWriter, nodeName, nodeValue, defaulValue);
        await xmlWriter.WriteAttributeStringAsync(null, nodeName, null, nodeValue?.ToString() ?? defaulValue);
		_logger.LogDebug("<<< Exiting WriteAttributeStringAsync(xmlWriter={XmlWriter},nodeName={NodeName},nodeValue={NodeValue},defaulValue={DefaulValue})", xmlWriter, nodeName, nodeValue, defaulValue);
    }

    /// <summary>
    /// Write element string async
    /// </summary>
    /// <param name="xmlWriter">XML writer</param>
    /// <param name="nodeName">Node name</param>
    /// <param name="nodeValue">Node value</param>
    /// <param name="defaulValue">Default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    public static async Task WriteElementStringAsync(this XmlWriter xmlWriter, string nodeName, object nodeValue, string defaulValue = "")
    {
		_logger.LogDebug(">>> Entering WriteElementStringAsync(xmlWriter={XmlWriter},nodeName={NodeName},nodeValue={NodeValue},defaulValue={DefaulValue})", xmlWriter, nodeName, nodeValue, defaulValue);
        await xmlWriter.WriteElementStringAsync(null, nodeName, null, nodeValue?.ToString() ?? defaulValue);
		_logger.LogDebug("<<< Exiting WriteElementStringAsync(xmlWriter={XmlWriter},nodeName={NodeName},nodeValue={NodeValue},defaulValue={DefaulValue})", xmlWriter, nodeName, nodeValue, defaulValue);
    }
}