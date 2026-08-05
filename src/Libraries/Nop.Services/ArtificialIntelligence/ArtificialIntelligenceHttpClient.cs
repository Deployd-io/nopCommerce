using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client to request artificial intelligence
/// </summary>
public partial class ArtificialIntelligenceHttpClient
{
    #region Fields

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;
    protected readonly HttpClient _httpClient;
    protected readonly IArtificialIntelligenceHttpClientHelper _artificialIntelligenceHttpClientHelper;

    #endregion

    #region Ctor

    public ArtificialIntelligenceHttpClient(ArtificialIntelligenceSettings artificialIntelligenceSettings,
        HttpClient httpClient)
    {
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
        _httpClient = httpClient;

        //configure client
        httpClient.Timeout = TimeSpan.FromSeconds(artificialIntelligenceSettings.RequestTimeout ??
            ArtificialIntelligenceDefaults.RequestTimeout);

        _artificialIntelligenceHttpClientHelper = _artificialIntelligenceSettings.ProviderType switch
        {
            ArtificialIntelligenceProviderType.Gemini => new GeminiHttpClientHelper(),
            ArtificialIntelligenceProviderType.ChatGpt => new ChatGptHttpClientHelper(),
            ArtificialIntelligenceProviderType.DeepSeek => new DeepSeekHttpClientHelper(),
            _ => throw new ArgumentOutOfRangeException()
        };

        _artificialIntelligenceHttpClientHelper.ConfigureClient(httpClient);
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
		logger.LogDebug(">>> Entering SendQueryAsync(query={Query})");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var request = _artificialIntelligenceHttpClientHelper.CreateRequest(_artificialIntelligenceSettings, query);

		logger.LogDebug("SendQueryAsync(query={Query}): request → {Request}", request);
        var httpResponse = await _httpClient.SendAsync(request);
        var response = await httpResponse.Content.ReadAsStringAsync();
		logger.LogInformation("SendQueryAsync(query)={Query}: external service call took {Arg1} ms", __logStart.ElapsedMilliseconds);
		logger.LogDebug("SendQueryAsync(query={Query}): httpResponse → {HttpResponse}", httpResponse);
		logger.LogDebug("SendQueryAsync(query={Query}): response → {Response}", response);

        if (!httpResponse.IsSuccessStatusCode)
            throw new NopException(httpResponse.ReasonPhrase, innerException: new Exception(response));

        var result = _artificialIntelligenceHttpClientHelper.ParseResponse(response);

		logger.LogDebug("SendQueryAsync(query={Query}): result → {Result}", result);
		logger.LogDebug("<<< Exiting SendQueryAsync(query={Query})");
        return result;
    }

    #endregion
}