using FluentAssertions;
using Nop.Services.Payments;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Payments;

[TestFixture]
public class PaymentServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IPaymentPluginManager _paymentPluginManager;
    private IPaymentService _paymentService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _paymentService = GetService<IPaymentService>();
        _paymentPluginManager = GetService<IPaymentPluginManager>();
		_logger.LogDebug("SetUp(): _paymentPluginManager → {_paymentPluginManager}", _paymentPluginManager);
		_logger.LogDebug("SetUp(): _paymentService → {_paymentService}", _paymentService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task CanLoadPaymentMethods()
    {
		_logger.LogDebug(">>> Entering CanLoadPaymentMethods()");
        var paymentMethods = await _paymentPluginManager.LoadAllPluginsAsync();
        paymentMethods.Should().NotBeNull();
		_logger.LogDebug("CanLoadPaymentMethods(): paymentMethods → {PaymentMethods}", paymentMethods);
		_logger.LogDebug("<<< Exiting CanLoadPaymentMethods()");
    }

    [Test]
    public async Task CanLoadPaymentMethodBySystemKeyword()
    {
		_logger.LogDebug(">>> Entering CanLoadPaymentMethodBySystemKeyword()");
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.TestMethod");
        paymentMethod.Should().NotBeNull();
		_logger.LogDebug("CanLoadPaymentMethodBySystemKeyword(): paymentMethod → {PaymentMethod}", paymentMethod);
		_logger.LogDebug("<<< Exiting CanLoadPaymentMethodBySystemKeyword()");
    }

    [Test]
    public async Task CanLoadActivePaymentMethods()
    {
		_logger.LogDebug(">>> Entering CanLoadActivePaymentMethods()");
        var paymentMethods = await _paymentPluginManager.LoadActivePluginsAsync(["Payments.TestMethod"]);
        paymentMethods.Should().NotBeNull();
		_logger.LogDebug("CanLoadActivePaymentMethods(): paymentMethods → {PaymentMethods}", paymentMethods);
        paymentMethods.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanLoadActivePaymentMethods()");
    }

    [Test]
    public void CanGetMaskedCreditCardNumber()
    {
		_logger.LogDebug(">>> Entering CanGetMaskedCreditCardNumber()");
        _paymentService.GetMaskedCreditCardNumber(string.Empty).Should().Be(string.Empty);
        _paymentService.GetMaskedCreditCardNumber("123").Should().Be("123");
        _paymentService.GetMaskedCreditCardNumber("1234567890123456").Should().Be("************3456");
		_logger.LogDebug("<<< Exiting CanGetMaskedCreditCardNumber()");
    }
}