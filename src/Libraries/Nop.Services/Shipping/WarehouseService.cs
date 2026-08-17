using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;

namespace Nop.Services.Shipping;

/// <summary>
/// Warehouse service
/// </summary>
public partial class WarehouseService : IWarehouseService
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IRepository<Warehouse> _warehouseRepository;

    #endregion

    #region Ctor

    public WarehouseService(IAddressService addressService,
        IRepository<Warehouse> warehouseRepository)
    {
		_logger.LogDebug(">>> Entering WarehouseService(addressService={AddressService},warehouseRepository={WarehouseRepository})", addressService, warehouseRepository);
        _addressService = addressService;
        _warehouseRepository = warehouseRepository;
		_logger.LogDebug("<<< Exiting WarehouseService(addressService={AddressService},warehouseRepository={WarehouseRepository})", addressService, warehouseRepository);
		_logger.LogDebug("WarehouseService(addressService={AddressService},warehouseRepository={WarehouseRepository}): _warehouseRepository → {_warehouseRepository}", addressService, warehouseRepository, _warehouseRepository);
		_logger.LogDebug("WarehouseService(addressService={AddressService},warehouseRepository={WarehouseRepository}): _addressService → {_addressService}", addressService, warehouseRepository, _addressService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a warehouse
    /// </summary>
    /// <param name="warehouse">The warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteWarehouseAsync(Warehouse warehouse)
    {
		_logger.LogDebug(">>> Entering DeleteWarehouseAsync(warehouse={Warehouse})", warehouse);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _warehouseRepository.DeleteAsync(warehouse);
		_logger.LogDebug("<<< Exiting DeleteWarehouseAsync(warehouse={Warehouse})", warehouse);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteWarehouseAsync(warehouse)={Warehouse}: external service call took {Arg1} ms", warehouse, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteWarehouseAsync(warehouse)={Warehouse}: external service call took {Arg1} ms", warehouse, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a warehouse
    /// </summary>
    /// <param name="warehouseId">The warehouse identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouse
    /// </returns>
    public virtual async Task<Warehouse> GetWarehouseByIdAsync(int warehouseId)
    {
		_logger.LogDebug(">>> Entering GetWarehouseByIdAsync(warehouseId={WarehouseId})", warehouseId);
		_logger.LogDebug("<<< Exiting GetWarehouseByIdAsync(warehouseId={WarehouseId})", warehouseId);
        return await _warehouseRepository.GetByIdAsync(warehouseId, _ => default);
    }

    /// <summary>
    /// Gets all warehouses
    /// </summary>
    /// <param name="name">Warehouse name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warehouses
    /// </returns>
    public virtual async Task<IList<Warehouse>> GetAllWarehousesAsync(string name = null)
    {
		_logger.LogDebug(">>> Entering GetAllWarehousesAsync(name={Name})", name);
        var warehouses = await _warehouseRepository.GetAllAsync(query =>
            from wh in query
			using Microsoft.Extensions.Logging;
            orderby wh.Name
			_logger.LogDebug("GetAllWarehousesAsync(name={Name}): warehouses → {Warehouses}", name, warehouses);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            select wh, _ => default);

		_logger.LogInformation("GetAllWarehousesAsync(name)={Name}: find query executed in {Arg1} ms", name, __logStart.ElapsedMilliseconds);
        if (!string.IsNullOrEmpty(name))
            warehouses = warehouses.Where(wh => wh.Name.Contains(name)).ToList();

        return warehouses;
    }

    /// <summary>
    /// Inserts a warehouse
    /// </summary>
    /// <param name="warehouse">Warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertWarehouseAsync(Warehouse warehouse)
    {
		private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        await _warehouseRepository.InsertAsync(warehouse);
		_logger.LogDebug(">>> Entering InsertWarehouseAsync(warehouse={Warehouse})", warehouse);
		_logger.LogDebug("<<< Exiting InsertWarehouseAsync(warehouse={Warehouse})", warehouse);
    }

    /// <summary>
    /// Updates the warehouse
    /// </summary>
    /// <param name="warehouse">Warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateWarehouseAsync(Warehouse warehouse)
    {
		_logger.LogDebug(">>> Entering UpdateWarehouseAsync(warehouse={Warehouse})", warehouse);
        await _warehouseRepository.UpdateAsync(warehouse);
		_logger.LogDebug("<<< Exiting UpdateWarehouseAsync(warehouse={Warehouse})", warehouse);
    }

    /// <summary>
    /// Get the nearest warehouse for the specified address
    /// </summary>
    /// <param name="address">Address</param>
    /// <param name="warehouses">List of warehouses, if null all warehouses are used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    public virtual async Task<Warehouse> GetNearestWarehouseAsync(Address address, IList<Warehouse> warehouses = null)
    {
		_logger.LogDebug(">>> Entering GetNearestWarehouseAsync(address={Address},warehouses={Warehouses})", address, warehouses);
        warehouses ??= await GetAllWarehousesAsync();

        //no address specified. return any
        if (address == null)
            return warehouses.FirstOrDefault();

        //of course, we should use some better logic to find nearest warehouse,
        //but we don't have a built-in geographic database which supports "distance" functionality
        //that's why we simply look for exact matches

        //find by country
        var matchedByCountry = new List<Warehouse>();
		_logger.LogDebug("GetNearestWarehouseAsync(address={Address},warehouses={Warehouses}): matchedByCountry → {MatchedByCountry}", address, warehouses, matchedByCountry);
        foreach (var warehouse in warehouses)
        {
            var warehouseAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);
			_logger.LogDebug("GetNearestWarehouseAsync(address={Address},warehouses={Warehouses}): warehouseAddress → {WarehouseAddress}", address, warehouses, warehouseAddress);
            if (warehouseAddress == null)
                continue;

            if (warehouseAddress.CountryId == address.CountryId)
                matchedByCountry.Add(warehouse);
        }
        //no country matches. return any
        if (!matchedByCountry.Any())
            return warehouses.FirstOrDefault();

        //find by state
        var matchedByState = new List<Warehouse>();
		_logger.LogDebug("GetNearestWarehouseAsync(address={Address},warehouses={Warehouses}): matchedByState → {MatchedByState}", address, warehouses, matchedByState);
        foreach (var warehouse in matchedByCountry)
        {
            var warehouseAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);
            if (warehouseAddress == null)
                continue;

            if (warehouseAddress.StateProvinceId == address.StateProvinceId)
                matchedByState.Add(warehouse);
        }

        if (matchedByState.Any())
		_logger.LogDebug("<<< Exiting GetNearestWarehouseAsync(address={Address},warehouses={Warehouses})", address, warehouses);
            return matchedByState.FirstOrDefault();

        //no state matches. return any
        return matchedByCountry.FirstOrDefault();
    }

    #endregion
}