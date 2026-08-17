using FluentAssertions;
using Nop.Core.Domain.Stores;
using Nop.Services.Stores;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Stores;

[TestFixture]
public class StoreExtensionsTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IStoreService _storeService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _storeService = GetService<IStoreService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _storeService → {_storeService}", _storeService);
    }

    [Test]
    public void CanFindHostValue()
    {
		_logger.LogDebug(">>> Entering CanFindHostValue()");
        var store = new Store
        {
            Hosts = "yourstore.com, www.yourstore.com, "
        };

        _storeService.ContainsHostValue(store, null).Should().BeFalse();
        _storeService.ContainsHostValue(store, string.Empty).Should().BeFalse();
        _storeService.ContainsHostValue(store, "store.com").Should().BeFalse();
        _storeService.ContainsHostValue(store, "yourstore.com").Should().BeTrue();
        _storeService.ContainsHostValue(store, "yoursTore.com").Should().BeTrue();
        _storeService.ContainsHostValue(store, "www.yourstore.com").Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanFindHostValue()");
    }
}