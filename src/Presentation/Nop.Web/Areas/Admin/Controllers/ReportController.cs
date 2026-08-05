using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ReportController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IPermissionService _permissionService;
    protected readonly IReportModelFactory _reportModelFactory;

    #endregion

    #region Ctor

    public ReportController(
        IPermissionService permissionService,
        IReportModelFactory reportModelFactory)
    {
        _permissionService = permissionService;
        _reportModelFactory = reportModelFactory;
    }

    #endregion

    #region Methods

    #region Sales summary

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> SalesSummary(List<int> orderStatuses = null, List<int> paymentStatuses = null)
    {
		_logger.LogDebug(">>> Entering SalesSummary(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses})", orderStatuses, paymentStatuses);
        //prepare model
        var model = await _reportModelFactory.PrepareSalesSummarySearchModelAsync(new SalesSummarySearchModel
        {
            OrderStatusIds = orderStatuses,
            PaymentStatusIds = paymentStatuses
        });

		_logger.LogDebug("<<< Exiting SalesSummary(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses})", orderStatuses, paymentStatuses);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> SalesSummaryList(SalesSummarySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering SalesSummaryList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareSalesSummaryListModelAsync(searchModel);

		_logger.LogDebug("SalesSummaryList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting SalesSummaryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }


    #endregion

    #region Low stock

    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LowStock()
    {
		_logger.LogDebug(">>> Entering LowStock()");
        //prepare model
        var model = await _reportModelFactory.PrepareLowStockProductSearchModelAsync(new LowStockProductSearchModel());

		_logger.LogDebug("LowStock(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting LowStock()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LowStockList(LowStockProductSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering LowStockList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareLowStockProductListModelAsync(searchModel);

		_logger.LogDebug("LowStockList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting LowStockList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Bestsellers

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> Bestsellers()
    {
		_logger.LogDebug(">>> Entering Bestsellers()");
        //prepare model
        var model = await _reportModelFactory.PrepareBestsellerSearchModelAsync(new BestsellerSearchModel());

		_logger.LogDebug("Bestsellers(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting Bestsellers()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> BestsellersList(BestsellerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering BestsellersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestsellerListModelAsync(searchModel);

		_logger.LogDebug("BestsellersList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting BestsellersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> BestsellersReportAggregates(BestsellerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering BestsellersReportAggregates(searchModel={SearchModel})", searchModel);
        //prepare model
        var totalAmount = await _reportModelFactory.GetBestsellerTotalAmountAsync(searchModel);

		_logger.LogDebug("BestsellersReportAggregates(searchModel={SearchModel}): totalAmount → {TotalAmount}", searchModel, totalAmount);
		_logger.LogDebug("<<< Exiting BestsellersReportAggregates(searchModel={SearchModel})", searchModel);
        return Json(new { aggregatortotal = totalAmount });
    }

    #endregion

    #region Never Sold

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> NeverSold()
    {
		_logger.LogDebug(">>> Entering NeverSold()");
        //prepare model
        var model = await _reportModelFactory.PrepareNeverSoldSearchModelAsync(new NeverSoldReportSearchModel());

		_logger.LogDebug("NeverSold(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting NeverSold()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> NeverSoldList(NeverSoldReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering NeverSoldList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareNeverSoldListModelAsync(searchModel);

		_logger.LogDebug("NeverSoldList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting NeverSoldList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Country sales

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> CountrySales()
    {
		_logger.LogDebug(">>> Entering CountrySales()");
        //prepare model
        var model = await _reportModelFactory.PrepareCountrySalesSearchModelAsync(new CountryReportSearchModel());

		_logger.LogDebug("CountrySales(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting CountrySales()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> CountrySalesList(CountryReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering CountrySalesList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareCountrySalesListModelAsync(searchModel);

		_logger.LogDebug("CountrySalesList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting CountrySalesList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Customer reports

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> RegisteredCustomers()
    {
		_logger.LogDebug(">>> Entering RegisteredCustomers()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

		_logger.LogDebug("RegisteredCustomers(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting RegisteredCustomers()");
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL)]
    public virtual async Task<IActionResult> BestCustomersByOrderTotal()
    {
		_logger.LogDebug(">>> Entering BestCustomersByOrderTotal()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

		_logger.LogDebug("BestCustomersByOrderTotal(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting BestCustomersByOrderTotal()");
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS)]
    public virtual async Task<IActionResult> BestCustomersByNumberOfOrders()
    {
		_logger.LogDebug(">>> Entering BestCustomersByNumberOfOrders()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

		_logger.LogDebug("BestCustomersByNumberOfOrders(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting BestCustomersByNumberOfOrders()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL)]
    public virtual async Task<IActionResult> ReportBestCustomersByOrderTotalList(BestCustomersReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ReportBestCustomersByOrderTotalList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);

		_logger.LogDebug("ReportBestCustomersByOrderTotalList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting ReportBestCustomersByOrderTotalList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS)]
    public virtual async Task<IActionResult> ReportBestCustomersByNumberOfOrdersList(BestCustomersReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);

		_logger.LogDebug("ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ReportRegisteredCustomersList(RegisteredCustomersReportSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ReportRegisteredCustomersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareRegisteredCustomersReportListModelAsync(searchModel);

		_logger.LogDebug("ReportRegisteredCustomersList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting ReportRegisteredCustomersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #endregion
}