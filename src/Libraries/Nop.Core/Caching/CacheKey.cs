using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Caching;

/// <summary>
/// Represents key for caching objects
/// </summary>
public partial class CacheKey
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Initialize a new instance with key and prefixes
    /// </summary>
    /// <param name="key">Key</param>
    public CacheKey(string key)
    {
		_logger.LogDebug(">>> Entering CacheKey(key={Key})", key);
        Key = key;
		_logger.LogDebug("<<< Exiting CacheKey(key={Key})", key);
		_logger.LogWarning("CacheKey(key={Key}): Key → {Key1}", key, Key);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a new instance from the current one and fill it with passed parameters
    /// </summary>
    /// <param name="createCacheKeyParameters">Function to create parameters</param>
    /// <param name="keyObjects">Objects to create parameters</param>
    /// <returns>Cache key</returns>
    public virtual CacheKey Create(Func<object, object> createCacheKeyParameters, params object[] keyObjects)
    {
        var cacheKey = new CacheKey(Key);

        if (!keyObjects.Any())
            return cacheKey;

        cacheKey.Key = string.Format(cacheKey.Key, keyObjects.Select(createCacheKeyParameters).ToArray());

        return cacheKey;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a cache key
    /// </summary>
    public string Key { get; protected set; }
    
    /// <summary>
    /// Gets or sets a cache time in minutes
    /// </summary>
    public int CacheTime { get; set; } = Singleton<AppSettings>.Instance.Get<CacheConfig>().DefaultCacheTime;

    #endregion
}