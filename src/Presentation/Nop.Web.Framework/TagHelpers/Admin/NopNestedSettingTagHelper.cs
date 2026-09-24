using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Web.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-nested-setting" tag helper
/// </summary>
[HtmlTargetElement("nop-nested-setting", Attributes = FOR_ATTRIBUTE_NAME)]
public partial class NopNestedSettingTagHelper : TagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    protected const string FOR_ATTRIBUTE_NAME = "asp-for";
    protected const string IS_CONDITION_INVERT = "is-condition-invert";
    protected const string DISABLE_AUTOGENERATION = "disable-auto-generation";

    #endregion

    #region Ctor

    public NopNestedSettingTagHelper(IHtmlGenerator generator)
    {
		_logger.LogError(">>> Entering NopNestedSettingTagHelper(generator={Generator})", generator);
        Generator = generator;
		_logger.LogDebug("NopNestedSettingTagHelper(generator={Generator}): Generator → {Generator1}", generator, Generator);
    _logger.LogWarning("<<< Exiting NopNestedSettingTagHelper(generator={Generator})", generator);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Asynchronously executes the tag helper with the given context and output
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag</param>
    /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
		_logger.LogError(">>> Entering ProcessAsync(context={Context},output={Output})", context, output);
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        var parentSettingName = For.Name;
        var jsConsistentParentSettingName = parentSettingName.Replace('.', '_');
		_logger.LogError("ProcessAsync(context={Context},output={Output}): jsConsistentParentSettingName → {JsConsistentParentSettingName}", context, output, jsConsistentParentSettingName);

        var random = CommonHelper.GenerateRandomInteger();
		_logger.LogError("ProcessAsync(context={Context},output={Output}): random → {Random}", context, output, random);
        var nestedSettingId = $"nestedSetting{random}";
		_logger.LogError("ProcessAsync(context={Context},output={Output}): nestedSettingId → {NestedSettingId}", context, output, nestedSettingId);
        var parentSettingId = $"parentSetting{random}";
		_logger.LogError("ProcessAsync(context={Context},output={Output}): parentSettingId → {ParentSettingId}", context, output, parentSettingId);

        //tag details
        output.TagName = "div";
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagName → {Arg2}", context, output, output.TagName);
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagMode → {Arg2}", context, output, output.TagMode);
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.Add("class", "nested-setting");

        if (context.AllAttributes.ContainsName("id"))
            nestedSettingId = context.AllAttributes["id"].Value.ToString();
        output.Attributes.Add("id", nestedSettingId);

        //use javascript
        var script = new TagBuilder("script");
		_logger.LogError("ProcessAsync(context={Context},output={Output}): script → {Script}", context, output, script);

        var isNot = IsConditionInvert ? "!" : "";
		_logger.LogError("ProcessAsync(context={Context},output={Output}): isNot → {IsNot}", context, output, isNot);

        script.InnerHtml.AppendHtml(
            "$(function() {" +
            $"initNestedSetting('{parentSettingName}', '{parentSettingId}', '{nestedSettingId}');"
        );

        if (!DisableAutoGeneration)
            script.InnerHtml.AppendHtml(
                $"$('#{jsConsistentParentSettingName}').click(toggle_{jsConsistentParentSettingName});" +
                $"toggle_{jsConsistentParentSettingName}();"
            );

        script.InnerHtml.AppendHtml("});");

        if (!DisableAutoGeneration)
            script.InnerHtml.AppendHtml(
                $"function toggle_{jsConsistentParentSettingName}() " + "{" +
                $"if ({isNot}$('#{jsConsistentParentSettingName}').is(':checked')) " + "{" +
                $"$('#{nestedSettingId}').showElement();" +
                "} else {" +
                $"$('#{nestedSettingId}').hideElement();" +
                "}" +
                "}"
            );

        var scriptTag = await script.RenderHtmlContentAsync();
		_logger.LogError("ProcessAsync(context={Context},output={Output}): scriptTag → {ScriptTag}", context, output, scriptTag);
        output.PreContent.SetHtmlContent(scriptTag);
		_logger.LogWarning("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
    }

    #endregion

    #region Properties

    protected IHtmlGenerator Generator { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model
    /// </summary>
    [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// Is condition inverted
    /// </summary>
    [HtmlAttributeName(IS_CONDITION_INVERT)]
    public bool IsConditionInvert { get; set; }

    /// <summary>
    /// Disable auto-generation js script
    /// </summary>
    [HtmlAttributeName(DISABLE_AUTOGENERATION)]
    public bool DisableAutoGeneration { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}