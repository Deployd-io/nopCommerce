using FluentAssertions;
using Nop.Services.Customers;
using Nop.Services.Logging;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Logging;

[TestFixture]
public class CustomerActivityServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ICustomerActivityService _customerActivityService;
    private ICustomerService _customerService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _customerActivityService = GetService<ICustomerActivityService>();
        _customerService = GetService<ICustomerService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _customerActivityService → {_customerActivityService}", _customerActivityService);
		_logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
    }

    [Test]
    public async Task CanFindActivities()
    {
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);

		_logger.LogDebug("CanFindActivities(): customer → {Customer}", customer);
        var activities = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id, pageSize: 10);
        activities.Any().Should().BeTrue();
		_logger.LogDebug("CanFindActivities(): activities → {Activities}", activities);

        customer = await _customerService.GetCustomerByEmailAsync("builtin@search_engine_record.com");

        activities = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id, pageSize: 10);
        activities.Any().Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanFindActivities()");
    }
}