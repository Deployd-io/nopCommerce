using Nop.Services.Cms;
using Nop.Services.Plugins;

namespace Nop.Tests.Nop.Web.Tests.Public;

public class TestWidgetPlugin : BasePlugin, IWidgetPlugin
{
    public bool HideInWidgetList { get; } = false;
    public Task<IList<string>> GetWidgetZonesAsync()
    {
		logger.LogDebug(">>> Entering GetWidgetZonesAsync()");
		logger.LogDebug("<<< Exiting GetWidgetZonesAsync()");
        return Task.FromResult<IList<string>>(new List<string> { "test widget zone" });
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
		logger.LogDebug(">>> Entering GetWidgetViewComponent(widgetZone={WidgetZone})", widgetZone);
		logger.LogDebug("<<< Exiting GetWidgetViewComponent(widgetZone={WidgetZone})", widgetZone);
        return typeof(TestWidgetPlugin);
    }
}
