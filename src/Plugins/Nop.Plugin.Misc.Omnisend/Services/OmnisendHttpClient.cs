using System.Net;
using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Services.Localization;
using Nop.Services.Logging;

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
		_logger.LogError(">>> Entering OmnisendHttpClient(httpClient={HttpClient},localizationService={LocalizationService},logger={Logger},workContext={WorkContext},omnisendSettings={OmnisendSettings})", httpClient, localizationService, logger, workContext, omnisendSettings);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        httpClient.BaseAddress = new Uri(OmnisendDefaults.BaseApiUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(omnisendSettings.RequestTimeout ?? OmnisendDefaults.RequestTimeout);
		_logger.LogError("OmnisendHttpClient(httpClient,localizationService,logger,workContext,omnisendSettings)={HttpClient},{LocalizationService},{Logger},{WorkContext},{OmnisendSettings}: external service call took {Arg5} ms", httpClient, localizationService, logger, workContext, omnisendSettings, __logStart.ElapsedMilliseconds);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, OmnisendDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

		_logger.LogWarning("<<< Exiting OmnisendHttpClient(httpClient={HttpClient},localizationService={LocalizationService},logger={Logger},workContext={WorkContext},omnisendSettings={OmnisendSettings})", httpClient, localizationService, logger, workContext, omnisendSettings);
        _httpClient = httpClient;
		_logger.LogDebug("OmnisendHttpClient(httpClient={HttpClient},localizationService={LocalizationService},logger={Logger},workContext={WorkContext},omnisendSettings={OmnisendSettings}): _httpClient → {_httpClient}", httpClient, localizationService, logger, workContext, omnisendSettings, _httpClient);
        _localizationService = localizationService;
		_logger.LogDebug("OmnisendHttpClient(httpClient={HttpClient},localizationService={LocalizationService},logger={Logger},workContext={WorkContext},omnisendSettings={OmnisendSettings}): _localizationService → {_localizationService}", httpClient, localizationService, logger, workContext, omnisendSettings, _localizationService);
        _logger = logger;
        _workContext = workContext;
        _omnisendSettings = omnisendSettings;
		_logger.LogDebug("OmnisendHttpClient(httpClient={HttpClient},localizationService={LocalizationService},logger={Logger},workContext={WorkContext},omnisendSettings={OmnisendSettings}): _omnisendSettings → {_omnisendSettings}", httpClient, localizationService, logger, workContext, omnisendSettings, _omnisendSettings);
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
		_logger.LogError(">>> Entering RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //specifies whether to skip BrandId parameter validation
        var skipBrandIdCheck = apiUri.Equals(OmnisendDefaults.AccountsApiUrl);
		_logger.LogError("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): skipBrandIdCheck → {SkipBrandIdCheck}", apiUri, data, httpMethod, skipBrandIdCheck);

        if (string.IsNullOrEmpty(_omnisendSettings.BrandId) && !skipBrandIdCheck)
        {
            if (_omnisendSettings.LogRequestErrors)
                await _logger.InsertLogAsync(LogLevel.Error, $"{OmnisendDefaults.SystemName} configuration error", await _localizationService.GetResourceAsync("Plugins.Misc.Omnisend.CantGetBrandId"));

            return null;
        }

        if (httpMethod == null)
            httpMethod = HttpMethod.Get;

        var requestUri = new Uri(apiUri);
		_logger.LogError("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): requestUri → {RequestUri}", apiUri, data, httpMethod, requestUri);

        if (_omnisendSettings.LogRequests)
        {
            var logMessage = $"{httpMethod.Method.ToUpper()} {requestUri.PathAndQuery}{Environment.NewLine}";
			_logger.LogError("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): logMessage → {LogMessage}", apiUri, data, httpMethod, logMessage);
            logMessage += $"Host: {requestUri.Host}{Environment.NewLine}";
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
		_logger.LogError("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): httpResponse → {HttpResponse}", apiUri, data, httpMethod, httpResponse);
        var response = await httpResponse.Content.ReadAsStringAsync();
		_logger.LogError("RequestAsync(apiUri,data,httpMethod)={ApiUri},{Data},{HttpMethod}: external service call {ApiUri1} took {Arg4} ms", apiUri, data, httpMethod, apiUri, __logStart.ElapsedMilliseconds);
		_logger.LogError("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): response → {Response}", apiUri, data, httpMethod, response);

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
						_logger.LogError("Throwing NopException from RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
                        throw new NopException(response);

                    throw new NopException("Omnisend unknown error.");
                }
        }
		_logger.LogWarning("<<< Exiting RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
    }

    private async Task<T> RequestAsync<T>(string apiUri, string data = null, HttpMethod httpMethod = null)
    {
        var response = await RequestAsync(apiUri, data, httpMethod);

        return string.IsNullOrEmpty(response) ? default : JsonConvert.DeserializeObject<T>(response);
    }

    private async Task<T> PerformRequestAsync<T>(Func<Task<T>> request)
    {
		_logger.LogError(">>> Entering PerformRequestAsync(request={Request})", request);
        try
        {
            var result = await request();
			_logger.LogError("PerformRequestAsync(request={Request}): result → {Result}", request, result);

            return result;
        }
        catch (Exception exception)
        {
            if (!_omnisendSettings.LogRequestErrors)
                return default;

            var errorMessage = exception.Message;

            var logMessage = $"{OmnisendDefaults.SystemName} error: {Environment.NewLine}{errorMessage}";
			_logger.LogError("PerformRequestAsync(request={Request}): logMessage → {LogMessage}", request, logMessage);
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

            return default;
		_logger.LogWarning("<<< Exiting PerformRequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
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