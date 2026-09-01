using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-doc-reference" tag helper
/// </summary>
[HtmlTargetElement("nop-doc-reference", Attributes = STRING_RESOURCE_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopDocReferenceTagHelper : TagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    protected const string STRING_RESOURCE_ATTRIBUTE_NAME = "asp-string-resource";
    protected const string ADD_WRAPPER_ATTRIBUTE_NAME = "asp-add-wrapper";

    #endregion

    #region Fields

    protected readonly AdminAreaSettings _adminAreaSettings;

    #endregion

    #region Ctor

    public NopDocReferenceTagHelper(AdminAreaSettings adminAreaSettings)
    {
		_logger.LogDebug(">>> Entering NopDocReferenceTagHelper(adminAreaSettings={AdminAreaSettings})", adminAreaSettings);
        _adminAreaSettings = adminAreaSettings;
		_logger.LogDebug("NopDocReferenceTagHelper(adminAreaSettings={AdminAreaSettings}): _adminAreaSettings → {_adminAreaSettings}", adminAreaSettings, _adminAreaSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Asynchronously executes the tag helper with the given context and output
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag</param>
    /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
		_logger.LogDebug(">>> Entering ProcessAsync(context={Context},output={Output})", context, output);
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        //clear the output
        output.SuppressOutput();

        if (_adminAreaSettings.ShowDocumentationReferenceLinks)
        {
            //add wrapper
            if (AddWrapper)
            {
                output.TagName = "div";
				_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagName → {Arg2}", context, output, output.TagName);
				_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagMode → {Arg2}", context, output, output.TagMode);
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.SetAttribute("class", "documentation-reference");
            }

            var hintHtml = $"<span>{StringResource}</span>";
			_logger.LogDebug("ProcessAsync(context={Context},output={Output}): hintHtml → {HintHtml}", context, output, hintHtml);
            output.Content.AppendHtml(hintHtml);
        }

		_logger.LogDebug("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// String resource value
    /// </summary>
    [HtmlAttributeName(STRING_RESOURCE_ATTRIBUTE_NAME)]
    public string StringResource { get; set; }

    /// <summary>
    /// Indicates whether the wrapper tag should be added
    /// </summary>
    [HtmlAttributeName(ADD_WRAPPER_ATTRIBUTE_NAME)]
    public bool AddWrapper { get; set; } = true;

    #endregion
}