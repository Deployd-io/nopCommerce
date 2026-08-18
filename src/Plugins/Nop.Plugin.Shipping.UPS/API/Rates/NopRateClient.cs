using Newtonsoft.Json;
using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.Rates;

public partial class RateClient
{
    private readonly UPSSettings _upsSettings;
    private readonly string _accessToken;

    public RateClient(HttpClient httpClient, UPSSettings upsSettings, string accessToken) : this(httpClient)
    {
		logger.LogDebug(">>> Entering RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>)", httpClient, upsSettings);
        _upsSettings = upsSettings;
        _accessToken = accessToken;
		logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): _accessToken → {_accessToken}", httpClient, upsSettings, _accessToken);
		logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): _upsSettings → {_upsSettings}", httpClient, upsSettings, _upsSettings);

        if (!_upsSettings.UseSandbox)
            BaseUrl = UPSDefaults.ApiUrl;
			logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): BaseUrl → {BaseUrl}", httpClient, upsSettings, BaseUrl);
			logger.LogDebug("<<< Exiting RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>)", httpClient, upsSettings);
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
		logger.LogDebug(">>> Entering ProcessResponse(client={Client},response={Response})", client, response);
        client.ProcessResponse(response, _upsSettings);
		logger.LogDebug("<<< Exiting ProcessResponse(client={Client},response={Response})", client, response);
    }
}