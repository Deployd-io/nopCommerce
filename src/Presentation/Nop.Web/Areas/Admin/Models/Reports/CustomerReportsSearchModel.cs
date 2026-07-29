using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a customer reports search model
/// </summary>
public partial record CustomerReportsSearchModel : BaseSearchModel
{
    #region Ctor

    public CustomerReportsSearchModel()
    {
		logger.LogDebug(">>> Entering CustomerReportsSearchModel()");
        BestCustomersByOrderTotal = new BestCustomersReportSearchModel();
        BestCustomersByNumberOfOrders = new BestCustomersReportSearchModel();
		logger.LogDebug("CustomerReportsSearchModel(): BestCustomersByOrderTotal → {BestCustomersByOrderTotal}", BestCustomersByOrderTotal);
		logger.LogDebug("CustomerReportsSearchModel(): BestCustomersByNumberOfOrders → {BestCustomersByNumberOfOrders}", BestCustomersByNumberOfOrders);
        RegisteredCustomers = new RegisteredCustomersReportSearchModel();
		logger.LogDebug("<<< Exiting CustomerReportsSearchModel()");
		logger.LogDebug("CustomerReportsSearchModel(): RegisteredCustomers → {RegisteredCustomers}", RegisteredCustomers);
    }

    #endregion

    #region Properties

    public BestCustomersReportSearchModel BestCustomersByOrderTotal { get; set; }

    public BestCustomersReportSearchModel BestCustomersByNumberOfOrders { get; set; }

    public RegisteredCustomersReportSearchModel RegisteredCustomers { get; set; }

    #endregion
}