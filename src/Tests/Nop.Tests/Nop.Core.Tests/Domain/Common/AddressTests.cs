using FluentAssertions;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Core.Tests.Domain.Common;

[TestFixture]
public class AddressTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IAddressService _addressService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _addressService = GetService<IAddressService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _addressService → {_addressService}", _addressService);
    }

    [Test]
    public void CanCloneAddress()
    {
		_logger.LogDebug(">>> Entering CanCloneAddress()");
        var address = new Address
        {
            Id = 1,
            FirstName = "FirstName 1",
            LastName = "LastName 1",
            Email = "Email 1",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        var newAddress = _addressService.CloneAddress(address);
		_logger.LogDebug("CanCloneAddress(): newAddress → {NewAddress}", newAddress);
        newAddress.Should().NotBeNull();
        newAddress.Id.Should().Be(0);
        newAddress.FirstName.Should().Be("FirstName 1");
        newAddress.LastName.Should().Be("LastName 1");
        newAddress.Email.Should().Be("Email 1");
        newAddress.Company.Should().Be("Company 1");
        newAddress.City.Should().Be("City 1");
        newAddress.County.Should().Be("County 1");
        newAddress.Address1.Should().Be("Address1");
        newAddress.Address2.Should().Be("Address2");
        newAddress.ZipPostalCode.Should().Be("ZipPostalCode 1");
        newAddress.PhoneNumber.Should().Be("PhoneNumber 1");
        newAddress.FaxNumber.Should().Be("FaxNumber 1");
        newAddress.CreatedOnUtc.Should().Be(new DateTime(2010, 01, 01));
        newAddress.CountryId.Should().Be(3);
        newAddress.StateProvinceId.Should().Be(4);
		_logger.LogDebug("<<< Exiting CanCloneAddress()");
    }

    [Test]
    public async Task CanValidateAddressAsync()
    {
		_logger.LogDebug(">>> Entering CanValidateAddressAsync()");
        //Valid address
        var validAddress = new Address
        {
            Id = 1,
            FirstName = "FirstName 1",
            LastName = "LastName 1",
            Email = "Email 1",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        var isAddressValid = await _addressService.IsAddressValidAsync(validAddress);
		_logger.LogDebug("CanValidateAddressAsync(): isAddressValid → {IsAddressValid}", isAddressValid);
        isAddressValid.Should().BeTrue();

        //InValid address 1 - empty FirstName
        var invalidAddress1 = new Address
        {
            Id = 1,
            FirstName = "",
            LastName = "LastName 1",
            Email = "Email 1",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        isAddressValid = await _addressService.IsAddressValidAsync(invalidAddress1);
        isAddressValid.Should().BeFalse();

        //InValid address 2 - empty LastName
        var invalidAddress2 = new Address
        {
            Id = 1,
            FirstName = "FirstName 1",
            LastName = "",
            Email = "Email 1",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        isAddressValid = await _addressService.IsAddressValidAsync(invalidAddress2);
        isAddressValid.Should().BeFalse();

        //InValid address 3 - empty Email
        var invalidAddress3 = new Address
        {
            Id = 1,
            FirstName = "FirstName 1",
            LastName = "LastName 1",
            Email = "",
            Company = "Company 1",
            CountryId = 3,
            StateProvinceId = 4,
            City = "City 1",
            County = "County 1",
            Address1 = "Address1",
            Address2 = "Address2",
            ZipPostalCode = "ZipPostalCode 1",
            PhoneNumber = "PhoneNumber 1",
            FaxNumber = "FaxNumber 1",
            CreatedOnUtc = new DateTime(2010, 01, 01),
        };

        isAddressValid = await _addressService.IsAddressValidAsync(invalidAddress3);
        isAddressValid.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanValidateAddressAsync()");
    }
}