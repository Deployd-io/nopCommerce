using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.Brevo.MarketingAutomation;

namespace Nop.Plugin.Misc.Brevo.Services;

/// <summary>
/// Represents HTTP client to request Brevo marketing automation services
/// </summary>
public class MarketingAutomationHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public MarketingAutomationHttpClient(HttpClient httpClient,
        BrevoSettings brevoSettings)
    {
        //configure client
        httpClient.BaseAddress = new Uri(BrevoDefaults.MarketingAutomationUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, BrevoDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

        //authentication
        httpClient.DefaultRequestHeaders.Add(BrevoDefaults.MarketingAutomationKeyHeader, brevoSettings.MarketingAutomationKey);

        _httpClient = httpClient;
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
		logger.LogDebug(">>> Entering RequestAsync(request={Request})", request);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var requestString = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
			logger.LogDebug("RequestAsync(request={Request}): requestString → {RequestString}", request, requestString);
			logger.LogDebug("RequestAsync(request={Request}): requestContent → {RequestContent}", request, requestContent);
			logger.LogInformation("RequestAsync(request)={Request}: external service call took {Arg1} ms", request, __logStart.ElapsedMilliseconds);
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), request.Path) { Content = requestContent };
            var httpResponse = await _httpClient.SendAsync(requestMessage);
			logger.LogDebug("RequestAsync(request={Request}): requestMessage → {RequestMessage}", request, requestMessage);
            httpResponse.EnsureSuccessStatusCode();
        }
		logger.LogDebug("RequestAsync(request={Request}): httpResponse → {HttpResponse}", request, httpResponse);
        catch (AggregateException exception)
        {
			logger.LogError("Exception in RequestAsync(request={Request}): {Arg1}", request, ex.Message);
            //rethrow actual exception
            throw exception.InnerException;
        }
		logger.LogDebug("<<< Exiting RequestAsync(request={Request})", request);
    }

    #endregion
}