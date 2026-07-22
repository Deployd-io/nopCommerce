using Nop.Core.Domain.Orders;
using Nop.Data;

namespace Nop.Services.Orders;

/// <summary>
/// Custom wishlist service
/// </summary>
public partial class CustomWishlistService : ICustomWishlistService
{
    #region Fields

    protected readonly IRepository<CustomWishlist> _customWishlistRepository;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    #endregion

    #region Ctor

    public CustomWishlistService(IRepository<CustomWishlist> customWishlistRepository, 
        ShoppingCartSettings shoppingCartSettings)
    {
        _customWishlistRepository = customWishlistRepository;
        _shoppingCartSettings = shoppingCartSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Retrieves all custom wishlists associated with the specified customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer whose custom wishlists are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
    /// cref="CustomWishlist"/> objects associated with the specified customer. If multiple wishlists  are not allowed,
    /// an empty list is returned.</returns>
    public virtual async Task<IList<CustomWishlist>> GetAllCustomWishlistsAsync(int customerId)
    {
		logger.LogDebug(">>> Entering GetAllCustomWishlistsAsync(customerId={CustomerId})", customerId);
        if (!_shoppingCartSettings.AllowMultipleWishlist)
            return new List<CustomWishlist>();
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var query = _customWishlistRepository.Table
            .Where(w => w.CustomerId == customerId)
            .OrderByDescending(w => w.CreatedOnUtc);
		logger.LogDebug("<<< Exiting GetAllCustomWishlistsAsync(customerId={CustomerId})", customerId);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Adds a custom wishlist item to the repository.
    /// </summary>
    /// <param name="item">The custom wishlist item to add. Cannot be <see langword="null"/>.</param>
    public virtual async Task AddCustomWishlistAsync(CustomWishlist item)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _customWishlistRepository.InsertAsync(item);
    }

    /// <summary>
    /// Removes a custom wishlist item with the specified identifier.
    /// </summary>
    /// <param name="itemId">The unique identifier of the custom wishlist item to remove. Must be a valid identifier of an existing item.</param>
    public virtual async Task RemoveCustomWishlistAsync(int itemId)
    {
		logger.LogDebug(">>> Entering RemoveCustomWishlistAsync(itemId={ItemId})", itemId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var item = await _customWishlistRepository.GetByIdAsync(itemId);
        if (item != null)
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("RemoveCustomWishlistAsync(itemId)={ItemId}: find query executed in {Arg1} ms", itemId, __logStart.ElapsedMilliseconds);
        {
            await _customWishlistRepository.DeleteAsync(item);
        }
		logger.LogDebug("<<< Exiting RemoveCustomWishlistAsync(itemId={ItemId})", itemId);
		logger.LogInformation("RemoveCustomWishlistAsync(itemId)={ItemId}: external service call took {Arg1} ms", itemId, __logStart.ElapsedMilliseconds);
		logger.LogDebug("RemoveCustomWishlistAsync(itemId={ItemId}): item → {Item}", itemId, item);
		logger.LogInformation("RemoveCustomWishlistAsync(itemId)={ItemId}: delete query executed in {Arg1} ms", itemId, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Retrieves a custom wishlist by its unique identifier.
    /// </summary>
    /// <param name="itemId">The unique identifier of the custom wishlist to retrieve. Must be a positive integer.</param>
    /// <returns>A <see cref="CustomWishlist"/> object representing the custom wishlist with the specified identifier. Returns
    /// null if no wishlist is found with the given identifier.</returns>
    public virtual async Task<CustomWishlist> GetCustomWishlistByIdAsync(int itemId)
    {
		logger.LogDebug(">>> Entering GetCustomWishlistByIdAsync(itemId={ItemId})", itemId);
		logger.LogDebug("<<< Exiting GetCustomWishlistByIdAsync(itemId={ItemId})", itemId);
        return await _customWishlistRepository.GetByIdAsync(itemId);
    }

    #endregion
}
