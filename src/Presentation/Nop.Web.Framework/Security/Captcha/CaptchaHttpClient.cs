using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Security.Captcha;

/// <summary>
/// Represents the HTTP client to request reCAPTCHA service
/// </summary>
public partial class CaptchaHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly HttpClient _httpClient;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public CaptchaHttpClient(CaptchaSettings captchaSettings,
        HttpClient client,
        IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering CaptchaHttpClient(captchaSettings={CaptchaSettings},client={Client},webHelper={WebHelper})", captchaSettings, client, webHelper);
        _captchaSettings = captchaSettings;
        _httpClient = client;
		_logger.LogDebug("CaptchaHttpClient(captchaSettings={CaptchaSettings},client={Client},webHelper={WebHelper}): _httpClient → {_httpClient}", captchaSettings, client, webHelper, _httpClient);
		_logger.LogDebug("CaptchaHttpClient(captchaSettings={CaptchaSettings},client={Client},webHelper={WebHelper}): _captchaSettings → {_captchaSettings}", captchaSettings, client, webHelper, _captchaSettings);
        _webHelper = webHelper;
		_logger.LogDebug("CaptchaHttpClient(captchaSettings={CaptchaSettings},client={Client},webHelper={WebHelper}): _webHelper → {_webHelper}", captchaSettings, client, webHelper, _webHelper);

        //configure client
        client.BaseAddress = new Uri(captchaSettings.ReCaptchaApiUrl);
        client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");

        if (captchaSettings.ReCaptchaRequestTimeout is int timeout && timeout > 0)
            client.Timeout = TimeSpan.FromSeconds(timeout);
			_logger.LogDebug("<<< Exiting CaptchaHttpClient(captchaSettings={CaptchaSettings},client={Client},webHelper={WebHelper})", captchaSettings, client, webHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Validate reCAPTCHA
    /// </summary>
    /// <param name="responseValue">Response value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains response from the reCAPTCHA service
    /// </returns>
    public virtual async Task<CaptchaResponse> ValidateCaptchaAsync(string responseValue)
    {
		_logger.LogDebug(">>> Entering ValidateCaptchaAsync(responseValue={ResponseValue})", responseValue);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare URL to request
        var url = string.Format(NopSecurityDefaults.RecaptchaValidationPath,
            _captchaSettings.ReCaptchaPrivateKey,
            responseValue,
            _webHelper.GetCurrentIpAddress());

        //get response
        var response = await _httpClient.GetStringAsync(url);
		_logger.LogDebug("ValidateCaptchaAsync(responseValue={ResponseValue}): response → {Response}", responseValue, response);
		_logger.LogInformation("ValidateCaptchaAsync(responseValue)={ResponseValue}: external service call {Url} took {Arg2} ms", responseValue, url, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting ValidateCaptchaAsync(responseValue={ResponseValue})", responseValue);
        return JsonConvert.DeserializeObject<CaptchaResponse>(response);

    }

    #endregion
}