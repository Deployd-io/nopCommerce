using Microsoft.Extensions.Logging;
﻿using Nop.Core.Infrastructure;

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
		_logger.LogDebug(">>> Entering CacheKeyManager(keys={Keys})", keys);
        _keys = keys;
		_logger.LogDebug("<<< Exiting CacheKeyManager(keys={Keys})", keys);
		_logger.LogDebug("CacheKeyManager(keys={Keys}): _keys → {_keys}", keys, _keys);
    }

    /// <summary>
    /// Add the key
    /// </summary>
    /// <param name="key">The key to add</param>
    public virtual void AddKey(string key)
    {
		_logger.LogDebug(">>> Entering AddKey(key={Key})", key);
        _keys.Add(key, default);
		_logger.LogDebug("<<< Exiting AddKey(key={Key})", key);
    }

    /// <summary>
    /// Remove the key
    /// </summary>
    /// <param name="key">The key to remove</param>
    public virtual void RemoveKey(string key)
    {
		_logger.LogDebug(">>> Entering RemoveKey(key={Key})", key);
        _keys.Remove(key);
		_logger.LogDebug("<<< Exiting RemoveKey(key={Key})", key);
    }

    /// <summary>
    /// Remove all keys
    /// </summary>
    public virtual void Clear()
    {
		_logger.LogDebug(">>> Entering Clear()");
        _keys.Clear();
		_logger.LogDebug("<<< Exiting Clear()");
    }

    /// <summary>
    /// Remove keys by prefix
    /// </summary>
    /// <param name="prefix">Prefix to delete keys</param>
    /// <returns>The list of removed keys</returns>
    public virtual IEnumerable<string> RemoveByPrefix(string prefix)
    {
		_logger.LogDebug(">>> Entering RemoveByPrefix(prefix={Prefix})", prefix);
        if (!_keys.Prune(prefix, out var subtree) || subtree?.Keys == null)
		_logger.LogDebug("<<< Exiting RemoveByPrefix(prefix={Prefix})", prefix);
            return Enumerable.Empty<string>();

        return subtree.Keys;
    }

    /// <summary>
    /// The list of keys
    /// </summary>
    public IEnumerable<string> Keys => _keys.Keys;
}