using Microsoft.Extensions.Logging;
using Nop.Core.Infrastructure;

namespace Nop.Core.Caching;

/// <summary>
/// Cache key manager
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class CacheKeyManager : ICacheKeyManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IConcurrentCollection<byte> _keys;

    public CacheKeyManager(IConcurrentCollection<byte> keys)
    {
		_logger.LogError(">>> Entering CacheKeyManager(keys={Keys})", keys);
        _keys = keys;
		_logger.LogDebug("CacheKeyManager(keys={Keys}): _keys → {_keys}", keys, _keys);
    _logger.LogWarning("<<< Exiting CacheKeyManager(keys={Keys})", keys);
    }

    /// <summary>
    /// Add the key
    /// </summary>
    /// <param name="key">The key to add</param>
    public virtual void AddKey(string key)
    {
		_logger.LogError(">>> Entering AddKey(key={Key})", key);
        _keys.Add(key, default);
		_logger.LogWarning("<<< Exiting AddKey(key={Key})", key);
    }

    /// <summary>
    /// Remove the key
    /// </summary>
    /// <param name="key">The key to remove</param>
    public virtual void RemoveKey(string key)
    {
		_logger.LogError(">>> Entering RemoveKey(key={Key})", key);
        _keys.Remove(key);
		_logger.LogWarning("<<< Exiting RemoveKey(key={Key})", key);
    }

    /// <summary>
    /// Remove all keys
    /// </summary>
    public virtual void Clear()
    {
		_logger.LogError(">>> Entering Clear()");
        _keys.Clear();
		_logger.LogWarning("<<< Exiting Clear()");
    }

    /// <summary>
    /// Remove keys by prefix
    /// </summary>
    /// <param name="prefix">Prefix to delete keys</param>
    /// <returns>The list of removed keys</returns>
    public virtual IEnumerable<string> RemoveByPrefix(string prefix)
    {
		_logger.LogError(">>> Entering RemoveByPrefix(prefix={Prefix})", prefix);
		_logger.LogWarning("<<< Exiting RemoveByPrefix(prefix={Prefix})", prefix);
        if (!_keys.Prune(prefix, out var subtree) || subtree?.Keys == null) {
            return Enumerable.Empty<string>();
        }

        return subtree.Keys;
    }

    /// <summary>
    /// The list of keys
    /// </summary>
    public IEnumerable<string> Keys => _keys.Keys;
}