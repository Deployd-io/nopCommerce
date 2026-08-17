using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Web.Framework.WebOptimizer.Processors;
using WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.WebOptimizer;

/// <summary>
/// Represents a bundle helper 
/// </summary>
public partial class NopAssetHelper : INopAssetHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IAssetPipeline _assetPipeline;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WebOptimizerConfig _webOptimizerConfig;

    #endregion

    #region Ctor


    public NopAssetHelper(AppSettings appSettings,
        IAssetPipeline assetPipeline,
        IHttpContextAccessor httpContextAccessor)
    {
		_logger.LogDebug(">>> Entering NopAssetHelper(appSettings={AppSettings},assetPipeline={AssetPipeline},httpContextAccessor={HttpContextAccessor})", appSettings, assetPipeline, httpContextAccessor);
        _assetPipeline = assetPipeline;
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("NopAssetHelper(appSettings={AppSettings},assetPipeline={AssetPipeline},httpContextAccessor={HttpContextAccessor}): _httpContextAccessor → {_httpContextAccessor}", appSettings, assetPipeline, httpContextAccessor, _httpContextAccessor);
        _webOptimizerConfig = appSettings.Get<WebOptimizerConfig>();
		_logger.LogDebug("<<< Exiting NopAssetHelper(appSettings={AppSettings},assetPipeline={AssetPipeline},httpContextAccessor={HttpContextAccessor})", appSettings, assetPipeline, httpContextAccessor);
		_logger.LogDebug("NopAssetHelper(appSettings={AppSettings},assetPipeline={AssetPipeline},httpContextAccessor={HttpContextAccessor}): _webOptimizerConfig → {_webOptimizerConfig}", appSettings, assetPipeline, httpContextAccessor, _webOptimizerConfig);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get or create a JavaScript bundle
    /// </summary>
    /// <param name="bundleKey">A unique route to bundle</param>
    /// <param name="sourceFiles">A list of relative file names of the sources to optimize</param>
    public IAsset GetOrCreateJavaScriptAsset(string bundleKey, params string[] sourceFiles)
    {
		_logger.LogDebug(">>> Entering GetOrCreateJavaScriptAsset(bundleKey={BundleKey},sourceFiles={SourceFiles})", bundleKey, sourceFiles);
        ArgumentException.ThrowIfNullOrEmpty(bundleKey);

        if (sourceFiles.Length == 0)
            sourceFiles = [bundleKey];

        if (!_assetPipeline.TryGetAssetFromRoute(bundleKey, out var asset))
        {
            asset = _assetPipeline.AddBundle(bundleKey, $"{MimeTypes.TextJavascript}; charset=UTF-8", sourceFiles)
                .EnforceFileExtensions(".js", ".es5", ".es6")
                .AddResponseHeader(HeaderNames.XContentTypeOptions, "nosniff");

            //to more precisely log problem files we minify them before concatenating
            asset.Processors.Add(new NopJsMinifier());

            asset.Concatenate();
        }
        else if (asset.SourceFiles.Count != sourceFiles.Length || !asset.SourceFiles.SequenceEqual(sourceFiles))
        {
            asset.SourceFiles.Clear();
            foreach (var source in sourceFiles)
                asset.TryAddSourceFile(source);
        }

		_logger.LogDebug("<<< Exiting GetOrCreateJavaScriptAsset(bundleKey={BundleKey},sourceFiles={SourceFiles})", bundleKey, sourceFiles);
        return asset;
    }

    /// <summary>
    /// Get or create a CSS bundle
    /// </summary>
    /// <param name="bundleKey">A unique route to bundle</param>
    /// <param name="sourceFiles">A list of relative file names of the sources to optimize</param>
    public IAsset GetOrCreateCssAsset(string bundleKey, params string[] sourceFiles)
    {
		_logger.LogDebug(">>> Entering GetOrCreateCssAsset(bundleKey={BundleKey},sourceFiles={SourceFiles})", bundleKey, sourceFiles);
        ArgumentException.ThrowIfNullOrEmpty(bundleKey);

        if (sourceFiles.Length == 0)
            sourceFiles = [bundleKey];

        if (!_assetPipeline.TryGetAssetFromRoute(bundleKey, out var asset))
        {
            asset = _assetPipeline.AddBundle(bundleKey, $"{MimeTypes.TextCss}; charset=UTF-8", sourceFiles)
                .EnforceFileExtensions(".css")
                .AdjustRelativePaths()
                .AddResponseHeader(HeaderNames.XContentTypeOptions, "nosniff");

            //to more precisely log problem files we minify them before concatenating
            asset.Processors.Add(new NopCssMinifier());

            asset.Concatenate();
        }
        else if (asset.SourceFiles.Count != sourceFiles.Length || !asset.SourceFiles.SequenceEqual(sourceFiles))
        {
            asset.SourceFiles.Clear();
            foreach (var source in sourceFiles)
                asset.TryAddSourceFile(source);
        }

		_logger.LogDebug("<<< Exiting GetOrCreateCssAsset(bundleKey={BundleKey},sourceFiles={SourceFiles})", bundleKey, sourceFiles);
        return asset;
    }

    /// <summary>
    /// Return a bundle route with cache busting
    /// </summary>
    /// <param name="asset">Bundle</param>
    public string CacheBusting(IAsset asset)
    {
		_logger.LogDebug(">>> Entering CacheBusting(asset={Asset})", asset);
        ArgumentNullException.ThrowIfNull(asset);

        try
        {
            var hash = asset.GenerateCacheKey(_httpContextAccessor.HttpContext, _webOptimizerConfig);
			_logger.LogDebug("CacheBusting(asset={Asset}): hash → {Hash}", asset, hash);
            return QueryHelpers.AddQueryString(asset.Route, "v", hash);
        }
        catch
        {
			_logger.LogError("Exception in CacheBusting(asset={Asset}): {Arg1}", asset, ex.Message);
            //in some cases WO can't handle assets (e.g. IIS sub-application with PathBase)
			_logger.LogDebug("<<< Exiting CacheBusting(asset={Asset})", asset);
            return asset.Route;
        }
    }

    #endregion
}