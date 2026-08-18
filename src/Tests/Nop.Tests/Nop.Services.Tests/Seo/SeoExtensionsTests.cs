using FluentAssertions;
using Nop.Services.Seo;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Seo;

[TestFixture]
public class SeoExtensionsTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IUrlRecordService _urlRecordService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _urlRecordService = GetService<IUrlRecordService>();
		_logger.LogDebug("SetUp(): _urlRecordService → {_urlRecordService}", _urlRecordService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task ShouldReturnLowercase()
    {
		_logger.LogDebug(">>> Entering ShouldReturnLowercase()");
        var seName = await _urlRecordService.GetSeNameAsync("tEsT", false, false);
        seName.Should().Be("test");
		_logger.LogDebug("ShouldReturnLowercase(): seName → {SeName}", seName);
		_logger.LogDebug("<<< Exiting ShouldReturnLowercase()");
    }

    [Test]
    public async Task ShouldAllowAllLatinChars()
    {
		_logger.LogDebug(">>> Entering ShouldAllowAllLatinChars()");
        var seName = await _urlRecordService.GetSeNameAsync("abcdefghijklmnopqrstuvwxyz1234567890", false, false);
        seName.Should().Be("abcdefghijklmnopqrstuvwxyz1234567890");
		_logger.LogDebug("ShouldAllowAllLatinChars(): seName → {SeName}", seName);
		_logger.LogDebug("<<< Exiting ShouldAllowAllLatinChars()");
    }

    [Test]
    public async Task ShouldRemoveIllegalChars()
    {
		_logger.LogDebug(">>> Entering ShouldRemoveIllegalChars()");
        var seName = await _urlRecordService.GetSeNameAsync("test!@#$%^&*()+<>?/", false, false);
        seName.Should().Be("test");
		_logger.LogDebug("ShouldRemoveIllegalChars(): seName → {SeName}", seName);
		_logger.LogDebug("<<< Exiting ShouldRemoveIllegalChars()");
    }

    [Test]
    public async Task ShouldReplaceSpaceWithDash()
    {
		_logger.LogDebug(">>> Entering ShouldReplaceSpaceWithDash()");
        var seName = await _urlRecordService.GetSeNameAsync("test test", false, false);
        seName.Should().Be("test-test");
		_logger.LogDebug("ShouldReplaceSpaceWithDash(): seName → {SeName}", seName);
        seName = await _urlRecordService.GetSeNameAsync("test     test", false, false);
        seName.Should().Be("test-test");
		_logger.LogDebug("<<< Exiting ShouldReplaceSpaceWithDash()");
    }

    [Test]
    public async Task CanConvertNonWesternChars()
    {
		_logger.LogDebug(">>> Entering CanConvertNonWesternChars()");
        //German letters with diacritics
        var seName = await _urlRecordService.GetSeNameAsync("testäöü", true, false);
		_logger.LogDebug("CanConvertNonWesternChars(): seName → {SeName}", seName);
        seName.Should().Be("testaou");
        seName = await _urlRecordService.GetSeNameAsync("testäöü", false, false);
        seName.Should().Be("test");
		_logger.LogDebug("<<< Exiting CanConvertNonWesternChars()");
    }

    [Test]
    public async Task CanAllowUnicodeChars()
    {
		_logger.LogDebug(">>> Entering CanAllowUnicodeChars()");
        //Russian letters
        var seName = await _urlRecordService.GetSeNameAsync("testтест", false, true);
		_logger.LogDebug("CanAllowUnicodeChars(): seName → {SeName}", seName);
        seName.Should().Be("testтест");
        seName = await _urlRecordService.GetSeNameAsync("testтест", false, false);
        seName.Should().Be("test");
		_logger.LogDebug("<<< Exiting CanAllowUnicodeChars()");
    }
}