using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Orders;

[TestFixture]
public class OrderServiceTests : ServiceTest
{
    private IOrderService _orderService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _orderService = GetService<IOrderService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _orderService → {_orderService}", _orderService);
    }

    [Test]
    public async Task ItShouldReturnNullOrderWhenOrderIdIsZero()
    {
		logger.LogDebug(">>> Entering ItShouldReturnNullOrderWhenOrderIdIsZero()");
        var order = await _orderService.GetOrderByIdAsync(0);
        order.Should().BeNull();
		logger.LogDebug("<<< Exiting ItShouldReturnNullOrderWhenOrderIdIsZero()");
		logger.LogDebug("ItShouldReturnNullOrderWhenOrderIdIsZero(): order → {Order}", order);
    }

    [Test]
    public async Task ItShouldReturnEmptyOrdersWhenOrderIdsNull()
    {
		logger.LogDebug(">>> Entering ItShouldReturnEmptyOrdersWhenOrderIdsNull()");
        var orders = await _orderService.GetOrdersByIdsAsync(null);
        orders.Count.Should().Be(0);
		logger.LogDebug("<<< Exiting ItShouldReturnEmptyOrdersWhenOrderIdsNull()");
		logger.LogDebug("ItShouldReturnEmptyOrdersWhenOrderIdsNull(): orders → {Orders}", orders);
    }

    [Test]
    public async Task ItShouldReturnEmptyOrdersWhenOrderIdsEmpty()
    {
		logger.LogDebug(">>> Entering ItShouldReturnEmptyOrdersWhenOrderIdsEmpty()");
        var orders = await _orderService.GetOrdersByIdsAsync(Array.Empty<int>());
        orders.Count.Should().Be(0);
		logger.LogDebug("<<< Exiting ItShouldReturnEmptyOrdersWhenOrderIdsEmpty()");
		logger.LogDebug("ItShouldReturnEmptyOrdersWhenOrderIdsEmpty(): orders → {Orders}", orders);
    }

    [Test]
    public async Task ItShouldReturnOrdersByIds()
    {
		logger.LogDebug(">>> Entering ItShouldReturnOrdersByIds()");
        var orders = await _orderService.GetOrdersByIdsAsync([1, 2]);
        orders.Count.Should().Be(2);
		logger.LogDebug("<<< Exiting ItShouldReturnOrdersByIds()");
		logger.LogDebug("ItShouldReturnOrdersByIds(): orders → {Orders}", orders);
    }

    [Test]
    public async Task ItShouldReturnNullWhenGuidIsEmpty()
    {
		logger.LogDebug(">>> Entering ItShouldReturnNullWhenGuidIsEmpty()");
        var order = await _orderService.GetOrderByGuidAsync(Guid.Empty);
        order.Should().BeNull();
		logger.LogDebug("<<< Exiting ItShouldReturnNullWhenGuidIsEmpty()");
		logger.LogDebug("ItShouldReturnNullWhenGuidIsEmpty(): order → {Order}", order);
    }

    [Test]
    public async Task ItShouldGetOrderByGuidWhenGuidNotEmpty()
    {
		logger.LogDebug(">>> Entering ItShouldGetOrderByGuidWhenGuidNotEmpty()");
        var order = await _orderService.GetOrderByIdAsync(1);

		logger.LogDebug("ItShouldGetOrderByGuidWhenGuidNotEmpty(): order → {Order}", order);
        var foundedOrder = await _orderService.GetOrderByGuidAsync(order.OrderGuid);
        foundedOrder.Id.Should().Be(order.Id);
		logger.LogDebug("<<< Exiting ItShouldGetOrderByGuidWhenGuidNotEmpty()");
		logger.LogDebug("ItShouldGetOrderByGuidWhenGuidNotEmpty(): foundedOrder → {FoundedOrder}", foundedOrder);
    }

    [Test]
    public void ItShouldThrowExceptionIfOrderIsNullWhenDeleteOrder()
    {
		logger.LogDebug(">>> Entering ItShouldThrowExceptionIfOrderIsNullWhenDeleteOrder()");
        Assert.Throws(typeof(AggregateException), () => _orderService.DeleteOrderAsync(null).Wait());
		logger.LogDebug("<<< Exiting ItShouldThrowExceptionIfOrderIsNullWhenDeleteOrder()");
    }

    [Test]
    public async Task ItShouldDeleteOrder()
    {
		logger.LogDebug(">>> Entering ItShouldDeleteOrder()");
        var order = await _orderService.GetOrderByIdAsync(1);
        await _orderService.DeleteOrderAsync(order);
		logger.LogDebug("ItShouldDeleteOrder(): order → {Order}", order);

        order.Deleted.Should().BeTrue();
        order = await GetService<IRepository<Order>>().GetByIdAsync(1);
        order.Should().NotBeNull();
		logger.LogDebug("ItShouldDeleteOrder(): order.Deleted → {Arg0}", order.Deleted);
        order.Deleted = false;
        await _orderService.UpdateOrderAsync(order);
		logger.LogDebug("<<< Exiting ItShouldDeleteOrder()");
    }

    [Test]
    public void ItShouldThrowIfOrderIsNullWhenInsertOrder()
    {
		logger.LogDebug(">>> Entering ItShouldThrowIfOrderIsNullWhenInsertOrder()");
        Assert.Throws<AggregateException>(() => _orderService.InsertOrderAsync(null).Wait());
		logger.LogDebug("<<< Exiting ItShouldThrowIfOrderIsNullWhenInsertOrder()");
    }

    [Test]
    public async Task ItShouldInsertOrder()
    {
		logger.LogDebug(">>> Entering ItShouldInsertOrder()");
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
		logger.LogDebug("<<< Exiting ItShouldInsertOrder()");
		logger.LogInformation("ItShouldInsertOrder(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public void ItShouldThrowIfOrderIsNullWhenUpdateOrder()
    {
		logger.LogDebug(">>> Entering ItShouldThrowIfOrderIsNullWhenUpdateOrder()");
        Assert.Throws<AggregateException>(() => _orderService.UpdateOrderAsync(null).Wait());
		logger.LogDebug("<<< Exiting ItShouldThrowIfOrderIsNullWhenUpdateOrder()");
    }
}