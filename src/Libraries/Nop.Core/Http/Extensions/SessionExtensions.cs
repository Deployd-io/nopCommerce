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
		_logger.LogDebug(">>> Entering GetAsync(session={Session},key={Key})", session, key);
        await LoadAsync(session);
        var value = session.GetString(key);
		_logger.LogDebug("GetAsync(session={Session},key={Key}): value → {Value}", session, key, value);
		_logger.LogDebug("<<< Exiting GetAsync(session={Session},key={Key})", session, key);
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
		_logger.LogDebug(">>> Entering RemoveAsync(session={Session},key={Key})", session, key);
        await LoadAsync(session);
        session.Remove(key);
		_logger.LogDebug("<<< Exiting RemoveAsync(session={Session},key={Key})", session, key);
    }

    /// <summary>
    /// Remove all entries from the current session, if any. The session cookie is not removed.
    /// </summary>
    /// <param name="session">Session</param>
    /// <returns> A task that represents the asynchronous operation</returns>
    public static async Task ClearAsync(this ISession session)
    {
		_logger.LogDebug(">>> Entering ClearAsync(session={Session})", session);
        await LoadAsync(session);
        session.Clear();
		_logger.LogDebug("<<< Exiting ClearAsync(session={Session})", session);
		_logger.LogDebug(">>> Entering LoadAsync(session={Session})", session);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("LoadAsync(session)={Session}: find query executed in {Arg1} ms", session, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting LoadAsync(session={Session})", session);
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
			_logger.LogError("Exception in LoadAsync(session={Session}): {Arg1}", session, ex.Message);
            //fallback to synchronous handling
        }
    }
}