using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.Brevo.MarketingAutomation;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents HTTP client to request Brevo marketing automation services
/// </summary>
public class MarketingAutomationHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public MarketingAutomationHttpClient(HttpClient httpClient,
        BrevoSettings brevoSettings)
    {
		_logger.LogDebug(">>> Entering MarketingAutomationHttpClient(httpClient={HttpClient},brevoSettings={BrevoSettings})", httpClient, brevoSettings);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //configure client
        httpClient.BaseAddress = new Uri(BrevoDefaults.MarketingAutomationUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(10);
		_logger.LogInformation("MarketingAutomationHttpClient(httpClient,brevoSettings)={HttpClient},{BrevoSettings}: external service call took {Arg2} ms", httpClient, brevoSettings, __logStart.ElapsedMilliseconds);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, BrevoDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

        //authentication
        httpClient.DefaultRequestHeaders.Add(BrevoDefaults.MarketingAutomationKeyHeader, brevoSettings.MarketingAutomationKey);

		_logger.LogDebug("<<< Exiting MarketingAutomationHttpClient(httpClient={HttpClient},brevoSettings={BrevoSettings})", httpClient, brevoSettings);
        _httpClient = httpClient;
		_logger.LogDebug("MarketingAutomationHttpClient(httpClient={HttpClient},brevoSettings={BrevoSettings}): _httpClient → {_httpClient}", httpClient, brevoSettings, _httpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Request API service
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <param name="request">Request</param>
    /// <returns>The asynchronous task whose result contains response details</returns>
    public async Task RequestAsync<TRequest>(TRequest request) where TRequest : Request
    {
		_logger.LogDebug(">>> Entering RequestAsync(request={Request})", request);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var requestString = JsonConvert.SerializeObject(request);
			_logger.LogDebug("RequestAsync(request={Request}): requestString → {RequestString}", request, requestString);
			_logger.LogInformation("RequestAsync(request)={Request}: external service call took {Arg1} ms", request, __logStart.ElapsedMilliseconds);
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
			_logger.LogDebug("RequestAsync(request={Request}): requestContent → {RequestContent}", request, requestContent);
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), request.Path) { Content = requestContent };
			_logger.LogDebug("RequestAsync(request={Request}): requestMessage → {RequestMessage}", request, requestMessage);
            var httpResponse = await _httpClient.SendAsync(requestMessage);
			_logger.LogDebug("RequestAsync(request={Request}): httpResponse → {HttpResponse}", request, httpResponse);
            httpResponse.EnsureSuccessStatusCode();
        }
        catch (AggregateException exception)
        {
			_logger.LogError("Exception in RequestAsync(request={Request}): {Arg1}", request, exception.Message);
			_logger.LogWarning("Throwing InnerException from RequestAsync(request={Request})", request);
            //rethrow actual exception
		_logger.LogDebug("<<< Exiting RequestAsync(request={Request})", request);
            throw exception.InnerException;
        }
    }

    #endregion
}