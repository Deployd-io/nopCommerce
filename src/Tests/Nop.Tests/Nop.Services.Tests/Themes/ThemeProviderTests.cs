using FluentAssertions;
using Nop.Services.Themes;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Themes;

[TestFixture]
public class ThemeProviderTests : BaseNopTest
{
    private IThemeProvider _themeProvider;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
		logger.LogDebug(">>> Entering OneTimeSetUpAsync()");
        _themeProvider = GetService<IThemeProvider>();
		logger.LogDebug("<<< Exiting OneTimeSetUpAsync()");
		logger.LogDebug("OneTimeSetUpAsync(): _themeProvider → {_themeProvider}", _themeProvider);
    }

    [Test]
    public void CanGetThemeDescriptorFromText()
    {
		logger.LogDebug(">>> Entering CanGetThemeDescriptorFromText()");
        var descriptor = "{ \"SystemName\": \"Test system name\", \"FriendlyName\": \"Test\", \"SupportRTL\": false, \"PreviewImageUrl\": \"~/Themes/Test/preview.jpg\", \"PreviewText\": \"The 'Test' site theme\" }";

		logger.LogDebug("CanGetThemeDescriptorFromText(): descriptor → {Descriptor}", descriptor);
        var themeDescriptor = _themeProvider.GetThemeDescriptorFromText(descriptor);

		logger.LogDebug("CanGetThemeDescriptorFromText(): themeDescriptor → {ThemeDescriptor}", themeDescriptor);
        themeDescriptor.Should().NotBeNull();
        themeDescriptor.SystemName.Should().BeEquivalentTo("Test system name");
		logger.LogDebug("<<< Exiting CanGetThemeDescriptorFromText()");
    }

    [Test]
    public async Task CanGetThemes()
    {
		logger.LogDebug(">>> Entering CanGetThemes()");
        var themes = await _themeProvider.GetThemesAsync();

		logger.LogDebug("CanGetThemes(): themes → {Themes}", themes);
        themes.Count.Should().BeGreaterThan(0);
		logger.LogDebug("<<< Exiting CanGetThemes()");
    }


    [Test]
    public async Task CanGetThemeBySystemName()
    {
		logger.LogDebug(">>> Entering CanGetThemeBySystemName()");
        var themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync("Test system name");
        themeDescriptor.Should().BeNull();
		logger.LogDebug("CanGetThemeBySystemName(): themeDescriptor → {ThemeDescriptor}", themeDescriptor);
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync(string.Empty);
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync(null);
        themeDescriptor.Should().BeNull();
        themeDescriptor = await _themeProvider.GetThemeBySystemNameAsync("DefaultClean");
        themeDescriptor.Should().NotBeNull();
        themeDescriptor.FriendlyName.Should().BeEquivalentTo("Default clean");
		logger.LogDebug("<<< Exiting CanGetThemeBySystemName()");
    }


    [Test]
    public async Task CanThemeExists()
    {
		logger.LogDebug(">>> Entering CanThemeExists()");
        var isExists = await _themeProvider.ThemeExistsAsync("Test system name");
        isExists.Should().BeFalse();
		logger.LogDebug("CanThemeExists(): isExists → {IsExists}", isExists);
        isExists = await _themeProvider.ThemeExistsAsync(string.Empty);
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync(null);
        isExists.Should().BeFalse();
        isExists = await _themeProvider.ThemeExistsAsync("DefaultClean");
        isExists.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanThemeExists()");
    }
}
