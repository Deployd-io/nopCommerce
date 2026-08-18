using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Html;

/// <summary>
/// Represents a ResolveLinks helper
/// </summary>
public partial class ResolveLinksHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected const string LINK = "<a href=\"{0}{1}\" rel=\"nofollow\">{2}</a>";
    protected const int MAX_LENGTH = 50;

    /// <summary>
    /// The regular expression used to parse links.
    /// </summary>
    protected static readonly Regex _regex = new("((http://|https://|www\\.)([A-Z0-9.\\-]{1,})\\.[0-9A-Z?;~&\\(\\)#,=\\-_\\./\\+]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #endregion

    #region Utilities

    /// <summary>
    /// Shortens any absolute URL to a specified maximum length
    /// </summary>
    protected static string ShortenUrl(string url, int max)
    {
		_logger.LogDebug(">>> Entering ShortenUrl(url={Url},max={Max})", url, max);
        if (url.Length <= max)
            return url;

        // Remove the protocol
        var startIndex = url.IndexOf("://", StringComparison.InvariantCultureIgnoreCase);
		_logger.LogDebug("ShortenUrl(url={Url},max={Max}): startIndex → {StartIndex}", url, max, startIndex);
        if (startIndex > -1)
            url = url[(startIndex + 3)..];
			_logger.LogDebug("ShortenUrl(url={Url},max={Max}): url → {Url1}", url, max, url);

        if (url.Length <= max)
            return url;

        // Compress folder structure
        var firstIndex = url.IndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1;
		_logger.LogDebug("ShortenUrl(url={Url},max={Max}): firstIndex → {FirstIndex}", url, max, firstIndex);
        var lastIndex = url.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase);
		_logger.LogDebug("ShortenUrl(url={Url},max={Max}): lastIndex → {LastIndex}", url, max, lastIndex);
        if (firstIndex < lastIndex)
        {
            url = url.Remove(firstIndex, lastIndex - firstIndex);
            url = url.Insert(firstIndex, "...");
        }

        if (url.Length <= max)
            return url;

        // Remove URL parameters
        var queryIndex = url.IndexOf("?", StringComparison.InvariantCultureIgnoreCase);
		_logger.LogDebug("ShortenUrl(url={Url},max={Max}): queryIndex → {QueryIndex}", url, max, queryIndex);
        if (queryIndex > -1)
            url = url[0..queryIndex];

        if (url.Length <= max)
            return url;

        // Remove URL fragment
        var fragmentIndex = url.IndexOf("#", StringComparison.InvariantCultureIgnoreCase);
		_logger.LogDebug("ShortenUrl(url={Url},max={Max}): fragmentIndex → {FragmentIndex}", url, max, fragmentIndex);
        if (fragmentIndex > -1)
            url = url[0..fragmentIndex];

        if (url.Length <= max)
            return url;

        // Compress page
        firstIndex = url.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1;
        lastIndex = url.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);

        if (lastIndex - firstIndex <= 10)
            return url;

        var page = url[firstIndex..lastIndex];
        var length = url.Length - max + 3;
        if (page.Length > length)
            url = url.Replace(page, "..." + page[length..]);

		_logger.LogDebug("<<< Exiting ShortenUrl(url={Url},max={Max})", url, max);
        return url;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats the text
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    public static string FormatText(string text)
    {
		_logger.LogDebug(">>> Entering FormatText(text={Text})", text);
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var info = CultureInfo.InvariantCulture;
        foreach (Match match in _regex.Matches(text))
        {
            text = text.Replace(match.Value,
                !match.Value.Contains("://")
                    ? string.Format(info, LINK, "http://", match.Value, ShortenUrl(match.Value, MAX_LENGTH))
                    : string.Format(info, LINK, string.Empty, match.Value, ShortenUrl(match.Value, MAX_LENGTH)));
        }

		_logger.LogDebug("<<< Exiting FormatText(text={Text})", text);
        return text;
    }

    #endregion
}