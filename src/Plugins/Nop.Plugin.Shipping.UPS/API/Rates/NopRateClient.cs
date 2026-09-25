using Newtonsoft.Json;
using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.Rates;

public partial class RateClient
{
    private readonly UPSSettings _upsSettings;
    private readonly string _accessToken;

    public RateClient(HttpClient httpClient, UPSSettings upsSettings, string accessToken) : this(httpClient)
    {
		logger.LogError(">>> Entering RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>)", httpClient, upsSettings);
        _upsSettings = upsSettings;
        _accessToken = accessToken;
		logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): _accessToken → {_accessToken}", httpClient, upsSettings, _accessToken);
		logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): _upsSettings → {_upsSettings}", httpClient, upsSettings, _upsSettings);

			logger.LogWarning("<<< Exiting RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>)", httpClient, upsSettings);
        if (!_upsSettings.UseSandbox) {
            BaseUrl = UPSDefaults.ApiUrl;
        }
			logger.LogDebug("RateClient(httpClient={HttpClient},upsSettings={UpsSettings},accessToken=<redacted>): BaseUrl → {BaseUrl}", httpClient, upsSettings, BaseUrl);
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request,
        string url)
    {
		logger.LogError(">>> Entering PrepareRequest(client={Client},request={Request},url={Url})", client, request, url);
        client.PrepareRequest(request, _upsSettings, _accessToken);
		logger.LogWarning("<<< Exiting PrepareRequest(client={Client},request={Request},url={Url})", client, request, url);
    }

    public async Task<RateResponse> ProcessRateAsync(RateRequest request)
    {
		logger.LogError(">>> Entering ProcessRateAsync(request={Request})", request);
        var response = await RateAsync(Guid.NewGuid().ToString(),
            _upsSettings.UseSandbox ? "testing" : UPSDefaults.SystemName, "timeintransit", "v2403", "Shop", new RATERequestWrapper
            {
                RateRequest = request
            });
			logger.LogDebug("ProcessRateAsync(request={Request}): RateRequest → {RateRequest}", request, RateRequest);

		logger.LogWarning("<<< Exiting ProcessRateAsync(request={Request})", request);
        return response.RateResponse;
    }

    static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
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