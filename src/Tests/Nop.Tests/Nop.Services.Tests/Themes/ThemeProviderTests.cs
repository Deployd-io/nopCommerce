using FluentAssertions;
using Nop.Services.Themes;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Themes;

[TestFixture]
public class ThemeProviderTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IThemeProvider _themeProvider;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
		_logger.LogDebug(">>> Entering OneTimeSetUpAsync()");
        _themeProvider = GetService<IThemeProvider>();
		_logger.LogDebug("<<< Exiting OneTimeSetUpAsync()");
		_logger.LogDebug("OneTimeSetUpAsync(): _themeProvider → {_themeProvider}", _themeProvider);
    }

    [Test]
    public void CanGetThemeDescriptorFromText()
    {
		_logger.LogDebug(">>> Entering CanGetThemeDescriptorFromText()");
        var descriptor = "{ \"SystemName\": \"Test system name\", \"FriendlyName\": \"Test\", \"SupportRTL\": false, \"PreviewImageUrl\": \"~/Themes/Test/preview.jpg\", \"PreviewText\": \"The 'Test' site theme\" }";

		_logger.LogDebug("CanGetThemeDescriptorFromText(): descriptor → {Descriptor}", descriptor);
        var themeDescriptor = _themeProvider.GetThemeDescriptorFromText(descriptor);

		_logger.LogDebug("CanGetThemeDescriptorFromText(): themeDescriptor → {ThemeDescriptor}", themeDescriptor);
        themeDescriptor.Should().NotBeNull();
        themeDescriptor.SystemName.Should().BeEquivalentTo("Test system name");
		_logger.LogDebug("<<< Exiting CanGetThemeDescriptorFromText()");
    }

    [Test]
    public async Task CanGetThemes()
    {
		_logger.LogDebug(">>> Entering CanGetThemes()");
        var themes = await _themeProvider.GetThemesAsync();

		_logger.LogDebug("CanGetThemes(): themes → {Themes}", themes);
        themes.Count.Should().BeGreaterThan(0);
		_logger.LogDebug("<<< Exiting CanGetThemes()");
    }


    [Test]
    public async Task CanGetThemeBySystemName()
    {
		_logger.LogDebug(">>> Entering CanGetThemeBySystemName()");
        var themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync("Test system name");
        themeDescriptor.Should().BeNull();
		_logger.LogDebug("CanGetThemeBySystemName(): themeDescriptor → {ThemeDescriptor}", themeDescriptor);
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync(string.Empty);
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync(null);
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync("DefaultClean");
        themeDescriptor.Should().NotBeNull();
        themeDescriptor.FriendlyName.Should().BeEquivalentTo("Default clean");
		_logger.LogDebug("<<< Exiting CanGetThemeBySystemName()");
    }


    [Test]
    public async Task CanThemeExists()
    {
		_logger.LogDebug(">>> Entering CanThemeExists()");
        var isExists = await _themeProvider.ThemeExistsAsync("Test system name");
        isExists.Should().BeFalse();
		_logger.LogDebug("CanThemeExists(): isExists → {IsExists}", isExists);
        isExists = await _themeProvider.ThemeExistsAsync(string.Empty);
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync(null);
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync("DefaultClean");
        isExists.Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanThemeExists()");
    }
}