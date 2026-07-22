using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-tabs" tag helper
/// </summary>
[HtmlTargetElement("nop-tabs", Attributes = ID_ATTRIBUTE_NAME)]
public partial class NopTabsTagHelper : TagHelper
{
    #region Constants

    protected const string ID_ATTRIBUTE_NAME = "id";
    protected const string TAB_NAME_TO_SELECT_ATTRIBUTE_NAME = "asp-tab-name-to-select";
    protected const string RENDER_SELECTED_TAB_INPUT_ATTRIBUTE_NAME = "asp-render-selected-tab-input";

    #endregion

    #region Fields

    protected readonly IHtmlHelper _htmlHelper;

    #endregion

    #region Ctor

    public NopTabsTagHelper(IHtmlHelper htmlHelper)
    {
		logger.LogDebug(">>> Entering NopTabsTagHelper(htmlHelper={HtmlHelper})", htmlHelper);
        _htmlHelper = htmlHelper;
		logger.LogDebug("<<< Exiting NopTabsTagHelper(htmlHelper={HtmlHelper})", htmlHelper);
		logger.LogDebug("NopTabsTagHelper(htmlHelper={HtmlHelper}): _htmlHelper → {_htmlHelper}", htmlHelper, _htmlHelper);
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

        //create context item
        var tabContext = new List<NopTabContextItem>();
        context.Items.Add(typeof(NopTabsTagHelper), tabContext);
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): tabContext → {TabContext}", context, output, tabContext);

        //get tab name which should be selected
        //first try get tab name from query
        var tabNameToSelect = ViewContext.HttpContext.Request.Query["tabNameToSelect"];

		logger.LogDebug("ProcessAsync(context={Context},output={Output}): tabNameToSelect → {TabNameToSelect}", context, output, tabNameToSelect);
        //then from attribute
        if (!string.IsNullOrEmpty(TabNameToSelect))
            tabNameToSelect = TabNameToSelect;

        //then save tab name in tab context to access it in tab item
        if (!string.IsNullOrEmpty(tabNameToSelect))
            context.Items.Add("tabNameToSelect", tabNameToSelect);

        //execute child tag helpers
        await output.GetChildContentAsync();

        //tabs title
        var tabsTitlediv = new TagBuilder("div");
        tabsTitlediv.AddCssClass("card-header p-0 pt-1 border-bottom-0");
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): tabsTitlediv → {TabsTitlediv}", context, output, tabsTitlediv);

        var tabsTitle = new TagBuilder("ul");
        tabsTitle.AddCssClass("nav");
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): tabsTitle → {TabsTitle}", context, output, tabsTitle);
        tabsTitle.AddCssClass("nav-tabs");
        tabsTitle.Attributes.Add("id", "custom-content-above-tab");
        tabsTitle.Attributes.Add("role", "tablist");

        //tabs content
        var tabsContentout = new TagBuilder("div");
        tabsContentout.AddCssClass("card-body");
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): tabsContentout → {TabsContentout}", context, output, tabsContentout);

        var tabsContent = new TagBuilder("div");
        tabsContent.AddCssClass("tab-content");
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): tabsContent → {TabsContent}", context, output, tabsContent);

        var outputiner = new TagBuilder("div");
        outputiner.AddCssClass("card card-primary card-outline card-outline-tabs");
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): outputiner → {Outputiner}", context, output, outputiner);

        foreach (var tabItem in tabContext)
        {
            tabsTitle.InnerHtml.AppendHtml(tabItem.Title);
            tabsContent.InnerHtml.AppendHtml(tabItem.Content);
        }

        tabsTitlediv.InnerHtml.AppendHtml(await tabsTitle.RenderHtmlContentAsync());
        tabsContentout.InnerHtml.AppendHtml(await tabsContent.RenderHtmlContentAsync());
        //append data
        output.Content.AppendHtml(await tabsTitlediv.RenderHtmlContentAsync());
        output.Content.AppendHtml(await tabsContentout.RenderHtmlContentAsync());

        _ = bool.TryParse(RenderSelectedTabInput, out var renderSelectedTabInput);
        if (string.IsNullOrEmpty(RenderSelectedTabInput) || renderSelectedTabInput)
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): _ → {_}", context, output, _);
        {
            //render input contains selected tab name
            var selectedTabInput = new TagBuilder("input");
            selectedTabInput.Attributes.Add("type", "hidden");
			logger.LogDebug("ProcessAsync(context={Context},output={Output}): selectedTabInput → {SelectedTabInput}", context, output, selectedTabInput);
            selectedTabInput.Attributes.Add("id", "selected-tab-name");
            selectedTabInput.Attributes.Add("name", "selected-tab-name");
            selectedTabInput.Attributes.Add("value", _htmlHelper.GetSelectedTabName());
            output.PreContent.SetHtmlContent(await selectedTabInput.RenderHtmlContentAsync());

            //render tabs script
            if (output.Attributes.ContainsName("id"))
            {
                var script = new TagBuilder("script");
                script.InnerHtml.AppendHtml(
                    "$(function() {" +
                    "bindBootstrapTabSelectEvent('" + output.Attributes["id"].Value + "', 'selected-tab-name');" +
                    "});");
					logger.LogDebug("ProcessAsync(context={Context},output={Output}): script → {Script}", context, output, script);
                var scriptTag = await script.RenderHtmlContentAsync();
                output.PostContent.SetHtmlContent(scriptTag);
				logger.LogDebug("ProcessAsync(context={Context},output={Output}): scriptTag → {ScriptTag}", context, output, scriptTag);
            }
        }

        output.TagName = "div";
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagName → {Arg2}", context, output, output.TagName);

        var itemClass = "card card-primary card-outline card-outline-tabs nav-tabs-custom";
        //merge classes
		logger.LogDebug("ProcessAsync(context={Context},output={Output}): itemClass → {ItemClass}", context, output, itemClass);
        var classValue = output.Attributes.ContainsName("class")
            ? $"{output.Attributes["class"].Value} {itemClass}"
            : itemClass;

        output.Attributes.SetAttribute("class", classValue);
		logger.LogDebug("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Name of the tab which should be selected
    /// </summary>
    [HtmlAttributeName(TAB_NAME_TO_SELECT_ATTRIBUTE_NAME)]
    public string TabNameToSelect { set; get; }

    /// <summary>
    /// Indicates whether the tab is default
    /// </summary>
    [HtmlAttributeName(RENDER_SELECTED_TAB_INPUT_ATTRIBUTE_NAME)]
    public string RenderSelectedTabInput { set; get; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}
