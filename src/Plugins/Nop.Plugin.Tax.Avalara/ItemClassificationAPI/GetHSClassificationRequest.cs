using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

public class GetHSClassificationRequestc : Request
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public GetHSClassificationRequestc(string hsClassificationId, string companyId)
    {
		_logger.LogDebug(">>> Entering GetHSClassificationRequestc(hsClassificationId={HsClassificationId},companyId={CompanyId})", hsClassificationId, companyId);
        HSClassificationId = hsClassificationId;
		_logger.LogDebug("GetHSClassificationRequestc(hsClassificationId={HsClassificationId},companyId={CompanyId}): HSClassificationId → {HSClassificationId}", hsClassificationId, companyId, HSClassificationId);
        CompanyId = companyId;
    }

    /// <summary>
    /// Gets or sets the HS classification Id
    /// </summary>
    [JsonIgnore]
    public string HSClassificationId { get; }

    /// <summary>
    /// Gets or sets the company Id
    /// </summary>
    [JsonIgnore]
    public string CompanyId { get; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => $"api/v2/companies/{CompanyId}/classifications/hs/{HSClassificationId}";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override string Method => HttpMethods.Get;
}