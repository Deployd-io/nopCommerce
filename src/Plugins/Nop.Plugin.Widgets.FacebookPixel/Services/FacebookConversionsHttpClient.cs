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
		_logger.LogDebug(">>> Entering FacebookConversionsHttpClient(httpClient={HttpClient})", httpClient);
        _httpClient = httpClient;
		_logger.LogDebug("FacebookConversionsHttpClient(httpClient={HttpClient}): _httpClient → {_httpClient}", httpClient, _httpClient);
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
        var urlString = string.Join($"/",
        [
            FacebookPixelDefaults.FbConversionsApiBaseAddress,
            FacebookPixelDefaults.FbConversionsApiVersion,
            facebookPixelConfiguration.PixelId,
            FacebookPixelDefaults.FbConversionsApiEventEndpoint
        ]) + $"?access_token=" + facebookPixelConfiguration.AccessToken;
		_logger.LogDebug(">>> Entering SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent})", facebookPixelConfiguration, conversionsEvent);

        var jsonString = JsonConvert.SerializeObject(conversionsEvent, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
		_logger.LogDebug("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): jsonString → {JsonString}", facebookPixelConfiguration, conversionsEvent, jsonString);
        var requestContent = new StringContent(jsonString, Encoding.UTF8, MimeTypes.ApplicationJson);
		_logger.LogDebug("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): requestContent → {RequestContent}", facebookPixelConfiguration, conversionsEvent, requestContent);
        var response = await _httpClient.PostAsync(urlString, requestContent);
		_logger.LogDebug("SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent}): response → {Response}", facebookPixelConfiguration, conversionsEvent, response);

		_logger.LogDebug("<<< Exiting SendEventAsync(facebookPixelConfiguration={FacebookPixelConfiguration},conversionsEvent={ConversionsEvent})", facebookPixelConfiguration, conversionsEvent);
        return await response.Content.ReadAsStringAsync();
    }

    #endregion
}