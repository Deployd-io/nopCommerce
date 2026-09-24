using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-delete-confirmation" tag helper
/// </summary>
[HtmlTargetElement("nop-delete-confirmation", Attributes = MODEL_ID_ATTRIBUTE_NAME + "," + BUTTON_ID_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopDeleteConfirmationTagHelper : TagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering NopDeleteConfirmationTagHelper(generator={Generator},htmlHelper={HtmlHelper})", generator, htmlHelper);
        Generator = generator;
        _htmlHelper = htmlHelper;
		_logger.LogDebug("NopDeleteConfirmationTagHelper(generator={Generator},htmlHelper={HtmlHelper}): Generator → {Generator1}", generator, htmlHelper, Generator);
    _logger.LogWarning("<<< Exiting NopDeleteConfirmationTagHelper(generator={Generator},htmlHelper={HtmlHelper})", generator, htmlHelper);
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

        //contextualize IHtmlHelper
        var viewContextAware = _htmlHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        if (string.IsNullOrEmpty(Action))
            Action = "Delete";
			_logger.LogError("ProcessAsync(context={Context},output={Output}): Action → {Action}", context, output, Action);
			_logger.LogDebug("ProcessAsync(context={Context},output={Output}): Action → {Action}", context, output, Action);

        var modelName = _htmlHelper.ViewData.ModelMetadata.ModelType.Name.ToLowerInvariant();
		_logger.LogError("ProcessAsync(context={Context},output={Output}): modelName → {ModelName}", context, output, modelName);
        if (!string.IsNullOrEmpty(Action))
            modelName += "-" + Action;
        var modalId = await new HtmlString(modelName + "-delete-confirmation").RenderHtmlContentAsync();
		_logger.LogError("ProcessAsync(context={Context},output={Output}): modalId → {ModalId}", context, output, modalId);

        var deleteConfirmationModel = new DeleteConfirmationModel
        {
            Id = ModelId,
            ControllerName = _htmlHelper.ViewContext.RouteData.Values["controller"].ToString(),
            ActionName = Action,
            WindowId = modalId
        };

        //tag details
        output.TagName = "div";
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagName → {Arg2}", context, output, output.TagName);
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagMode → {Arg2}", context, output, output.TagMode);
        output.TagMode = TagMode.StartTagAndEndTag;

        output.Attributes.Add("id", modalId);
        output.Attributes.Add("class", "modal fade");
        output.Attributes.Add("tabindex", "-1");
        output.Attributes.Add("role", "dialog");
        output.Attributes.Add("aria-labelledby", $"{modalId}-title");

        var partialView = await _htmlHelper.PartialAsync("Delete", deleteConfirmationModel);
		_logger.LogError("ProcessAsync(context={Context},output={Output}): partialView → {PartialView}", context, output, partialView);
        output.Content.SetHtmlContent(partialView);

        //modal script
        var script = new TagBuilder("script");
		_logger.LogError("ProcessAsync(context={Context},output={Output}): script → {Script}", context, output, script);
        script.InnerHtml.AppendHtml(
            "$(function() {" +
            $"$('#{ButtonId}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{modalId}\")" +
            "});");
        var scriptTag = await script.RenderHtmlContentAsync();
		_logger.LogError("ProcessAsync(context={Context},output={Output}): scriptTag → {ScriptTag}", context, output, scriptTag);
        output.PostContent.SetHtmlContent(scriptTag);
		_logger.LogWarning("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
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