namespace Nop.Plugin.Misc.Omnisend.DTO;

public static class DtoExtensions
{
    public static string ToDtoString(this DateTime date)
    {
		logger.LogDebug(">>> Entering ToDtoString(date={Date})", date);
		logger.LogDebug("<<< Exiting ToDtoString(date={Date})", date);
        return date.ToString("s") + "Z";
    }

    public static int ToCents(this decimal price)
    {
		logger.LogDebug(">>> Entering ToCents(price={Price})", price);
		logger.LogDebug("<<< Exiting ToCents(price={Price})", price);
        return (int)(price * 100);
    }
}