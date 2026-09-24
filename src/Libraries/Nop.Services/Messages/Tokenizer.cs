using System.Linq.Dynamic.Core;
using System.Net;
using System.Text.RegularExpressions;
using Nop.Core.Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Tokenizer
/// </summary>
public partial class Tokenizer : ITokenizer
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly MessageTemplatesSettings _messageTemplatesSettings;

    #endregion

    #region Ctor

    public Tokenizer(MessageTemplatesSettings messageTemplatesSettings)
    {
		_logger.LogError(">>> Entering Tokenizer(messageTemplatesSettings={MessageTemplatesSettings})", messageTemplatesSettings);
        _messageTemplatesSettings = messageTemplatesSettings;
		_logger.LogDebug("Tokenizer(messageTemplatesSettings={MessageTemplatesSettings}): _messageTemplatesSettings → {_messageTemplatesSettings}", messageTemplatesSettings, _messageTemplatesSettings);
    _logger.LogWarning("<<< Exiting Tokenizer(messageTemplatesSettings={MessageTemplatesSettings})", messageTemplatesSettings);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string
    /// </summary>
    /// <param name="original">Original string</param>
    /// <param name="pattern">The string to be replaced</param>
    /// <param name="replacement">The string to replace all occurrences of pattern string</param>
    /// <returns>A string that is equivalent to the current string except that all instances of pattern are replaced with replacement string</returns>
    protected string Replace(string original, string pattern, string replacement)
    {
		_logger.LogError(">>> Entering Replace(original={Original},pattern={Pattern},replacement={Replacement})", original, pattern, replacement);
        //for case sensitive comparison use base string.Replace() method
        var stringComparison = _messageTemplatesSettings.CaseInvariantReplacement ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (stringComparison == StringComparison.Ordinal)
            return original.Replace(pattern, replacement);

        //or do some routine work here
        var count = 0;
		_logger.LogError("Replace(original={Original},pattern={Pattern},replacement={Replacement}): count → {Count}", original, pattern, replacement, count);
        var position0 = 0;
		_logger.LogError("Replace(original={Original},pattern={Pattern},replacement={Replacement}): position0 → {Position0}", original, pattern, replacement, position0);
        int position1;

        var inc = original.Length / pattern.Length * (replacement.Length - pattern.Length);
		_logger.LogError("Replace(original={Original},pattern={Pattern},replacement={Replacement}): inc → {Inc}", original, pattern, replacement, inc);
        var chars = new char[original.Length + Math.Max(0, inc)];
		_logger.LogError("Replace(original={Original},pattern={Pattern},replacement={Replacement}): chars → {Chars}", original, pattern, replacement, chars);
        while ((position1 = original.IndexOf(pattern, position0, stringComparison)) != -1)
        {
            for (var i = position0; i < position1; ++i)
                chars[count++] = original[i];
            for (var i = 0; i < replacement.Length; ++i)
                chars[count++] = replacement[i];
            position0 = position1 + pattern.Length;
        }

        if (position0 == 0)
            return original;

        for (var i = position0; i < original.Length; ++i)
            chars[count++] = original[i];

		_logger.LogWarning("<<< Exiting Replace(original={Original},pattern={Pattern},replacement={Replacement})", original, pattern, replacement);
		_logger.LogWarning("<<< Exiting Replace(template={Template},tokens=<redacted>,htmlEncode={HtmlEncode})", template, htmlEncode);
        return new string(chars, 0, count);
    }

    /// <summary>
    /// Replace tokens
    /// </summary>
    /// <param name="template">The template with token keys inside</param>
    /// <param name="tokens">The sequence of tokens to use</param>
    /// <param name="htmlEncode">The value indicating whether tokens should be HTML encoded</param>
    /// <param name="stringWithQuotes">The value indicating whether string token values should be wrapped in quotes</param>
    /// <returns>Text with all token keys replaces by token value</returns>
    protected string ReplaceTokens(string template, IEnumerable<Token> tokens, bool htmlEncode = false, bool stringWithQuotes = false)
    {
		_logger.LogError(">>> Entering ReplaceTokens(template={Template},tokens=<redacted>,htmlEncode={HtmlEncode},stringWithQuotes={StringWithQuotes})", template, htmlEncode, stringWithQuotes);
        foreach (var token in tokens)
        {
            var tokenValue = token.Value ?? string.Empty;
			_logger.LogError("ReplaceTokens(template={Template},tokens=<redacted>,htmlEncode={HtmlEncode},stringWithQuotes={StringWithQuotes}): tokenValue → {TokenValue}", template, htmlEncode, stringWithQuotes, tokenValue);

            //wrap the value in quotes
            if (stringWithQuotes && tokenValue is string)
                tokenValue = $"\"{tokenValue}\"";
            else
            {
                //do not encode URLs
                if (htmlEncode && !token.NeverHtmlEncoded)
                    tokenValue = WebUtility.HtmlEncode(tokenValue.ToString());
            }

            template = Replace(template, $@"%{token.Key}%", tokenValue.ToString());
			_logger.LogError("ReplaceTokens(template={Template},tokens=<redacted>,htmlEncode={HtmlEncode},stringWithQuotes={StringWithQuotes}): template → {Template1}", template, htmlEncode, stringWithQuotes, template);
        }

		_logger.LogWarning("<<< Exiting ReplaceTokens(template={Template},tokens=<redacted>,htmlEncode={HtmlEncode},stringWithQuotes={StringWithQuotes})", template, htmlEncode, stringWithQuotes);
        return template;
    }

    /// <summary>
    /// Resolve conditional statements and replace them with appropriate values
    /// </summary>
    /// <param name="template">The template with token keys inside</param>
    /// <param name="tokens">The sequence of tokens to use</param>
    /// <returns>Text with all conditional statements replaces by appropriate values</returns>
    protected string ReplaceConditionalStatements(string template, IEnumerable<Token> tokens)
    {
		_logger.LogError(">>> Entering ReplaceConditionalStatements(template={Template},tokens=<redacted>)", template);
        //define regex rules
        var regexFullConditionalSatement = new Regex(@"(?:(?'Group' %if)|(?'Condition-Group' endif%)|(?! (%if|endif%)).)*(?(Group)(?!))",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        var regexCondition = new Regex(@"\s*\((?:(?'Group' \()|(?'-Group' \))|[^()])*(?(Group)(?!))\)\s*",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //find conditional statements in the original template
        var conditionalStatements = regexFullConditionalSatement.Matches(template)
            .SelectMany(match => match.Groups["Condition"].Captures.Select(capture => new
            {
                capture.Index,
                FullStatement = capture.Value,
                Condition = regexCondition.Match(capture.Value).Value
            })).ToList();
			_logger.LogDebug("ReplaceConditionalStatements(template={Template},tokens=<redacted>): Condition → {Condition}", template, Condition);

        if (!conditionalStatements.Any())
            return template;

        //replace conditional statements
        foreach (var statement in conditionalStatements.OrderBy(statement => statement.Index))
        {
            var conditionIsMet = false;
			_logger.LogError("ReplaceConditionalStatements(template={Template},tokens=<redacted>): conditionIsMet → {ConditionIsMet}", template, conditionIsMet);
            if (!string.IsNullOrEmpty(statement.Condition))
            {
                try
                {
                    //replace tokens (string values are wrap in quotes)
                    var conditionString = ReplaceTokens(statement.Condition, tokens, stringWithQuotes: true);
					_logger.LogError("ReplaceConditionalStatements(template={Template},tokens=<redacted>): conditionString → {ConditionString}", template, conditionString);
                    conditionIsMet = new[] { statement }.AsQueryable().Where(conditionString).Any();
                }
                catch
                {
					_logger.LogError("Exception in ReplaceConditionalStatements(template={Template},tokens=<redacted>): {Arg2}", template, ex.Message);
                    // ignored
                }
            }

            template = template.Replace(conditionIsMet ? statement.Condition : statement.FullStatement, string.Empty);
			_logger.LogError("ReplaceConditionalStatements(template={Template},tokens=<redacted>): template → {Template1}", template, template);
        }

        template = template.Replace("%if", string.Empty).Replace("endif%", string.Empty);

        //return template with resolved conditional statements
		_logger.LogWarning("<<< Exiting ReplaceConditionalStatements(template={Template},tokens=<redacted>)", template);
        return template;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Replace all of the token key occurrences inside the specified template text with corresponded token values
    /// </summary>
    /// <param name="template">The template with token keys inside</param>
    /// <param name="tokens">The sequence of tokens to use</param>
    /// <param name="htmlEncode">The value indicating whether tokens should be HTML encoded</param>
    /// <returns>Text with all token keys replaces by token value</returns>
    public string Replace(string template, IEnumerable<Token> tokens, bool htmlEncode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(template);

        ArgumentNullException.ThrowIfNull(tokens);

        //replace conditional statements
        template = ReplaceConditionalStatements(template, tokens);

        //replace tokens
        template = ReplaceTokens(template, tokens, htmlEncode);

        return template;
    }

    #endregion
}