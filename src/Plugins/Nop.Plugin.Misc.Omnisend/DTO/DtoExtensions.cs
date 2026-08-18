using Microsoft.Extensions.Logging;
﻿namespace Nop.Plugin.Misc.Omnisend.DTO;

public static class DtoExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public static string ToDtoString(this DateTime date)
    {
		_logger.LogDebug(">>> Entering ToDtoString(date={Date})", date);
		_logger.LogDebug("<<< Exiting ToDtoString(date={Date})", date);
        return date.ToString("s") + "Z";
    }

    public static int ToCents(this decimal price)
    {
		_logger.LogDebug(">>> Entering ToCents(price={Price})", price);
		_logger.LogDebug("<<< Exiting ToCents(price={Price})", price);
        return (int)(price * 100);
    }
}