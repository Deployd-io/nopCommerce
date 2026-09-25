using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.WebOptimizer;

public partial class WebOptimizerConfig : IWebOptimizerOptions, IConfig
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// A value indicating whether JS file bundling and minification is enabled
    /// </summary>
    public bool EnableJavaScriptBundling { get; protected set; } = true;

    /// <summary>
    /// A value indicating whether CSS file bundling and minification is enabled
    /// </summary>
    public bool EnableCssBundling { get; protected set; } = true;

    /// <summary>
    /// Gets or sets a suffix for the js-file name of generated bundles
    /// </summary>
    public string JavaScriptBundleSuffix { get; protected set; } = ".scripts";

    /// <summary>
    /// Gets or sets a suffix for the css-file name of generated bundles
    /// </summary>
    public string CssBundleSuffix { get; protected set; } = ".styles";

    /// <summary>
    /// Gets a section name to load configuration
    /// </summary>
    [JsonIgnore]
    public string Name => "WebOptimizer";

    /// <summary>
    /// Gets an order of configuration
    /// </summary>
    /// <returns>Order</returns>
    public int GetOrder()
    {
        _logger.LogError(">>> Entering GetOrder()");
        _logger.LogWarning("<<< Exiting GetOrder()");
        return 2;
    }

    #region WebOptimizer options

    public bool? EnableCaching { get; set; } = true;
    public bool? EnableMemoryCache { get; set; } = true;
    public bool? EnableDiskCache { get; set; } = true;

    private string _cacheDirectory = string.Empty;
    public string CacheDirectory
    {
        get
        {
            if (string.IsNullOrEmpty(_cacheDirectory))
            {
                var fileProvider = EngineContext.Current.Resolve<INopFileProvider>() ?? CommonHelper.DefaultFileProvider;
                _cacheDirectory = fileProvider.Combine(fileProvider.MapPath("~/"), @"wwwroot\bundles");
            }

            return _cacheDirectory;
        }
        set => _cacheDirectory = value;
    }
    public bool? EnableTagHelperBundling { get; set; } = false;
    public string CdnUrl { get; set; } = "";
    public bool? AllowEmptyBundle { get; set; } = true;
    public HttpsCompressionMode HttpsCompression { get; set; } = HttpsCompressionMode.Compress;
    public TimeSpan MemoryCacheTimeToLive { get; set; } = TimeSpan.FromMinutes(60);

    #endregion

    #endregion
}