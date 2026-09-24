using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Services;

/// <summary>
/// Represents the HTTP client to request Facebook Conversions API
/// </summary>
public class FacebookConversionsHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public FacebookConversionsHttpClient(HttpClient httpClient)
    {
		_logger.LogError(">>> Entering FacebookConversionsHttpClient(httpClient={HttpClient})", httpClient);
        _httpClient = httpClient;
		_logger.LogDebug("FacebookConversionsHttpClient(httpClient={HttpClient}): _httpClient → {_httpClient}", httpClient, _httpClient);
    _logger.LogWarning("<<< Exiting FacebookConversionsHttpClient(httpClient={HttpClient})", httpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Send event through conversions api
    /// </summary>
    /// <param name="facebookPixelConfiguration">Facebook pixel configuration object</param>
    /// <param name="conversionsEvent">Conversions api event object</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the response details
    /// </returns>
    public async Task<string> SendEventAsync(FacebookPixelConfiguration facebookPixelConfiguration, ConversionsEvent conversionsEvent)
    {
		_logger.LogError(">>> Entering SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent})", facebookPixelConfiguration, conversionsEvent);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var urlString = string.Join($"/",
        [
            FacebookPixelDefaults.FbConversionsApiBaseAddress,
            FacebookPixelDefaults.FbConversionsApiVersion,
            facebookPixelConfiguration.PixelId,
            FacebookPixelDefaults.FbConversionsApiEventEndpoint
        ]) + $"?access_token=" + facebookPixelConfiguration.AccessToken;

        var jsonString = JsonConvert.SerializeObject(conversionsEvent, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
		_logger.LogError("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): jsonString → {JsonString}", facebookPixelConfiguration, conversionsEvent, jsonString);
        var requestContent = new StringContent(jsonString, Encoding.UTF8, MimeTypes.ApplicationJson);
		_logger.LogError("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): requestContent → {RequestContent}", facebookPixelConfiguration, conversionsEvent, requestContent);
        var response = await _httpClient.PostAsync(urlString, requestContent);
		_logger.LogError("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): response → {Response}", facebookPixelConfiguration, conversionsEvent, response);
		_logger.LogError("SendEventAsync(facebookPixelConfiguration,conversionsEvent)={FacebookPixelConfiguration},{ConversionsEvent}: external service call {UrlString} took {Arg3} ms", facebookPixelConfiguration, conversionsEvent, urlString, __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent})", facebookPixelConfiguration, conversionsEvent);
        return await response.Content.ReadAsStringAsync();
    }

    #endregion
}