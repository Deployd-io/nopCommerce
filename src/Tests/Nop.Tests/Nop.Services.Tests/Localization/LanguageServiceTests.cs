using FluentAssertions;
using Nop.Services.Localization;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Localization;

[TestFixture]
public class LanguageServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ILanguageService _languageService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _languageService = GetService<ILanguageService>();
		_logger.LogDebug("SetUp(): _languageService → {_languageService}", _languageService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task CanGetAllLanguages()
    {
		_logger.LogDebug(">>> Entering CanGetAllLanguages()");
        var languages = await _languageService.GetAllLanguagesAsync();
        languages.Should().NotBeNull();
		_logger.LogDebug("CanGetAllLanguages(): languages → {Languages}", languages);
        languages.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanGetAllLanguages()");
    }
}