using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.UI;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers.Shared;

/// <summary>
/// CSS bundling tag helper
/// </summary>
[HtmlTargetElement(LINK_TAG_NAME, Attributes = "[rel=stylesheet]")]
public partial class NopLinkTagHelper : UrlResolutionTagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    protected const string LINK_TAG_NAME = "link";
    protected const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "asp-exclude-from-bundle";
    protected const string HREF_ATTRIBUTE_NAME = "href";

    #endregion

    #region Fields
    protected readonly INopHtmlHelper _nopHtmlHelper;

    #endregion

    #region Ctor

    public NopLinkTagHelper(HtmlEncoder htmlEncoder,
        INopHtmlHelper nopHtmlHelper,
        IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory, htmlEncoder)
    {
		_logger.LogDebug(">>> Entering NopLinkTagHelper(htmlEncoder={HtmlEncoder},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory})", htmlEncoder, nopHtmlHelper, urlHelperFactory);
        _nopHtmlHelper = nopHtmlHelper;
		_logger.LogDebug("NopLinkTagHelper(htmlEncoder={HtmlEncoder},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory}): _nopHtmlHelper → {_nopHtmlHelper}", htmlEncoder, nopHtmlHelper, urlHelperFactory, _nopHtmlHelper);
		_logger.LogDebug("<<< Exiting NopLinkTagHelper(htmlEncoder={HtmlEncoder},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory})", htmlEncoder, nopHtmlHelper, urlHelperFactory);
    }

    #endregion

    #region Methods

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
		_logger.LogDebug(">>> Entering ProcessAsync(context={Context},output={Output})", context, output);
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        _nopHtmlHelper.AddCssFileParts(Href, string.Empty, ExcludeFromBundle);

        output.SuppressOutput();

		_logger.LogDebug("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// A value indicating if a file should be excluded from the bundle
    /// </summary>
    [HtmlAttributeName(EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
    public bool ExcludeFromBundle { get; set; }

    /// <summary>
    /// Address of the linked resource
    /// </summary>
    [HtmlAttributeName(HREF_ATTRIBUTE_NAME)]
    public string Href { get; set; }

    #endregion
}