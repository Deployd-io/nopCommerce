using Microsoft.AspNetCore.Hosting;
using Moq;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;
using Nop.Tests.Nop.Web.Tests.Public;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests;

[TestFixture]
public abstract class WebTest : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected WebTest()
    {
		_logger.LogDebug(">>> Entering WebTest()");
        //init plugins
        InitPlugins();
		_logger.LogDebug("<<< Exiting WebTest()");
    }

    private void InitPlugins()
    {
		_logger.LogDebug(">>> Entering InitPlugins()");
        var webHostEnvironment = new Mock<IWebHostEnvironment>();
        webHostEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
		_logger.LogDebug("InitPlugins(): webHostEnvironment → {WebHostEnvironment}", webHostEnvironment);
        webHostEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
        CommonHelper.DefaultFileProvider = new NopFileProvider(webHostEnvironment.Object);

        Singleton<IPluginsInfo>.Instance = new PluginsInfo(CommonHelper.DefaultFileProvider)
        {
            PluginDescriptors = new List<(PluginDescriptor, bool)>
            {
                (new PluginDescriptor
                {
                    PluginType = typeof(TestWidgetPlugin),
                    SystemName = "TestWidgetPlugin",
                    FriendlyName = "Test widget plugin",
                    Installed = true,
                    ReferencedAssembly = typeof(TestWidgetPlugin).Assembly
                }, true)
            }
        };
		_logger.LogDebug("<<< Exiting InitPlugins()");
    }
}