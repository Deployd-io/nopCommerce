using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

namespace Nop.Plugin.Tax.Avalara.Services;

/// <summary>
/// Represents HTTP client to request Avalara Classification services
/// </summary>
public class ItemClassificationHttpClient
{
    #region Fields

    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public ItemClassificationHttpClient(AvalaraTaxSettings avalaraTaxSettings,
        HttpClient httpClient)
    {
        //configure client
        httpClient.BaseAddress = new Uri(avalaraTaxSettings.UseSandbox
            ? AvalaraTaxDefaults.ClassificationUrl.Sandbox
            : AvalaraTaxDefaults.ClassificationUrl.Live);
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, AvalaraTaxDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

        _avalaraTaxSettings = avalaraTaxSettings;
        _httpClient = httpClient;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Set security using CompanyId / License Key
    /// </summary>
    /// <returns>Security token</returns>
    protected string PrepareSecurity()
    {
		logger.LogDebug(">>> Entering PrepareSecurity()");
        if (_avalaraTaxSettings.AccountId is null)
            throw new NopException("AccountId not set");

        if (_avalaraTaxSettings.LicenseKey is null)
            throw new NopException("LicenseKey is not set");

        var s = $"{_avalaraTaxSettings.AccountId}:{_avalaraTaxSettings.LicenseKey}";
		logger.LogDebug("PrepareSecurity(): s → {S}", s);
		logger.LogDebug("<<< Exiting PrepareSecurity()");
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
		logger.LogDebug(">>> Entering RequestAsync(request={Request})", request);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            //prepare request parameters
            var requestString = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);
			logger.LogDebug("RequestAsync(request={Request}): requestString → {RequestString}", request, requestString);
			logger.LogDebug("RequestAsync(request={Request}): requestContent → {RequestContent}", request, requestContent);

            //execute request and get response
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), request.Path) { Content = requestContent };

			logger.LogDebug("RequestAsync(request={Request}): requestMessage → {RequestMessage}", request, requestMessage);
            //add authorization
            var securityToken = PrepareSecurity();

			logger.LogDebug("RequestAsync(request={Request}): securityToken → {SecurityToken}", request, securityToken);
            //call PrepareSecurity
            requestMessage.Headers.Add(HeaderNames.Authorization, $"Basic {securityToken}");

            var httpResponse = await _httpClient.SendAsync(requestMessage);

			logger.LogInformation("RequestAsync(request)={Request}: external service call {SecurityToken} took {Arg2} ms", request, securityToken, __logStart.ElapsedMilliseconds);
			logger.LogDebug("RequestAsync(request={Request}): httpResponse → {HttpResponse}", request, httpResponse);
            //return result
            TResponse result = null;
            var responseString = await httpResponse.Content.ReadAsStringAsync();
			logger.LogDebug("RequestAsync(request={Request}): result → {Result}", request, result);
			logger.LogDebug("RequestAsync(request={Request}): responseString → {ResponseString}", request, responseString);
            try
            {
                result = JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty);
            }
            catch (Exception ex)
            {
				logger.LogError("Exception in RequestAsync(request={Request}): {Arg1}", request, ex.Message);
                throw new NopException($"Item HS classification error: response content invalid - {ex.Message}");
            }
            if (!string.IsNullOrEmpty(result?.Error?.Code))
            {
                var error = result.Error.Code;
                if (result.Error.Details?.Any() ?? false)
                    error += result.Error.Details.Aggregate(string.Empty, (text, e) => $"{text}{e.Message} {e.Description};{Environment.NewLine}");

                throw new NopException($"Item HS classification error: {error}");
            }

            return result;
        }
        catch (AggregateException exception)
        {
			logger.LogError("Exception in RequestAsync(request={Request}): {Arg1}", request, ex.Message);
            //rethrow actual exception
            throw exception.InnerException;
        }
		logger.LogDebug("<<< Exiting RequestAsync(request={Request})", request);
    }

    #endregion
}
