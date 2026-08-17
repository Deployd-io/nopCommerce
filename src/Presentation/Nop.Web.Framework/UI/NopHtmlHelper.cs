using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Seo;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.UI;

/// <summary>
/// Represents the HTML helper implementation
/// </summary>
public partial class NopHtmlHelper : INopHtmlHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly HtmlEncoder _htmlEncoder;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IHtmlHelper _htmlHelper;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly INopAssetHelper _bundleHelper;
    protected readonly Lazy<ILocalizationService> _localizationService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWebHostEnvironment _webHostEnvironment;
    protected readonly SeoSettings _seoSettings;

    protected readonly Dictionary<ResourceLocation, List<ScriptReferenceMeta>> _scriptParts = new();
    protected readonly Dictionary<ResourceLocation, List<string>> _inlineScriptParts = new();
    protected readonly List<CssReferenceMeta> _cssParts = new();

    protected readonly List<string> _canonicalUrlParts = new();
    protected readonly List<string> _headCustomParts = new();
    protected readonly List<string> _metaDescriptionParts = new();
    protected readonly List<string> _metaKeywordParts = new();
    protected readonly List<string> _pageCssClassParts = new();
    protected readonly List<string> _titleParts = new();

    protected string _activeAdminMenuSystemName;
    protected string _editPageUrl;

    #endregion

    #region Ctor

    public NopHtmlHelper(AppSettings appSettings,
        HtmlEncoder htmlEncoder,
        IActionContextAccessor actionContextAccessor,
        IHtmlHelper htmlHelper,
        IHttpContextAccessor httpContextAccessor,
        INopAssetHelper bundleHelper,
        Lazy<ILocalizationService> localizationService,
        IStoreContext storeContext,
        IUrlHelperFactory urlHelperFactory,
        IWebHostEnvironment webHostEnvironment,
        SeoSettings seoSettings)
    {
		_logger.LogDebug(">>> Entering NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings})", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings);
        _appSettings = appSettings;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _appSettings → {_appSettings}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _appSettings);
        _htmlEncoder = htmlEncoder;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _htmlEncoder → {_htmlEncoder}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _htmlEncoder);
        _actionContextAccessor = actionContextAccessor;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _actionContextAccessor → {_actionContextAccessor}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _actionContextAccessor);
        _htmlHelper = htmlHelper;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _htmlHelper → {_htmlHelper}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _htmlHelper);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _httpContextAccessor → {_httpContextAccessor}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _httpContextAccessor);
        _bundleHelper = bundleHelper;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _bundleHelper → {_bundleHelper}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _bundleHelper);
        _localizationService = localizationService;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _localizationService → {_localizationService}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _localizationService);
        _storeContext = storeContext;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _storeContext → {_storeContext}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _storeContext);
        _urlHelperFactory = urlHelperFactory;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _urlHelperFactory → {_urlHelperFactory}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _urlHelperFactory);
        _webHostEnvironment = webHostEnvironment;
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _webHostEnvironment → {_webHostEnvironment}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _webHostEnvironment);
        _seoSettings = seoSettings;
		_logger.LogDebug("<<< Exiting NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings})", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings);
		_logger.LogDebug("NopHtmlHelper(appSettings={AppSettings},htmlEncoder={HtmlEncoder},actionContextAccessor={ActionContextAccessor},htmlHelper={HtmlHelper},httpContextAccessor={HttpContextAccessor},bundleHelper={BundleHelper},localizationService={LocalizationService},storeContext={StoreContext},urlHelperFactory={UrlHelperFactory},webHostEnvironment={WebHostEnvironment},seoSettings={SeoSettings}): _seoSettings → {_seoSettings}", appSettings, htmlEncoder, actionContextAccessor, htmlHelper, httpContextAccessor, bundleHelper, localizationService, storeContext, urlHelperFactory, webHostEnvironment, seoSettings, _seoSettings);
    }

    #endregion

    #region Utilities

    protected static string GetAssetKey(string[] keys, string suffix)
    {
		_logger.LogDebug(">>> Entering GetAssetKey(keys={Keys},suffix={Suffix})", keys, suffix);
        ArgumentNullException.ThrowIfNull(keys?.Length > 0 ? keys : null, nameof(keys));

        var hashInput = string.Join(',', keys);
		_logger.LogDebug("GetAssetKey(keys={Keys},suffix={Suffix}): hashInput → {HashInput}", keys, suffix, hashInput);
        var input = MD5.HashData(Encoding.Unicode.GetBytes(hashInput));
		_logger.LogDebug("GetAssetKey(keys={Keys},suffix={Suffix}): input → {Input}", keys, suffix, input);

        var key = string.Concat(WebEncoders.Base64UrlEncode(input));
		_logger.LogWarning("GetAssetKey(keys={Keys},suffix={Suffix}): key → {Key}", keys, suffix, key);

        if (!string.IsNullOrEmpty(suffix))
            key += suffix;

		_logger.LogDebug("<<< Exiting GetAssetKey(keys={Keys},suffix={Suffix})", keys, suffix);
        return key.ToLower();
    }

    /// <summary>
    /// Get URL of "src" parameter and check whether it is local
    /// </summary>
    /// <param name="src">Src</param>
    /// <returns>URL; check result</returns>
    protected virtual (string Url, bool IsLocal) GetSrcUrl(string src)
    {
		_logger.LogDebug(">>> Entering GetSrcUrl(Url={Url},IsLocal={IsLocal})", Url, IsLocal);
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

		_logger.LogDebug("GetSrcUrl(Url={Url},IsLocal={IsLocal}): urlHelper → {UrlHelper}", Url, IsLocal, urlHelper);
        var isLocal = urlHelper.IsLocalUrl(src);
		_logger.LogDebug("GetSrcUrl(Url={Url},IsLocal={IsLocal}): isLocal → {IsLocal1}", Url, IsLocal, isLocal);
        var url = isLocal
            ? urlHelper.Content(src).RemoveApplicationPathFromRawUrl(_httpContextAccessor.HttpContext.Request.PathBase)
            : src;

		_logger.LogDebug("<<< Exiting GetSrcUrl(Url={Url},IsLocal={IsLocal})", Url, IsLocal);
        return (url, isLocal);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Add title element to the <![CDATA[<head>]]>
    /// </summary>
    /// <param name="part">Title part</param>
    public virtual void AddTitleParts(string part)
    {
		_logger.LogDebug(">>> Entering AddTitleParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _titleParts.Add(part);
		_logger.LogDebug("<<< Exiting AddTitleParts(part={Part})", part);
    }

    /// <summary>
    /// Append title element to the <![CDATA[<head>]]>
    /// </summary>
    /// <param name="part">Title part</param>
    public virtual void AppendTitleParts(string part)
    {
		_logger.LogDebug(">>> Entering AppendTitleParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _titleParts.Insert(0, part);
		_logger.LogDebug("<<< Exiting AppendTitleParts(part={Part})", part);
    }

    /// <summary>
    /// Generate all title parts
    /// </summary>
    /// <param name="addDefaultTitle">A value indicating whether to insert a default title</param>
    /// <param name="part">Title part</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains generated HTML string</returns>
    public virtual async Task<IHtmlContent> GenerateTitleAsync(bool addDefaultTitle = true, string part = "")
    {
		_logger.LogDebug(">>> Entering GenerateTitleAsync(addDefaultTitle={AddDefaultTitle},part={Part})", addDefaultTitle, part);
        AppendTitleParts(part);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("GenerateTitleAsync(addDefaultTitle={AddDefaultTitle},part={Part}): store → {Store}", addDefaultTitle, part, store);
        var defaultTitle = await _localizationService.Value.GetLocalizedAsync(store, s => s.DefaultTitle);
		_logger.LogDebug("GenerateTitleAsync(addDefaultTitle={AddDefaultTitle},part={Part}): defaultTitle → {DefaultTitle}", addDefaultTitle, part, defaultTitle);

        var specificTitle = string.Join(_seoSettings.PageTitleSeparator, _titleParts.AsEnumerable().Reverse().ToArray());
		_logger.LogDebug("GenerateTitleAsync(addDefaultTitle={AddDefaultTitle},part={Part}): specificTitle → {SpecificTitle}", addDefaultTitle, part, specificTitle);
        string result;
        if (!string.IsNullOrEmpty(specificTitle))
        {
            if (addDefaultTitle)
            {
                //store name + page title
                switch (_seoSettings.PageTitleSeoAdjustment)
                {
                    case PageTitleSeoAdjustment.PagenameAfterStorename:
                        {
                            result = string.Join(_seoSettings.PageTitleSeparator, defaultTitle, specificTitle);
							_logger.LogDebug("GenerateTitleAsync(addDefaultTitle={AddDefaultTitle},part={Part}): result → {Result}", addDefaultTitle, part, result);
                        }
                        break;
                    case PageTitleSeoAdjustment.StorenameAfterPagename:
                    default:
                        {
                            result = string.Join(_seoSettings.PageTitleSeparator, specificTitle, defaultTitle);
                        }
                        break;
                }
            }
            else
            {
                //page title only
                result = specificTitle;
            }
        }
        else
        {
            //store name only
            result = defaultTitle;
        }

		_logger.LogDebug("<<< Exiting GenerateTitleAsync(addDefaultTitle={AddDefaultTitle},part={Part})", addDefaultTitle, part);
        return new HtmlString(_htmlEncoder.Encode(result ?? string.Empty));
    }

    /// <summary>
    /// Add meta description element to the <![CDATA[<head>]]>
    /// </summary>
    /// <param name="part">Meta description part</param>
    public virtual void AddMetaDescriptionParts(string part)
    {
		_logger.LogDebug(">>> Entering AddMetaDescriptionParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _metaDescriptionParts.Add(part);
		_logger.LogDebug("<<< Exiting AddMetaDescriptionParts(part={Part})", part);
    }

    /// <summary>
    /// Append meta description element to the <![CDATA[<head>]]>
    /// </summary>
    /// <param name="part">Meta description part</param>
    public virtual void AppendMetaDescriptionParts(string part)
    {
		_logger.LogDebug(">>> Entering AppendMetaDescriptionParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _metaDescriptionParts.Insert(0, part);
		_logger.LogDebug("<<< Exiting AppendMetaDescriptionParts(part={Part})", part);
    }

    /// <summary>
    /// Generate all description parts
    /// </summary>
    /// <param name="part">Meta description part</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains generated HTML string</returns>
    public virtual async Task<IHtmlContent> GenerateMetaDescriptionAsync(string part = "")
    {
		_logger.LogDebug(">>> Entering GenerateMetaDescriptionAsync(part={Part})", part);
        AppendMetaDescriptionParts(part);

        var metaDescription = string.Join(", ", _metaDescriptionParts.AsEnumerable().Reverse().ToArray());
		_logger.LogDebug("GenerateMetaDescriptionAsync(part={Part}): metaDescription → {MetaDescription}", part, metaDescription);
        var result = !string.IsNullOrEmpty(metaDescription)
            ? metaDescription
            : await _localizationService.Value.GetLocalizedAsync(await _storeContext.GetCurrentStoreAsync(),
                s => s.DefaultMetaDescription);

		_logger.LogDebug("<<< Exiting GenerateMetaDescriptionAsync(part={Part})", part);
        return new HtmlString(_htmlEncoder.Encode(result ?? string.Empty));
    }

    /// <summary>
    /// Add meta keyword element to the <![CDATA[<head>]]>
    /// </summary>
    /// <param name="part">Meta keyword part</param>
    public virtual void AddMetaKeywordParts(string part)
    {
		_logger.LogDebug(">>> Entering AddMetaKeywordParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _metaKeywordParts.Add(part);
		_logger.LogDebug("<<< Exiting AddMetaKeywordParts(part={Part})", part);
    }

    /// <summary>
    /// Append meta keyword element to the <![CDATA[<head>]]>
    /// </summary>
    /// <param name="part">Meta keyword part</param>
    public virtual void AppendMetaKeywordParts(string part)
    {
		_logger.LogDebug(">>> Entering AppendMetaKeywordParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _metaKeywordParts.Insert(0, part);
		_logger.LogDebug("<<< Exiting AppendMetaKeywordParts(part={Part})", part);
    }

    /// <summary>
    /// Generate all keyword parts
    /// </summary>
    /// <param name="part">Meta keyword part</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains generated HTML string</returns>
    public virtual async Task<IHtmlContent> GenerateMetaKeywordsAsync(string part = "")
    {
		_logger.LogDebug(">>> Entering GenerateMetaKeywordsAsync(part={Part})", part);
        AppendMetaKeywordParts(part);

        var metaKeyword = string.Join(", ", _metaKeywordParts.AsEnumerable().Reverse().ToArray());
		_logger.LogDebug("GenerateMetaKeywordsAsync(part={Part}): metaKeyword → {MetaKeyword}", part, metaKeyword);
        var result = !string.IsNullOrEmpty(metaKeyword)
            ? metaKeyword
            : await _localizationService.Value.GetLocalizedAsync(await _storeContext.GetCurrentStoreAsync(),
                s => s.DefaultMetaKeywords);

		_logger.LogDebug("<<< Exiting GenerateMetaKeywordsAsync(part={Part})", part);
        return new HtmlString(_htmlEncoder.Encode(result ?? string.Empty));
    }

    /// <summary>
    /// Add script element
    /// </summary>
    /// <param name="location">A location of the script element</param>
    /// <param name="src">Script path (minified version)</param>
    /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
    /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
    public virtual void AddScriptParts(ResourceLocation location, string src, string debugSrc = "", bool excludeFromBundle = false)
    {
		_logger.LogDebug(">>> Entering AddScriptParts(location={Location},src={Src},debugSrc={DebugSrc},excludeFromBundle={ExcludeFromBundle})", location, src, debugSrc, excludeFromBundle);
        if (!_scriptParts.ContainsKey(location))
            _scriptParts.Add(location, new List<ScriptReferenceMeta>());

        if (string.IsNullOrEmpty(src))
            return;

        if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
            src = debugSrc;

        var (url, isLocal) = GetSrcUrl(src);
        _scriptParts[location].Add(new ScriptReferenceMeta
        {
            ExcludeFromBundle = excludeFromBundle,
            IsLocal = isLocal,
            Src = url
        });
		_logger.LogDebug("<<< Exiting AddScriptParts(location={Location},src={Src},debugSrc={DebugSrc},excludeFromBundle={ExcludeFromBundle})", location, src, debugSrc, excludeFromBundle);
    }

    /// <summary>
    /// Append script element
    /// </summary>
    /// <param name="location">A location of the script element</param>
    /// <param name="src">Script path (minified version)</param>
    /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
    /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
    public virtual void AppendScriptParts(ResourceLocation location, string src, string debugSrc = "", bool excludeFromBundle = false)
    {
		_logger.LogDebug(">>> Entering AppendScriptParts(location={Location},src={Src},debugSrc={DebugSrc},excludeFromBundle={ExcludeFromBundle})", location, src, debugSrc, excludeFromBundle);
        if (!_scriptParts.ContainsKey(location))
            _scriptParts.Add(location, new List<ScriptReferenceMeta>());

        if (string.IsNullOrEmpty(src))
            return;

        if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
            src = debugSrc;

        var (url, isLocal) = GetSrcUrl(src);
        _scriptParts[location].Insert(0, new ScriptReferenceMeta
        {
            ExcludeFromBundle = excludeFromBundle,
            IsLocal = isLocal,
            Src = url
        });
		_logger.LogDebug("<<< Exiting AppendScriptParts(location={Location},src={Src},debugSrc={DebugSrc},excludeFromBundle={ExcludeFromBundle})", location, src, debugSrc, excludeFromBundle);
    }

    /// <summary>
    /// Generate all script parts
    /// </summary>
    /// <param name="location">A location of the script element</param>
    /// <returns>Generated HTML string</returns>
    public virtual IHtmlContent GenerateScripts(ResourceLocation location)
    {
		_logger.LogDebug(">>> Entering GenerateScripts(location={Location})", location);
        if (!_scriptParts.TryGetValue(location, out var value) || value == null)
            return HtmlString.Empty;

        if (!_scriptParts.Any())
            return HtmlString.Empty;

        var result = new StringBuilder();
		_logger.LogDebug("GenerateScripts(location={Location}): result → {Result}", location, result);
        var woConfig = _appSettings.Get<WebOptimizerConfig>();
		_logger.LogDebug("GenerateScripts(location={Location}): woConfig → {WoConfig}", location, woConfig);
        var pathBase = _httpContextAccessor.HttpContext.Request.PathBase;

        if (woConfig.EnableJavaScriptBundling && value.Any(item => !item.ExcludeFromBundle))
        {
            var sources = value.Where(item => !item.ExcludeFromBundle && item.IsLocal)
                .Select(item => item.Src)
                .Distinct().ToArray();

            var bundleKey = $"/js/{GetAssetKey(sources, woConfig.JavaScriptBundleSuffix)}.js";
			_logger.LogDebug("GenerateScripts(location={Location}): bundleKey → {BundleKey}", location, bundleKey);

            var bundleAsset = _bundleHelper.GetOrCreateJavaScriptAsset(bundleKey, sources);
			_logger.LogDebug("GenerateScripts(location={Location}): bundleAsset → {BundleAsset}", location, bundleAsset);
            var route = _bundleHelper.CacheBusting(bundleAsset);
			_logger.LogDebug("GenerateScripts(location={Location}): route → {Route}", location, route);

            result.AppendFormat("<script type=\"{0}\" src=\"{1}{2}\"></script>",
                MimeTypes.TextJavascript, pathBase, route);
        }

        var scripts = value.Where(item => !woConfig.EnableJavaScriptBundling || item.ExcludeFromBundle || !item.IsLocal)
            .Distinct();

        foreach (var item in scripts)
        {
            if (!item.IsLocal)
            {
                result.AppendFormat("<script type=\"{0}\" src=\"{1}\"></script>", MimeTypes.TextJavascript, item.Src);
                result.Append(Environment.NewLine);
                continue;
            }

            var asset = _bundleHelper.GetOrCreateJavaScriptAsset(item.Src);
			_logger.LogDebug("GenerateScripts(location={Location}): asset → {Asset}", location, asset);
            var route = _bundleHelper.CacheBusting(asset);

            result.AppendFormat("<script type=\"{0}\" src=\"{1}{2}\"></script>",
                MimeTypes.TextJavascript, pathBase, route);

            result.Append(Environment.NewLine);
        }

		_logger.LogDebug("<<< Exiting GenerateScripts(location={Location})", location);
        return new HtmlString(result.ToString());
    }

    /// <summary>
    /// Add inline script element
    /// </summary>
    /// <param name="location">A location of the script element</param>
    /// <param name="script">Script</param>
    public virtual void AddInlineScriptParts(ResourceLocation location, string script)
    {
		_logger.LogDebug(">>> Entering AddInlineScriptParts(location={Location},script={Script})", location, script);
        if (!_inlineScriptParts.ContainsKey(location))
            _inlineScriptParts.Add(location, new());

        if (string.IsNullOrEmpty(script))
            return;

        if (_inlineScriptParts[location].Contains(script))
            return;

        _inlineScriptParts[location].Add(script);
		_logger.LogDebug("<<< Exiting AddInlineScriptParts(location={Location},script={Script})", location, script);
    }

    /// <summary>
    /// Append inline script element
    /// </summary>
    /// <param name="location">A location of the script element</param>
    /// <param name="script">Script</param>
    public virtual void AppendInlineScriptParts(ResourceLocation location, string script)
    {
		_logger.LogDebug(">>> Entering AppendInlineScriptParts(location={Location},script={Script})", location, script);
        if (!_inlineScriptParts.ContainsKey(location))
            _inlineScriptParts.Add(location, new());

        if (string.IsNullOrEmpty(script))
            return;

        if (_inlineScriptParts[location].Contains(script))
            return;

        _inlineScriptParts[location].Insert(0, script);
		_logger.LogDebug("<<< Exiting AppendInlineScriptParts(location={Location},script={Script})", location, script);
    }

    /// <summary>
    /// Generate all inline script parts
    /// </summary>
    /// <param name="location">A location of the script element</param>
    /// <returns>Generated HTML string</returns>
    public virtual IHtmlContent GenerateInlineScripts(ResourceLocation location)
    {
		_logger.LogDebug(">>> Entering GenerateInlineScripts(location={Location})", location);
        if (!_inlineScriptParts.TryGetValue(location, out var value) || value == null)
            return HtmlString.Empty;

        if (!_inlineScriptParts.Any())
            return HtmlString.Empty;

        var result = new StringBuilder();
		_logger.LogDebug("GenerateInlineScripts(location={Location}): result → {Result}", location, result);
        foreach (var item in value)
        {
            result.Append(item);
            result.Append(Environment.NewLine);
        }
		_logger.LogDebug("<<< Exiting GenerateInlineScripts(location={Location})", location);
        return new HtmlString(result.ToString());
    }

    /// <summary>
    /// Add CSS element
    /// </summary>
    /// <param name="src">Script path (minified version)</param>
    /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
    /// <param name="excludeFromBundle">A value indicating whether to exclude this style sheet from bundling</param>
    public virtual void AddCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false)
    {
		_logger.LogDebug(">>> Entering AddCssFileParts(src={Src},debugSrc={DebugSrc},excludeFromBundle={ExcludeFromBundle})", src, debugSrc, excludeFromBundle);
        if (string.IsNullOrEmpty(src))
            return;

        if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
            src = debugSrc;

        var (url, isLocal) = GetSrcUrl(src);
        _cssParts.Add(new CssReferenceMeta
        {
            ExcludeFromBundle = excludeFromBundle,
            IsLocal = isLocal,
            Src = url
        });
		_logger.LogDebug("<<< Exiting AddCssFileParts(src={Src},debugSrc={DebugSrc},excludeFromBundle={ExcludeFromBundle})", src, debugSrc, excludeFromBundle);
    }

    /// <summary>
    /// Append CSS element
    /// </summary>
    /// <param name="src">Script path (minified version)</param>
    /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
    /// <param name="excludeFromBundle">A value indicating whether to exclude this style sheet from bundling</param>
    public virtual void AppendCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false)
    {
		_logger.LogDebug(">>> Entering AppendCssFileParts(src={Src},debugSrc={DebugSrc},excludeFromBundle={ExcludeFromBundle})", src, debugSrc, excludeFromBundle);
        if (string.IsNullOrEmpty(src))
            return;

        if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
            src = debugSrc;

        var (url, isLocal) = GetSrcUrl(src);
        _cssParts.Insert(0, new CssReferenceMeta
        {
            ExcludeFromBundle = excludeFromBundle,
            IsLocal = isLocal,
            Src = url
        });
		_logger.LogDebug("<<< Exiting AppendCssFileParts(src={Src},debugSrc={DebugSrc},excludeFromBundle={ExcludeFromBundle})", src, debugSrc, excludeFromBundle);
    }

    /// <summary>
    /// Generate all CSS parts
    /// </summary>
    /// <returns>Generated HTML string</returns>
    public virtual IHtmlContent GenerateCssFiles()
    {
		_logger.LogDebug(">>> Entering GenerateCssFiles()");
        if (!_cssParts.Any())
            return HtmlString.Empty;

        var result = new StringBuilder();
		_logger.LogDebug("GenerateCssFiles(): result → {Result}", result);

        var woConfig = _appSettings.Get<WebOptimizerConfig>();
		_logger.LogDebug("GenerateCssFiles(): woConfig → {WoConfig}", woConfig);
        var pathBase = _httpContextAccessor.HttpContext.Request.PathBase;

        if (woConfig.EnableCssBundling && _cssParts.Any(item => !item.ExcludeFromBundle))
        {
            var bundleSuffix = woConfig.CssBundleSuffix;

            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                bundleSuffix += ".rtl";

            var sources = _cssParts
                .Where(item => !item.ExcludeFromBundle && item.IsLocal)
                .Distinct()
                //remove the application path from the generated URL if exists
                .Select(item => item.Src).ToArray();

            var bundleKey = $"/css/{GetAssetKey(sources, bundleSuffix)}.css";
			_logger.LogDebug("GenerateCssFiles(): bundleKey → {BundleKey}", bundleKey);

            var bundleAsset = _bundleHelper.GetOrCreateCssAsset(bundleKey, sources);
			_logger.LogDebug("GenerateCssFiles(): bundleAsset → {BundleAsset}", bundleAsset);
            var route = _bundleHelper.CacheBusting(bundleAsset);
			_logger.LogDebug("GenerateCssFiles(): route → {Route}", route);


            result.AppendFormat("<link rel=\"stylesheet\" type=\"{0}\" href=\"{1}{2}\" />",
                MimeTypes.TextCss, pathBase, route);
        }

        var styles = _cssParts
            .Where(item => !woConfig.EnableCssBundling || item.ExcludeFromBundle || !item.IsLocal)
            .Distinct();

        foreach (var item in styles)
        {
            if (!item.IsLocal)
            {
                result.AppendFormat("<link rel=\"stylesheet\" type=\"{0}\" href=\"{1}\" />", MimeTypes.TextCss, item.Src);
                result.Append(Environment.NewLine);
                continue;
            }

            var asset = _bundleHelper.GetOrCreateCssAsset(item.Src);
			_logger.LogDebug("GenerateCssFiles(): asset → {Asset}", asset);
            var route = _bundleHelper.CacheBusting(asset);

            result.AppendFormat("<link rel=\"stylesheet\" type=\"{0}\" href=\"{1}{2}\" />",
                MimeTypes.TextCss, pathBase, route);
            result.AppendLine();
        }

		_logger.LogDebug("<<< Exiting GenerateCssFiles()");
        return new HtmlString(result.ToString());
    }

    /// <summary>
    /// Add canonical URL element to the <![CDATA[<head>]]>
    /// </summary>
    /// <param name="part">Canonical URL part</param>
    /// <param name="withQueryString">Whether to use canonical URLs with query string parameters</param>
    public virtual void AddCanonicalUrlParts(string part, bool withQueryString = false)
    {
		_logger.LogDebug(">>> Entering AddCanonicalUrlParts(part={Part},withQueryString={WithQueryString})", part, withQueryString);
        if (string.IsNullOrEmpty(part))
            return;

        if (withQueryString)
        {
            //add ordered query string parameters
            var queryParameters = _httpContextAccessor.HttpContext.Request.Query
                .OrderBy(parameter => parameter.Key)
                .ToDictionary(parameter => parameter.Key, parameter => parameter.Value.ToString());
            part = QueryHelpers.AddQueryString(part, queryParameters);
			_logger.LogDebug("AddCanonicalUrlParts(part={Part},withQueryString={WithQueryString}): part → {Part1}", part, withQueryString, part);
        }

        _canonicalUrlParts.Add(part);
		_logger.LogDebug("<<< Exiting AddCanonicalUrlParts(part={Part},withQueryString={WithQueryString})", part, withQueryString);
    }

    /// <summary>
    /// Append canonical URL element to the <![CDATA[<head>]]>
    /// </summary>
    /// <param name="part">Canonical URL part</param>
    public virtual void AppendCanonicalUrlParts(string part)
    {
		_logger.LogDebug(">>> Entering AppendCanonicalUrlParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _canonicalUrlParts.Insert(0, part);
		_logger.LogDebug("<<< Exiting AppendCanonicalUrlParts(part={Part})", part);
    }

    /// <summary>
    /// Generate all canonical URL parts
    /// </summary>
    /// <returns>Generated HTML string</returns>
    public virtual IHtmlContent GenerateCanonicalUrls()
    {
		_logger.LogDebug(">>> Entering GenerateCanonicalUrls()");
        var result = new StringBuilder();
        foreach (var canonicalUrl in _canonicalUrlParts)
        {
		_logger.LogDebug("GenerateCanonicalUrls(): result → {Result}", result);
            result.AppendFormat("<link rel=\"canonical\" href=\"{0}\" />", canonicalUrl);
            result.Append(Environment.NewLine);
        }
		_logger.LogDebug("<<< Exiting GenerateCanonicalUrls()");
        return new HtmlString(result.ToString());
    }

    /// <summary>
    /// Add any custom element to the <![CDATA[<head>]]> element
    /// </summary>
    /// <param name="part">The entire element. For example, <![CDATA[<meta name="msvalidate.01" content="123121231231313123123" />]]></param>
    public virtual void AddHeadCustomParts(string part)
    {
		_logger.LogDebug(">>> Entering AddHeadCustomParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _headCustomParts.Add(part);
		_logger.LogDebug("<<< Exiting AddHeadCustomParts(part={Part})", part);
    }

    /// <summary>
    /// Append any custom element to the <![CDATA[<head>]]> element
    /// </summary>
    /// <param name="part">The entire element. For example, <![CDATA[<meta name="msvalidate.01" content="123121231231313123123" />]]></param>
    public virtual void AppendHeadCustomParts(string part)
    {
		_logger.LogDebug(">>> Entering AppendHeadCustomParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _headCustomParts.Insert(0, part);
		_logger.LogDebug("<<< Exiting AppendHeadCustomParts(part={Part})", part);
    }

    /// <summary>
    /// Generate all custom elements
    /// </summary>
    /// <returns>Generated HTML string</returns>
    public virtual IHtmlContent GenerateHeadCustom()
    {
		_logger.LogDebug(">>> Entering GenerateHeadCustom()");
        //use only distinct rows
        var distinctParts = _headCustomParts.Distinct().ToList();
		_logger.LogDebug("GenerateHeadCustom(): distinctParts → {DistinctParts}", distinctParts);
        if (!distinctParts.Any())
            return HtmlString.Empty;

        var result = new StringBuilder();
		_logger.LogDebug("GenerateHeadCustom(): result → {Result}", result);
        foreach (var path in distinctParts)
        {
            result.Append(path);
            result.Append(Environment.NewLine);
        }
		_logger.LogDebug("<<< Exiting GenerateHeadCustom()");
        return new HtmlString(result.ToString());
    }

    /// <summary>
    /// Add CSS class to the <![CDATA[<head>]]> element
    /// </summary>
    /// <param name="part">CSS class</param>
    public virtual void AddPageCssClassParts(string part)
    {
		_logger.LogDebug(">>> Entering AddPageCssClassParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _pageCssClassParts.Add(part);
		_logger.LogDebug("<<< Exiting AddPageCssClassParts(part={Part})", part);
    }

    /// <summary>
    /// Append CSS class to the <![CDATA[<head>]]> element
    /// </summary>
    /// <param name="part">CSS class</param>
    public virtual void AppendPageCssClassParts(string part)
    {
		_logger.LogDebug(">>> Entering AppendPageCssClassParts(part={Part})", part);
        if (string.IsNullOrEmpty(part))
            return;

        _pageCssClassParts.Insert(0, part);
		_logger.LogDebug("<<< Exiting AppendPageCssClassParts(part={Part})", part);
    }

    /// <summary>
    /// Generate all title parts
    /// </summary>
    /// <param name="part">CSS class</param>
    /// <returns>Generated string</returns>
    public virtual string GeneratePageCssClasses(string part = "")
    {
		_logger.LogDebug(">>> Entering GeneratePageCssClasses(part={Part})", part);
        AppendPageCssClassParts(part);

        var result = string.Join(" ", _pageCssClassParts.AsEnumerable().Reverse().ToArray());
		_logger.LogDebug("GeneratePageCssClasses(part={Part}): result → {Result}", part, result);

        if (string.IsNullOrEmpty(result))
		_logger.LogDebug("<<< Exiting GeneratePageCssClasses(part={Part})", part);
            return string.Empty;

        return _htmlEncoder.Encode(result);
    }

    /// <summary>
    /// Specify "edit page" URL
    /// </summary>
    /// <param name="url">URL</param>
    public virtual void AddEditPageUrl(string url)
    {
        _editPageUrl = url;
    }

    /// <summary>
    /// Get "edit page" URL
    /// </summary>
    /// <returns>URL</returns>
    public virtual string GetEditPageUrl()
    {
		_logger.LogDebug(">>> Entering GetEditPageUrl()");
		_logger.LogDebug("<<< Exiting GetEditPageUrl()");
        return _editPageUrl;
    }

    /// <summary>
    /// Specify system name of admin menu item that should be selected (expanded)
    /// </summary>
    /// <param name="systemName">System name</param>
    public virtual void SetActiveMenuItemSystemName(string systemName)
    {
		_logger.LogDebug(">>> Entering SetActiveMenuItemSystemName(systemName={SystemName})", systemName);
        _activeAdminMenuSystemName = systemName;
		_logger.LogDebug("<<< Exiting SetActiveMenuItemSystemName(systemName={SystemName})", systemName);
		_logger.LogDebug("SetActiveMenuItemSystemName(systemName={SystemName}): _activeAdminMenuSystemName → {_activeAdminMenuSystemName}", systemName, _activeAdminMenuSystemName);
    }

    /// <summary>
    /// Get system name of admin menu item that should be selected (expanded)
    /// </summary>
    /// <returns>System name</returns>
    public virtual string GetActiveMenuItemSystemName()
    {
		_logger.LogDebug(">>> Entering GetActiveMenuItemSystemName()");
		_logger.LogDebug("<<< Exiting GetActiveMenuItemSystemName()");
        return _activeAdminMenuSystemName;
    }

    /// <summary>
    /// Get the route name associated with the request rendering this page
    /// </summary>
    /// <param name="handleDefaultRoutes">A value indicating whether to build the name using engine information unless otherwise specified</param>
    /// <returns>Route name</returns>
    public virtual string GetRouteName(bool handleDefaultRoutes = false)
    {
		_logger.LogDebug(">>> Entering GetRouteName(handleDefaultRoutes={HandleDefaultRoutes})", handleDefaultRoutes);
        var httpContext = _httpContextAccessor.HttpContext;
        var routeName = httpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName ?? string.Empty;
		_logger.LogDebug("GetRouteName(handleDefaultRoutes={HandleDefaultRoutes}): routeName → {RouteName}", handleDefaultRoutes, routeName);

        if (!string.IsNullOrEmpty(routeName) && routeName != "areaRoute")
            return routeName;

        //then try to get a generic one (actually it's an action name, not the route)
        if (httpContext.GetRouteValue(NopRoutingDefaults.RouteValue.SeName) is not null &&
            httpContext.GetRouteValue(NopRoutingDefaults.RouteValue.Action) is string actionName)
        {
            return actionName;
        }

        if (handleDefaultRoutes)
        {
            return _actionContextAccessor.ActionContext.ActionDescriptor switch
            {
                ControllerActionDescriptor controllerAction => string.Concat(controllerAction.ControllerName, controllerAction.ActionName),
                CompiledPageActionDescriptor compiledPage => string.Concat(compiledPage.AreaName, compiledPage.ViewEnginePath.Replace("/", "")),
                PageActionDescriptor pageAction => string.Concat(pageAction.AreaName, pageAction.ViewEnginePath.Replace("/", "")),
                _ => _actionContextAccessor.ActionContext.ActionDescriptor.DisplayName?.Replace("/", "") ?? string.Empty
            };
        }

		_logger.LogDebug("<<< Exiting GetRouteName(handleDefaultRoutes={HandleDefaultRoutes})", handleDefaultRoutes);
        return routeName;
    }

    /// <summary>
    /// Add JSON-LD to the <![CDATA[<head>]]> element
    /// </summary>
    /// <param name="jsonLd">The JSON-LD serialized model></param>
    public virtual void AddJsonLdParts(string jsonLd)
    {
		_logger.LogDebug(">>> Entering AddJsonLdParts(jsonLd={JsonLd})", jsonLd);
        if (_seoSettings.MicrodataEnabled)
            AddHeadCustomParts("<script type=\"application/ld+json\">" + _htmlHelper.Raw(jsonLd) + "</script>");
			_logger.LogDebug("<<< Exiting AddJsonLdParts(jsonLd={JsonLd})", jsonLd);
    }

    #endregion

    #region Nested classes

    /// <summary>
    /// JS file meta data
    /// </summary>
    protected partial record ScriptReferenceMeta
    {
        /// <summary>
        /// A value indicating whether to exclude the script from bundling
        /// </summary>
        public bool ExcludeFromBundle { get; init; }

        /// <summary>
        /// A value indicating whether the src is local
        /// </summary>
        public bool IsLocal { get; init; }

        /// <summary>
        /// Src for production
        /// </summary>
        public string Src { get; init; }
    }

    /// <summary>
    /// CSS file meta data
    /// </summary>
    protected partial record CssReferenceMeta
    {
        /// <summary>
        /// A value indicating whether to exclude the script from bundling
        /// </summary>
        public bool ExcludeFromBundle { get; init; }

        /// <summary>
        /// Src for production
        /// </summary>
        public string Src { get; init; }

        /// <summary>
        /// A value indicating whether the Src is local
        /// </summary>
        public bool IsLocal { get; init; }
    }

    #endregion
}