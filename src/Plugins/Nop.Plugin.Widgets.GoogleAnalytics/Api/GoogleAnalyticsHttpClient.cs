using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api;

/// <summary>
/// Represents HTTP client to request Google Analytics 4 services
/// </summary>
public class GoogleAnalyticsHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public GoogleAnalyticsHttpClient(HttpClient httpClient)
    {
		logger.LogDebug(">>> Entering GoogleAnalyticsHttpClient(httpClient={HttpClient})", httpClient);
        _httpClient = httpClient;
		logger.LogDebug("<<< Exiting GoogleAnalyticsHttpClient(httpClient={HttpClient})", httpClient);
		logger.LogDebug("GoogleAnalyticsHttpClient(httpClient={HttpClient}): _httpClient → {_httpClient}", httpClient, _httpClient);
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
		logger.LogDebug(">>> Entering RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings})", request, googleAnalyticsSettings);
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

			logger.LogDebug("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): uri → {Uri}", request, googleAnalyticsSettings, uri);
            _httpClient.BaseAddress = new Uri(uri);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
			logger.LogInformation("RequestAsync(request,googleAnalyticsSettings)={Request},{GoogleAnalyticsSettings}: external service call {Uri} took {Arg3} ms", request, googleAnalyticsSettings, uri, __logStart.ElapsedMilliseconds);

            var requestString = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
			logger.LogDebug("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): requestString → {RequestString}", request, googleAnalyticsSettings, requestString);
			logger.LogDebug("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): requestContent → {RequestContent}", request, googleAnalyticsSettings, requestContent);
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), null as Uri) { Content = requestContent };
            var httpResponse = await _httpClient.SendAsync(requestMessage);
			logger.LogDebug("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): requestMessage → {RequestMessage}", request, googleAnalyticsSettings, requestMessage);
			logger.LogDebug("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): httpResponse → {HttpResponse}", request, googleAnalyticsSettings, httpResponse);
            httpResponse.EnsureSuccessStatusCode();

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response>(responseString);
			logger.LogDebug("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): responseString → {ResponseString}", request, googleAnalyticsSettings, responseString);
			logger.LogDebug("RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): result → {Result}", request, googleAnalyticsSettings, result);

            if (googleAnalyticsSettings.UseSandbox && (result?.ValidationMessages.Any() ?? false))
                throw new NopException($@"Google Analytics validation error (Measurement Protocol):
                        {responseString}");
        }
        catch (AggregateException exception)
        {
			logger.LogError("Exception in RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings}): {Arg2}", request, googleAnalyticsSettings, ex.Message);
            //rethrow actual exception
            throw exception.InnerException;
        }
		logger.LogDebug("<<< Exiting RequestAsync(request={Request},googleAnalyticsSettings={GoogleAnalyticsSettings})", request, googleAnalyticsSettings);
    }

    #endregion
}
