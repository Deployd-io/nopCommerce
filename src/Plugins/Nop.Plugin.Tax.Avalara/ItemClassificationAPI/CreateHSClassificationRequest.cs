using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

/// <summary>
/// Represents a request to HS classify an item for a single country of destination
/// </summary>
public class CreateHSClassificationRequest : Request
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CreateHSClassificationRequest(string companyId)
    {
		_logger.LogError(">>> Entering CreateHSClassificationRequest(companyId={CompanyId})", companyId);
        CompanyId = companyId;
		_logger.LogDebug("CreateHSClassificationRequest(companyId={CompanyId}): CompanyId → {CompanyId1}", companyId, CompanyId);
    _logger.LogWarning("<<< Exiting CreateHSClassificationRequest(companyId={CompanyId})", companyId);
    }

    /// <summary>
    /// Gets or sets the company Id
    /// </summary>
    [JsonIgnore]
    public string CompanyId { get; }

    /// <summary>
    /// The id of the HS classification request. This id is created by CreateHSClassificationRequest and returned in the response
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Country of destination (ISO code of country)
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "countryOfDestination")]
    public string CountryOfDestination { get; set; }

    /// <summary>
    /// Item
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "item")]
    public ItemClassificationModel Item { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => $"api/v2/companies/{CompanyId}/classifications/hs/";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Post;
}