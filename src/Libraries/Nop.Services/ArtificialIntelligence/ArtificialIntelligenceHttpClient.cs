using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Microsoft.Extensions.Logging;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client to request artificial intelligence
/// </summary>
public partial class ArtificialIntelligenceHttpClient
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;
    protected readonly HttpClient _httpClient;
    protected readonly IArtificialIntelligenceHttpClientHelper _artificialIntelligenceHttpClientHelper;

    #endregion

    #region Ctor

    public ArtificialIntelligenceHttpClient(ArtificialIntelligenceSettings artificialIntelligenceSettings,
        HttpClient httpClient)
    {
		_logger.LogError(">>> Entering ArtificialIntelligenceHttpClient(artificialIntelligenceSettings={ArtificialIntelligenceSettings},httpClient={HttpClient})", artificialIntelligenceSettings, httpClient);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _httpClient = httpClient;
		_logger.LogDebug("ArtificialIntelligenceHttpClient(artificialIntelligenceSettings={ArtificialIntelligenceSettings},httpClient={HttpClient}): _httpClient → {_httpClient}", artificialIntelligenceSettings, httpClient, _httpClient);
		_logger.LogDebug("ArtificialIntelligenceHttpClient(artificialIntelligenceSettings={ArtificialIntelligenceSettings},httpClient={HttpClient}): _artificialIntelligenceSettings → {_artificialIntelligenceSettings}", artificialIntelligenceSettings, httpClient, _artificialIntelligenceSettings);

        //configure client
        httpClient.Timeout = TimeSpan.FromSeconds(artificialIntelligenceSettings.RequestTimeout ??
            ArtificialIntelligenceDefaults.RequestTimeout);

		_logger.LogError("ArtificialIntelligenceHttpClient(artificialIntelligenceSettings,httpClient)={ArtificialIntelligenceSettings},{HttpClient}: external service call took {Arg2} ms", artificialIntelligenceSettings, httpClient, __logStart.ElapsedMilliseconds);
        _artificialIntelligenceHttpClientHelper = _artificialIntelligenceSettings.ProviderType switch
        {
            ArtificialIntelligenceProviderType.Gemini => new GeminiHttpClientHelper(),
            ArtificialIntelligenceProviderType.ChatGpt => new ChatGptHttpClientHelper(),
            ArtificialIntelligenceProviderType.DeepSeek => new DeepSeekHttpClientHelper(),
            _ => throw new ArgumentOutOfRangeException()
        };
			_logger.LogDebug("ArtificialIntelligenceHttpClient(artificialIntelligenceSettings={ArtificialIntelligenceSettings},httpClient={HttpClient}): _artificialIntelligenceHttpClientHelper → {_artificialIntelligenceHttpClientHelper}", artificialIntelligenceSettings, httpClient, _artificialIntelligenceHttpClientHelper);

        _artificialIntelligenceHttpClientHelper.ConfigureClient(httpClient);
		_logger.LogWarning("<<< Exiting ArtificialIntelligenceHttpClient(artificialIntelligenceSettings={ArtificialIntelligenceSettings},httpClient={HttpClient})", artificialIntelligenceSettings, httpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Send query to artificial intelligence host
    /// </summary>
    /// <param name="query">Query to artificial intelligence host</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the response from the artificial intelligence host
    /// </returns>
    public virtual async Task<string> SendQueryAsync(string query)
    {
		_logger.LogError(">>> Entering SendQueryAsync(query={Query})");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var request = _artificialIntelligenceHttpClientHelper.CreateRequest(_artificialIntelligenceSettings, query);

		_logger.LogError("SendQueryAsync(query={Query}): request → {Request}", request);
        var httpResponse = await _httpClient.SendAsync(request);
		_logger.LogError("SendQueryAsync(query={Query}): httpResponse → {HttpResponse}", httpResponse);
        var response = await httpResponse.Content.ReadAsStringAsync();
		_logger.LogError("SendQueryAsync(query)={Query}: external service call took {Arg1} ms", __logStart.ElapsedMilliseconds);
		_logger.LogError("SendQueryAsync(query={Query}): response → {Response}", response);

        if (!httpResponse.IsSuccessStatusCode)
			_logger.LogError("Throwing NopException from SendQueryAsync(query={Query})");
            throw new NopException(httpResponse.ReasonPhrase, innerException: new Exception(response));

        var result = _artificialIntelligenceHttpClientHelper.ParseResponse(response);
		_logger.LogError("SendQueryAsync(query={Query}): result → {Result}", result);

		_logger.LogWarning("<<< Exiting SendQueryAsync(query={Query})");
        return result;
    }

    #endregion
}