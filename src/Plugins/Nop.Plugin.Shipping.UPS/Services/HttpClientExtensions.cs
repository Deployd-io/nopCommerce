using System.Text;
using Microsoft.Net.Http.Headers;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using static System.TimeSpan;

namespace Nop.Plugin.Shipping.UPS.Services;

public static class HttpClientExtensions
{
    public static void PrepareRequest(this HttpClient httpClient,
        HttpRequestMessage request, UPSSettings upsSettings, string accessToken = null)
    {
		logger.LogError(">>> Entering PrepareRequest(httpClient={HttpClient},request={Request},upsSettings={UpsSettings},accessToken=<redacted>)", httpClient, request, upsSettings);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(upsSettings);

        httpClient.Timeout = FromSeconds(upsSettings.RequestTimeout ?? UPSDefaults.RequestTimeout);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, UPSDefaults.UserAgent);
		logger.LogError("PrepareRequest(httpClient,request,upsSettings,accessToken)={HttpClient},{Request},{UpsSettings},<redacted>: external service call took {Arg4} ms", httpClient, request, upsSettings, __logStart.ElapsedMilliseconds);

        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken}");
        else
        {
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{upsSettings.ClientId}:{upsSettings.ClientSecret}"));
            request.Headers.Add(HeaderNames.Authorization, $"Basic {base64}");
        }

        //save debug info
        if (!upsSettings.Tracing)
            return;

        var logger = EngineContext.Current.Resolve<ILogger>();
        logger.Information($"UPS rates. Request: {request}{Environment.NewLine}Content: {request.Content?.ReadAsStringAsync().Result}");
		logger.LogWarning("<<< Exiting PrepareRequest(httpClient={HttpClient},request={Request},upsSettings={UpsSettings},accessToken=<redacted>)", httpClient, request, upsSettings);
    }

    public static void ProcessResponse(this HttpClient httpClient, HttpResponseMessage response, UPSSettings upsSettings)
    {
		logger.LogError(">>> Entering ProcessResponse(httpClient={HttpClient},response={Response},upsSettings={UpsSettings})", httpClient, response, upsSettings);
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(upsSettings);

        //save debug info
        if (!upsSettings.Tracing)
            return;

        var logger = EngineContext.Current.Resolve<ILogger>();
        logger.Information($"UPS rates. Response: {response}{Environment.NewLine}Content: {response.Content.ReadAsStringAsync().Result}");
		logger.LogWarning("<<< Exiting ProcessResponse(httpClient={HttpClient},response={Response},upsSettings={UpsSettings})", httpClient, response, upsSettings);
    }
}