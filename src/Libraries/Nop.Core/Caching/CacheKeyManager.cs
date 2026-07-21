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
    protected readonly IConcurrentCollection<byte> _keys;

    public CacheKeyManager(IConcurrentCollection<byte> keys)
    {
		logger.LogDebug(">>> Entering CacheKeyManager(keys={Keys})", keys);
        _keys = keys;
		logger.LogDebug("<<< Exiting CacheKeyManager(keys={Keys})", keys);
		logger.LogDebug("CacheKeyManager(keys={Keys}): _keys → {_keys}", keys, _keys);
    }

    /// <summary>
    /// Add the key
    /// </summary>
    /// <param name="key">The key to add</param>
    public virtual void AddKey(string key)
    {
		logger.LogDebug(">>> Entering AddKey(key={Key})", key);
        _keys.Add(key, default);
		logger.LogDebug("<<< Exiting AddKey(key={Key})", key);
    }

    /// <summary>
    /// Remove the key
    /// </summary>
    /// <param name="key">The key to remove</param>
    public virtual void RemoveKey(string key)
    {
		logger.LogDebug(">>> Entering RemoveKey(key={Key})", key);
        _keys.Remove(key);
		logger.LogDebug("<<< Exiting RemoveKey(key={Key})", key);
    }

    /// <summary>
    /// Remove all keys
    /// </summary>
    public virtual void Clear()
    {
		logger.LogDebug(">>> Entering Clear()");
        _keys.Clear();
		logger.LogDebug("<<< Exiting Clear()");
    }

    /// <summary>
    /// Remove keys by prefix
    /// </summary>
    /// <param name="prefix">Prefix to delete keys</param>
    /// <returns>The list of removed keys</returns>
    public virtual IEnumerable<string> RemoveByPrefix(string prefix)
    {
		logger.LogDebug(">>> Entering RemoveByPrefix(prefix={Prefix})", prefix);
        if (!_keys.Prune(prefix, out var subtree) || subtree?.Keys == null)
		logger.LogDebug("<<< Exiting RemoveByPrefix(prefix={Prefix})", prefix);
            return Enumerable.Empty<string>();

        return subtree.Keys;
    }

    /// <summary>
    /// The list of keys
    /// </summary>
    public IEnumerable<string> Keys => _keys.Keys;
}
