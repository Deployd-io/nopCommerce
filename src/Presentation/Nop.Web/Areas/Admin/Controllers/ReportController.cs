using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ReportController : BaseAdminController
{
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
		logger.LogDebug(">>> Entering SalesSummary(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses})", orderStatuses, paymentStatuses);
        //prepare model
        var model = await _reportModelFactory.PrepareSalesSummarySearchModelAsync(new SalesSummarySearchModel
        {
            OrderStatusIds = orderStatuses,
            PaymentStatusIds = paymentStatuses
        });

		logger.LogDebug("<<< Exiting SalesSummary(orderStatuses={OrderStatuses},paymentStatuses={PaymentStatuses})", orderStatuses, paymentStatuses);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.SALES_SUMMARY)]
    public virtual async Task<IActionResult> SalesSummaryList(SalesSummarySearchModel searchModel)
    {
		logger.LogDebug(">>> Entering SalesSummaryList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareSalesSummaryListModelAsync(searchModel);

		logger.LogDebug("SalesSummaryList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting SalesSummaryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }


    #endregion

    #region Low stock

    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LowStock()
    {
		logger.LogDebug(">>> Entering LowStock()");
        //prepare model
        var model = await _reportModelFactory.PrepareLowStockProductSearchModelAsync(new LowStockProductSearchModel());

		logger.LogDebug("LowStock(): model → {Model}", model);
		logger.LogDebug("<<< Exiting LowStock()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Reports.LOW_STOCK)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LowStockList(LowStockProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering LowStockList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareLowStockProductListModelAsync(searchModel);

		logger.LogDebug("LowStockList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting LowStockList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Bestsellers

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> Bestsellers()
    {
		logger.LogDebug(">>> Entering Bestsellers()");
        //prepare model
        var model = await _reportModelFactory.PrepareBestsellerSearchModelAsync(new BestsellerSearchModel());

		logger.LogDebug("Bestsellers(): model → {Model}", model);
		logger.LogDebug("<<< Exiting Bestsellers()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> BestsellersList(BestsellerSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering BestsellersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestsellerListModelAsync(searchModel);

		logger.LogDebug("BestsellersList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting BestsellersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.BESTSELLERS)]
    public virtual async Task<IActionResult> BestsellersReportAggregates(BestsellerSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering BestsellersReportAggregates(searchModel={SearchModel})", searchModel);
        //prepare model
        var totalAmount = await _reportModelFactory.GetBestsellerTotalAmountAsync(searchModel);

		logger.LogDebug("BestsellersReportAggregates(searchModel={SearchModel}): totalAmount → {TotalAmount}", searchModel, totalAmount);
		logger.LogDebug("<<< Exiting BestsellersReportAggregates(searchModel={SearchModel})", searchModel);
        return Json(new { aggregatortotal = totalAmount });
    }

    #endregion

    #region Never Sold

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> NeverSold()
    {
		logger.LogDebug(">>> Entering NeverSold()");
        //prepare model
        var model = await _reportModelFactory.PrepareNeverSoldSearchModelAsync(new NeverSoldReportSearchModel());

		logger.LogDebug("NeverSold(): model → {Model}", model);
		logger.LogDebug("<<< Exiting NeverSold()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.PRODUCTS_NEVER_PURCHASED)]
    public virtual async Task<IActionResult> NeverSoldList(NeverSoldReportSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering NeverSoldList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareNeverSoldListModelAsync(searchModel);

		logger.LogDebug("NeverSoldList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting NeverSoldList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Country sales

    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> CountrySales()
    {
		logger.LogDebug(">>> Entering CountrySales()");
        //prepare model
        var model = await _reportModelFactory.PrepareCountrySalesSearchModelAsync(new CountryReportSearchModel());

		logger.LogDebug("CountrySales(): model → {Model}", model);
		logger.LogDebug("<<< Exiting CountrySales()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.COUNTRY_SALES)]
    public virtual async Task<IActionResult> CountrySalesList(CountryReportSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering CountrySalesList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareCountrySalesListModelAsync(searchModel);

		logger.LogDebug("CountrySalesList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting CountrySalesList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Customer reports

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> RegisteredCustomers()
    {
		logger.LogDebug(">>> Entering RegisteredCustomers()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

		logger.LogDebug("RegisteredCustomers(): model → {Model}", model);
		logger.LogDebug("<<< Exiting RegisteredCustomers()");
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL)]
    public virtual async Task<IActionResult> BestCustomersByOrderTotal()
    {
		logger.LogDebug(">>> Entering BestCustomersByOrderTotal()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

		logger.LogDebug("BestCustomersByOrderTotal(): model → {Model}", model);
		logger.LogDebug("<<< Exiting BestCustomersByOrderTotal()");
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS)]
    public virtual async Task<IActionResult> BestCustomersByNumberOfOrders()
    {
		logger.LogDebug(">>> Entering BestCustomersByNumberOfOrders()");
        //prepare model
        var model = await _reportModelFactory.PrepareCustomerReportsSearchModelAsync(new CustomerReportsSearchModel());

		logger.LogDebug("BestCustomersByNumberOfOrders(): model → {Model}", model);
		logger.LogDebug("<<< Exiting BestCustomersByNumberOfOrders()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_ORDER_TOTAL)]
    public virtual async Task<IActionResult> ReportBestCustomersByOrderTotalList(BestCustomersReportSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ReportBestCustomersByOrderTotalList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);

		logger.LogDebug("ReportBestCustomersByOrderTotalList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ReportBestCustomersByOrderTotalList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS)]
    public virtual async Task<IActionResult> ReportBestCustomersByNumberOfOrdersList(BestCustomersReportSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareBestCustomersReportListModelAsync(searchModel);

		logger.LogDebug("ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ReportBestCustomersByNumberOfOrdersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Reports.REGISTERED_CUSTOMERS)]
    public virtual async Task<IActionResult> ReportRegisteredCustomersList(RegisteredCustomersReportSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ReportRegisteredCustomersList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _reportModelFactory.PrepareRegisteredCustomersReportListModelAsync(searchModel);

		logger.LogDebug("ReportRegisteredCustomersList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ReportRegisteredCustomersList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #endregion
}
