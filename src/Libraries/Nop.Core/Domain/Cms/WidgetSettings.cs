using Nop.Core.Configuration;

namespace Nop.Core.Domain.Cms;

/// <summary>
/// Widget settings
/// </summary>
public partial class WidgetSettings : ISettings
{
    public WidgetSettings()
    {
		logger.LogDebug(">>> Entering WidgetSettings()");
        ActiveWidgetSystemNames = new List<string>();
		logger.LogDebug("<<< Exiting WidgetSettings()");
		logger.LogDebug("WidgetSettings(): ActiveWidgetSystemNames → {ActiveWidgetSystemNames}", ActiveWidgetSystemNames);
    }

    /// <summary>
    /// Gets or sets a system names of active widgets
    /// </summary>
    public List<string> ActiveWidgetSystemNames { get; set; }
}