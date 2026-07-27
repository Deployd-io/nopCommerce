using Nop.Core;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Plugin.Tax.Avalara.Services;

/// <summary>
/// Represents the tax transaction log service implementation
/// </summary>
public class TaxTransactionLogService
{
    #region Fields

    protected readonly IRepository<TaxTransactionLog> _taxTransactionLogRepository;

    #endregion

    #region Ctor

    public TaxTransactionLogService(IRepository<TaxTransactionLog> taxTransactionLogRepository)
    {
		logger.LogDebug(">>> Entering TaxTransactionLogService(taxTransactionLogRepository={TaxTransactionLogRepository})", taxTransactionLogRepository);
        _taxTransactionLogRepository = taxTransactionLogRepository;
		logger.LogDebug("<<< Exiting TaxTransactionLogService(taxTransactionLogRepository={TaxTransactionLogRepository})", taxTransactionLogRepository);
		logger.LogDebug("TaxTransactionLogService(taxTransactionLogRepository={TaxTransactionLogRepository}): _taxTransactionLogRepository → {_taxTransactionLogRepository}", taxTransactionLogRepository, _taxTransactionLogRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get tax transaction log
    /// </summary>
    /// <param name="customerId">Customer identifier; pass null to load all records</param>
    /// <param name="createdFromUtc">Log item creation from; pass null to load all records</param>
    /// <param name="createdToUtc">Log item creation to; pass null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of tax transaction log items
    /// </returns>
    public async Task<IPagedList<TaxTransactionLog>> GetTaxTransactionLogAsync(int? customerId = null,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        //get all logs
        var query = _taxTransactionLogRepository.Table;

        //filter by customer
        if (customerId.HasValue)
            query = query.Where(logItem => logItem.CustomerId == customerId);

        //filter by dates
        if (createdFromUtc.HasValue)
            query = query.Where(logItem => logItem.CreatedDateUtc >= createdFromUtc.Value);
        if (createdToUtc.HasValue)
            query = query.Where(logItem => logItem.CreatedDateUtc <= createdToUtc.Value);

        //order log records
        query = query.OrderByDescending(logItem => logItem.CreatedDateUtc).ThenByDescending(logItem => logItem.Id);

        //return paged log
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Get a log item by the identifier
    /// </summary>
    /// <param name="logItemId">Log item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the log item
    /// </returns>
    public async Task<TaxTransactionLog> GetTaxTransactionLogByIdAsync(int logItemId)
    {
		logger.LogDebug(">>> Entering GetTaxTransactionLogByIdAsync(logItemId={LogItemId})", logItemId);
		logger.LogDebug("<<< Exiting GetTaxTransactionLogByIdAsync(logItemId={LogItemId})", logItemId);
        return await _taxTransactionLogRepository.GetByIdAsync(logItemId);
    }

    /// <summary>
    /// Insert the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		logger.LogDebug(">>> Entering InsertTaxTransactionLogAsync(logItem={LogItem})", logItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(logItem);

        await _taxTransactionLogRepository.InsertAsync(logItem, false);
		logger.LogDebug("<<< Exiting InsertTaxTransactionLogAsync(logItem={LogItem})", logItem);
		logger.LogInformation("InsertTaxTransactionLogAsync(logItem)={LogItem}: save query executed in {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Update the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		logger.LogDebug(">>> Entering UpdateTaxTransactionLogAsync(logItem={LogItem})", logItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(logItem);

        await _taxTransactionLogRepository.UpdateAsync(logItem, false);
		logger.LogDebug("<<< Exiting UpdateTaxTransactionLogAsync(logItem={LogItem})", logItem);
		logger.LogInformation("UpdateTaxTransactionLogAsync(logItem)={LogItem}: update query executed in {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Delete the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		logger.LogDebug(">>> Entering DeleteTaxTransactionLogAsync(logItem={LogItem})", logItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxTransactionLogRepository.DeleteAsync(logItem, false);
		logger.LogDebug("<<< Exiting DeleteTaxTransactionLogAsync(logItem={LogItem})", logItem);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTaxTransactionLogAsync(logItem)={LogItem}: external service call took {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTaxTransactionLogAsync(logItem)={LogItem}: delete query executed in {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteTaxTransactionLogAsync(logItem)={LogItem}: external service call took {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteTaxTransactionLogAsync(logItem)={LogItem}: delete query executed in {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Delete log items
    /// </summary>
    /// <param name="ids">Log items identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteTaxTransactionLogAsync(int[] ids)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug(">>> Entering DeleteTaxTransactionLogAsync(ids={Ids})", ids);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTaxTransactionLogAsync(ids)={Ids}: external service call took {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTaxTransactionLogAsync(ids)={Ids}: delete query executed in {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
        await _taxTransactionLogRepository.DeleteAsync(logItem => ids.Contains(logItem.Id));
		logger.LogDebug("<<< Exiting DeleteTaxTransactionLogAsync(ids={Ids})", ids);
    }

    /// <summary>
    /// Clear the log
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ClearLogAsync()
    {
		logger.LogDebug(">>> Entering ClearLogAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxTransactionLogRepository.TruncateAsync();
		logger.LogDebug("<<< Exiting ClearLogAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("ClearLogAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("ClearLogAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
    #endregion
}