using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping;

/// <summary>
/// Represents a response of getting shipping rate options
/// </summary>
public partial class GetShippingOptionResponse : BaseNopResult
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public GetShippingOptionResponse()
    {
		_logger.LogDebug(">>> Entering GetShippingOptionResponse()");
        ShippingOptions = new List<ShippingOption>();
		_logger.LogDebug("GetShippingOptionResponse(): ShippingOptions → {ShippingOptions}", ShippingOptions);
    }

    /// <summary>
    /// Gets or sets a list of shipping options
    /// </summary>
    public IList<ShippingOption> ShippingOptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether shipping is done from multiple locations (warehouses)
    /// </summary>
    public bool ShippingFromMultipleLocations { get; set; }
}