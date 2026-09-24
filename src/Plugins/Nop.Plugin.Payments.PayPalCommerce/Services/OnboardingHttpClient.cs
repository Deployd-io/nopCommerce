using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Represents the HTTP client to request onboarding services
/// </summary>
public class OnboardingHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public OnboardingHttpClient(HttpClient httpClient)
    {
		_logger.LogError(">>> Entering OnboardingHttpClient(httpClient={HttpClient})", httpClient);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //configure client
        httpClient.BaseAddress = new Uri(PayPalCommerceDefaults.Onboarding.ServiceUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(20);
		_logger.LogError("OnboardingHttpClient(httpClient)={HttpClient}: external service call took {Arg1} ms", httpClient, __logStart.ElapsedMilliseconds);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, PayPalCommerceDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

		_logger.LogWarning("<<< Exiting OnboardingHttpClient(httpClient={HttpClient})", httpClient);
        _httpClient = httpClient;
		_logger.LogDebug("OnboardingHttpClient(httpClient={HttpClient}): _httpClient → {_httpClient}", httpClient, _httpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the merchant details
    /// </summary>
    /// <param name="merchantGuid">Internal merchant id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the merchant details
    /// </returns>
    public async Task<Merchant> GetMerchantAsync(string merchantGuid)
    {
		_logger.LogError(">>> Entering GetMerchantAsync(merchantGuid={MerchantGuid})", merchantGuid);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var httpResponse = await _httpClient.GetAsync($"paypal/merchant/{Uri.EscapeDataString(merchantGuid)}");
        var responseString = await httpResponse.Content.ReadAsStringAsync();
		_logger.LogError("GetMerchantAsync(merchantGuid)={MerchantGuid}: external service call took {Arg1} ms", merchantGuid, __logStart.ElapsedMilliseconds);
		_logger.LogError("GetMerchantAsync(merchantGuid={MerchantGuid}): httpResponse → {HttpResponse}", merchantGuid, httpResponse);
		_logger.LogError("GetMerchantAsync(merchantGuid={MerchantGuid}): responseString → {ResponseString}", merchantGuid, responseString);
        httpResponse.EnsureSuccessStatusCode();
        var result = new { Result = string.Empty, Data = new Merchant() };
        result = JsonConvert.DeserializeAnonymousType(responseString ?? string.Empty, result) ?? default;
		_logger.LogError("GetMerchantAsync(merchantGuid={MerchantGuid}): result → {Result}", merchantGuid, result);
		_logger.LogWarning("<<< Exiting GetMerchantAsync(merchantGuid={MerchantGuid})", merchantGuid);
        return result.Data;
    }

    #endregion
}