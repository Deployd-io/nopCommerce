using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Tests.Nop.Services.Tests.Payments;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Orders;

[TestFixture]
public class OrderProcessingServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IOrderService _orderService;
    private IOrderProcessingService _orderProcessingService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _orderService = GetService<IOrderService>();
        _orderProcessingService = GetService<IOrderProcessingService>();
		_logger.LogDebug("SetUp(): _orderProcessingService → {_orderProcessingService}", _orderProcessingService);
		_logger.LogDebug("SetUp(): _orderService → {_orderService}", _orderService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        TestPaymentMethod.TestSupportRefund = false;
        TestPaymentMethod.TestSupportCapture = false;
		_logger.LogDebug("TearDown(): TestPaymentMethod.TestSupportCapture → {Arg0}", TestPaymentMethod.TestSupportCapture);
		_logger.LogDebug("TearDown(): TestPaymentMethod.TestSupportPartiallyRefund → {Arg0}", TestPaymentMethod.TestSupportPartiallyRefund);
		_logger.LogDebug("TearDown(): TestPaymentMethod.TestSupportVoid → {Arg0}", TestPaymentMethod.TestSupportVoid);
		_logger.LogDebug("TearDown(): TestPaymentMethod.TestSupportRefund → {Arg0}", TestPaymentMethod.TestSupportRefund);
        TestPaymentMethod.TestSupportPartiallyRefund = false;
        TestPaymentMethod.TestSupportVoid = false;

        await GetService<IRepository<RecurringPayment>>().TruncateAsync();
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanGenerateOrderGuid()
    {
		_logger.LogDebug(">>> Entering CanGenerateOrderGuid()");
        var request = new ProcessPaymentRequest();
        request.OrderGuid.Should().NotBe(Guid.Empty);
		_logger.LogDebug("CanGenerateOrderGuid(): request → {Request}", request);
        var oldGuid = request.OrderGuid;
        await _orderProcessingService.SetProcessPaymentRequestAsync(request);
        request = new ProcessPaymentRequest();
        request.OrderGuid.Should().NotBe(oldGuid);
        await _orderProcessingService.SetProcessPaymentRequestAsync(request);
        request.OrderGuid.Should().Be(oldGuid);
		_logger.LogDebug("<<< Exiting CanGenerateOrderGuid()");
    }

    [Test]
    public void EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet()");
        var order = new Order();
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
		_logger.LogDebug("EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet(): order → {Order}", order);
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;
            if (os != OrderStatus.Cancelled)
                _orderProcessingService.CanCancelOrder(order).Should().BeTrue();
            else
                _orderProcessingService.CanCancelOrder(order).Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet()");
    }

    [Test]
    public void EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending()");
        var order = new Order();
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
		_logger.LogDebug("EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending(): order → {Order}", order);
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;
            if (os != OrderStatus.Cancelled && ps == PaymentStatus.Pending)
                _orderProcessingService.CanMarkOrderAsAuthorized(order).Should().BeTrue();
            else
                _orderProcessingService.CanMarkOrderAsAuthorized(order).Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending()");
    }

    [Test]
    public async Task EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture()");
        var order = new Order
        {
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportCapture = true;
		_logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): TestPaymentMethod.TestSupportCapture → {Arg0}", TestPaymentMethod.TestSupportCapture);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (os != OrderStatus.Cancelled && os != OrderStatus.Pending
                                            && ps == PaymentStatus.Authorized)
            {
                var canCapture = await _orderProcessingService.CanCaptureAsync(order);
				_logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): canCapture → {CanCapture}", canCapture);
                canCapture.Should().BeTrue();
            }
            else
            {
                var canCapture = await _orderProcessingService.CanCaptureAsync(order);
                canCapture.Should().BeFalse();
            }
        }

        TestPaymentMethod.TestSupportCapture = false;

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canCapture = await _orderProcessingService.CanCaptureAsync(order);
            canCapture.Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture()");
    }

    [Test]
    public void EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided()");
        var order = new Order();
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
		_logger.LogDebug("EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided(): order → {Order}", order);
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;
            if (os == OrderStatus.Cancelled || ps == PaymentStatus.Paid || ps == PaymentStatus.Refunded || ps == PaymentStatus.Voided)
                _orderProcessingService.CanMarkOrderAsPaid(order).Should().BeFalse();
            else
                _orderProcessingService.CanMarkOrderAsPaid(order).Should().BeTrue();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided()");
    }

    [Test]
    public async Task EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund()");
        var order = new Order
        {
            OrderTotal = 1,
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportRefund = true;
		_logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): TestPaymentMethod.TestSupportRefund → {Arg0}", TestPaymentMethod.TestSupportRefund);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Paid)
            {
                var canRefund = await _orderProcessingService.CanRefundAsync(order);
				_logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): canRefund → {CanRefund}", canRefund);
                canRefund.Should().BeTrue();
            }
            else
            {
                var canRefund = await _orderProcessingService.CanRefundAsync(order);
                canRefund.Should().BeFalse();
            }
        }

        TestPaymentMethod.TestSupportRefund = false;

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canRefund = await _orderProcessingService.CanRefundAsync(order);
            canRefund.Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund()");
    }

    [Test]
    public async Task EnsureOrderCannotBeRefundedWhenOrderTotalIsZero()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCannotBeRefundedWhenOrderTotalIsZero()");
        var order = new Order
        {
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportRefund = true;
		_logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): TestPaymentMethod.TestSupportRefund → {Arg0}", TestPaymentMethod.TestSupportRefund);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canRefund = await _orderProcessingService.CanRefundAsync(order);
			_logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): canRefund → {CanRefund}", canRefund);
            canRefund.Should().BeFalse();
        }

        TestPaymentMethod.TestSupportRefund = false;
		_logger.LogDebug("<<< Exiting EnsureOrderCannotBeRefundedWhenOrderTotalIsZero()");
    }

    [Test]
    public void EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid()");
        var order = new Order
        {
            OrderTotal = 1
        };
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Paid)
                _orderProcessingService.CanRefundOffline(order).Should().BeTrue();
            else
                _orderProcessingService.CanRefundOffline(order).Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid()");
    }

    [Test]
    public void EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero()");
        var order = new Order();

		_logger.LogDebug("EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero(): order → {Order}", order);
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            _orderProcessingService.CanRefundOffline(order).Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero()");
    }

    [Test]
    public async Task EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid()");
        var order = new Order
        {
            OrderTotal = 1,
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportVoid = true;
		_logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): TestPaymentMethod.TestSupportVoid → {Arg0}", TestPaymentMethod.TestSupportVoid);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Authorized)
            {
                var canVoid = await _orderProcessingService.CanVoidAsync(order);
				_logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): canVoid → {CanVoid}", canVoid);
                canVoid.Should().BeTrue();
            }
            else
            {
                var canVoid = await _orderProcessingService.CanVoidAsync(order);
                canVoid.Should().BeFalse();
            }
        }

        TestPaymentMethod.TestSupportVoid = false;

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canVoid = await _orderProcessingService.CanVoidAsync(order);
            canVoid.Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid()");
    }

    [Test]
    public async Task EnsureOrderCannotBeVoidedWhenOrderTotalIsZero()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCannotBeVoidedWhenOrderTotalIsZero()");
        var order = new Order
        {
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportVoid = true;
		_logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): TestPaymentMethod.TestSupportVoid → {Arg0}", TestPaymentMethod.TestSupportVoid);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canVoid = await _orderProcessingService.CanVoidAsync(order);
			_logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): canVoid → {CanVoid}", canVoid);
            canVoid.Should().BeFalse();
        }

        TestPaymentMethod.TestSupportVoid = false;
		_logger.LogDebug("<<< Exiting EnsureOrderCannotBeVoidedWhenOrderTotalIsZero()");
    }

    [Test]
    public void EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized()");
        var order = new Order
        {
            OrderTotal = 1
        };
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Authorized)
                _orderProcessingService.CanVoidOffline(order).Should().BeTrue();
            else
                _orderProcessingService.CanVoidOffline(order).Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized()");
    }

    [Test]
    public void EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero()");
        var order = new Order();

		_logger.LogDebug("EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero(): order → {Order}", order);
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            _orderProcessingService.CanVoidOffline(order).Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero()");
    }

    [Test]
    public async Task EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund()");
        var order = new Order
        {
            OrderTotal = 100,
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportPartiallyRefund = true;
		_logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): TestPaymentMethod.TestSupportPartiallyRefund → {Arg0}", TestPaymentMethod.TestSupportPartiallyRefund);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded)
            {
                var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 10);
				_logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): canPartiallyRefund → {CanPartiallyRefund}", canPartiallyRefund);
                canPartiallyRefund.Should().BeTrue();
            }
            else
            {
                var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 10);
                canPartiallyRefund.Should().BeFalse();
            }
        }

        TestPaymentMethod.TestSupportPartiallyRefund = false;

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 10);
            canPartiallyRefund.Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund()");
    }

    [Test]
    public async Task EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()");
        var order = new Order
        {
            OrderTotal = 100,
            RefundedAmount = 30, //100-30=70 can be refunded
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportPartiallyRefund = true;
		_logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): TestPaymentMethod.TestSupportPartiallyRefund → {Arg0}", TestPaymentMethod.TestSupportPartiallyRefund);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 80);
			_logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): canPartiallyRefund → {CanPartiallyRefund}", canPartiallyRefund);
            canPartiallyRefund.Should().BeFalse();
        }

        TestPaymentMethod.TestSupportPartiallyRefund = false;
		_logger.LogDebug("<<< Exiting EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()");
    }

    [Test]
    public void EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded()");
        var order = new Order
        {
            OrderTotal = 100
        };

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            {
                order.OrderStatus = os;
                order.PaymentStatus = ps;
                order.ShippingStatus = ss;

                if (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                    _orderProcessingService.CanPartiallyRefundOffline(order, 10).Should().BeTrue();
                else
                    _orderProcessingService.CanPartiallyRefundOffline(order, 10).Should().BeFalse();
            }
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded()");
    }

    [Test]
    public void EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()
    {
		_logger.LogDebug(">>> Entering EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()");
        var order = new Order
        {
            OrderTotal = 100,
            RefundedAmount = 30 //100-30=70 can be refunded
        };

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			_logger.LogDebug("EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.OrderStatus → {Arg0}", order.OrderStatus);
			_logger.LogDebug("EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			_logger.LogDebug("EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            _orderProcessingService.CanPartiallyRefundOffline(order, 80).Should().BeFalse();
        }
		_logger.LogDebug("<<< Exiting EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()");
    }

    //RecurringPaymentHistory
    [Test]
    public async Task CanCalculateNextPaymentDateWithDaysAsCyclePeriod()
    {
		_logger.LogDebug(">>> Entering CanCalculateNextPaymentDateWithDaysAsCyclePeriod()");
        var rp = new RecurringPayment
        {
            CycleLength = 7,
            CyclePeriod = RecurringProductCyclePeriod.Days,
            TotalCycles = 3,
            StartDateUtc = new DateTime(2010, 3, 1),
            CreatedOnUtc = new DateTime(2010, 1, 1),
            IsActive = true,
            InitialOrderId = 1
        };

        await _orderService.InsertRecurringPaymentAsync(rp);

        var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
		_logger.LogDebug("CanCalculateNextPaymentDateWithDaysAsCyclePeriod(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));

        //add one history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 8));

        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 15));

        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanCalculateNextPaymentDateWithDaysAsCyclePeriod()");
    }

    [Test]
    public async Task CanCalculateNextPaymentDateWithWeeksAsCyclePeriod()
    {
		_logger.LogDebug(">>> Entering CanCalculateNextPaymentDateWithWeeksAsCyclePeriod()");
        var rp = new RecurringPayment
        {
            CycleLength = 2,
            CyclePeriod = RecurringProductCyclePeriod.Weeks,
            TotalCycles = 3,
            StartDateUtc = new DateTime(2010, 3, 1),
            CreatedOnUtc = new DateTime(2010, 1, 1),
            IsActive = true,
            InitialOrderId = 1
        };

        await _orderService.InsertRecurringPaymentAsync(rp);

        var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
		_logger.LogDebug("CanCalculateNextPaymentDateWithWeeksAsCyclePeriod(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));

        //add one history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 15));
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 29));
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanCalculateNextPaymentDateWithWeeksAsCyclePeriod()");
    }

    [Test]
    public async Task CanCalculateNextPaymentDateWithMonthsAsCyclePeriod()
    {
		_logger.LogDebug(">>> Entering CanCalculateNextPaymentDateWithMonthsAsCyclePeriod()");
        var rp = new RecurringPayment
        {
            CycleLength = 2,
            CyclePeriod = RecurringProductCyclePeriod.Months,
            TotalCycles = 3,
            StartDateUtc = new DateTime(2010, 3, 1),
            CreatedOnUtc = new DateTime(2010, 1, 1),
            IsActive = true,
            InitialOrderId = 1
        };

        await _orderService.InsertRecurringPaymentAsync(rp);

        var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
		_logger.LogDebug("CanCalculateNextPaymentDateWithMonthsAsCyclePeriod(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));

        //add one history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().Be(new DateTime(2010, 5, 1));
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().Be(new DateTime(2010, 7, 1));
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanCalculateNextPaymentDateWithMonthsAsCyclePeriod()");
    }

    [Test]
    public async Task CanCalculateNextPaymentDateWithYearsAsCyclePeriod()
    {
		_logger.LogDebug(">>> Entering CanCalculateNextPaymentDateWithYearsAsCyclePeriod()");
        var rp = new RecurringPayment
        {
            CycleLength = 2,
            CyclePeriod = RecurringProductCyclePeriod.Years,
            TotalCycles = 3,
            StartDateUtc = new DateTime(2010, 3, 1),
            CreatedOnUtc = new DateTime(2010, 1, 1),
            IsActive = true,
            InitialOrderId = 1
        };

        await _orderService.InsertRecurringPaymentAsync(rp);

        var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
		_logger.LogDebug("CanCalculateNextPaymentDateWithYearsAsCyclePeriod(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));

        //add one history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().Be(new DateTime(2012, 3, 1));
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().Be(new DateTime(2014, 3, 1));
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanCalculateNextPaymentDateWithYearsAsCyclePeriod()");
    }

    [Test]
    public async Task NextPaymentDateIsNullWhenRecurringPaymentIsNotActive()
    {
		_logger.LogDebug(">>> Entering NextPaymentDateIsNullWhenRecurringPaymentIsNotActive()");
        var rp = new RecurringPayment
        {
            CycleLength = 7,
            CyclePeriod = RecurringProductCyclePeriod.Days,
            TotalCycles = 3,
            StartDateUtc = new DateTime(2010, 3, 1),
            CreatedOnUtc = new DateTime(2010, 1, 1),
            IsActive = false,
            InitialOrderId = 1
        };

        var nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
		_logger.LogDebug("NextPaymentDateIsNullWhenRecurringPaymentIsNotActive(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);
        nextPaymentDate.Should().BeNull();

        await _orderService.InsertRecurringPaymentAsync(rp);

        //add one history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().BeNull();
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().BeNull();
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        nextPaymentDate = await _orderProcessingService.GetNextPaymentDateAsync(rp);
        nextPaymentDate.Should().BeNull();
		_logger.LogDebug("<<< Exiting NextPaymentDateIsNullWhenRecurringPaymentIsNotActive()");
    }

    [Test]
    public async Task CanCalculateNumberOfRemainingCycle()
    {
		_logger.LogDebug(">>> Entering CanCalculateNumberOfRemainingCycle()");
        var rp = new RecurringPayment
        {
            CycleLength = 2,
            CyclePeriod = RecurringProductCyclePeriod.Days,
            TotalCycles = 3,
            StartDateUtc = new DateTime(2010, 3, 1),
            CreatedOnUtc = new DateTime(2010, 1, 1),
            IsActive = true,
            InitialOrderId = 1
        };

        await _orderService.InsertRecurringPaymentAsync(rp);

        var cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
		_logger.LogDebug("CanCalculateNumberOfRemainingCycle(): cyclesRemaining → {CyclesRemaining}", cyclesRemaining);
        cyclesRemaining.Should().Be(3);

        //add one history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
        cyclesRemaining.Should().Be(2);
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
        cyclesRemaining.Should().Be(1);
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
        cyclesRemaining.Should().Be(0);
        //add one more history record
        await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory { RecurringPaymentId = rp.Id });
        cyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(rp);
        cyclesRemaining.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanCalculateNumberOfRemainingCycle()");
    }
}