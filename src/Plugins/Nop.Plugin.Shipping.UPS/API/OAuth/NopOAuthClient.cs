using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.OAuth;

public partial class OAuthClient
{
    private UPSSettings _upsSettings;

    public OAuthClient(HttpClient httpClient, UPSSettings upsSettings) : this(httpClient)
    {
		logger.LogDebug(">>> Entering OAuthClient(httpClient={HttpClient},upsSettings={UpsSettings})", httpClient, upsSettings);
        _upsSettings = upsSettings;

		logger.LogDebug("OAuthClient(httpClient={HttpClient},upsSettings={UpsSettings}): _upsSettings → {_upsSettings}", httpClient, upsSettings, _upsSettings);
        if (!_upsSettings.UseSandbox)
            BaseUrl = UPSDefaults.ApiUrl.Replace("/api", "");
			logger.LogDebug("<<< Exiting OAuthClient(httpClient={HttpClient},upsSettings={UpsSettings})", httpClient, upsSettings);
			logger.LogDebug("OAuthClient(httpClient={HttpClient},upsSettings={UpsSettings}): BaseUrl → {BaseUrl}", httpClient, upsSettings, BaseUrl);
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request,
        string url)
    {
        client.PrepareRequest(request, _upsSettings);
    }

    public virtual Task<GenerateTokenSuccessResponse> GenerateTokenAsync()
    {
		logger.LogDebug(">>> Entering GenerateTokenAsync()");
		logger.LogDebug("<<< Exiting GenerateTokenAsync()");
        return GenerateTokenAsync(null, new Body(), CancellationToken.None);
    }

    partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
    {
		logger.LogDebug(">>> Entering ProcessResponse(client={Client},response={Response})", client, response);
        client.ProcessResponse(response, _upsSettings);
		logger.LogDebug("<<< Exiting ProcessResponse(client={Client},response={Response})", client, response);
    }
}
