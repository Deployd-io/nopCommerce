using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Payments;

/// <summary>
/// Represents a payment methods model
/// </summary>
public partial record PaymentMethodsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PaymentMethodsModel()
    {
		_logger.LogDebug(">>> Entering PaymentMethodsModel()");
        PaymentsMethod = new PaymentMethodSearchModel();
        PaymentMethodRestriction = new PaymentMethodRestrictionModel();
		_logger.LogDebug("PaymentMethodsModel(): PaymentMethodRestriction → {PaymentMethodRestriction}", PaymentMethodRestriction);
		_logger.LogDebug("PaymentMethodsModel(): PaymentsMethod → {PaymentsMethod}", PaymentsMethod);
		_logger.LogDebug("<<< Exiting PaymentMethodsModel()");
    }

    #endregion

    #region Properties

    public PaymentMethodSearchModel PaymentsMethod { get; set; }

    public PaymentMethodRestrictionModel PaymentMethodRestriction { get; set; }

    #endregion
}