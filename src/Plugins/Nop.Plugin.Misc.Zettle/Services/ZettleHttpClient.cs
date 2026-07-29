using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.Zettle.Domain.Api;
using Nop.Plugin.Misc.Zettle.Domain.Api.OAuth;

namespace Nop.Plugin.Misc.Zettle.Services;

/// <summary>
/// Represents HTTP client to request third-party services
/// </summary>
public class ZettleHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;
    protected readonly ZettleSettings _zettleSettings;

    protected string _accessToken;

    #endregion

    #region Ctor

    public ZettleHttpClient(HttpClient httpClient, ZettleSettings zettleSettings)
    {
		logger.LogDebug(">>> Entering ZettleHttpClient(httpClient={HttpClient},zettleSettings={ZettleSettings})", httpClient, zettleSettings);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        httpClient.Timeout = TimeSpan.FromSeconds(zettleSettings.RequestTimeout ?? ZettleDefaults.RequestTimeout);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, ZettleDefaults.UserAgent);
		logger.LogInformation("ZettleHttpClient(httpClient,zettleSettings)={HttpClient},{ZettleSettings}: external service call took {Arg2} ms", httpClient, zettleSettings, __logStart.ElapsedMilliseconds);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);
        httpClient.DefaultRequestHeaders.Add(ZettleDefaults.PartnerHeader.Name, ZettleDefaults.PartnerHeader.Value);

        _httpClient = httpClient;
		logger.LogDebug("ZettleHttpClient(httpClient={HttpClient},zettleSettings={ZettleSettings}): _httpClient → {_httpClient}", httpClient, zettleSettings, _httpClient);
        _zettleSettings = zettleSettings;
		logger.LogDebug("<<< Exiting ZettleHttpClient(httpClient={HttpClient},zettleSettings={ZettleSettings})", httpClient, zettleSettings);
		logger.LogDebug("ZettleHttpClient(httpClient={HttpClient},zettleSettings={ZettleSettings}): _zettleSettings → {_zettleSettings}", httpClient, zettleSettings, _zettleSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get access token
    /// </summary>
    /// <returns>The asynchronous task whose result contains access token</returns>
    protected async Task<string> GetAccessTokenAsync()
    {
		logger.LogDebug(">>> Entering GetAccessTokenAsync()");
        if (!string.IsNullOrEmpty(_accessToken))
            return _accessToken;

        if (string.IsNullOrEmpty(_zettleSettings.ApiKey))
            throw new NopException("API key is not set");

        _accessToken = (await RequestAsync<GetAuthenticationRequest, Authentication>(new()
        {
		logger.LogDebug("GetAccessTokenAsync(): _accessToken → {_accessToken}", _accessToken);
            Assertion = _zettleSettings.ApiKey,
            ClientId = ZettleDefaults.PartnerHeader.Value,
            GrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer"
			logger.LogDebug("GetAccessTokenAsync(): Assertion → {Assertion}", Assertion);
			logger.LogDebug("GetAccessTokenAsync(): ClientId → {ClientId}", ClientId);
			logger.LogDebug("GetAccessTokenAsync(): GrantType → {GrantType}", GrantType);
        }))?.AccessToken;

		logger.LogDebug("<<< Exiting GetAccessTokenAsync()");
        return _accessToken;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Request services
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request</param>
    /// <returns>The asynchronous task whose result contains response details</returns>
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TRequest : IApiRequest where TResponse : IApiResponse
    {
		logger.LogDebug(">>> Entering RequestAsync(request={Request})", request);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare request parameters
        var requestString = JsonConvert.SerializeObject(request);
        var requestContent = request is not GetAuthenticationRequest authentication
			logger.LogDebug("RequestAsync(request={Request}): requestString → {RequestString}", request, requestString);
            ? (ByteArrayContent)new StringContent(requestString, Encoding.UTF8, MimeTypes.ApplicationJson)
            : new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["assertion"] = authentication.Assertion,
                ["client_id"] = authentication.ClientId,
                ["grant_type"] = authentication.GrantType
            });

        var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(new Uri(request.BaseUrl), request.Path))
        {
            Content = requestContent
        };

        //add authorization
        if (request is IAuthorizedRequest)
        {
            var accessToken = await GetAccessTokenAsync();
            requestMessage.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken}");
			logger.LogDebug("RequestAsync(request={Request}): accessToken → {AccessToken}", request, accessToken);
        }

        //add ETag
        if (request is IConditionalRequest conditionalRequest)
        {
            var header = request.Method == HttpMethods.Get
                ? HeaderNames.IfNoneMatch
                : HeaderNames.IfMatch;
            requestMessage.Headers.Add(header, conditionalRequest.ETag);
        }

        //execute request and get result
        var httpResponse = await _httpClient.SendAsync(requestMessage);
        var responseString = await httpResponse.Content.ReadAsStringAsync();
		logger.LogInformation("RequestAsync(request)={Request}: external service call took {Arg1} ms", request, __logStart.ElapsedMilliseconds);
		logger.LogDebug("RequestAsync(request={Request}): httpResponse → {HttpResponse}", request, httpResponse);
		logger.LogDebug("RequestAsync(request={Request}): responseString → {ResponseString}", request, responseString);
        var result = JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty);
        if (!string.IsNullOrEmpty(result?.Error))
        {
			logger.LogDebug("RequestAsync(request={Request}): result → {Result}", request, result);
            var error = !string.IsNullOrEmpty(result.ErrorDescription) ? result.ErrorDescription : result.Error;
            throw new NopException($"Request error: {error}");
        }
        if (!string.IsNullOrEmpty(result?.DeveloperMessage))
            throw new NopException($"Request error: {result.DeveloperMessage}{Environment.NewLine}Details: {responseString}");

        return result;
    }

    #endregion
}