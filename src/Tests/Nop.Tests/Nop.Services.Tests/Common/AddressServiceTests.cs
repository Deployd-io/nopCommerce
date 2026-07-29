using FluentAssertions;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
public class AddressServiceTests : ServiceTest
{
    private IAddressService _addressService;
    private ICountryService _countryService;
    private ILocalizationService _localizationService;
    private IStateProvinceService _stateProvinceService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _localizationService = GetService<ILocalizationService>();
        _countryService = GetService<ICountryService>();
		logger.LogDebug("SetUp(): _localizationService → {_localizationService}", _localizationService);
		logger.LogDebug("SetUp(): _countryService → {_countryService}", _countryService);
        _stateProvinceService = GetService<IStateProvinceService>();

		logger.LogDebug("SetUp(): _stateProvinceService → {_stateProvinceService}", _stateProvinceService);
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            //zipPostalCode, Country, StateProvince 
            ["Address.LineFormat"] = "{6}{0}{1}"
        });

        _addressService = GetService<IAddressService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _addressService → {_addressService}", _addressService);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Address.LineFormat"] = "{0}{1}{2}{3}{4}{5}{6}"
        });
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanFormatAddressAsync()
    {
		logger.LogDebug(">>> Entering CanFormatAddressAsync()");
        var address = await _addressService.GetAddressByIdAsync(1);

		logger.LogDebug("CanFormatAddressAsync(): address → {Address}", address);
        var country = await _countryService.GetCountryByAddressAsync(address);
        var countryName = country != null ? await _localizationService.GetLocalizedAsync(country, x => x.Name, 1) : string.Empty;
		logger.LogDebug("CanFormatAddressAsync(): country → {Country}", country);
		logger.LogDebug("CanFormatAddressAsync(): countryName → {CountryName}", countryName);
        var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(address);
        var stateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, 1) : string.Empty;
		logger.LogDebug("CanFormatAddressAsync(): stateProvince → {StateProvince}", stateProvince);
		logger.LogDebug("CanFormatAddressAsync(): stateProvinceName → {StateProvinceName}", stateProvinceName);

        var (addressLine, addressFields) = await _addressService.FormatAddressAsync(address, 1);

        addressFields[0].Value.Should().Be(address.ZipPostalCode);
        addressFields[1].Value.Should().Be(countryName);
        addressFields[2].Value.Should().Be(stateProvinceName);

        addressLine.Should().Be($"{address.ZipPostalCode}, {countryName}, {stateProvinceName}");
		logger.LogDebug("<<< Exiting CanFormatAddressAsync()");
    }
}
