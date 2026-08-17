using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a customer reports search model
/// </summary>
public partial record CustomerReportsSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomerReportsSearchModel()
    {
		_logger.LogDebug(">>> Entering CustomerReportsSearchModel()");
        BestCustomersByOrderTotal = new BestCustomersReportSearchModel();
        BestCustomersByNumberOfOrders = new BestCustomersReportSearchModel();
		_logger.LogDebug("CustomerReportsSearchModel(): BestCustomersByNumberOfOrders → {BestCustomersByNumberOfOrders}", BestCustomersByNumberOfOrders);
		_logger.LogDebug("CustomerReportsSearchModel(): BestCustomersByOrderTotal → {BestCustomersByOrderTotal}", BestCustomersByOrderTotal);
        RegisteredCustomers = new RegisteredCustomersReportSearchModel();
		_logger.LogDebug("<<< Exiting CustomerReportsSearchModel()");
		_logger.LogDebug("CustomerReportsSearchModel(): RegisteredCustomers → {RegisteredCustomers}", RegisteredCustomers);
    }

    #endregion

    #region Properties

    public BestCustomersReportSearchModel BestCustomersByOrderTotal { get; set; }

    public BestCustomersReportSearchModel BestCustomersByNumberOfOrders { get; set; }

    public RegisteredCustomersReportSearchModel RegisteredCustomers { get; set; }

    #endregion
}