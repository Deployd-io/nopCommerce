namespace Nop.Core.Domain.Messages;

/// <summary>
/// MessageDelayPeriod Extensions
/// </summary>
public static class MessageDelayPeriodExtensions
{
    /// <summary>
    /// Returns message delay in hours
    /// </summary>
    /// <param name="period">Message delay period</param>
    /// <param name="value">Value of delay send</param>
    /// <returns>Value of message delay in hours</returns>
    public static int ToHours(this MessageDelayPeriod period, int value)
    {
		logger.LogDebug(">>> Entering ToHours(period={Period},value={Value})", period, value);
		logger.LogDebug("ToHours(period={Period},value={Value}): MessageDelayPeriod.Hours → {Arg2}", period, value, MessageDelayPeriod.Hours);
		logger.LogDebug("ToHours(period={Period},value={Value}): MessageDelayPeriod.Days → {Arg2}", period, value, MessageDelayPeriod.Days);
		logger.LogDebug("<<< Exiting ToHours(period={Period},value={Value})", period, value);
        return period switch
        {
            MessageDelayPeriod.Hours => value,
            MessageDelayPeriod.Days => value * 24,
            _ => throw new ArgumentOutOfRangeException(nameof(period)),
        };
    }
}
