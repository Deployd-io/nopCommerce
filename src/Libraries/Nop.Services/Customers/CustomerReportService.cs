using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Helpers;
using Nop.Services.Orders;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Customers;

/// <summary>
/// Customer report service
/// </summary>
public partial class CustomerReportService : ICustomerReportService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<Order> _orderRepository;

    #endregion

    #region Ctor

    public CustomerReportService(ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IRepository<Customer> customerRepository,
        IRepository<Order> orderRepository)
    {
		_logger.LogDebug(">>> Entering CustomerReportService(customerService={CustomerService},dateTimeHelper={DateTimeHelper},customerRepository={CustomerRepository},orderRepository={OrderRepository})", customerService, dateTimeHelper, customerRepository, orderRepository);
        _customerService = customerService;
		_logger.LogDebug("CustomerReportService(customerService={CustomerService},dateTimeHelper={DateTimeHelper},customerRepository={CustomerRepository},orderRepository={OrderRepository}): _customerService → {_customerService}", customerService, dateTimeHelper, customerRepository, orderRepository, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("CustomerReportService(customerService={CustomerService},dateTimeHelper={DateTimeHelper},customerRepository={CustomerRepository},orderRepository={OrderRepository}): _dateTimeHelper → {_dateTimeHelper}", customerService, dateTimeHelper, customerRepository, orderRepository, _dateTimeHelper);
        _customerRepository = customerRepository;
		_logger.LogDebug("CustomerReportService(customerService={CustomerService},dateTimeHelper={DateTimeHelper},customerRepository={CustomerRepository},orderRepository={OrderRepository}): _customerRepository → {_customerRepository}", customerService, dateTimeHelper, customerRepository, orderRepository, _customerRepository);
        _orderRepository = orderRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get best customers
    /// </summary>
    /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
    /// <param name="os">Order status; null to load all records</param>
    /// <param name="ps">Order payment status; null to load all records</param>
    /// <param name="ss">Order shipment status; null to load all records</param>
    /// <param name="orderBy">1 - order by order total, 2 - order by number of orders</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the report
    /// </returns>
    public virtual async Task<IPagedList<BestCustomerReportLine>> GetBestCustomersReportAsync(DateTime? createdFromUtc,
        DateTime? createdToUtc, OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, OrderByEnum orderBy,
        int pageIndex = 0, int pageSize = 214748364)
    {
		_logger.LogDebug(">>> Entering GetBestCustomersReportAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},os={Os},ps={Ps},ss={Ss},orderBy={OrderBy},pageIndex={PageIndex},pageSize={PageSize})", createdFromUtc, createdToUtc, os, ps, ss, orderBy, pageIndex, pageSize);
        int? orderStatusId = null;
        if (os.HasValue)
            orderStatusId = (int)os.Value;

		_logger.LogDebug("GetBestCustomersReportAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},os={Os},ps={Ps},ss={Ss},orderBy={OrderBy},pageIndex={PageIndex},pageSize={PageSize}): orderStatusId → {OrderStatusId}", createdFromUtc, createdToUtc, os, ps, ss, orderBy, pageIndex, pageSize, orderStatusId);
        int? paymentStatusId = null;
        if (ps.HasValue)
            paymentStatusId = (int)ps.Value;

		_logger.LogDebug("GetBestCustomersReportAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},os={Os},ps={Ps},ss={Ss},orderBy={OrderBy},pageIndex={PageIndex},pageSize={PageSize}): paymentStatusId → {PaymentStatusId}", createdFromUtc, createdToUtc, os, ps, ss, orderBy, pageIndex, pageSize, paymentStatusId);
        int? shippingStatusId = null;
        if (ss.HasValue)
            shippingStatusId = (int)ss.Value;
        var query1 = from c in _customerRepository.Table
			_logger.LogDebug("GetBestCustomersReportAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},os={Os},ps={Ps},ss={Ss},orderBy={OrderBy},pageIndex={PageIndex},pageSize={PageSize}): shippingStatusId → {ShippingStatusId}", createdFromUtc, createdToUtc, os, ps, ss, orderBy, pageIndex, pageSize, shippingStatusId);
            join o in _orderRepository.Table on c.Id equals o.CustomerId
            where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                  (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                  (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                  (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                  (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
                  !o.Deleted &&
                  !c.Deleted
            select new { c, o };

        var query2 = from co in query1
            group co by co.c.Id into g
            select new
            {
                CustomerId = g.Key,
                OrderTotal = g.Sum(x => x.o.OrderTotal),
                OrderCount = g.Count()
            };
				_logger.LogDebug("GetBestCustomersReportAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},os={Os},ps={Ps},ss={Ss},orderBy={OrderBy},pageIndex={PageIndex},pageSize={PageSize}): CustomerId → {CustomerId}", createdFromUtc, createdToUtc, os, ps, ss, orderBy, pageIndex, pageSize, CustomerId);
				_logger.LogDebug("GetBestCustomersReportAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},os={Os},ps={Ps},ss={Ss},orderBy={OrderBy},pageIndex={PageIndex},pageSize={PageSize}): OrderTotal → {OrderTotal}", createdFromUtc, createdToUtc, os, ps, ss, orderBy, pageIndex, pageSize, OrderTotal);
				_logger.LogDebug("GetBestCustomersReportAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},os={Os},ps={Ps},ss={Ss},orderBy={OrderBy},pageIndex={PageIndex},pageSize={PageSize}): OrderCount → {OrderCount}", createdFromUtc, createdToUtc, os, ps, ss, orderBy, pageIndex, pageSize, OrderCount);
        query2 = orderBy switch
        {
            OrderByEnum.OrderByQuantity => query2.OrderByDescending(x => x.OrderCount),
            OrderByEnum.OrderByTotalAmount => query2.OrderByDescending(x => x.OrderTotal),
            _ => throw new ArgumentException("Wrong orderBy parameter", nameof(orderBy)),
        };
        var tmp = await query2.ToPagedListAsync(pageIndex, pageSize);
        return new PagedList<BestCustomerReportLine>(tmp.Select(x => new BestCustomerReportLine
            {
				_logger.LogDebug("<<< Exiting GetBestCustomersReportAsync(createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},os={Os},ps={Ps},ss={Ss},orderBy={OrderBy},pageIndex={PageIndex},pageSize={PageSize})", createdFromUtc, createdToUtc, os, ps, ss, orderBy, pageIndex, pageSize);
                CustomerId = x.CustomerId,
                OrderTotal = x.OrderTotal,
                OrderCount = x.OrderCount
            }).ToList(),
            tmp.PageIndex, tmp.PageSize, tmp.TotalCount);
    }

    /// <summary>
    /// Gets a report of customers registered in the last days
    /// </summary>
    /// <param name="days">Customers registered in the last days</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of registered customers
    /// </returns>
    public virtual async Task<int> GetRegisteredCustomersReportAsync(int days)
    {
		_logger.LogDebug(">>> Entering GetRegisteredCustomersReportAsync(days={Days})", days);
        var date = (await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now)).AddDays(-days);

		_logger.LogDebug("GetRegisteredCustomersReportAsync(days={Days}): date → {Date}", days, date);
        var registeredCustomerRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
		_logger.LogDebug("GetRegisteredCustomersReportAsync(days={Days}): registeredCustomerRole → {RegisteredCustomerRole}", days, registeredCustomerRole);
        if (registeredCustomerRole == null)
            return 0;

        return (await _customerService.GetAllCustomersAsync(
            date,
            customerRoleIds: new[] { registeredCustomerRole.Id })).Count;
    }

    #endregion
}