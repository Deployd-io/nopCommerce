using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutConfirmModel : BaseNopModel
{
    public CheckoutConfirmModel()
    {
		logger.LogDebug(">>> Entering CheckoutConfirmModel()");
        Warnings = new List<string>();
		logger.LogDebug("<<< Exiting CheckoutConfirmModel()");
		logger.LogDebug("CheckoutConfirmModel(): Warnings → {Warnings}", Warnings);
    }

    public bool TermsOfServiceOnOrderConfirmPage { get; set; }
    public bool TermsOfServicePopup { get; set; }
    public string MinOrderTotalWarning { get; set; }
    public bool DisplayCaptcha { get; set; }

    public IList<string> Warnings { get; set; }
}