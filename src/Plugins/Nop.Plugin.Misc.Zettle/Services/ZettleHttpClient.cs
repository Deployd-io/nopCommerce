using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.Zettle.Domain.Api;
using Nop.Plugin.Misc.Zettle.Domain.Api.OAuth;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Zettle.Services;

/// <summary>
/// Represents HTTP client to request third-party services
/// </summary>
public class ZettleHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly HttpClient _httpClient;
    protected readonly ZettleSettings _zettleSettings;

    protected string _accessToken;

    #endregion

    #region Ctor

    public ZettleHttpClient(HttpClient httpClient, ZettleSettings zettleSettings)
    {
		_logger.LogError(">>> Entering ZettleHttpClient(httpClient={HttpClient},zettleSettings={ZettleSettings})", httpClient, zettleSettings);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        httpClient.Timeout = TimeSpan.FromSeconds(zettleSettings.RequestTimeout ?? ZettleDefaults.RequestTimeout);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, ZettleDefaults.UserAgent);
		_logger.LogError("ZettleHttpClient(httpClient,zettleSettings)={HttpClient},{ZettleSettings}: external service call took {Arg2} ms", httpClient, zettleSettings, __logStart.ElapsedMilliseconds);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);
        httpClient.DefaultRequestHeaders.Add(ZettleDefaults.PartnerHeader.Name, ZettleDefaults.PartnerHeader.Value);

		_logger.LogWarning("<<< Exiting ZettleHttpClient(httpClient={HttpClient},zettleSettings={ZettleSettings})", httpClient, zettleSettings);
        _httpClient = httpClient;
		_logger.LogDebug("ZettleHttpClient(httpClient={HttpClient},zettleSettings={ZettleSettings}): _httpClient → {_httpClient}", httpClient, zettleSettings, _httpClient);
        _zettleSettings = zettleSettings;
		_logger.LogDebug("ZettleHttpClient(httpClient={HttpClient},zettleSettings={ZettleSettings}): _zettleSettings → {_zettleSettings}", httpClient, zettleSettings, _zettleSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get access token
    /// </summary>
    /// <returns>The asynchronous task whose result contains access token</returns>
    protected async Task<string> GetAccessTokenAsync()
    {
		_logger.LogError(">>> Entering GetAccessTokenAsync()");
        if (!string.IsNullOrEmpty(_accessToken))
            return _accessToken;

        if (string.IsNullOrEmpty(_zettleSettings.ApiKey))
			_logger.LogError("Throwing NopException from GetAccessTokenAsync()");
            throw new NopException("API key is not set");

        _accessToken = (await RequestAsync<GetAuthenticationRequest, Authentication>(new()
        {
            Assertion = _zettleSettings.ApiKey,
            ClientId = ZettleDefaults.PartnerHeader.Value,
            GrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer"
			_logger.LogDebug("GetAccessTokenAsync(): GrantType → {GrantType}", GrantType);
        }))?.AccessToken;

		_logger.LogWarning("<<< Exiting GetAccessTokenAsync()");
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
		_logger.LogError(">>> Entering RequestAsync(request={Request})", request);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //prepare request parameters
        var requestString = JsonConvert.SerializeObject(request);
		_logger.LogError("RequestAsync(request={Request}): requestString → {RequestString}", request, requestString);
        var requestContent = request is not GetAuthenticationRequest authentication
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
			_logger.LogError("RequestAsync(request={Request}): accessToken → {AccessToken}", request, accessToken);
            requestMessage.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken}");
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
		_logger.LogError("RequestAsync(request={Request}): httpResponse → {HttpResponse}", request, httpResponse);
        var responseString = await httpResponse.Content.ReadAsStringAsync();
		_logger.LogError("RequestAsync(request)={Request}: external service call took {Arg1} ms", request, __logStart.ElapsedMilliseconds);
		_logger.LogError("RequestAsync(request={Request}): responseString → {ResponseString}", request, responseString);
        var result = JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty);
        if (!string.IsNullOrEmpty(result?.Error))
        {
			_logger.LogError("RequestAsync(request={Request}): result → {Result}", request, result);
            var error = !string.IsNullOrEmpty(result.ErrorDescription) ? result.ErrorDescription : result.Error;
			_logger.LogError("Throwing NopException from RequestAsync(request={Request})", request);
			_logger.LogError("RequestAsync(request={Request}): error → {Error}", request, error);
            throw new NopException($"Request error: {error}");
        }
        if (!string.IsNullOrEmpty(result?.DeveloperMessage))
            throw new NopException($"Request error: {result.DeveloperMessage}{Environment.NewLine}Details: {responseString}");

		_logger.LogWarning("<<< Exiting RequestAsync(request={Request})", request);
        return result;
    }

    #endregion
}