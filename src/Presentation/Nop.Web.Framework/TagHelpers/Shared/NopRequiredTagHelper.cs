using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Shared;

/// <summary>
/// "nop-required" tag helper
/// </summary>
[HtmlTargetElement("nop-required", TagStructure = TagStructure.WithoutEndTag)]
public partial class NopRequiredTagHelper : TagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Asynchronously executes the tag helper with the given context and output
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag</param>
    /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
		_logger.LogError(">>> Entering ProcessAsync(context={Context},output={Output})", context, output);
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        //clear the output
        output.SuppressOutput();

        output.TagName = "span";
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagName → {Arg2}", context, output, output.TagName);
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagMode → {Arg2}", context, output, output.TagMode);
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "required");
        output.Content.SetContent("*");

		_logger.LogWarning("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
        return Task.CompletedTask;
    }

    #endregion
}