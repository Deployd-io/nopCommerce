using Nop.Core;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Services;

/// <summary>
/// Represents the tax transaction log service implementation
/// </summary>
public class TaxTransactionLogService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<TaxTransactionLog> _taxTransactionLogRepository;

    #endregion

    #region Ctor

    public TaxTransactionLogService(IRepository<TaxTransactionLog> taxTransactionLogRepository)
    {
		_logger.LogError(">>> Entering TaxTransactionLogService(taxTransactionLogRepository={TaxTransactionLogRepository})", taxTransactionLogRepository);
        _taxTransactionLogRepository = taxTransactionLogRepository;
		_logger.LogDebug("TaxTransactionLogService(taxTransactionLogRepository={TaxTransactionLogRepository}): _taxTransactionLogRepository → {_taxTransactionLogRepository}", taxTransactionLogRepository, _taxTransactionLogRepository);
    _logger.LogWarning("<<< Exiting TaxTransactionLogService(taxTransactionLogRepository={TaxTransactionLogRepository})", taxTransactionLogRepository);
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
		_logger.LogError(">>> Entering GetTaxTransactionLogAsync(customerId={CustomerId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize})", customerId, createdFromUtc, createdToUtc, pageIndex, pageSize);
        //get all logs
        var query = _taxTransactionLogRepository.Table;
		_logger.LogError("GetTaxTransactionLogAsync(customerId={CustomerId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", customerId, createdFromUtc, createdToUtc, pageIndex, pageSize, query);

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
		_logger.LogWarning("<<< Exiting GetTaxTransactionLogAsync(customerId={CustomerId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize})", customerId, createdFromUtc, createdToUtc, pageIndex, pageSize);
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
		_logger.LogError(">>> Entering GetTaxTransactionLogByIdAsync(logItemId={LogItemId})", logItemId);
        _logger.LogWarning("<<< Exiting GetTaxTransactionLogByIdAsync(logItemId={LogItemId})", logItemId);
        return await _taxTransactionLogRepository.GetByIdAsync(logItemId);
    }

    /// <summary>
    /// Insert the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		_logger.LogError(">>> Entering InsertTaxTransactionLogAsync(logItem={LogItem})", logItem);
        ArgumentNullException.ThrowIfNull(logItem);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("InsertTaxTransactionLogAsync(logItem)={LogItem}: save query executed in {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting InsertTaxTransactionLogAsync(logItem={LogItem})", logItem);
        await _taxTransactionLogRepository.InsertAsync(logItem, false);
    }

    /// <summary>
    /// Update the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		_logger.LogError(">>> Entering UpdateTaxTransactionLogAsync(logItem={LogItem})", logItem);
        ArgumentNullException.ThrowIfNull(logItem);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("UpdateTaxTransactionLogAsync(logItem)={LogItem}: update query executed in {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting UpdateTaxTransactionLogAsync(logItem={LogItem})", logItem);
        await _taxTransactionLogRepository.UpdateAsync(logItem, false);
    }

    /// <summary>
    /// Delete the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		_logger.LogError(">>> Entering DeleteTaxTransactionLogAsync(logItem={LogItem})", logItem);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxTransactionLogRepository.DeleteAsync(logItem, false);
		_logger.LogError("DeleteTaxTransactionLogAsync(logItem)={LogItem}: external service call took {Arg1} ms", logItem, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteTaxTransactionLogAsync(logItem={LogItem})", logItem);
    }

    /// <summary>
    /// Delete log items
    /// </summary>
    /// <param name="ids">Log items identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteTaxTransactionLogAsync(int[] ids)
    {
		_logger.LogError(">>> Entering DeleteTaxTransactionLogAsync(ids={Ids})", ids);
        await _taxTransactionLogRepository.DeleteAsync(logItem => ids.Contains(logItem.Id));
		_logger.LogWarning("<<< Exiting DeleteTaxTransactionLogAsync(ids={Ids})", ids);
    }

    /// <summary>
    /// Clear the log
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ClearLogAsync()
    {
		_logger.LogError(">>> Entering ClearLogAsync()");
        await _taxTransactionLogRepository.TruncateAsync();
		_logger.LogWarning("<<< Exiting ClearLogAsync()");
    }

    #endregion
}