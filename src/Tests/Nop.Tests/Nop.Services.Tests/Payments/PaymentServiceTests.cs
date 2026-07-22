using FluentAssertions;
using Nop.Services.Payments;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Payments;

[TestFixture]
public class PaymentServiceTests : ServiceTest
{
    private IPaymentPluginManager _paymentPluginManager;
    private IPaymentService _paymentService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _paymentService = GetService<IPaymentService>();
        _paymentPluginManager = GetService<IPaymentPluginManager>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _paymentService → {_paymentService}", _paymentService);
		logger.LogDebug("SetUp(): _paymentPluginManager → {_paymentPluginManager}", _paymentPluginManager);
    }

    [Test]
    public async Task CanLoadPaymentMethods()
    {
		logger.LogDebug(">>> Entering CanLoadPaymentMethods()");
        var paymentMethods = await _paymentPluginManager.LoadAllPluginsAsync();
        paymentMethods.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanLoadPaymentMethods()");
		logger.LogDebug("CanLoadPaymentMethods(): paymentMethods → {PaymentMethods}", paymentMethods);
    }

    [Test]
    public async Task CanLoadPaymentMethodBySystemKeyword()
    {
		logger.LogDebug(">>> Entering CanLoadPaymentMethodBySystemKeyword()");
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.TestMethod");
        paymentMethod.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanLoadPaymentMethodBySystemKeyword()");
		logger.LogDebug("CanLoadPaymentMethodBySystemKeyword(): paymentMethod → {PaymentMethod}", paymentMethod);
    }

    [Test]
    public async Task CanLoadActivePaymentMethods()
    {
		logger.LogDebug(">>> Entering CanLoadActivePaymentMethods()");
        var paymentMethods = await _paymentPluginManager.LoadActivePluginsAsync(["Payments.TestMethod"]);
        paymentMethods.Should().NotBeNull();
		logger.LogDebug("CanLoadActivePaymentMethods(): paymentMethods → {PaymentMethods}", paymentMethods);
        paymentMethods.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanLoadActivePaymentMethods()");
    }

    [Test]
    public void CanGetMaskedCreditCardNumber()
    {
		logger.LogDebug(">>> Entering CanGetMaskedCreditCardNumber()");
        _paymentService.GetMaskedCreditCardNumber(string.Empty).Should().Be(string.Empty);
        _paymentService.GetMaskedCreditCardNumber("123").Should().Be("123");
        _paymentService.GetMaskedCreditCardNumber("1234567890123456").Should().Be("************3456");
		logger.LogDebug("<<< Exiting CanGetMaskedCreditCardNumber()");
    }
}
