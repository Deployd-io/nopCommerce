using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutPickupPointsModel : BaseNopModel
{
    public CheckoutPickupPointsModel()
    {
		logger.LogDebug(">>> Entering CheckoutPickupPointsModel()");
        Warnings = new List<string>();
        PickupPoints = new List<CheckoutPickupPointModel>();
		logger.LogDebug("<<< Exiting CheckoutPickupPointsModel()");
		logger.LogDebug("CheckoutPickupPointsModel(): Warnings → {Warnings}", Warnings);
		logger.LogDebug("CheckoutPickupPointsModel(): PickupPoints → {PickupPoints}", PickupPoints);
    }

    public IList<string> Warnings { get; set; }

    public IList<CheckoutPickupPointModel> PickupPoints { get; set; }
    public bool AllowPickupInStore { get; set; }
    public bool PickupInStore { get; set; }
    public bool PickupInStoreOnly { get; set; }
    public bool DisplayPickupPointsOnMap { get; set; }
    public string GoogleMapsApiKey { get; set; }
}