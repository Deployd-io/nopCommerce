using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.FacebookPixel.Domain;

namespace Nop.Plugin.Widgets.FacebookPixel.Services;

/// <summary>
/// Represents the HTTP client to request Facebook Conversions API
/// </summary>
public class FacebookConversionsHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public FacebookConversionsHttpClient(HttpClient httpClient)
    {
		logger.LogDebug(">>> Entering FacebookConversionsHttpClient(httpClient={HttpClient})", httpClient);
        _httpClient = httpClient;
		logger.LogDebug("<<< Exiting FacebookConversionsHttpClient(httpClient={HttpClient})", httpClient);
		logger.LogDebug("FacebookConversionsHttpClient(httpClient={HttpClient}): _httpClient → {_httpClient}", httpClient, _httpClient);
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
		logger.LogDebug(">>> Entering SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent})", facebookPixelConfiguration, conversionsEvent);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var urlString = string.Join($"/",
        [
            FacebookPixelDefaults.FbConversionsApiBaseAddress,
            FacebookPixelDefaults.FbConversionsApiVersion,
            facebookPixelConfiguration.PixelId,
            FacebookPixelDefaults.FbConversionsApiEventEndpoint
        ]) + $"?access_token=" + facebookPixelConfiguration.AccessToken;

        var jsonString = JsonConvert.SerializeObject(conversionsEvent, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        var requestContent = new StringContent(jsonString, Encoding.UTF8, MimeTypes.ApplicationJson);
		logger.LogDebug("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): jsonString → {JsonString}", facebookPixelConfiguration, conversionsEvent, jsonString);
		logger.LogDebug("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): requestContent → {RequestContent}", facebookPixelConfiguration, conversionsEvent, requestContent);
		logger.LogInformation("SendEventAsync(facebookPixelConfiguration,conversionsEvent)={FacebookPixelConfiguration},{ConversionsEvent}: external service call {UrlString} took {Arg3} ms", facebookPixelConfiguration, conversionsEvent, urlString, __logStart.ElapsedMilliseconds);
        var response = await _httpClient.PostAsync(urlString, requestContent);

		logger.LogDebug("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): response → {Response}", facebookPixelConfiguration, conversionsEvent, response);
		logger.LogDebug("<<< Exiting SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent})", facebookPixelConfiguration, conversionsEvent);
        return await response.Content.ReadAsStringAsync();
    }

    #endregion
}