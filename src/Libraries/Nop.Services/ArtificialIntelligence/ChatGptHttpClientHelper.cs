using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client helper to create ChatGPT requests
/// </summary>
public partial class ChatGptHttpClientHelper : IArtificialIntelligenceHttpClientHelper
{
    #region Methods

    /// <summary>
    /// Configure client
    /// </summary>
    /// <param name="httpClient">HTTP client for configuration</param>
    public virtual void ConfigureClient(HttpClient httpClient)
    {
		logger.LogDebug(">>> Entering ConfigureClient(httpClient={HttpClient})", httpClient);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        httpClient.BaseAddress = new Uri(ArtificialIntelligenceDefaults.ChatGptBaseApiUrl);
		logger.LogDebug("<<< Exiting ConfigureClient(httpClient={HttpClient})", httpClient);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("ConfigureClient(httpClient)={HttpClient}: external service call took {Arg1} ms", httpClient, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("ConfigureClient(httpClient)={HttpClient}: external service call took {Arg1} ms", httpClient, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Create HTTP request
    /// </summary>
    /// <param name="settings">Artificial intelligence settings</param>
    /// <param name="query">query to send into artificial intelligence host</param>
    /// <returns>Created HttpRequestMessage</returns>
    public virtual HttpRequestMessage CreateRequest(ArtificialIntelligenceSettings settings, string query)
    {
		logger.LogDebug(">>> Entering CreateRequest(settings={Settings},query={Query})", settings);
        var request = new HttpRequestMessage { Method = HttpMethod.Post };
        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, $"Bearer {settings.ChatGptApiKey}");

        var data = JsonConvert.SerializeObject(new Dictionary<string, string>
        {
            ["model"] = ArtificialIntelligenceDefaults.ChatGptApiModel,
            ["input"] = query
        });

        request.Content = new StringContent(data, Encoding.UTF8, MimeTypes.ApplicationJson);

		logger.LogDebug("<<< Exiting CreateRequest(settings={Settings},query={Query})", settings);
        return request;
    }

    /// <summary>
    /// Parse response
    /// </summary>
    /// <param name="responseText">Response text to parse</param>
    /// <returns>Generated text from artificial intelligence host</returns>
    public virtual string ParseResponse(string responseText)
    {
		logger.LogDebug(">>> Entering ParseResponse(responseText={ResponseText})", responseText);
        var response = JsonConvert.DeserializeAnonymousType(responseText,
            new
            {
                output = new[]
				logger.LogDebug("ParseResponse(responseText={ResponseText}): output → {Output}", responseText, output);
                {
                    new { content = new [] { new { text = string.Empty } } }
                }
            });

        var result = response.output.Select(o => o.content).FirstOrDefault();

		logger.LogDebug("ParseResponse(responseText={ResponseText}): result → {Result}", responseText, result);
		logger.LogDebug("<<< Exiting ParseResponse(responseText={ResponseText})", responseText);
        return result?[0].text ?? string.Empty;
    }

    #endregion
}