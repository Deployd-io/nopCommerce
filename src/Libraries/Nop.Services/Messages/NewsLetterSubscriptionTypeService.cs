using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Stores;

namespace Nop.Services.Messages;

/// <summary>
/// Represents newsletter subscription type service implementation
/// </summary>
public partial class NewsLetterSubscriptionTypeService : INewsLetterSubscriptionTypeService
{
    #region Fields

    protected readonly IRepository<NewsLetterSubscriptionType> _newsLetterSubscriptionTypeRepository;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionTypeService(IRepository<NewsLetterSubscriptionType> newsLetterSubscriptionTypeRepository,
        IStoreMappingService storeMappingService)
    {
        _newsLetterSubscriptionTypeRepository = newsLetterSubscriptionTypeRepository;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inserts a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">Newsletter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
		logger.LogDebug(">>> Entering InsertNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType={NewsLetterSubscriptionType})", newsLetterSubscriptionType);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(newsLetterSubscriptionType);

        await _newsLetterSubscriptionTypeRepository.InsertAsync(newsLetterSubscriptionType);
		logger.LogDebug("<<< Exiting InsertNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType={NewsLetterSubscriptionType})", newsLetterSubscriptionType);
		logger.LogInformation("InsertNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType)={NewsLetterSubscriptionType}: save query executed in {Arg1} ms", newsLetterSubscriptionType, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Updates a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">Newsletter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
		logger.LogDebug(">>> Entering UpdateNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType={NewsLetterSubscriptionType})", newsLetterSubscriptionType);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(newsLetterSubscriptionType);

        await _newsLetterSubscriptionTypeRepository.UpdateAsync(newsLetterSubscriptionType);
		logger.LogDebug("<<< Exiting UpdateNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType={NewsLetterSubscriptionType})", newsLetterSubscriptionType);
		logger.LogInformation("UpdateNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType)={NewsLetterSubscriptionType}: update query executed in {Arg1} ms", newsLetterSubscriptionType, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Deletes a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">Newsletter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
		logger.LogDebug(">>> Entering DeleteNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType={NewsLetterSubscriptionType})", newsLetterSubscriptionType);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(newsLetterSubscriptionType);        

        await _newsLetterSubscriptionTypeRepository.DeleteAsync(newsLetterSubscriptionType);
		logger.LogDebug("<<< Exiting DeleteNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType={NewsLetterSubscriptionType})", newsLetterSubscriptionType);
		logger.LogInformation("DeleteNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType)={NewsLetterSubscriptionType}: external service call took {Arg1} ms", newsLetterSubscriptionType, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteNewsLetterSubscriptionTypeAsync(newsLetterSubscriptionType)={NewsLetterSubscriptionType}: delete query executed in {Arg1} ms", newsLetterSubscriptionType, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Gets a newsletter subscription type by newsletter subscription type identifier
    /// </summary>
    /// <param name="newsLetterSubscriptionTypeId">The newsletter subscription type identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription type
    /// </returns>
    public virtual async Task<NewsLetterSubscriptionType> GetNewsLetterSubscriptionTypeByIdAsync(int newsLetterSubscriptionTypeId)
    {
		logger.LogDebug(">>> Entering GetNewsLetterSubscriptionTypeByIdAsync(newsLetterSubscriptionTypeId={NewsLetterSubscriptionTypeId})", newsLetterSubscriptionTypeId);
		logger.LogDebug("<<< Exiting GetNewsLetterSubscriptionTypeByIdAsync(newsLetterSubscriptionTypeId={NewsLetterSubscriptionTypeId})", newsLetterSubscriptionTypeId);
        return await _newsLetterSubscriptionTypeRepository.GetByIdAsync(newsLetterSubscriptionTypeId, cache => default);
    }

    /// <summary>
    /// Gets the newsletter subscription type list
    /// </summary>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription types
    /// </returns>
    public virtual async Task<IList<NewsLetterSubscriptionType>> GetAllNewsLetterSubscriptionTypesAsync(int storeId = 0)
    {
		logger.LogDebug(">>> Entering GetAllNewsLetterSubscriptionTypesAsync(storeId={StoreId})", storeId);
        return await _newsLetterSubscriptionTypeRepository.GetAllAsync(async query =>
        {
            if (storeId > 0)
				logger.LogDebug("GetAllNewsLetterSubscriptionTypesAsync(storeId={StoreId}): query → {Query}", storeId, query);
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

			logger.LogInformation("GetAllNewsLetterSubscriptionTypesAsync(storeId)={StoreId}: find query executed in {Arg1} ms", storeId, __logStart.ElapsedMilliseconds);
            query = query
                .OrderBy(subscriptionType => subscriptionType.DisplayOrder)
                .ThenBy(subscriptionType => subscriptionType.Id);

            return query;
        });
		logger.LogDebug("<<< Exiting GetAllNewsLetterSubscriptionTypesAsync(storeId={StoreId})", storeId);
    }

    #endregion
}