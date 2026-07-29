namespace Nop.Core.Domain.Customers;

/// <summary>
/// RewardPointsActivatingDelayPeriod Extensions
/// </summary>
public static class RewardPointsActivatingDelayPeriodExtensions
{
    /// <summary>
    /// Returns a delay period before activating points in hours
    /// </summary>
    /// <param name="period">Reward points activating delay period</param>
    /// <param name="value">Value of delay</param>
    /// <returns>Value of delay in hours</returns>
    public static int ToHours(this RewardPointsActivatingDelayPeriod period, int value)
    {
		logger.LogDebug(">>> Entering ToHours(period={Period},value={Value})", period, value);
		logger.LogDebug("ToHours(period={Period},value={Value}): RewardPointsActivatingDelayPeriod.Hours → {Arg2}", period, value, RewardPointsActivatingDelayPeriod.Hours);
		logger.LogDebug("ToHours(period={Period},value={Value}): RewardPointsActivatingDelayPeriod.Days → {Arg2}", period, value, RewardPointsActivatingDelayPeriod.Days);
		logger.LogDebug("<<< Exiting ToHours(period={Period},value={Value})", period, value);
        return period switch
        {
            RewardPointsActivatingDelayPeriod.Hours => value,
            RewardPointsActivatingDelayPeriod.Days => value * 24,
            _ => throw new ArgumentOutOfRangeException(nameof(period)),
        };
    }
}