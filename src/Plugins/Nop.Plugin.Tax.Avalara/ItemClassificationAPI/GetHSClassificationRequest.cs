using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

public class GetHSClassificationRequestc : Request
{
    public GetHSClassificationRequestc(string hsClassificationId, string companyId)
    {
		logger.LogDebug(">>> Entering GetHSClassificationRequestc(hsClassificationId={HsClassificationId},companyId={CompanyId})", hsClassificationId, companyId);
        HSClassificationId = hsClassificationId;
        CompanyId = companyId;
		logger.LogDebug("<<< Exiting GetHSClassificationRequestc(hsClassificationId={HsClassificationId},companyId={CompanyId})", hsClassificationId, companyId);
		logger.LogDebug("GetHSClassificationRequestc(hsClassificationId={HsClassificationId},companyId={CompanyId}): CompanyId → {CompanyId1}", hsClassificationId, companyId, CompanyId);
		logger.LogDebug("GetHSClassificationRequestc(hsClassificationId={HsClassificationId},companyId={CompanyId}): HSClassificationId → {HSClassificationId}", hsClassificationId, companyId, HSClassificationId);
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
