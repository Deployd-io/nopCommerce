using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;

namespace Nop.Web.Framework.TagHelpers.Public;

/// <summary>
/// "nop-bb-code-editor" tag helper
/// </summary>
[HtmlTargetElement("nop-bb-code-editor", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopBBCodeEditorTagHelper : TagHelper
{
    #region Constants

    protected const string FOR_ATTRIBUTE_NAME = "asp-for";

    #endregion

    #region Fields

    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public NopBBCodeEditorTagHelper(IWebHelper webHelper)
    {
		logger.LogDebug(">>> Entering NopBBCodeEditorTagHelper(webHelper={WebHelper})", webHelper);
        _webHelper = webHelper;
		logger.LogDebug("<<< Exiting NopBBCodeEditorTagHelper(webHelper={WebHelper})", webHelper);
		logger.LogDebug("NopBBCodeEditorTagHelper(webHelper={WebHelper}): _webHelper → {_webHelper}", webHelper, _webHelper);
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

        output.TagName = "div";
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagName → {Arg2}", context, output, output.TagName);
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagMode → {Arg2}", context, output, output.TagMode);
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.Add("class", "bb-code-editor-wrapper");

        var storeLocation = _webHelper.GetStoreLocation();

		logger.LogDebug("ProcessAsync(context={Context},output={Output}): storeLocation → {StoreLocation}", context, output, storeLocation);
        var bbEditorWebRoot = $"{storeLocation}js/";

		logger.LogDebug("ProcessAsync(context={Context},output={Output}): bbEditorWebRoot → {BbEditorWebRoot}", context, output, bbEditorWebRoot);
        var script1 = new TagBuilder("script");
        script1.Attributes.Add("src", $"{storeLocation}js/bbeditor/ed.js");
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): script1 → {Script1}", context, output, script1);

        var script2 = new TagBuilder("script");
        script2.Attributes.Add("language", "javascript");
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): script2 → {Script2}", context, output, script2);
        script2.InnerHtml.AppendHtml($"edToolbar('{For.Name}','{bbEditorWebRoot}');");

        output.Content.AppendHtml(script1);
        output.Content.AppendHtml(script2);

		logger.LogDebug("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// An expression to be evaluated against the current model
    /// </summary>
    [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}