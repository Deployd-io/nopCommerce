using Newtonsoft.Json;
using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.Track;

public partial class TrackClient
{
    private UPSSettings _upsSettings;
    private string _accessToken;

    public TrackClient(HttpClient httpClient, UPSSettings upsSettings, string accessToken) : this(httpClient)
    {
		logger.LogDebug(">>> Entering TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken})", httpClient, upsSettings, accessToken);
        _upsSettings = upsSettings;
        _accessToken = accessToken;
		logger.LogDebug("TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken}): _accessToken → {_accessToken}", httpClient, upsSettings, accessToken, _accessToken);
		logger.LogDebug("TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken}): _upsSettings → {_upsSettings}", httpClient, upsSettings, accessToken, _upsSettings);

        if (!_upsSettings.UseSandbox)
            BaseUrl = UPSDefaults.ApiUrl;
			logger.LogDebug("<<< Exiting TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken})", httpClient, upsSettings, accessToken);
			logger.LogDebug("TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken}): BaseUrl → {BaseUrl}", httpClient, upsSettings, accessToken, BaseUrl);
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request,
        string url)
    {
        client.PrepareRequest(request, _upsSettings, _accessToken);
    }

    /// <summary>
    /// Get tracking info
    /// </summary>
    /// <param name="trackingNumber">The tracking number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the tracking info
    /// </returns>
    public virtual async Task<TrackResponse> TrackAsync(string trackingNumber)
    {
		logger.LogDebug(">>> Entering TrackAsync(trackingNumber={TrackingNumber})", trackingNumber);
        var response = await TrackV1DetailsAsync(trackingNumber, Guid.NewGuid().ToString(),
            _upsSettings.UseSandbox ? "testing" : UPSDefaults.SystemName, string.Empty);

		logger.LogDebug("<<< Exiting TrackAsync(trackingNumber={TrackingNumber})", trackingNumber);
        return response.TrackResponse;
    }

    partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
		logger.LogDebug(">>> Entering UpdateJsonSerializerSettings(settings={Settings})", settings);
        settings.ContractResolver = new NullToEmptyStringResolver();
		logger.LogDebug("<<< Exiting UpdateJsonSerializerSettings(settings={Settings})", settings);
    }

    partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
    {
        client.ProcessResponse(response, _upsSettings);
    }
}
