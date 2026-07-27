using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Shared;

/// <summary>
/// "nop-antiforgery-token" tag helper
/// </summary>
[HtmlTargetElement("nop-antiforgery-token", TagStructure = TagStructure.WithoutEndTag)]
public partial class NopAntiForgeryTokenTagHelper : TagHelper
{
    #region Properties

    protected IHtmlGenerator Generator { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion

    #region Ctor

    public NopAntiForgeryTokenTagHelper(IHtmlGenerator generator)
    {
		logger.LogDebug(">>> Entering NopAntiForgeryTokenTagHelper(generator={Generator})", generator);
        Generator = generator;
		logger.LogDebug("<<< Exiting NopAntiForgeryTokenTagHelper(generator={Generator})", generator);
		logger.LogDebug("NopAntiForgeryTokenTagHelper(generator={Generator}): Generator → {Generator1}", generator, Generator);
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
		logger.LogDebug(">>> Entering ProcessAsync(context={Context},output={Output})", context, output);
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        //clear the output
        output.SuppressOutput();

        //generate antiforgery
        var antiforgeryTag = Generator.GenerateAntiforgery(ViewContext);
        if (antiforgeryTag != null)
			logger.LogDebug("ProcessAsync(context={Context},output={Output}): antiforgeryTag → {AntiforgeryTag}", context, output, antiforgeryTag);
            output.Content.SetHtmlContent(antiforgeryTag);

		logger.LogDebug("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
        return Task.CompletedTask;
    }

    #endregion
}