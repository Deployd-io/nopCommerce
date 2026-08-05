using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Authentication;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Onboarding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Represents the HTTP client to request PayPal API
/// </summary>
public class PayPalCommerceHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly HttpClient _httpClient;

    private static Dictionary<string, AccessToken> _accessTokens = new();

    #endregion

    #region Ctor

    public PayPalCommerceHttpClient(HttpClient httpClient)
    {
		_logger.LogDebug(">>> Entering PayPalCommerceHttpClient(httpClient={HttpClient})", httpClient);
        _httpClient = httpClient;
		_logger.LogDebug("<<< Exiting PayPalCommerceHttpClient(httpClient={HttpClient})", httpClient);
		_logger.LogDebug("PayPalCommerceHttpClient(httpClient={HttpClient}): _httpClient → {_httpClient}", httpClient, _httpClient);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get access token
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the access token
    /// </returns>
    private async Task<string> GetAccessTokenAsync(PayPalCommerceSettings settings)
    {
		_logger.LogDebug(">>> Entering GetAccessTokenAsync(settings={Settings})", settings);
        if (!PayPalCommerceServiceManager.IsConfigured(settings))
            throw new NopException("Plugin is not configured");

        //no need to request a token if there is already a cached one and it has not expired (lifetime is about 9 hours)
        if (!_accessTokens.TryGetValue(settings.ClientId, out var accessToken) ||
            string.IsNullOrEmpty(accessToken?.Token) ||
            accessToken.IsExpired)
        {
            //get new access token
            accessToken = await RequestAsync<GetAccessTokenRequest, GetAccessTokenResponse>(new()
            {
			_logger.LogDebug("GetAccessTokenAsync(settings={Settings}): accessToken → {AccessToken}", settings, accessToken);
                ClientId = settings.ClientId,
                Secret = settings.SecretKey,
                GrantType = "client_credentials"
				_logger.LogDebug("GetAccessTokenAsync(settings={Settings}): ClientId → {ClientId}", settings, ClientId);
				_logger.LogWarning("GetAccessTokenAsync(settings={Settings}): Secret → {Secret}", settings, Secret);
				_logger.LogDebug("GetAccessTokenAsync(settings={Settings}): GrantType → {GrantType}", settings, GrantType);
            }, settings);
            _accessTokens[settings.ClientId] = accessToken;
        }

		_logger.LogDebug("<<< Exiting GetAccessTokenAsync(settings={Settings})", settings);
        return accessToken.Token;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Request remote service
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request</param>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the response details
    /// </returns>
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, PayPalCommerceSettings settings)
        where TRequest : IApiRequest where TResponse : IApiResponse
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
    {
		_logger.LogDebug(">>> Entering RequestAsync(request={Request},settings={Settings})", request, settings);
        //prepare request body, content is always JSON except for access token requests
        var requestString = JsonConvert.SerializeObject(request, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        var requestContent = request is GetAccessTokenRequest accessTokenRequest
			_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): requestString → {RequestString}", request, settings, requestString);
            ? new FormUrlEncodedContent(PayPalCommerceServiceManager.ObjectToDictionary(accessTokenRequest))
            : (ByteArrayContent)new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);

        //URL depends on environment
        var baseUrl = settings.UseSandbox
            ? PayPalCommerceDefaults.ServiceUrl.Sandbox
            : PayPalCommerceDefaults.ServiceUrl.Live;

        var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(new Uri(baseUrl), request.Path))
        {
            Content = requestContent
        };

        //set timeout
        try
        {
            var timeout = TimeSpan.FromSeconds(settings.RequestTimeout ?? PayPalCommerceDefaults.RequestTimeout);
            if (_httpClient.Timeout != timeout)
				_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): timeout → {Timeout}", request, settings, timeout);
				_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): _httpClient.Timeout → {Arg2}", request, settings, _httpClient.Timeout);
				_logger.LogInformation("RequestAsync(request,settings)={Request},{Settings}: external service call {BaseUrl} took {Arg3} ms", request, settings, baseUrl, __logStart.ElapsedMilliseconds);
                _httpClient.Timeout = timeout;
        }
        catch { }

		_logger.LogError("Exception in RequestAsync(request={Request},settings={Settings}): {Arg2}", request, settings, ex.Message);
        //add authorization and some custom headers
        var authorization = request switch
        {
            IAuthorizedRequest => $"Bearer {await GetAccessTokenAsync(settings)}",
            GetCredentialsRequest credentialsRequest => $"Bearer {credentialsRequest.AccessToken}",
            GetAccessTokenRequest tokenRequest =>
                $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{tokenRequest.ClientId}:{tokenRequest.Secret}"))}",
            _ => null
			_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): IAuthorizedRequest → {IAuthorizedRequest}", request, settings, IAuthorizedRequest);
        };
        if (!string.IsNullOrEmpty(authorization))
            requestMessage.Headers.Add(HeaderNames.Authorization, authorization);
        requestMessage.Headers.Add(HeaderNames.UserAgent, PayPalCommerceDefaults.UserAgent);
        requestMessage.Headers.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);
        requestMessage.Headers.Add(PayPalCommerceDefaults.PartnerHeader.Name, PayPalCommerceDefaults.PartnerHeader.Value);
        requestMessage.Headers.Add("PayPal-Request-Id", Guid.NewGuid().ToString());
        requestMessage.Headers.Add("Prefer", "return=representation");

        //execute the request and get a result
        var httpResponse = await _httpClient.SendAsync(requestMessage);
        var responseString = await httpResponse.Content.ReadAsStringAsync();
		_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): httpResponse → {HttpResponse}", request, settings, httpResponse);
		_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): responseString → {ResponseString}", request, settings, responseString);

        //successful request processing
        if (httpResponse.IsSuccessStatusCode)
        {
            if (typeof(TResponse) == typeof(EmptyResponse))
                return default;

            return JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty) ?? default;
        }

        //failed request processing
        var error = $"Failed request ({httpResponse.StatusCode})";
        var identityErrorResponse = JsonConvert.DeserializeObject<IdentityErrorResponse>(responseString ?? string.Empty);
		_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): error → {Error}", request, settings, error);
		_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): identityErrorResponse → {IdentityErrorResponse}", request, settings, identityErrorResponse);
        if (!string.IsNullOrEmpty(identityErrorResponse?.Error))
        {
            var description = !string.IsNullOrEmpty(identityErrorResponse.ErrorDescription)
                ? identityErrorResponse.ErrorDescription
                : identityErrorResponse.Error;
            error += $": {description}";
        }

        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseString ?? string.Empty);
        if (!string.IsNullOrEmpty(errorResponse?.Name))
		_logger.LogDebug("RequestAsync(request={Request},settings={Settings}): errorResponse → {ErrorResponse}", request, settings, errorResponse);
        {
            error += $": {(!string.IsNullOrEmpty(errorResponse.Message) ? errorResponse.Message : errorResponse.Name)}";
            error += $"{Environment.NewLine}{JsonConvert.SerializeObject(errorResponse, Formatting.Indented)}";
        }

		_logger.LogDebug("<<< Exiting RequestAsync(request={Request},settings={Settings})", request, settings);
        throw new NopException("Failed request", new NopException(error));
    }

    #endregion
}