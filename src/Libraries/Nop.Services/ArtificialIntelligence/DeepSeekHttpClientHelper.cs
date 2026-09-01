using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Microsoft.Extensions.Logging;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client helper to create DeepSeek requests
/// </summary>
public partial class DeepSeekHttpClientHelper : IArtificialIntelligenceHttpClientHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Configure client
    /// </summary>
    /// <param name="httpClient">HTTP client for configuration</param>
    public virtual void ConfigureClient(HttpClient httpClient)
    {
		_logger.LogDebug(">>> Entering ConfigureClient(httpClient={HttpClient})", httpClient);
        httpClient.BaseAddress = new Uri(ArtificialIntelligenceDefaults.DeepSeekBaseApiUrl);
    }

    /// <summary>
    /// Create HTTP request
    /// </summary>
    /// <param name="settings">Artificial intelligence settings</param>
    /// <param name="query">query to send into artificial intelligence host</param>
    /// <returns>Created HttpRequestMessage</returns>
    public virtual HttpRequestMessage CreateRequest(ArtificialIntelligenceSettings settings, string query)
    {
		_logger.LogDebug(">>> Entering CreateRequest(settings={Settings},query={Query})", settings);
        var request = new HttpRequestMessage { Method = HttpMethod.Post };
        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, $"Bearer {settings.DeepSeekApiKey}");

        var data = JsonConvert.SerializeObject(new
        {
            messages = new List<Dictionary<string, string>> { new() { ["content"] = query, ["role"] = "system" } },
            model = ArtificialIntelligenceDefaults.DeepSeekApiModel
			_logger.LogDebug("CreateRequest(settings={Settings},query={Query}): model → {Model}", settings, model);
        });

        request.Content = new StringContent(data, Encoding.UTF8, MimeTypes.ApplicationJson);

		_logger.LogDebug("<<< Exiting CreateRequest(settings={Settings},query={Query})", settings);
        return request;
    }

    /// <summary>
    /// Parse response
    /// </summary>
    /// <param name="responseText">Response text to parse</param>
    /// <returns>Generated text from artificial intelligence host</returns>
    public virtual string ParseResponse(string responseText)
    {
		_logger.LogDebug(">>> Entering ParseResponse(responseText={ResponseText})", responseText);
        var response = JsonConvert.DeserializeAnonymousType(responseText,
            new
            {
                choices = new[]
				_logger.LogDebug("ParseResponse(responseText={ResponseText}): choices → {Choices}", responseText, choices);
                {
                    new { message =  new { content = string.Empty  } }
                }
            });

        var result = response.choices.FirstOrDefault()?.message?.content;
		_logger.LogDebug("ParseResponse(responseText={ResponseText}): result → {Result}", responseText, result);

		_logger.LogDebug("<<< Exiting ParseResponse(responseText={ResponseText})", responseText);
        return result ?? string.Empty;
    }

    #endregion
}