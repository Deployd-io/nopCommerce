using FluentAssertions;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Shipping;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class WarehouseServiceTests : ServiceTest<Warehouse>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IAddressService _addressService;
    private IWarehouseService _warehouseService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _addressService = GetService<IAddressService>();
        _warehouseService = GetService<IWarehouseService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _warehouseService → {_warehouseService}", _warehouseService);
		_logger.LogDebug("SetUp(): _addressService → {_addressService}", _addressService);
    }

    [Test]
    public async Task CanGetAllWarehouses()
    {
		_logger.LogDebug(">>> Entering CanGetAllWarehouses()");
        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        warehouses.Any().Should().BeTrue();
		_logger.LogDebug("CanGetAllWarehouses(): warehouses → {Warehouses}", warehouses);
        warehouses.Count.Should().Be(2);

        warehouses = await _warehouseService.GetAllWarehousesAsync("Warehouse 1 (New York)");
        warehouses.Any().Should().BeTrue();
        warehouses.Count.Should().Be(1);
		_logger.LogDebug("<<< Exiting CanGetAllWarehouses()");
    }

    [Test]
    public async Task CanGetNearestWarehouse()
    {
		_logger.LogDebug(">>> Entering CanGetNearestWarehouse()");
        var address = await _addressService.GetAddressByIdAsync(1);

		_logger.LogDebug("CanGetNearestWarehouse(): address → {Address}", address);
        var warehouses = await _warehouseService.GetNearestWarehouseAsync(address);
		_logger.LogDebug("CanGetNearestWarehouse(): warehouses → {Warehouses}", warehouses);
        warehouses.Should().NotBeNull();
        warehouses.Name.Should().BeEquivalentTo("Warehouse 1 (New York)");

        address = await _addressService.GetAddressByIdAsync(2);

        warehouses = await _warehouseService.GetNearestWarehouseAsync(address);
        warehouses.Should().NotBeNull();
        warehouses.Name.Should().BeEquivalentTo("Warehouse 2 (Los Angeles)");

        address = await _addressService.GetAddressByIdAsync(3);

        warehouses = await _warehouseService.GetNearestWarehouseAsync(address);
        warehouses.Should().NotBeNull();

        warehouses = await _warehouseService.GetNearestWarehouseAsync(null);
        warehouses.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanGetNearestWarehouse()");
    }

    protected override CrudData<Warehouse> CrudData
    {
        get
        {
            var baseEntity = new Warehouse
            {
                Name = "Test shipping method",
                AddressId = 1,
                AdminComment = "Test admin comment"
            };

            var updatedEntity = new Warehouse
            {
                Name = "New test shipping method",
                AddressId = 1,
                AdminComment = "Test admin comment"
            };

            return new CrudData<Warehouse>
            {
                BaseEntity = baseEntity,
                UpdatedEntity = updatedEntity,
                Insert = _warehouseService.InsertWarehouseAsync,
                Update = _warehouseService.UpdateWarehouseAsync,
                Delete = _warehouseService.DeleteWarehouseAsync,
                GetById = _warehouseService.GetWarehouseByIdAsync,
                IsEqual = (first, second) => first.AddressId == second.AddressId && first.Name.Equals(second.Name) && first.AdminComment.Equals(second.AdminComment)
            };
        }
    }
}
