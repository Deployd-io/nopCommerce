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
		_logger.LogError(">>> Entering ReportController(permissionService={PermissionService},reportModelFactory={ReportModelFactory})", permissionService, reportModelFactory);
        _permissionService = permissionService;
        _reportModelFactory = reportModelFactory;
		_logger.LogDebug("ReportController(permissionService={PermissionService},reportModelFactory={ReportModelFactory}): _reportModelFactory → {_reportModelFactory}", permissionService, reportModelFactory, _reportModelFactory);
    }

    #endregion

    #region Methods

    #region Sales summary

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> SalesSummary(List<int> orderStatuses = null, List<int> paymentStatuses = null)
    {
		_logger.LogError(">>> Entering SalesSummary(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses})", orderStatuses, paymentStatuses);
        //prepare model
        var model = await _reportModelFactory.PrepareSalesSummarySearchModelAsync(new SalesSummarySearchModel
        {
            OrderStatusIds = orderStatuses,
            PaymentStatusIds = paymentStatuses
        });
		_logger.LogDebug("SalesSummary(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses}): PaymentStatusIds → {PaymentStatusIds}", orderStatuses, paymentStatuses, PaymentStatusIds);

		_logger.LogWarning("<<< Exiting SalesSummary(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses})", orderStatuses, paymentStatuses);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> SalesSummaryList(SalesSummarySearchModel searchModel)
    {
		_logger.LogError(">>> Entering SalesSummaryList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareSalesSummaryListModelAsync(searchModel);
		_logger.LogError("SalesSummaryList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting SalesSummaryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }


    #endregion

    #region Low stock

    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LowStock()
    {
		_logger.LogError(">>> Entering LowStock()");
        //prepare model
        var model = await _reportModelFactory.PrepareLowStockProductSearchModelAsync(new LowStockProductSearchModel());
		_logger.LogError("LowStock(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting LowStock()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LowStockList(LowStockProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering LowStockList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareLowStockProductListModelAsync(searchModel);
		_logger.LogError("LowStockList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting LowStockList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Bestsellers

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> Bestsellers()
    {
		_logger.LogError(">>> Entering Bestsellers()");
        //prepare model
        var model = await _reportModelFactory.PrepareBestsellerSearchModelAsync(new BestsellerSearchModel());
		_logger.LogError("Bestsellers(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Bestsellers()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> BestsellersList(BestsellerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering BestsellersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestsellerListModelAsync(searchModel);
		_logger.LogError("BestsellersList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting BestsellersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> BestsellersReportAggregates(BestsellerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering BestsellersReportAggregates(searchModel={SearchModel})", searchModel);
        //prepare model
        var totalAmount = await _reportModelFactory.GetBestsellerTotalAmountAsync(searchModel);
		_logger.LogError("BestsellersReportAggregates(searchModel={SearchModel}): totalAmount → {TotalAmount}", searchModel, totalAmount);

		_logger.LogWarning("<<< Exiting BestsellersReportAggregates(searchModel={SearchModel})", searchModel);
        return Json(new { aggregatortotal = totalAmount });
    }

    #endregion

    #region Never Sold

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> NeverSold()
    {
		_logger.LogError(">>> Entering NeverSold()");
        //prepare model
        var model = await _reportModelFactory.PrepareNeverSoldSearchModelAsync(new NeverSoldReportSearchModel());
		_logger.LogError("NeverSold(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting NeverSold()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> NeverSoldList(NeverSoldReportSearchModel searchModel)
    {
		_logger.LogError(">>> Entering NeverSoldList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareNeverSoldListModelAsync(searchModel);
		_logger.LogError("NeverSoldList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting NeverSoldList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Country sales

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> CountrySales()
    {
		_logger.LogError(">>> Entering CountrySales()");
        //prepare model
        var model = await _reportModelFactory.PrepareCountrySalesSearchModelAsync(new CountryReportSearchModel());
		_logger.LogError("CountrySales(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting CountrySales()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> CountrySalesList(CountryReportSearchModel searchModel)
    {
		_logger.LogError(">>> Entering CountrySalesList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareCountrySalesListModelAsync(searchModel);
		_logger.LogError("CountrySalesList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting CountrySalesList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Customer reports

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> RegisteredCustomers()
    {
		_logger.LogError(">>> Entering RegisteredCustomers()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());
		_logger.LogError("RegisteredCustomers(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting RegisteredCustomers()");
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL)]
    public virtual async Task<IActionResult> BestCustomersByOrderTotal()
    {
		_logger.LogError(">>> Entering BestCustomersByOrderTotal()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());
		_logger.LogError("BestCustomersByOrderTotal(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting BestCustomersByOrderTotal()");
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS)]
    public virtual async Task<IActionResult> BestCustomersByNumberOfOrders()
    {
		_logger.LogError(">>> Entering BestCustomersByNumberOfOrders()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());
		_logger.LogError("BestCustomersByNumberOfOrders(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting BestCustomersByNumberOfOrders()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL)]
    public virtual async Task<IActionResult> ReportBestCustomersByOrderTotalList(BestCustomersReportSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ReportBestCustomersByOrderTotalList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);
		_logger.LogError("ReportBestCustomersByOrderTotalList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ReportBestCustomersByOrderTotalList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS)]
    public virtual async Task<IActionResult> ReportBestCustomersByNumberOfOrdersList(BestCustomersReportSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);
		_logger.LogError("ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ReportRegisteredCustomersList(RegisteredCustomersReportSearchModel searchModel)
    {
		_logger.LogError(">>> Entering ReportRegisteredCustomersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareRegisteredCustomersReportListModelAsync(searchModel);
		_logger.LogError("ReportRegisteredCustomersList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ReportRegisteredCustomersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #endregion
}