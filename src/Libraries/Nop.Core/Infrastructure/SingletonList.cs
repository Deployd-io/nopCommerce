using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Infrastructure;

/// <summary>
/// Provides a singleton list for a certain type.
/// </summary>
/// <typeparam name="T">The type of list to store.</typeparam>
public partial class SingletonList<T> : Singleton<IList<T>>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    static SingletonList()
    {
		_logger.LogDebug(">>> Entering SingletonList()");
        Singleton<IList<T>>.Instance = new List<T>();
		_logger.LogDebug("<<< Exiting SingletonList()");
    }

    /// <summary>
    /// The singleton instance for the specified type T. Only one instance (at the time) of this list for each type of T.
    /// </summary>
    public static new IList<T> Instance => Singleton<IList<T>>.Instance;
}