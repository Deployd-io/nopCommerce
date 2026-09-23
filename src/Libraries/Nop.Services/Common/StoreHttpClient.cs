using Microsoft.Extensions.Logging;
using Nop.Core;

namespace Nop.Services.Common;

/// <summary>
/// Represents the HTTP client to request current store
/// </summary>
public partial class StoreHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public StoreHttpClient(HttpClient client,
        IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering StoreHttpClient(client={Client},webHelper={WebHelper})", client, webHelper);
        //configure client
        client.BaseAddress = new Uri(webHelper.GetStoreLocation());

		_logger.LogWarning("<<< Exiting StoreHttpClient(client={Client},webHelper={WebHelper})", client, webHelper);
        _httpClient = client;
		_logger.LogDebug("StoreHttpClient(client={Client},webHelper={WebHelper}): _httpClient → {_httpClient}", client, webHelper, _httpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Keep the current store site alive
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result determines that request completed
    /// </returns>
    public virtual async Task KeepAliveAsync()
    {
		_logger.LogError(">>> Entering KeepAliveAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _httpClient.GetStringAsync(NopCommonDefaults.KeepAlivePath);
		_logger.LogError("KeepAliveAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting KeepAliveAsync()");
    }

    #endregion
}