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

namespace Nop.Tests.Nop.Services.Tests.Orders;

[TestFixture]
public class OrderProcessingServiceTests : ServiceTest
{
    private IOrderService _orderService;
    private IOrderProcessingService _orderProcessingService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _orderService = GetService<IOrderService>();
        _orderProcessingService = GetService<IOrderProcessingService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _orderService → {_orderService}", _orderService);
		logger.LogDebug("SetUp(): _orderProcessingService → {_orderProcessingService}", _orderProcessingService);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        TestPaymentMethod.TestSupportRefund = false;
        TestPaymentMethod.TestSupportCapture = false;
		logger.LogDebug("TearDown(): TestPaymentMethod.TestSupportCapture → {Arg0}", TestPaymentMethod.TestSupportCapture);
		logger.LogDebug("TearDown(): TestPaymentMethod.TestSupportPartiallyRefund → {Arg0}", TestPaymentMethod.TestSupportPartiallyRefund);
		logger.LogDebug("TearDown(): TestPaymentMethod.TestSupportVoid → {Arg0}", TestPaymentMethod.TestSupportVoid);
		logger.LogDebug("TearDown(): TestPaymentMethod.TestSupportRefund → {Arg0}", TestPaymentMethod.TestSupportRefund);
        TestPaymentMethod.TestSupportPartiallyRefund = false;
        TestPaymentMethod.TestSupportVoid = false;

        await GetService<IRepository<RecurringPayment>>().TruncateAsync();
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanGenerateOrderGuid()
    {
		logger.LogDebug(">>> Entering CanGenerateOrderGuid()");
        var request = new ProcessPaymentRequest();
        request.OrderGuid.Should().NotBe(Guid.Empty);
		logger.LogDebug("CanGenerateOrderGuid(): request → {Request}", request);
        var oldGuid = request.OrderGuid;
        await _orderProcessingService.SetProcessPaymentRequestAsync(request);
        request = new ProcessPaymentRequest();
        request.OrderGuid.Should().NotBe(oldGuid);
        await _orderProcessingService.SetProcessPaymentRequestAsync(request);
        request.OrderGuid.Should().Be(oldGuid);
		logger.LogDebug("<<< Exiting CanGenerateOrderGuid()");
    }

