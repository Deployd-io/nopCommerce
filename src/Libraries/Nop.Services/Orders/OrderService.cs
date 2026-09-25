using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Html;
using Nop.Services.Shipping;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Order service
/// </summary>
public partial class OrderService : IOrderService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly IProductService _productService;
    protected readonly IRepository<Address> _addressRepository;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IRepository<OrderItem> _orderItemRepository;
    protected readonly IRepository<OrderNote> _orderNoteRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
    protected readonly IRepository<RecurringPayment> _recurringPaymentRepository;
    protected readonly IRepository<RecurringPaymentHistory> _recurringPaymentHistoryRepository;
    protected readonly IShipmentService _shipmentService;
    private static readonly char[] _separator = [';'];

    #endregion

    #region Ctor

    public OrderService(IHtmlFormatter htmlFormatter,
        IProductService productService,
        IRepository<Address> addressRepository,
        IRepository<Customer> customerRepository,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<OrderNote> orderNoteRepository,
        IRepository<Product> productRepository,
        IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
        IRepository<RecurringPayment> recurringPaymentRepository,
        IRepository<RecurringPaymentHistory> recurringPaymentHistoryRepository,
        IShipmentService shipmentService)
    {
		_logger.LogError(">>> Entering OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService})", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService);
        _htmlFormatter = htmlFormatter;
        _productService = productService;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _productService → {_productService}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _productService);
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _htmlFormatter → {_htmlFormatter}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _htmlFormatter);
    _logger.LogWarning("<<< Exiting OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService})", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService);
        _addressRepository = addressRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _addressRepository → {_addressRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _addressRepository);
        _customerRepository = customerRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _customerRepository → {_customerRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _customerRepository);
        _orderRepository = orderRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _orderRepository → {_orderRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _orderRepository);
        _orderItemRepository = orderItemRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _orderItemRepository → {_orderItemRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _orderItemRepository);
        _orderNoteRepository = orderNoteRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _orderNoteRepository → {_orderNoteRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _orderNoteRepository);
        _productRepository = productRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _productRepository → {_productRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _productRepository);
        _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _productWarehouseInventoryRepository → {_productWarehouseInventoryRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _productWarehouseInventoryRepository);
        _recurringPaymentRepository = recurringPaymentRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _recurringPaymentRepository → {_recurringPaymentRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _recurringPaymentRepository);
        _recurringPaymentHistoryRepository = recurringPaymentHistoryRepository;
		_logger.LogDebug("OrderService(htmlFormatter={HtmlFormatter},productService={ProductService},addressRepository={AddressRepository},customerRepository={CustomerRepository},orderRepository={OrderRepository},orderItemRepository={OrderItemRepository},orderNoteRepository={OrderNoteRepository},productRepository={ProductRepository},productWarehouseInventoryRepository={ProductWarehouseInventoryRepository},recurringPaymentRepository={RecurringPaymentRepository},recurringPaymentHistoryRepository={RecurringPaymentHistoryRepository},shipmentService={ShipmentService}): _recurringPaymentHistoryRepository → {_recurringPaymentHistoryRepository}", htmlFormatter, productService, addressRepository, customerRepository, orderRepository, orderItemRepository, orderNoteRepository, productRepository, productWarehouseInventoryRepository, recurringPaymentRepository, recurringPaymentHistoryRepository, shipmentService, _recurringPaymentHistoryRepository);
        _shipmentService = shipmentService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the value indicating whether there are shipment items with a positive quantity in order shipments.
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="predicate">Predicate to filter shipments or null to check all shipments</param>
    /// <returns>The <see cref="Task"/> containing the value indicating whether there are shipment items with a positive quantity in order shipments.</returns>
    protected virtual async Task<bool> HasShipmentItemsAsync(Order order, Func<Shipment, bool> predicate = null)
    {
		_logger.LogError(">>> Entering HasShipmentItemsAsync(order={Order},predicate={Predicate})", order, predicate);
        var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(order.Id);
		_logger.LogError("HasShipmentItemsAsync(order={Order},predicate={Predicate}): shipments → {Shipments}", order, predicate, shipments);
        if (shipments?.Any(shipment => predicate == null || predicate(shipment)) == true)
        {
            var orderItems = await GetOrderItemsAsync(order.Id, isShipEnabled: true);
			_logger.LogError("HasShipmentItemsAsync(order={Order},predicate={Predicate}): orderItems → {OrderItems}", order, predicate, orderItems);
            if (orderItems?.Any() == true)
            {
                foreach (var shipment in shipments)
                {
                    if (predicate?.Invoke(shipment) == false)
                        continue;

                    bool hasPositiveQuantity(ShipmentItem shipmentItem)
                    {
                        return orderItems.Any(orderItem => orderItem.Id == shipmentItem.OrderItemId && shipmentItem.Quantity > 0);
                    }

                    var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);
					_logger.LogError("HasShipmentItemsAsync(order={Order},predicate={Predicate}): shipmentItems → {ShipmentItems}", order, predicate, shipmentItems);
                    if (shipmentItems?.Any(hasPositiveQuantity) == true)
                        return true;
                }
            }
        }

        return false;
    }

    #endregion

    #region Methods

    #region Orders

    /// <summary>
    /// Gets an order
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order
    /// </returns>
    public virtual async Task<Order> GetOrderByIdAsync(int orderId)
    {
		_logger.LogError(">>> Entering GetOrderByIdAsync(orderId={OrderId})", orderId);
		_logger.LogError("GetOrderByIdAsync(orderId)={OrderId}: find query executing", orderId);
    _logger.LogWarning("<<< Exiting GetOrderByIdAsync(orderId={OrderId})", orderId);
        return await _orderRepository.GetByIdAsync(orderId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets an order
    /// </summary>
    /// <param name="customOrderNumber">The custom order number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order
    /// </returns>
    public virtual async Task<Order> GetOrderByCustomOrderNumberAsync(string customOrderNumber)
    {
		_logger.LogError(">>> Entering GetOrderByCustomOrderNumberAsync(customOrderNumber={CustomOrderNumber})", customOrderNumber);
		_logger.LogWarning("<<< Exiting GetOrderByCustomOrderNumberAsync(customOrderNumber={CustomOrderNumber})", customOrderNumber);
        if (string.IsNullOrEmpty(customOrderNumber)) {
            return null;
        }

        return await _orderRepository.Table
            .FirstOrDefaultAsync(o => o.CustomOrderNumber == customOrderNumber);
    }

    /// <summary>
    /// Gets an order by order item identifier
    /// </summary>
    /// <param name="orderItemId">The order item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order
    /// </returns>
    public virtual async Task<Order> GetOrderByOrderItemAsync(int orderItemId)
    {
		_logger.LogError(">>> Entering GetOrderByOrderItemAsync(orderItemId={OrderItemId})", orderItemId);
        if (orderItemId == 0)
            return null;

        return await (from o in _orderRepository.Table
            join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
            where oi.Id == orderItemId
            select o).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Get orders by identifiers
    /// </summary>
    /// <param name="orderIds">Order identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order
    /// </returns>
    public virtual async Task<IList<Order>> GetOrdersByIdsAsync(int[] orderIds)
    {
		_logger.LogError(">>> Entering GetOrdersByIdsAsync(orderIds={OrderIds})", orderIds);
        _logger.LogWarning("<<< Exiting GetOrdersByIdsAsync(orderIds={OrderIds})", orderIds);
        return await _orderRepository.GetByIdsAsync(orderIds, includeDeleted: false);
    }

    /// <summary>
    /// Get orders by guids
    /// </summary>
    /// <param name="orderGuids">Order guids</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the orders
    /// </returns>
    public virtual async Task<IList<Order>> GetOrdersByGuidsAsync(Guid[] orderGuids)
    {
		_logger.LogError(">>> Entering GetOrdersByGuidsAsync(orderGuids={OrderGuids})", orderGuids);
        if (orderGuids == null)
            return null;

        var query = from o in _orderRepository.Table
            where orderGuids.Contains(o.OrderGuid)
            select o;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var orders = await query.ToListAsync();

		_logger.LogError("GetOrdersByGuidsAsync(orderGuids={OrderGuids}): orders → {Orders}", orderGuids, orders);
		_logger.LogError("GetOrdersByGuidsAsync(orderGuids)={OrderGuids}: database query executed in {Arg1} ms", orderGuids, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetOrdersByGuidsAsync(orderGuids={OrderGuids})", orderGuids);
        return orders;
    }

    /// <summary>
    /// Gets an order
    /// </summary>
    /// <param name="orderGuid">The order identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order
    /// </returns>
    public virtual async Task<Order> GetOrderByGuidAsync(Guid orderGuid)
    {
		_logger.LogError(">>> Entering GetOrderByGuidAsync(orderGuid={OrderGuid})", orderGuid);
        if (orderGuid == Guid.Empty)
            return null;

        var query = from o in _orderRepository.Table
            where o.OrderGuid == orderGuid
            select o;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var order = await query.FirstOrDefaultAsync();

		_logger.LogError("GetOrderByGuidAsync(orderGuid={OrderGuid}): order → {Order}", orderGuid, order);
		_logger.LogError("GetOrderByGuidAsync(orderGuid)={OrderGuid}: database query executed in {Arg1} ms", orderGuid, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetOrderByGuidAsync(orderGuid={OrderGuid})", orderGuid);
        return order;
    }

    /// <summary>
    /// Deletes an order
    /// </summary>
    /// <param name="order">The order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteOrderAsync(Order order)
    {
		_logger.LogError(">>> Entering DeleteOrderAsync(order={Order})", order);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _orderRepository.DeleteAsync(order);
		_logger.LogError("DeleteOrderAsync(order)={Order}: external service call took {Arg1} ms", order, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteOrderAsync(order={Order})", order);
    }

    /// <summary>
    /// Search orders
    /// </summary>
    /// <param name="storeId">Store identifier; 0 to load all orders</param>
    /// <param name="vendorId">Vendor identifier; null to load all orders</param>
    /// <param name="customerId">Customer identifier; 0 to load all orders</param>
    /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
    /// <param name="affiliateId">Affiliate identifier; 0 to load all orders</param>
    /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
    /// <param name="warehouseId">Warehouse identifier, only orders with products from a specified warehouse will be loaded; 0 to load all orders</param>
    /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="osIds">Order status identifiers; null to load all orders</param>
    /// <param name="psIds">Payment status identifiers; null to load all orders</param>
    /// <param name="ssIds">Shipping status identifiers; null to load all orders</param>
    /// <param name="billingPhone">Billing phone. Leave empty to load all records.</param>
    /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
    /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
    /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the orders
    /// </returns>
    public virtual async Task<IPagedList<Order>> SearchOrdersAsync(int storeId = 0,
        int vendorId = 0, int customerId = 0,
        int productId = 0, int affiliateId = 0, int warehouseId = 0,
        int billingCountryId = 0, string paymentMethodSystemName = null,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
        string billingPhone = null, string billingEmail = null, string billingLastName = "",
        string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
		_logger.LogError(">>> Entering SearchOrdersAsync(storeId={StoreId},vendorId={VendorId},customerId={CustomerId},productId={ProductId},affiliateId={AffiliateId},warehouseId={WarehouseId},billingCountryId={BillingCountryId},paymentMethodSystemName={PaymentMethodSystemName},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},osIds={OsIds},psIds={PsIds},ssIds={SsIds},billingPhone={BillingPhone},billingEmail={BillingEmail},billingLastName={BillingLastName},orderNotes={OrderNotes},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", storeId, vendorId, customerId, productId, affiliateId, warehouseId, billingCountryId, paymentMethodSystemName, createdFromUtc, createdToUtc, osIds, psIds, ssIds, billingPhone, billingEmail, billingLastName, orderNotes, pageIndex, pageSize, getOnlyTotalCount);
        var query = _orderRepository.Table;

		_logger.LogError("SearchOrdersAsync(storeId={StoreId},vendorId={VendorId},customerId={CustomerId},productId={ProductId},affiliateId={AffiliateId},warehouseId={WarehouseId},billingCountryId={BillingCountryId},paymentMethodSystemName={PaymentMethodSystemName},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},osIds={OsIds},psIds={PsIds},ssIds={SsIds},billingPhone={BillingPhone},billingEmail={BillingEmail},billingLastName={BillingLastName},orderNotes={OrderNotes},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount}): query → {Query}", storeId, vendorId, customerId, productId, affiliateId, warehouseId, billingCountryId, paymentMethodSystemName, createdFromUtc, createdToUtc, osIds, psIds, ssIds, billingPhone, billingEmail, billingLastName, orderNotes, pageIndex, pageSize, getOnlyTotalCount, query);
        if (storeId > 0)
            query = query.Where(o => o.StoreId == storeId);

        if (vendorId > 0)
        {
            query = from o in query
                join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                join p in _productRepository.Table on oi.ProductId equals p.Id
                where p.VendorId == vendorId
                select o;

            query = query.Distinct();
        }

        if (customerId > 0)
            query = query.Where(o => o.CustomerId == customerId);

        if (productId > 0)
        {
            query = from o in query
                join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                where oi.ProductId == productId
                select o;

            query = query.Distinct();
        }

        if (warehouseId > 0)
        {
            var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
			_logger.LogError("SearchOrdersAsync(storeId={StoreId},vendorId={VendorId},customerId={CustomerId},productId={ProductId},affiliateId={AffiliateId},warehouseId={WarehouseId},billingCountryId={BillingCountryId},paymentMethodSystemName={PaymentMethodSystemName},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},osIds={OsIds},psIds={PsIds},ssIds={SsIds},billingPhone={BillingPhone},billingEmail={BillingEmail},billingLastName={BillingLastName},orderNotes={OrderNotes},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount}): manageStockInventoryMethodId → {ManageStockInventoryMethodId}", storeId, vendorId, customerId, productId, affiliateId, warehouseId, billingCountryId, paymentMethodSystemName, createdFromUtc, createdToUtc, osIds, psIds, ssIds, billingPhone, billingEmail, billingLastName, orderNotes, pageIndex, pageSize, getOnlyTotalCount, manageStockInventoryMethodId);

            query = from o in query
                join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                join p in _productRepository.Table on oi.ProductId equals p.Id
                join pwi in _productWarehouseInventoryRepository.Table on p.Id equals pwi.ProductId into ps
                from pwi in ps.DefaultIfEmpty()
                where
                    //"Use multiple warehouses" enabled
                    //we search in each warehouse
                    (p.ManageInventoryMethodId == manageStockInventoryMethodId && p.UseMultipleWarehouses && pwi.WarehouseId == warehouseId) ||
                    //"Use multiple warehouses" disabled
                    //we use standard "warehouse" property
                    ((p.ManageInventoryMethodId != manageStockInventoryMethodId || !p.UseMultipleWarehouses) && p.WarehouseId == warehouseId)
                select o;

            query = query.Distinct();
        }

        if (!string.IsNullOrEmpty(paymentMethodSystemName))
            query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

        if (affiliateId > 0)
            query = query.Where(o => o.AffiliateId == affiliateId);

        if (createdFromUtc.HasValue)
            query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);

        if (createdToUtc.HasValue)
            query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);

        if (osIds != null && osIds.Any())
            query = query.Where(o => osIds.Contains(o.OrderStatusId));

        if (psIds != null && psIds.Any())
            query = query.Where(o => psIds.Contains(o.PaymentStatusId));

        if (ssIds != null && ssIds.Any())
            query = query.Where(o => ssIds.Contains(o.ShippingStatusId));

        if (!string.IsNullOrEmpty(orderNotes))
            query = query.Where(o => _orderNoteRepository.Table.Any(oNote => oNote.OrderId == o.Id && oNote.Note.Contains(orderNotes)));

        query = from o in query
            join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
            where
                (billingCountryId <= 0 || (oba.CountryId == billingCountryId)) &&
                (string.IsNullOrEmpty(billingPhone) || (!string.IsNullOrEmpty(oba.PhoneNumber) && oba.PhoneNumber.Contains(billingPhone))) &&
                (string.IsNullOrEmpty(billingEmail) || (!string.IsNullOrEmpty(oba.Email) && oba.Email.Contains(billingEmail))) &&
                (string.IsNullOrEmpty(billingLastName) || (!string.IsNullOrEmpty(oba.LastName) && oba.LastName.Contains(billingLastName)))
            select o;

        query = query.Where(o => !o.Deleted);
        query = query.OrderByDescending(o => o.CreatedOnUtc);

        //database layer paging
		_logger.LogWarning("<<< Exiting SearchOrdersAsync(storeId={StoreId},vendorId={VendorId},customerId={CustomerId},productId={ProductId},affiliateId={AffiliateId},warehouseId={WarehouseId},billingCountryId={BillingCountryId},paymentMethodSystemName={PaymentMethodSystemName},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},osIds={OsIds},psIds={PsIds},ssIds={SsIds},billingPhone={BillingPhone},billingEmail={BillingEmail},billingLastName={BillingLastName},orderNotes={OrderNotes},pageIndex={PageIndex},pageSize={PageSize},getOnlyTotalCount={GetOnlyTotalCount})", storeId, vendorId, customerId, productId, affiliateId, warehouseId, billingCountryId, paymentMethodSystemName, createdFromUtc, createdToUtc, osIds, psIds, ssIds, billingPhone, billingEmail, billingLastName, orderNotes, pageIndex, pageSize, getOnlyTotalCount);
        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Inserts an order
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertOrderAsync(Order order)
    {
		_logger.LogError(">>> Entering InsertOrderAsync(order={Order})", order);
        await _orderRepository.InsertAsync(order);
		_logger.LogWarning("<<< Exiting InsertOrderAsync(order={Order})", order);
    }

    /// <summary>
    /// Updates the order
    /// </summary>
    /// <param name="order">The order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateOrderAsync(Order order)
    {
		_logger.LogError(">>> Entering UpdateOrderAsync(order={Order})", order);
        await _orderRepository.UpdateAsync(order);
		_logger.LogWarning("<<< Exiting UpdateOrderAsync(order={Order})", order);
    }

    /// <summary>
    /// Parse tax rates
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="taxRatesStr"></param>
    /// <returns>Rates</returns>
    public virtual SortedDictionary<decimal, decimal> ParseTaxRates(Order order, string taxRatesStr)
    {
        var taxRatesDictionary = new SortedDictionary<decimal, decimal>();

        if (string.IsNullOrEmpty(taxRatesStr))
            return taxRatesDictionary;

        var lines = taxRatesStr.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line.Trim()))
                continue;

            var taxes = line.Split(':');
            if (taxes.Length != 2)
                continue;

            try
            {
                var taxRate = decimal.Parse(taxes[0].Trim(), CultureInfo.InvariantCulture);
                var taxValue = decimal.Parse(taxes[1].Trim(), CultureInfo.InvariantCulture);
                taxRatesDictionary.Add(taxRate, taxValue);
            }
            catch
            {
                // ignored
            }
        }

        //add at least one tax rate (0%)
        if (!taxRatesDictionary.Any())
            taxRatesDictionary.Add(decimal.Zero, decimal.Zero);

        return taxRatesDictionary;
    }

    /// <summary>
    /// Gets a value indicating whether an order has items to be added to a shipment
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether an order has items to be added to a shipment
    /// </returns>
    public virtual async Task<bool> HasItemsToAddToShipmentAsync(Order order)
    {
		_logger.LogError(">>> Entering HasItemsToAddToShipmentAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);

        foreach (var orderItem in await GetOrderItemsAsync(order.Id, isShipEnabled: true)) //we can ship only shippable products
        {
            var totalNumberOfItemsCanBeAddedToShipment = await GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
			_logger.LogError("HasItemsToAddToShipmentAsync(order={Order}): totalNumberOfItemsCanBeAddedToShipment → {TotalNumberOfItemsCanBeAddedToShipment}", order, totalNumberOfItemsCanBeAddedToShipment);
            if (totalNumberOfItemsCanBeAddedToShipment <= 0)
                continue;

            //yes, we have at least one item to create a new shipment
            return true;
        }

		_logger.LogWarning("<<< Exiting HasItemsToAddToShipmentAsync(order={Order})", order);
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether an order has items to ship
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether an order has items to ship
    /// </returns>
    public virtual async Task<bool> HasItemsToShipAsync(Order order)
    {
		_logger.LogError(">>> Entering HasItemsToShipAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);

		_logger.LogWarning("<<< Exiting HasItemsToShipAsync(order={Order})", order);
        if (order.PickupInStore) {
            return false;
        }

        return await HasShipmentItemsAsync(order, shipment => !shipment.ShippedDateUtc.HasValue);
    }

    /// <summary>
    /// Gets a value indicating whether there are shipment items to mark as 'ready for pickup' in order shipments.
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether there are shipment items to mark as 'ready for pickup' in order shipments.
    /// </returns>
    public virtual async Task<bool> HasItemsToReadyForPickupAsync(Order order)
    {
		_logger.LogError(">>> Entering HasItemsToReadyForPickupAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);

		_logger.LogWarning("<<< Exiting HasItemsToReadyForPickupAsync(order={Order})", order);
        if (!order.PickupInStore) {
            return false;
        }

        return await HasShipmentItemsAsync(order, shipment => !shipment.ReadyForPickupDateUtc.HasValue);
    }

    /// <summary>
    /// Gets a value indicating whether an order has items to deliver
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether an order has items to deliver
    /// </returns>
    public virtual async Task<bool> HasItemsToDeliverAsync(Order order)
    {
		_logger.LogError(">>> Entering HasItemsToDeliverAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);

		_logger.LogWarning("<<< Exiting HasItemsToDeliverAsync(order={Order})", order);
        return await HasShipmentItemsAsync(order, shipment => (shipment.ShippedDateUtc.HasValue || shipment.ReadyForPickupDateUtc.HasValue) && !shipment.DeliveryDateUtc.HasValue);
    }

    #endregion

    #region Orders items

    /// <summary>
    /// Gets an order item
    /// </summary>
    /// <param name="orderItemId">Order item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order item
    /// </returns>
    public virtual async Task<OrderItem> GetOrderItemByIdAsync(int orderItemId)
    {
		_logger.LogError(">>> Entering GetOrderItemByIdAsync(orderItemId={OrderItemId})", orderItemId);
		_logger.LogError("GetOrderItemByIdAsync(orderItemId)={OrderItemId}: find query executing", orderItemId);
    _logger.LogWarning("<<< Exiting GetOrderItemByIdAsync(orderItemId={OrderItemId})", orderItemId);
        return await _orderItemRepository.GetByIdAsync(orderItemId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets a product of specify order item
    /// </summary>
    /// <param name="orderItemId">Order item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product
    /// </returns>
    public virtual async Task<Product> GetProductByOrderItemIdAsync(int orderItemId)
    {
		_logger.LogError(">>> Entering GetProductByOrderItemIdAsync(orderItemId={OrderItemId})", orderItemId);
        if (orderItemId == 0)
            return null;

        return await (from p in _productRepository.Table
            join oi in _orderItemRepository.Table on p.Id equals oi.ProductId
            where oi.Id == orderItemId
            select p).SingleOrDefaultAsync();
    }

    /// <summary>
    /// Gets a list items of order
    /// </summary>
    /// <param name="orderId">Order identifier</param>
    /// <param name="isNotReturnable">Value indicating whether this product is returnable; pass null to ignore</param>
    /// <param name="isShipEnabled">Value indicating whether the entity is ship enabled; pass null to ignore</param>
    /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<OrderItem>> GetOrderItemsAsync(int orderId, bool? isNotReturnable = null, bool? isShipEnabled = null, int vendorId = 0)
    {
        if (orderId == 0)
            return new List<OrderItem>();

        return await (from oi in _orderItemRepository.Table
            join p in _productRepository.Table on oi.ProductId equals p.Id
            where
                oi.OrderId == orderId &&
                (!isShipEnabled.HasValue || (p.IsShipEnabled == isShipEnabled.Value)) &&
                (!isNotReturnable.HasValue || (p.NotReturnable == isNotReturnable)) &&
                (vendorId <= 0 || (p.VendorId == vendorId))
            select oi).ToListAsync();
    }

    /// <summary>
    /// Gets an item
    /// </summary>
    /// <param name="orderItemGuid">Order identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order item
    /// </returns>
    public virtual async Task<OrderItem> GetOrderItemByGuidAsync(Guid orderItemGuid)
    {
		_logger.LogError(">>> Entering GetOrderItemByGuidAsync(orderItemGuid={OrderItemGuid})", orderItemGuid);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (orderItemGuid == Guid.Empty)
            return null;

        var query = from orderItem in _orderItemRepository.Table
            where orderItem.OrderItemGuid == orderItemGuid
            select orderItem;
			_logger.LogError("GetOrderItemByGuidAsync(orderItemGuid)={OrderItemGuid}: database query executed in {Arg1} ms", orderItemGuid, __logStart.ElapsedMilliseconds);
        var item = await query.FirstOrDefaultAsync();
		_logger.LogError("GetOrderItemByGuidAsync(orderItemGuid={OrderItemGuid}): item → {Item}", orderItemGuid, item);
		_logger.LogWarning("<<< Exiting GetOrderItemByGuidAsync(orderItemGuid={OrderItemGuid})", orderItemGuid);
        return item;
    }

    /// <summary>
    /// Gets all downloadable order items
    /// </summary>
    /// <param name="customerId">Customer identifier; null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order items
    /// </returns>
    public virtual async Task<IList<OrderItem>> GetDownloadableOrderItemsAsync(int customerId)
    {
		_logger.LogError(">>> Entering GetDownloadableOrderItemsAsync(customerId={CustomerId})", customerId);
			_logger.LogError("Throwing ArgumentOutOfRangeException from GetDownloadableOrderItemsAsync(customerId={CustomerId})", customerId);
        if (customerId == 0) {
            throw new ArgumentOutOfRangeException(nameof(customerId));
        }

        var query = from orderItem in _orderItemRepository.Table
            join o in _orderRepository.Table on orderItem.OrderId equals o.Id
            join p in _productRepository.Table on orderItem.ProductId equals p.Id
            where customerId == o.CustomerId &&
                  p.IsDownload &&
                  !o.Deleted
            orderby o.CreatedOnUtc descending, orderItem.Id
            select orderItem;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("GetDownloadableOrderItemsAsync(customerId)={CustomerId}: database query executed in {Arg1} ms", customerId, __logStart.ElapsedMilliseconds);
        var orderItems = await query.ToListAsync();
		_logger.LogError("GetDownloadableOrderItemsAsync(customerId={CustomerId}): orderItems → {OrderItems}", customerId, orderItems);
		_logger.LogWarning("<<< Exiting GetDownloadableOrderItemsAsync(customerId={CustomerId})", customerId);
        return orderItems;
    }

    /// <summary>
    /// Delete an order item
    /// </summary>
    /// <param name="orderItem">The order item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteOrderItemAsync(OrderItem orderItem)
    {
		_logger.LogError(">>> Entering DeleteOrderItemAsync(orderItem={OrderItem})", orderItem);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _orderItemRepository.DeleteAsync(orderItem);
		_logger.LogError("DeleteOrderItemAsync(orderItem)={OrderItem}: external service call took {Arg1} ms", orderItem, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteOrderItemAsync(orderItem={OrderItem})", orderItem);
    }

    /// <summary>
    /// Gets a total number of items in all shipments
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the total number of items in all shipments
    /// </returns>
    public virtual async Task<int> GetTotalNumberOfItemsInAllShipmentsAsync(OrderItem orderItem)
    {
		_logger.LogError(">>> Entering GetTotalNumberOfItemsInAllShipmentsAsync(orderItem={OrderItem})", orderItem);
        ArgumentNullException.ThrowIfNull(orderItem);

        var totalInShipments = 0;
        var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(orderItem.OrderId);
		_logger.LogError("GetTotalNumberOfItemsInAllShipmentsAsync(orderItem={OrderItem}): shipments → {Shipments}", orderItem, shipments);

        for (var i = 0; i < shipments.Count; i++)
        {
            var shipment = shipments[i];
            var si = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
                .FirstOrDefault(x => x.OrderItemId == orderItem.Id);
            if (si != null)
            {
                totalInShipments += si.Quantity;
            }
        }

		_logger.LogWarning("<<< Exiting GetTotalNumberOfItemsInAllShipmentsAsync(orderItem={OrderItem})", orderItem);
        return totalInShipments;
    }

    /// <summary>
    /// Gets a total number of already items which can be added to new shipments
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the total number of already delivered items which can be added to new shipments
    /// </returns>
    public virtual async Task<int> GetTotalNumberOfItemsCanBeAddedToShipmentAsync(OrderItem orderItem)
    {
		_logger.LogError(">>> Entering GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem={OrderItem})", orderItem);
        ArgumentNullException.ThrowIfNull(orderItem);

        var totalInShipments = await GetTotalNumberOfItemsInAllShipmentsAsync(orderItem);
		_logger.LogError("GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem={OrderItem}): totalInShipments → {TotalInShipments}", orderItem, totalInShipments);

        var qtyOrdered = orderItem.Quantity;
        var qtyCanBeAddedToShipmentTotal = qtyOrdered - totalInShipments;
		_logger.LogError("GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem={OrderItem}): qtyCanBeAddedToShipmentTotal → {QtyCanBeAddedToShipmentTotal}", orderItem, qtyCanBeAddedToShipmentTotal);
        if (qtyCanBeAddedToShipmentTotal < 0)
            qtyCanBeAddedToShipmentTotal = 0;

		_logger.LogWarning("<<< Exiting GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem={OrderItem})", orderItem);
        return qtyCanBeAddedToShipmentTotal;
    }

    /// <summary>
    /// Gets a value indicating whether download is allowed
    /// </summary>
    /// <param name="orderItem">Order item to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true if download is allowed; otherwise, false.
    /// </returns>
    public virtual async Task<bool> IsDownloadAllowedAsync(OrderItem orderItem)
    {
		_logger.LogError(">>> Entering IsDownloadAllowedAsync(orderItem={OrderItem})", orderItem);
        if (orderItem is null)
            return false;

        var order = await GetOrderByIdAsync(orderItem.OrderId);
		_logger.LogError("IsDownloadAllowedAsync(orderItem={OrderItem}): order → {Order}", orderItem, order);
        if (order == null || order.Deleted)
            return false;

        //order status
        if (order.OrderStatus == OrderStatus.Cancelled)
            return false;

        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
		_logger.LogError("IsDownloadAllowedAsync(orderItem={OrderItem}): product → {Product}", orderItem, product);

        if (product == null || !product.IsDownload)
            return false;

        //payment status
        switch (product.DownloadActivationType)
        {
            case DownloadActivationType.WhenOrderIsPaid:
                if (order.PaymentStatus == PaymentStatus.Paid && order.PaidDateUtc.HasValue)
                {
                    //expiration date
                    if (product.DownloadExpirationDays.HasValue)
                    {
                        if (order.PaidDateUtc.Value.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                break;
            case DownloadActivationType.Manually:
                if (orderItem.IsDownloadActivated)
                {
                    //expiration date
                    if (product.DownloadExpirationDays.HasValue)
                    {
                        if (order.CreatedOnUtc.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                break;
            default:
                break;
        }

		_logger.LogWarning("<<< Exiting IsDownloadAllowedAsync(orderItem={OrderItem})", orderItem);
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether license download is allowed
    /// </summary>
    /// <param name="orderItem">Order item to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true if license download is allowed; otherwise, false.
    /// </returns>
    public virtual async Task<bool> IsLicenseDownloadAllowedAsync(OrderItem orderItem)
    {
		_logger.LogError(">>> Entering IsLicenseDownloadAllowedAsync(orderItem={OrderItem})", orderItem);
		_logger.LogWarning("<<< Exiting IsLicenseDownloadAllowedAsync(orderItem={OrderItem})", orderItem);
        if (orderItem == null) {
            return false;
        }

        return await IsDownloadAllowedAsync(orderItem) &&
               orderItem.LicenseDownloadId.HasValue &&
               orderItem.LicenseDownloadId > 0;
    }

    /// <summary>
    /// Inserts a order item
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertOrderItemAsync(OrderItem orderItem)
    {
		_logger.LogError(">>> Entering InsertOrderItemAsync(orderItem={OrderItem})", orderItem);
        await _orderItemRepository.InsertAsync(orderItem);
		_logger.LogWarning("<<< Exiting InsertOrderItemAsync(orderItem={OrderItem})", orderItem);
    }

    /// <summary>
    /// Updates a order item
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateOrderItemAsync(OrderItem orderItem)
    {
		_logger.LogError(">>> Entering UpdateOrderItemAsync(orderItem={OrderItem})", orderItem);
        await _orderItemRepository.UpdateAsync(orderItem);
		_logger.LogWarning("<<< Exiting UpdateOrderItemAsync(orderItem={OrderItem})", orderItem);
    }

    #endregion

    #region Orders notes

    /// <summary>
    /// Gets an order note
    /// </summary>
    /// <param name="orderNoteId">The order note identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order note
    /// </returns>
    public virtual async Task<OrderNote> GetOrderNoteByIdAsync(int orderNoteId)
    {
		_logger.LogError(">>> Entering GetOrderNoteByIdAsync(orderNoteId={OrderNoteId})", orderNoteId);
        _logger.LogWarning("<<< Exiting GetOrderNoteByIdAsync(orderNoteId={OrderNoteId})", orderNoteId);
        return await _orderNoteRepository.GetByIdAsync(orderNoteId);
    }

    /// <summary>
    /// Gets a list notes of order
    /// </summary>
    /// <param name="orderId">Order identifier</param>
    /// <param name="displayToCustomer">Value indicating whether a customer can see a note; pass null to ignore</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<OrderNote>> GetOrderNotesByOrderIdAsync(int orderId, bool? displayToCustomer = null)
    {
		_logger.LogError(">>> Entering GetOrderNotesByOrderIdAsync(orderId={OrderId},displayToCustomer={DisplayToCustomer})", orderId, displayToCustomer);
        if (orderId == 0)
            return new List<OrderNote>();

        var query = _orderNoteRepository.Table.Where(on => on.OrderId == orderId);
		_logger.LogError("GetOrderNotesByOrderIdAsync(orderId={OrderId},displayToCustomer={DisplayToCustomer}): query → {Query}", orderId, displayToCustomer, query);

        if (displayToCustomer.HasValue)
        {
            query = query.Where(on => on.DisplayToCustomer == displayToCustomer);
        }

		_logger.LogWarning("<<< Exiting GetOrderNotesByOrderIdAsync(orderId={OrderId},displayToCustomer={DisplayToCustomer})", orderId, displayToCustomer);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Deletes an order note
    /// </summary>
    /// <param name="orderNote">The order note</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteOrderNoteAsync(OrderNote orderNote)
    {
		_logger.LogError(">>> Entering DeleteOrderNoteAsync(orderNote={OrderNote})", orderNote);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _orderNoteRepository.DeleteAsync(orderNote);
		_logger.LogError("DeleteOrderNoteAsync(orderNote)={OrderNote}: external service call took {Arg1} ms", orderNote, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteOrderNoteAsync(orderNote={OrderNote})", orderNote);
    }

    /// <summary>
    /// Formats the order note text
    /// </summary>
    /// <param name="orderNote">Order note</param>
    /// <returns>Formatted text</returns>
    public virtual string FormatOrderNoteText(OrderNote orderNote)
    {
		_logger.LogError(">>> Entering FormatOrderNoteText(orderNote={OrderNote})", orderNote);
        ArgumentNullException.ThrowIfNull(orderNote);

        var text = orderNote.Note;
		_logger.LogError("FormatOrderNoteText(orderNote={OrderNote}): text → {Text}", orderNote, text);

        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);

		_logger.LogWarning("<<< Exiting FormatOrderNoteText(orderNote={OrderNote})", orderNote);
        return text;
    }

    /// <summary>
    /// Inserts an order note
    /// </summary>
    /// <param name="orderNote">The order note</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertOrderNoteAsync(OrderNote orderNote)
    {
		_logger.LogError(">>> Entering InsertOrderNoteAsync(orderNote={OrderNote})", orderNote);
        await _orderNoteRepository.InsertAsync(orderNote);
		_logger.LogWarning("<<< Exiting InsertOrderNoteAsync(orderNote={OrderNote})", orderNote);
    }

    #endregion

    #region Recurring payments

    /// <summary>
    /// Deletes a recurring payment
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteRecurringPaymentAsync(RecurringPayment recurringPayment)
    {
		_logger.LogError(">>> Entering DeleteRecurringPaymentAsync(recurringPayment={RecurringPayment})", recurringPayment);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _recurringPaymentRepository.DeleteAsync(recurringPayment);
		_logger.LogError("DeleteRecurringPaymentAsync(recurringPayment)={RecurringPayment}: external service call took {Arg1} ms", recurringPayment, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteRecurringPaymentAsync(recurringPayment={RecurringPayment})", recurringPayment);
    }

    /// <summary>
    /// Gets a recurring payment
    /// </summary>
    /// <param name="recurringPaymentId">The recurring payment identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the recurring payment
    /// </returns>
    public virtual async Task<RecurringPayment> GetRecurringPaymentByIdAsync(int recurringPaymentId)
    {
		_logger.LogError(">>> Entering GetRecurringPaymentByIdAsync(recurringPaymentId={RecurringPaymentId})", recurringPaymentId);
		_logger.LogError("GetRecurringPaymentByIdAsync(recurringPaymentId)={RecurringPaymentId}: find query executing", recurringPaymentId);
    _logger.LogWarning("<<< Exiting GetRecurringPaymentByIdAsync(recurringPaymentId={RecurringPaymentId})", recurringPaymentId);
        return await _recurringPaymentRepository.GetByIdAsync(recurringPaymentId, cache => default);
    }

    /// <summary>
    /// Inserts a recurring payment
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertRecurringPaymentAsync(RecurringPayment recurringPayment)
    {
		_logger.LogError(">>> Entering InsertRecurringPaymentAsync(recurringPayment={RecurringPayment})", recurringPayment);
        await _recurringPaymentRepository.InsertAsync(recurringPayment);
		_logger.LogWarning("<<< Exiting InsertRecurringPaymentAsync(recurringPayment={RecurringPayment})", recurringPayment);
    }

    /// <summary>
    /// Updates the recurring payment
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateRecurringPaymentAsync(RecurringPayment recurringPayment)
    {
		_logger.LogError(">>> Entering UpdateRecurringPaymentAsync(recurringPayment={RecurringPayment})", recurringPayment);
        await _recurringPaymentRepository.UpdateAsync(recurringPayment);
		_logger.LogWarning("<<< Exiting UpdateRecurringPaymentAsync(recurringPayment={RecurringPayment})", recurringPayment);
    }

    /// <summary>
    /// Search recurring payments
    /// </summary>
    /// <param name="storeId">The store identifier; 0 to load all records</param>
    /// <param name="customerId">The customer identifier; 0 to load all records</param>
    /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
    /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the recurring payments
    /// </returns>
    public virtual async Task<IPagedList<RecurringPayment>> SearchRecurringPaymentsAsync(int storeId = 0,
        int customerId = 0, int initialOrderId = 0, OrderStatus? initialOrderStatus = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
    {
		_logger.LogError(">>> Entering SearchRecurringPaymentsAsync(storeId={StoreId},customerId={CustomerId},initialOrderId={InitialOrderId},initialOrderStatus={InitialOrderStatus},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", storeId, customerId, initialOrderId, initialOrderStatus, pageIndex, pageSize, showHidden);
        int? initialOrderStatusId = null;
        if (initialOrderStatus.HasValue)
            initialOrderStatusId = (int)initialOrderStatus.Value;
			_logger.LogError("SearchRecurringPaymentsAsync(storeId={StoreId},customerId={CustomerId},initialOrderId={InitialOrderId},initialOrderStatus={InitialOrderStatus},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): initialOrderStatusId → {InitialOrderStatusId}", storeId, customerId, initialOrderId, initialOrderStatus, pageIndex, pageSize, showHidden, initialOrderStatusId);
			_logger.LogDebug("SearchRecurringPaymentsAsync(storeId={StoreId},customerId={CustomerId},initialOrderId={InitialOrderId},initialOrderStatus={InitialOrderStatus},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): initialOrderStatusId → {InitialOrderStatusId}", storeId, customerId, initialOrderId, initialOrderStatus, pageIndex, pageSize, showHidden, initialOrderStatusId);

        var query1 = from rp in _recurringPaymentRepository.Table
            join o in _orderRepository.Table on rp.InitialOrderId equals o.Id
            join c in _customerRepository.Table on o.CustomerId equals c.Id
            where
                !rp.Deleted &&
                (showHidden || !o.Deleted) &&
                (showHidden || !c.Deleted) &&
                (showHidden || rp.IsActive) &&
                (customerId == 0 || o.CustomerId == customerId) &&
                (storeId == 0 || o.StoreId == storeId) &&
                (initialOrderId == 0 || o.Id == initialOrderId) &&
                (!initialOrderStatusId.HasValue || initialOrderStatusId.Value == 0 ||
                 o.OrderStatusId == initialOrderStatusId.Value)
            select rp.Id;

        var query2 = from rp in _recurringPaymentRepository.Table
            where query1.Contains(rp.Id)
            orderby rp.StartDateUtc, rp.Id
            select rp;

        var recurringPayments = await query2.ToPagedListAsync(pageIndex, pageSize);
		_logger.LogError("SearchRecurringPaymentsAsync(storeId={StoreId},customerId={CustomerId},initialOrderId={InitialOrderId},initialOrderStatus={InitialOrderStatus},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden}): recurringPayments → {RecurringPayments}", storeId, customerId, initialOrderId, initialOrderStatus, pageIndex, pageSize, showHidden, recurringPayments);

		_logger.LogWarning("<<< Exiting SearchRecurringPaymentsAsync(storeId={StoreId},customerId={CustomerId},initialOrderId={InitialOrderId},initialOrderStatus={InitialOrderStatus},pageIndex={PageIndex},pageSize={PageSize},showHidden={ShowHidden})", storeId, customerId, initialOrderId, initialOrderStatus, pageIndex, pageSize, showHidden);
        return recurringPayments;
    }

    #endregion

    #region Recurring payments history

    /// <summary>
    /// Gets a recurring payment history
    /// </summary>
    /// <param name="recurringPayment">The recurring payment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<RecurringPaymentHistory>> GetRecurringPaymentHistoryAsync(RecurringPayment recurringPayment)
    {
		_logger.LogError(">>> Entering GetRecurringPaymentHistoryAsync(recurringPayment={RecurringPayment})", recurringPayment);
        ArgumentNullException.ThrowIfNull(recurringPayment);

        return await _recurringPaymentHistoryRepository.Table
            .Where(rph => rph.RecurringPaymentId == recurringPayment.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Inserts a recurring payment history entry
    /// </summary>
    /// <param name="recurringPaymentHistory">Recurring payment history entry</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertRecurringPaymentHistoryAsync(RecurringPaymentHistory recurringPaymentHistory)
    {
		_logger.LogError(">>> Entering InsertRecurringPaymentHistoryAsync(recurringPaymentHistory={RecurringPaymentHistory})", recurringPaymentHistory);
        await _recurringPaymentHistoryRepository.InsertAsync(recurringPaymentHistory);
		_logger.LogWarning("<<< Exiting InsertRecurringPaymentHistoryAsync(recurringPaymentHistory={RecurringPaymentHistory})", recurringPaymentHistory);
    }

    #endregion

    #endregion
}