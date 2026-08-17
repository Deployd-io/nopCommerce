using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Models.Customer;

/// <summary>
/// Represents a tax exemption model
/// </summary>
public record TaxExemptionModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TaxExemptionModel()
    {
		_logger.LogDebug(">>> Entering TaxExemptionModel()");
        Certificates = new List<ExemptionCertificateModel>();
        AvailableExposureZones = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting TaxExemptionModel()");
		_logger.LogDebug("TaxExemptionModel(): AvailableExposureZones → {AvailableExposureZones}", AvailableExposureZones);
		_logger.LogDebug("TaxExemptionModel(): Certificates → {Certificates}", Certificates);
    }

    #endregion

    #region Properties

    public string Token { get; set; }

    public string Link { get; set; }

    public int CustomerId { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.ExemptionCertificates.Add.ExposureZone")]
    public int ExposureZone { get; set; }

    public IList<ExemptionCertificateModel> Certificates { get; set; }

    public IList<SelectListItem> AvailableExposureZones { get; set; }

    #endregion
}