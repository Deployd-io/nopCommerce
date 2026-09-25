using Microsoft.Extensions.Logging;
namespace Nop.Plugin.Misc.Omnisend.DTO;

public static class DtoExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public static string ToDtoString(this DateTime date)
    {
		_logger.LogError(">>> Entering ToDtoString(date={Date})", date);
        _logger.LogWarning("<<< Exiting ToDtoString(date={Date})", date);
        return date.ToString("s") + "Z";
    }

    public static int ToCents(this decimal price)
    {
		_logger.LogError(">>> Entering ToCents(price={Price})", price);
        _logger.LogWarning("<<< Exiting ToCents(price={Price})", price);
        return (int)(price * 100);
    }
}