using FluentAssertions;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingPluginManagerTests : ServiceTest
{
    #region Fields

    private IShippingPluginManager _shippingPluginManager;

    #endregion

    #region Setup

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _shippingPluginManager = GetService<IShippingPluginManager>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _shippingPluginManager → {_shippingPluginManager}", _shippingPluginManager);
    }

    #endregion

    [Test]
    public async Task CanLoadShippingRateComputationMethods()
    {
		logger.LogDebug(">>> Entering CanLoadShippingRateComputationMethods()");
        var shippingRateComputationMethods = await _shippingPluginManager.LoadAllPluginsAsync();
        shippingRateComputationMethods.Should().NotBeNull();
		logger.LogDebug("CanLoadShippingRateComputationMethods(): shippingRateComputationMethods → {ShippingRateComputationMethods}", shippingRateComputationMethods);
        shippingRateComputationMethods.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanLoadShippingRateComputationMethods()");
    }

    [Test]
    public async Task CanLoadShippingRateComputationMethodBySystemKeyword()
    {
		logger.LogDebug(">>> Entering CanLoadShippingRateComputationMethodBySystemKeyword()");
        var shippingRateComputationMethod = await _shippingPluginManager.LoadPluginBySystemNameAsync("FixedRateTestShippingRateComputationMethod");
        shippingRateComputationMethod.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanLoadShippingRateComputationMethodBySystemKeyword()");
		logger.LogDebug("CanLoadShippingRateComputationMethodBySystemKeyword(): shippingRateComputationMethod → {ShippingRateComputationMethod}", shippingRateComputationMethod);
    }

    [Test]
    public async Task CanLoadActiveShippingRateComputationMethods()
    {
		logger.LogDebug(">>> Entering CanLoadActiveShippingRateComputationMethods()");
        var shippingRateComputationMethods = await _shippingPluginManager.LoadActivePluginsAsync(["FixedRateTestShippingRateComputationMethod"]);
        shippingRateComputationMethods.Should().NotBeNull();
		logger.LogDebug("CanLoadActiveShippingRateComputationMethods(): shippingRateComputationMethods → {ShippingRateComputationMethods}", shippingRateComputationMethods);
        shippingRateComputationMethods.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanLoadActiveShippingRateComputationMethods()");
    }
}
