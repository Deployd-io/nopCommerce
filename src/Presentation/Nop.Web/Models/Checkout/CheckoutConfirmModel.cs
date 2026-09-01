using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutConfirmModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CheckoutConfirmModel()
    {
		_logger.LogDebug(">>> Entering CheckoutConfirmModel()");
        Warnings = new List<string>();
		_logger.LogDebug("CheckoutConfirmModel(): Warnings → {Warnings}", Warnings);
    }

    public bool TermsOfServiceOnOrderConfirmPage { get; set; }
    public bool TermsOfServicePopup { get; set; }
    public string MinOrderTotalWarning { get; set; }
    public bool DisplayCaptcha { get; set; }

    public IList<string> Warnings { get; set; }
}