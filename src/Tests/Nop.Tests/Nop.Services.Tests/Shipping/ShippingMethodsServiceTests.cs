using FluentAssertions;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingMethodsServiceTests : ServiceTest<ShippingMethod>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IShippingMethodsService _shippingMethodsService;
    private ShippingMethodCountryMapping _shippingMethodCountryMapping;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _shippingMethodCountryMapping = new ShippingMethodCountryMapping { CountryId = 1, ShippingMethodId = 1 };

		_logger.LogDebug("SetUp(): _shippingMethodCountryMapping → {_shippingMethodCountryMapping}", _shippingMethodCountryMapping);
        _shippingMethodsService = GetService<IShippingMethodsService>();
        await _shippingMethodsService.InsertShippingMethodCountryMappingAsync(_shippingMethodCountryMapping);
		_logger.LogDebug("SetUp(): _shippingMethodsService → {_shippingMethodsService}", _shippingMethodsService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _shippingMethodsService.DeleteShippingMethodCountryMappingAsync(_shippingMethodCountryMapping);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanGetAllShippingMethods()
    {
		_logger.LogDebug(">>> Entering CanGetAllShippingMethods()");
        var methods = await _shippingMethodsService.GetAllShippingMethodsAsync();

		_logger.LogDebug("CanGetAllShippingMethods(): methods → {Methods}", methods);
        methods.Any().Should().BeTrue();
        methods.Count.Should().Be(3);

        methods = await _shippingMethodsService.GetAllShippingMethodsAsync(1);
        methods.Any().Should().BeTrue();
        methods.Count.Should().Be(2);
		_logger.LogDebug("<<< Exiting CanGetAllShippingMethods()");
    }

    [Test]
    public async Task CanGetShippingMethodCountryMapping()
    {
		_logger.LogDebug(">>> Entering CanGetShippingMethodCountryMapping()");
        var mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(1, 1);
        mapping.Any().Should().BeTrue();
		_logger.LogDebug("CanGetShippingMethodCountryMapping(): mapping → {Mapping}", mapping);

        mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(1, 2);
        mapping.Any().Should().BeFalse();

        mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(2, 1);
        mapping.Any().Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanGetShippingMethodCountryMapping()");
    }

    [Test]
    public async Task CanCheckCountryRestrictionExists()
    {
		_logger.LogDebug(">>> Entering CanCheckCountryRestrictionExists()");
        var isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(1), 1);
        isExists.Should().BeTrue();
		_logger.LogDebug("CanCheckCountryRestrictionExists(): isExists → {IsExists}", isExists);

        isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(1), 2);
        isExists.Should().BeFalse();

        isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(2), 1);
        isExists.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanCheckCountryRestrictionExists()");
    }

    protected override CrudData<ShippingMethod> CrudData
    {
        get
        {
            var baseEntity = new ShippingMethod
            {
                Name = "Test shipping method",
                Description = "Test shipping method description",
                DisplayOrder = 1
            };

            var updatedEntity = new ShippingMethod
            {
                Name = "Test shipping method",
                Description = "Test shipping method description",
                DisplayOrder = 2
            };

            return new CrudData<ShippingMethod>
            {
                BaseEntity = baseEntity,
                UpdatedEntity = updatedEntity,
                Insert = _shippingMethodsService.InsertShippingMethodAsync,
                Update = _shippingMethodsService.UpdateShippingMethodAsync,
                Delete = _shippingMethodsService.DeleteShippingMethodAsync,
                GetById = _shippingMethodsService.GetShippingMethodByIdAsync,
                IsEqual = (first, second) => first.DisplayOrder == second.DisplayOrder && first.Name.Equals(second.Name) && first.Description.Equals(second.Description)
            };
        }
    }
}
