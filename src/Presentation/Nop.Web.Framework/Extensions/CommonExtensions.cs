using System.Globalization;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Extensions;

/// <summary>
/// Extensions
/// </summary>
public static class CommonExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Returns a value indicating whether real selection is not possible
    /// </summary>
    /// <param name="items">Items</param>
    /// <param name="ignoreZeroValue">A value indicating whether we should ignore items with "0" value</param>
    /// <returns>A value indicating whether real selection is not possible</returns>
    public static bool SelectionIsNotPossible(this IList<SelectListItem> items, bool ignoreZeroValue = true)
    {
		_logger.LogError(">>> Entering SelectionIsNotPossible(items={Items},ignoreZeroValue={IgnoreZeroValue})", items, ignoreZeroValue);
        ArgumentNullException.ThrowIfNull(items);

        //we ignore items with "0" value? Usually it's something like "Select All", "etc
		_logger.LogWarning("<<< Exiting SelectionIsNotPossible(items={Items},ignoreZeroValue={IgnoreZeroValue})", items, ignoreZeroValue);
        return items.Count(x => !ignoreZeroValue || !x.Value.ToString().Equals("0")) < 2;
    }

    /// <summary>
    /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
    /// </summary>
    /// <param name="source">Source (UTC format)</param>
    /// <param name="languageCode">Language culture code</param>
    /// <returns>Formatted date and time string</returns>
    public static string RelativeFormat(this DateTime source, string languageCode = "en-US")
    {
		_logger.LogError(">>> Entering RelativeFormat(source={Source},languageCode={LanguageCode})", source, languageCode);
        var ts = new TimeSpan(DateTime.UtcNow.Ticks - source.Ticks);
        var delta = ts.TotalSeconds;
		_logger.LogError("RelativeFormat(source={Source},languageCode={LanguageCode}): ts → {Ts}", source, languageCode, ts);

        CultureInfo culture;
        try
        {
            culture = new CultureInfo(languageCode);
			_logger.LogError("RelativeFormat(source={Source},languageCode={LanguageCode}): culture → {Culture}", source, languageCode, culture);
        }
        catch (CultureNotFoundException)
        {
			_logger.LogError("Exception in RelativeFormat(source={Source},languageCode={LanguageCode}): {Arg2}", source, languageCode, CultureNotFoundException.Message);
            culture = new CultureInfo("en-US");
			_logger.LogDebug("RelativeFormat(source={Source},languageCode={LanguageCode}): culture → {Culture}", source, languageCode, culture);
        }
		_logger.LogWarning("<<< Exiting RelativeFormat(source={Source},languageCode={LanguageCode})", source, languageCode);
        return TimeSpan.FromSeconds(delta).Humanize(precision: 1, culture: culture, maxUnit: TimeUnit.Year);
    }
}