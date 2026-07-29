using FluentAssertions;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingMethodsServiceTests : ServiceTest<ShippingMethod>
{
    private IShippingMethodsService _shippingMethodsService;
    private ShippingMethodCountryMapping _shippingMethodCountryMapping;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _shippingMethodCountryMapping = new ShippingMethodCountryMapping { CountryId = 1, ShippingMethodId = 1 };

		logger.LogDebug("SetUp(): _shippingMethodCountryMapping → {_shippingMethodCountryMapping}", _shippingMethodCountryMapping);
        _shippingMethodsService = GetService<IShippingMethodsService>();
		logger.LogDebug("SetUp(): _shippingMethodsService → {_shippingMethodsService}", _shippingMethodsService);
        await _shippingMethodsService.InsertShippingMethodCountryMappingAsync(_shippingMethodCountryMapping);
		logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        await _shippingMethodsService.DeleteShippingMethodCountryMappingAsync(_shippingMethodCountryMapping);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanGetAllShippingMethods()
    {
		logger.LogDebug(">>> Entering CanGetAllShippingMethods()");
        var methods = await _shippingMethodsService.GetAllShippingMethodsAsync();

		logger.LogDebug("CanGetAllShippingMethods(): methods → {Methods}", methods);
        methods.Any().Should().BeTrue();
        methods.Count.Should().Be(3);

        methods = await _shippingMethodsService.GetAllShippingMethodsAsync(1);
        methods.Any().Should().BeTrue();
        methods.Count.Should().Be(2);
		logger.LogDebug("<<< Exiting CanGetAllShippingMethods()");
    }

    [Test]
    public async Task CanGetShippingMethodCountryMapping()
    {
		logger.LogDebug(">>> Entering CanGetShippingMethodCountryMapping()");
        var mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(1, 1);
        mapping.Any().Should().BeTrue();
		logger.LogDebug("CanGetShippingMethodCountryMapping(): mapping → {Mapping}", mapping);

        mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(1, 2);
        mapping.Any().Should().BeFalse();

        mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(2, 1);
        mapping.Any().Should().BeFalse();
		logger.LogDebug("<<< Exiting CanGetShippingMethodCountryMapping()");
    }

    [Test]
    public async Task CanCheckCountryRestrictionExists()
    {
		logger.LogDebug(">>> Entering CanCheckCountryRestrictionExists()");
        var isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(1), 1);
        isExists.Should().BeTrue();
		logger.LogDebug("CanCheckCountryRestrictionExists(): isExists → {IsExists}", isExists);

        isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(1), 2);
        isExists.Should().BeFalse();

        isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(2), 1);
        isExists.Should().BeFalse();
		logger.LogDebug("<<< Exiting CanCheckCountryRestrictionExists()");
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
