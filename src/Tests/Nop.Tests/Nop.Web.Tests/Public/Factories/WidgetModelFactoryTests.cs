using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Cms;
using Nop.Services.Configuration;
using Nop.Web.Framework.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class WidgetModelFactoryTests : WebTest
{
    private ISettingService _settingsService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _settingsService = GetService<ISettingService>();

		logger.LogDebug("SetUp(): _settingsService → {_settingsService}", _settingsService);
        var widgetSettings = GetService<WidgetSettings>();

		logger.LogDebug("SetUp(): widgetSettings → {WidgetSettings}", widgetSettings);
        widgetSettings.ActiveWidgetSystemNames.Add("TestWidgetPlugin");

        await _settingsService.SaveSettingAsync(widgetSettings);
		logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        var widgetSettings = GetService<WidgetSettings>();

		logger.LogDebug("TearDown(): widgetSettings → {WidgetSettings}", widgetSettings);
        widgetSettings.ActiveWidgetSystemNames.Remove("TestWidgetPlugin");

        await _settingsService.SaveSettingAsync(widgetSettings);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanPrepareRenderWidgetModel()
    {
		logger.LogDebug(">>> Entering CanPrepareRenderWidgetModel()");
        var models = await GetService<IWidgetModelFactory>().PrepareRenderWidgetModelAsync("test widget zone");

		logger.LogDebug("CanPrepareRenderWidgetModel(): models → {Models}", models);
        models.Any().Should().BeTrue();

        var model = models[0];

        var args = model.WidgetViewComponentArguments as RouteValueDictionary;
        args.Should().NotBeNull();
        args.Count.Should().Be(2);
        args["widgetZone"].Should().Be("test widget zone");
        model.WidgetViewComponent.Should().Be(typeof(TestWidgetPlugin));
		logger.LogDebug("<<< Exiting CanPrepareRenderWidgetModel()");
    }
}