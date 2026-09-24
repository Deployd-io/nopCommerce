using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api;

/// <summary>
/// Represents HTTP client to request Google Analytics 4 services
/// </summary>
public class GoogleAnalyticsHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public GoogleAnalyticsHttpClient(HttpClient httpClient)
    {
		_logger.LogError(">>> Entering GoogleAnalyticsHttpClient(httpClient={HttpClient})", httpClient);
        _httpClient = httpClient;
		_logger.LogDebug("GoogleAnalyticsHttpClient(httpClient={HttpClient}): _httpClient → {_httpClient}", httpClient, _httpClient);
    _logger.LogWarning("<<< Exiting GoogleAnalyticsHttpClient(httpClient={HttpClient})", httpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Request API service
    /// </summary>
    /// <param name="request">Event Request</param>
    /// <param name="googleAnalyticsSettings">Google Analytics settings</param>
    /// <returns>The asynchronous task whose result contains response details</returns>
    public async Task RequestAsync(EventRequest request, GoogleAnalyticsSettings googleAnalyticsSettings)
    {
		_logger.LogError(">>> Entering RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings})", request, googleAnalyticsSettings);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //configure client
        try
        {
            var query = new Dictionary<string, string>()
            {
                ["api_secret"] = googleAnalyticsSettings.ApiSecret,
                ["measurement_id"] = googleAnalyticsSettings.GoogleId
            };

            var uri = QueryHelpers.AddQueryString(googleAnalyticsSettings.UseSandbox ? GoogleAnalyticsDefaults.EndPointDebugUrl : GoogleAnalyticsDefaults.EndPointUrl, query);
			_logger.LogError("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): uri → {Uri}", request, googleAnalyticsSettings, uri);

            _httpClient.BaseAddress = new Uri(uri);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
			_logger.LogError("RequestAsync(request,googleAnalyticsSettings)={Request},{GoogleAnalyticsSettings}: external service call {Uri} took {Arg3} ms", request, googleAnalyticsSettings, uri, __logStart.ElapsedMilliseconds);

            var requestString = JsonConvert.SerializeObject(request);
			_logger.LogError("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): requestString → {RequestString}", request, googleAnalyticsSettings, requestString);
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
			_logger.LogError("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): requestContent → {RequestContent}", request, googleAnalyticsSettings, requestContent);
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), null as Uri) { Content = requestContent };
			_logger.LogError("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): requestMessage → {RequestMessage}", request, googleAnalyticsSettings, requestMessage);
            var httpResponse = await _httpClient.SendAsync(requestMessage);
			_logger.LogError("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): httpResponse → {HttpResponse}", request, googleAnalyticsSettings, httpResponse);
            httpResponse.EnsureSuccessStatusCode();

            var responseString = await httpResponse.Content.ReadAsStringAsync();
			_logger.LogError("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): responseString → {ResponseString}", request, googleAnalyticsSettings, responseString);
            var result = JsonConvert.DeserializeObject<Response>(responseString);
			_logger.LogError("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): result → {Result}", request, googleAnalyticsSettings, result);

            if (googleAnalyticsSettings.UseSandbox && (result?.ValidationMessages.Any() ?? false))
                throw new NopException($@"Google Analytics validation error (Measurement Protocol):
                        {responseString}");
        }
        catch (AggregateException exception)
        {
			_logger.LogError("Throwing NopException from RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings})", request, googleAnalyticsSettings);
			_logger.LogError("Exception in RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): {Arg2}", request, googleAnalyticsSettings, exception.Message);
            //rethrow actual exception
			_logger.LogError("Throwing InnerException from RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings})", request, googleAnalyticsSettings);
		_logger.LogWarning("<<< Exiting RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings})", request, googleAnalyticsSettings);
            throw exception.InnerException;
        }
    }

    #endregion
}