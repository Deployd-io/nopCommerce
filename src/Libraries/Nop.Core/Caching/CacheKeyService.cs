using System.Globalization;
using System.Text;
using Nop.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Caching;

/// <summary>
/// Represents the default cache key service implementation
/// </summary>
public abstract partial class CacheKeyService : ICacheKeyService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AppSettings _appSettings;

    #endregion

    #region Ctor

    protected CacheKeyService(AppSettings appSettings)
    {
		_logger.LogDebug(">>> Entering CacheKeyService(appSettings={AppSettings})", appSettings);
        _appSettings = appSettings;
		_logger.LogDebug("CacheKeyService(appSettings={AppSettings}): _appSettings → {_appSettings}", appSettings, _appSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare the cache key prefix
    /// </summary>
    /// <param name="prefix">Cache key prefix</param>
    /// <param name="prefixParameters">Parameters to create cache key prefix</param>
    protected virtual string PrepareKeyPrefix(string prefix, params object[] prefixParameters)
    {
		_logger.LogDebug(">>> Entering PrepareKeyPrefix(prefix={Prefix},prefixParameters={PrefixParameters})", prefix, prefixParameters);
        return prefixParameters?.Any() ?? false
            ? string.Format(prefix, prefixParameters.Select(CreateCacheKeyParameters).ToArray())
            : prefix;
    }

    /// <summary>
    /// Create the hash value of the passed identifiers
    /// </summary>
    /// <param name="ids">Collection of identifiers</param>
    /// <returns>String hash value</returns>
    protected virtual string CreateIdsHash(IEnumerable<int> ids)
    {
		_logger.LogDebug(">>> Entering CreateIdsHash(ids={Ids})", ids);
        var identifiers = ids.ToList();

		_logger.LogDebug("CreateIdsHash(ids={Ids}): identifiers → {Identifiers}", ids, identifiers);
        if (!identifiers.Any())
            return string.Empty;

        var identifiersString = string.Join(", ", identifiers.OrderBy(id => id));
		_logger.LogDebug("CreateIdsHash(ids={Ids}): identifiersString → {IdentifiersString}", ids, identifiersString);
		_logger.LogDebug("<<< Exiting CreateIdsHash(ids={Ids})", ids);
        return HashHelper.CreateHash(Encoding.UTF8.GetBytes(identifiersString), HashAlgorithm);
    }

    /// <summary>
    /// Converts an object to cache parameter
    /// </summary>
    /// <param name="parameter">Object to convert</param>
    /// <returns>Cache parameter</returns>
    protected virtual object CreateCacheKeyParameters(object parameter)
    {
		_logger.LogDebug(">>> Entering CreateCacheKeyParameters(parameter={Parameter})", parameter);
		_logger.LogDebug("<<< Exiting CreateCacheKeyParameters(parameter={Parameter})", parameter);
        return parameter switch
        {
            null => "null",
            IEnumerable<int> ids => CreateIdsHash(ids),
            IEnumerable<BaseEntity> entities => CreateIdsHash(entities.Select(entity => entity.Id)),
            BaseEntity entity => entity.Id,
            decimal param => param.ToString(CultureInfo.InvariantCulture),
            _ => parameter
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a copy of cache key and fills it by passed parameters
    /// </summary>
    /// <param name="cacheKey">Initial cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>Cache key</returns>
    public virtual CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters)
    {
		_logger.LogDebug(">>> Entering PrepareKey(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
        return cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);
    }

    /// <summary>
    /// Create a copy of cache key using the default cache time and fills it by passed parameters
    /// </summary>
    /// <param name="cacheKey">Initial cache key</param>
    /// <param name="cacheKeyParameters">Parameters to create cache key</param>
    /// <returns>Cache key</returns>
    public virtual CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters)
    {
		_logger.LogDebug(">>> Entering PrepareKeyForDefaultCache(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
        var key = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);

		_logger.LogWarning("PrepareKeyForDefaultCache(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters}): key → {Key}", cacheKey, cacheKeyParameters, key);
        key.CacheTime = _appSettings.Get<CacheConfig>().DefaultCacheTime;

		_logger.LogDebug("<<< Exiting PrepareKeyForDefaultCache(cacheKey={CacheKey},cacheKeyParameters={CacheKeyParameters})", cacheKey, cacheKeyParameters);
        return key;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets an algorithm used to create the hash value of identifiers need to cache
    /// </summary>
    protected string HashAlgorithm => "SHA1";

    #endregion
}