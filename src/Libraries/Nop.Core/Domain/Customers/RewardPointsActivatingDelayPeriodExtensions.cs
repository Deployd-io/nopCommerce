using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Customers;

/// <summary>
/// RewardPointsActivatingDelayPeriod Extensions
/// </summary>
public static class RewardPointsActivatingDelayPeriodExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Returns a delay period before activating points in hours
    /// </summary>
    /// <param name="period">Reward points activating delay period</param>
    /// <param name="value">Value of delay</param>
    /// <returns>Value of delay in hours</returns>
    public static int ToHours(this RewardPointsActivatingDelayPeriod period, int value)
    {
		_logger.LogError(">>> Entering ToHours(period={Period},value={Value})", period, value);
		_logger.LogWarning("<<< Exiting ToHours(period={Period},value={Value})", period, value);
        return period switch
        {
            RewardPointsActivatingDelayPeriod.Hours => value,
            RewardPointsActivatingDelayPeriod.Days => value * 24,
            _ => throw new ArgumentOutOfRangeException(nameof(period)),
        };
    }
}