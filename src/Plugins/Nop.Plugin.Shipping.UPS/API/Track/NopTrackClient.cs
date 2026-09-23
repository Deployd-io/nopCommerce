using Newtonsoft.Json;
using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.Track;

public partial class TrackClient
{
    private UPSSettings _upsSettings;
    private string _accessToken;

    public TrackClient(HttpClient httpClient, UPSSettings upsSettings, string accessToken) : this(httpClient)
    {
		logger.LogError(">>> Entering TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>)", httpClient, upsSettings);
        _upsSettings = upsSettings;
        _accessToken = accessToken;
		logger.LogDebug("TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): _accessToken → {_accessToken}", httpClient, upsSettings, _accessToken);
		logger.LogDebug("TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): _upsSettings → {_upsSettings}", httpClient, upsSettings, _upsSettings);

        if (!_upsSettings.UseSandbox)
            BaseUrl = UPSDefaults.ApiUrl;
			logger.LogDebug("TrackClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): BaseUrl → {BaseUrl}", httpClient, upsSettings, BaseUrl);
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request,
        string url)
    {
		logger.LogError(">>> Entering PrepareRequest(client={Client},request={Request},url={Url})", client, request, url);
        client.PrepareRequest(request, _upsSettings, _accessToken);
		logger.LogWarning("<<< Exiting PrepareRequest(client={Client},request={Request},url={Url})", client, request, url);
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
		logger.LogError(">>> Entering TrackAsync(trackingNumber={TrackingNumber})", trackingNumber);
        var response = await TrackV1DetailsAsync(trackingNumber, Guid.NewGuid().ToString(),
            _upsSettings.UseSandbox ? "testing" : UPSDefaults.SystemName, string.Empty);

		logger.LogWarning("<<< Exiting TrackAsync(trackingNumber={TrackingNumber})", trackingNumber);
        return response.TrackResponse;
    }

    partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
		logger.LogError(">>> Entering UpdateJsonSerializerSettings(settings={Settings})", settings);
        settings.ContractResolver = new NullToEmptyStringResolver();
		logger.LogWarning("<<< Exiting UpdateJsonSerializerSettings(settings={Settings})", settings);
    }

    partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
    {
		logger.LogError(">>> Entering ProcessResponse(client={Client},response={Response})", client, response);
        client.ProcessResponse(response, _upsSettings);
		logger.LogWarning("<<< Exiting ProcessResponse(client={Client},response={Response})", client, response);
    }
}