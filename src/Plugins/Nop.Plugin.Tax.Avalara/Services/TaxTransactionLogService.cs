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
		_logger.LogDebug(">>> Entering TaxTransactionLogService(taxTransactionLogRepository={TaxTransactionLogRepository})", taxTransactionLogRepository);
        _taxTransactionLogRepository = taxTransactionLogRepository;
		_logger.LogDebug("TaxTransactionLogService(taxTransactionLogRepository={TaxTransactionLogRepository}): _taxTransactionLogRepository → {_taxTransactionLogRepository}", taxTransactionLogRepository, _taxTransactionLogRepository);
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
		_logger.LogDebug(">>> Entering GetTaxTransactionLogAsync(customerId={CustomerId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize})", customerId, createdFromUtc, createdToUtc, pageIndex, pageSize);
        //get all logs
        var query = _taxTransactionLogRepository.Table;
		_logger.LogDebug("GetTaxTransactionLogAsync(customerId={CustomerId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", customerId, createdFromUtc, createdToUtc, pageIndex, pageSize, query);

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
		_logger.LogDebug("<<< Exiting GetTaxTransactionLogAsync(customerId={CustomerId},createdFromUtc={CreatedFromUtc},createdToUtc={CreatedToUtc},pageIndex={PageIndex},pageSize={PageSize})", customerId, createdFromUtc, createdToUtc, pageIndex, pageSize);
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
		_logger.LogDebug(">>> Entering GetTaxTransactionLogByIdAsync(logItemId={LogItemId})", logItemId);
        return await _taxTransactionLogRepository.GetByIdAsync(logItemId);
    }

    /// <summary>
    /// Insert the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		_logger.LogDebug(">>> Entering InsertTaxTransactionLogAsync(logItem={LogItem})", logItem);
        ArgumentNullException.ThrowIfNull(logItem);

		_logger.LogDebug("<<< Exiting InsertTaxTransactionLogAsync(logItem={LogItem})", logItem);
        await _taxTransactionLogRepository.InsertAsync(logItem, false);
    }

    /// <summary>
    /// Update the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		_logger.LogDebug(">>> Entering UpdateTaxTransactionLogAsync(logItem={LogItem})", logItem);
        ArgumentNullException.ThrowIfNull(logItem);

		_logger.LogDebug("<<< Exiting UpdateTaxTransactionLogAsync(logItem={LogItem})", logItem);
        await _taxTransactionLogRepository.UpdateAsync(logItem, false);
    }

    /// <summary>
    /// Delete the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteTaxTransactionLogAsync(TaxTransactionLog logItem)
    {
		_logger.LogDebug(">>> Entering DeleteTaxTransactionLogAsync(logItem={LogItem})", logItem);
        await _taxTransactionLogRepository.DeleteAsync(logItem, false);
		_logger.LogDebug("<<< Exiting DeleteTaxTransactionLogAsync(ids={Ids})", ids);
    }

    /// <summary>
    /// Delete log items
    /// </summary>
    /// <param name="ids">Log items identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteTaxTransactionLogAsync(int[] ids)
    {
		_logger.LogDebug(">>> Entering DeleteTaxTransactionLogAsync(ids={Ids})", ids);
        await _taxTransactionLogRepository.DeleteAsync(logItem => ids.Contains(logItem.Id));
    }

    /// <summary>
    /// Clear the log
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ClearLogAsync()
    {
		_logger.LogDebug(">>> Entering ClearLogAsync()");
        await _taxTransactionLogRepository.TruncateAsync();
    }

    #endregion
}