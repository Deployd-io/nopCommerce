using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Tax.Avalara.ItemClassificationAPI;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Services;

/// <summary>
/// Represents HTTP client to request Avalara Classification services
/// </summary>
public class ItemClassificationHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public ItemClassificationHttpClient(AvalaraTaxSettings avalaraTaxSettings,
        HttpClient httpClient)
    {
		_logger.LogError(">>> Entering ItemClassificationHttpClient(avalaraTaxSettings={AvalaraTaxSettings},httpClient={HttpClient})", avalaraTaxSettings, httpClient);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //configure client
        httpClient.BaseAddress = new Uri(avalaraTaxSettings.UseSandbox
            ? AvalaraTaxDefaults.ClassificationUrl.Sandbox
            : AvalaraTaxDefaults.ClassificationUrl.Live);
        httpClient.Timeout = TimeSpan.FromSeconds(10);
		_logger.LogError("ItemClassificationHttpClient(avalaraTaxSettings,httpClient)={AvalaraTaxSettings},{HttpClient}: external service call took {Arg2} ms", avalaraTaxSettings, httpClient, __logStart.ElapsedMilliseconds);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, AvalaraTaxDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

		_logger.LogWarning("<<< Exiting ItemClassificationHttpClient(avalaraTaxSettings={AvalaraTaxSettings},httpClient={HttpClient})", avalaraTaxSettings, httpClient);
        _avalaraTaxSettings = avalaraTaxSettings;
		_logger.LogDebug("ItemClassificationHttpClient(avalaraTaxSettings={AvalaraTaxSettings},httpClient={HttpClient}): _avalaraTaxSettings → {_avalaraTaxSettings}", avalaraTaxSettings, httpClient, _avalaraTaxSettings);
        _httpClient = httpClient;
		_logger.LogDebug("ItemClassificationHttpClient(avalaraTaxSettings={AvalaraTaxSettings},httpClient={HttpClient}): _httpClient → {_httpClient}", avalaraTaxSettings, httpClient, _httpClient);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Set security using CompanyId / License Key
    /// </summary>
    /// <returns>Security token</returns>
    protected string PrepareSecurity()
    {
		_logger.LogError(">>> Entering PrepareSecurity()");
			_logger.LogError("Throwing NopException from PrepareSecurity()");
        if (_avalaraTaxSettings.AccountId is null) {
            throw new NopException("AccountId not set");
        }

        if (_avalaraTaxSettings.LicenseKey is null)
            throw new NopException("LicenseKey is not set");

        var s = $"{_avalaraTaxSettings.AccountId}:{_avalaraTaxSettings.LicenseKey}";
		_logger.LogError("PrepareSecurity(): s → {S}", s);
		_logger.LogWarning("<<< Exiting PrepareSecurity()");
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
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
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TRequest : Request where TResponse : Response
    {
		_logger.LogError(">>> Entering RequestAsync(request={Request})", request);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            //prepare request parameters
            var requestString = JsonConvert.SerializeObject(request);
			_logger.LogError("RequestAsync(request={Request}): requestString → {RequestString}", request, requestString);
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
			_logger.LogError("RequestAsync(request={Request}): requestContent → {RequestContent}", request, requestContent);

            //execute request and get response
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), request.Path) { Content = requestContent };
			_logger.LogError("RequestAsync(request={Request}): requestMessage → {RequestMessage}", request, requestMessage);

            //add authorization
            var securityToken = PrepareSecurity();
			_logger.LogError("RequestAsync(request={Request}): securityToken → {SecurityToken}", request, securityToken);

            //call PrepareSecurity
            requestMessage.Headers.Add(HeaderNames.Authorization, $"Basic {securityToken}");

            var httpResponse = await _httpClient.SendAsync(requestMessage);
			_logger.LogError("RequestAsync(request={Request}): httpResponse → {HttpResponse}", request, httpResponse);
			_logger.LogError("RequestAsync(request)={Request}: external service call {SecurityToken} took {Arg2} ms", request, securityToken, __logStart.ElapsedMilliseconds);

            //return result
            TResponse result = null;
			_logger.LogError("RequestAsync(request={Request}): result → {Result}", request, result);
            var responseString = await httpResponse.Content.ReadAsStringAsync();
			_logger.LogError("RequestAsync(request={Request}): responseString → {ResponseString}", request, responseString);
            try
            {
                result = JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty);
            }
            catch (Exception ex)
            {
				_logger.LogError("Exception in RequestAsync(request={Request}): {Arg1}", request, ex.Message);
                throw new NopException($"Item HS classification error: response content invalid - {ex.Message}");
            }
            if (!string.IsNullOrEmpty(result?.Error?.Code))
            {
                var error = result.Error.Code;
                if (result.Error.Details?.Any() ?? false)
                    error += result.Error.Details.Aggregate(string.Empty, (text, e) => $"{text}{e.Message} {e.Description};{Environment.NewLine}");

				_logger.LogError("Throwing NopException from RequestAsync(request={Request})", request);
                throw new NopException($"Item HS classification error: {error}");
            }

            return result;
        }
        catch (AggregateException exception)
        {
			_logger.LogError("Exception in RequestAsync(request={Request}): {Arg1}", request, exception.Message);
			_logger.LogError("Throwing InnerException from RequestAsync(request={Request})", request);
            //rethrow actual exception
		_logger.LogWarning("<<< Exiting RequestAsync(request={Request})", request);
            throw exception.InnerException;
        }
    }

    #endregion
}