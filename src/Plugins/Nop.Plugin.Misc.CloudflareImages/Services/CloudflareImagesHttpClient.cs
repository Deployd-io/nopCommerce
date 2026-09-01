using System.Net;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.CloudflareImages.Services;

/// <summary>
/// Represents the HTTP client to request Cloudflare Images
/// </summary>
public class CloudflareImagesHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly CloudflareImagesSettings _cloudflareImagesSettings;
    private readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public CloudflareImagesHttpClient(CloudflareImagesSettings cloudflareImagesSettings, HttpClient httpClient)
    {
		_logger.LogDebug(">>> Entering CloudflareImagesHttpClient(cloudflareImagesSettings={CloudflareImagesSettings},httpClient={HttpClient})", cloudflareImagesSettings, httpClient);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        _cloudflareImagesSettings = cloudflareImagesSettings;
        httpClient.Timeout = TimeSpan.FromSeconds(_cloudflareImagesSettings.RequestTimeout ?? CloudflareImagesDefaults.RequestTimeout);
		_logger.LogDebug("CloudflareImagesHttpClient(cloudflareImagesSettings={CloudflareImagesSettings},httpClient={HttpClient}): _cloudflareImagesSettings → {_cloudflareImagesSettings}", cloudflareImagesSettings, httpClient, _cloudflareImagesSettings);
		_logger.LogInformation("CloudflareImagesHttpClient(cloudflareImagesSettings,httpClient)={CloudflareImagesSettings},{HttpClient}: external service call took {Arg2} ms", cloudflareImagesSettings, httpClient, __logStart.ElapsedMilliseconds);
        _httpClient = httpClient;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// HTTP request services
    /// </summary>
    /// <param name="apiUri">Request URL</param>
    /// <param name="data">Data to send</param>
    /// <param name="httpMethod">Request type. null == HttpMethod.Get</param>
    /// <returns>The asynchronous task whose result contains response details</returns>
    private async Task<string> RequestAsync(string apiUri = "", HttpContent data = null, HttpMethod httpMethod = null)
    {
		_logger.LogDebug(">>> Entering RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
        httpMethod ??= HttpMethod.Get;

        var requestUri = new Uri(string.Format(CloudflareImagesDefaults.BaseApiUrl, _cloudflareImagesSettings.AccountId, apiUri));
		_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): requestUri → {RequestUri}", apiUri, data, httpMethod, requestUri);

        var request = new HttpRequestMessage
        {
            Method = httpMethod,
            RequestUri = requestUri
        };

        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, $"Bearer {_cloudflareImagesSettings.AccessToken}");

        if (httpMethod == HttpMethod.Post)
            request.Content = data;
			_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): request.Content → {Arg3}", apiUri, data, httpMethod, request.Content);

        var httpResponse = await _httpClient.SendAsync(request);
		_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): httpResponse → {HttpResponse}", apiUri, data, httpMethod, httpResponse);
		_logger.LogInformation("RequestAsync(apiUri,data,httpMethod)={ApiUri},{Data},{HttpMethod}: external service call {ApiUri1} took {Arg4} ms", apiUri, data, httpMethod, apiUri, __logStart.ElapsedMilliseconds);
        var response = await httpResponse.Content.ReadAsStringAsync();

		_logger.LogDebug("RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod}): response → {Response}", apiUri, data, httpMethod, response);
        if (httpResponse.StatusCode == HttpStatusCode.OK)
            return response;

        if (!string.IsNullOrEmpty(response))
			_logger.LogWarning("Throwing NopException from RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
            throw new NopException(response);

		_logger.LogDebug("<<< Exiting RequestAsync(apiUri={ApiUri},data={Data},httpMethod={HttpMethod})", apiUri, data, httpMethod);
        throw new NopException($"{CloudflareImagesDefaults.SystemName} unknown error.");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Save a picture thumb
    /// </summary>
    /// <param name="dataContent">Content</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the response details
    /// </returns>
    public async Task<CloudflareImagesResponse> SaveThumbAsync(MultipartFormDataContent dataContent)
    {
		_logger.LogDebug(">>> Entering SaveThumbAsync(dataContent={DataContent})", dataContent);
        try
        {
            var result = await RequestAsync(string.Empty, dataContent, HttpMethod.Post);
			_logger.LogDebug("SaveThumbAsync(dataContent={DataContent}): result → {Result}", dataContent, result);
            return JsonConvert.DeserializeObject<CloudflareImagesResponse>(result);
        }
        catch
        {
			_logger.LogError("Exception in SaveThumbAsync(dataContent={DataContent}): {Arg1}", dataContent, ex.Message);
			_logger.LogDebug("<<< Exiting SaveThumbAsync(dataContent={DataContent})", dataContent);
            return null;
        }
    }

    /// <summary>
    /// Delete picture thumb
    /// </summary>
    /// <param name="imageId">Image identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteThumbAsync(string imageId)
    {
		_logger.LogDebug(">>> Entering DeleteThumbAsync(imageId={ImageId})", imageId);
        try
        {
            await RequestAsync($"/{imageId}", null, HttpMethod.Delete);
        }
        catch
        {
			_logger.LogError("Exception in DeleteThumbAsync(imageId={ImageId}): {Arg1}", imageId, ex.Message);
            // ignored
        }
		_logger.LogDebug("<<< Exiting DeleteThumbAsync(imageId={ImageId})", imageId);
    }

    #endregion

    #region Nested classes

    public class CloudflareImagesResponse
    {
        [JsonProperty("errors")]
        public List<ResponseInfo> Errors { get; set; } = new();

        [JsonProperty("messages")]
        public List<ResponseInfo> Messages { get; set; } = new();

        [JsonProperty("result")]
        public Image Result { get; set; }

        /// <summary>
        /// Gets or sets whether the API call was successful
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public class ResponseInfo
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("documentation_url")]
        public string DocumentationUrl { get; set; }
    }

    public class Image
    {
        /// <summary>
        /// Image unique identifier
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Image file name
        /// </summary>
        [JsonProperty("filename")]
        public string FileName { get; set; }
    }

    #endregion
}