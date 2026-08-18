using Microsoft.Extensions.Logging;
﻿using Microsoft.AspNetCore.Html;

namespace Nop.Web.Framework.Localization;

/// <summary>
/// Localized string
/// </summary>
public partial class LocalizedString : HtmlString
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="localized">Localized value</param>
    public LocalizedString(string localized) : base(localized)
    {
		_logger.LogDebug(">>> Entering LocalizedString(localized={Localized})", localized);
        Text = localized;
		_logger.LogDebug("LocalizedString(localized={Localized}): Text → {Text}", localized, Text);
		_logger.LogDebug("<<< Exiting LocalizedString(localized={Localized})", localized);
    }

    /// <summary>
    /// Text
    /// </summary>
    public string Text { get; }
}