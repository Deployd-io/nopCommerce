using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Customers;

[TestFixture]
public class CustomerRegistrationServiceTests : ServiceTest
{
    private ICustomerService _customerService;
    private IEncryptionService _encryptionService;
    private ICustomerRegistrationService _customerRegistrationService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _customerService = GetService<ICustomerService>();
        _encryptionService = GetService<IEncryptionService>();
		logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
		logger.LogDebug("SetUp(): _encryptionService → {_encryptionService}", _encryptionService);
        _customerRegistrationService = GetService<ICustomerRegistrationService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _customerRegistrationService → {_customerRegistrationService}", _customerRegistrationService);
    }

    private async Task<Customer> CreateCustomerAsync(PasswordFormat passwordFormat, bool isRegistered = true)
    {
		logger.LogDebug(">>> Entering CreateCustomerAsync(passwordFormat={PasswordFormat},isRegistered={IsRegistered})", passwordFormat, isRegistered);
        var customer = new Customer
        {
            Username = "test@test.com",
            Email = "test@test.com",
            Active = true
        };

        await _customerService.InsertCustomerAsync(customer);

        var password = "password";
        if (passwordFormat == PasswordFormat.Encrypted)
			logger.LogWarning("CreateCustomerAsync(passwordFormat={PasswordFormat},isRegistered={IsRegistered}): password → {Password}", passwordFormat, isRegistered, password);
            password = _encryptionService.EncryptText(password);

        await _customerService.InsertCustomerPasswordAsync(new CustomerPassword
        {
            CustomerId = customer.Id,
            PasswordFormat = passwordFormat,
            Password = password,
            CreatedOnUtc = DateTime.UtcNow
        });

        if (isRegistered)
        {
            var registeredRole = await _customerService
                .GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
            {
                CustomerId = customer.Id,
                CustomerRoleId = registeredRole.Id
            });
        }

		logger.LogDebug("<<< Exiting CreateCustomerAsync(passwordFormat={PasswordFormat},isRegistered={IsRegistered})", passwordFormat, isRegistered);
        return customer;
    }

    private async Task DeleteCustomerAsync(Customer customer)
    {
        customer.Username = customer.Email = string.Empty;
        customer.Active = false;
        await _customerService.UpdateCustomerAsync(customer);
        await _customerService.DeleteCustomerAsync(customer);
    }

    [Test]
    public async Task EnsureOnlyRegisteredCustomersCanLogin()
    {
		logger.LogDebug(">>> Entering EnsureOnlyRegisteredCustomersCanLogin()");
        var result = await _customerRegistrationService.ValidateCustomerAsync(NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminPassword);
        result.Should().Be(CustomerLoginResults.Successful);
		logger.LogDebug("EnsureOnlyRegisteredCustomersCanLogin(): result → {Result}", result);

        var customer = await CreateCustomerAsync(PasswordFormat.Clear, false);

		logger.LogDebug("EnsureOnlyRegisteredCustomersCanLogin(): customer → {Customer}", customer);
        result = await _customerRegistrationService.ValidateCustomerAsync("test@test.com", "password");
        await DeleteCustomerAsync(customer);

        result.Should().Be(CustomerLoginResults.NotRegistered);
		logger.LogDebug("<<< Exiting EnsureOnlyRegisteredCustomersCanLogin()");
    }

    [Test]
    public async Task CanValidateHashedPassword()
    {
		logger.LogDebug(">>> Entering CanValidateHashedPassword()");
        var result = await _customerRegistrationService.ValidateCustomerAsync(NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminPassword);
        result.Should().Be(CustomerLoginResults.Successful);
		logger.LogDebug("<<< Exiting CanValidateHashedPassword()");
		logger.LogDebug("CanValidateHashedPassword(): result → {Result}", result);
    }

    [Test]
    public async Task CanValidateClearPassword()
    {
		logger.LogDebug(">>> Entering CanValidateClearPassword()");
        var customer = await CreateCustomerAsync(PasswordFormat.Clear);

		logger.LogDebug("CanValidateClearPassword(): customer → {Customer}", customer);
        var result = await _customerRegistrationService.ValidateCustomerAsync("test@test.com", "password");
        await DeleteCustomerAsync(customer);
		logger.LogDebug("CanValidateClearPassword(): result → {Result}", result);

        result.Should().Be(CustomerLoginResults.Successful);
		logger.LogDebug("<<< Exiting CanValidateClearPassword()");
    }

    [Test]
    public async Task CanValidateEncryptedPassword()
    {
		logger.LogDebug(">>> Entering CanValidateEncryptedPassword()");
        var customer = await CreateCustomerAsync(PasswordFormat.Encrypted);

		logger.LogDebug("CanValidateEncryptedPassword(): customer → {Customer}", customer);
        var result = await _customerRegistrationService.ValidateCustomerAsync("test@test.com", "password");
        await DeleteCustomerAsync(customer);
		logger.LogDebug("CanValidateEncryptedPassword(): result → {Result}", result);

        result.Should().Be(CustomerLoginResults.Successful);
		logger.LogDebug("<<< Exiting CanValidateEncryptedPassword()");
    }

    [Test]
    public async Task CanChangePassword()
    {
		logger.LogDebug(">>> Entering CanChangePassword()");
        var customer = await CreateCustomerAsync(PasswordFormat.Encrypted);

		logger.LogDebug("CanChangePassword(): customer → {Customer}", customer);
        var request = new ChangePasswordRequest("test@test.com", true, PasswordFormat.Clear, "password", "password");
        var unSuccess = await _customerRegistrationService.ChangePasswordAsync(request);
		logger.LogDebug("CanChangePassword(): request → {Request}", request);
		logger.LogDebug("CanChangePassword(): unSuccess → {UnSuccess}", unSuccess);

        request = new ChangePasswordRequest("test@test.com", true, PasswordFormat.Hashed, "newpassword", "password");
        var success = await _customerRegistrationService.ChangePasswordAsync(request);
		logger.LogDebug("CanChangePassword(): success → {Success}", success);

        unSuccess.Success.Should().BeFalse();
        success.Success.Should().BeTrue();

        await DeleteCustomerAsync(customer);
		logger.LogDebug("<<< Exiting CanChangePassword()");
    }
}