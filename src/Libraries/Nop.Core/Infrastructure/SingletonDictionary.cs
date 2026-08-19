using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Infrastructure;

/// <summary>
/// Provides a singleton dictionary for a certain key and value type.
/// </summary>
/// <typeparam name="TKey">The type of key.</typeparam>
/// <typeparam name="TValue">The type of value.</typeparam>
public partial class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    static SingletonDictionary()
    {
		_logger.LogDebug(">>> Entering SingletonDictionary()");
        Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
		_logger.LogDebug("<<< Exiting SingletonDictionary()");
    }

    /// <summary>
    /// The singleton instance for the specified type T. Only one instance (at the time) of this dictionary for each type of T.
    /// </summary>
    public static new IDictionary<TKey, TValue> Instance => Singleton<Dictionary<TKey, TValue>>.Instance;
}