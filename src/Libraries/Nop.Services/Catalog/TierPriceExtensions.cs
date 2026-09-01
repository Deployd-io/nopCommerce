using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Tier price extensions
/// </summary>
public static class TierPriceExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Filter tier prices by a store
    /// </summary>
    /// <param name="source">Tier prices</param>
    /// <param name="store">Store reference</param>
    /// <returns>Filtered tier prices</returns>
    public static IEnumerable<TierPrice> FilterByStore(this IEnumerable<TierPrice> source, Store store)
    {
		_logger.LogDebug(">>> Entering FilterByStore(source={Source},store={Store})", source, store);
        ArgumentNullException.ThrowIfNull(source);

        var storeId = store?.Id ?? 0;

		_logger.LogDebug("<<< Exiting FilterByStore(source={Source},store={Store})", source, store);
        return source.Where(tierPrice => tierPrice.StoreId == 0 || tierPrice.StoreId == storeId);
    }

    /// <summary>
    /// Filter tier prices by customer roles
    /// </summary>
    /// <param name="source">Tier prices</param>
    /// <param name="customerRoleIds">Customer role identifiers</param>
    /// <returns>Filtered tier prices</returns>
    public static IEnumerable<TierPrice> FilterByCustomerRole(this IEnumerable<TierPrice> source, int[] customerRoleIds)
    {
		_logger.LogDebug(">>> Entering FilterByCustomerRole(source={Source},customerRoleIds={CustomerRoleIds})", source, customerRoleIds);
        ArgumentNullException.ThrowIfNull(source);

        ArgumentNullException.ThrowIfNull(customerRoleIds);

        if (!customerRoleIds.Any())
            return source;

        return source.Where(tierPrice =>
            !tierPrice.CustomerRoleId.HasValue || tierPrice.CustomerRoleId == 0 || customerRoleIds.Contains(tierPrice.CustomerRoleId.Value));
    }

    /// <summary>
    /// Remove duplicated quantities (leave only an tier price with minimum price)
    /// </summary>
    /// <param name="source">Tier prices</param>
    /// <returns>Filtered tier prices</returns>
    public static IEnumerable<TierPrice> RemoveDuplicatedQuantities(this IEnumerable<TierPrice> source)
    {
		_logger.LogDebug(">>> Entering RemoveDuplicatedQuantities(source={Source})", source);
        ArgumentNullException.ThrowIfNull(source);

        var tierPrices = source.ToList();
		_logger.LogDebug("RemoveDuplicatedQuantities(source={Source}): tierPrices → {TierPrices}", source, tierPrices);

        //get group of tier prices with the same quantity
        var tierPricesWithDuplicates = tierPrices.GroupBy(tierPrice => tierPrice.Quantity).Where(group => group.Count() > 1);
		_logger.LogDebug("RemoveDuplicatedQuantities(source={Source}): tierPricesWithDuplicates → {TierPricesWithDuplicates}", source, tierPricesWithDuplicates);

        //get tier prices with higher prices 
        var duplicatedPrices = tierPricesWithDuplicates.SelectMany(group =>
        {
            //find minimal price for quantity
            var minTierPrice = group.Aggregate((currentMinTierPrice, nextTierPrice) =>
                (currentMinTierPrice.Price < nextTierPrice.Price ? currentMinTierPrice : nextTierPrice));

            //and return all other with higher price
		_logger.LogDebug("<<< Exiting RemoveDuplicatedQuantities(source={Source})", source);
            return group.Where(tierPrice => tierPrice.Id != minTierPrice.Id);
        });

        //return tier prices without duplicates
        return tierPrices.Where(tierPrice => duplicatedPrices.All(duplicatedPrice => duplicatedPrice.Id != tierPrice.Id));
    }

    /// <summary>
    /// Filter tier prices by date
    /// </summary>
    /// <param name="source">Tier prices</param>
    /// <param name="date">Date in UTC; pass null to filter by current date</param>
    /// <returns>Filtered tier prices</returns>
    public static IEnumerable<TierPrice> FilterByDate(this IEnumerable<TierPrice> source, DateTime? date = null)
    {
		_logger.LogDebug(">>> Entering FilterByDate(source={Source},date={Date})", source, date);
        ArgumentNullException.ThrowIfNull(source);

        if (!date.HasValue)
            date = DateTime.UtcNow;

		_logger.LogDebug("<<< Exiting FilterByDate(source={Source},date={Date})", source, date);
        return source.Where(tierPrice =>
            (!tierPrice.StartDateTimeUtc.HasValue || tierPrice.StartDateTimeUtc.Value < date) &&
            (!tierPrice.EndDateTimeUtc.HasValue || tierPrice.EndDateTimeUtc.Value > date));
    }
}