using Nop.Services.Cms;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public;

public class TestWidgetPlugin : BasePlugin, IWidgetPlugin
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public bool HideInWidgetList { get; } = false;
    public Task<IList<string>> GetWidgetZonesAsync()
    {
		_logger.LogDebug(">>> Entering GetWidgetZonesAsync()");
		_logger.LogDebug("<<< Exiting GetWidgetZonesAsync()");
        return Task.FromResult<IList<string>>(new List<string> { "test widget zone" });
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
		_logger.LogDebug(">>> Entering GetWidgetViewComponent(widgetZone={WidgetZone})", widgetZone);
		_logger.LogDebug("<<< Exiting GetWidgetViewComponent(widgetZone={WidgetZone})", widgetZone);
        return typeof(TestWidgetPlugin);
    }
}