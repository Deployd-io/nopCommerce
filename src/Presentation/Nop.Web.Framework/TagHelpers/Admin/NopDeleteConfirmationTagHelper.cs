using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-delete-confirmation" tag helper
/// </summary>
[HtmlTargetElement("nop-delete-confirmation", Attributes = MODEL_ID_ATTRIBUTE_NAME + "," + BUTTON_ID_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopDeleteConfirmationTagHelper : TagHelper
{
    #region Constants

    protected const string MODEL_ID_ATTRIBUTE_NAME = "asp-model-id";
    protected const string BUTTON_ID_ATTRIBUTE_NAME = "asp-button-id";
    protected const string ACTION_ATTRIBUTE_NAME = "asp-action";

    #endregion

    #region Fields

    protected readonly IHtmlHelper _htmlHelper;

    #endregion

    #region Ctor

    public NopDeleteConfirmationTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
    {
		logger.LogDebug(">>> Entering NopDeleteConfirmationTagHelper(generator={Generator},htmlHelper={HtmlHelper})", generator, htmlHelper);
        Generator = generator;
        _htmlHelper = htmlHelper;
		logger.LogDebug("<<< Exiting NopDeleteConfirmationTagHelper(generator={Generator},htmlHelper={HtmlHelper})", generator, htmlHelper);
		logger.LogDebug("NopDeleteConfirmationTagHelper(generator={Generator},htmlHelper={HtmlHelper}): _htmlHelper → {_htmlHelper}", generator, htmlHelper, _htmlHelper);
		logger.LogDebug("NopDeleteConfirmationTagHelper(generator={Generator},htmlHelper={HtmlHelper}): Generator → {Generator1}", generator, htmlHelper, Generator);
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

        if (string.IsNullOrEmpty(Action))
            Action = "Delete";
			logger.LogDebug("ProcessAsync(context={Context},output={Output}): Action → {Action}", context, output, Action);

        var modelName = _htmlHelper.ViewData.ModelMetadata.ModelType.Name.ToLowerInvariant();
        if (!string.IsNullOrEmpty(Action))
			logger.LogDebug("ProcessAsync(context={Context},output={Output}): modelName → {ModelName}", context, output, modelName);
            modelName += "-" + Action;
        var modalId = await new HtmlString(modelName + "-delete-confirmation").RenderHtmlContentAsync();

		logger.LogDebug("ProcessAsync(context={Context},output={Output}): modalId → {ModalId}", context, output, modalId);
        var deleteConfirmationModel = new DeleteConfirmationModel
        {
            Id = ModelId,
            ControllerName = _htmlHelper.ViewContext.RouteData.Values["controller"].ToString(),
            ActionName = Action,
            WindowId = modalId
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

        var partialView = await _htmlHelper.PartialAsync("Delete", deleteConfirmationModel);
        output.Content.SetHtmlContent(partialView);
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): partialView → {PartialView}", context, output, partialView);

        //modal script
        var script = new TagBuilder("script");
        script.InnerHtml.AppendHtml(
            "$(function() {" +
            $"$('#{ButtonId}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{modalId}\")" +
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
    /// Model identifier
    /// </summary>
    [HtmlAttributeName(MODEL_ID_ATTRIBUTE_NAME)]
    public string ModelId { get; set; }

    /// <summary>
    /// Button identifier
    /// </summary>
    [HtmlAttributeName(BUTTON_ID_ATTRIBUTE_NAME)]
    public string ButtonId { get; set; }

    /// <summary>
    /// Delete action name
    /// </summary>
    [HtmlAttributeName(ACTION_ATTRIBUTE_NAME)]
    public string Action { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}
