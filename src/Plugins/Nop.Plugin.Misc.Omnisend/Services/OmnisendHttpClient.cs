using System.Net;
using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents HTTP client to request third-party services
/// </summary>
public class OmnisendHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly HttpClient _httpClient;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger _logger;
    private readonly IWorkContext _workContext;
    private readonly OmnisendSettings _omnisendSettings;

    #endregion

    #region Ctor

    public OmnisendHttpClient(HttpClient httpClient,
        ILocalizationService localizationService,
        ILogger logger,
        IWorkContext workContext,
        OmnisendSettings omnisendSettings)
    {
        //configure client
        httpClient.BaseAddress = new Uri(OmnisendDefaults.BaseApiUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(omnisendSettings.RequestTimeout ?? OmnisendDefaults.RequestTimeout);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, OmnisendDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

        _httpClient = httpClient;
        _localizationService = localizationService;
        _logger = logger;
        _workContext = workContext;
        _omnisendSettings = omnisendSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// HTTP request services
    /// </summary>
    /// <param name="apiUri">Request URL</param>
    /// <param name="data">Data to send</param>
    /// <param name="httpMethod">Request type. null == HttpMethod.Get</param>
    /// <returns>The asynchronous task whose result contains response details</returns>
    private async Task<string> RequestAsync(string apiUri, string data = null, HttpMethod httpMethod = null)
    {
		_logger.LogDebug(">>> Entering RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //specifies whether to skip BrandId parameter validation
        var skipBrandIdCheck = apiUri.Equals(OmnisendDefaults.AccountsApiUrl);

		_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): skipBrandIdCheck → {SkipBrandIdCheck}", apiUri, data, httpMethod, skipBrandIdCheck);
        if (string.IsNullOrEmpty(_omnisendSettings.BrandId) && !skipBrandIdCheck)
        {
            if (_omnisendSettings.LogRequestErrors)
                await _logger.InsertLogAsync(LogLevel.Error, $"{OmnisendDefaults.SystemName} configuration error", await _localizationService.GetResourceAsync("Plugins.Misc.Omnisend.CantGetBrandId"));

            return null;
        }

        if (httpMethod == null)
            httpMethod = HttpMethod.Get;

        var requestUri = new Uri(apiUri);

		_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): requestUri → {RequestUri}", apiUri, data, httpMethod, requestUri);
        if (_omnisendSettings.LogRequests)
        {
            var logMessage = $"{httpMethod.Method.ToUpper()} {requestUri.PathAndQuery}{Environment.NewLine}";
            logMessage += $"Host: {requestUri.Host}{Environment.NewLine}";
			_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): logMessage → {LogMessage}", apiUri, data, httpMethod, logMessage);
            if (httpMethod != HttpMethod.Get)
            {
                logMessage += $"Content-Type: {MimeTypes.ApplicationJson}{Environment.NewLine}";
                logMessage += $"{Environment.NewLine}{data}{Environment.NewLine}";
            }

            await _logger.InsertLogAsync(LogLevel.Debug, $"{OmnisendDefaults.SystemName} request details", logMessage);
        }

        var request = new HttpRequestMessage
        {
            Method = httpMethod,
            RequestUri = requestUri
        };

        request.Headers.TryAddWithoutValidation(OmnisendDefaults.ApiKeyHeader, ApiKey);

        if (httpMethod != HttpMethod.Get && !string.IsNullOrEmpty(data))
            request.Content = new StringContent(data, Encoding.UTF8, MimeTypes.ApplicationJson);

        var httpResponse = await _httpClient.SendAsync(request);
        var response = await httpResponse.Content.ReadAsStringAsync();
		_logger.LogInformation("RequestAsync(apiUri,data,httpMethod)={ApiUri},{Data},{HttpMethod}: external service call {ApiUri1} took {Arg4} ms", apiUri, data, httpMethod, apiUri, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): httpResponse → {HttpResponse}", apiUri, data, httpMethod, httpResponse);
		_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): response → {Response}", apiUri, data, httpMethod, response);

        if (_omnisendSettings.LogRequests)
        {
            var logMessage = $"Response:{Environment.NewLine}";
            logMessage += !httpResponse.IsSuccessStatusCode
                ? $"{httpResponse.StatusCode}: {httpResponse.RequestMessage?.ToString()}"
                : response;

            await _logger.InsertLogAsync(LogLevel.Debug, $"{OmnisendDefaults.SystemName} response details",
                logMessage);
        }

        switch (httpResponse.StatusCode)
        {
            case HttpStatusCode.OK:
            case HttpStatusCode.Accepted:
            case HttpStatusCode.NoContent:
                {
                    return response;
                }
            case HttpStatusCode.NotFound:
                return string.Empty;
            default:
                {
                    if (!string.IsNullOrEmpty(response))
                        throw new NopException(response);

                    throw new NopException("Omnisend unknown error.");
                }
        }
		_logger.LogDebug("<<< Exiting RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
    }

    private async Task<T> RequestAsync<T>(string apiUri, string data = null, HttpMethod httpMethod = null)
    {
        var response = await RequestAsync(apiUri, data, httpMethod);

        return string.IsNullOrEmpty(response) ? default : JsonConvert.DeserializeObject<T>(response);
    }

    private async Task<T> PerformRequestAsync<T>(Func<Task<T>> request)
    {
		_logger.LogDebug(">>> Entering PerformRequestAsync(request={Request})", request);
        try
        {
            var result = await request();

			_logger.LogDebug("PerformRequestAsync(request={Request}): result → {Result}", request, result);
            return result;
        }
        catch (Exception exception)
        {
            if (!_omnisendSettings.LogRequestErrors)
                return default;

            var errorMessage = exception.Message;

            var logMessage = $"{OmnisendDefaults.SystemName} error: {Environment.NewLine}{errorMessage}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());
			_logger.LogDebug("PerformRequestAsync(request={Request}): logMessage → {LogMessage}", request, logMessage);

            return default;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Perform request
    /// </summary>
    /// <param name="apiUri">REST API URL</param>
    /// <param name="data">Data to send</param>
    /// <param name="httpMethod">HTTP method</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the HTTP response data
    /// </returns>
    public async Task<string> PerformRequestAsync(string apiUri, string data = null, HttpMethod httpMethod = null)
    {
        return await PerformRequestAsync(async () => await RequestAsync(apiUri, data, httpMethod));
    }

    /// <summary>
    /// Perform request
    /// </summary>
    /// <param name="apiUri">REST API URL</param>
    /// <param name="data">Data to send</param>
    /// <param name="httpMethod">HTTP method</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the object
    /// </returns>
    public async Task<T> PerformRequestAsync<T>(string apiUri, string data = null, HttpMethod httpMethod = null)
    {
        return await PerformRequestAsync(async () => await RequestAsync<T>(apiUri, data, httpMethod));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the API key
    /// </summary>
    public string ApiKey
    {
        get => _omnisendSettings.ApiKey;
        set => _omnisendSettings.ApiKey = value;
    }

    #endregion
}