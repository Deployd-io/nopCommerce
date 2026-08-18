using FluentAssertions;
using Nop.Services.Shipping;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingPluginManagerTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private IShippingPluginManager _shippingPluginManager;

    #endregion

    #region Setup

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _shippingPluginManager = GetService<IShippingPluginManager>();
		_logger.LogDebug("SetUp(): _shippingPluginManager → {_shippingPluginManager}", _shippingPluginManager);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    #endregion

    [Test]
    public async Task CanLoadShippingRateComputationMethods()
    {
		_logger.LogDebug(">>> Entering CanLoadShippingRateComputationMethods()");
        var shippingRateComputationMethods = await _shippingPluginManager.LoadAllPluginsAsync();
        shippingRateComputationMethods.Should().NotBeNull();
		_logger.LogDebug("CanLoadShippingRateComputationMethods(): shippingRateComputationMethods → {ShippingRateComputationMethods}", shippingRateComputationMethods);
        shippingRateComputationMethods.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanLoadShippingRateComputationMethods()");
    }

    [Test]
    public async Task CanLoadShippingRateComputationMethodBySystemKeyword()
    {
		_logger.LogDebug(">>> Entering CanLoadShippingRateComputationMethodBySystemKeyword()");
        var shippingRateComputationMethod = await _shippingPluginManager.LoadPluginBySystemNameAsync("FixedRateTestShippingRateComputationMethod");
        shippingRateComputationMethod.Should().NotBeNull();
		_logger.LogDebug("CanLoadShippingRateComputationMethodBySystemKeyword(): shippingRateComputationMethod → {ShippingRateComputationMethod}", shippingRateComputationMethod);
		_logger.LogDebug("<<< Exiting CanLoadShippingRateComputationMethodBySystemKeyword()");
    }

    [Test]
    public async Task CanLoadActiveShippingRateComputationMethods()
    {
		_logger.LogDebug(">>> Entering CanLoadActiveShippingRateComputationMethods()");
        var shippingRateComputationMethods = await _shippingPluginManager.LoadActivePluginsAsync(["FixedRateTestShippingRateComputationMethod"]);
        shippingRateComputationMethods.Should().NotBeNull();
		_logger.LogDebug("CanLoadActiveShippingRateComputationMethods(): shippingRateComputationMethods → {ShippingRateComputationMethods}", shippingRateComputationMethods);
        shippingRateComputationMethods.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanLoadActiveShippingRateComputationMethods()");
    }
}