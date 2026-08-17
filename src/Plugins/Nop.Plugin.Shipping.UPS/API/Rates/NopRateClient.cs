using Newtonsoft.Json;
using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.Rates;

public partial class RateClient
{
    private readonly UPSSettings _upsSettings;
    private readonly string _accessToken;

    public RateClient(HttpClient httpClient, UPSSettings upsSettings, string accessToken) : this(httpClient)
    {
        _upsSettings = upsSettings;
        _accessToken = accessToken;

        if (!_upsSettings.UseSandbox)
            BaseUrl = UPSDefaults.ApiUrl;
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request,
        string url)
    {
		logger.LogDebug(">>> Entering PrepareRequest(client={Client},request={Request},url={Url})", client, request, url);
        client.PrepareRequest(request, _upsSettings, _accessToken);
		logger.LogDebug("<<< Exiting PrepareRequest(client={Client},request={Request},url={Url})", client, request, url);
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