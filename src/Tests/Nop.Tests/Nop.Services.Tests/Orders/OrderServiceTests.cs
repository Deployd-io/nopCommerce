using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Orders;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Orders;

[TestFixture]
public class OrderServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IOrderService _orderService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _orderService = GetService<IOrderService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _orderService → {_orderService}", _orderService);
    }

    [Test]
    public async Task ItShouldReturnNullOrderWhenOrderIdIsZero()
    {
		_logger.LogDebug(">>> Entering ItShouldReturnNullOrderWhenOrderIdIsZero()");
        var order = await _orderService.GetOrderByIdAsync(0);
        order.Should().BeNull();
		_logger.LogDebug("<<< Exiting ItShouldReturnNullOrderWhenOrderIdIsZero()");
		_logger.LogDebug("ItShouldReturnNullOrderWhenOrderIdIsZero(): order → {Order}", order);
    }

    [Test]
    public async Task ItShouldReturnEmptyOrdersWhenOrderIdsNull()
    {
		_logger.LogDebug(">>> Entering ItShouldReturnEmptyOrdersWhenOrderIdsNull()");
        var orders = await _orderService.GetOrdersByIdsAsync(null);
        orders.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting ItShouldReturnEmptyOrdersWhenOrderIdsNull()");
		_logger.LogDebug("ItShouldReturnEmptyOrdersWhenOrderIdsNull(): orders → {Orders}", orders);
    }

    [Test]
    public async Task ItShouldReturnEmptyOrdersWhenOrderIdsEmpty()
    {
		_logger.LogDebug(">>> Entering ItShouldReturnEmptyOrdersWhenOrderIdsEmpty()");
        var orders = await _orderService.GetOrdersByIdsAsync(Array.Empty<int>());
        orders.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting ItShouldReturnEmptyOrdersWhenOrderIdsEmpty()");
		_logger.LogDebug("ItShouldReturnEmptyOrdersWhenOrderIdsEmpty(): orders → {Orders}", orders);
    }

    [Test]
    public async Task ItShouldReturnOrdersByIds()
    {
		_logger.LogDebug(">>> Entering ItShouldReturnOrdersByIds()");
        var orders = await _orderService.GetOrdersByIdsAsync([1, 2]);
        orders.Count.Should().Be(2);
		_logger.LogDebug("<<< Exiting ItShouldReturnOrdersByIds()");
		_logger.LogDebug("ItShouldReturnOrdersByIds(): orders → {Orders}", orders);
    }

    [Test]
    public async Task ItShouldReturnNullWhenGuidIsEmpty()
    {
		_logger.LogDebug(">>> Entering ItShouldReturnNullWhenGuidIsEmpty()");
        var order = await _orderService.GetOrderByGuidAsync(Guid.Empty);
        order.Should().BeNull();
		_logger.LogDebug("<<< Exiting ItShouldReturnNullWhenGuidIsEmpty()");
		_logger.LogDebug("ItShouldReturnNullWhenGuidIsEmpty(): order → {Order}", order);
    }

    [Test]
    public async Task ItShouldGetOrderByGuidWhenGuidNotEmpty()
    {
		_logger.LogDebug(">>> Entering ItShouldGetOrderByGuidWhenGuidNotEmpty()");
        var order = await _orderService.GetOrderByIdAsync(1);

		_logger.LogDebug("ItShouldGetOrderByGuidWhenGuidNotEmpty(): order → {Order}", order);
        var foundedOrder = await _orderService.GetOrderByGuidAsync(order.OrderGuid);
		_logger.LogDebug("ItShouldGetOrderByGuidWhenGuidNotEmpty(): foundedOrder → {FoundedOrder}", foundedOrder);
        foundedOrder.Id.Should().Be(order.Id);
		_logger.LogDebug("<<< Exiting ItShouldGetOrderByGuidWhenGuidNotEmpty()");
    }

    [Test]
    public void ItShouldThrowExceptionIfOrderIsNullWhenDeleteOrder()
    {
		_logger.LogDebug(">>> Entering ItShouldThrowExceptionIfOrderIsNullWhenDeleteOrder()");
        Assert.Throws(typeof(AggregateException), () => _orderService.DeleteOrderAsync(null).Wait());
		_logger.LogDebug("<<< Exiting ItShouldThrowExceptionIfOrderIsNullWhenDeleteOrder()");
    }

    [Test]
    public async Task ItShouldDeleteOrder()
    {
		_logger.LogDebug(">>> Entering ItShouldDeleteOrder()");
        var order = await _orderService.GetOrderByIdAsync(1);
        await _orderService.DeleteOrderAsync(order);
		_logger.LogDebug("ItShouldDeleteOrder(): order → {Order}", order);

        order.Deleted.Should().BeTrue();
        order = await GetService<IRepository<Order>>().GetByIdAsync(1);
        order.Should().NotBeNull();
        order.Deleted = false;
		_logger.LogDebug("ItShouldDeleteOrder(): order.Deleted → {Arg0}", order.Deleted);
        await _orderService.UpdateOrderAsync(order);
		_logger.LogDebug("<<< Exiting ItShouldDeleteOrder()");
    }

    [Test]
    public void ItShouldThrowIfOrderIsNullWhenInsertOrder()
    {
		_logger.LogDebug(">>> Entering ItShouldThrowIfOrderIsNullWhenInsertOrder()");
        Assert.Throws<AggregateException>(() => _orderService.InsertOrderAsync(null).Wait());
		_logger.LogDebug("<<< Exiting ItShouldThrowIfOrderIsNullWhenInsertOrder()");
    }

    [Test]
    public async Task ItShouldInsertOrder()
    {
		_logger.LogDebug(">>> Entering ItShouldInsertOrder()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var order = new Order
        {
            CustomOrderNumber = string.Empty,
            CustomerId = 1,
            BillingAddressId = 1
        };

        await _orderService.InsertOrderAsync(order);
        order.Id.Should().BeGreaterThan(0);
        await GetService<IRepository<Order>>().DeleteAsync(order);
		_logger.LogDebug("<<< Exiting ItShouldInsertOrder()");
		_logger.LogInformation("ItShouldInsertOrder(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public void ItShouldThrowIfOrderIsNullWhenUpdateOrder()
    {
		_logger.LogDebug(">>> Entering ItShouldThrowIfOrderIsNullWhenUpdateOrder()");
        Assert.Throws<AggregateException>(() => _orderService.UpdateOrderAsync(null).Wait());
		_logger.LogDebug("<<< Exiting ItShouldThrowIfOrderIsNullWhenUpdateOrder()");
    }
}