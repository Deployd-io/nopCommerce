using Microsoft.AspNetCore.Http;
using WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.WebOptimizer;

/// <summary>
/// Represents the default bundle implementation
/// </summary>
public partial class DefaultAsset : IAsset
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public DefaultAsset(string route, string contentType)
    {
		_logger.LogError(">>> Entering DefaultAsset(route={Route},contentType={ContentType})", route, contentType);
        Route = route;
        ContentType = contentType;
		_logger.LogDebug("DefaultAsset(route={Route},contentType={ContentType}): ContentType → {ContentType1}", route, contentType, ContentType);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the processors and returns the modified content
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="options">Configuration options</param>
    public virtual Task<byte[]> ExecuteAsync(HttpContext context, IWebOptimizerOptions options)
    {
		_logger.LogError(">>> Entering ExecuteAsync(context={Context},options={Options})", context, options);
		_logger.LogError("Throwing NotImplementedException from ExecuteAsync(context={Context},options={Options})", context, options);
    _logger.LogWarning("<<< Exiting ExecuteAsync(context={Context},options={Options})", context, options);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the cache key associated with this version of the asset
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="options">Configuration options</param>
    /// <returns></returns>
    public virtual string GenerateCacheKey(HttpContext context, IWebOptimizerOptions options)
    {
		_logger.LogError(">>> Entering GenerateCacheKey(context={Context},options={Options})", context, options);
        _logger.LogWarning("<<< Exiting GenerateCacheKey(context={Context},options={Options})", context, options);
        return string.Empty;
    }

    /// <summary>
    /// Adds a source file to the asset
    /// </summary>
    /// <param name="route">Relative path of a source file</param>
    public virtual void TryAddSourceFile(string route)
    {
		_logger.LogError(">>> Entering TryAddSourceFile(route={Route})", route);
		_logger.LogError("Throwing NotImplementedException from TryAddSourceFile(route={Route})", route);
    _logger.LogWarning("<<< Exiting TryAddSourceFile(route={Route})", route);
        throw new NotImplementedException();
    }
    
    #endregion

    #region Properties

    /// <summary>
    /// Gets the content type produced by the transform
    /// </summary>
    public string ContentType { get; init; }

    /// <summary>
    /// Gets a list of processors
    /// </summary>
    public IList<IProcessor> Processors => new List<IProcessor>();

    /// <summary>
    /// Gets the items collection for the asset
    /// </summary>
    public IDictionary<string, object> Items => new Dictionary<string, object>();

    /// <summary>
    /// Gets the route to the bundle output
    /// </summary>
    public string Route { get; init; }

    /// <summary>
    /// Gets files to exclude from output results
    /// </summary>
    public IList<string> ExcludeFiles => new List<string>();

    /// <summary>
    /// Gets the webroot relative source files
    /// </summary>
    public IList<string> SourceFiles => new List<string>();

    #endregion
}