using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutPickupPointsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CheckoutPickupPointsModel()
    {
		_logger.LogDebug(">>> Entering CheckoutPickupPointsModel()");
        Warnings = new List<string>();
		_logger.LogDebug("CheckoutPickupPointsModel(): Warnings → {Warnings}", Warnings);
        PickupPoints = new List<CheckoutPickupPointModel>();
    }

    public IList<string> Warnings { get; set; }

    public IList<CheckoutPickupPointModel> PickupPoints { get; set; }
    public bool AllowPickupInStore { get; set; }
    public bool PickupInStore { get; set; }
    public bool PickupInStoreOnly { get; set; }
    public bool DisplayPickupPointsOnMap { get; set; }
    public string GoogleMapsApiKey { get; set; }
}