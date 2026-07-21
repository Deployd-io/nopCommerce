using Microsoft.AspNetCore.Html;

namespace Nop.Web.Framework.Localization;

/// <summary>
/// Localized string
/// </summary>
public partial class LocalizedString : HtmlString
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="localized">Localized value</param>
    public LocalizedString(string localized) : base(localized)
    {
		logger.LogDebug(">>> Entering LocalizedString(localized={Localized})", localized);
        Text = localized;
		logger.LogDebug("<<< Exiting LocalizedString(localized={Localized})", localized);
		logger.LogDebug("LocalizedString(localized={Localized}): Text → {Text}", localized, Text);
    }

    /// <summary>
    /// Text
    /// </summary>
    public string Text { get; }
}
