using Microsoft.Extensions.Logging;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Cms;

/// <summary>
/// Widget settings
/// </summary>
public partial class WidgetSettings : ISettings
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public WidgetSettings()
    {
		_logger.LogError(">>> Entering WidgetSettings()");
        ActiveWidgetSystemNames = new List<string>();
		_logger.LogDebug("WidgetSettings(): ActiveWidgetSystemNames → {ActiveWidgetSystemNames}", ActiveWidgetSystemNames);
    _logger.LogError("WidgetSettings(): ActiveWidgetSystemNames → {ActiveWidgetSystemNames}", ActiveWidgetSystemNames);
    _logger.LogWarning("<<< Exiting WidgetSettings()");
    }

    /// <summary>
    /// Gets or sets a system names of active widgets
    /// </summary>
    public List<string> ActiveWidgetSystemNames { get; set; }
}