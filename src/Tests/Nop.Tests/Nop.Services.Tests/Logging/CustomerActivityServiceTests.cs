using FluentAssertions;
using Nop.Services.Customers;
using Nop.Services.Logging;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Logging;

[TestFixture]
public class CustomerActivityServiceTests : ServiceTest
{
    private ICustomerActivityService _customerActivityService;
    private ICustomerService _customerService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _customerActivityService = GetService<ICustomerActivityService>();
        _customerService = GetService<ICustomerService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _customerActivityService → {_customerActivityService}", _customerActivityService);
		logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
    }

    [Test]
    public async Task CanFindActivities()
    {
		logger.LogDebug(">>> Entering CanFindActivities()");
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);

		logger.LogDebug("CanFindActivities(): customer → {Customer}", customer);
        var activities = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id, pageSize: 10);
        activities.Any().Should().BeTrue();
		logger.LogDebug("CanFindActivities(): activities → {Activities}", activities);

        customer = await _customerService.GetCustomerByEmailAsync("builtin@search_engine_record.com");

        activities = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id, pageSize: 10);
        activities.Any().Should().BeFalse();
		logger.LogDebug("<<< Exiting CanFindActivities()");
    }
}
