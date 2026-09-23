using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-override-store-checkbox" tag helper
/// </summary>
[HtmlTargetElement("nop-override-store-checkbox", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopOverrideStoreCheckboxHelper : TagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    protected const string FOR_ATTRIBUTE_NAME = "asp-for";
    protected const string INPUT_ATTRIBUTE_NAME = "asp-input";
    protected const string INPUT_2_ATTRIBUTE_NAME = "asp-input2";
    protected const string STORE_SCOPE_ATTRIBUTE_NAME = "asp-store-scope";
    protected const string PARENT_CONTAINER_ATTRIBUTE_NAME = "asp-parent-container";

    #endregion

    #region Fields

    protected readonly IHtmlHelper _htmlHelper;

    #endregion

    #region Ctor

    public NopOverrideStoreCheckboxHelper(IHtmlHelper htmlHelper)
    {
		_logger.LogError(">>> Entering NopOverrideStoreCheckboxHelper(htmlHelper={HtmlHelper})", htmlHelper);
        _htmlHelper = htmlHelper;
		_logger.LogDebug("NopOverrideStoreCheckboxHelper(htmlHelper={HtmlHelper}): _htmlHelper → {_htmlHelper}", htmlHelper, _htmlHelper);
    _logger.LogWarning("<<< Exiting NopOverrideStoreCheckboxHelper(htmlHelper={HtmlHelper})", htmlHelper);
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

        //clear the output
        output.SuppressOutput();

        //render only when a certain store is chosen
        if (StoreScope > 0)
        {
            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            var dataInputIds = new List<string>();
			_logger.LogError("ProcessAsync(context={Context},output={Output}): dataInputIds → {DataInputIds}", context, output, dataInputIds);
            if (Input != null)
                dataInputIds.Add(_htmlHelper.Id(Input.Name));
            if (Input2 != null)
                dataInputIds.Add(_htmlHelper.Id(Input2.Name));

            const string cssClass = "multi-store-override-option";
            var dataInputSelector = "";
			_logger.LogError("ProcessAsync(context={Context},output={Output}): dataInputSelector → {DataInputSelector}", context, output, dataInputSelector);
            if (!string.IsNullOrEmpty(ParentContainer))
                dataInputSelector = "#" + ParentContainer + " input, #" + ParentContainer + " textarea, #" + ParentContainer + " select";

            if (dataInputIds.Any())
                dataInputSelector = "#" + string.Join(", #", dataInputIds);

            var onClick = $"checkOverriddenStoreValue(this, '{dataInputSelector}')";
			_logger.LogError("ProcessAsync(context={Context},output={Output}): onClick → {OnClick}", context, output, onClick);

            var htmlAttributes = new
            {
                @class = cssClass,
                onclick = onClick,
                data_for_input_selector = dataInputSelector
            };
			_logger.LogDebug("ProcessAsync(context={Context},output={Output}): data_for_input_selector → {Data_for_input_selector}", context, output, data_for_input_selector);
            var htmlOutput = await _htmlHelper.CheckBox(For.Name, null, htmlAttributes).RenderHtmlContentAsync();
			_logger.LogError("ProcessAsync(context={Context},output={Output}): htmlOutput → {HtmlOutput}", context, output, htmlOutput);
            output.Content.SetHtmlContent(htmlOutput);
        }
		_logger.LogWarning("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
    }

    #endregion

    #region Properties

    /// <summary>
    /// An expression to be evaluated against the current model
    /// </summary>
    [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// The input #1
    /// </summary>
    [HtmlAttributeName(INPUT_ATTRIBUTE_NAME)]
    public ModelExpression Input { set; get; }

    /// <summary>
    /// The input #2
    /// </summary>
    [HtmlAttributeName(INPUT_2_ATTRIBUTE_NAME)]
    public ModelExpression Input2 { set; get; }

    /// <summary>
    ///The store scope
    /// </summary>
    [HtmlAttributeName(STORE_SCOPE_ATTRIBUTE_NAME)]
    public int StoreScope { set; get; }

    /// <summary>
    /// Parent container
    /// </summary>
    [HtmlAttributeName(PARENT_CONTAINER_ATTRIBUTE_NAME)]
    public string ParentContainer { set; get; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}