    [Test]
    public void EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet()");
        var order = new Order();
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
		logger.LogDebug("EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet(): order → {Order}", order);
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;
            if (os != OrderStatus.Cancelled)
                _orderProcessingService.CanCancelOrder(order).Should().BeTrue();
            else
                _orderProcessingService.CanCancelOrder(order).Should().BeFalse();
        }
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeCancelledWhenOrderStatusIsNotCanCelledYet()");
    }

    [Test]
    public void EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending()");
        var order = new Order();
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
		logger.LogDebug("EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending(): order → {Order}", order);
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;
            if (os != OrderStatus.Cancelled && ps == PaymentStatus.Pending)
                _orderProcessingService.CanMarkOrderAsAuthorized(order).Should().BeTrue();
            else
                _orderProcessingService.CanMarkOrderAsAuthorized(order).Should().BeFalse();
        }
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeMarkedAsAuthorizedWhenOrderStatusIsNotCancelledAndPaymentStatusIsPending()");
    }

    [Test]
    public async Task EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture()");
        var order = new Order
        {
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportCapture = true;
		logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): TestPaymentMethod.TestSupportCapture → {Arg0}", TestPaymentMethod.TestSupportCapture);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (os != OrderStatus.Cancelled && os != OrderStatus.Pending
                                            && ps == PaymentStatus.Authorized)
            {
                var canCapture = await _orderProcessingService.CanCaptureAsync(order);
                canCapture.Should().BeTrue();
				logger.LogDebug("EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture(): canCapture → {CanCapture}", canCapture);
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
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeCapturedWhenOrderStatusIsNotCancelledOrPendingAndPaymentStatusIsAuthorizedAndPaymentModuleSupportsCapture()");
    }

    [Test]
    public void EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided()
    {
		logger.LogDebug(">>> Entering EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided()");
        var order = new Order();
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
		logger.LogDebug("EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided(): order → {Order}", order);
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;
            if (os == OrderStatus.Cancelled || ps == PaymentStatus.Paid || ps == PaymentStatus.Refunded || ps == PaymentStatus.Voided)
                _orderProcessingService.CanMarkOrderAsPaid(order).Should().BeFalse();
            else
                _orderProcessingService.CanMarkOrderAsPaid(order).Should().BeTrue();
        }
		logger.LogDebug("<<< Exiting EnsureOrderCannotBeMarkedAsPaidWhenOrderStatusIsCancelledOrPaymentStatusIsPaidOrRefundedOrVoided()");
    }

    [Test]
    public async Task EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund()");
        var order = new Order
        {
            OrderTotal = 1,
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportRefund = true;
		logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): TestPaymentMethod.TestSupportRefund → {Arg0}", TestPaymentMethod.TestSupportRefund);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Paid)
            {
                var canRefund = await _orderProcessingService.CanRefundAsync(order);
                canRefund.Should().BeTrue();
				logger.LogDebug("EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund(): canRefund → {CanRefund}", canRefund);
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
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeRefundedWhenPaymentStatusIsPaidAndPaymentModuleSupportsRefund()");
    }

    [Test]
    public async Task EnsureOrderCannotBeRefundedWhenOrderTotalIsZero()
    {
		logger.LogDebug(">>> Entering EnsureOrderCannotBeRefundedWhenOrderTotalIsZero()");
        var order = new Order
        {
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportRefund = true;
		logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): TestPaymentMethod.TestSupportRefund → {Arg0}", TestPaymentMethod.TestSupportRefund);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canRefund = await _orderProcessingService.CanRefundAsync(order);
            canRefund.Should().BeFalse();
			logger.LogDebug("EnsureOrderCannotBeRefundedWhenOrderTotalIsZero(): canRefund → {CanRefund}", canRefund);
        }

        TestPaymentMethod.TestSupportRefund = false;
		logger.LogDebug("<<< Exiting EnsureOrderCannotBeRefundedWhenOrderTotalIsZero()");
    }

    [Test]
    public void EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid()");
        var order = new Order
        {
            OrderTotal = 1
        };
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Paid)
                _orderProcessingService.CanRefundOffline(order).Should().BeTrue();
            else
                _orderProcessingService.CanRefundOffline(order).Should().BeFalse();
        }
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeRefundedOfflineWhenPaymentStatusIsPaid()");
    }

    [Test]
    public void EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero()
    {
		logger.LogDebug(">>> Entering EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero()");
        var order = new Order();

		logger.LogDebug("EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero(): order → {Order}", order);
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            _orderProcessingService.CanRefundOffline(order).Should().BeFalse();
        }
		logger.LogDebug("<<< Exiting EnsureOrderCannotBeRefundedOfflineWhenOrdertotalIsZero()");
    }

    [Test]
    public async Task EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid()");
        var order = new Order
        {
            OrderTotal = 1,
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportVoid = true;
		logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): TestPaymentMethod.TestSupportVoid → {Arg0}", TestPaymentMethod.TestSupportVoid);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Authorized)
            {
                var canVoid = await _orderProcessingService.CanVoidAsync(order);
                canVoid.Should().BeTrue();
				logger.LogDebug("EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid(): canVoid → {CanVoid}", canVoid);
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
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeVoidedWhenPaymentStatusIsAuthorizedAndPaymentModuleSupportsVoid()");
    }

    [Test]
    public async Task EnsureOrderCannotBeVoidedWhenOrderTotalIsZero()
    {
		logger.LogDebug(">>> Entering EnsureOrderCannotBeVoidedWhenOrderTotalIsZero()");
        var order = new Order
        {
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportVoid = true;
		logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): TestPaymentMethod.TestSupportVoid → {Arg0}", TestPaymentMethod.TestSupportVoid);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canVoid = await _orderProcessingService.CanVoidAsync(order);
            canVoid.Should().BeFalse();
			logger.LogDebug("EnsureOrderCannotBeVoidedWhenOrderTotalIsZero(): canVoid → {CanVoid}", canVoid);
        }

        TestPaymentMethod.TestSupportVoid = false;
		logger.LogDebug("<<< Exiting EnsureOrderCannotBeVoidedWhenOrderTotalIsZero()");
    }

    [Test]
    public void EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized()");
        var order = new Order
        {
            OrderTotal = 1
        };
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Authorized)
                _orderProcessingService.CanVoidOffline(order).Should().BeTrue();
            else
                _orderProcessingService.CanVoidOffline(order).Should().BeFalse();
        }
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBeVoidedOfflineWhenPaymentStatusIsAuthorized()");
    }

    [Test]
    public void EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero()
    {
		logger.LogDebug(">>> Entering EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero()");
        var order = new Order();

		logger.LogDebug("EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero(): order → {Order}", order);
        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            _orderProcessingService.CanVoidOffline(order).Should().BeFalse();
        }
		logger.LogDebug("<<< Exiting EnsureOrderCannotBeVoidedOfflineWhenOrderTotalIsZero()");
    }

    [Test]
    public async Task EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund()");
        var order = new Order
        {
            OrderTotal = 100,
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportPartiallyRefund = true;
		logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): TestPaymentMethod.TestSupportPartiallyRefund → {Arg0}", TestPaymentMethod.TestSupportPartiallyRefund);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            if (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded)
            {
                var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 10);
                canPartiallyRefund.Should().BeTrue();
				logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund(): canPartiallyRefund → {CanPartiallyRefund}", canPartiallyRefund);
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
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBePartiallyRefundedWhenPaymentStatusIsPaidOrPartiallyRefundedAndPaymentModuleSupportsPartialRefund()");
    }

    [Test]
    public async Task EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()
    {
		logger.LogDebug(">>> Entering EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()");
        var order = new Order
        {
            OrderTotal = 100,
            RefundedAmount = 30, //100-30=70 can be refunded
            PaymentMethodSystemName = "Payments.TestMethod"
        };

        TestPaymentMethod.TestSupportPartiallyRefund = true;
		logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): TestPaymentMethod.TestSupportPartiallyRefund → {Arg0}", TestPaymentMethod.TestSupportPartiallyRefund);

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            var canPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, 80);
            canPartiallyRefund.Should().BeFalse();
			logger.LogDebug("EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): canPartiallyRefund → {CanPartiallyRefund}", canPartiallyRefund);
        }

        TestPaymentMethod.TestSupportPartiallyRefund = false;
		logger.LogDebug("<<< Exiting EnsureOrderCannotBePartiallyRefundedWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()");
    }

    [Test]
    public void EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded()
    {
		logger.LogDebug(">>> Entering EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded()");
        var order = new Order
        {
            OrderTotal = 100
        };

        foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
        foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
        foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
        {
            order.OrderStatus = os;
			logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
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
		logger.LogDebug("<<< Exiting EnsureOrderCanOnlyBePartiallyRefundedOfflineWhenPaymentStatusIsPaidOrPartiallyRefunded()");
    }

    [Test]
    public void EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()
    {
		logger.LogDebug(">>> Entering EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()");
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
			logger.LogDebug("EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.OrderStatus → {Arg0}", order.OrderStatus);
			logger.LogDebug("EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.PaymentStatus → {Arg0}", order.PaymentStatus);
			logger.LogDebug("EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded(): order.ShippingStatus → {Arg0}", order.ShippingStatus);
            order.PaymentStatus = ps;
            order.ShippingStatus = ss;

            _orderProcessingService.CanPartiallyRefundOffline(order, 80).Should().BeFalse();
        }
		logger.LogDebug("<<< Exiting EnsureOrderCannotBePartiallyRefundedOfflineWhenAmountToRefundIsGreaterThanAmountThatCanBeRefunded()");
    }

    //RecurringPaymentHistory
    [Test]
    public async Task CanCalculateNextPaymentDateWithDaysAsCyclePeriod()
    {
		logger.LogDebug(">>> Entering CanCalculateNextPaymentDateWithDaysAsCyclePeriod()");
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
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));
		logger.LogDebug("CanCalculateNextPaymentDateWithDaysAsCyclePeriod(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);

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
		logger.LogDebug("<<< Exiting CanCalculateNextPaymentDateWithDaysAsCyclePeriod()");
    }

    [Test]
    public async Task CanCalculateNextPaymentDateWithWeeksAsCyclePeriod()
    {
		logger.LogDebug(">>> Entering CanCalculateNextPaymentDateWithWeeksAsCyclePeriod()");
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
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));
		logger.LogDebug("CanCalculateNextPaymentDateWithWeeksAsCyclePeriod(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);

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
		logger.LogDebug("<<< Exiting CanCalculateNextPaymentDateWithWeeksAsCyclePeriod()");
    }

    [Test]
    public async Task CanCalculateNextPaymentDateWithMonthsAsCyclePeriod()
    {
		logger.LogDebug(">>> Entering CanCalculateNextPaymentDateWithMonthsAsCyclePeriod()");
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
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));
		logger.LogDebug("CanCalculateNextPaymentDateWithMonthsAsCyclePeriod(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);

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
		logger.LogDebug("<<< Exiting CanCalculateNextPaymentDateWithMonthsAsCyclePeriod()");
    }

    [Test]
    public async Task CanCalculateNextPaymentDateWithYearsAsCyclePeriod()
    {
		logger.LogDebug(">>> Entering CanCalculateNextPaymentDateWithYearsAsCyclePeriod()");
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
        nextPaymentDate.Should().Be(new DateTime(2010, 3, 1));
		logger.LogDebug("CanCalculateNextPaymentDateWithYearsAsCyclePeriod(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);

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
		logger.LogDebug("<<< Exiting CanCalculateNextPaymentDateWithYearsAsCyclePeriod()");
    }

    [Test]
    public async Task NextPaymentDateIsNullWhenRecurringPaymentIsNotActive()
    {
		logger.LogDebug(">>> Entering NextPaymentDateIsNullWhenRecurringPaymentIsNotActive()");
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
        nextPaymentDate.Should().BeNull();
		logger.LogDebug("NextPaymentDateIsNullWhenRecurringPaymentIsNotActive(): nextPaymentDate → {NextPaymentDate}", nextPaymentDate);

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
		logger.LogDebug("<<< Exiting NextPaymentDateIsNullWhenRecurringPaymentIsNotActive()");
    }

    [Test]
    public async Task CanCalculateNumberOfRemainingCycle()
    {
		logger.LogDebug(">>> Entering CanCalculateNumberOfRemainingCycle()");
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
        cyclesRemaining.Should().Be(3);
		logger.LogDebug("CanCalculateNumberOfRemainingCycle(): cyclesRemaining → {CyclesRemaining}", cyclesRemaining);

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
		logger.LogDebug("<<< Exiting CanCalculateNumberOfRemainingCycle()");
    }
}