using Nop.Core;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Services;

/// <summary>
/// Represents the item classification service implementation
/// </summary>
public class ItemClassificationService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly IRepository<ItemClassification> _itemClassificationRepository;

    #endregion

    #region Ctor

    public ItemClassificationService(AvalaraTaxSettings avalaraTaxSettings,
        IRepository<ItemClassification> itemClassificationRepository)
    {
		_logger.LogError(">>> Entering ItemClassificationService(avalaraTaxSettings={AvalaraTaxSettings},itemClassificationRepository={ItemClassificationRepository})", avalaraTaxSettings, itemClassificationRepository);
        _avalaraTaxSettings = avalaraTaxSettings;
        _itemClassificationRepository = itemClassificationRepository;
		_logger.LogDebug("ItemClassificationService(avalaraTaxSettings={AvalaraTaxSettings},itemClassificationRepository={ItemClassificationRepository}): _itemClassificationRepository → {_itemClassificationRepository}", avalaraTaxSettings, itemClassificationRepository, _itemClassificationRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get item classification
    /// </summary>
    /// <param name="countryId">Country identifier</param>
    /// <param name="productId">Product identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of classification items
    /// </returns>
    public async Task<IPagedList<ItemClassification>> GetItemClassificationAsync(int? countryId = null,
        int? productId = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetItemClassificationAsync(countryId={CountryId},productId={ProductId},pageIndex={PageIndex},pageSize={PageSize})", countryId, productId, pageIndex, pageSize);
        //get all items
        var query = _itemClassificationRepository.Table;
		_logger.LogError("GetItemClassificationAsync(countryId={CountryId},productId={ProductId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", countryId, productId, pageIndex, pageSize, query);

        //filter by country
        if (countryId.HasValue && countryId > 0)
            query = query.Where(item => item.CountryId == countryId);

        //filter by product
        if (productId.HasValue)
            query = query.Where(item => item.ProductId == productId);

        //order item records
        query = query.OrderByDescending(item => item.UpdatedOnUtc).ThenByDescending(item => item.Id);

		_logger.LogWarning("<<< Exiting GetItemClassificationAsync(countryId={CountryId},productId={ProductId},pageIndex={PageIndex},pageSize={PageSize})", countryId, productId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Get item classification by the identifier
    /// </summary>
    /// <param name="itemId">Item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the item classification
    /// </returns>
    public async Task<ItemClassification> GetItemClassificationByIdAsync(int itemId)
    {
		_logger.LogError(">>> Entering GetItemClassificationByIdAsync(itemId={ItemId})", itemId);
        _logger.LogWarning("<<< Exiting GetItemClassificationByIdAsync(itemId={ItemId})", itemId);
        return await _itemClassificationRepository.GetByIdAsync(itemId);
    }

    /// <summary>
    /// Get item classification by the request identifier
    /// </summary>
    /// <param name="requestId">Request identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the item classification
    /// </returns>
    public async Task<ItemClassification> GetItemClassificationByRequestIdAsync(string requestId)
    {
		_logger.LogError(">>> Entering GetItemClassificationByRequestIdAsync(requestId={RequestId})", requestId);
        if (string.IsNullOrEmpty(requestId))
            return null;

        return await _itemClassificationRepository.Table
            .FirstOrDefaultAsync(item => requestId.Equals(item.HSClassificationRequestId, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Add items for classification
    /// </summary>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task AddItemClassificationAsync(List<int> productIds)
    {
		_logger.LogError(">>> Entering AddItemClassificationAsync(productIds={ProductIds})", productIds);
        if (!productIds?.Any() ?? true)
            return;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var newProductIds = productIds.Except(await _itemClassificationRepository.Table.Select(record => record.ProductId).ToListAsync()).ToList();
        if (!newProductIds.Any())
            return;

        if (!_avalaraTaxSettings.SelectedCountryIds?.Any() ?? true)
            return;

        var records = _avalaraTaxSettings.SelectedCountryIds.SelectMany(countryId => newProductIds.Select(productId => new ItemClassification
        {
            CountryId = countryId,
            ProductId = productId,
            UpdatedOnUtc = DateTime.UtcNow,
        }));

        await _itemClassificationRepository.InsertAsync(records.ToList(), false);
		_logger.LogError("AddItemClassificationAsync(productIds)={ProductIds}: save query executed in {Arg1} ms", productIds, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting AddItemClassificationAsync(productIds={ProductIds})", productIds);
    }

    /// <summary>
    /// Update the item classification
    /// </summary>
    /// <param name="item">Item classification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateItemClassificationAsync(ItemClassification item)
    {
		_logger.LogError(">>> Entering UpdateItemClassificationAsync(item={Item})", item);
        ArgumentNullException.ThrowIfNull(item);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("UpdateItemClassificationAsync(item)={Item}: update query executed in {Arg1} ms", item, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting UpdateItemClassificationAsync(item={Item})", item);
        await _itemClassificationRepository.UpdateAsync(item, false);
    }

    /// <summary>
    /// Delete items
    /// </summary>
    /// <param name="ids">Items identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteItemsAsync(List<int> ids)
    {
		_logger.LogError(">>> Entering DeleteItemsAsync(ids={Ids})", ids);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _itemClassificationRepository.DeleteAsync(item => ids.Contains(item.Id));
		_logger.LogError("DeleteItemsAsync(ids)={Ids}: external service call took {Arg1} ms", ids, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteItemsAsync(ids={Ids})", ids);
    }

    /// <summary>
    /// Clear the item classification
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ClearItemClassificationAsync()
    {
		_logger.LogError(">>> Entering ClearItemClassificationAsync()");
        await _itemClassificationRepository.TruncateAsync();
		_logger.LogWarning("<<< Exiting ClearItemClassificationAsync()");
    }

    #endregion
}