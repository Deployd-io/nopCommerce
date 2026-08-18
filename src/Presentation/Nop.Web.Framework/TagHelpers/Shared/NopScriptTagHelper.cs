using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Hosting;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.UI;
using Nop.Web.Framework.WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.TagHelpers.Shared;

/// <summary>
/// Script bundling tag helper
/// </summary>
[HtmlTargetElement(SCRIPT_TAG_NAME)]
public partial class NopScriptTagHelper : UrlResolutionTagHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    protected const string SCRIPT_TAG_NAME = "script";
    protected const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "asp-exclude-from-bundle";
    protected const string DEBUG_SRC_ATTRIBUTE_NAME = "asp-debug-src";
    protected const string LOCATION_ATTRIBUTE_NAME = "asp-location";
    protected const string SRC_ATTRIBUTE_NAME = "src";

    #endregion

    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly INopAssetHelper _bundleHelper;
    protected readonly INopHtmlHelper _nopHtmlHelper;
    protected readonly IWebHelper _webHelper;
    protected readonly IWebHostEnvironment _webHostEnvironment;

    #endregion

    #region Ctor

    public NopScriptTagHelper(AppSettings appSettings,
        HtmlEncoder htmlEncoder,
        INopAssetHelper bundleHelper,
        INopHtmlHelper nopHtmlHelper,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        IWebHostEnvironment webHostEnvironment) : base(urlHelperFactory, htmlEncoder)
    {
		_logger.LogDebug(">>> Entering NopScriptTagHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},bundleHelper={BundleHelper},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment})", appSettings, htmlEncoder, bundleHelper, nopHtmlHelper, urlHelperFactory, webHelper, webHostEnvironment);
        _appSettings = appSettings;
		_logger.LogDebug("NopScriptTagHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},bundleHelper={BundleHelper},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _appSettings → {_appSettings}", appSettings, htmlEncoder, bundleHelper, nopHtmlHelper, urlHelperFactory, webHelper, webHostEnvironment, _appSettings);
        _bundleHelper = bundleHelper;
		_logger.LogDebug("NopScriptTagHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},bundleHelper={BundleHelper},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _bundleHelper → {_bundleHelper}", appSettings, htmlEncoder, bundleHelper, nopHtmlHelper, urlHelperFactory, webHelper, webHostEnvironment, _bundleHelper);
        _nopHtmlHelper = nopHtmlHelper;
		_logger.LogDebug("NopScriptTagHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},bundleHelper={BundleHelper},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _nopHtmlHelper → {_nopHtmlHelper}", appSettings, htmlEncoder, bundleHelper, nopHtmlHelper, urlHelperFactory, webHelper, webHostEnvironment, _nopHtmlHelper);
        _webHelper = webHelper;
		_logger.LogDebug("NopScriptTagHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},bundleHelper={BundleHelper},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _webHelper → {_webHelper}", appSettings, htmlEncoder, bundleHelper, nopHtmlHelper, urlHelperFactory, webHelper, webHostEnvironment, _webHelper);
        _webHostEnvironment = webHostEnvironment;
		_logger.LogDebug("NopScriptTagHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},bundleHelper={BundleHelper},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment}): _webHostEnvironment → {_webHostEnvironment}", appSettings, htmlEncoder, bundleHelper, nopHtmlHelper, urlHelperFactory, webHelper, webHostEnvironment, _webHostEnvironment);
		_logger.LogDebug("<<< Exiting NopScriptTagHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},bundleHelper={BundleHelper},nopHtmlHelper={NopHtmlHelper},urlHelperFactory={UrlHelperFactory},webHelper={WebHelper},webHostEnvironment={WebHostEnvironment})", appSettings, htmlEncoder, bundleHelper, nopHtmlHelper, urlHelperFactory, webHelper, webHostEnvironment);
    }

    #endregion

    #region Utilities

    protected static async Task<string> BuildInlineScriptTagAsync(TagHelperOutput output)
    {
		_logger.LogDebug(">>> Entering BuildInlineScriptTagAsync(output={Output})", output);
        //get JavaScript
        var scriptTag = new TagBuilder(SCRIPT_TAG_NAME);
		_logger.LogDebug("BuildInlineScriptTagAsync(output={Output}): scriptTag → {ScriptTag}", output, scriptTag);

        var childContent = await output.GetChildContentAsync();
		_logger.LogDebug("BuildInlineScriptTagAsync(output={Output}): childContent → {ChildContent}", output, childContent);
        var script = childContent.GetContent();
		_logger.LogDebug("BuildInlineScriptTagAsync(output={Output}): script → {Script}", output, script);

        if (!string.IsNullOrEmpty(script))
            scriptTag.InnerHtml.SetHtmlContent(new HtmlString(script));

        scriptTag.MergeAttributes(await output.GetAttributeDictionaryAsync(), replaceExisting: false);

		_logger.LogDebug("<<< Exiting BuildInlineScriptTagAsync(output={Output})", output);
        return await scriptTag.RenderHtmlContentAsync() + Environment.NewLine;
    }

    protected void ProcessSrcAttribute(TagHelperContext context, TagHelperOutput output)
    {
		_logger.LogDebug(">>> Entering ProcessSrcAttribute(context={Context},output={Output})", context, output);
        if (!string.IsNullOrEmpty(DebugSrc) && _webHostEnvironment.IsDevelopment())
            Src = DebugSrc;

		_logger.LogDebug("ProcessSrcAttribute(context={Context},output={Output}): Src → {Src}", context, output, Src);
        // Pass through attribute that is also a well-known HTML attribute.
        if (Src != null)
            output.CopyHtmlAttribute(SRC_ATTRIBUTE_NAME, context);

        // If there's no "src" attribute in output.Attributes this will noop.
        ProcessUrlAttribute(SRC_ATTRIBUTE_NAME, output);

        // Retrieve the TagHelperOutput variation of the "src" attribute in case other TagHelpers in the
        // pipeline have touched the value. If the value is already encoded this ScriptTagHelper may
        // not function properly.
        if (output.Attributes[SRC_ATTRIBUTE_NAME]?.Value is string srcAttribute)
            Src = srcAttribute;
			_logger.LogDebug("<<< Exiting ProcessSrcAttribute(context={Context},output={Output})", context, output);
    }

    protected void ProcessAsset(TagHelperOutput output)
    {
		_logger.LogDebug(">>> Entering ProcessAsset(output={Output})", output);
        if (string.IsNullOrEmpty(Src))
            return;

        var urlHelper = UrlHelperFactory.GetUrlHelper(ViewContext);
		_logger.LogDebug("ProcessAsset(output={Output}): urlHelper → {UrlHelper}", output, urlHelper);
        if (!urlHelper.IsLocalUrl(Src))
        {
            output.Attributes.SetAttribute(SRC_ATTRIBUTE_NAME, Src);
            return;
        }

        var asset = _bundleHelper.GetOrCreateJavaScriptAsset(Src, [Src]);
		_logger.LogDebug("ProcessAsset(output={Output}): asset → {Asset}", output, asset);
        output.Attributes.SetAttribute(SRC_ATTRIBUTE_NAME, _bundleHelper.CacheBusting(asset));
		_logger.LogDebug("<<< Exiting ProcessAsset(output={Output})", output);
    }

    #endregion

    #region Methods

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
		_logger.LogDebug(">>> Entering ProcessAsync(context={Context},output={Output})", context, output);
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        if (_webHelper.IsAjaxRequest(ViewContext.HttpContext?.Request))
            return;

        output.TagMode = TagMode.StartTagAndEndTag;
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): output.TagMode → {Arg2}", context, output, output.TagMode);

        //process only text/javascript scripts
        if (context.AllAttributes.TryGetAttribute("type", out var attribute)
             && !string.Equals(attribute.Value?.ToString(), MimeTypes.TextJavascript, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var woConfig = _appSettings.Get<WebOptimizerConfig>();
		_logger.LogDebug("ProcessAsync(context={Context},output={Output}): woConfig → {WoConfig}", context, output, woConfig);

        if (Location == ResourceLocation.Auto)
        {
            // move script to the footer bundle when bundling is enabled
            Location = woConfig.EnableJavaScriptBundling ? ResourceLocation.Footer : ResourceLocation.None;
			_logger.LogDebug("ProcessAsync(context={Context},output={Output}): Location → {Location}", context, output, Location);
        }

        if (Location == ResourceLocation.None)
        {
            if (!string.IsNullOrEmpty(Src))
            {
                ProcessSrcAttribute(context, output);
                ProcessAsset(output);
            }

            return;
        }

        if (string.IsNullOrEmpty(Src))
            _nopHtmlHelper.AddInlineScriptParts(Location, await BuildInlineScriptTagAsync(output));
        else
            _nopHtmlHelper.AddScriptParts(Location, Src, DebugSrc, ExcludeFromBundle);

        output.SuppressOutput();
		_logger.LogDebug("<<< Exiting ProcessAsync(context={Context},output={Output})", context, output);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Script path (e.g. full debug version). If empty, then minified version will be used
    /// </summary>
    [HtmlAttributeName(DEBUG_SRC_ATTRIBUTE_NAME)]
    public string DebugSrc { get; set; }

    /// <summary>
    /// Indicates where the script should be rendered
    /// </summary>
    [HtmlAttributeName(LOCATION_ATTRIBUTE_NAME)]
    public ResourceLocation Location { set; get; }

    /// <summary>
    /// A value indicating if a file should be excluded from the bundle
    /// </summary>
    [HtmlAttributeName(EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
    public bool ExcludeFromBundle { get; set; }

    /// <summary>
    /// Address of the external script to use
    /// </summary>
    [HtmlAttributeName(SRC_ATTRIBUTE_NAME)]
    public string Src { get; set; }

    #endregion
}