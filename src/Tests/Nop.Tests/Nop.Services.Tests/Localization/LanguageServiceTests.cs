using FluentAssertions;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Localization;

[TestFixture]
public class LanguageServiceTests : ServiceTest
{
    private ILanguageService _languageService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _languageService = GetService<ILanguageService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _languageService → {_languageService}", _languageService);
    }

    [Test]
    public async Task CanGetAllLanguages()
    {
		logger.LogDebug(">>> Entering CanGetAllLanguages()");
        var languages = await _languageService.GetAllLanguagesAsync();
        languages.Should().NotBeNull();
		logger.LogDebug("CanGetAllLanguages(): languages → {Languages}", languages);
        languages.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanGetAllLanguages()");
    }
}