using Nop.Core.Domain.Stores;
using Nop.Data;

namespace Nop.Services.Stores;

/// <summary>
/// Store service
/// </summary>
public partial class StoreService : IStoreService
{
    #region Fields

    protected readonly IRepository<Store> _storeRepository;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public StoreService(IRepository<Store> storeRepository)
    {
		logger.LogDebug(">>> Entering StoreService(storeRepository={StoreRepository})", storeRepository);
        _storeRepository = storeRepository;
		logger.LogDebug("<<< Exiting StoreService(storeRepository={StoreRepository})", storeRepository);
		logger.LogDebug("StoreService(storeRepository={StoreRepository}): _storeRepository → {_storeRepository}", storeRepository, _storeRepository);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Parse comma-separated Hosts
    /// </summary>
    /// <param name="store">Store</param>
    /// <returns>Comma-separated hosts</returns>
    protected virtual string[] ParseHostValues(Store store)
    {
		logger.LogDebug(">>> Entering ParseHostValues(store={Store})", store);
        ArgumentNullException.ThrowIfNull(store);

        var parsedValues = new List<string>();
        if (string.IsNullOrEmpty(store.Hosts))
			logger.LogDebug("ParseHostValues(store={Store}): parsedValues → {ParsedValues}", store, parsedValues);
            return parsedValues.ToArray();

        var hosts = store.Hosts.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

		logger.LogDebug("ParseHostValues(store={Store}): hosts → {Hosts}", store, hosts);
        foreach (var host in hosts)
        {
            var tmp = host.Trim();
            if (!string.IsNullOrEmpty(tmp))
                parsedValues.Add(tmp);
        }

		logger.LogDebug("<<< Exiting ParseHostValues(store={Store})", store);
        return parsedValues.ToArray();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a store
    /// </summary>
    /// <param name="store">Store</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteStoreAsync(Store store)
    {
		logger.LogDebug(">>> Entering DeleteStoreAsync(store={Store})", store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(store);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allStores = await GetAllStoresAsync();
        if (allStores.Count == 1)
			logger.LogDebug("<<< Exiting DeleteStoreAsync(store={Store})", store);
			logger.LogDebug("DeleteStoreAsync(store={Store}): allStores → {AllStores}", store, allStores);
            throw new Exception("You cannot delete the only configured store");

        await _storeRepository.DeleteAsync(store);
		logger.LogInformation("DeleteStoreAsync(store)={Store}: external service call took {Arg1} ms", store, __logStart.ElapsedMilliseconds);
		logger.LogInformation("DeleteStoreAsync(store)={Store}: delete query executed in {Arg1} ms", store, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Gets all stores
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the stores
    /// </returns>
    public virtual async Task<IList<Store>> GetAllStoresAsync()
    {
		logger.LogDebug(">>> Entering GetAllStoresAsync()");
        return await _storeRepository.GetAllAsync(query =>
        {
            return from s in query orderby s.DisplayOrder, s.Id select s;
        }, _ => default, includeDeleted: false);
		logger.LogDebug("<<< Exiting GetAllStoresAsync()");
    }

    /// <summary>
    /// Gets all stores
    /// </summary>
    /// <returns>
    /// The stores
    /// </returns>
    public virtual IList<Store> GetAllStores()
    {
		logger.LogDebug(">>> Entering GetAllStores()");
        return _storeRepository.GetAll(query =>
        {
            return from s in query orderby s.DisplayOrder, s.Id select s;
        }, _ => default, includeDeleted: false);
    }

    /// <summary>
    /// Gets a store 
    /// </summary>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store
    /// </returns>
    public virtual async Task<Store> GetStoreByIdAsync(int storeId)
    {
		logger.LogDebug(">>> Entering GetStoreByIdAsync(storeId={StoreId})", storeId);
		logger.LogDebug("<<< Exiting GetStoreByIdAsync(storeId={StoreId})", storeId);
        return await _storeRepository.GetByIdAsync(storeId, cache => default, false);
    }

    /// <summary>
    /// Inserts a store
    /// </summary>
    /// <param name="store">Store</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertStoreAsync(Store store)
    {
		logger.LogDebug(">>> Entering InsertStoreAsync(store={Store})", store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _storeRepository.InsertAsync(store);
		logger.LogDebug("<<< Exiting InsertStoreAsync(store={Store})", store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertStoreAsync(store)={Store}: save query executed in {Arg1} ms", store, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertStoreAsync(store)={Store}: save query executed in {Arg1} ms", store, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the store
    /// </summary>
    /// <param name="store">Store</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateStoreAsync(Store store)
    {
		logger.LogDebug(">>> Entering UpdateStoreAsync(store={Store})", store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _storeRepository.UpdateAsync(store);
		logger.LogDebug("<<< Exiting UpdateStoreAsync(store={Store})", store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateStoreAsync(store)={Store}: update query executed in {Arg1} ms", store, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateStoreAsync(store)={Store}: update query executed in {Arg1} ms", store, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the store
    /// </summary>
    /// <param name="store">Store</param>
    public virtual void UpdateStore(Store store)
    {
		logger.LogDebug(">>> Entering UpdateStore(store={Store})", store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        _storeRepository.Update(store);
		logger.LogDebug("<<< Exiting UpdateStore(store={Store})", store);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateStore(store)={Store}: update query executed in {Arg1} ms", store, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateStore(store)={Store}: update query executed in {Arg1} ms", store, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Indicates whether a store contains a specified host
    /// </summary>
    /// <param name="store">Store</param>
    /// <param name="host">Host</param>
    /// <returns>true - contains, false - no</returns>
    public virtual bool ContainsHostValue(Store store, string host)
    {
		logger.LogDebug(">>> Entering ContainsHostValue(store={Store},host={Host})", store, host);
        ArgumentNullException.ThrowIfNull(store);

        if (string.IsNullOrEmpty(host))
            return false;

        var contains = ParseHostValues(store).Any(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase));

		logger.LogDebug("ContainsHostValue(store={Store},host={Host}): contains → {Contains}", store, host, contains);
		logger.LogDebug("<<< Exiting ContainsHostValue(store={Store},host={Host})", store, host);
        return contains;
    }

    /// <summary>
    /// Returns a list of names of not existing stores
    /// </summary>
    /// <param name="storeIdsNames">The names and/or IDs of the store to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of names and/or IDs not existing stores
    /// </returns>
    public virtual async Task<string[]> GetNotExistingStoresAsync(string[] storeIdsNames)
    {
		logger.LogDebug(">>> Entering GetNotExistingStoresAsync(storeIdsNames={StoreIdsNames})", storeIdsNames);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(storeIdsNames);

        var query = _storeRepository.Table;
        var queryFilter = storeIdsNames.Distinct().ToArray();
		logger.LogDebug("GetNotExistingStoresAsync(storeIdsNames={StoreIdsNames}): query → {Query}", storeIdsNames, query);
		logger.LogDebug("GetNotExistingStoresAsync(storeIdsNames={StoreIdsNames}): queryFilter → {QueryFilter}", storeIdsNames, queryFilter);
        //filtering by name
        var filter = await query.Select(store => store.Name)
            .Where(store => queryFilter.Contains(store))
            .ToListAsync();
			logger.LogInformation("GetNotExistingStoresAsync(storeIdsNames)={StoreIdsNames}: database query executed in {Arg1} ms", storeIdsNames, __logStart.ElapsedMilliseconds);
        queryFilter = queryFilter.Except(filter).ToArray();

        //if some names not found
        if (!queryFilter.Any())
            return queryFilter.ToArray();

        //filtering by IDs
        filter = await query.Select(store => store.Id.ToString())
            .Where(store => queryFilter.Contains(store))
            .ToListAsync();
        queryFilter = queryFilter.Except(filter).ToArray();

		logger.LogDebug("<<< Exiting GetNotExistingStoresAsync(storeIdsNames={StoreIdsNames})", storeIdsNames);
        return queryFilter.ToArray();
    }

    #endregion
}