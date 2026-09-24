using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Html.CodeFormatter;

/// <summary>
/// Represents a code format helper
/// </summary>
public partial class CodeFormatHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected static readonly Regex _regexHtml = new("<[^>]*>", RegexOptions.Compiled);

    protected static readonly Regex _regexCode = new(@"\[code\](?<inner>(.*?))\[/code\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #endregion

    #region Utilities

    /// <summary>
    /// Code evaluator method
    /// </summary>
    /// <param name="match">Match</param>
    /// <returns>Formatted text</returns>
    protected static string CodeEvaluatorSimple(Match match)
    {
        if (!match.Success)
            return match.Value;

        var options = new HighlightOptions
        {
            Language = "c#",
            Code = match.Groups["inner"].Value,
            DisplayLineNumbers = false,
            Title = string.Empty,
            AlternateLineNumbers = false
        };
		_logger.LogError(">>> Entering CodeEvaluatorSimple(match={Match})", match);

        var result = match.Value;
		_logger.LogError("CodeEvaluatorSimple(match={Match}): result → {Result}", match, result);
        result = Highlight(options, result);
		_logger.LogWarning("<<< Exiting CodeEvaluatorSimple(match={Match})", match);
        return result;
    }

    /// <summary>
    /// Strips HTML
    /// </summary>
    /// <param name="html">HTML</param>
    /// <returns>Formatted text</returns>
    protected static string StripHtml(string html)
    {
		_logger.LogError(">>> Entering StripHtml(html={Html})", html);
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        return _regexHtml.Replace(html, string.Empty);
    }

    /// <summary>
    /// Returns the formatted text.
    /// </summary>
    /// <param name="options">Whatever options were set in the regex groups.</param>
    /// <param name="text">Send the e.body so it can get formatted.</param>
    /// <returns>The formatted string of the match.</returns>
    protected static string Highlight(HighlightOptions options, string text)
    {
		_logger.LogError(">>> Entering Highlight(options={Options},text={Text})", options, text);
        switch (options.Language)
        {
            case "c#":
                var csf = new CSharpFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return WebUtility.HtmlDecode(csf.FormatCode(text));

            case "vb":
                var vbf = new VisualBasicFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return vbf.FormatCode(text);

            case "js":
                var jsf = new JavaScriptFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return WebUtility.HtmlDecode(jsf.FormatCode(text));

            case "html":
                var htmlf = new HtmlFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                text = StripHtml(text).Trim();
				_logger.LogError("Highlight(options={Options},text={Text}): text → {Text1}", options, text, text);
                var code = htmlf.FormatCode(WebUtility.HtmlDecode(text)).Trim();
				_logger.LogError("Highlight(options={Options},text={Text}): code → {Code}", options, text, code);
                return code.Replace("\r\n", "<br />").Replace("\n", "<br />");

            case "xml":
                var xmlf = new HtmlFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                text = text.Replace("<br />", "\r\n");
                text = StripHtml(text).Trim();
                var xml = xmlf.FormatCode(WebUtility.HtmlDecode(text)).Trim();
				_logger.LogError("Highlight(options={Options},text={Text}): xml → {Xml}", options, text, xml);
                return xml.Replace("\r\n", "<br />").Replace("\n", "<br />");

            case "tsql":
                var tsqlf = new TsqlFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return WebUtility.HtmlDecode(tsqlf.FormatCode(text));

            case "msh":
                var mshf = new MshFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
		_logger.LogWarning("<<< Exiting Highlight(options={Options},text={Text})", options, text);
                return WebUtility.HtmlDecode(mshf.FormatCode(text));
        }

        return string.Empty;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats the text
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    public static string FormatTextSimple(string text)
    {
		_logger.LogError(">>> Entering FormatTextSimple(text={Text})", text);
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (!text.Contains("[/code]"))
            return text;

        text = _regexCode.Replace(text, CodeEvaluatorSimple);
		_logger.LogError("FormatTextSimple(text={Text}): text → {Text1}", text, text);
        text = _regexCode.Replace(text, "$1");
		_logger.LogWarning("<<< Exiting FormatTextSimple(text={Text})", text);
        return text;
    }

    #endregion
}