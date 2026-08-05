using Microsoft.Extensions.Logging;
﻿using Nop.Core.Configuration;

namespace Nop.Core.Domain.Cms;

/// <summary>
/// Widget settings
/// </summary>
public partial class WidgetSettings : ISettings
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public WidgetSettings()
    {
		_logger.LogDebug(">>> Entering WidgetSettings()");
        ActiveWidgetSystemNames = new List<string>();
		_logger.LogDebug("<<< Exiting WidgetSettings()");
		_logger.LogDebug("WidgetSettings(): ActiveWidgetSystemNames → {ActiveWidgetSystemNames}", ActiveWidgetSystemNames);
    }

    /// <summary>
    /// Gets or sets a system names of active widgets
    /// </summary>
    public List<string> ActiveWidgetSystemNames { get; set; }
}