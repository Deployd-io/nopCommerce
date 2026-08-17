using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers;

/// <summary>
/// Tag helper extensions
/// </summary>
public static class TagHelperExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Get string value from tag helper output
    /// </summary>
    /// <param name="output">An information associated with the current HTML tag</param>
    /// <param name="attributeName">Name of the attribute</param>
    /// <returns>Matching name</returns>
    public static async Task<string> GetAttributeValueAsync(this TagHelperOutput output, string attributeName)
    {
	_logger.LogDebug(">>> Entering GetAttributeValueAsync(output={Output},attributeName={AttributeName})", output, attributeName);

        if (string.IsNullOrEmpty(attributeName) || !output.Attributes.TryGetAttribute(attributeName, out var attr))
            return null;

        if (attr.Value is string stringValue)
		_logger.LogDebug("<<< Exiting GetAttributeValueAsync(output={Output},attributeName={AttributeName})", output, attributeName);
            return stringValue;

        return attr.Value switch
        {
            HtmlString htmlString => htmlString.ToString(),
            IHtmlContent content => await content.RenderHtmlContentAsync(),
            _ => default
        };
    }

    /// <summary>
    /// Get attributes from tag helper output as collection of key/string value pairs
    /// </summary>
    /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
    /// <returns>Collection of key/string value pairs</returns>
    public static async Task<IDictionary<string, string>> GetAttributeDictionaryAsync(this TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        var result = new Dictionary<string, string>();

        if (!output.Attributes.Any())
            return result;

        foreach (var attrName in output.Attributes.Select(x => x.Name).Distinct())
        {
            result.Add(attrName, await output.GetAttributeValueAsync(attrName));
        }

        return result;
    }

    #endregion
}