using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Shipping;

/// <summary>
/// Warehouse service
/// </summary>
public partial class WarehouseService : IWarehouseService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IRepository<Warehouse> _warehouseRepository;

    #endregion

    #region Ctor

    public WarehouseService(IAddressService addressService,
        IRepository<Warehouse> warehouseRepository)
    {
		_logger.LogError(">>> Entering WarehouseService(addressService={AddressService},warehouseRepository={WarehouseRepository})", addressService, warehouseRepository);
        _addressService = addressService;
        _warehouseRepository = warehouseRepository;
		_logger.LogDebug("WarehouseService(addressService={AddressService},warehouseRepository={WarehouseRepository}): _warehouseRepository → {_warehouseRepository}", addressService, warehouseRepository, _warehouseRepository);
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
		_logger.LogError(">>> Entering DeleteWarehouseAsync(warehouse={Warehouse})", warehouse);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _warehouseRepository.DeleteAsync(warehouse);
		_logger.LogError("DeleteWarehouseAsync(warehouse)={Warehouse}: external service call took {Arg1} ms", warehouse, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteWarehouseAsync(warehouse={Warehouse})", warehouse);
    }

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
		_logger.LogError(">>> Entering GetWarehouseByIdAsync(warehouseId={WarehouseId})", warehouseId);
		_logger.LogError("GetWarehouseByIdAsync(warehouseId)={WarehouseId}: find query executing", warehouseId);
    _logger.LogWarning("<<< Exiting GetWarehouseByIdAsync(warehouseId={WarehouseId})", warehouseId);
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
		_logger.LogError(">>> Entering GetAllWarehousesAsync(name={Name})", name);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var warehouses = await _warehouseRepository.GetAllAsync(query =>
            from wh in query
            orderby wh.Name
            select wh, _ => default);
			_logger.LogError("GetAllWarehousesAsync(name)={Name}: find query executed in {Arg1} ms", name, __logStart.ElapsedMilliseconds);

        if (!string.IsNullOrEmpty(name))
            warehouses = warehouses.Where(wh => wh.Name.Contains(name)).ToList();
			_logger.LogError("GetAllWarehousesAsync(name={Name}): warehouses → {Warehouses}", name, warehouses);

		_logger.LogWarning("<<< Exiting GetAllWarehousesAsync(name={Name})", name);
        return warehouses;
    }

    /// <summary>
    /// Inserts a warehouse
    /// </summary>
    /// <param name="warehouse">Warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertWarehouseAsync(Warehouse warehouse)
    {
		_logger.LogError(">>> Entering InsertWarehouseAsync(warehouse={Warehouse})", warehouse);
        await _warehouseRepository.InsertAsync(warehouse);
		_logger.LogWarning("<<< Exiting InsertWarehouseAsync(warehouse={Warehouse})", warehouse);
    }

    /// <summary>
    /// Updates the warehouse
    /// </summary>
    /// <param name="warehouse">Warehouse</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateWarehouseAsync(Warehouse warehouse)
    {
		_logger.LogError(">>> Entering UpdateWarehouseAsync(warehouse={Warehouse})", warehouse);
        await _warehouseRepository.UpdateAsync(warehouse);
		_logger.LogWarning("<<< Exiting UpdateWarehouseAsync(warehouse={Warehouse})", warehouse);
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
		_logger.LogError(">>> Entering GetNearestWarehouseAsync(address={Address},warehouses={Warehouses})", address, warehouses);
        warehouses ??= await GetAllWarehousesAsync();

        //no address specified. return any
        if (address == null)
            return warehouses.FirstOrDefault();

        //of course, we should use some better logic to find nearest warehouse,
        //but we don't have a built-in geographic database which supports "distance" functionality
        //that's why we simply look for exact matches

        //find by country
        var matchedByCountry = new List<Warehouse>();
		_logger.LogError("GetNearestWarehouseAsync(address={Address},warehouses={Warehouses}): matchedByCountry → {MatchedByCountry}", address, warehouses, matchedByCountry);
        foreach (var warehouse in warehouses)
        {
            var warehouseAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);
			_logger.LogError("GetNearestWarehouseAsync(address={Address},warehouses={Warehouses}): warehouseAddress → {WarehouseAddress}", address, warehouses, warehouseAddress);
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
		_logger.LogError("GetNearestWarehouseAsync(address={Address},warehouses={Warehouses}): matchedByState → {MatchedByState}", address, warehouses, matchedByState);
        foreach (var warehouse in matchedByCountry)
        {
            var warehouseAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);
            if (warehouseAddress == null)
                continue;

            if (warehouseAddress.StateProvinceId == address.StateProvinceId)
                matchedByState.Add(warehouse);
        }

        if (matchedByState.Any())
            return matchedByState.FirstOrDefault();

        //no state matches. return any
        return matchedByCountry.FirstOrDefault();
    }

    #endregion
}