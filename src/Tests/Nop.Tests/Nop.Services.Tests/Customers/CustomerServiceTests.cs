using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Customers;

[TestFixture]
public class CustomerServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ICustomerService _customerService;

    [SetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _customerService = GetService<ICustomerService>();
        var moderator = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.ForumModeratorsRoleName);
		_logger.LogDebug("SetUp(): moderator → {Moderator}", moderator);
		_logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
		_logger.LogDebug("SetUp(): moderator.Active → {Arg0}", moderator.Active);
        moderator.Active = false;
        await _customerService.UpdateCustomerRoleAsync(moderator);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [TearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        var moderator = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.ForumModeratorsRoleName);
        moderator.Active = true;
		_logger.LogDebug("TearDown(): moderator.Active → {Arg0}", moderator.Active);
		_logger.LogDebug("TearDown(): moderator → {Moderator}", moderator);
        await _customerService.UpdateCustomerRoleAsync(moderator);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanCheckIsInCustomerRole()
    {
		_logger.LogDebug(">>> Entering CanCheckIsInCustomerRole()");
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);

		_logger.LogDebug("CanCheckIsInCustomerRole(): customer → {Customer}", customer);
        var isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName, false);
		_logger.LogDebug("CanCheckIsInCustomerRole(): isInCustomerRole → {IsInCustomerRole}", isInCustomerRole);
        isInCustomerRole.Should().BeTrue();
        isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName);
        isInCustomerRole.Should().BeTrue();

        isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.ForumModeratorsRoleName, false);
        isInCustomerRole.Should().BeTrue();
        isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.ForumModeratorsRoleName);
        isInCustomerRole.Should().BeFalse();

        isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.GuestsRoleName, false);
        isInCustomerRole.Should().BeFalse();
        isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.GuestsRoleName);
        isInCustomerRole.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanCheckIsInCustomerRole()");
    }

    [Test]
    public async Task CanCheckWhetherCustomerIsAdmin()
    {
		_logger.LogDebug(">>> Entering CanCheckWhetherCustomerIsAdmin()");
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        var isAdmin = await _customerService.IsAdminAsync(customer);
		_logger.LogDebug("CanCheckWhetherCustomerIsAdmin(): isAdmin → {IsAdmin}", isAdmin);
		_logger.LogDebug("CanCheckWhetherCustomerIsAdmin(): customer → {Customer}", customer);
        isAdmin.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanCheckWhetherCustomerIsAdmin()");
    }

    [Test]
    public async Task CanCheckWhetherCustomerIsForumModerator()
    {
		_logger.LogDebug(">>> Entering CanCheckWhetherCustomerIsForumModerator()");
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        var isForumModerator = await _customerService.IsForumModeratorAsync(customer, false);
		_logger.LogDebug("CanCheckWhetherCustomerIsForumModerator(): isForumModerator → {IsForumModerator}", isForumModerator);
		_logger.LogDebug("CanCheckWhetherCustomerIsForumModerator(): customer → {Customer}", customer);
        isForumModerator.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanCheckWhetherCustomerIsForumModerator()");
    }

    [Test]
    public async Task CanCheckWhetherCustomerIsGuest()
    {
		_logger.LogDebug(">>> Entering CanCheckWhetherCustomerIsGuest()");
        var customer = await _customerService.GetCustomerByEmailAsync("builtin@search_engine_record.com");
        var isGuest = await _customerService.IsGuestAsync(customer);
		_logger.LogDebug("CanCheckWhetherCustomerIsGuest(): isGuest → {IsGuest}", isGuest);
		_logger.LogDebug("CanCheckWhetherCustomerIsGuest(): customer → {Customer}", customer);
        isGuest.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanCheckWhetherCustomerIsGuest()");
    }

    [Test]
    public async Task CanCheckWhetherCustomerIsRegistered()
    {
		_logger.LogDebug(">>> Entering CanCheckWhetherCustomerIsRegistered()");
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);

		_logger.LogDebug("CanCheckWhetherCustomerIsRegistered(): customer → {Customer}", customer);
        var isRegistered = await _customerService.IsRegisteredAsync(customer);
		_logger.LogDebug("CanCheckWhetherCustomerIsRegistered(): isRegistered → {IsRegistered}", isRegistered);
        isRegistered.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanCheckWhetherCustomerIsRegistered()");
    }

    [Test]
    public async Task CanRemoveAddressAssignedAsBillingAddress()
    {
		_logger.LogDebug(">>> Entering CanRemoveAddressAssignedAsBillingAddress()");
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
        var addresses = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
		_logger.LogDebug("CanRemoveAddressAssignedAsBillingAddress(): addresses → {Addresses}", addresses);
		_logger.LogDebug("CanRemoveAddressAssignedAsBillingAddress(): customer → {Customer}", customer);

        addresses.Count.Should().Be(1);

        var address = addresses[0];

        await _customerService.InsertCustomerAddressAsync(customer, address);

        var addressesByCustomer = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
		_logger.LogDebug("CanRemoveAddressAssignedAsBillingAddress(): addressesByCustomer → {AddressesByCustomer}", addressesByCustomer);
        addressesByCustomer.Count.Should().Be(1);

        var billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
		_logger.LogDebug("CanRemoveAddressAssignedAsBillingAddress(): billingAddress → {BillingAddress}", billingAddress);
        billingAddress.Should().NotBeNull();

        billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
        billingAddress.Id.Should().Be(address.Id);

        await _customerService.RemoveCustomerAddressAsync(customer, address);

        addressesByCustomer = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
        var countAddresses = addressesByCustomer.Count;

        var billingAddressId = customer.BillingAddressId;

        await _customerService.InsertCustomerAddressAsync(customer, address);
        customer.BillingAddressId = address.Id;
		_logger.LogDebug("CanRemoveAddressAssignedAsBillingAddress(): customer.BillingAddressId → {Arg0}", customer.BillingAddressId);

        countAddresses.Should().Be(0);
        billingAddressId.Should().BeNull();
		_logger.LogDebug("<<< Exiting CanRemoveAddressAssignedAsBillingAddress()");
    }
}