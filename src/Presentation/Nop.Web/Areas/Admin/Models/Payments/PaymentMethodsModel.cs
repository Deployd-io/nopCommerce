using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Payments;

/// <summary>
/// Represents a payment methods model
/// </summary>
public partial record PaymentMethodsModel : BaseNopModel
{
    #region Ctor

    public PaymentMethodsModel()
    {
		logger.LogDebug(">>> Entering PaymentMethodsModel()");
        PaymentsMethod = new PaymentMethodSearchModel();
        PaymentMethodRestriction = new PaymentMethodRestrictionModel();
		logger.LogDebug("<<< Exiting PaymentMethodsModel()");
		logger.LogDebug("PaymentMethodsModel(): PaymentsMethod → {PaymentsMethod}", PaymentsMethod);
		logger.LogDebug("PaymentMethodsModel(): PaymentMethodRestriction → {PaymentMethodRestriction}", PaymentMethodRestriction);
    }

    #endregion

    #region Properties

    public PaymentMethodSearchModel PaymentsMethod { get; set; }

    public PaymentMethodRestrictionModel PaymentMethodRestriction { get; set; }

    #endregion
}