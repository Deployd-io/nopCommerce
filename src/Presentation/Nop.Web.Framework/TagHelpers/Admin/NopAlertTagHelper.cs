using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-alert" tag helper
/// </summary>
[HtmlTargetElement("nop-alert", Attributes = ALERT_NAME_ID, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopAlertTagHelper : TagHelper
{
    #region Constants

    protected const string ALERT_NAME_ID = "asp-alert-id";
    protected const string ALERT_MESSAGE_NAME = "asp-alert-message";

    #endregion

    #region Fields

    protected readonly IHtmlHelper _htmlHelper;

    #endregion

    #region Ctor

    public NopAlertTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
    {
		logger.LogDebug(">>> Entering NopAlertTagHelper(generator={Generator},htmlHelper={HtmlHelper})", generator, htmlHelper);
        Generator = generator;
        _htmlHelper = htmlHelper;
		logger.LogDebug("<<< Exiting NopAlertTagHelper(generator={Generator},htmlHelper={HtmlHelper})", generator, htmlHelper);
		logger.LogDebug("NopAlertTagHelper(generator={Generator},htmlHelper={HtmlHelper}): _htmlHelper → {_htmlHelper}", generator, htmlHelper, _htmlHelper);
		logger.LogDebug("NopAlertTagHelper(generator={Generator},htmlHelper={HtmlHelper}): Generator → {Generator1}", generator, htmlHelper, Generator);
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
		logger.LogDebug(">>> Entering ProcessAsync(context={Context},output={Output})", context, output);
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        //contextualize IHtmlHelper
        var viewContextAware = _htmlHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        var modalId = await new HtmlString(AlertId + "-action-alert").RenderHtmlContentAsync();

		logger.LogDebug("ProcessAsync(context={Context},output={Output}): modalId → {ModalId}", context, output, modalId);
        var actionAlertModel = new ActionAlertModel()
        {
            AlertId = AlertId,
            WindowId = modalId,
            AlertMessage = Message
        };

        //tag details
        output.TagName = "div";
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagName → {Arg2}", context, output, output.TagName);
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagMode → {Arg2}", context, output, output.TagMode);
        output.TagMode = TagMode.StartTagAndEndTag;

        output.Attributes.Add("id", modalId);
        output.Attributes.Add("class", "modal fade");
        output.Attributes.Add("tabindex", "-1");
        output.Attributes.Add("role", "dialog");
        output.Attributes.Add("aria-labelledby", $"{modalId}-title");

        var partialView = await _htmlHelper.PartialAsync("Alert", actionAlertModel);
        output.Content.SetHtmlContent(partialView);
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): partialView → {PartialView}", context, output, partialView);

        //modal script
        var script = new TagBuilder("script");
        script.InnerHtml.AppendHtml(
            "$(function() {" +
            $"$('#{AlertId}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{modalId}\")" +
            "});");
			logger.LogDebug("ProcessAsync(context={Context},output={Output}): script → {Script}", context, output, script);
        var scriptTag = await script.RenderHtmlContentAsync();
        output.PostContent.SetHtmlContent(scriptTag);
		logger.LogDebug("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): scriptTag → {ScriptTag}", context, output, scriptTag);
    }

    #endregion

    #region Properties

    protected IHtmlGenerator Generator { get; set; }

    /// <summary>
    /// Alert identifier
    /// </summary>
    [HtmlAttributeName(ALERT_NAME_ID)]
    public string AlertId { get; set; }

    /// <summary>
    /// Additional confirm text
    /// </summary>
    [HtmlAttributeName(ALERT_MESSAGE_NAME)]
    public string Message { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}