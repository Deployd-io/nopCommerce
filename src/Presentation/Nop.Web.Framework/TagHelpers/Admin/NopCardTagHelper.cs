using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Models.Cms;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-card" tag helper
/// </summary>
[HtmlTargetElement("nop-card", Attributes = NAME_ATTRIBUTE_NAME)]
public partial class NopCardTagHelper : TagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    protected const string NAME_ATTRIBUTE_NAME = "asp-name";
    protected const string TITLE_ATTRIBUTE_NAME = "asp-title";
    protected const string HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME = "asp-hide-block-attribute-name";
    protected const string IS_HIDE_ATTRIBUTE_NAME = "asp-hide";
    protected const string IS_ADVANCED_ATTRIBUTE_NAME = "asp-advanced";
    protected const string CARD_ICON_ATTRIBUTE_NAME = "asp-icon";


    #endregion

    #region Fields

    protected readonly IViewComponentHelper _viewComponentHelper;
    protected readonly IWidgetModelFactory _widgetModelFactory;

    #endregion

    #region Ctor

    public NopCardTagHelper(IViewComponentHelper viewComponentHelper, IWidgetModelFactory widgetModelFactory)
    {
		_logger.LogError(">>> Entering NopCardTagHelper(viewComponentHelper={ViewComponentHelper},widgetModelFactory={WidgetModelFactory})", viewComponentHelper, widgetModelFactory);
        _viewComponentHelper = viewComponentHelper;
        _widgetModelFactory = widgetModelFactory;
		_logger.LogDebug("NopCardTagHelper(viewComponentHelper={ViewComponentHelper},widgetModelFactory={WidgetModelFactory}): _widgetModelFactory → {_widgetModelFactory}", viewComponentHelper, widgetModelFactory, _widgetModelFactory);
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
        var viewContextAware = _viewComponentHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        //create card
        var card = new TagBuilder("div")
        {
            Attributes =
            {
                new KeyValuePair<string, string>("id", Name),
                new KeyValuePair<string, string>("data-card-name", Name),
            }
        };
        card.AddCssClass("card card-secondary card-outline");
        if (context.AllAttributes.ContainsName(IS_ADVANCED_ATTRIBUTE_NAME)
            && context.AllAttributes[IS_ADVANCED_ATTRIBUTE_NAME].Value.Equals(true))
        {
            card.AddCssClass("advanced-setting");
        }
        card.Attributes.Add("data-hideAttribute", context.AllAttributes[HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME].Value.ToString());

        if (context.AllAttributes[IS_HIDE_ATTRIBUTE_NAME].Value.Equals(true))
        {
            card.AddCssClass("collapsed-card");
        }

        //create card heading and append title and icon to it
        var cardHeading = new TagBuilder("div");
		_logger.LogError("ProcessAsync(context={Context},output={Output}): cardHeading → {CardHeading}", context, output, cardHeading);
        cardHeading.AddCssClass("card-header with-border clearfix");

        if (context.AllAttributes.ContainsName(CARD_ICON_ATTRIBUTE_NAME))
        {
            var cardIcon = new TagBuilder("i");
			_logger.LogError("ProcessAsync(context={Context},output={Output}): cardIcon → {CardIcon}", context, output, cardIcon);
            cardIcon.AddCssClass(context.AllAttributes[CARD_ICON_ATTRIBUTE_NAME].Value.ToString());
            var iconContainer = new TagBuilder("div");
			_logger.LogError("ProcessAsync(context={Context},output={Output}): iconContainer → {IconContainer}", context, output, iconContainer);
            iconContainer.AddCssClass("card-title");
            iconContainer.InnerHtml.AppendHtml(cardIcon);
            iconContainer.InnerHtml.AppendHtml($"{context.AllAttributes[TITLE_ATTRIBUTE_NAME].Value}");
            cardHeading.InnerHtml.AppendHtml(iconContainer);
        }

        var collapseIcon = new TagBuilder("i");
		_logger.LogError("ProcessAsync(context={Context},output={Output}): collapseIcon → {CollapseIcon}", context, output, collapseIcon);
        collapseIcon.AddCssClass("fa");
        collapseIcon.AddCssClass("toggle-icon");
        collapseIcon.AddCssClass(context.AllAttributes[IS_HIDE_ATTRIBUTE_NAME].Value.Equals(true) ? "fa-plus" : "fa-minus");

        var cardToolContainer = new TagBuilder("div");
		_logger.LogError("ProcessAsync(context={Context},output={Output}): cardToolContainer → {CardToolContainer}", context, output, cardToolContainer);
        cardToolContainer.AddCssClass("card-tools float-right");
        var cardbtnContainer = new TagBuilder("button");
		_logger.LogError("ProcessAsync(context={Context},output={Output}): cardbtnContainer → {CardbtnContainer}", context, output, cardbtnContainer);

        cardbtnContainer.AddCssClass("btn btn-tool");
        cardbtnContainer.MergeAttribute("type", "button");
        cardbtnContainer.MergeAttribute("data-card-widget", "collapse");
        cardbtnContainer.InnerHtml.AppendHtml(collapseIcon);

        cardToolContainer.InnerHtml.AppendHtml(cardbtnContainer);
        cardHeading.InnerHtml.AppendHtml(cardToolContainer);

        //add heading and container to card
        card.InnerHtml.AppendHtml(cardHeading);

        var modelsBefore = await _widgetModelFactory.PrepareRenderWidgetModelAsync(AdminWidgetZones.CardBefore, new WidgetNopCardModel { CardName = Name });
		_logger.LogError("ProcessAsync(context={Context},output={Output}): modelsBefore → {ModelsBefore}", context, output, modelsBefore);

        foreach (var model in modelsBefore)
        {
            var content = await _viewComponentHelper.InvokeAsync(model.WidgetViewComponent, model.WidgetViewComponentArguments);
			_logger.LogError("ProcessAsync(context={Context},output={Output}): content → {Content}", context, output, content);
            card.InnerHtml.AppendHtml(content);
        }

        var childContent = await output.GetChildContentAsync();
		_logger.LogError("ProcessAsync(context={Context},output={Output}): childContent → {ChildContent}", context, output, childContent);
        card.InnerHtml.AppendHtml(childContent.GetContent());

        var modelsAfter = await _widgetModelFactory.PrepareRenderWidgetModelAsync(AdminWidgetZones.CardAfter, new WidgetNopCardModel { CardName = Name });
		_logger.LogError("ProcessAsync(context={Context},output={Output}): modelsAfter → {ModelsAfter}", context, output, modelsAfter);

        foreach (var model in modelsAfter)
        {
            var content = await _viewComponentHelper.InvokeAsync(model.WidgetViewComponent, model.WidgetViewComponentArguments);
            card.InnerHtml.AppendHtml(content);
        }

        output.Content.AppendHtml(await card.RenderHtmlContentAsync());
		_logger.LogWarning("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Title of the card
    /// </summary>
    [HtmlAttributeName(TITLE_ATTRIBUTE_NAME)]
    public string Title { get; set; }

    /// <summary>
    /// Name of the card
    /// </summary>
    [HtmlAttributeName(NAME_ATTRIBUTE_NAME)]
    public string Name { get; set; }

    /// <summary>
    /// Name of the hide attribute of the card
    /// </summary>
    [HtmlAttributeName(HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME)]
    public string HideBlockAttributeName { get; set; }

    /// <summary>
    /// Indicates whether a block is hidden or not
    /// </summary>
    [HtmlAttributeName(IS_HIDE_ATTRIBUTE_NAME)]
    public bool IsHide { get; set; }

    /// <summary>
    /// Indicates whether a card is advanced or not
    /// </summary>
    [HtmlAttributeName(IS_ADVANCED_ATTRIBUTE_NAME)]
    public bool IsAdvanced { get; set; }

    /// <summary>
    /// Card icon
    /// </summary>
    [HtmlAttributeName(CARD_ICON_ATTRIBUTE_NAME)]
    public string CardIconIsAdvanced { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}