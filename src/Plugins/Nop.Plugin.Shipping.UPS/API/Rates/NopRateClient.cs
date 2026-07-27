using Newtonsoft.Json;
using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.Rates;

public partial class RateClient
{
    private readonly UPSSettings _upsSettings;
    private readonly string _accessToken;

    public RateClient(HttpClient httpClient, UPSSettings upsSettings, string accessToken) : this(httpClient)
    {
		logger.LogDebug(">>> Entering RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken})", httpClient, upsSettings, accessToken);
        _upsSettings = upsSettings;
        _accessToken = accessToken;
		logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken}): _accessToken → {_accessToken}", httpClient, upsSettings, accessToken, _accessToken);
		logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken}): _upsSettings → {_upsSettings}", httpClient, upsSettings, accessToken, _upsSettings);

        if (!_upsSettings.UseSandbox)
            BaseUrl = UPSDefaults.ApiUrl;
			logger.LogDebug("<<< Exiting RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken})", httpClient, upsSettings, accessToken);
			logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken={AccessToken}): BaseUrl → {BaseUrl}", httpClient, upsSettings, accessToken, BaseUrl);
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request,
        string url)
    {
        client.PrepareRequest(request, _upsSettings, _accessToken);
    }

    public async Task<RateResponse> ProcessRateAsync(RateRequest request)
    {
		logger.LogDebug(">>> Entering ProcessRateAsync(request={Request})", request);
        var response = await RateAsync(Guid.NewGuid().ToString(),
            _upsSettings.UseSandbox ? "testing" : UPSDefaults.SystemName, "timeintransit", "v2403", "Shop", new RATERequestWrapper
            {
                RateRequest = request
            });

		logger.LogDebug("<<< Exiting ProcessRateAsync(request={Request})", request);
        return response.RateResponse;
    }

    static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
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