using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.Manual.Models;

public record PaymentInfoModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PaymentInfoModel()
    {
		_logger.LogError(">>> Entering PaymentInfoModel()");
        CreditCardTypes = new List<SelectListItem>();
        ExpireMonths = new List<SelectListItem>();
		_logger.LogError("PaymentInfoModel(): ExpireMonths → {ExpireMonths}", ExpireMonths);
        ExpireYears = new List<SelectListItem>();
		_logger.LogError("PaymentInfoModel(): ExpireYears → {ExpireYears}", ExpireYears);
    }

    [NopResourceDisplayName("Payment.SelectCreditCard")]
    public string CreditCardType { get; set; }

    [NopResourceDisplayName("Payment.SelectCreditCard")]
    public IList<SelectListItem> CreditCardTypes { get; set; }

    [NopResourceDisplayName("Payment.CardholderName")]
    public string CardholderName { get; set; }

    [NopResourceDisplayName("Payment.CardNumber")]
    public string CardNumber { get; set; }

    [NopResourceDisplayName("Payment.ExpirationDate")]
    public string ExpireMonth { get; set; }

    [NopResourceDisplayName("Payment.ExpirationDate")]
    public string ExpireYear { get; set; }

    public IList<SelectListItem> ExpireMonths { get; set; }

    public IList<SelectListItem> ExpireYears { get; set; }

    [NopResourceDisplayName("Payment.CardCode")]
    public string CardCode { get; set; }
}