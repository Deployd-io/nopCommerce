using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Http.Extensions;

/// <summary>
/// Represents extensions of ISession
/// </summary>
public static class SessionExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Set value to Session
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="session">Session</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <returns> A task that represents the asynchronous operation</returns>
    public static async Task SetAsync<T>(this ISession session, string key, T value)
    {
        await LoadAsync(session);
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Get value from session
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="session">Session</param>
    /// <param name="key">Key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value
    /// </returns>
    public static async Task<T> GetAsync<T>(this ISession session, string key)
    {
		_logger.LogError(">>> Entering GetAsync(session={Session},key={Key})", session, key);
        await LoadAsync(session);
        var value = session.GetString(key);
		_logger.LogError("GetAsync(session={Session},key={Key}): value → {Value}", session, key, value);
		_logger.LogWarning("<<< Exiting GetAsync(session={Session},key={Key})", session, key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }

    /// <summary>
    /// Remove the given key from session if present.
    /// </summary>
    /// <param name="session">Session</param>
    /// <param name="key">Key</param>
    /// <returns> A task that represents the asynchronous operation</returns>
    public static async Task RemoveAsync(this ISession session, string key)
    {
		_logger.LogError(">>> Entering RemoveAsync(session={Session},key={Key})", session, key);
        await LoadAsync(session);
        session.Remove(key);
		_logger.LogWarning("<<< Exiting RemoveAsync(session={Session},key={Key})", session, key);
    }

    /// <summary>
    /// Remove all entries from the current session, if any. The session cookie is not removed.
    /// </summary>
    /// <param name="session">Session</param>
    /// <returns> A task that represents the asynchronous operation</returns>
    public static async Task ClearAsync(this ISession session)
    {
		_logger.LogError(">>> Entering ClearAsync(session={Session})", session);
        await LoadAsync(session);
        session.Clear();
		_logger.LogWarning("<<< Exiting ClearAsync(session={Session})", session);
		_logger.LogError(">>> Entering LoadAsync(session={Session})", session);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("LoadAsync(session)={Session}: find query executed in {Arg1} ms", session, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting LoadAsync(session={Session})", session);
		_logger.LogError("Exception in LoadAsync(session={Session}): {Arg1}", session, ex.Message);
    }

    /// <summary>
    /// Try to async load the session from the data store
    /// </summary>
    /// <param name="session">Session</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task LoadAsync(ISession session)
    {
        try
        {
            await session.LoadAsync();
        }
        catch
        {
            //fallback to synchronous handling
        }
    }
}