using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core.Domain.Security;
using Nop.Web.Framework.Security.Captcha;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers.Public;

/// <summary>
/// "nop-captcha" tag helper
/// </summary>
[HtmlTargetElement("nop-captcha", TagStructure = TagStructure.WithoutEndTag)]
public partial class NopGenerateCaptchaTagHelper : TagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly IHtmlHelper _htmlHelper;

    #endregion

    #region Ctor

    public NopGenerateCaptchaTagHelper(CaptchaSettings captchaSettings, IHtmlHelper htmlHelper)
    {
		_logger.LogError(">>> Entering NopGenerateCaptchaTagHelper(captchaSettings={CaptchaSettings},htmlHelper={HtmlHelper})", captchaSettings, htmlHelper);
        _captchaSettings = captchaSettings;
        _htmlHelper = htmlHelper;
		_logger.LogDebug("NopGenerateCaptchaTagHelper(captchaSettings={CaptchaSettings},htmlHelper={HtmlHelper}): _htmlHelper → {_htmlHelper}", captchaSettings, htmlHelper, _htmlHelper);
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

        IHtmlContent captchaHtmlContent;
        switch (_captchaSettings.CaptchaType)
        {
            case CaptchaType.CheckBoxReCaptchaV2:
                output.Attributes.Add("class", "captcha-box");
                captchaHtmlContent = await _htmlHelper.GenerateCheckBoxReCaptchaV2Async(_captchaSettings);
				_logger.LogError("ProcessAsync(context={Context},output={Output}): captchaHtmlContent → {CaptchaHtmlContent}", context, output, captchaHtmlContent);
				_logger.LogDebug("ProcessAsync(context={Context},output={Output}): captchaHtmlContent → {CaptchaHtmlContent}", context, output, captchaHtmlContent);
                break;
            case CaptchaType.ReCaptchaV3:
                captchaHtmlContent = await _htmlHelper.GenerateReCaptchaV3Async(_captchaSettings, ActionName);
                break;
            default:
				_logger.LogError("Throwing InvalidOperationException from ProcessAsync(context={Context},output={Output})", context, output);
                throw new InvalidOperationException("Invalid captcha type.");
        }

        //tag details
        output.TagName = "div";
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagName → {Arg2}", context, output, output.TagName);
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagMode → {Arg2}", context, output, output.TagMode);
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Content.SetHtmlContent(captchaHtmlContent);
		_logger.LogWarning("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
    }

    #endregion

    #region Properties

    /// <summary>
    /// ActionName
    /// </summary>
    [HtmlAttributeName("action-name")]
    public string ActionName { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}